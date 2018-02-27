using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboUI
{
    public class RoboUICombinedResult : RoboUIResult
    {
        private readonly ICollection<RoboUIResult> results;

        public RoboUICombinedResult()
        {
            results = new List<RoboUIResult>();
        }

        public RoboUICombinedResult(params RoboUIResult[] results)
        {
            this.results = new List<RoboUIResult>(results);
        }

        public void Add(RoboUIResult result)
        {
            results.Add(result);
        }

        public override async Task<bool> OverrideExecuteResult()
        {
            foreach (var result in results)
            {
                if (result != null)
                {
                    if (await result.OverrideExecuteResult())
                    {
                        return true;
                    }
                }
            }

            return await base.OverrideExecuteResult();
        }

        public override string GenerateView()
        {
            return string.Join("", results.Where(x => x != null).Select(f => f.GenerateView()));
        }
    }
}
