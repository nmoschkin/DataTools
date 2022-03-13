using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

using static DataTools.Text.TextTools;

namespace DataTools.Extras
{

    public enum PartType
    {
        Monolithic,
        Composite,
        Numeric
    }

    public class ExpPart
    {

        private PartType partType = PartType.Monolithic;
        private List<ExpPart> parts;
        private ExpPart parent;
        private string monoVal;
        private CultureInfo ci = CultureInfo.CurrentCulture;
        
        public IReadOnlyList<ExpPart> SubParts => parts.ToArray();

        public ExpPart Parent => parent;

        public PartType PartType => partType;

        public bool HasChildren
        {
            get
            {
                return SubParts.Count > 0;
            }
        }

        public CultureInfo CultureInfo => ci;

        //public double CalculateValue(Dictionary<string, double> variables = null)
        //{
        //    if (monoVal != null)
        //    {
        //        if (variables.TryGetValue(monoVal, out var value))
        //        {
        //            return value;
        //        }
        //        else if (IsNumber(monoVal))
        //        {
        //            return FVal(monoVal) ?? 0d;
        //        }
        //    }
        //}

        public ExpPart(string value) : this(value, null, null)
        {
        }
        public ExpPart(string value, CultureInfo ci) : this(value, null, ci)
        {
        }

        private ExpPart(string value, ExpPart parent, CultureInfo ci)
        {
            this.ci = ci ?? this.ci ?? CultureInfo.CurrentCulture;

            parts = new List<ExpPart>();
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
                        parts.Add(new ExpPart(sb.ToString(), this, ci));
                        sb.Clear();
                    }

                    var val = TextBetween(value, i, sc1, sc2, out b, out e);
                    parts.Add(new ExpPart(val, this, ci)
                    {
                        partType = PartType.Composite
                    });

                    if (b is int && e is int n)
                    {
                        i = n + 1;
                        continue;
                    }
                }
                else if (value[i] == ' ')
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(new ExpPart(sb.ToString(), this, ci));
                        sb.Clear();
                    }
                }
                else
                {
                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out _))
                    {
                        if ((sb.ToString() + value[i] != "0x") && !double.TryParse(sb.ToString() + value[i], NumberStyles.Any, ci, out _))
                        {
                            parts.Add(new ExpPart(sb.ToString(), this, ci)
                            {
                                partType = PartType.Numeric
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && !long.TryParse(sb.ToString().Substring(2) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpPart(sb.ToString(), this, ci)
                            {
                                partType = PartType.Numeric
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && !long.TryParse(sb.ToString().Substring(2) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpPart(sb.ToString(), this, ci)
                            {
                                partType = PartType.Numeric
                            });
                            sb.Clear();
                        }
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && !long.TryParse(sb.ToString().Substring(1) + value[i], NumberStyles.AllowHexSpecifier, ci, out _))
                        {
                            parts.Add(new ExpPart(sb.ToString(), this, ci)
                            {
                                partType = PartType.Numeric
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

                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out _))
                    {
                        partType = PartType.Numeric;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        partType = PartType.Numeric;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        partType = PartType.Numeric;
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        partType = PartType.Numeric;
                    }

                    monoVal = sb.ToString();

                }
                else
                {
                    var newPart = new ExpPart(sb.ToString(), this, ci);

                    if (double.TryParse(sb.ToString(), NumberStyles.Any, ci, out _))
                    {
                        newPart.partType = PartType.Numeric;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "0x" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        newPart.partType = PartType.Numeric;
                    }
                    else if (sb.Length > 2 && sb.ToString().Substring(0, 2) == "&H" && long.TryParse(sb.ToString().Substring(2), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        newPart.partType = PartType.Numeric;
                    }
                    else if (sb.Length > 1 && sb.ToString().Substring(0, 1) == "#" && long.TryParse(sb.ToString().Substring(1), NumberStyles.AllowHexSpecifier, ci, out _))
                    {
                        newPart.partType = PartType.Numeric;
                    }

                    parts.Add(newPart);
                    
                    if (parent == null)
                    {
                        partType = PartType.Composite;
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

        public static implicit operator string(ExpPart val)
        {
            return val.ToString();  
        }

        public static explicit operator ExpPart(string val)
        {
            return new ExpPart(val);    
        }



    }
}
