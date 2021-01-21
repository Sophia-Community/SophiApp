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
        
        internal bool TestsResult { get; private set; } = false;

        internal string TestsResultText { get; private set; } = string.Empty;

        internal void TestsRun()
        {
            TestsResultEventArgs args = new TestsResultEventArgs();

            foreach (var test in tests)
            {
                args.Text = test.ResultText;
                OnResultTextChanged(args);
                test.Run();
                TestsResult = test.Result;                

                if (TestsResult == false)
                {
                    TestsResultText = test.ResultText;
                    args.Text = TestsResultText;
                    OnResultTextChanged(args);
                    break;
                }                   
            }
        }        
    }
}
