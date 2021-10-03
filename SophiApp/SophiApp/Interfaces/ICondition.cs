namespace SophiApp.Interfaces
{
    internal interface ICondition
    {
        bool Result { get; set; }
        string Tag { get; set; }

        bool Invoke();
    }
}