using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace RoboUI
{
    public abstract class RoboUIResult : ViewResult
    {
        private readonly IDictionary<string, string> hiddenValues;

        protected RoboUIResult()
        {
            hiddenValues = new Dictionary<string, string>();
        }

        public IDictionary<string, string> HiddenValues => hiddenValues;

        public IRoboUIFormProvider RoboUIFormProvider { get; set; }

        public string Title { get; set; }

        public void AddHiddenValue(string name, string value)
        {
            hiddenValues.Add(name, value);
        }

        public override async Task ExecuteResultAsync(ActionContext actionContext)
        {
            if (await OverrideExecuteResult())
            {
                return;
            }

            if (!string.IsNullOrEmpty(Title))
            {
                ViewData["Title"] = Title;
            }

            // Generate Robo Form content
            var roboFormContent = GenerateView();

            var razorViewEngine = (IRazorViewEngine) actionContext.HttpContext.RequestServices.GetService(typeof(IRazorViewEngine));
            var tempDataProvider = (ITempDataProvider) actionContext.HttpContext.RequestServices.GetService(typeof(ITempDataProvider));
            var viewResult = razorViewEngine.FindView(actionContext, "RoboFormResult", false);

            if (viewResult.View == null)
            {
                throw new ArgumentException("RoboFormResult does not match any available view.");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), actionContext.ModelState);

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                viewContext.ViewData = ViewData;
                viewContext.TempData = TempData;

                await viewResult.View.RenderAsync(viewContext);
                var str = sw.ToString();
                str = str.Replace("[ROBO_UI_PLACEHOLDER]", roboFormContent);

                await actionContext.HttpContext.Response.WriteAsync(str);
            }
        }

        public abstract string GenerateView();

        public virtual Task<bool> OverrideExecuteResult()
        {
            return Task.FromResult(false);
        }
    }
}
