using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace RoboUI
{
    public interface IScriptRegister
    {
        void AddInlineScript(string script);

        void AddExternalScript(string src);

        IHtmlContent RenderScripts();
    }

    public class ScriptRegister : IScriptRegister
    {
        private readonly ICollection<ScriptResource> scriptResources;

        public ScriptRegister()
        {
            scriptResources = new List<ScriptResource>();
        }

        public void AddInlineScript(string script)
        {
            scriptResources.Add(new ScriptResource
            {
                Source = script,
                Inline = true
            });
        }

        public void AddExternalScript(string src)
        {
            scriptResources.Add(new ScriptResource
            {
                Source = src,
                Inline = false
            });
        }

        public IHtmlContent RenderScripts()
        {
            if (scriptResources.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();

            foreach (var scriptResource in scriptResources.Where(x => x.Inline == false))
            {
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", scriptResource.Source);
            }

            foreach (var scriptResource in scriptResources.Where(x => x.Inline))
            {
                sb.AppendFormat("<script type=\"text/javascript\">{0}</script>", scriptResource.Source);
            }

            return new HtmlString(sb.ToString());
        }

        private class ScriptResource
        {
            public string Source { get; set; }

            public bool Inline { get; set; }
        }
    }
}
