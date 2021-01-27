using SophiAppCE.EventsArgs;
using SophiAppCE.Interfaces;
using SophiAppCE.Requirements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Helpers
{
    internal class RequirementsHelper
    {
        private readonly List<IRequirementTest> tests = new List<IRequirementTest>()
        {
            new WindowsVersionTest()
        };

        public event EventHandler<TestsResultEventArgs> ResultTextChanged;

        protected void OnResultTextChanged(TestsResultEventArgs e) => (ResultTextChanged)?.Invoke(this, e);
        
        internal bool Result { get; private set; } = false;

        internal string Text { get; private set; } = string.Empty;

        internal void Run()
        {
            TestsResultEventArgs args = new TestsResultEventArgs();

            foreach (var test in tests)
            {
                args.Text = test.Name;
                OnResultTextChanged(args);
                test.Run();
                Result = test.Result;                

                if (Result == false)
                {
                    Text = test.Error;                    
                    break;
                }                   
            }
        }        
    }
}
