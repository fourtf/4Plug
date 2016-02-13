using FPlug.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FPlug.Scripting
{
    public class Script
    {
        /// <summary>
        /// List of the functions available in all scripts.
        /// </summary>
        //public static List<FunctionDesc> Functions = new List<FunctionDesc>();

        /// <summary>
        /// The SettingsWindow which belongs to this Script
        /// </summary>
        public SettingsWindow SettingsWindow { get; private set; }

        /// <summary>
        /// The StringParser used to parse this script.
        /// </summary>
        public StringParser Parser { get; set; }

        /// <summary>
        /// Root code block of the script.
        /// </summary>
        public FunctionStack RootBlock { get; private set; }

        string _folder;

        /// <summary>
        /// Initialized a new Script.
        /// </summary>
        /// <param name="window">SettingsWindow this script belongs to.</param>
        /// <param name="script">Text of the script.</param>
        /// <param name="folder">Root folder for file operations.</param>
        public Script(SettingsWindow window, string script, string folder, Tuple<int, int> textOffset, bool compile = true)
        {
            SettingsWindow = window;

            _folder = folder;

            //foreach (string s in Directory.EnumerateFiles(folder))
            //{
            //    File.Delete(s);
            //}
            //
            //AddFunction("Do", Types.Boolean, Types.Any, Types.Boolean, () => new DoFunction());
            //AddFunction("Dont", Types.Boolean, Types.Any, Types.Boolean, () => new DontFunction());

            //String text = File.ReadAllText("script.4s");

            string text = script;

            Parser = new StringParser(text);
            Parser.TextOffset = textOffset;

            if (compile)
                Compile();
        }

        private Variable startVariable;

        public FolderCache FolderCache { get; private set; }

        public bool Compiled { get; private set; }

        public void Compile()
        {
            Stopwatch compileTime = Stopwatch.StartNew();

            try
            {
                RootBlock = ParseStack(Parser, false);
                Compiled = true;

                startVariable = new Variable(new object[VariableNames.Count], this);

                compileTime.Stop();
                //Console.WriteLine("-- Compiled in " + compileTime.Elapsed.TotalMilliseconds + " ms");
            }
            catch (IndexOutOfRangeException)
            {
                SettingsWindow.LogError("Unexpected end of file.", ErrorType.Compiler);

                //Console.WriteLine("-- Error while compiling.");
                //Console.WriteLine("- Did not reach the end");
            }
            catch (ScriptException exc)
            {
                compileTime.Stop();
                var loc = exc.Parser.GetTextLocation(exc.Position);

                SettingsWindow.LogError(exc.Message, loc.Item1, loc.Item2, ErrorType.Compiler);
            }
            catch (Exception exc)
            {
                Console.WriteLine("- while compiling: " + exc.Message);
            }
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        public void Execute()
        {
            if (Compiled)
            {
                FolderCache = SettingsWindow.FolderCache;

                for (int i = 0; i < VariableNames.Count; i++)
                {
                    object o;
                    if (startVariables.TryGetValue(VariableNames[i], out o))
                        startVariable.Variables[i] = o;
                }

                var executeTime = Stopwatch.StartNew();
                RootBlock.ExecuteByVarClone(startVariable);
                executeTime.Stop();
                //Console.WriteLine("-- Executed in " + executeTime.Elapsed.TotalMilliseconds + " ms");
            }
            else
            {
                Console.WriteLine("-- Scipt Not Compiled");
            }
        }

        public List<string> VariableNames = new List<string>();
        public List<ScriptType> VariableTypes = new List<ScriptType>();

        private Dictionary<string, object> startVariables = new Dictionary<string, object>();

        public void RegisterVariable(string name, object variable, ScriptType type)
        {
            VariableNames.Add(name);
            VariableTypes.Add(type);
            startVariables[name] = variable;
        }

        FunctionStack ParseStack(StringParser parser, bool inParameter, int logicLevel = 0)
        {
            char c;
            List<string> variablesToBeAssigned = null;

            FunctionStack stack = new FunctionStack();
            List<Function> functions = new List<Function>();
            //bool intro = true;

            ScriptType lastType = ScriptType.Static;

            while (true)
            {
                parser.ReadWhitespace();
                if (parser.Ended())
                    break;

                if ((c = parser.CurrentChar) >= 'a' && c <= 'z') // variable or if
                {
                    #region variable & if
                    string name = parser.ReadName();
                    int nameposition = parser.Position;

                    bool isIf;

                    if (name == "true")
                    {
                        functions.Add(new Constant(true));
                        lastType = ScriptType.Bool;
                    }
                    else if (name == "false")
                    {
                        functions.Add(new Constant(false));
                        lastType = ScriptType.Bool;
                    }
                    else if ((isIf = name == "if") || name == "else")
                    {
                        if (lastType != ScriptType.Static || inParameter)
                            throw new ScriptException(parser, nameposition, "\"if\" is not allowed here.");

                        bool elseIf = false;
                        if (!isIf)
                        {
                            parser.ReadWhitespace();
                            int tmp = parser.Position;
                            if (parser.ReadName() == "if")
                            {
                                isIf = true;
                                elseIf = true;
                            }
                            else
                            {
                                parser.Position = tmp;
                            }
                        }

                        if ((!isIf || elseIf) && (functions.Count == 0 || !(functions[functions.Count - 1] is IfFunction || (!(functions[functions.Count - 1] is ElseFunction) || ((ElseFunction)functions[functions.Count - 1]).Condition != null))))
                            throw new ScriptException(parser, "\"else\" is only allowed after \"if\" and \"else if\".");

                        parser.ReadWhitespace();

                        FunctionStack condition = null;

                        if (isIf)
                        {
                            if (parser.CurrentChar != '(')
                                unexpected();
                            ++parser.Position;
                            condition = ParseStack(parser, true);
                            if (parser.CurrentChar != ')')
                                unexpected();
                            parser.Position++;
                            if (condition.ReturnType != ScriptType.Bool)
                                throw new ScriptException(parser, parser.Position - 1, "if condition needs to be a bool.");
                            parser.ReadWhitespace();
                        }

                        if (parser.CurrentChar != '{')
                            unexpected();
                        parser.Position++;
                        var st = ParseStack(parser, false);

                        if (isIf)
                        {
                            if (elseIf) // else if
                            {
                                functions.Add(new ElseFunction() { Condition = condition, Args = new[] { st } });
                            }
                            else // if
                            {
                                functions.Add(new IfFunction() { Condition = condition, Args = new[] { st } });
                            }
                        }
                        else // else
                        {
                            functions.Add(new ElseFunction() { Args = new[] { st } });
                        }
                        lastType = ScriptType.Static;
                    }
                    else // variable
                    {
                        parser.ReadWhitespace();

                        if (parser.CurrentChar == '=' && parser.PeekChar() != '=')
                        {
                            (variablesToBeAssigned ?? (variablesToBeAssigned = new List<string>())).Add(name);
                            parser.Position++;
                            continue;
                        }
                        else
                        {
                            int index;
                            if ((index = VariableNames.IndexOf(name)) == -1)
                            {
                                throw new ScriptException(parser, nameposition, "Variable \"" + name + "\" doesn't exist");
                            }
                            else
                            {
                                functions.Add(new GetVariable(this, name, index));
                                lastType = VariableTypes[index];
                            }
                        }
                    }
                    #endregion
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    #region function / property
                    int namePosition = parser.Position;
                    string name = parser.ReadName();
                    parser.ReadWhitespace();

                    if (!parser.Ended() && parser.CurrentChar == '(')
                    {
                        parser.Position++;
                        List<FunctionStack> args = new List<FunctionStack>();

                        parser.ReadWhitespace();

                        ScriptType[] types = null;
                        if (parser.CurrentChar != ')')
                        {
                            do
                                args.Add(ParseStack(parser, true));
                            while (parser.CurrentChar == ',' && ++parser.Position != -69);

                            if (parser.CurrentChar != ')')
                                unexpected();

                            types = args.Select(x => x.ReturnType).ToArray();
                        }

                        parser.Position++;

                        Method method = lastType.GetMethod(name, types);
                        if (method == null)
                            throw new ScriptException(Parser, namePosition, "Method \"" + (lastType == ScriptType.Static ? "" : lastType.Name + ".") + name + "(" + (types == null ? "" : string.Join(", ", types.Select(t => t.Name))) + ")\" does not exist.");
                        var function = method.CreateFunction(this);

                        if (args.Count > 0)
                            function.Args = args.ToArray();

                        functions.Add(function);
                        lastType = method.ReturnType ?? lastType;
                    }
                    else
                    {
                        Property property = lastType.GetProperty(name);
                        if (property == null)
                            throw new ScriptException(Parser, namePosition, "Property \"" + (lastType == ScriptType.Static ? "" : lastType.Name + ".") + name + "\" does not exist.");

                        if (parser.CurrentChar == '=')
                        {
                            parser.Position++;
                            var s = ParseStack(parser, true);

                            if (!s.ReturnType.ExtendsOrIs(property.ReturnType))
                                throw new ScriptException(parser);
                            if (!property.HasSet)
                                throw new ScriptException(Parser, namePosition, "Property \"" + (lastType == ScriptType.Static ? "" : lastType.Name + ".") + name + "\" is readonly.");

                            var set = property.GetSetFunction();
                            set.Args = new FunctionStack[] { s };
                            functions.Add(set);
                            continue;
                        }
                        else
                        {
                            if (!property.HasGet)
                                throw new ScriptException(Parser, namePosition, "Property \"" + (lastType == ScriptType.Static ? "" : lastType.Name + ".") + name + "\" is writeonly.");

                            functions.Add(property.GetGetFunction());
                        }
                        lastType = property.ReturnType ?? lastType;
                    }
                    #endregion
                }
                else if (c == '$')
                {
                    #region Widget
                    parser.Position++;
                    string name = parser.ReadName();
                    var child = SettingsWindow.GetChildByID(name);
                    if (child == null)
                        throw new Exception("A Control with the name \"" + name + "\" doesn't exist");

                    functions.Add(new Constant(child));
                    lastType = child.ScriptType;
                    #endregion
                }
                else if (c == '(')
                {
                    parser.Position++;
                    var s = ParseStack(parser, true);
                    if (parser.CurrentChar != ')')
                        unexpected(parser.Position);
                    parser.Position++;
                    functions.Add(s);
                    lastType = s.ReturnType;
                }
                else if (c == '!')
                {
                    parser.Position++;
                    var s = ParseStack(parser, true, 543255234);
                    if (s.ReturnType != ScriptType.Bool)
                        throw new ScriptException(parser, parser.Position - 1, "\"!\" can only be applied to Bools.");

                    functions.Add(new ParamLambda((v, p) => v.CurrentObject = !((bool)p), s));

                    lastType = ScriptType.Bool;
                    parser.ReadWhitespace();
                    goto calcStart;
                }
                else if ((c >= '0' && c <= '9') || c == '-') // number
                {
                    functions.Add(new Constant(parser.ReadNumber()));
                    lastType = ScriptType.Number;
                }
                else if (c == '\"' || c == ':') // string
                {
                    functions.Add(new Constant(parser.ReadEscaped(c)));
                    lastType = ScriptType.String;
                }
                else if (!inParameter && c == '}')
                {
                    break;
                }
                else if (!inParameter && c == ';')
                {
                    parser.Position++;
                    continue;
                }
                else
                {
                    unexpected(parser.Position);
                }

                parser.ReadWhitespace();
                if (parser.Ended())
                    break;

            calcStart:

                c = parser.CurrentChar;

                if (c == '+')
                {
                    if (logicLevel >= 3)
                        break;
                    parser.Position++;
                    var s = ParseStack(parser, true, 3);
                    if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                    {
                        functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject + (double)p, s));
                        lastType = ScriptType.Number;
                    }
                    else if ((lastType == ScriptType.String || s.ReturnType == ScriptType.String) &&
                        ((lastType == ScriptType.String || lastType == ScriptType.Number || lastType == ScriptType.Bool) &&
                        (s.ReturnType == ScriptType.String || s.ReturnType == ScriptType.Number || s.ReturnType == ScriptType.Bool)))
                    {
                        functions.Add(new ParamLambda((v, p) => v.CurrentObject = v.CurrentObject.ToString() + p.ToString(), s));
                        lastType = ScriptType.String;
                    }
                    else
                        throw new ScriptException(parser, parser.Position - 1, "Can not add \"" + lastType.Name + "\" and \"" + s.ReturnType.Name + "\".");

                    parser.ReadWhitespace();
                    goto calcStart;
                }
                else if (c == '-')
                {
                    if (logicLevel >= 3)
                        break;
                    parser.Position++;
                    var s = ParseStack(parser, true, 3);
                    if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                        functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject - (double)p, s));
                    else
                        throw new ScriptException(parser, parser.Position - 1, "Can not subtract \"" + lastType.Name + "\" and \"" + s.ReturnType.Name + "\".");

                    parser.ReadWhitespace();
                    lastType = ScriptType.Number;
                    goto calcStart;
                }
                else if (c == '*')
                {
                    if (logicLevel >= 4)
                        break;
                    parser.Position++;
                    var s = ParseStack(parser, true, 4);
                    if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                        functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject * (double)p, s));
                    else
                        throw new ScriptException(parser, parser.Position - 1, "Can not multiply \"" + lastType.Name + "\" and \"" + s.ReturnType.Name + "\".");

                    parser.ReadWhitespace();
                    lastType = ScriptType.Number;
                    goto calcStart;

                }
                else if (c == '/')
                {
                    if (logicLevel >= 4)
                        break;
                    parser.Position++;
                    var s = ParseStack(parser, true, 4);
                    if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                        functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject / (double)p, s));
                    else
                        throw new ScriptException(parser, parser.Position - 1, "Can not divide \"" + lastType.Name + "\" and \"" + s.ReturnType.Name + "\".");

                    parser.ReadWhitespace();
                    lastType = ScriptType.Number;
                    goto calcStart;
                }
                else if (c == '<')
                {
                    if (parser.PeekChar() == '=')
                    {
                        if (logicLevel >= 2)
                            break;
                        parser.Position += 2;
                        var s = ParseStack(parser, true, 2);
                        if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                            functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject <= (double)p, s));
                        else
                            throw new ScriptException(parser, parser.Position - 1, "\"<=\" can only compare Numers.");
                        parser.ReadWhitespace();
                        lastType = ScriptType.Bool;
                        goto calcStart;
                    }
                    else
                    {
                        if (logicLevel >= 2)
                            break;
                        parser.Position += 1;
                        var s = ParseStack(parser, true, 2);
                        if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                            functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject < (double)p, s));
                        else
                            throw new ScriptException(parser, parser.Position - 1, "\"<\" can only compare Numers.");

                        parser.ReadWhitespace();
                        lastType = ScriptType.Bool;
                        goto calcStart;
                    }
                }
                else if (c == '>')
                {
                    if (parser.PeekChar() == '=')
                    {
                        if (logicLevel >= 2)
                            break;
                        parser.Position += 2;
                        var s = ParseStack(parser, true, 2);
                        if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                            functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject >= (double)p, s));
                        else
                            throw new ScriptException(parser, parser.Position - 1, "\">=\" can only compare Numers.");

                        parser.ReadWhitespace();
                        lastType = ScriptType.Bool;
                        goto calcStart;
                    }
                    else
                    {
                        if (logicLevel >= 2)
                            break;
                        parser.Position += 1;
                        var s = ParseStack(parser, true, 2);
                        if (lastType == ScriptType.Number && s.ReturnType == ScriptType.Number)
                            functions.Add(new ParamLambda((v, p) => v.CurrentObject = (double)v.CurrentObject > (double)p, s));
                        else
                            throw new ScriptException(parser, parser.Position - 1, "\">\" can only compare Numers.");
                        parser.ReadWhitespace();
                        lastType = ScriptType.Bool;
                        goto calcStart;
                    }
                }
                else if (c == '=' && parser.PeekChar() == '=')
                {
                    if (logicLevel >= 2)
                        break;
                    parser.Position += 2;
                    var s = ParseStack(parser, true, 2);
                    if ((lastType == ScriptType.String || lastType == ScriptType.Number || lastType == ScriptType.Bool) &&
                        (s.ReturnType == ScriptType.String || s.ReturnType == ScriptType.Number || s.ReturnType == ScriptType.Bool))
                        functions.Add(new ParamLambda((v, p) => v.CurrentObject = v.CurrentObject.Equals(p), s));
                    else
                        throw new ScriptException(parser, parser.Position - 1, "\"==\" can only compare Bools, Strings and Number.");

                    parser.ReadWhitespace();
                    lastType = ScriptType.Bool;
                    goto calcStart;
                }
                else if (c == '!' && parser.PeekChar() == '=')
                {
                    if (logicLevel >= 2)
                        break;
                    parser.Position += 2;
                    var s = ParseStack(parser, true, 2);
                    if ((lastType == ScriptType.String || lastType == ScriptType.Number || lastType == ScriptType.Bool) &&
                         (s.ReturnType == ScriptType.String || s.ReturnType == ScriptType.Number || s.ReturnType == ScriptType.Bool))
                        functions.Add(new ParamLambda((v, p) => v.CurrentObject = !v.CurrentObject.Equals(p), s));
                    else
                        throw new ScriptException(parser, parser.Position - 1, "\"!=\" can only compare Bools, Strings and Number.");

                    parser.ReadWhitespace();
                    lastType = ScriptType.Bool;
                    goto calcStart;
                }
                else if ((c == '&' && parser.PeekChar() == '&') || (c == 'A' && parser.PeekIs("AND")))
                {
                    if (logicLevel >= 1)
                        break;
                    parser.Position += c == 'A' ? 3 : 2;
                    var s = ParseStack(parser, true, 1);
                    if (lastType == ScriptType.Bool && s.ReturnType == ScriptType.Bool)
                        functions.Add(new ShortCircuitAnd(s));
                    else
                        throw new ScriptException(parser, parser.Position - 1, "\"&&\" can only compare Bools.");

                    parser.ReadWhitespace();
                    lastType = ScriptType.Bool;
                    goto calcStart;
                }
                else if ((c == '|' && parser.PeekChar() == '|') || (c == 'O' && parser.PeekChar() == 'R'))
                {
                    if (logicLevel >= 1)
                        break;
                    parser.Position += 2;
                    var s = ParseStack(parser, true, 1);
                    if (lastType == ScriptType.Bool && s.ReturnType == ScriptType.Bool)
                        functions.Add(new ShortCircuitOr(s));
                    else
                        throw new ScriptException(parser, parser.Position - 1, "\"||\" can only compare Bools.");

                    parser.ReadWhitespace();
                    lastType = ScriptType.Bool;
                    goto calcStart;
                }
                else if (c == '.')
                    parser.Position++;
                else if (inParameter)
                {
                    break;
                }
                else
                {
                    if (c == ';')
                    {
                        parser.Position++;
                        parser.ReadWhitespace();
                        if (parser.Ended())
                            break;
                        c = parser.CurrentChar;
                    }

                    if (variablesToBeAssigned != null)
                    {
                        while (variablesToBeAssigned.Count > 0)
                            functions.Add(new SetVariable(variablesToBeAssigned.Pop(), this, lastType));
                    }
                    variablesToBeAssigned = null;

                    if (c == '}')
                    {
                        parser.Position++;
                        break;
                    }
                    else
                    {
                        lastType = ScriptType.Static;
                    }
                }
                //else if (c == ')' || (inParameter && c == ','))
                //{
                //    break;
                //}
            }

            if (variablesToBeAssigned != null)
            {
                while (variablesToBeAssigned.Count > 0)
                    functions.Add(new SetVariable(variablesToBeAssigned.Pop(), this, lastType));
            }

            stack.ReturnType = lastType;
            stack.Functions = functions.ToArray();
            parser.ReadWhitespace();
            return stack;
        }

        // throws an exception when an unexpectet char is encountered
        void unexpected()
        {
            unexpected(Parser.Position);
        }

        void unexpected(int position)
        {
            throw new UnexpectedCharException(position, Parser);
        }

        // throws an exception if the end of the script was reached unexpectedly
        void ended()
        {
            throw new UnexpectedEndException(Parser.Text.Length - 1, Parser);
        }
    }
}
