using DataTools.Extras.Conversion;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

using static DataTools.Text.TextTools;

namespace DataTools.Extras
{

    [Flags]
    public enum PartType
    {
        Simple = 0,
        Composite = 0x1,
        Literal = 0x2,
        Operator = 0x4,
        Sign = 0x8,
        Variable = 0x10,
        Unit = 0x20,
        CompositeUnit = 0x21,
        Equality = 0x44,
        Assignment = 0x84
    }

    public enum Position
    {
        Expression,
        LeftHand,
        RightHand,
    }

    public class ExpressionSegment : ICloneable
    {
        public static readonly string[] Operations = new[] { "log", "log10", "sin", "cos", "tan", "asin", "acos", "atan", "^", "exp", "abs", "sqrt", "*", "/", @"\", "mod", "%", "+", "-" };

    protected PartType partType = PartType.Simple;
        protected List<ExpressionSegment> parts = new List<ExpressionSegment>();
        protected ExpressionSegment parent = null;
        protected string monoVal = null;
        protected CultureInfo ci = CultureInfo.CurrentCulture;
        protected MetricUnit unit = null;
        protected string varSym = "$";
        protected object value = null;
        protected Position position = Position.Expression;

        public Position Position
        {
            get
            {
                if (parent != null && parent.Position != Position.Expression)
                {
                    return parent.Position;
                }

                return position;
            }
        }

        public object Value
        {
            get => value ?? monoVal;
            set
            {
                this.value = value;
                if (value != null)
                {
                    monoVal = value.ToString();
                }
            }
        }

        public string ValueAsString()
        {
            return monoVal ?? value?.ToString();
        }

        public double ValueAsDouble()
        {
            if (value is double d)
            {
                return d;
            }
            else
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// The expression-wide variable symbol.
        /// </summary>
        /// <remarks>
        /// On the root node can dictate the variable symbol.
        /// </remarks>
        public virtual string VariableSymbol
        {
            get => Parent?.VariableSymbol ?? varSym;
            set
            {
                if (Parent != null)
                {
                    Parent.VariableSymbol = value;
                }
                else
                {
                    varSym = value;
                }
            }
        }

        public virtual MetricUnit Unit => unit;

        public virtual IReadOnlyList<ExpressionSegment> SubParts => parts.ToArray();

        public virtual ExpressionSegment Parent => parent;

        public virtual PartType PartType => partType;

        public virtual bool HasChildren
        {
            get
            {
                return SubParts.Count > 0;
            }
        }

        public bool Solvable
        {
            get
            {
                if (parts.Count >= 3)
                {
                    return CheckUnits();
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual CultureInfo CultureInfo => ci;

        public ExpressionSegment GetSide(Position side)
        {
            if (side == Position.Expression) return this.Clone();

            var res = parts.Where((p) => p.Position == side).ToList();
            if (res.Count == 1) return res[0].Clone();

            var es = new ExpressionSegment();

            es.position = Position.Expression;
            es.partType = PartType.Composite;

            foreach (var item in res)
            {
                var newItem = item.Clone();
                newItem.parent = es;
                es.parts.Add(newItem);
            }

            return es;
        }

        public bool UnitsMatch(ExpressionSegment other)
        {
            var lh = this;
            var rh = other;

            if (lh.parts.Count > 0)
            {
                var ucmp1 = lh.parts.Where((p) => (p.PartType & PartType.Unit) == PartType.Unit).ToList();
                var ucmp2 = lh.parts.Where((p) => (p.PartType & PartType.Unit) == PartType.Unit).ToList();

                if (ucmp1.Count != ucmp2.Count) return false;

                int c = ucmp1.Count;
                for (int i = 0; i < c; i++)
                {
                    if (ucmp1[i].partType != ucmp2[i].partType) return false;

                    if ((ucmp1[i].partType & PartType.Composite) == PartType.Composite)
                    {
                        if (!ucmp1[i].UnitsMatch(ucmp1[i])) return false;   
                    }
                }


                return true;
            }
            else if ((lh.PartType & PartType.Unit) == PartType.Unit)
            {
                return lh.Unit.Measures == rh.Unit.Measures;
            }
            else
            {
                return false;
            }

        }

        public bool HasMatchingUnits()
        {
            var lh = GetSide(Position.LeftHand);
            var rh = GetSide(Position.RightHand);

            return lh.UnitsMatch(rh);

        }

        protected bool CheckUnits()
        {
            var i = parts.Count((e) => e.partType == PartType.Assignment || e.partType == PartType.Equality);
            if (i == 1)
            {
                return HasMatchingUnits();
            }
            else
            {
                return false;
            }
        }


        public ExpressionSegment(string value) : this(value, null, null)
        {
        }
        public ExpressionSegment(string value, CultureInfo ci) : this(value, null, ci)
        {
        }

        private ExpressionSegment()
        {
        }


        protected ExpressionSegment(string value, ExpressionSegment parent, CultureInfo ci)
        {
            this.ci = ci ?? this.ci ?? CultureInfo.CurrentCulture;
            this.parent = parent;

            value = SpaceOperators(OneSpace(value));
            
            int? b, e;
            int i, c = value.Length;

            char sc1 = '(', sc2 = ')';
            var sb = new StringBuilder();
            
            for (i = 0; i < c; )
            {
                if (value[i] == sc1)
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(new ExpressionSegment(sb.ToString(), this, ci));
                        sb.Clear();
                    }

                    var val = TextBetween(value, i, sc1, sc2, out b, out e);
                    parts.Add(new ExpressionSegment(val, this, ci)
                    {
                        partType = PartType.Composite
                    });

                    if (b is int && e is int n)
                    {
                        i = n + 1;
                        continue;
                    }
                    else
                    {
                        throw new SyntaxErrorException($"Open parenthesis missing closing parenthesis at position {i}");
                    }
                }
                else if (value[i] == ' ')
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(new ExpressionSegment(sb.ToString(), this, ci));
                        sb.Clear();
                    }
                }
                else
                {
                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out double doubleTest))
                    {
                        if ((sb.ToString() + value[i] != "0x") && !double.TryParse(sb.ToString() + value[i], NumberStyles.Any, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci)
                            {
                                partType = PartType.Literal,
                                value = doubleTest
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out long longTest))
                    {
                        if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && !long.TryParse(sb.ToString().Substring(2) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci)
                            {
                                partType = PartType.Literal,
                                value = longTest
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && !long.TryParse(sb.ToString().Substring(2) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci)
                            {
                                partType = PartType.Literal,
                                value = longTest
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && !long.TryParse(sb.ToString().Substring(1) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci)
                            {
                                partType = PartType.Literal,
                                value = longTest
                            });
                            sb.Clear();
                        }
                    }

                    sb.Append(value[i]);
                }

                i++;
            }

            if (sb.Length > 0)
            {
                if (parts.Count == 0)
                {

                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out double doubleTest))
                    {
                        partType = PartType.Literal;
                        this.value = doubleTest;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out long longTest))
                    {
                        partType = PartType.Literal;
                        this.value = longTest;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        partType = PartType.Literal;
                        this.value = longTest;
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        partType = PartType.Literal;
                        this.value = longTest;
                    }

