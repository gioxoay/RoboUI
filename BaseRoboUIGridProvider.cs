using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using RoboUI.Extensions;

namespace RoboUI
{
    public abstract class BaseRoboUIGridProvider : IRoboUIGridProvider
    {
        public abstract string Render<TModel>(RoboUIGridResult<TModel> roboUIGrid, IHtmlHelper htmlHelper) where TModel : class;

        public abstract RoboUIGridRequest CreateGridRequest(ControllerContext controllerContext);

        public abstract Task ExecuteGridRequest<TModel>(RoboUIGridResult<TModel> roboUIGrid, RoboUIGridRequest request, ControllerContext controllerContext) where TModel : class;

        public abstract void HandingError(ControllerContext controllerContext, Exception ex);

        public abstract string GetReloadClientScript(string clientId);

        protected void Write(StringBuilder builder, string htmlString)
        {
            builder.Append(htmlString);
        }

        protected virtual string BeginForm<TModel>(IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, string formId) where TModel : class
        {
            string formActionUrl = string.IsNullOrEmpty(roboForm.FormActionUrl)
                ? htmlHelper.ViewContext.HttpContext.Request.Path.ToString()
                : roboForm.FormActionUrl;

            var form = new FluentTagBuilder("form", TagRenderMode.StartTag)
                .MergeAttribute("action", formActionUrl)
                .MergeAttribute("id", formId)
                .MergeAttribute("method", roboForm.FormMethod.ToString().ToLowerInvariant());

            if (roboForm.AjaxEnabled && roboForm.Layout == RoboFormLayout.Tab)
            {
                form = form.MergeAttribute("data-ajax-begin", formId.Replace("-", "_") + "_ValidateTabs");
            }

            if (!roboForm.AjaxEnabled)
            {
                form = form.MergeAttribute("enctype", "multipart/form-data");
            }
            else
            {
                form = form.MergeAttribute("data-ajax", "true");
            }

            form = form.MergeAttribute("method", roboForm.FormMethod.ToString().ToLowerInvariant());
            form = form.MergeAttributes(roboForm.HtmlAttributes);

            return form.ToString();
        }

        protected virtual string BeginForm<TModel>(IHtmlHelper htmlHelper, RoboUIGridResult<TModel> roboGrid) where TModel : class
        {
            if (string.IsNullOrEmpty(roboGrid.FormActionUrl))
            {
                return string.Empty;
            }

            var form = new FluentTagBuilder("form", TagRenderMode.StartTag)
                .MergeAttribute("action", roboGrid.FormActionUrl)
                .MergeAttribute("method", "post");

            if (roboGrid.IsAjaxSupported)
            {
                form = form.MergeAttribute("data-ajax", "true");
            }

            return form.ToString();
        }

        protected virtual void WriteActions(StringBuilder builder, IHtmlHelper htmlHelper, IEnumerable<RoboUIFormAction> actions)
        {
            if (!actions.Any())
            {
                return;
            }

            Write(builder, string.Format("<div class=\"{0}\">", "form-group"));

            Write(builder, "<div class=\"btn-toolbar\">");

            foreach (var action in actions.OrderBy(x => x.Order))
            {
                Write(builder, "<div class=\"btn-group\">");
                Write(builder, RenderAction(htmlHelper, action));
                Write(builder, "</div>");
            }

            Write(builder, "</div></div>");
        }

