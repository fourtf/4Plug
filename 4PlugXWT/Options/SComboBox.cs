using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    [ScriptClass("Combo")]
    public class SComboBox : SSingleWidget<ComboBox>
    {
        [ScriptMember(ScriptTypeID.String)]
        public string Selected
        {
            get { return control.SelectedText; }
            set
            {
                control.SelectedText = value;
                if (_items.Length > 0)
                {
                    foreach (string s in _items)
                    {
                        if (string.Equals(s, value, StringComparison.OrdinalIgnoreCase))
                            control.SelectedItem = s;
                    }
                }
            }
        }

        string items = "";
        string[] _items = new string[0];

        [ScriptMember(ScriptTypeID.String)]
        public string Items
        {
            get { return items; }
            set
            {
                items = value;
                _items = Util.SplitAndTrimEscaped(items, '\\', ',');
                control.Items.Clear();

                foreach (string s in _items)
                    control.Items.Add(s);

                if (_items.Length > 0)
                    control.SelectedIndex = 0;
            }
        }

        private string source;
        Source _source = null;

        [ScriptMember(ScriptTypeID.String)]
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                if (source != value)
                {
                    source = value;

                    _source = new Source(Window, source);
                    var c = _source.GetValue();
                    if (c != null)
                        Selected = c;
                    var I = _source.GetValues();
                    if (I != null)
                    {
                        _items = I.ToArray();
                        items = string.Join(",", I.Select((s) => s.Replace(",", "\\,")));
                        control.Items.Clear();
                        I.ForEach(s => control.Items.Add(s));
                    }
                }
            }
        }

        [ScriptMember(ScriptTypeID.Boolean)]
        public bool SaveSource { get; set; }

        [ScriptEvent]
        public Script OnValueChanged { get; set; }

        public SComboBox()
        {
            //Height += 2;
            PaddingBottom = 2;

            SaveSource = true;

            control.SelectionChanged += (s, e) => { if (OnValueChanged != null) OnValueChanged.Execute(); };
        }

        public override void Apply()
        {
            base.Apply();

            if (_source != null && control.Items.Count > 0)
                _source.SetValue((string)control.SelectedItem);
        }

        public override void CompileScripts()
        {
            base.CompileScripts();

            if (OnValueChanged != null)
                OnValueChanged.Compile();
        }
    }
}
