using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    [ScriptClass("FavoriteServer")]
    public class SFavouriteServer : SControlCollection
    {
        [ScriptMember(Scripting.ScriptTypeID.String)]
        public string Title
        {
            get { return box.Text; }
            set { box.Text = value; }
        }

        [ScriptMember(Scripting.ScriptTypeID.String)]
        public string Text
        {
            get
            {
                return text.Text;
            }
            set
            {
                text.Text = value;
            }
        }

        [ScriptMember(Scripting.ScriptTypeID.String)]
        public string ServerIP
        {
            get
            {
                return ip.Text;
            }
            set
            {
                ip.Text = value;
            }
        }

        [ScriptMember(Scripting.ScriptTypeID.String)]
        public string Password
        {
            get
            {
                return password.Text;
            }
            set
            {
                password.Text = value;
            }
        }

        private bool suppressCode;

        [ScriptMember(Scripting.ScriptTypeID.Boolean)]
        public bool SuppressCode
        {
            get { return suppressCode; }
            set { suppressCode = value; }
        }

        STextInput text;
        STextInput ip;
        STextInput password;
        SLabel lbl1;
        SLabel lbl2;
        SLabel lbl3;

        public override void Layout(double width)
        {
            lbl1.WidthPercentage = lbl2.WidthPercentage = lbl3.WidthPercentage = 80 / width;
            base.Layout(width);
        }

        public SFavouriteServer()
        {
            //box.Title = "SFavouriteServer";
            box
                .AddChild(lbl1 = new SLabel() { Text = "Text:" })
                .AddChild(text = new STextInput())
                .AddChild(lbl2 = new SLabel() { Text = "Server IP:" })
                .AddChild(ip = new STextInput())
                .AddChild(lbl3 = new SLabel() { Text = "Password:" })
                .AddChild(password = new STextInput())
                ;
        }
    }
}