                    monoVal = sb.ToString();

                }
                else
                {
                    var newPart = new ExpressionSegment(sb.ToString(), this, ci);

                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out double doubleTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.value = doubleTest;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out long longTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.value = longTest;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.value = longTest;
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.value = longTest;
                    }

                    parts.Add(newPart);
                    
                    if (parent == null)
                    {
                        partType = PartType.Composite;
                    }
                }

                if (monoVal != null)
                {
                    if (monoVal == "=")
                    {
                        partType = PartType.Assignment;
                    }
                    else if (monoVal == "=")
                    {
                        partType = PartType.Equality;
                    }
                    else if (Operations.Contains(monoVal))
                    {
                        partType = PartType.Operator;
                    }
                    else if (!string.IsNullOrEmpty(VariableSymbol) && monoVal.StartsWith(VariableSymbol))
                    {
                        partType = PartType.Variable;
                    }
                    else
                    {
                        var idunit = MetricTool.IdentifyUnit(monoVal);
                        if (idunit != null)
                        {
                            partType = PartType.Unit;
                            unit = idunit;
                        }
                    }
                }

                CheckCompoundUnits();
            }
        }

        private void CheckCompoundUnits()
        {

            var sb = new StringBuilder();
            var mode = 0;
            bool ef = false;

            if (partType == PartType.Composite && parts.Count >= 3)
            {
                int c = parts.Count;
                int i;

                ExpressionSegment es;

                int? startIdx = null;
                int? stopIdx = null;

                int j;

                for (j = 0; j < c; )
                {
                    if (!ef && parts[j].partType == PartType.Equality || parts[j].partType == PartType.Assignment)
                    {
                        ef = true;
                    }

                    for (i = j; i < c; i++)
                    {
                        if (mode == 0 && parts[i].partType == PartType.Unit)
                        {
                            if (startIdx == null) startIdx = i;
                            mode = 1;
                        }
                        else if (mode == 1 && parts[i].partType == PartType.Operator)
                        {
                            mode = 2;
                        }
                        else if (mode == 2 && parts[i].partType == PartType.Unit)
                        {
                            mode = 1;
                            stopIdx = i;
                        }
                        else if (mode != 0)
                        {
                            mode = 0;
                            break;
                        }
                    }

                    if (startIdx != null && stopIdx != null)
                    {
                        es = new ExpressionSegment();

                        es.partType = PartType.CompositeUnit;
                        es.parent = this;
                        es.ci = ci;

                        for (i = startIdx.Value; i <= stopIdx.Value; i++)
                        {
                            parts[i].parent = es;
                            es.parts.Add(parts[i]);
                        }

                        parts.RemoveRange(startIdx.Value + 1, stopIdx.Value - startIdx.Value);
                        parts[startIdx.Value] = es;

                        j = startIdx.Value + 1;
                        c = parts.Count;

                        mode = 0;
                        startIdx = stopIdx = null;
                    }
                    else
                    {
                        j++;
                    }
                }

                if (ef)
                {
                    ef = false;
                    for (i = 0; i < c; i++)
                    {
                        if (parts[i].partType == PartType.Equality || parts[i].partType == PartType.Assignment)
                        {
                            ef = true;
                        }
                        else
                        {
                            if (ef)
                            {
                                parts[i].position = Position.RightHand;
                            }
                            else
                            {
                                parts[i].position = Position.LeftHand;
                            }
                        }
                    }

                }
            }



        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!HasChildren)
            {
                sb.Append(monoVal);
            }
            else
            {
                foreach (var item in parts)
                {
                    if (sb.Length > 0) sb.Append(" ");

                    sb.Append(item.ToString());
                }
            }

            if (this.partType == PartType.Composite && this.Parent != null)
            {
                return $"({sb})";
            }
            else
            {
                return sb.ToString();
            }
        }

        /// <summary>
        /// Clones the current expression segment to a new expression.
        /// </summary>
        /// <returns>A new expression segment.</returns>
        /// <remarks>
        /// <see cref="Parent"/> is set to null for all new cloned segment roots.
        /// </remarks>
        public ExpressionSegment Clone()
        {
            var itemNew = (ExpressionSegment)MemberwiseClone();

            itemNew.parts = new List<ExpressionSegment>();
            itemNew.parent = null;

            if (value != null)
            {
                if (value is ICloneable cl)
                {
                    itemNew.value = cl.Clone();
                }
            }

            foreach (var item in parts)
            {
                var addItem = item.Clone();
                addItem.parent = itemNew;
                itemNew.parts.Add(addItem);
            }

            return itemNew;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public static implicit operator string(ExpressionSegment val)
        {
            return val.ToString();  
        }

        public static explicit operator ExpressionSegment(string val)
        {
            return new ExpressionSegment(val);    
        }

    }
}
