using SophiApp.Commons;

namespace SophiApp.Interfaces
{
    internal interface IStartupCondition
    {
        bool HasProblem { get; set; }
        ConditionsTag Tag { get; set; }

        bool Invoke();
    }
}