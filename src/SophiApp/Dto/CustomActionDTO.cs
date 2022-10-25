using System;

namespace SophiApp.Dto
{
    public class CustomActionDto
    {
        public CustomActionDto(uint id, Action<bool> action, bool parameter)
        {
            Id = id;
            Action = action;
            Parameter = parameter;
        }

        public Action<bool> Action { get; set; }
        public uint Id { get; set; }
        public bool Parameter { get; set; }

        internal void Invoke() => Action.Invoke(Parameter);
    }
}