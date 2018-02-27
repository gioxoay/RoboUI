using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RoboUI
{
    public enum RoboChoiceType
    {
        CheckBox,
        CheckBoxList,
        DropDownList,
        RadioButtonList,
    }

    public class RoboChoiceAttribute : RoboControlAttribute
    {
        private readonly RoboChoiceType type;

        public RoboChoiceAttribute(RoboChoiceType type)
        {
            this.type = type;
        }

        public bool AllowMultiple { get; set; }

        public int Columns { get; set; }

        public bool GroupedByCategory { get; set; }

        public override bool HideLabelControl
        {
            get => type == RoboChoiceType.CheckBox || base.HideLabelControl;
            set => base.HideLabelControl = value;
        }

        public string OnSelectedIndexChanged { get; set; }

        public string OptionLabel { get; set; }

        public bool RequiredIfHaveItemsOnly { get; set; }

        public bool InlineControls { get; set; }

        public IEnumerable<SelectListItem> SelectListItems { get; set; }

        public RoboChoiceType Type => type;
    }
}
