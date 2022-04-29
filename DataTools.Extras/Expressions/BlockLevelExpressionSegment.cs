using DataTools.PlugInFramework;
using DataTools.Streams;
using DataTools.Text;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace DataTools.Extras.Expressions
{

    public class BlockPair
    {
        public string Begin { get; set; }

        public string End { get; set; }

        public bool IsRegex { get; set; }
        
        public int RegexNameGroup { get; set; }

    }

    public class BlockExpressionConfig
    {

        public List<string> EntityTypeKeywords { get; set; } = new List<string>();

        public List<string> AdditionalKeywords { get; set; } = new List<string>();

        public List<string> IntrinsicTypes { get; set; } = new List<string>();

        public List<string> AccessModifiers { get; set; } = new List<string>();

        public List<(string Begin, string End)> AdditionalBlockPairs { get; set; } = new List<(string Begin, string End)>();

        public List<Assembly> AdditionalTypes { get; set; } = new List<Assembly>();


        public static readonly BlockExpressionConfig CSBlockConfig;


        public bool HasPointers { get; set; }

        static BlockExpressionConfig()
        {
            CSBlockConfig = new BlockExpressionConfig();

            CSBlockConfig.AccessModifiers.AddRange(new[] { "public", "private", "protected", "internal", "global" });
            CSBlockConfig.EntityTypeKeywords.AddRange(new[] { "class", "const", "struct", "record", "interface", "namespace", "delegate", "event", "namespace" });
            CSBlockConfig.AdditionalKeywords.AddRange(new[] { "async", "extern", "override", "abstract", "new", "unsafe", "fixed", "static", "using", "for", "while", "do", "if", "else", "switch", "case", "default", "break", "yield", "return", "throw", "try", "catch", "finally", "foreach", "in", "out", "ref", "null", "is", "not" });
            CSBlockConfig.IntrinsicTypes.AddRange(new[] { "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "Guid", "DateTime", "string", "char", "void", "var", "dynamic", "object", "Enum", "Tuple", "bool" });
        }
    }
    
    public class BlockLevelExpressionSegment : ExpressionSegment
    {
        private BlockExpressionConfig config = BlockExpressionConfig.CSBlockConfig;

        public string EntityType { get; protected set; }

        public string[] Keywords { get;protected set; }

        public string[] AccessModifiers { get; protected set; }

        public bool IsIntrinsicType { get; protected set; }

        public (string, string) BlockPair { get; protected set; } = ("", "");

        public bool NoSpace { get; protected set; }

        public bool AddLine { get; protected set; }

        public bool IsParameter { get; protected set; }

        public static readonly string[] ProgrammingOperations = new string[] { "~", "!", "&", "|", "&&", "||", "^", "?", "==", "!=", "=>", ">", "<", "<=", ">=", "+=", "-=", "++", "--", ":", "=", "+", "/", "*", "-" };

        protected BlockLevelExpressionSegment()
        {
        }

        protected BlockLevelExpressionSegment(string value, ExpressionSegment parent, CultureInfo ci, StorageMode mode, string varSym) : base (value, parent, ci, mode, varSym)
        {
        }

        protected override void Initialize(string value, ExpressionSegment parent, CultureInfo ci, StorageMode mode, string varSym)
        {
            int i, c = value.Length;

            var proc = TextTools.SpaceOperators(TextTools.OneSpace(value)).Trim();

            var sb = new StringBuilder();
            var wd = new StringBuilder();

            int currLine = 1;

            if (parent == null)
            {
                partType = PartType.BlockLevelEntity;
            }

            for (i = 0; i < c; i++)
            {
                if (value[i] == '\n')
                {
                    var word = wd.ToString().Trim();
                    if (!string.IsNullOrEmpty(word))
                    {
                        this.parts.Add(new BlockLevelExpressionSegment()
                        {
                            parent = this,
                            ci = ci,
                            storageMode = mode,
                            stringValue = word,
                            partType = PartType.Variable
                        });
                    }
                    this.parts.Add(new BlockLevelExpressionSegment()
                    {
                        parent = this,
                        ci = ci,
                        storageMode = mode,
                        NoSpace = true,
                        stringValue = value[i].ToString(),
                        partType = PartType.Variable
                    });
                    sb.Clear();
                    wd.Clear();
                }
                else if (value[i] == '{')
                {

                    if (sb.Length > 0)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(sb.ToString(), this, ci, mode, varSym));
                        sb.Clear();
                    }

                    wd.Clear();

                    var txt = TextTools.TextBetween(value, i, "{", "}", out int? startIdx, out int? stopIdx);
                    
                    if (txt != null && stopIdx is int x)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(txt, this, ci, mode, varSym)
                        {
                            partType = PartType.BlockLevelEntity,
                            BlockPair = ("{\n", "}\n")
                        });
                        i = x + 1;
                    }
                }

                else if (value[i] == '(')
                {

                    if (sb.Length > 0)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(sb.ToString(), this, ci, mode, varSym));
                        sb.Clear();
                    }

                    wd.Clear();
                    var txt = TextTools.TextBetween(value, i, "(", ")", out int? startIdx, out int? stopIdx);

                    if (txt != null && stopIdx is int x)
                    {
                        var newPart = new BlockLevelExpressionSegment()
                        {
                            parent = this,
                            ci = ci,
                            storageMode = mode,
                            partType = PartType.Parenthesis,
                            BlockPair = ("(", ")")

                        };

                        this.parts.Add(newPart);

                        var sp = TextTools.Split(txt, ",");

                        if (sp.Length > 0)
                        {
                            foreach (var part in sp)
                            {
                                newPart.parts.Add(new BlockLevelExpressionSegment(part, newPart, ci, mode, varSym)
                                {
                                    IsParameter = true
                                });
                            }
                        }

                        i = x;
                    }
                }

                else if (value[i] == '<')
                {

                    if (sb.Length > 0)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(sb.ToString(), this, ci, mode, varSym));
                        sb.Clear();
                    }

                    wd.Clear();
                    var txt = TextTools.TextBetween(value, i, "<", ">", out int? startIdx, out int? stopIdx);

                    if (txt != null && stopIdx is int x)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(txt, this, ci, mode, varSym)
                        {
                            BlockPair = ("<", ">")
                        });
                        i = x;
                    }

                }
                else if (value[i] == '[')
                {

                    if (sb.Length > 0)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(sb.ToString(), this, ci, mode, varSym));
                        sb.Clear();
                    }

                    wd.Clear();
                    var txt = TextTools.TextBetween(value, i, "[", "]", out int? startIdx, out int? stopIdx);

                    if (txt != null && stopIdx is int x)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(txt, this, ci, mode, varSym)
                        {
                            BlockPair = ("[", "]"),
                            AddLine = true
                        });
                        i = x;
                    }

                }
                else if (value[i] == ';')
                {

                    if (sb.Length > 0)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(sb.ToString(), this, ci, mode, varSym));
                    }

                    this.parts.Add(new BlockLevelExpressionSegment()
                    {
                        parent = this,
                        ci = ci,
                        storageMode = mode,
                        stringValue = ";",
                        partType = PartType.Executive,
                        NoSpace = true
                    });
                                        
                    sb.Clear();
                    wd.Clear();
                }
                else if (value[i] == ',')
                {
                    this.parts.Add(new BlockLevelExpressionSegment(sb.ToString(), this, ci, mode, varSym)
                    {
                        IsParameter = true
                    });

                    sb.Clear();
                    wd.Clear();

                }
                else if (value[i] == '"')
                {
                    var txt = TextTools.QuoteFromHere(value, i, out int? startPos, out int? endPos);
                    if (txt != null && endPos is int x)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(txt, this, ci, mode, varSym)
                        {
                            partType = PartType.String
                        });
                        i = x + 1;
                    }
                }
                else if (value[i] == '\'')
                {
                    var txt = TextTools.QuoteFromHere(value, i, out int? startPos, out int? endPos, quoteChar: '\'');
                    if (txt != null && endPos is int x)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment(txt, this, ci, mode, varSym)
                        {
                            partType = PartType.String
                        });
                        i = x + 1;
                    }
                }
                else if (value[i] == '/' && i < c - 1 && value[i + 1] == '/')
                {
                    var stopIdx = value.IndexOf('\n', i + 2);
                    string txt;
                    if (stopIdx == -1)
                    {
                        txt = value.Substring(i);
                    }
                    else
                    {
                        txt = value.Substring(i, stopIdx - i + 1);
                    }

                    if (txt != null && stopIdx > 0)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment()
                        {
                            parent = this,
                            ci = ci,
                            storageMode = mode,
                            stringValue = txt,
                            partType = PartType.LineComment
                        });
                        i = stopIdx + 1;
                    }

                }
                else if (value[i] == '/' && i < c - 1 && value[i + 1] == '*')
                {
                    var stopIdx = value.IndexOf("*/", i + 2);
                    string txt;
                    if (stopIdx == -1)
                    {
                        txt = value.Substring(i);
                    }
                    else
                    {
                        txt = value.Substring(i, stopIdx - i + 1);
                    }

                    if (txt != null && stopIdx > 0)
                    {
                        this.parts.Add(new BlockLevelExpressionSegment()
                        {
                            parent = this,
                            ci = ci,
                            storageMode = mode,
                            stringValue = txt,
                            partType = PartType.BlockComment
                        });
                        i = stopIdx + 1;
                    }

                }
                else if (value[i] == '.' || value[i] == '?')
                {
                    var word = wd.ToString().Trim();
                    if (!string.IsNullOrEmpty(word))
                    {
                        this.parts.Add(new BlockLevelExpressionSegment()
                        {
                            parent = this,
                            ci = ci,
                            storageMode = mode,
                            stringValue = word,
                            partType = PartType.Variable
                        });
                    }
                    this.parts.Add(new BlockLevelExpressionSegment()
                    {
                        parent = this,
                        ci = ci,
                        storageMode = mode,
                        NoSpace = true,
                        stringValue = value[i].ToString(),
                        partType = PartType.Variable
                    });
                    sb.Clear();
                    wd.Clear();
                }
                else if (value[i] == ' ')
                {
                    var word = wd.ToString().Trim();
                    if (sb.Length > 0 && sb.ToString() != word)
                    {
                        var txt = sb.ToString();
                        if (txt.EndsWith(word))
                        {
                            txt = txt.Substring(0, txt.Length - word.Length);
                        }
                        sb.Clear();
                        this.parts.Add(new BlockLevelExpressionSegment(txt, this, ci, mode, varSym));
                    }
                    else
                    {
                        sb.Clear();
                    }

                    wd.Clear();
                    var part = new BlockLevelExpressionSegment()
                    {
                        parent = this,
                        ci = ci,
                        storageMode = mode,
                        stringValue = word,
                        partType = PartType.Keyword
                    };

                    parts.Add(part);
                }
                else
                {
                    wd.Append(value[i]);    
                    sb.Append(value[i]);
                }
            }

            var last = sb.ToString().Trim();
            if (last.Length > 0)            
            {
                if (parts.Count == 0)
                {
                    if (stringValue == null) stringValue = "";
                    partType = PartType.Simple;
                    stringValue += sb.ToString();
                }
                else
                {
                    var p = new BlockLevelExpressionSegment(sb.ToString(), this, ci, mode, varSym);
                    if (((BlockLevelExpressionSegment)parts[parts.Count - 1]).IsParameter) p.IsParameter = true;
                    this.parts.Add(p);

                }
            }

            if (!string.IsNullOrEmpty(stringValue))
            {
                stringValue = stringValue.Trim();
                CheckWord(stringValue, this);
            }

            var d = parts.Count - 1;
            for (int z = d; z >= 0; z--)
            {
                var part = parts[z] as BlockLevelExpressionSegment;
                if (part.stringValue != null)
                {
                    part.stringValue = part.stringValue?.Trim() ?? "";
                    if (string.IsNullOrEmpty(part.stringValue))
                    {
                        parts.RemoveAt(z);
                    }
                    else
                    {
                        CheckWord(part.stringValue, part);
                    }
                }
            }
        }

        private int CheckWord(string word, BlockLevelExpressionSegment part)
        {
            if (config.AccessModifiers.Contains(word))
            {
                part.partType = PartType.AccessModifier;
                return 1;
            }
            else if (config.EntityTypeKeywords.Contains(word))
            {
                part.partType = PartType.EntityType;

                return 2;
            }
            else if (config.AdditionalKeywords.Contains(word))
            {
                part.partType = PartType.Keyword;
                return 3;
            }
            else if (config.IntrinsicTypes.Contains(word))
            {
                part.partType = PartType.IntrinsicType;
                return 4;
            }
            else if (!string.IsNullOrEmpty(word))
            {
                part.partType = PartType.Variable;
                return 5;
            }
            else if (ProgrammingOperations.Contains(word))
            {
                part.partType = PartType.Operator;
                return 6;
            }
            else if (Operations.Contains(word))
            {
                part.partType = PartType.Operator;
                return 6;
            }
            return 0;
        }

        public override string ToString()
        {
            return ToString(0);
        }

        protected string ToString(int level)
        {
            var sb = new StringBuilder();
            var (start, stop) = BlockPair;
            var sv = stringValue?.Trim() ?? "";
            var le = "";
            
            if (level > 0)
            {
                le = new string(' ', level * 4);
            }

            if (partType == PartType.BlockLevelEntity) sb.Append("\n" + le);

            sb.Append(start);
            int x = 0;
            if (parts.Count > 0)
            {
                sb.Append(le);
                foreach (BlockLevelExpressionSegment part in parts)
                {
                    if ((part.partType & PartType.BlockLevelEntity) == PartType.BlockLevelEntity)
                    {
                        sb.Append(part.ToString(level + 1));
                    }
                    else
                    {
                        if (x > 0)
                        {
                            if (part.IsParameter) sb.Append(",");

                            if (!(part.NoSpace || (x > 0 && ((BlockLevelExpressionSegment)parts[x - 1]).NoSpace)))
                            {
                                sb.Append(" ");
                            }

                        }
                        
                        sb.Append(part.ToString());
                        x++;
                    }

                }
            }
            else if (sv != "")
            {
                sb.Append(sv);
            }


            if (partType == PartType.Executive)
            {
                sb.Insert(0, le);
                sb.Append("\n");
            }
            else
            {
                sb.Append(le);
                sb.Append(stop);
                if (partType == PartType.LineComment || partType == PartType.Executive || AddLine) sb.Append("\n");
            }

            
            return sb.ToString().Replace("\n", "\r\n");
        }

        public static BlockLevelExpressionSegment LoadFile(string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException(filename);

            var txt = File.ReadAllText(filename);
            return new BlockLevelExpressionSegment(txt, null, CultureInfo.CurrentCulture, StorageMode.AsDouble, "");
        }

    }
}
