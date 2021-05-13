using SophiApp.Commons;
using System;
using System.Linq;

namespace SophiApp.Models
{
    internal class RadioButtonGroup : BaseContainer
    {
        internal bool IsSelected { get; set; } = false;
        internal uint DefaultSelectedId { get; private set; } = default;

        public delegate void RadioButtonGroupErrorOccurred(uint id, string target, string message);

        public event RadioButtonGroupErrorOccurred ErrorOccurred;

        public RadioButtonGroup(JsonDTO json) : base(json)
        {
        }

        internal void SetDefaultSelectedId()
        {
            try
            {
                DefaultSelectedId = Collection.Where(element => element.State == UIElementState.CHECKED).First().Id;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(Id, e.TargetSite.Name, e.Message);
            }
        }
    }
}