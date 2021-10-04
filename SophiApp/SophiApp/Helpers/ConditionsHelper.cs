using SophiApp.Conditions;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class ConditionsHelper
    {
        private List<ICondition> ConditionsList;

        public ConditionsHelper(EventHandler<Exception> errorHandler, EventHandler<ICondition> resultHandler)
        {
            InitializingConditions();
            ErrorOccurred += errorHandler;
            ConditionResult += resultHandler;
        }

        public event EventHandler<ICondition> ConditionResult;

        public event EventHandler<Exception> ErrorOccurred;

        public bool Result { get; set; } = false;

        private void InitializingConditions() => ConditionsList = new List<ICondition>()
        {
            new OsBuildVersion(), new OsUpdateBuildRevision(),
            new LoggedUserIsAdmin(), new OsNotInfected(),
            new NoNewVersion()
        };

        internal async Task InvokeAsync()
        {
            await Task.Run(() =>
            {
                foreach (var condition in ConditionsList)
                {
                    try
                    {
                        Result = condition.Invoke();
                        ConditionResult?.Invoke(null, condition);

                        if (Result.Invert())
                            break;
                    }
                    catch (Exception e)
                    {
                        Result = false;
                        ErrorOccurred.Invoke(this, e);
                        break;
                    }
                }
            });
        }
    }
}