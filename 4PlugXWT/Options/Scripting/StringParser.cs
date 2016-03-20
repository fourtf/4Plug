using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FPlug.Options.Scripting
{
    public class StringParser
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string TextUntilNow
        {
            get
            {
                return Text.Remove(position);
            }
        }

        private int position;

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public int[] NewLines { get; private set; }

        public Coordinate TextOffset = new Coordinate(1, 1);

        public StringParser(string text)
        {
            this.text = text;
            position = 0;

            List<int> newLines = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r')
                {
                    if (text.Length > i && text[i + 1] == '\n')
                        newLines.Add(++i + 1);
                    else
                        newLines.Add(i + 1);
                }
                else if (text[i] == '\n')
                {
                    newLines.Add(i + 1);
                }
            }

            NewLines = newLines.ToArray();
        }

        public Coordinate GetTextLocation(int position)
        {
            int line = 0;
            int column = 0;

            for (int i = 0; i < NewLines.Length; i++)
            {
                if (position > NewLines[i])
                {
                    line = i + 1;
                    column = position - NewLines[i];
                }
                else
                    break;
            }

            if (line != 0)
            {
                int clmn = 0;
                for (int i = NewLines[line - 1]; i < position; i++)
                {
                    if (text[i] == '\t')
                        clmn = (clmn + 1) / 4 * 4 + 4;
                    else
                        clmn++;
                }
                column = clmn;
            }

            return new Coordinate(line + TextOffset.Line, line == 0 ? column + TextOffset.Char : column + 1);
        }

        public string GetString(int start, int end)
        {
            return text.Substring(start, end - start);
        }

        public void ReadWhitespace()
        {
            int i = position;
            string s = text;
        start:
            for (; i < s.Length; i++)
            {
                if (s[i] > ' ')
                    break;
            }

            if (i < s.Length - 1 && s[i] == '/')
            {
                if (s[i + 1] == '/')
                {
                    i += 2;
                    for (; i < s.Length; i++)
                    {
                        if (s[i] == '\n' || s[i] == '\r')
                            break;
                    }
                    goto start;
                }
                else if (s[i + 1] == '*')
                {
                    i += 2;
                    for (; i < s.Length; i++)
                    {
                        if (s[i] == '*' && i < s.Length - 1 && s[i + 1] == '/')
                        {
                            i += 2;
                            position = i;
                            break;
                        }
                    }
                    goto start;
                }
            }
            position = i;
        }

        public bool ReadWhitespaceInThisLine()
        {
            int i = position;
            string s = text;
            bool returnValue = false;

        start:
            for (; i < s.Length; i++)
            {
                char c = s[i];

                if (c == '\r' || c == '\n')
                    returnValue = true;

                if (c > ' ')
                    break;
            }

            if (i < s.Length - 1 && s[i] == '/')
            {
                if (s[i + 1] == '/')
                {
                    i += 2;
                    for (; i < s.Length; i++)
                    {
                        if (s[i] == '\n' || s[i] == '\r')
                        {
                            returnValue = true;
                            break;
                        }
                    }
                    goto start;
                }
                else if (s[i + 1] == '*')
                {
                    i += 2;
                    for (; i < s.Length; i++)
                    {
                        char c = s[i];
                        if (c == '\r' || c == '\n')
                            returnValue = true;

                        if (c == '*' && i < s.Length - 1 && s[i + 1] == '/')
                        {
                            i += 2;
                            position = i;
                            break;
                        }
                    }
                    goto start;
                }
            }
            position = i;
            return returnValue;
        }

        public void ReadWhitespaceInThisLineOrThrow()
        {
            int pos = position;
            if (ReadWhitespaceInThisLine())
                ScriptException.ThrowUnexpectedEndOfLineException(this, pos);
        }

        //public void ReadName2()
        //{
        //    int i = position;
        //    string s = text;
        //    for (; i < s.Length; i++)
        //    {
        //        char c;
        //        if (
        //            ((c = s[i]) >= 'a' && c <= 'z') ||
        //            (c >= 'A' && c <= 'Z') ||
        //            (c >= '0' && c <= '9') ||
        //            (c == '_')
        //            )
        //            return;
        //    }
        //    position = i;
        //}

        public string ReadToken()
        {
            int start = position, i = position;
            string s = text;
            for (; i < s.Length; i++)
            {
                char c;
                if (
                    ((c = s[i]) < 'a' || c > 'z') &&
                    (c < 'A' || c > 'Z') &&
                    (c < '0' || c > '9') &&
                    (c != '_')
                    )
                    break;
            }
            position = i;
            return text.Substring(start, i - start);
        }

        public string ReadTokenOrThrow()
        {
            int pos = position;

            string token = ReadToken();

            if (token == "")
                throw new ScriptException(this, position, "Not a valid token!");

            return token;
        }

        public string ReadOperator()
        {
            int start = position, i = position;
            string s = text;
            for (; i < s.Length; i++)
            {
                char c;
                if (
                    ((c = s[i]) < '!' || c > '/') &&
                    (c < '{' || c > '~') &&
                    (c < '[' || c > '^')
                    )
                    break;
            }
            position = i;
            return text.Substring(start, i - start);
        }

        public string ReadEscaped(char endChar)
        {
            StringBuilder builder = new StringBuilder(32);
            int i = position + 1;
            string s = text;
            for (; i < s.Length; i++)
            {
                if (s[i] == endChar)
                {
                    if (s[i + 1] == endChar)
                    {
                        builder.Append(endChar);
                        i++;
                    }
                    else
                        break;
                }
                else
                    builder.Append(s[i]);
            }
            position = i + 1;
            return builder.ToString();
        }

        public double ReadNumber()
        {
            int start = position, i = position;
            string s = text;
            char c;
            bool hasPoint = false;

            if ((c = s[i]) == '-')
                ++i;

            for (; i < s.Length; i++)
            {
                if (((c = s[i]) < '0' || c > '9') && (c != '.' || !(hasPoint = !hasPoint)))
                    break;
            }
            if (text[i - 1] == '.')
                i--;
            position = i;
            string s2 = text.Substring(start, i - start);
            return double.Parse(s2, CultureInfo.InvariantCulture);
        }

        public void ReadUntil(char c)
        {
            int i = position;
            string s = text;
            for (; i < s.Length; i++)
            {
                if (s[i] == c)
                    break;
            }
            position = i;
        }

        public string ReadUntilAny(params string[] tokens)
        {
            int start = 0, i = position;
            string s = text;
            for (; i < s.Length; i++)
            {
                char c = s[i];
                for (int j = 0; j < tokens.Length; j++)
                {
                    string tkn = tokens[j];
                    if (s.Length - i < tkn.Length)
                        continue;
                    if (c == tkn[0])
                    {
                        start = i;
                        int k = 1;
                        for (; k < tkn.Length; k++)
                        {
                            if (s[k + i] != tkn[k])
                                goto jizz;
                        }
                        position = i + k;
                        return tokens[j];
                    }
                jizz:;
                }
            }
            position = s.Length;
            return null; //text.Substring(start, i - start);
        }

        public void ReadCfgWs()
        {
            int i = position;
            string s = text;
            for (; i < s.Length; i++)
            {
                char c = s[i];
                if (c != ' ' && c != '\t' /*|| c != '\r' || c != '\n'*/)
                    break;
            }
            position = i;
        }

        public string ReadCfgToken()
        {
            int start = position, i = position;
            string s = text;
            for (; i < s.Length; i++)
            {
                char c = s[i];
                if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                    break;
            }
            position = i;
            return text.Substring(start, i - start);
        }

        public void ReadCharWs()
        {
            int i = position;
            string s = text;
            for (; i < s.Length; i++)
            {
                char c = s[i];
                if (!Char.IsWhiteSpace(c))
                    break;
            }
            position = i;
        }

        public string ReadLine()
        {
            int start = position, i = position;
            string s = text;
            int end = s.Length;
            for (; i < s.Length; ++i)
            {
                end = i;
                if (s[i] == '\r')
                {
                    end = i;
                    ++i;
                    if (i < s.Length && s[i] == '\n')
                        ++i;
                    break;
                }
                else if (s[i] == '\n')
                {
                    end = i;
                    ++i;
                    break;
                }
            }
            position = i;
            return text.Substring(start, end - start);
        }

        public char CurrentChar
        {
            get
            {
                return text[position];
            }
        }

        public char PeekChar()
        {
            return text[position + 1];
        }

        public bool PeekIs(string s)
        {
            if (s.Length < text.Length - position)
                for (int x = 0; x < s.Length; x++)
                {
                    if (text[position + x] != s[x])
                        return false;
                }
            return true;
        }

        public bool Ended()
        {
            return position >= text.Length;
        }
    }
}
