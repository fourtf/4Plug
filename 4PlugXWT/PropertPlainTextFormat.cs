using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xwt.Backends;
using Xwt.Formats;

namespace FPlug
{
    public class ProperPlainTextFormat : TextFormat
    {
        public static readonly ProperPlainTextFormat Proper = new ProperPlainTextFormat();

        public override void Parse(Stream input, IRichTextBuffer buffer)
        {
            using (var reader = new StreamReader(input))
            {
                buffer.EmitStartParagraph(0);
                string s = null;
                while ((s = reader.ReadLine()) != null)
                {
                    buffer.EmitText(s, RichTextInlineStyle.Normal);
                    buffer.EmitText(Environment.NewLine, RichTextInlineStyle.Normal);
                }
                buffer.EmitEndParagraph();
            }
        }
    }
}
