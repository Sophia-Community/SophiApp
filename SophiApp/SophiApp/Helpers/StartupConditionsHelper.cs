using SophiApp.Conditions;
using SophiApp.Interfaces;
using SophiApp.StartupConditions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class StartupConditionsHelper
    {
        private List<IStartupCondition> Conditions;

        public StartupConditionsHelper(EventHandler<Exception> errorHandler, EventHandler<IStartupCondition> resultHandler)
        {
            Initializing();
            ErrorOccurred += errorHandler;
            ConditionHasProblem += resultHandler;
        }

        public event EventHandler<IStartupCondition> ConditionHasProblem;

        public event EventHandler<Exception> ErrorOccurred;

        public bool HasProblem { get; set; }

        private void Initializing() => Conditions = new List<IStartupCondition>()
        {
            new OsVersionCondition(), new OsBuildVersionCondition(), new RebootRequiredCondition(),
            new SingleInstanceCondition(), new SingleAdminSessionCondition(), new Win10TweakerCondition(),
            new SycnexScriptCondition(), new DefenderWarningCondition(), new Win10TweakerDefenderBroken(),
            //new NewVersionCondition()
        };

        internal async Task CheckAsync()
        {
            await Task.Run(() =>
            {
                foreach (var condition in Conditions)
                {
                    try
                    {
                        HasProblem = condition.Invoke();

                        if (HasProblem)
                        {
                            ConditionHasProblem?.Invoke(null, condition);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        HasProblem = true;
                        ErrorOccurred.Invoke(this, e);
                        break;
                    }
                }
            });
        }
    }
}