using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Scripting2
{
    public class Script
    {
        Dictionary<string, Variable> GlobalVariables = new Dictionary<string, Variable>();

        string script = "";

        // ctor
        public Script(string script)
        {
            this.script = script;
        }

        // compiling
        public bool IsCompiled { get; private set; }

        public void Compile()
        {
            //check if already compiled
            if (IsCompiled)
                throw new InvalidOperationException("The Script is already compiled");

            StringParser parser = new StringParser(script);
            int position = 0;

            parser.ReadWhitespace();

            position = parser.Position;
            var token = parser.ReadToken();

            // lowest level
            if (token == "var")
            {

            }
            else if (token == "on")
            {

            }
            else
            {

            }


            // set compiled to true
            IsCompiled = true;
        }

        private void invalidToken()
        {

        }
    }
}
