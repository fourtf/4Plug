using FPlug.Options.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPlug.Options
{
    public class Script
    {
        public SettingsWindow Window { get; private set; }




        //  static
        public static Task InitializeOptionsTask;

        public static void InitializeScriptingAsync()
        {
            InitializeOptionsTask = new Task(() =>
            {
                Control.InitializeControls();
            });

            InitializeOptionsTask.Start();
        }

        public static void InitializeScripting()
        {
            Control.InitializeControls();
        }
    }
}
