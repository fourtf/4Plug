using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options
{
    public class SChild : Canvas
    {
        public static long cID = 0;

        [ScriptMember(Scripting.ScriptTypeID.String, AutoAdd = false)]
        public string ID { get; set; }

        private double widthPercentage;

        [ScriptMember(Scripting.ScriptTypeID.Number, Name="Width", AutoAdd=false)]
        public double WidthPercentage
        {
            get
            {
                return widthPercentage;
            }
            set
            {
                value = Math.Max(Math.Min(value, 1d), 0);
                widthPercentage = value;
            }
        }

        public ScriptType ScriptType { get; private set; }

        //public SettingsWidgetTreeNode LinkedNode = null;

        public new SContainer Parent = null;
        public SettingsWindow Window = null;

        public virtual void Apply()
        {
            //if (OnApply != null)
            //    OnApply.Execute();
        }

        public void ExecApplyScript()
        {
            if (OnApply != null)
                OnApply.Execute();
        }
        
        //public virtual void MouseEnter()
        //{
        //    //if (OnMouseEnter != null)
        //    //    OnMouseEnter.Execute();
        //}
        
        public void ExecMouseEnterScript()
        {
            if (OnMouseEnter != null)
                OnMouseEnter.Execute();
        }
        
        //public virtual void MouseLeave()
        //{
        //    //if (OnMouseLeave != null)
        //    //    OnMouseLeave.Execute();
        //}
        
        public void ExecMouseLeaveScript()
        {
            if (OnMouseLeave != null)
                OnMouseLeave.Execute();
        }

        [ScriptEvent] public Script OnApply { get; set; }
        [ScriptEvent] public Script OnMouseEnter { get; set; }
        [ScriptEvent] public Script OnMouseLeave { get; set; }

        public string LegacyName { get; set; }

        public double Height = 100;
        public double Width = 100;

        public SChild()
        {
            this.ID = cID++.ToString();
            WidthPercentage = 0;

            this.BackgroundColor = Colors.Transparent;

           ScriptType = App.ScriptTypeByType[GetType()];
        }

        protected override void OnMouseEntered(EventArgs args)
        {
            base.OnMouseEntered(args);
        
            ExecMouseEnterScript();
        }
        
        protected override void OnMouseExited(EventArgs args)
        {
            base.OnMouseExited(args);
        
            ExecMouseLeaveScript();
        }

        public virtual void Layout(double width)
        {
            this.Width = width;
        }

        public override string ToString()
        {
            return GetType().ToString() + "(" + ID + ")" + " " + Width + "x" + Height;
        }

        public virtual void CompileScripts()
        {
            if (OnApply != null)
                OnApply.Compile();
            if (OnMouseEnter != null)
                OnMouseEnter.Compile();
            if (OnMouseLeave != null)
                OnMouseLeave.Compile();
        }

        public virtual void RenewSources()
        {

        }
    }
}
