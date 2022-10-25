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
            new OsVersionCondition(), new OsBuildVersionCondition(), new OsFilesCorruptedCondition(), new RebootRequiredCondition(),
            new SingleInstanceCondition(), new SingleAdminSessionCondition(), new Win10TweakerCondition(), new SycnexScriptCondition(),
            new DefenderCorruptedCondition(), new NewVersionCondition()
        };

        internal async Task CheckAsync()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < Conditions.Count; i++)
                {
                    try
                    {
                        var isLastCondition = Conditions.Count - i == 1;
                        HasProblem = Conditions[i].Invoke();
                        DebugHelper.StartupConditionInvoked(name: Conditions[i].GetType().Name, result: HasProblem.Invert());
                        DebugHelper.NextStartupCondition(name: isLastCondition ? Conditions[i].GetType().Name : Conditions[i + 1].GetType().Name, isLast: isLastCondition);

                        if (HasProblem)
                        {
                            ConditionHasProblem?.Invoke(null, Conditions[i]);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        HasProblem = true;
                        ErrorOccurred?.Invoke(this, e);
                        break;
                    }
                }
            });
        }
    }
}