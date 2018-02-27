using System.Collections.Generic;

namespace RoboUI
{
    public class RoboUITabbedLayout<TModel>
    {
        private readonly IList<RoboUIGroupedLayout<TModel>> groups;

        public RoboUITabbedLayout(string title)
        {
            Title = title;
            groups = new List<RoboUIGroupedLayout<TModel>>();
        }

        public IList<RoboUIGroupedLayout<TModel>> Groups => groups;

        public string Title { get; set; }

        public RoboUIGroupedLayout<TModel> AddGroup(string title = null)
        {
            var group = new RoboUIGroupedLayout<TModel>(title);
            groups.Add(group);
            return group;
        }
    }
}
