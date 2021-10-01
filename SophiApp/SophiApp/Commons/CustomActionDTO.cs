using System;

namespace SophiApp.Commons
{
    public class CustomActionDto
    {
        public Action<bool> Action { get; set; }
        public uint Id { get; set; }
        public bool Parameter { get; set; }

        internal void Invoke() => Action.Invoke(Parameter);
    }
}