namespace RoboUI
{
    public class RoboComplexAttribute : RoboControlAttribute
    {
        public RoboComplexAttribute()
        {
            Column = 1;
            EnableGrid = false;
        }

        public override bool HasLabelControl => false;

        public int Column { get; set; }

        public bool EnableGrid { get; set; }
    }
}
