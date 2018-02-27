namespace RoboUI
{
    public class RoboUIContentResult : RoboUIResult
    {
        private readonly string content;

        public RoboUIContentResult(string content)
        {
            this.content = content;
        }

        public override string GenerateView()
        {
            return content;
        }
    }
}
