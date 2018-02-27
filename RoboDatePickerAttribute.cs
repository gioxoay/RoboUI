namespace RoboUI
{
    public class RoboDatePickerAttribute : RoboControlAttribute
    {
        public RoboDatePickerAttribute(bool isDateTimePicker = false)
        {
            DateFormat = isDateTimePicker ? Constants.FullDatePattern : Constants.ShortDatePattern;
            DateTimePicker = isDateTimePicker;
        }

        public bool DateTimePicker { get; set; }

        /// <summary>
        /// The format for parsed and displayed date.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Sort Required
        /// </summary>
        /// <returns></returns>
        public bool EnableSortRequired { get; set; }

        /// <summary>
        /// The property name of end date range
        /// </summary>
        public string EndDateRange { get; set; }

        /// <summary>
        /// The maximum value.
        /// </summary>
        public string MaximumValue { get; set; }

        /// <summary>
        /// The minimum value.
        /// </summary>
        public string MinimumValue { get; set; }

        /// <summary>
        /// The property name of start date range
        /// </summary>
        public string StartDateRange { get; set; }

        //TODO: rename this.. it's very bad
        public string ToChildrenDate { get; set; }
    }
}
