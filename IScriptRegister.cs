using System;
using System.Collections.Generic;
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
        public void AddInlineScript(string script)
        {
            throw new NotImplementedException();
        }

        public void AddExternalScript(string src)
        {
            throw new NotImplementedException();
        }

        public IHtmlContent RenderScripts()
        {
            throw new NotImplementedException();
        }
    }
}
