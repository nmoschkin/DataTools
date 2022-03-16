using DataTools.Extras.Conversion;
using DataTools.Extras.Converters;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.XPath;

using static DataTools.Text.TextTools;
using static DataTools.MathTools.MathLib;

namespace DataTools.Extras.Expressions
{

    /// <summary>
    /// Flags for the type of <see cref="ExpressionSegment"/>.
    /// </summary>
    [Flags]
    [JsonConverter(typeof(EnumToStringConverter<PartType>))]
    public enum PartType
    {
        /// <summary>
        /// Indicates the expression segment is a simple phrase or other value that is not otherwise explicitly defined.
        /// </summary>
        Simple = 0,

        /// <summary>
        /// Inidicates the expression segment is a composite of two or more subordinate expressions.
        /// </summary>
        Composite = 0x1,

        /// <summary>
        /// Indicates the expression is wrapped in parenthesis.
        /// </summary>
        Parenthesis = 0x201,

        /// <summary>
        /// Indicates the expression segment represents a literal value.
        /// </summary>
        Literal = 0x2,

        /// <summary>
        /// Indicates the expression segment represents an operator.
        /// </summary>
        Operator = 0x4,

        /// <summary>
        /// Indicates the expression segment represents a sign value.
        /// </summary>
        Executive = 0x8,

        /// <summary>
        /// Indicates the expression segment represents a variable. 
        /// </summary>
        /// <remarks>
        /// If the <see cref="ExpressionSegment.VariableSymbol"/> is null or empty, then simple phrases will be interpreted as variables.
        /// </remarks>
        Variable = 0x10,

        /// <summary>
        /// Indicates the expression is a solitary or comma-separated literal or variable value inside of a composite.
        /// </summary>
        Parameter = 0x100,

        /// <summary>
        /// Indicates the expression segment represents a recognized unit of measurement.
        /// </summary>
        Unit = 0x20,

        /// <summary>
        /// Indicates the expression segment represents a composite of two or more recognized units of measurement.
        /// </summary>
        CompositeUnit = 0x21,

        /// <summary>
        /// Indicates the expression segment represents a unit/value pair.
        /// </summary>
        ValueUnitPair = 0x23,

        /// <summary>
        /// Indicates that the expression segment tests for equality, and has exactly one left-hand part and exactly one right-hand part.
        /// </summary>
        Equality = 0x44,

        /// <summary>
        /// Indicates that the expression segment performs an assignment, and has exactly one left-hand part and exactly one right-hand part.
        /// </summary>
        Assignment = 0x84,

        /// <summary>
        /// Indicates that the expression has exactly one left-hand part and exactly one right-hand part.
        /// </summary>
        Equation = 0x45
    }

    /// <summary>
    /// Positional indicator for an <see cref="ExpressionSegment"/>.
    /// </summary>
    [JsonConverter(typeof(EnumToStringConverter<Position>))]
    public enum Position
    {
        /// <summary>
        /// The expression segment represents a whole expression.
        /// </summary>
        Expression,

        /// <summary>
        /// The expression segment is on the left-hand side of the parent <see cref="ExpressionSegment"/>.
        /// </summary>
        LeftHand,

        /// <summary>
        /// The expression segment is on the right-hand side of the parent <see cref="ExpressionSegment"/>.
        /// </summary>
        RightHand,
    }

    /// <summary>
    /// Indicates how literal values are stored in an <see cref="ExpressionSegment"/>.
    /// </summary>
    [JsonConverter(typeof(EnumToStringConverter<StorageMode>))]
    public enum StorageMode
    {
        /// <summary>
        /// Literal values are stored as <see cref="double"/> values.
        /// </summary>
        AsDouble,

        /// <summary>
        /// Literal values are stored as <see cref="decimal"/> values.
        /// </summary>
        AsDecimal
    }

    /// <summary>
    /// An object that represents a mathematical expression segment.
    /// </summary>
    public sealed class ExpressionSegment : ICloneable
    {
        #region Public Fields

