using FPlug.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Formats;

namespace FPlug.Scripting
{
    public class ErrorWindow : Window
    {
        DataField<int> errorIndex = new DataField<int>();
        DataField<string> errorField = new DataField<string>();
        //DataField<int> xField = new DataField<int>();
        //DataField<int> yField = new DataField<int>();
        DataField<string> errorLevelField = new DataField<string>();

        ListStore listStore;
        ListView listView;

        public ErrorWindow()
        {
            Title = "Errors";

            this.Icon = App.Icon;
            this.

            listStore = new ListStore(errorIndex, errorLevelField, errorField /*, xField, yField*/);
            listView = new ListView(listStore);

            listView.Columns.Add("#", errorIndex);
            listView.Columns.Add("Type", errorLevelField);
            listView.Columns.Add("Message", errorField);
            //listView.Columns.Add("Line", xField);
            //listView.Columns.Add("Column", yField);

            Content = listView;

            Size = new Size(500, 150);
        }

        public Boolean FINISHHIM = false;

        protected override bool OnCloseRequested()
        {
            return FINISHHIM;
        }

        int currentindex = 0;

        public void Log(string text, ErrorType level)
        {
            var index = listStore.AddRow();
            listStore.SetValue(index, errorIndex, currentindex++);
            listStore.SetValue(index, errorField, text);
            listStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void Log(string text, int line, int column, ErrorType level)
        {
            var index = listStore.AddRow();
            listStore.SetValue(index, errorIndex, currentindex++); //.ToString().PadLeft(3, '0')
            listStore.SetValue(index, errorField, "Line " + line + ", Char " + column + Environment.NewLine + text);
            //listStore.SetValues(index, xField, line, yField, column);
            listStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void ClearLogs()
        {
            listStore.Clear();
            currentindex = 0;
        }
    }
}
