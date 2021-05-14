using SophiApp.Commons;
using System;
using System.Linq;

namespace SophiApp.Models
{
    internal class RadioButtonGroup : BaseContainer
    {
        public RadioButtonGroup(JsonDTO json) : base(json)
        {
        }

        public delegate void RadioButtonGroupErrorOccurred(uint id, string target, string message);

        public event RadioButtonGroupErrorOccurred ErrorOccurred;

        internal uint DefaultSelectedId { get; private set; } = default;
        internal bool IsSelected { get; set; } = false;

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