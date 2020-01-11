using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AdvancedConsoleViewNS
{
    public partial class Form1 : Form, IAdvancedConsoleOwner
    {
        public Form1()
        {
            InitializeComponent();

            advancedConsoleView1.PromptInfo = string.Format("{0}:~ {1}$:", System.Environment.MachineName, System.Environment.UserName);
            advancedConsoleView1.WriteLine("Advanced Console View By Hielke Hielkema.");
            advancedConsoleView1.WriteLine();
            advancedConsoleView1.ConsoleStrollBar = vScrollBar1;

            // Add some items to test
            advancedConsoleView1.Suggestions.AddRange(new List<string>() {
                "system.test.a",
                "system.memory.dump();",
                "system.test();",
                "system.core.insert(",
                "system.core.kernel.run",
                "system.core.kernel.su.login(",
                "system.memory.clear();",
                "system.memory.release(",
                "system.memory.info();",
                "system.core.kernel.su.login(",
                "system.core.kernel.su.login(",
                "system.core.kernel.su.abc(",
                "system.core.kernel.su.def("
            });
        }

        #region IAdvancedConsoleOwner Members
     
        void IAdvancedConsoleOwner.ProcessInput(AdvancedConsoleView sender, string input)
        {
            // Process console input here
        }

        #endregion
    }
}