        public static readonly string[] Operations = new[] { "floor", "ceil", "min", "max", "log", "log10", "sin", "cos", "tan", "sinh", "cosh", "tanh", "asin", "acos", "atan", "^", "exp", "abs", "sqrt", "root", "*", "/", @"\", "mod", "%", "+", "-" };
        public static readonly string[] OperationOrders = new[] { "floor,ceil,log,log10,sin,cos,tan,sinh,cosh,tanh,asin,acos,atan,abs", "^,exp,sqrt,root", "mod,%", "*,/,\\", "+,-" };
        public static readonly string[] UnitaryOperations = new[] { "max", "min", "floor", "ceil", "log", "log10", "sin", "cos", "tan", "sinh", "cosh", "tanh", "asin", "acos", "atan", "abs", "sqrt", "root" };
        public static readonly string[] ProductOperations = new[] { "^", "exp", "*", "/", @"\", "mod", "%", "+", "-" };

        #endregion Public Fields

        #region Private Fields

        private static Dictionary<string, object> constants = new Dictionary<string, object>();

        private CultureInfo ci = CultureInfo.CurrentCulture;
        private string monoVal = null;
        private ExpressionSegment parent = null;

        private List<ExpressionSegment> parts = new List<ExpressionSegment>();

        private PartType partType = PartType.Simple;
        private Position position = Position.Expression;
        private StorageMode storageMode = StorageMode.AsDouble;

        private MetricUnit unit = null;
        private object value = null;
        private string varSym = "$";
        private string errorText = "";

        private Dictionary<string, object> variables;


        #endregion Private Fields

        #region Static Constructor


        static ExpressionSegment()
        {
            // Add some common mathematical constants.

            constants.Add("pi", Math.PI);
            constants.Add("phi", 1.618033988749894d);
            constants.Add("π", Math.PI);
            constants.Add("φ", 1.618033988749894d);
            constants.Add("rad", 180d / Math.PI);

        }
        #endregion

        #region Public Constructors

        /// <summary>
        /// Instantiate a new <see cref="ExpressionSegment"/> object with the specified string.
        /// </summary>
        /// <param name="value">The expression string to evaluate.</param>
        /// <param name="varSym">The variable symbol (default is "$").</param>
        public ExpressionSegment(string value, string varSym = "$") : this(value, null, null, StorageMode.AsDouble, varSym)
        {
        }

        /// <summary>
        /// Instantiate a new <see cref="ExpressionSegment"/> object with the specified string and cultural context.
        /// </summary>
        /// <param name="value">The expression string to evaluate.</param>
        /// <param name="ci">The <see cref="System.Globalization.CultureInfo"/> context to use.</param>
        public ExpressionSegment(string value, CultureInfo ci, string varSym = "$") : this(value, null, ci, StorageMode.AsDouble, varSym)
        {
        }

        /// <summary>
        /// Instantiate a new <see cref="ExpressionSegment"/> object with the specified string and cultural context.
        /// </summary>
        /// <param name="value">The expression string to evaluate.</param>
        /// <param name="ci">The <see cref="System.Globalization.CultureInfo"/> context to use.</param>
        /// <param name="mode">The storage mode (double or decimal) for literal values.</param>
        public ExpressionSegment(string value, CultureInfo ci, StorageMode mode, string varSym = "$") : this(value, null, ci, mode, varSym)
        {
        }

        /// <summary>
        /// Instantiate a new <see cref="ExpressionSegment"/> object with the specified string and cultural context.
        /// </summary>
        /// <param name="value">The expression string to evaluate.</param>
        /// <param name="mode">The storage mode (double or decimal) for literal values.</param>
        public ExpressionSegment(string value, StorageMode mode, string varSym = "$") : this(value, null, null, mode, varSym)
        {
        }

        #endregion Public Constructors

        #region Private Constructors

        private ExpressionSegment()
        {
        }

        private ExpressionSegment(string value, ExpressionSegment parent, CultureInfo ci, StorageMode mode, string varSym)
        {
            storageMode = mode;
            this.varSym = varSym;
            this.ci = ci ?? this.ci ?? CultureInfo.CurrentCulture;
            this.parent = parent;

            bool hasp = false;

            value = SpaceOperators(OneSpace(value));

            int? b, e;
            int i, c = value.Length;

            char sc1 = '(', sc2 = ')';
            var sb = new StringBuilder();

            for (i = 0; i < c;)
            {
                if (value[i] == sc1)
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(new ExpressionSegment(sb.ToString(), this, ci, mode, varSym));
                        sb.Clear();
                    }

                    var val = TextBetween(value, i, sc1, sc2, out b, out e);

                    var tseg = Split(val, ",", true);
                    
                    if (tseg.Length == 1)
                    {
                        var seg = new ExpressionSegment(val, this, ci, mode, varSym);

                        seg.partType = seg.partType | PartType.Parenthesis;

                        var tp = parts.LastOrDefault();
                        if (tp != null)
                        {
                            if (tp.IsUnitaryOperator)
                            {
                                seg.partType = seg.partType | PartType.Parameter;
                            }
                        }

                        parts.Add(seg);
                    }
                    else
                    {
                        var seg = new ExpressionSegment();
                        
                        seg.partType = PartType.Parameter | PartType.Parenthesis;
                        seg.parent = this;
                        seg.ci = ci;
                        seg.varSym = varSym;
                        seg.position = position;
                        seg.storageMode = StorageMode;

                        parts.Add(seg);

                        foreach (var val2 in tseg)
                        {
                            var seg2 = new ExpressionSegment(val2, seg, ci, mode, varSym);
                            seg2.partType |= PartType.Parameter;
                            seg.parts.Add(seg2);
                        }

                    }

                    if (b is int && e is int n)
                    {
                        i = n + 1;
                        continue;
                    }
                    else
                    {
                        ErrorText = $"Open parenthesis missing closing parenthesis at position {i} of string '{value}'";
                        throw new SyntaxErrorException(ErrorText);
                    }
                }
                else if (value[i] == ',')
                {
                    if (sb.Length > 0)
                    {
                        var part = new ExpressionSegment(sb.ToString(), this, ci, mode, varSym);
                        parts.Add(part);
                        part.partType = part.partType | PartType.Parameter;
                        hasp = true;
                        sb.Clear();
                    }
                }
                else if (value[i] == ' ')
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(new ExpressionSegment(sb.ToString(), this, ci, mode, varSym));
                        sb.Clear();
                    }
                }
                else
                {
                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out double doubleTest))
                    {
                        if (sb.ToString() + value[i] != "0x" && !double.TryParse(sb.ToString() + value[i], NumberStyles.Any, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci, mode, varSym)
                            {
                                partType = PartType.Literal,
                                Value = doubleTest,
                                monoVal = sb.ToString()
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out long longTest))
                    {
                        if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && !long.TryParse(sb.ToString().Substring(2) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci, mode, varSym)
                            {
                                partType = PartType.Literal,
                                Value = longTest,
                                monoVal = sb.ToString()
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && !long.TryParse(sb.ToString().Substring(2) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci, mode, varSym)
                            {
                                partType = PartType.Literal,
                                Value = longTest,
                                monoVal = sb.ToString()
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && !long.TryParse(sb.ToString().Substring(1) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpressionSegment(sb.ToString(), this, ci, mode, varSym)
                            {
                                partType = PartType.Literal,
                                Value = longTest,
                                monoVal = sb.ToString()
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
                        Value = doubleTest;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out long longTest))
                    {
                        partType = PartType.Literal;
                        Value = longTest;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        partType = PartType.Literal;
                        Value = longTest;
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        partType = PartType.Literal;
                        Value = longTest;
                    }

                    monoVal = sb.ToString();

                }
                else
                {
                    var newPart = new ExpressionSegment(sb.ToString(), this, ci, mode, varSym);

                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out double doubleTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.Value = doubleTest;
                        newPart.monoVal = sb.ToString();
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out long longTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.Value = longTest;
                        newPart.monoVal = sb.ToString();
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.Value = longTest;
                        newPart.monoVal = sb.ToString();
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out longTest))
                    {
                        newPart.partType = PartType.Literal;
                        newPart.Value = longTest;
                        newPart.monoVal = sb.ToString();
                    }

                    if (hasp)
                    {
                        newPart.partType = newPart.partType | PartType.Parameter;
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
                        else if (partType != PartType.Literal && string.IsNullOrEmpty(VariableSymbol))
                        {
                            partType = PartType.Variable;
                        }
                    }
                }

            }

