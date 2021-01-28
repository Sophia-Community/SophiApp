using SophiAppCE.EventsArgs;
using SophiAppCE.Interfaces;
using SophiAppCE.Requirements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Helpers
{
    internal class RequirementsHelper
    {
        private string text = string.Empty;
        private readonly List<IRequirementTest> tests = new List<IRequirementTest>()
        {
            new WindowsVersionTest()
        };

        internal EventHandler<TestsResultEventArgs> OnTextChanged
        {
            get { return null; }
            set { TextChanged += value; }
        }

        private event EventHandler<TestsResultEventArgs> TextChanged;

        internal bool Result { get; private set; }

        internal string Text
        {
            get => text;
            private set
            {
                text = value;
                TextChanged?.Invoke(this, new TestsResultEventArgs(text));
            }
        }

        internal string ErrorDescription { get; set; }

        public string ErrorUrl { get; set; }

        internal void Run()
        {
            foreach (var test in tests)
            {
                Text = test.Name;
                Thread.Sleep(3000);
                test.Run();
                Result = test.Result;                

                //HACK: !!!
                if (Result == true)
                {
                    ErrorDescription = test.ErrorDescription;
                    ErrorUrl = test.ErrorUrl;
                    break;
                }                   
            }
        }
    }
}
