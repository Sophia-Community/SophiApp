using SophiApp.Conditions;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class ConditionsHelper
    {
        private List<ICondition> Conditions;

        public ConditionsHelper(EventHandler<Exception> errorHandler, EventHandler<ICondition> resultHandler)
        {
            InitializingConditions();
            ErrorOccurred += errorHandler;
            ConditionResult += resultHandler;
        }

        public event EventHandler<ICondition> ConditionResult;

        public event EventHandler<Exception> ErrorOccurred;

        public bool Result { get; set; } = false;

        private void InitializingConditions() => Conditions = new List<ICondition>()
        {
            new OsBuildVersion(), new OsUpdateBuildRevision(), new OneInstanceOnly(), new IsSingleSession(),
            new LoggedUserAdmin(), new OsNotInfected(), new Windows10DebloaterNotUsed(),
            //new NoNewVersion()
        };

        internal async Task InvokeAsync()
        {
            await Task.Run(() =>
            {
                foreach (var condition in Conditions)
                {
                    try
                    {
                        Result = condition.Invoke();
                        ConditionResult?.Invoke(null, condition);

                        if (Result == false)
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