            if (parent == null)
            {
                variables = new Dictionary<string, object>();
            }

            FormalizeStructure();
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets variables dynamically.
        /// </summary>
        /// <param name="key">The variable key name.</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (Variables != null && Variables.TryGetValue(key, out var value))
                {
                    return value;
                }

                return null;
            }
            set
            {
                if (Variables == null)
                {
                    Variables = new Dictionary<string, object>();
                }

                if (Variables.ContainsKey(key))
                {
                    Variables[key] = value;
                }
                else
                {
                    Variables.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a list of global constants that are used throughout the system.
        /// </summary>
        public static Dictionary<string, object> Constants
        {
            get => constants;
            set
            {
                constants = value;
            }
        }

        /// <summary>
        /// Gets the components of a composite <see cref="ExpressionSegment"/>.
        /// </summary>
        public IReadOnlyList<ExpressionSegment> Components => parts.ToArray();

        /// <summary>
        /// Gets the current cultural context used for parsing numeric literals.
        /// </summary>
        public CultureInfo CultureInfo => ci;

        public string ErrorText
        {
            get => errorText;
            private set
            {
                errorText = value;
            }
        }

        /// <summary>
        /// Gets a value indicating that this expression is a composite expression.
        /// </summary>
        public bool IsComposite
        {
            get
            {
                return Components.Count > 0;
            }
        }

        public bool IsProductOperator
        {
            get
            {
                return partType == PartType.Operator && ((IList<string>)ProductOperations).Contains(monoVal);
            }
        }

        public bool IsUnitaryOperator
        {
            get
            {
                return partType == PartType.Operator && ((IList<string>)UnitaryOperations).Contains(monoVal);
            }
        }

        public bool IsNumeric
        {
            get
            {
                return ((value != null) && (value.GetType().IsPrimitive || value.GetType().IsEnum));
            }
        }

        public bool IsOperator
        {
            get
            {
                return partType == PartType.Operator;
            }
        }

        /// <summary>
        /// Gets a value indicating that the expression is solvable.
        /// </summary>
        public bool IsSolvable
        {
            get
            {
                if (parts.Count >= 2)
                {
                    return CheckSolvable();
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the parent expression of this <see cref="ExpressionSegment"/>, or null if root.
        /// </summary>
        [JsonIgnore]
        public ExpressionSegment Parent => parent;

        /// <summary>
        /// Gets the <see cref="Expressions.PartType"/> of the <see cref="ExpressionSegment"/>.
        /// </summary>
        public PartType PartType => partType;

        /// <summary>
        /// Gets the <see cref="Expressions.Position"/> of the current <see cref="ExpressionSegment"/> in terms of the parent expression (left-hand, right-hand, or expression).
        /// </summary>
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

        /// <summary>
        /// Gets or sets the value <see cref="Expressions.StorageMode"/> (double or decimal) of the <see cref="ExpressionSegment"/> literals.
        /// </summary>
        /// <remarks>
        /// Only the root node can dictate the storage mode. Setting this value on a child node will bubble up to the root node.
        /// </remarks>
        public StorageMode StorageMode
        {
            get => Parent?.StorageMode ?? storageMode;
            set
            {
                if (value == (Parent?.StorageMode ?? storageMode)) return;

                storageMode = value;

                if (parent != null)
                {
                    parent.StorageMode = value;
                }

                Value = Value;
            }
        }
        /// <summary>
        /// Gets the identified <see cref="MetricUnit"/> information (or null if not a unit)
        /// </summary>
        public MetricUnit Unit => unit;

        /// <summary>
        /// Gets or sets the literal value of this <see cref="ExpressionSegment"/>.
        /// </summary>
        /// <remarks>
        /// Setting this value will overwrite the original string value of this expression segment, as well.
        /// </remarks>
        public object Value
        {
            get => value ?? monoVal;
            set
            {
                object v = value;
                if (v is string s)
                {
                    monoVal = s;
                    this.value = null;
                    return;
                }

                if (storageMode == StorageMode.AsDouble)
                {
                    if (v is int i)
                    {
                        v = (double)i;
                    }
                    else if (v is double d)
                    {
                        v = d;
                    }
                    else if (v is decimal de)
                    {
                        v = (double)de;
                    }
                    else if (v is long l)
                    {
                        v = (double)l;
                    }
                    else if (v is float f)
                    {
                        v = (double)f;
                    }
                    else if (v is uint ui)
                    {
                        v = (double)ui;
                    }
                    else if (v is ulong ul)
                    {
                        v = (double)ul;
                    }
                    else if (v is short si)
                    {
                        v = (double)si;
                    }
                    else if (v is ushort usi)
                    {
                        v = (double)usi;
                    }
                    else if (v is sbyte sb)
                    {
                        v = (double)sb;
                    }
                    else if (v is byte by)
                    {
                        v = (double)by;
                    }
                }
                else
                {
                    if (v is int i)
                    {
                        v = (decimal)i;
                    }
                    else if (v is double d)
                    {
                        v = (decimal)d;
                    }
                    else if (v is decimal de)
                    {
                        v = de;
                    }
                    else if (v is long l)
                    {
                        v = (decimal)l;
                    }
                    else if (v is float f)
                    {
                        v = (decimal)f;
                    }
                    else if (v is uint ui)
                    {
                        v = (decimal)ui;
                    }
                    else if (v is ulong ul)
                    {
                        v = (decimal)ul;
                    }
                    else if (v is short si)
                    {
                        v = (decimal)si;
                    }
                    else if (v is ushort usi)
                    {
                        v = (decimal)usi;
                    }
                    else if (v is sbyte sb)
                    {
                        v = (decimal)sb;
                    }
                    else if (v is byte by)
                    {
                        v = (decimal)by;
                    }

                }

                this.value = v;

                if (v != null)
                {
                    monoVal = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the variables to use for solving equations.
        /// </summary>
        public Dictionary<string, object> Variables
        {
            get => parent?.Variables ?? variables;
            set
            {
                if (parent != null)
                {
                    parent.Variables = value;
                }

                variables = value;
            }
        }

        /// <summary>
        /// Gets or sets the expression-wide variable symbol.
        /// </summary>
        /// <remarks>
        /// Only the root node can dictate the variable symbol. Setting this value on a child node will bubble up to the root node.
        /// </remarks>
        public string VariableSymbol
        {
            get => Parent?.VariableSymbol ?? varSym;
            set
            {
                varSym = value;

                if (Parent != null)
                {
                    Parent.VariableSymbol = value;
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Check that the unit form is compatible with the unit form of the specified object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if the unit forms are compatible.</returns>
        public bool CheckUnitsMatch(ExpressionSegment other)
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
                        if (!ucmp1[i].CheckUnitsMatch(ucmp1[i])) return false;
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

        /// <summary>
        /// Clones the current <see cref="ExpressionSegment"/> to a new expression.
        /// </summary>
        /// <returns>A new <see cref="ExpressionSegment"/>.</returns>
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

        public override bool Equals(object obj)
        {
            if (obj is ExpressionSegment es)
            {
                if (es.partType == partType)
                {
                    if (partType == PartType.Literal)
                    {
                        return Equals(value, es.value);
                    }
                    else if (partType == PartType.Unit)
                    {
                        return es.Unit.Equals(es.Unit);
                    }
                    else
                    {
                        return es.ToString() == ToString();
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (partType == PartType.Literal && value != null)
            {
                return value.Equals(obj);
            }
            else
            {
                return false;
            }
        }

        public double? Execute()
        {
            double? execVal = null;
            double pValA, pValB;

            List<double> pars = null;

            if ((partType & PartType.Executive) != PartType.Executive)
            {
                if ((partType & PartType.Literal) == PartType.Literal)
                {
                    return ValueToDouble();
                }
                else if ((partType & PartType.Variable) == PartType.Variable)
                {
                    if (Variables.TryGetValue(monoVal, out object v))
                    {
                        if (v != null && double.TryParse(v.ToString(), out double dd))
                        {
                            return dd;
                        }
                    }
                }

                return null;
            }

            if (parts.Count == 2)
            {
                if ((parts[1].partType & PartType.Executive) == PartType.Executive)
                {
                    execVal = parts[1].Execute();
                }
                else if (parts[1].partType == PartType.ValueUnitPair)
                {
                    execVal = parts[1].parts.FirstOrDefault()?.ValueToDouble();
                }
                else if ((parts[1].partType & PartType.Literal) == PartType.Literal)
                {
                    execVal = parts[1].ValueToDouble();
                }
                else if ((parts[1].partType & PartType.Variable) == PartType.Variable)
                {
                    var obj = this[parts[1].monoVal];
                    if (obj != null && double.TryParse(obj.ToString(), out double dd))
                    {
                        execVal = dd;
                    }
                }
                else if ((parts[1].partType & PartType.Parameter) == PartType.Parameter)
                {
                    pars = new List<double>();
                    foreach (var examine in parts[1].parts)
                    {
                        pars.Add(examine.Execute() ?? 0);
                    }
                }
                else
                {
                    return null;
                }

                if (execVal == null && pars == null)
                {
                    return null;
                }
                else if (execVal != null)
                {
                    pValA = (double)execVal;
                }
                else
                {
                    pValA = double.NaN;
                }

                switch (parts[0].monoVal)
                {

                    case "min":

                        if (pars != null)
                        {
                            execVal = Min(pars.ToArray());
                        }
                        else if (execVal != null)
                        {
                            break;
                        }
                        else
                        {
                            throw new SyntaxErrorException("Cannot parse parameters for unitary operator");
                        }

                        break;

                    case "max":

                        if (pars != null)
                        {
                            execVal = Max(pars.ToArray());
                        }
                        else if (execVal != null)
                        {
                            break;
                        }
                        else
                        {
                            throw new SyntaxErrorException("Cannot parse parameters for unitary operator");
                        }

                        break;


                    case "floor":
                        execVal = Math.Floor(pValA);
                        break;

                    case "ceil":
                        execVal = Math.Ceiling(pValA);
                        break;

                    case "abs":

                        execVal = Math.Abs(pValA);
                        break;

                    case "sqrt":
                    case "root":

                        execVal = Math.Sqrt(pValA);
                        break;

                    case "log":

                        execVal = Math.Log(pValA);
                        break;

                    case "log10":

                        execVal = Math.Log10(pValA);
                        break;

                    case "sin":

                        execVal = Math.Sin(pValA);
                        break;

                    case "cos":

                        execVal = Math.Cos(pValA);
                        break;

                    case "tan":

                        execVal = Math.Tan(pValA);
                        break;

                    case "sinh":

                        execVal = Math.Sinh(pValA);
                        break;

                    case "cosh":

                        execVal = Math.Cosh(pValA);
                        break;

                    case "tanh":

                        execVal = Math.Tanh(pValA);
                        break;

                    case "asin":

                        execVal = Math.Asin(pValA);
                        break;

                    case "acos":

                        execVal = Math.Acos(pValA);
                        break;

                    case "atan":

                        execVal = Math.Atan(pValA);
                        break;

                }
            }
            else if (parts.Count == 3)
            {

                if ((parts[0].partType & PartType.Executive) == PartType.Executive)
                {
                    execVal = parts[0].Execute();
                }
                else if (parts[0].partType == PartType.ValueUnitPair)
                {
                    execVal = parts[0].parts.FirstOrDefault()?.ValueToDouble();
                }
                else if (parts[0].partType == PartType.Literal)
                {
                    execVal = parts[0].ValueToDouble();
                }
                else if ((parts[0].partType & PartType.Variable) == PartType.Variable)
                {
                    var obj = this[parts[0].monoVal];
                    if (obj != null && double.TryParse(obj.ToString(), out double dd))
                    {
                        execVal = dd;
                    }
                }
                else
                {
                    return null;
                }

                if (execVal == null)
                {
                    return null;
                }
                else
                {
                    pValA = (double)execVal;
                }

                if ((parts[2].partType & PartType.Executive) == PartType.Executive)
                {
                    execVal = parts[2].Execute();
                }
                else if (parts[2].partType == PartType.ValueUnitPair)
                {
                    execVal = parts[2].parts.FirstOrDefault()?.ValueToDouble();
                }
                else if (parts[2].partType == PartType.Literal)
                {
                    execVal = parts[2].ValueToDouble();
                }
                else if ((parts[2].partType & PartType.Variable) == PartType.Variable)
                {
                    var obj = this[parts[2].monoVal];
                    if (obj != null && double.TryParse(obj.ToString(), out double dd))
                    {
                        execVal = dd;
                    }
                }
                else
                {
                    return null;
                }

                if (execVal == null)
                {
                    return null;
                }
                else
                {
                    pValB = (double)execVal;
                }

                switch (parts[1].monoVal)
                {
                    case "exp":
                    case "^":

                        execVal = Math.Pow(pValA, pValB);
                        break;

                    case "*":
                        execVal = pValA * pValB;
                        break;

                    case "/":
                        execVal = pValA / pValB;
                        break;

                    case "\\":
                        execVal = ((int)pValA) / ((int)pValB);
                        break;

                    case "%":
                    case "mod":
                        execVal = pValA % pValB;
                        break;

                    case "+":
                        execVal = pValA + pValB;
                        break;

                    case "-":
                        execVal = pValA - pValB;
                        break;

                }



            }

            return execVal;
        }


        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Gets the expression root for the specified side.
        /// </summary>
        /// <param name="side">The side of the expression.</param>
        /// <returns>An <see cref="ExpressionSegment"/> root.</returns>
        public ExpressionSegment GetSide(Position side)
        {
            if (side == Position.Expression) return Clone();

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

        /// <summary>
        /// Check whether this root has left-hand and right-hand sides and the units match.
        /// </summary>
        /// <returns></returns>
        public bool HasMatchingUnits()
        {
            if (partType != PartType.Equation) return false;

            var lh = GetSide(Position.LeftHand);
            var rh = GetSide(Position.RightHand);

            return lh.CheckUnitsMatch(rh);
        }

        public List<(ExpressionSegment, ExpressionSegment)> GetValueUnitPairs()
        {
            if (parent == null && !IsSolvable) return null;

            List<(ExpressionSegment, ExpressionSegment)> result = new List<(ExpressionSegment, ExpressionSegment)>();

            int i, c = parts.Count;

            for (i = 0; i < c; i++)
            {
                if ((parts[i].partType & PartType.Composite) == PartType.Composite)
                {
                    result.AddRange(parts[i].GetValueUnitPairs());
                }
                else if (i > 0)
                {
                    if (parts[i].partType == PartType.Unit)
                    {
                        result.Add((parts[i - 1], parts[i]));   
                    }
                }
            }

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!IsComposite)
            {
                sb.Append(monoVal);
            }
            else
            {
                bool hasp = false;
                bool unit = false;
                if (parts.Count == 3 && (parts[1].partType & PartType.Operator) == PartType.Operator)
                {
                    if (parts[1].monoVal == "^")
                    {
                        return $"{parts[0]}{parts[1]}{parts[2]}";
                    }
                }
                foreach (var item in parts)
                {
                    
                    if (hasp)
                    {
                        if (sb.Length > 0) sb.Append(", ");

                    }
                    else
                    {
                        if (sb.Length > 0 && !unit) sb.Append(" ");
                    }

                    if ((item.partType & PartType.Parameter) == PartType.Parameter)
                    {
                        hasp = true;
                    }
                    else
                    {
                        if (item.IsUnitaryOperator)
                        {
                            unit = true;
                        }
                        else
                        {
                            unit = false;
                        }

                        hasp = false;
                    }

                    sb.Append(item.ToString());
                }
            }

            if ((partType & PartType.Parenthesis) == PartType.Parenthesis && Parent != null)
            {
                return $"({sb})";
            }
            else
            {
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the literal value as a decimal number.
        /// </summary>
        /// <returns>A decimal number or null if the value cannot be coerced to a decimal.</returns>
        public decimal? ValueToDecimal()
        {
            if (value is double d)
            {
                return (decimal)d;
            }
            else if (value is decimal de)
            {
                return de;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the literal value as a double number.
        /// </summary>
        /// <returns>A double number or null if the value cannot be coerced to a double.</returns>
        public double? ValueToDouble()
        {
            if (value is double d)
            {
                return d;
            }
            else if (value is decimal de)
            {
                return (double)de;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the literal value as a string.
        /// </summary>
        /// <returns></returns>
        public string ValueToString()
        {
            return monoVal ?? value?.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Check the integrity of the expression, formalize the structure, and determine if it is solvable.
        /// </summary>
        private void FormalizeStructure()
        {
            var mode = 0;
            bool ef;

            if (partType == PartType.Composite && parts.Count >= 3)
            {
                int c = parts.Count;
                int i;

                ExpressionSegment es;

                int? startIdx = null;
                int? stopIdx = null;

                int j;
                int eqs = 0;

                bool di;

                for (i = 0; i < c; i++)
                {
                    di = false;

                    if (parts[i].partType == PartType.Unit)
                    {
                        if (i == 0)
                        {
                            di = true;
                        }
                        else
                        {
                            if (parts[i - 1].partType != PartType.Composite && parts[i - 1].partType != PartType.Literal && parts[i - 1].partType != PartType.Variable)
                            {
                                di = true;
                            }
                        }
                    }

                    if (di)
                    {
                        var exp = new ExpressionSegment("1", ci, storageMode, varSym);
                        exp.parent = this;
                        parts.Insert(i, exp);
                        c++;
                        i++;
                    }
                }

                for (j = 0; j < c;)
                {
                    if (parts[j].partType == PartType.Equality || parts[j].partType == PartType.Assignment)
                    {
                        eqs++;
                    }

                    for (i = j; i < c; i++)
                    {
                        if (mode == 0 && (parts[i].partType == PartType.Composite || parts[i].partType == PartType.Literal || parts[i].partType == PartType.Variable))
                        {
                            if (startIdx == null) startIdx = i;
                            mode = 1;
                        }
                        else if (mode == 1 && parts[i].partType == PartType.Unit)
                        {
                            mode = 2;
                        }
                        else if (mode == 2 && parts[i].partType == PartType.Operator)
                        {
                            mode = 3;
                        }
                        else if (mode == 3 && (parts[i].partType == PartType.Composite || parts[i].partType == PartType.Literal || parts[i].partType == PartType.Variable))
                        {
                            mode = 4;
                        }
                        else if (mode == 4 && parts[i].partType == PartType.Unit)
                        {
                            mode = 2;
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
                        es.storageMode = storageMode;

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

                ef = eqs == 1;

                if (ef)
                {
                    ef = false;
                    partType = PartType.Equation;

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

            if (parts.Count == 1 && (partType & PartType.Composite) != 0)
            {
                if (((parts[0].partType & PartType.Variable) != 0) || ((parts[0].partType & PartType.Literal) != 0))
                {
                    parts[0].partType = parts[0].partType | PartType.Parameter;
                }
            }

            if (parts.Count > 1) MakeUnitValuePairs();
            GroupByOrderOps();
        }

        private void MakeUnitValuePairs()
        {
            int i, c = parts.Count;

            for (i = 0; i < c; i++)
            {
                if (parts[i].parts.Count > 1)
                {
                    parts[i].MakeUnitValuePairs();
                }

                if (i < (c - 1))
                {
                    if ((parts[i + 1].partType == PartType.Unit) && (parts[i].partType == PartType.Composite || parts[i].partType == PartType.Literal || parts[i].partType == PartType.Variable))
                    {
                        // Make a pair
                        var es = new ExpressionSegment();

                        es.parent = this;
                        es.partType = PartType.ValueUnitPair;

                        var epart = parts[i];
                        var upart = parts[i + 1];

                        parts.RemoveRange(i, 2);

                        epart.parent = es;
                        upart.parent = es;

                        es.parts.Add(epart);
                        es.parts.Add(upart);

                        parts.Insert(i, es);
                        c -= 1;
                    }

                    
                }
            }

        }

        private void GroupByOrderOps()
        {

            int opcount = parts.Count((p) => p.partType == PartType.Operator);
            ExpressionSegment es;
            IList<string> cops = OperationOrders;

            int i, c = cops.Count;
            int j, d;
            string[] pops;

            ExpressionSegment p1, p2, p3;

            if (opcount == 1)
            {
                if ((partType & PartType.Composite) == PartType.Composite)
                    partType |= PartType.Executive;
                else
                    partType |= PartType.Executive;

                if (parts.Count == 2)
                {
                    if (!parts[0].IsUnitaryOperator)
                    {
                        ErrorText = "Unexpected identifier '" + parts[0] + "' in expression '" + ToString() + "'";
                        throw new SyntaxErrorException(errorText);
                    }
                    else if ((parts[1].partType & PartType.Composite) == 0)
                    {
                        ErrorText = "Unexpected identifier '" + parts[1] + "' in expression '" + ToString() + "'";
                        throw new SyntaxErrorException(errorText);
                    }

                }
                else if (parts.Count == 3 && parts[1].IsUnitaryOperator)
                {
                    es = new ExpressionSegment("*");
                    parts.Insert(1, es);

                    es = new ExpressionSegment();
                    
                    es.parent = this;
                    es.ci = ci;
                    es.position = position;
                    es.varSym = varSym;

                    parts[1].parent = es;
                    parts[2].parent = es;

                    es.parts.Add(parts[2]);
                    es.parts.Add(parts[3]);

                    es.partType |= PartType.Executive;
                    parts.RemoveRange(2, 2);
                    parts.Insert(2, es);

                }

            }
            else if (opcount > 1)
            {
                for (i = 0; i < c; i++)
                {
                    if (parts.Count((p) => p.partType == PartType.Operator) < 2) break;
                    d = parts.Count;

                    pops = cops[i].Split(',');

                    for (j = 0; j < d; j++)
                    {
                        if (parts.Count((p) => p.partType == PartType.Operator) < 2) break;

                        if (parts[j].partType == PartType.Operator)
                        {
                            if (pops.Contains(parts[j].monoVal))
                            {
                                if (parts[j].IsUnitaryOperator)
                                {
                                    var oj = j;
                                    while (parts[j].IsUnitaryOperator && j < d - 1)
                                    {
                                        j++;
                                    }
                                    j--;
                                    if (j == d - 1)
                                    {
                                        ErrorText = "Unexpected identifier '" + parts[j] + "' in expression '" + ToString() + "'";
                                        throw new SyntaxErrorException(errorText);
                                        //return;
                                    }
                                    else if ((parts[j + 1].partType & PartType.Composite) == 0)
                                    {
                                        ErrorText = "Unexpected identifier '" + parts[j + 1] + "' in expression '" + ToString() + "'";
                                        throw new SyntaxErrorException(errorText);
                                    }

                                    p1 = parts[j];
                                    p2 = parts[j + 1];

                                    es = new ExpressionSegment();

                                    es.ci = ci;
                                    es.parent = this;
                                    es.partType |= PartType.Executive;
                                    es.storageMode = storageMode;
                                    es.position = position;

                                    p1.parent = p2.parent = es;
                                    --d;

                                    es.parts.Add(p1);
                                    es.parts.Add(p2);

                                    parts.RemoveRange(j, 2);
                                    parts.Insert(j, es);                                        

                                    if (j >= d - 2) break;

                                }
                                else 
                                { 
                                    if (j == 0 || j == d - 1)
                                    {
                                        ErrorText = "Unexpected identifier '" + parts[j] + "' in expression '" + ToString() + "'";
                                        throw new SyntaxErrorException(errorText);
                                        //return;
                                    }


                                    p1 = parts[j];
                                    p2 = parts[j + 1];
                                    p3 = parts[j - 1];

                                    es = new ExpressionSegment();

                                    es.ci = ci;
                                    es.parent = this;
                                    es.partType |= PartType.Executive;
                                    es.storageMode = storageMode;
                                    es.position = position;

                                    p1.parent = p2.parent = p3.parent = es;
                                    es.parts.Add(p3);
                                    es.parts.Add(p1);
                                    es.parts.Add(p2);

                                    parts.RemoveRange(j - 1, 3);
                                    parts.Insert(j - 1, es);

                                    d -= 2;
                                    j -= 1;

                                    if (j >= d - 2) break;
                                }
                            }
                        }
                    }
                }

                if (parts.Count((p) => p.partType == PartType.Operator) == 1)
                {
                    if (partType == PartType.Composite)
                        partType |= PartType.Executive;
                    else
                        partType |= PartType.Executive;
                }
            }

            d = parts.Count;
            for (j = 0; j < d; j++)
            {
                var part = parts[j];
               
                if (part.parts.Count < 2)
                {
                    continue;
                }
                else
                {
                    opcount = part.parts.Count((p) => p.partType == PartType.Operator);
                    if (opcount == 1)
                    {
                        if ((part.partType & PartType.Composite) == PartType.Composite)
                            part.partType |= PartType.Executive;
                        else
                            part.partType |= PartType.Executive;

                    }
                }
                j++;
            }
        }

        /// <summary>
        /// Check if the expression is solvable.
        /// </summary>
        /// <returns></returns>
        private bool CheckSolvable()
        {
            var i = parts.Count((e) => e.partType == PartType.Assignment || e.partType == PartType.Equality);
            if (i == 1 && parts[0].PartType != PartType.Assignment && parts[0].PartType != PartType.Equality)
            {
                return HasMatchingUnits();
            }
            else if ((partType & PartType.Executive) == PartType.Executive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Private Methods

        #region Operators

        public static explicit operator decimal(ExpressionSegment val)
        {
            return val.ValueToDecimal() ?? decimal.Zero;
        }

        public static explicit operator decimal?(ExpressionSegment val)
        {
            return val.ValueToDecimal();
        }

        public static explicit operator double(ExpressionSegment val)
        {
            return val.ValueToDouble() ?? double.NaN;
        }

        public static explicit operator double?(ExpressionSegment val)
        {
            return val.ValueToDouble();
        }

        public static explicit operator ExpressionSegment(string val)
        {
            return new ExpressionSegment(val);
        }

        public static explicit operator ExpressionSegment(sbyte val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(byte val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(float val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(ushort val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(short val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(uint val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(ulong val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(int val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(long val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(double val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDouble);
        }

        public static explicit operator ExpressionSegment(decimal val)
        {
            return new ExpressionSegment(val.ToString(), StorageMode.AsDecimal);
        }

        public static implicit operator string(ExpressionSegment val)
        {
            return val.ToString();
        }

        public static bool operator !=(ExpressionSegment left, ExpressionSegment right)
        {
            if (left is null && right is null) return false;
            else if (left is ExpressionSegment && right is ExpressionSegment)
            {
                return !left.Equals(right);
            }
            return true;
        }

        public static bool operator ==(ExpressionSegment left, ExpressionSegment right)
        {
            if (left is null && right is null) return true;
            else if (left is ExpressionSegment && right is ExpressionSegment)
            {
                return left.Equals(right);
            }
            return false;
        }

        #endregion

    }
}