        public string RenderAction(IHtmlHelper htmlHelper, RoboUIFormAction action)
        {
            if (action.HtmlBuilder != null)
            {
                return action.HtmlBuilder();
            }

            if (action.MenuItems.Count > 0)
            {
                var sb = new StringBuilder();

                sb.AppendFormat("<button data-toggle=\"dropdown\" class=\"{0} dropdown-toggle\">",
                    string.IsNullOrEmpty(action.CssClass) ? "btn btn-default" : action.CssClass.Trim());

                sb.Append(action.Text);
                sb.Append("&nbsp;<span class=\"caret\"></span>");
                sb.AppendFormat("</button>");

                sb.Append("<ul class=\"dropdown-menu\">");
                foreach (var childAction in action.MenuItems)
                {
                    sb.Append(childAction);
                }
                sb.Append("</ul>");

                return sb.ToString();
            }

            if (action.IsSubmitButton)
            {
                var attributes = new RouteValueDictionary();

                if (!action.HtmlAttributes.IsNullOrEmpty())
                {
                    foreach (var attribute in action.HtmlAttributes)
                    {
                        attributes.Add(attribute.Key, attribute.Value);
                    }
                }

                var cssClass = (RoboUI.DefaultRoboUIFormProvider.GetButtonSizeCssClass(action.ButtonSize) + " " + RoboUI.DefaultRoboUIFormProvider.GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass + (!action.IsValidationSupported ? " cancel" : "")).Trim();

                if (!string.IsNullOrEmpty(cssClass))
                {
                    attributes.Add("class", cssClass);
                }

                if (!string.IsNullOrEmpty(action.ClientId))
                {
                    attributes.Add("id", action.ClientId);
                }

                if (!string.IsNullOrEmpty(action.ConfirmMessage))
                {
                    attributes.Add("onclick", string.Format("return confirm(\"{0}\");", action.ConfirmMessage));
                }

                if (!string.IsNullOrEmpty(action.ClientClickCode))
                {
                    attributes["onclick"] = action.ClientClickCode;
                }

                var tagBuilder = new TagBuilder("button");
                tagBuilder.MergeAttribute("type", "submit");
                tagBuilder.MergeAttribute("value", action.Value);
                tagBuilder.MergeAttribute("name", action.Name);
                tagBuilder.MergeAttribute("id", "btn" + action.Name);
                tagBuilder.MergeAttribute("title", action.Description ?? action.Text);
                tagBuilder.MergeAttributes(attributes, true);

                if (!string.IsNullOrEmpty(action.IconCssClass))
                {
                    var icon = new TagBuilder("i");
                    icon.AddCssClass(action.IconCssClass);

                    tagBuilder.InnerHtml.AppendHtml(string.Concat(icon.ToHtmlString(), " ", action.Text));
                }
                else
                {
                    tagBuilder.InnerHtml.AppendHtml(action.Text);
                }

                return tagBuilder.ToHtmlString();
            }
            else
            {
                var attributes = new RouteValueDictionary();

                if (!action.HtmlAttributes.IsNullOrEmpty())
                {
                    foreach (var attribute in action.HtmlAttributes)
                    {
                        attributes.Add(attribute.Key, attribute.Value);
                    }
                }

                var cssClass = (RoboUI.DefaultRoboUIFormProvider.GetButtonSizeCssClass(action.ButtonSize) + " " + RoboUI.DefaultRoboUIFormProvider.GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass + (!action.IsValidationSupported ? " cancel" : "")).Trim();
                if (!string.IsNullOrEmpty(cssClass))
                {
                    attributes.Add("class", cssClass);
                }

                if (!string.IsNullOrEmpty(action.ClientId))
                {
                    attributes.Add("id", action.ClientId);
                }

                if (!string.IsNullOrEmpty(action.ConfirmMessage))
                {
                    attributes.Add("onclick", string.Format("return confirm(\"{0}\");", action.ConfirmMessage));
                }

                if (!string.IsNullOrEmpty(action.ClientClickCode))
                {
                    attributes["onclick"] = action.ClientClickCode;
                }

                attributes["href"] = action.Url;

                if (action.IsShowModalDialog)
                {
                    attributes.Add("data-toggle", "fancybox");
                    attributes.Add("data-fancybox-type", "iframe");
                    attributes.Add("data-fancybox-width", action.ModalDialogWidth);
                }

                var tagBuilder = new TagBuilder("a");
                tagBuilder.MergeAttributes(attributes);

                if (!string.IsNullOrEmpty(action.IconCssClass))
                {
                    var icon = new TagBuilder("i");
                    icon.AddCssClass(action.IconCssClass);

                    tagBuilder.InnerHtml.AppendHtml(string.Concat(icon.ToHtmlString(), " ", action.Text));
                }
                else
                {
                    tagBuilder.InnerHtml.AppendHtml(action.Text);
                }

                return tagBuilder.ToHtmlString();
            }
        }
    }
}
