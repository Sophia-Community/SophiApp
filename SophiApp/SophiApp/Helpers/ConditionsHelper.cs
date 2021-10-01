using SophiApp.Conditions;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class ConditionsHelper
    {
        public bool Result { get; set; } = false;

        public event EventHandler<ICondition> ConditionResult;

        public event EventHandler<Exception> ErrorOccurred;

        private List<ICondition> conditions;

        public ConditionsHelper(EventHandler<Exception> errorHandler, EventHandler<ICondition> resultHandler)
        {
            InitConditionsList();
            ErrorOccurred += errorHandler;
            ConditionResult += resultHandler;
        }

        private void InitConditionsList() => conditions = new List<ICondition>()
        {
            new OsBitness(), new OsBuildVersion(),
        };

        internal async Task InvokeAsync()
        {
            await Task.Run(() =>
            {
                var result = false;

                foreach (var condition in conditions)
                {
                    try
                    {
                        result = condition.Invoke();
                        ConditionResult?.Invoke(null, condition);
                        Result = result;

                        if (Result.Invert())
                            break;

                    }
                    catch (Exception e)
                    {
                        ErrorOccurred.Invoke(this, e);
                    }
                }
            });
        }

    }
}
