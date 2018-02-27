using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using RoboUI.Extensions;

namespace RoboUI
{
    public class Bootstrap3RoboUIFormProvider : BaseRoboUIFormProvider
    {
        private const string ControlCssClass = "form-control";
        private const string FormGroupCssClass = "form-group";
        private const string HorizontalLabelCssClass = "col-sm-3 control-label";
        private const string HorizontalControlCssClass = "col-sm-9";

        #region IRoboUIFormProvider Members

        public override string GetButtonSizeCssClass(ButtonSize buttonSize)
        {
            switch (buttonSize)
            {
                case ButtonSize.Default: return "btn";
                case ButtonSize.Large: return "btn btn-lg";
                case ButtonSize.Small: return "btn btn-sm";
                case ButtonSize.ExtraSmall: return "btn btn-xs";
                default:
                    throw new ArgumentOutOfRangeException("buttonSize");
            }
        }

        public override string GetButtonStyleCssClass(ButtonStyle buttonStyle)
        {
            switch (buttonStyle)
            {
                case ButtonStyle.Default: return "btn-default";
                case ButtonStyle.Primary: return "btn-primary";
                case ButtonStyle.Info: return "btn-info";
                case ButtonStyle.Success: return "btn-success";
                case ButtonStyle.Warning: return "btn-warning";
                case ButtonStyle.Danger: return "btn-danger";
                case ButtonStyle.Inverse: return "btn-inverse";
                case ButtonStyle.Link: return "btn-link";
                default:
                    throw new ArgumentOutOfRangeException("buttonStyle");
            }
        }

        public override string RenderAction(IHtmlHelper htmlHelper, RoboUIFormAction action)
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

                var cssClass = (GetButtonSizeCssClass(action.ButtonSize) + " " + GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass + (!action.IsValidationSupported ? " cancel" : "")).Trim();

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
                    attributes.Add("onclick", string.Format("return confirm('{0}');", action.ConfirmMessage));
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
                tagBuilder.MergeAttributes(attributes);

                if (!string.IsNullOrEmpty(action.IconCssClass))
                {
                    var icon = new TagBuilder("i");
                    icon.AddCssClass(action.IconCssClass);
                    tagBuilder.InnerHtml.AppendHtml(icon.ToHtmlString());
                    tagBuilder.InnerHtml.Append(" " + action.Text);
                }
                else
                {
                    tagBuilder.InnerHtml.Append(action.Text);
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

                var cssClass = (GetButtonSizeCssClass(action.ButtonSize) + " " + GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass + (!action.IsValidationSupported ? " cancel" : "")).Trim();
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
                    attributes.Add("onclick", string.Format("return confirm('{0}');", action.ConfirmMessage));
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
                    tagBuilder.InnerHtml.AppendHtml(icon.ToHtmlString());
                    tagBuilder.InnerHtml.Append(" " + action.Text);
                }
                else
                {
                    tagBuilder.InnerHtml.Append(action.Text);
                }

                return tagBuilder.ToHtmlString();
            }
        }

        public override string RenderActions(IHtmlHelper htmlHelper, IEnumerable<RoboUIFormAction> actions)
        {
            if (!actions.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder(256);

            sb.Append("<div class=\"form-group\"><div class=\"btn-toolbar\">");

            foreach (var action in actions)
            {
                sb.Append("<div class=\"btn-group\">");
                sb.Append(RenderAction(htmlHelper, action));
                sb.Append("</div>");
            }

            sb.Append("</div></div>");

            return sb.ToString();
        }

        public override string RenderForm<TModel>(IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm)
        {
            var sb = new StringBuilder(2048);
            var writer = new HtmlTextWriter(new StringWriter(sb));

            var formId = string.IsNullOrWhiteSpace(roboForm.FormId)
                ? "robo_form_" + Guid.NewGuid().ToString("N").ToLowerInvariant()
                : roboForm.FormId.Replace("-", "_");

            var scriptRegister = roboForm.ControllerContext.HttpContext.RequestServices.GetService<IScriptRegister>();

            writer.Write(string.IsNullOrEmpty(roboForm.CssClass)
                ? string.Format("<div class=\"robo-form-container robo-form-layout-{0}\">", roboForm.Layout.ToString().ToLowerInvariant())
                : string.Format("<div class=\"robo-form-container robo-form-layout-{1} {0}\">", roboForm.CssClass, roboForm.Layout.ToString().ToLowerInvariant()));

            if (!roboForm.DisableGenerateForm)
            {
                string form = BeginForm(htmlHelper, roboForm, formId);
                writer.Write(form);
            }

            if (roboForm.RequireAntiForgeryToken)
            {
                throw new NotImplementedException();
                //writer.Write(AntiForgery.GetHtml());
            }

            if (!string.IsNullOrEmpty(roboForm.FormWrapperStartHtml))
            {
                writer.Write(roboForm.FormWrapperStartHtml);
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "robo-form-container-wrap");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            #region Buttons

            var htmlActions = new List<string>();

            if (roboForm.ShowSubmitButton && !roboForm.ReadOnly)
            {
                var tagBuilder = new TagBuilder("button");
                tagBuilder.InnerHtml.AppendHtml(roboForm.SubmitButtonText);
                tagBuilder.MergeAttribute("type", "submit");
                tagBuilder.MergeAttribute("name", "Save");
                tagBuilder.AddCssClass(GetButtonSizeCssClass(ButtonSize.Default));
                tagBuilder.AddCssClass(GetButtonStyleCssClass(ButtonStyle.Primary));
                tagBuilder.MergeAttributes(roboForm.SubmitButtonHtmlAttributes, true);

                htmlActions.Add(tagBuilder.ToHtmlString());
            }

            htmlActions.AddRange(roboForm.Actions.Select(x => RenderAction(htmlHelper, x)));

            if (roboForm.ShowCancelButton)
            {
                if (!string.IsNullOrEmpty(roboForm.CancelButtonUrl))
                {
                    var tagBuilder = new TagBuilder("a");
                    tagBuilder.InnerHtml.AppendHtml(roboForm.CancelButtonText);
                    tagBuilder.MergeAttribute("href", roboForm.CancelButtonUrl);
                    tagBuilder.AddCssClass(GetButtonSizeCssClass(ButtonSize.Default));
                    tagBuilder.AddCssClass(GetButtonStyleCssClass(ButtonStyle.Default));
                    tagBuilder.MergeAttributes(roboForm.CancelButtonHtmlAttributes);
                    htmlActions.Add(tagBuilder.ToHtmlString());
                }
                else
                {
                    var tagBuilder = new TagBuilder("button");
                    tagBuilder.InnerHtml.AppendHtml(roboForm.CancelButtonText);
                    tagBuilder.MergeAttribute("type", "button");
                    tagBuilder.MergeAttribute("name", "Cancel");
                    tagBuilder.MergeAttribute("onclick", "if(self != top){ parent.jQuery.fancybox.close(); }else{ history.back(); }");
                    tagBuilder.AddCssClass(GetButtonSizeCssClass(ButtonSize.Default));
                    tagBuilder.AddCssClass(GetButtonStyleCssClass(ButtonStyle.Default));
                    tagBuilder.MergeAttributes(roboForm.CancelButtonHtmlAttributes);
                    htmlActions.Add(tagBuilder.ToHtmlString());
                }
            }

            #endregion Buttons

            var properties = roboForm.GetProperties().ToList();

            if (properties.Count > 0)
            {
                foreach (var property in properties)
                {
                    property.IsReadOnly = property.IsReadOnly || roboForm.ReadOnlyProperties.Contains(property.Name);
                }

                switch (roboForm.Layout)
                {
                    #region RoboFormLayout.Grouped

                    case RoboFormLayout.Grouped:
                    {
                        foreach (var groupedLayout in roboForm.GroupedLayouts)
                        {
                            // todo: change to grouplayout properties.
                            var layout = groupedLayout;
                            var grpProperties = properties.Where(x => layout.Properties.Contains(x.PropertyName));

                            if (!string.IsNullOrEmpty(groupedLayout.FormGroupWrapperStartHtml))
                            {
                                writer.Write(groupedLayout.FormGroupWrapperStartHtml);
                            }

                            writer.Write("<div class=\"{0}\">", groupedLayout.CssClass);

                            if (groupedLayout.EnableScrollbar)
                            {
                                writer.Write("<div style=\"overflow: auto;\">");
                            }

                            var groupedLayoutColumns = groupedLayout.Column;

                            if (groupedLayoutColumns == 0)
                            {
                                groupedLayoutColumns = 1;
                            }

                            var groupedLayoutRows =
                                (int)Math.Ceiling((double)groupedLayout.Properties.Count / groupedLayoutColumns);

                            // Render hidden fields
                            foreach (
                                var property in
                                grpProperties.Where(
                                    x => !roboForm.ExcludedProperties.Contains(x.Name) && x is RoboHiddenAttribute))
                            {
                                RenderControl(writer, htmlHelper, roboForm, property);
                            }

                            if (groupedLayout.EnableGrid)
                            {
                                writer.Write("<table style=\"width: 100%;\">");

                                var columnWith = 100 / groupedLayoutColumns;

                                writer.Write("<colgroup>");

                                for (int i = 0; i < groupedLayoutColumns; i++)
                                {
                                    writer.Write("<col style=\"width: {0}%\">", columnWith);
                                }

                                writer.Write("</colgroup>");

                                var index = 0;

                                for (var i = 0; i < groupedLayoutRows; i++)
                                {
                                    writer.Write("<tr>");

                                    for (var j = 0; j < groupedLayoutColumns; j++)
                                    {
                                        if (index == groupedLayout.Properties.Count)
                                        {
                                            continue;
                                        }
                                        var propertyName = groupedLayout.Properties.ElementAt(index);

                                        if (roboForm.ExcludedProperties.Contains(propertyName))
                                        {
                                            writer.Write("<td></td>");
                                            continue;
                                        }

                                        var property = properties.First(x => x.Name == propertyName);

                                        if (!string.IsNullOrEmpty(property.ControlSpan))
                                        {
                                            continue;
                                        }

                                        var spanControls = properties.Where(x => x.ControlSpan == propertyName).ToList();
                                        spanControls.Insert(0, property);

                                        if (property is RoboHiddenAttribute)
                                        {
                                            RenderControls(writer, htmlHelper, roboForm, spanControls.ToArray());
                                            index++;
                                            j--;
                                            continue;
                                        }

                                        writer.Write("<td>");

                                        RenderControls(writer, htmlHelper, roboForm, spanControls.ToArray());

                                        writer.Write("</td>");
                                        index++;
                                    }

                                    writer.Write("</tr>");
                                }

                                writer.Write("</table>");
                            }
                            else
                            {
                                var groupedLayoutProperties =
                                    groupedLayout.Properties.Select(x => properties.First(y => y.Name == x)).ToList();
                                var max = groupedLayoutProperties.Max(x => x.ContainerRowIndex);

                                if (max != -100)
                                {
                                    for (var i = 0; i <= max; i++)
                                    {
                                        var propertiesInRow =
                                            groupedLayoutProperties.Where(
                                                    x =>
                                                        x.ContainerRowIndex == i &&
                                                        !roboForm.ExcludedProperties.Contains(x.Name))
                                                .ToList();
                                        if (!propertiesInRow.Any())
                                        {
                                            continue;
                                        }

                                        writer.Write("<div class=\"row\">");

                                        foreach (var property in propertiesInRow)
                                        {
                                            RenderControls(writer, htmlHelper, roboForm, property);
                                        }

                                        writer.Write("</div>");
                                    }
                                }
                                else
                                {
                                    foreach (
                                        var property in
                                        groupedLayoutProperties.Where(
                                            x =>
                                                !roboForm.ExcludedProperties.Contains(x.Name) &&
                                                !(x is RoboHiddenAttribute)))
                                    {
                                        writer.Write("<div class=\"row\">");

                                        RenderControls(writer, htmlHelper, roboForm, property);

                                        writer.Write("</div>");
                                    }
                                }
                            }

                            if (groupedLayout.EnableScrollbar)
                            {
                                writer.Write("</div>");
                            }

                            writer.Write("</div>");

                            if (!string.IsNullOrEmpty(groupedLayout.FormGroupWrapperEndHtml))
                            {
                                writer.Write(groupedLayout.FormGroupWrapperEndHtml);
                            }
                        }

                        writer.Write("<div class=\"row row-actions\">");
                        RenderActions(writer, roboForm, htmlActions.ToArray());
                        writer.Write("</div>");
                    }
                        break;

                    #endregion RoboFormLayout.Grouped

                    #region RoboFormLayout.Tab

                    case RoboFormLayout.Tab:
                    {
                        writer.Write("<ul class=\"nav nav-tabs\" role=\"tablist\">");

                        var tabIndex = 0;
                        foreach (var tabbedLayout in roboForm.TabbedLayouts)
                        {
                            writer.Write(tabIndex == 0
                                ? string.Format(
                                    "<li role=\"presentation\" class=\"active\"><a data-toggle=\"tab\" href=\"#{1}_Tab{2}\">{0}</a></li>",
                                    tabbedLayout.Title,
                                    formId,
                                    tabIndex)
                                : string.Format(
                                    "<li role=\"presentation\"><a data-toggle=\"tab\" href=\"#{1}_Tab{2}\">{0}</a></li>",
                                    tabbedLayout.Title,
                                    formId,
                                    tabIndex));

                            tabIndex++;
                        }

                        writer.Write("</ul>");

                        writer.Write("<div class=\"tab-content\">");

                        tabIndex = 0;
                        foreach (var tabbedLayout in roboForm.TabbedLayouts)
                        {
                            writer.Write(tabIndex == 0
                                ? string.Format("<div role=\"tabpanel\" id=\"{0}_Tab{1}\" class=\"tab-pane active\">",
                                    formId, tabIndex)
                                : string.Format("<div role=\"tabpanel\" id=\"{0}_Tab{1}\" class=\"tab-pane\">", formId,
                                    tabIndex));
                            tabIndex++;

                            foreach (var item in tabbedLayout.Groups)
                            {
                                var propertiesInGroup = properties.Where(x => item.Properties.Contains(x.Name)).ToList();
                                if (propertiesInGroup.Count == 0)
                                {
                                    continue;
                                }

                                if (propertiesInGroup.All(x => x.ContainerRowIndex == -100))
                                {
                                    var index = 0;
                                    foreach (
                                        var attribute in
                                        propertiesInGroup.Where(attribute => !(attribute is RoboHiddenAttribute)))
                                    {
                                        attribute.ContainerRowIndex = index;
                                        index++;
                                    }
                                }

                                // Render hidden fields
                                foreach (
                                    var attribute in
                                    propertiesInGroup.Where(attribute => attribute is RoboHiddenAttribute))
                                {
                                    RenderControl(writer, htmlHelper, roboForm, attribute);
                                }

                                var max = propertiesInGroup.Max(x => x.ContainerRowIndex);
                                var min = propertiesInGroup.Min(x => x.ContainerRowIndex);
                                for (var i = min; i <= max; i++)
                                {
                                    var propertiesInRow =
                                        propertiesInGroup.Where(
                                            x => x.ContainerRowIndex == i && !(x is RoboHiddenAttribute)).ToList();
                                    if (propertiesInRow.Count == 0)
                                    {
                                        continue;
                                    }

                                    writer.Write("<div class=\"row\">");

                                    foreach (var property in propertiesInRow)
                                    {
                                        if (roboForm.ExcludedProperties.Contains(property.Name))
                                        {
                                            continue;
                                        }
                                        RenderControls(writer, htmlHelper, roboForm, property);
                                    }

                                    writer.Write("</div>");
                                }
                            }

                            writer.Write("</div>");
                        }

                        writer.Write("<div class=\"row\">");
                        RenderActions(writer, roboForm, htmlActions.ToArray());
                        writer.Write("</div>");

                        writer.Write("</div>");
                    }

                        scriptRegister.AddInlineScript(
                            string.Format(
                                "$('#{0} a[data-toggle=\"tab\"]').on('shown.bs.tab', function (e) {{ var $id = $(e.target).attr('href');  $($id).find('select').change();  }});",
                                formId));
                        break;

                    #endregion RoboFormLayout.Tab

                    #region RoboFormLayout.Flat

                    case RoboFormLayout.Flat:
                    {
                        if (!string.IsNullOrEmpty(roboForm.Description))
                        {
                            writer.Write("<p class=\"text-info\">" + roboForm.Description + "</p>");
                        }

                        if (roboForm.ShowValidationSummary)
                        {
                            if (string.IsNullOrEmpty(roboForm.ValidationSummary))
                            {
                                writer.Write(
                                    "<div data-valmsg-summary=\"true\" class=\"validation-summary\"><ul></ul></div>");
                            }
                            else
                            {
                                writer.Write(
                                    "<div data-valmsg-summary=\"true\" class=\"validation-summary\"><span>{0}</span><ul></ul></div>",
                                    roboForm.ValidationSummary);
                            }
                        }

                        // Auto assign grid row index
                        if (properties.Where(pair => !roboForm.ExcludedProperties.Contains(pair.Name))
                            .All(x => x.ContainerRowIndex == -1))
                        {
                            var rowIndex = 0;
                            foreach (var property in properties)
                            {
                                if (property is RoboHiddenAttribute)
                                {
                                    continue;
                                }
                                property.ContainerRowIndex = rowIndex;
                                rowIndex++;
                            }
                        }

                        // Render hidden fields
                        foreach (var property in
                            properties.Where(
                                x => !roboForm.ExcludedProperties.Contains(x.Name) && x is RoboHiddenAttribute))
                        {
                            RenderControl(writer, htmlHelper, roboForm, property);
                        }

                        var columns = properties.Select(x => x.ContainerColumnIndex).Distinct().OrderBy(x => x).ToList();
                        var columnCssClass = string.Format("col-md-{0}", 12 / columns.Count);

                        writer.Write("<div class=\"row\">");

                        foreach (var column in columns)
                        {
                            writer.Write("<div class=\"{0}\">", columnCssClass);

                            var columnProperties = properties.Where(x => x.ContainerColumnIndex == column).ToList();
                            if (columnProperties.Any(x => x.Order != 0))
                            {
                                columnProperties = columnProperties.OrderBy(x => x.Order).ToList();
                            }

                            var max = columnProperties.Max(x => x.ContainerRowIndex);

                            if (max != -100)
                            {
                                for (var i = 0; i <= max; i++)
                                {
                                    var propertiesInRow = columnProperties.Where(x =>
                                        x.ContainerRowIndex == i && !roboForm.ExcludedProperties.Contains(x.Name) &&
                                        string.IsNullOrEmpty(x.ControlSpan)).ToList();
                                    if (!propertiesInRow.Any())
                                    {
                                        continue;
                                    }

                                    writer.Write("<div class=\"row\">");

                                    foreach (var property in propertiesInRow)
                                    {
                                        var propertyName = property.Name;
                                        var spanControls = properties.Where(x => x.ControlSpan == propertyName).ToList();
                                        spanControls.Insert(0, property);
                                        RenderControls(writer, htmlHelper, roboForm, spanControls.ToArray());
                                    }

                                    writer.Write("</div>");
                                }
                            }
                            else
                            {
                                foreach (var property in columnProperties.Where(x => !roboForm.ExcludedProperties.Contains(x.Name) && !(x is RoboHiddenAttribute)))
                                {
                                    writer.Write("<div class=\"row\">");

                                    RenderControls(writer, htmlHelper, roboForm, property);

                                    writer.Write("</div>");
                                }
                            }

                            writer.Write("</div>");
                        }

                        writer.Write("</div>");

                        if (htmlActions.Any())
                        {
                            writer.Write("<div class=\"row row-actions\">");
                            RenderActions(writer, roboForm, htmlActions.ToArray());
                            writer.Write("</div>");
                        }
                    }
                        break;

                    #endregion RoboFormLayout.Flat

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(roboForm.Description))
                {
                    writer.Write("<div class=\"lead\">" + roboForm.Description + "</div>");
                }

                writer.Write("<div class=\"row row-actions\">");
                RenderActions(writer, roboForm, htmlActions.ToArray());
                writer.Write("</div>");
            }

            foreach (var hiddenValue in roboForm.HiddenValues)
            {
                writer.Write("<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\"/>", hiddenValue.Key, WebUtility.HtmlEncode(hiddenValue.Value));
            }

            writer.RenderEndTag(); // div

            if (!string.IsNullOrEmpty(roboForm.FormWrapperEndHtml))
            {
                writer.Write(roboForm.FormWrapperEndHtml);
            }

            if (!roboForm.DisableGenerateForm)
            {
                writer.Write("</form>");
            }

            if (!roboForm.DisableBlockUI)
            {
                // Block UI
                writer.Write("<div class=\"blockUI\" style=\"display:none; z-index: 100; border: none; margin: 0; padding: 0; width: 100%; height: 100%; top: 0; left: 0; background-color: #000000; opacity: 0.05; filter: alpha(opacity = 5); cursor: wait; position: absolute;\"></div>");

                // Block Msg
                writer.Write("<div class=\"blockUIMsg\" style=\"display:none;\">Processing...</div>");

                if (roboForm.AjaxEnabled)
                {
                    scriptRegister.AddInlineScript("$(document).bind(\"ajaxSend\", function(){ $(\".blockUI, .blockUIMsg\").show(); }).bind(\"ajaxComplete\", function(){ $(\".blockUI, .blockUIMsg\").hide(); });");
                }
                else
                {
                    scriptRegister.AddInlineScript(string.Format("$('#{0}').on(\"submit\", function(){{ var isValid = $('#{0}').valid(); if(isValid){{ $(\".blockUI, .blockUIMsg\").show(); }} }});", formId));
                }
            }

            // End div container
            writer.Write("</div>");

            if (roboForm.AjaxEnabled && roboForm.Layout == RoboFormLayout.Tab)
            {
                scriptRegister.AddInlineScript(string.Format("function {1}_ValidateTabs(){{ var validationInfo = $('#{1}').data('unobtrusiveValidation'); for(var i = 0; i < {0}; i++){{ $('a[href=#{1}_Tab' + i + ']').tab('show'); var isValid = validationInfo.validate(); if(!isValid){{ return false; }} }} }}", roboForm.TabbedLayouts.Count, formId));
            }

            return sb.ToString();
        }

        public override void RenderButtonAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboButtonAttribute roboAttribute)
        {
            var attributes = new Dictionary<string, object>(roboAttribute.HtmlAttributes);

            if (roboAttribute.Disabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }

            if (!string.IsNullOrEmpty(roboAttribute.OnClick))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, roboAttribute.OnClick);
            }

            if (!attributes.ContainsKey("id"))
            {
                attributes.Add("id", htmlHelper.GenerateIdFromName(roboAttribute.Name));
            }

            if (!attributes.ContainsKey("name"))
            {
                attributes.Add("name", roboForm.ViewData.TemplateInfo.GetFullHtmlFieldName(roboAttribute.Name));
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Type, roboAttribute.ButtonType);
            writer.AddAttributes(attributes);
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write(roboAttribute.LabelText);
            writer.RenderEndTag(); // button
        }

        public override void RenderCascadingCheckBoxListAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboCascadingCheckBoxListAttribute roboAttribute)
        {
            if (string.IsNullOrEmpty(roboAttribute.ParentControl))
            {
                throw new ArgumentException("The ParentControl must be not null or empty.");
            }

            if (!typeof(IEnumerable).IsAssignableFrom(roboAttribute.PropertyType))
            {
                throw new NotSupportedException("Cannot apply robo choice for non enumerable property as checkbox list.");
            }

            var clientId = "divcbl_" + Guid.NewGuid().ToString("N").ToLowerInvariant();
            var sourceUrl = roboForm.GetCascadingCheckBoxDataSource(roboAttribute.Name);
            var cssClass = ControlCssClass + " ";

            if (roboAttribute.HtmlAttributes.ContainsKey("class"))
            {
                cssClass += roboAttribute.HtmlAttributes["class"];
            }
            else
            {
                cssClass += "checkbox";
            }

            var value = roboAttribute.Value as IEnumerable;

            var values = new List<string>();
            string selectedItems = "";
            if (value != null)
            {
                values.AddRange(from object item in value select Convert.ToString(item));
                selectedItems = string.Join(",", values.ToArray());
            }

            var sb = new StringBuilder();
            sb.AppendFormat("$('#{0}').change(function(){{", htmlHelper.GenerateIdFromName(roboAttribute.ParentControl));
            if (roboAttribute.IsReadOnly)
            {
                sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ var control = $('#{1}'); control.empty(); if(!result || !result.length){{ return; }} var items = '{5}'; $.each(result, function(index, item){{ if(items.indexOf(item.Value) != -1){{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\" checked=\"checked\" disabled=\"disabled\">' + item.Text + '</label>');}} else{{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\" disabled=\"disabled\">' + item.Text + '</label>');}} }}); }} }});", sourceUrl, clientId, roboAttribute.ParentControl, cssClass, roboAttribute.Name, selectedItems);
            }
            else
            {
                sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ var control = $('#{1}'); control.empty(); if(!result || !result.length){{ return; }} var items = '{5}'; $.each(result, function(index, item){{ if(items.indexOf(item.Value) != -1){{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\" checked=\"checked\">' + item.Text + '</label>');}} else{{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\">' + item.Text + '</label>');}} }}); }} }});", sourceUrl, clientId, roboAttribute.ParentControl, cssClass, roboAttribute.Name, selectedItems);
            }

            var scriptRegister = roboForm.ControllerContext.HttpContext.RequestServices.GetService<IScriptRegister>();
            scriptRegister.AddInlineScript(sb.ToString());

            writer.Write("<div class=\"row no-padding\" id=\"{0}\"></div>", clientId);
        }

        public override void RenderCascadingDropDownAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboCascadingDropDownAttribute roboAttribute)
        {
            RoboCascadingDropDownOptions options;

            if (roboAttribute.Name.Contains('['))
            {
                var attrs = roboAttribute.Name.Split('.');
                if (attrs.Length > 2)
                {
                    options = roboForm.GetCascadingDropDownDataSource(string.Format("{0}.{1}", attrs[0], attrs[2]));
                }
                else
                {
                    options = roboForm.GetCascadingDropDownDataSource(roboAttribute.Name.RemoveBetween('[', ']'));
                }
            }
            else
            {
                options = roboForm.GetCascadingDropDownDataSource(roboAttribute.Name);
            }

            var parentControl = options.ParentControl ?? roboAttribute.ParentControl;

            if (string.IsNullOrEmpty(parentControl))
            {
                throw new ArgumentException("The ParentControl must be not null or empty.");
            }

            if (!roboAttribute.AbsoluteParentControl)
            {
                parentControl = roboAttribute.Name.Replace(roboAttribute.Name.Split('.').Last(), parentControl);
            }

            var attributes = new Dictionary<string, object>(roboAttribute.HtmlAttributes);
            if (roboAttribute.IsReadOnly || roboForm.ReadOnly)
            {
                MergeHtmlAttribute(attributes, "disabled", "disabled");
            }

            MergeHtmlAttribute(attributes, "class", ControlCssClass);

            if (roboAttribute.IsRequired)
            {
                MergeHtmlAttribute(attributes, "data-val", "true");
                MergeHtmlAttribute(attributes, "data-val-required", Constants.Validation.Required);
            }

            if (roboAttribute.AllowMultiple)
            {
                MergeHtmlAttribute(attributes, "multiple", "multiple");
            }

            if (!string.IsNullOrEmpty(roboAttribute.OnSelectedIndexChanged))
            {
                MergeHtmlAttribute(attributes, "onchange", roboAttribute.OnSelectedIndexChanged);
            }

            if (roboAttribute.Value != null)
            {
                MergeHtmlAttribute(attributes, "data-value", roboAttribute.Value.ToString());
            }

            var clientId = htmlHelper.GenerateIdFromName(roboAttribute.Name);
            var parentControlId = htmlHelper.GenerateIdFromName(parentControl);

            var sb = new StringBuilder();

            sb.AppendFormat("$('#{0}').change(function(){{", parentControlId);

            sb.Append("if($(this).is(':hidden')){ return; }");

            if (roboAttribute.EnableChosen)
            {
                if (roboAttribute.AllowMultiple)
                {
                    var multilValue = "";
                    if (roboAttribute.Value != null)
                    {
                        var arr = ((IEnumerable)roboAttribute.Value).Cast<object>()
                            .Select(x => x.ToString())
                            .ToArray();
                        multilValue = string.Format("[{0}]", string.Join(",", arr));
                    }

                    sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={3}&command={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ {5} var control = $('#{1}'); var oldValue = control.data('value'); control.empty(); if(!result || !result.length){{ return; }} $.each(result, function(index, item){{ control.append($('<option></option>').attr('value', item.value).text(item.text)); }}); if(oldValue){{ control.val(oldValue); }} control.change(); if ($('#{1}').attr('data-value')!== undefined) {{ $('#{1}').val({4}).trigger(\"liszt:updated\"); $('#{1}').removeAttr('data-value');}} $('#{1}').trigger(\"chosen:updated\"); }} }});", options.SourceUrl, clientId, options.Command, roboAttribute.Name, multilValue, roboAttribute.OnSuccess);
                }
                else
                {
                    sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={3}&command={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ {4} var control = $('#{1}'); var oldValue = control.data('value'); control.empty(); if(!result || !result.length){{ return; }} $.each(result, function(index, item){{ control.append($('<option></option>').attr('value', item.value).text(item.text)); }}); if(oldValue){{ control.val(oldValue); }} control.change(); $('#{1}').trigger(\"chosen:updated\"); }} }});", options.SourceUrl, clientId, options.Command, roboAttribute.Name, roboAttribute.OnSuccess);
                }
            }
            else
            {
                sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={3}&command={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ {4} var control = $('#{1}'); var oldValue = control.data('value'); control.empty(); if(!result || !result.length){{ return; }} $.each(result, function(index, item){{ control.append($('<option></option>').attr('value', item.value).text(item.text)); }}); if(oldValue){{ control.val(oldValue); }} control.change(); }} }});", options.SourceUrl, clientId, options.Command, parentControl, roboAttribute.OnSuccess);
            }

            sb.Append("});");

            if (roboAttribute.EnableChosen)
            {
                sb.AppendFormat("$('#{0}').chosen({{ no_results_text: \"No results matched\", allow_single_deselect:true,width: 'auto' }});", clientId);
            }

            var scriptRegister = roboForm.ControllerContext.HttpContext.RequestServices.GetService<IScriptRegister>();

            scriptRegister.AddInlineScript(sb.ToString());

            // Trigger parent control change once time only
            scriptRegister.AddInlineScript(string.Format("$('#{0}').change();", parentControlId));

            //Long: fisrt load controll will return null
            var selectList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };

            writer.Write(htmlHelper.DropDownList(roboAttribute.Name, selectList, attributes).ToHtmlString());

            if (!string.IsNullOrEmpty(roboAttribute.HelpText))
            {
                writer.Write("<span class=\"help-block\">{0}</span>", roboAttribute.HelpText);
            }

            writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", roboAttribute.Name);
        }

        public override void RenderChoiceAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboChoiceAttribute roboAttribute)
        {
            switch (roboAttribute.Type)
            {
                case RoboChoiceType.CheckBox:
                    RenderCheckBox(writer, htmlHelper, roboForm, roboAttribute);
                    break;

                case RoboChoiceType.CheckBoxList:
                    RenderCheckBoxList(writer, htmlHelper, roboForm, roboAttribute);
                    break;

                case RoboChoiceType.DropDownList:
                case RoboChoiceType.RadioButtonList:
                    RenderSingleChoice(writer, htmlHelper, roboForm, roboAttribute);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void RenderComplexAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboComplexAttribute roboAttribute)
        {
            var attributes = new List<RoboControlAttribute>();
            foreach (var propertyInfo in roboAttribute.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attribute = propertyInfo.GetCustomAttribute<RoboControlAttribute>();
                if (attribute != null)
                {
                    attribute.Name = roboAttribute.Name + "." + propertyInfo.Name;
                    attribute.PropertyName = propertyInfo.Name;
                    attribute.PropertyType = propertyInfo.PropertyType;
                    attribute.PropertyInfo = propertyInfo;

                    // Custom attributes
                    var htmlAttributes = propertyInfo.GetCustomAttributes<RoboHtmlAttributeAttribute>().ToList();
                    if (htmlAttributes.Any())
                    {
                        foreach (var htmlAttribute in htmlAttributes)
                        {
                            attribute.HtmlAttributes.Add(htmlAttribute.Name, htmlAttribute.Value);
                        }
                    }

                    attributes.Add(attribute);
                }
            }

            if (roboAttribute.EnableGrid)
            {
                var groupedLayoutColumns = roboAttribute.Column;

                if (groupedLayoutColumns == 0)
                {
                    groupedLayoutColumns = 1;
                }

                var groupedLayoutRows = (int)Math.Ceiling((double)attributes.Count / groupedLayoutColumns);

                writer.Write("<table style=\"width: 100%;\">");

                var columnWith = 100 / groupedLayoutColumns;

                writer.Write("<colgroup>");

                for (int i = 0; i < groupedLayoutColumns; i++)
                {
                    writer.Write("<col style=\"width: {0}%\">", columnWith);
                }

                writer.Write("</colgroup>");

                var index = 0;

                for (var i = 0; i < groupedLayoutRows; i++)
                {
                    writer.Write("<tr>");

                    for (var j = 0; j < groupedLayoutColumns; j++)
                    {
                        if (index == attributes.Count)
                        {
                            continue;
                        }
                        var attribute = attributes[index];

                        attribute.Value = roboForm.GetPropertyValue(roboAttribute.Value, attribute.PropertyName);

                        if (!string.IsNullOrEmpty(attribute.ControlSpan))
                        {
                            continue;
                        }

                        var spanControls = attributes.Where(x => x.ControlSpan == attribute.PropertyName).ToList();
                        foreach (var roboFormAttribute in spanControls)
                        {
                            roboFormAttribute.Value = roboForm.GetPropertyValue(roboAttribute.Value, roboFormAttribute.PropertyName);
                        }
                        spanControls.Insert(0, attribute);

                        if (attribute is RoboHiddenAttribute)
                        {
                            RenderControls(writer, htmlHelper, roboForm, spanControls.ToArray());
                            index++;
                            j--;
                            continue;
                        }

                        writer.Write("<td>");

                        RenderControls(writer, htmlHelper, roboForm, spanControls.ToArray());

                        writer.Write("</td>");
                        index++;
                    }

                    writer.Write("</tr>");
                }

                writer.Write("</table>");
            }
            else
            {
                foreach (var attribute in attributes)
                {
                    if (roboAttribute.IsReadOnly)
                    {
                        attribute.IsReadOnly = true;
                    }

                    attribute.Value = roboForm.GetPropertyValue(roboAttribute.Value, attribute.PropertyName);

                    if (!string.IsNullOrEmpty(attribute.ControlSpan))
                    {
                        continue;
                    }

                    var spanControls = attributes.Where(x => x.ControlSpan == attribute.PropertyName).ToList();
                    foreach (var roboFormAttribute in spanControls)
                    {
                        roboFormAttribute.Value = roboForm.GetPropertyValue(roboAttribute.Value, roboFormAttribute.PropertyName);
                    }
                    spanControls.Insert(0, attribute);

                    RenderControls(writer, htmlHelper, roboForm, spanControls.ToArray());
                }
            }
        }

        public override void RenderDatePickerAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboDatePickerAttribute roboAttribute)
        {
            var dateFormat = roboAttribute.DateFormat;
            if (string.IsNullOrEmpty(dateFormat))
            {
                dateFormat = roboAttribute.DateTimePicker ? Constants.FullDatePattern : Constants.ShortDatePattern;
            }
            string formatedValue = null;

            var rawValue = roboAttribute.Value;
            if (rawValue is DateTime dtValue)
            {
                formatedValue = dtValue != DateTime.MinValue ? dtValue.ToString(dateFormat) : string.Empty;
            }
            else if (rawValue is string)
            {
                formatedValue = rawValue as string;
            }

            var attributes = new Dictionary<string, object>(roboAttribute.HtmlAttributes);
            MergeHtmlAttribute(attributes, "class", ControlCssClass);

            if (roboForm.ReadOnly || roboAttribute.IsReadOnly)
            {
                attributes.Add("readonly", "readonly");
                writer.Write(htmlHelper.TextBox(roboAttribute.Name, formatedValue, attributes).ToHtmlString());
                return;
            }

            MergeHtmlAttribute(attributes, "autocomplete", "off");

            MergeHtmlAttribute(attributes, "class", roboAttribute.DateTimePicker ? "datetimepicker" : "datepicker");

            attributes.Add("data-val", "true");

            if (roboAttribute.IsRequired)
            {
                attributes.Add("data-val-required", roboAttribute.EnableSortRequired ? "*" : Constants.Validation.Required);
            }

            if (!string.IsNullOrEmpty(roboAttribute.MinimumValue))
            {
                attributes.Add("data-date-start-date", roboAttribute.MinimumValue);
            }

            if (!string.IsNullOrEmpty(roboAttribute.MaximumValue))
            {
                attributes.Add("data-date-end-date", roboAttribute.MaximumValue);
            }

            writer.Write(htmlHelper.TextBox(roboAttribute.Name, formatedValue, attributes).ToHtmlString());

            writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", roboAttribute.Name);

            if (!string.IsNullOrEmpty(roboAttribute.HelpText))
            {
                writer.Write("<span class=\"help-block\">{0}</span>", roboAttribute.HelpText);
            }
        }

        public override void RenderTimePickerAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm,
            RoboTimePickerAttribute roboAttribute)
        {
            var scriptRegister = roboForm.ControllerContext.HttpContext.RequestServices.GetService<IScriptRegister>();

            var id = htmlHelper.GenerateIdFromName(roboAttribute.Name);
            scriptRegister.AddInlineScript(string.Format("$('#{0}').kendoTimePicker({{ format: 'HH:mm', parseFormats: ['HH:mm'], interval: {1} }});", id, roboAttribute.Interval));

            var htmlAttributes = new Dictionary<string, object>(roboAttribute.HtmlAttributes);
            MergeHtmlAttribute(htmlAttributes, "class", ControlCssClass);

            if (roboAttribute.IsRequired)
            {
                MergeHtmlAttribute(htmlAttributes, "data-val", "true");
                MergeHtmlAttribute(htmlAttributes, "required", "required");
                MergeHtmlAttribute(htmlAttributes, "data-val-required", Constants.Validation.Required);
            }

            if (!roboAttribute.EnableTypingText)
            {
                MergeHtmlAttribute(htmlAttributes, "readonly", "readonly");
            }

            writer.Write(htmlHelper.TextBox(roboAttribute.Name, roboAttribute.Value, htmlAttributes));
            writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", roboAttribute.Name);
        }

        public override void RenderDivAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboDivAttribute roboAttribute)
        {
            if (roboAttribute.RenderContentOnly)
            {
                if (roboAttribute.Value != null)
                {
                    writer.Write(Convert.ToString(roboAttribute.Value));
                }
            }
            else
            {
                writer.AddAttributes(roboAttribute.HtmlAttributes);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (roboAttribute.Value != null)
                {
                    writer.Write(Convert.ToString(roboAttribute.Value));
                }

                writer.RenderEndTag();
            }
        }

        public override void RenderGridAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboGridAttribute roboAttribute)
        {
            roboAttribute.EnsureProperties();
            var clientId = "tbl" + htmlHelper.GenerateIdFromName(roboAttribute.Name);

            if (roboAttribute.EnabledScroll)
            {
                writer.Write("<div style=\"overflow: auto;\">");
            }

            // Fake Index value
            writer.Write("<input type=\"hidden\" name=\"{0}.Index\" value=\"-1\" />", roboAttribute.Name);

            writer.AddAttributes(roboAttribute.HtmlAttributes);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, clientId);
            writer.AddAttribute("data-min-rows", roboAttribute.MinRows.ToString(CultureInfo.InvariantCulture));
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            if (!roboAttribute.ShowAsStack && (roboAttribute.ShowTableHead || !string.IsNullOrEmpty(roboAttribute.TableHeadHtml)))
            {
                writer.Write("<thead>");

                if (string.IsNullOrEmpty(roboAttribute.TableHeadHtml))
                {
                    writer.Write("<tr>");

                    writer.Write("<th style=\"display: none;\"></th>");

                    foreach (var attribute in roboAttribute.Attributes)
                    {
                        var propertyName = string.Concat(roboAttribute.Name, ".", attribute.PropertyName);
                        if (roboForm.ExcludedProperties.Contains(propertyName))
                        {
                            continue;
                        }

                        if (attribute is RoboHiddenAttribute)
                        {
                            writer.Write("<th style=\"display: none;\">&nbsp;</th>");
                        }
                        else
                        {
                            if (attribute.ColumnWidth > 0)
                            {
                                writer.Write("<th style=\"width: {1}px;\">{0}</th>", attribute.LabelText, attribute.ColumnWidth);
                            }
                            else
                            {
                                writer.Write("<th>{0}</th>", attribute.LabelText);
                            }
                        }
                    }

                    if (roboAttribute.ShowRowsControl)
                    {
                        if (!roboAttribute.IsReadOnly && !roboForm.ReadOnly)
                        {
                            writer.Write("<th style=\"width: 1%;\">&nbsp;</th>");
                        }
                    }

                    writer.Write("</tr>");
                }
                else
                {
                    writer.Write(roboAttribute.TableHeadHtml);
                }
                writer.Write("</thead>");
            }

            var actualRows = 0;

            var value = roboAttribute.Value as IEnumerable<object>;
            if (value != null)
            {
                actualRows = value.Count();
            }

            var maxRows = roboAttribute.ShowRowsControl ? roboAttribute.MaxRows : actualRows;

            writer.Write("<tbody>");

            for (var i = 0; i < maxRows; i++)
            {
                if (i >= actualRows && (i >= roboAttribute.DefaultRows || actualRows > 0))
                {
                    writer.Write("<tr style=\"display: none;\">");
                    writer.Write("<td style=\"display: none;\"><input type=\"checkbox\" name=\"{0}.Index\" class=\"RoboGrid__Index\" value=\"{1}\" autocomplete=\"off\" /></td>", roboAttribute.Name, i);
                }
                else
                {
                    writer.Write("<tr>");
                    writer.Write("<td style=\"display: none;\"><input type=\"checkbox\" name=\"{0}.Index\" class=\"RoboGrid__Index\" value=\"{1}\" checked=\"checked\" autocomplete=\"off\" /></td>", roboAttribute.Name, i);
                }

                if (roboAttribute.ShowAsStack)
                {
                    writer.Write("<td style=\"padding: 0;\">");

                    foreach (var attribute in roboAttribute.Attributes)
                    {
                        var name = string.Concat(roboAttribute.Name, ".", attribute.PropertyName);
                        if (roboForm.ExcludedProperties.Contains(name))
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(attribute.ControlSpan))
                        {
                            continue;
                        }

                        attribute.Name = roboAttribute.Name + "[" + i + "]." + attribute.PropertyName;

                        if (value != null && i < actualRows)
                        {
                            var obj = value.ElementAt(i);
                            attribute.Value = roboForm.GetPropertyValue(obj, attribute.PropertyName);
                        }
                        else
                        {
                            attribute.Value = RoboGridAttribute.GetDefaultValue(attribute.PropertyType);
                        }

                        if (attribute is RoboHiddenAttribute)
                        {
                            RenderControl(writer, htmlHelper, roboForm, attribute);
                        }
                        else
                        {
                            var propertyName = attribute.PropertyName;
                            var spanAttributes = roboAttribute.Attributes.Where(x => x.ControlSpan == propertyName).ToList();

                            foreach (var spanAttribute in spanAttributes)
                            {
                                spanAttribute.Name = string.Concat(roboAttribute.Name, "[", i, "].", spanAttribute.PropertyName);

                                if (value != null && i < actualRows)
                                {
                                    var obj = value.ElementAt(i);
                                    spanAttribute.Value = roboForm.GetPropertyValue(obj, spanAttribute.PropertyName);
                                }
                                else
                                {
                                    spanAttribute.Value = RoboGridAttribute.GetDefaultValue(spanAttribute.PropertyType);
                                }
                            }

                            spanAttributes.Insert(0, attribute);

                            writer.Write("<div class=\"control-group\">");

                            if (attribute.HasLabelControl)
                            {
                                writer.Write("<label class=\"control-label\">{0}</label>", attribute.LabelText);
                            }

                            writer.Write("<div class=\"controls\">");
                            foreach (var spanAttribute in spanAttributes)
                            {
                                RenderControl(writer, htmlHelper, roboForm, spanAttribute);
                            }
                            writer.Write("</div></div>");
                        }
                    }

                    writer.Write("</td>");
                }
                else
                {
                    foreach (var attribute in roboAttribute.Attributes)
                    {
                        var propertyName = string.Concat(roboAttribute.Name, ".", attribute.PropertyName);
                        if (roboForm.ExcludedProperties.Contains(propertyName))
                        {
                            continue;
                        }

                        attribute.Name = string.Concat(roboAttribute.Name, "[", i, "].", attribute.PropertyName);

                        if (value != null && i < actualRows)
                        {
                            var obj = value.ElementAt(i);
                            attribute.Value = roboForm.GetPropertyValue(obj, attribute.PropertyName);
                        }
                        else
                        {
                            attribute.Value = RoboGridAttribute.GetDefaultValue(attribute.PropertyType);
                        }

                        if (roboAttribute.IsReadOnly)
                        {
                            if (attribute is RoboHiddenAttribute)
                            {
                                writer.Write("<td style=\"display: none;\">{0}</td>", attribute.Value);
                            }
                            else
                            {
                                writer.Write("<td>{0}</td>", attribute.Value);
                            }
                        }
                        else
                        {
                            if (attribute is RoboHiddenAttribute)
                            {
                                writer.Write("<td style=\"display: none;\">");
                            }
                            else
                            {
                                writer.Write("<td>");
                            }
                            RenderControl(writer, htmlHelper, roboForm, attribute);
                            writer.Write("</td>");
                        }
                    }
                }

                if (roboAttribute.ShowRowsControl)
                {
                    if (!roboAttribute.IsReadOnly && !roboForm.ReadOnly)
                    {
                        writer.Write("<td style=\"width: 1%; vertical-align: top;\"><button type=\"button\" onclick=\"var visible = $('#{0} tbody tr:visible').length; var min = parseInt($('#{0}').data('min-rows')); if(visible >= min) {{ $(this).closest('tr').hide().find('.RoboGrid__Index').removeAttr('checked'); $('#{0}_AddButton').show(); }}\" class=\"{1} {2} pull-right\"><i class=\"cx-icon cx-icon-remove cx-icon-white\"></i></button></td>", clientId, GetButtonSizeCssClass(ButtonSize.ExtraSmall), GetButtonStyleCssClass(ButtonStyle.Danger));
                    }
                }

                writer.Write("</tr>");
            }
            writer.Write("</tbody>");

            if (roboAttribute.ShowRowsControl)
            {
                if (!roboAttribute.IsReadOnly && !roboForm.ReadOnly)
                {
                    if (roboAttribute.ShowAsStack)
                    {
                        if (actualRows == maxRows || roboAttribute.DefaultRows == maxRows)
                        {
                            writer.Write("<tfoot><tr><td colspan=\"{0}\"><button style=\"display:none;\" id=\"{1}_AddButton\" type=\"button\" onclick=\"$('#{1} tbody tr:hidden').first().show().find('.RoboGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"cx-icon cx-icon-add cx-icon-white\"></i></button></td></tr></tfoot>", 3, clientId, GetButtonSizeCssClass(ButtonSize.ExtraSmall), GetButtonStyleCssClass(ButtonStyle.Info));
                        }
                        else
                        {
                            writer.Write("<tfoot><tr><td colspan=\"{0}\"><button id=\"{1}_AddButton\" type=\"button\" onclick=\"$('#{1} tbody tr:hidden').first().show().find('.RoboGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"cx-icon cx-icon-add cx-icon-white\"></i></button></td></tr></tfoot>", 3, clientId, GetButtonSizeCssClass(ButtonSize.ExtraSmall), GetButtonStyleCssClass(ButtonStyle.Success));
                        }
                    }
                    else
                    {
                        if (actualRows == maxRows || roboAttribute.DefaultRows == maxRows)
                        {
                            writer.Write("<tfoot><tr><td colspan=\"{0}\"><button style=\"display:none;\" id=\"{1}_AddButton\" type=\"button\" onclick=\"$('#{1} tbody tr:hidden').first().show().find('.RoboGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"cx-icon cx-icon-add cx-icon-white\"></i></button></td></tr></tfoot>", roboAttribute.Attributes.Count + 1, clientId, GetButtonSizeCssClass(ButtonSize.ExtraSmall), GetButtonStyleCssClass(ButtonStyle.Success));
                        }
                        else
                        {
                            writer.Write("<tfoot><tr><td colspan=\"{0}\"><button id=\"{1}_AddButton\" type=\"button\" onclick=\"var row = $('#{1} tbody tr:hidden').first().show(); $('select', row).change(); row.find('.RoboGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"cx-icon cx-icon-add cx-icon-white\"></i></button></td></tr></tfoot>", roboAttribute.Attributes.Count + 1, clientId, GetButtonSizeCssClass(ButtonSize.ExtraSmall), GetButtonStyleCssClass(ButtonStyle.Success));
                        }
                    }
                }
            }

            writer.RenderEndTag();

            if (roboAttribute.EnabledScroll)
            {
                writer.Write("</div>");
            }
        }

        public override void RenderEditableGridAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm,
            RoboEditableGridAttribute roboAttribute)
        {
            roboAttribute.EnsureProperties();
            var clientId = "div" + htmlHelper.GenerateIdFromName(roboAttribute.Name);
            var editableGridName = "editableGrid_" + htmlHelper.GenerateIdFromName(roboAttribute.Name);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-primary pull-right");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("{0}.append(new Date().getTime(), {{}}, null, true)", editableGridName));
            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginBottom, "10px");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write("<i class=\"glyphicon glyphicon-plus\"></i>&nbsp;");
            writer.Write("Add");
            writer.RenderEndTag(); // button
            writer.RenderEndTag(); // div

            writer.AddAttributes(roboAttribute.HtmlAttributes);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, clientId);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag(); // div

            var metadata = new JArray();
            var valueBuilders = new Dictionary<string, Func<object, JToken>>();

            metadata.Add(new JObject
            {
                ["name"] = "rowIndex",
                ["label"] = "#",
                ["datatype"] = "string",
                ["editable"] = false
            });

            var setCellRenderers = new StringBuilder();

            foreach (var attribute in roboAttribute.Attributes)
            {
                var propertyName = string.Concat(roboAttribute.Name, ".", attribute.PropertyName);
                if (roboForm.ExcludedProperties.Contains(propertyName))
                {
                    continue;
                }

                if (attribute is RoboHiddenAttribute)
                {
                    continue;
                }

                var jObject = new JObject
                {
                    ["name"] = attribute.PropertyName,
                    ["label"] = attribute.LabelText,
                    ["editable"] = !attribute.IsReadOnly
                };

                if (attribute.PropertyType.GetTypeInfo().IsEnum)
                {
                    jObject["datatype"] = "string";
                    valueBuilders[attribute.PropertyName] = value => Convert.ToString(value);
                }
                else
                {
                    switch (Type.GetTypeCode(attribute.PropertyType))
                    {
                        case TypeCode.Boolean:
                            jObject["datatype"] = "boolean";
                            valueBuilders[attribute.PropertyName] = value => (bool)value;
                            break;

                        case TypeCode.Char:
                        case TypeCode.String:
                            jObject["datatype"] = "string";
                            valueBuilders[attribute.PropertyName] = value => (string)value;
                            break;

                        case TypeCode.SByte:
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                            jObject["datatype"] = "integer";
                            valueBuilders[attribute.PropertyName] = value => Convert.ToInt64(value);
                            break;

                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            jObject["datatype"] = "double(m,2)";
                            valueBuilders[attribute.PropertyName] = value => Convert.ToDecimal(value);
                            break;

                        case TypeCode.DateTime:
                            jObject["datatype"] = "date";
                            valueBuilders[attribute.PropertyName] = value => (DateTime)value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (attribute is RoboChoiceAttribute)
                {
                    var roboChoiceAttribute = (RoboChoiceAttribute)attribute;
                    IList<SelectListItem> selectListItems;
                    if (attribute.PropertyType != null && attribute.PropertyType.GetTypeInfo().IsEnum)
                    {
                        var enumValues = Enum.GetValues(attribute.PropertyType);

                        selectListItems = (from object value in enumValues
                            select new SelectListItem
                            {
                                Text = GetEnumValueDescription(attribute.PropertyType, value),
                                Value = Convert.ToString(value),
                            }).ToList();
                    }
                    else
                    {
                        if (roboChoiceAttribute.SelectListItems == null)
                        {
                            var tmpSelectItems = roboForm.GetExternalDataSource(roboAttribute.Name + "." + attribute.Name);

                            if (tmpSelectItems == null)
                            {
                                tmpSelectItems = roboForm.GetExternalDataSource(attribute.Name);
                            }

                            if (tmpSelectItems == null)
                            {
                                throw new NotSupportedException("You need to register an external data source for " + attribute.Name);
                            }
                            selectListItems = tmpSelectItems.ToList();
                        }
                        else
                        {
                            selectListItems = roboChoiceAttribute.SelectListItems.ToList();
                        }
                    }

                    var childValues = new JObject();

                    foreach (var selectListItem in selectListItems)
                    {
                        childValues[selectListItem.Value] = selectListItem.Text;
                    }

                    jObject["values"] = childValues;
                }
                else if (attribute is RoboCascadingDropDownAttribute)
                {
                    var cascadingDropDownAttribute = (RoboCascadingDropDownAttribute)attribute;
                    var options = roboForm.GetCascadingDropDownDataSource(roboAttribute.Name + "." + attribute.Name);
                    var parentControl = options.ParentControl ?? cascadingDropDownAttribute.ParentControl;

                    if (string.IsNullOrEmpty(parentControl))
                    {
                        throw new ArgumentException("The ParentControl must be not null or empty.");
                    }

                    jObject["values"] = new JObject();

                    setCellRenderers.AppendFormat("{0}.setCellEditor(\"{1}\", new RoboCascadingDropDownCellEditor({{ ajaxUrl: '{2}', dependencyProperty: '{3}' }}));", editableGridName, attribute.Name, options.SourceUrl, parentControl);
                }

                metadata.Add(jObject);
            }

            metadata.Add(new JObject
            {
                ["name"] = "actions",
                ["label"] = "Actions",
                ["datatype"] = "html",
                ["textAlign"] = "center",
                ["editable"] = false
            });

            var data = new JArray();
            var values = roboAttribute.Value as IEnumerable<object>;

            if (values != null)
            {
                var index = 0;
                foreach (var value in values)
                {
                    index++;

                    var jValue = new JObject { ["id"] = index };
                    var jValues = new JObject();

                    foreach (var attribute in roboAttribute.Attributes)
                    {
                        var propertyName = string.Concat(roboAttribute.Name, ".", attribute.PropertyName);
                        if (roboForm.ExcludedProperties.Contains(propertyName))
                        {
                            continue;
                        }

                        if (attribute is RoboHiddenAttribute)
                        {
                            continue;
                        }

                        var valueBuilder = valueBuilders[attribute.PropertyName];
                        jValues[attribute.PropertyName] = valueBuilder(roboForm.GetPropertyValue(value, attribute.PropertyName));
                    }

                    jValue["values"] = jValues;

                    data.Add(jValue);
                }
            }

            var sb = new StringBuilder();
            sb.AppendFormat("var {1} = new EditableGrid(\"{0}_EditableGrid\", {{ enableSort: false, propertyName: \"{0}\" }});", roboAttribute.Name, editableGridName);
            sb.AppendFormat("{2}.load({{\"metadata\": {0}, \"data\": {1}}});", metadata.ToString(Formatting.None), data.ToString(Formatting.None), editableGridName);
            sb.AppendFormat("{0}.setCellRenderer(\"rowIndex\", new CellRenderer({{ _render: function(rowIndex, columnIndex, element, value){{ element.innerHTML = rowIndex + 1; }} }}));", editableGridName);
            sb.AppendFormat("{0}.setCellRenderer(\"actions\", new CellRenderer({{ render: function(cell, value){{ var rowId = {0}.getRowId(cell.rowIndex); cell.innerHTML = \"<a class=\\\"btn btn-xs btn-danger\\\" onclick=\\\"if (confirm('Are you sure you want to delete this row?')) {{ {0}.remove(\" + cell.rowIndex + \"); }} \\\" style=\\\"cursor:pointer\\\"><i class=\\\"glyphicon glyphicon-trash\\\" alt=\\\"delete\\\" title=\\\"Delete row\\\"></i></a>\"; }} }}));", editableGridName);
            sb.Append(setCellRenderers);
            sb.AppendFormat("{1}.renderGrid(\"{0}\", \"table table-bordered table-grid\");", clientId, editableGridName);

            var scriptRegister = roboForm.ControllerContext.HttpContext.RequestServices.GetService<IScriptRegister>();
            scriptRegister.AddInlineScript(sb.ToString());
        }

        public override void RenderHiddenFieldAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboHiddenAttribute roboAttribute)
        {
            writer.Write(htmlHelper.Hidden(roboAttribute.Name, roboAttribute.Value, roboAttribute.HtmlAttributes).ToHtmlString());
        }

        public override void RenderHtmlViewAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboHtmlViewAttribute roboAttribute)
        {
            throw new NotImplementedException();
            //var htmlHelper = new ControllerContext
            //{
            //    RouteData = htmlHelper.ViewContext.RouteData,
            //    HttpContext = htmlHelper.ViewContext.HttpContext
            //};
            //var result = ViewEngines.Engines.FindPartialView(htmlHelper, roboAttribute.ViewName);

            //if (result != null && result.View != null)
            //{
            //    var viewData = new ViewDataDictionary(htmlHelper.ViewData);
            //    var viewContext = new ViewContext(htmlHelper, result.View, viewData, new TempDataDictionary(), writer);
            //    viewData.Model = roboAttribute.Model ?? roboAttribute.Value;

            //    result.View.Render(viewContext, writer);
            //}
        }

        public override void RenderImageAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboImageAttribute roboAttribute)
        {
            if (roboAttribute.Value != null)
            {
                writer.Write("<div style=\"{0}\">", GenerateStyleAttribute("width", roboAttribute.Width));
                writer.Write("<a href=\"{0}\" class=\"thumbnail\" target=\"_blank\">", roboAttribute.Value);
                writer.Write("<img src=\"{0}\" alt=\"\" style=\"width: 100%;{1}{2}\" />", roboAttribute.Value, GenerateStyleAttribute("height", roboAttribute.Height), GenerateStyleAttribute("max-height", roboAttribute.MaxHeight));
                writer.Write("</a>");
                writer.Write("</div>");
            }
        }

        public override void RenderLabelAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboLabelAttribute roboAttribute)
        {
            var encodedValue = roboAttribute.Value == null ? string.Empty : roboAttribute.Value.ToString().HtmlEncode();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-control-static");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(encodedValue);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, roboAttribute.Name);
            writer.AddAttribute(HtmlTextWriterAttribute.Value, encodedValue);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        public override void RenderNumericAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboNumericAttribute roboAttribute)
        {
            var attributes = new Dictionary<string, object>(roboAttribute.HtmlAttributes);
            MergeHtmlAttribute(attributes, "class", ControlCssClass);

            if (roboAttribute.IsReadOnly || roboForm.ReadOnly)
            {
                MergeHtmlAttribute(attributes, "readonly", "readonly");
            }
            else
            {
                MergeHtmlAttribute(attributes, "data-val", "true");
                MergeHtmlAttribute(attributes, "data-val-number", Constants.Validation.Number);

                if (roboAttribute.IsRequired)
                {
                    MergeHtmlAttribute(attributes, "data-val-required", Constants.Validation.Required);
                }

                if (roboAttribute.MaxLength > 0)
                {
                    MergeHtmlAttribute(attributes, "maxlength", Convert.ToString(roboAttribute.MaxLength));
                }

                var typeCode = Type.GetTypeCode(roboAttribute.PropertyType);
                if (typeCode == TypeCode.Object)
                {
                    if (roboAttribute.PropertyType.Name == "Nullable`1")
                    {
                        typeCode = Type.GetTypeCode(roboAttribute.PropertyType.GetGenericArguments()[0]);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                string minimumValue = null;
                string maximumValue = null;

                switch (typeCode)
                {
                    case TypeCode.SByte:
                        minimumValue = "-128";
                        maximumValue = "127";
                        break;

                    case TypeCode.Byte:
                        minimumValue = "0";
                        maximumValue = "255";
                        break;

                    case TypeCode.Int16:
                        minimumValue = "-32768";
                        maximumValue = "32767";
                        break;

                    case TypeCode.UInt16:
                        minimumValue = "0";
                        maximumValue = "65535";
                        break;

                    case TypeCode.Int32:
                        minimumValue = "-2147483648";
                        maximumValue = "2147483647";
                        break;

                    case TypeCode.UInt32:
                        minimumValue = "0";
                        maximumValue = "4294967295";
                        break;

                    case TypeCode.Int64:
                        minimumValue = "-9223372036854775808";
                        maximumValue = "9223372036854775807";
                        break;

                    case TypeCode.UInt64:
                        minimumValue = "0";
                        maximumValue = "18446744073709551615";
                        break;

                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        break;
                }

                if (!string.IsNullOrEmpty(roboAttribute.MinimumValue))
                {
                    minimumValue = roboAttribute.MinimumValue;

                    if (minimumValue == "{YearNow}")
                    {
                        minimumValue = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                    }
                }

                if (!string.IsNullOrEmpty(roboAttribute.MaximumValue))
                {
                    maximumValue = roboAttribute.MaximumValue;

                    if (maximumValue == "{YearNow}")
                    {
                        maximumValue = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                    }
                }

                if (!string.IsNullOrEmpty(minimumValue) && !string.IsNullOrEmpty(maximumValue))
                {
                    MergeHtmlAttribute(attributes, "data-val-range-min", minimumValue);
                    MergeHtmlAttribute(attributes, "data-val-range-max", maximumValue);
                    MergeHtmlAttribute(attributes, "data-val-range", string.Format(Constants.Validation.Range, minimumValue, maximumValue));
                }
                else if (!string.IsNullOrEmpty(minimumValue))
                {
                    MergeHtmlAttribute(attributes, "data-val-range-min", minimumValue);
                    MergeHtmlAttribute(attributes, "data-val-range", string.Format(Constants.Validation.RangeMin, minimumValue));
                }
                else if (!string.IsNullOrEmpty(maximumValue))
                {
                    MergeHtmlAttribute(attributes, "data-val-range-max", minimumValue);
                    MergeHtmlAttribute(attributes, "data-val-range", string.Format(Constants.Validation.RangeMax, maximumValue));
                }
            }

            writer.Write(htmlHelper.TextBox(roboAttribute.Name, roboAttribute.Value, attributes).ToHtmlString());

            if (!string.IsNullOrEmpty(roboAttribute.HelpText))
            {
                writer.Write("<span class=\"help-block\">{0}</span>", roboAttribute.HelpText);
            }

            writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", roboAttribute.Name);
        }

        public override void RenderSlugAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboSlugAttribute roboAttribute)
        {
            var id = htmlHelper.GenerateIdFromName(roboAttribute.Name);
            var attributes = new Dictionary<string, object>(roboAttribute.HtmlAttributes);
            MergeHtmlAttribute(attributes, "class", ControlCssClass);

            if (roboAttribute.MaxLength > 0)
            {
                attributes.Add("maxlength", roboAttribute.MaxLength.ToString(CultureInfo.InvariantCulture));
            }

            MergeHtmlAttribute(attributes, "id", id);
            MergeHtmlAttribute(attributes, "name", roboAttribute.Name);
            MergeHtmlAttribute(attributes, "value", Convert.ToString(roboAttribute.Value));
            MergeHtmlAttribute(attributes, "type", "text");
            MergeHtmlAttribute(attributes, "readonly", "readonly");

            if (roboAttribute.IsReadOnly || roboForm.ReadOnly)
            {
                writer.Write(htmlHelper.TextBox(roboAttribute.Name, roboAttribute.Value, attributes));
                return;
            }

            var tagBuilder = new TagBuilder("input")
            {
                TagRenderMode = TagRenderMode.SelfClosing
            };
            tagBuilder.MergeAttributes(attributes);

            writer.Write("<div class=\"input-group\">{0}<span class=\"input-group-btn\"><button class=\"btn btn-default robo-slug-trigger\" type=\"button\" onclick=\"var $this = $('#{1}'); $this.attr('readonly') ? $this.removeAttr('readonly') : $this.attr('readonly', 'readonly');\"><i class=\"cx-icon cx-icon-edit\"></i></button></span></div>", tagBuilder.ToHtmlString(), id);
        }

        public override void RenderTextAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboTextAttribute roboAttribute)
        {
            // For security reason, does not show password value
            if (roboAttribute.Type == RoboTextType.Password)
            {
                roboAttribute.Value = string.Empty;
            }

            var htmlAttributes = new Dictionary<string, object>(roboAttribute.HtmlAttributes);

            MergeHtmlAttribute(htmlAttributes, "class", ControlCssClass);

            if (roboForm.ReadOnly || roboAttribute.IsReadOnly)
            {
                MergeHtmlAttribute(htmlAttributes, "readonly", "readonly");
            }
            else
            {
                MergeHtmlAttribute(htmlAttributes, "data-val", "true");

                if (roboAttribute.IsRequired)
                {
                    MergeHtmlAttribute(htmlAttributes, "required", "required");
                    MergeHtmlAttribute(htmlAttributes, "data-val-required", string.IsNullOrEmpty(roboAttribute.MessageValidate) ? Constants.Validation.Required : roboAttribute.MessageValidate);
                }

                if (roboAttribute.MinLength > 0 && roboAttribute.MaxLength > 0)
                {
                    MergeHtmlAttribute(htmlAttributes, "data-val-length-min", Convert.ToString(roboAttribute.MinLength));
                    MergeHtmlAttribute(htmlAttributes, "data-val-length-max", Convert.ToString(roboAttribute.MaxLength));
                    MergeHtmlAttribute(htmlAttributes, "data-val-length", string.Format(Constants.Validation.RangeLength, roboAttribute.MinLength, roboAttribute.MaxLength));
                    MergeHtmlAttribute(htmlAttributes, "maxlength", Convert.ToString(roboAttribute.MaxLength));
                }
                else if (roboAttribute.MinLength > 0)
                {
                    MergeHtmlAttribute(htmlAttributes, "data-val-length-min", Convert.ToString(roboAttribute.MinLength));
                    MergeHtmlAttribute(htmlAttributes, "data-val-length", string.Format(Constants.Validation.MinLength, roboAttribute.MinLength));
                }
                else if (roboAttribute.MaxLength > 0)
                {
                    MergeHtmlAttribute(htmlAttributes, "data-val-length-max", Convert.ToString(roboAttribute.MaxLength));
                    MergeHtmlAttribute(htmlAttributes, "data-val-length", string.Format(Constants.Validation.MaxLength, roboAttribute.MaxLength));
                    MergeHtmlAttribute(htmlAttributes, "maxlength", Convert.ToString(roboAttribute.MaxLength));
                }

                if (!string.IsNullOrEmpty(roboAttribute.RegexPattern))
                {
                    MergeHtmlAttribute(htmlAttributes, "data-val-regex-pattern", Convert.ToString(roboAttribute.RegexPattern));
                    MergeHtmlAttribute(htmlAttributes, "data-val-regex", Convert.ToString(roboAttribute.RegexValue));
                }

                switch (roboAttribute.Type)
                {
                    case RoboTextType.TextBox:
                        MergeHtmlAttribute(htmlAttributes, "type", "text");
                        break;

                    case RoboTextType.Email:
                        MergeHtmlAttribute(htmlAttributes, "data-val-email", string.IsNullOrEmpty(roboAttribute.MessageValidate) ? Constants.Validation.Email : roboAttribute.MessageValidate);
                        MergeHtmlAttribute(htmlAttributes, "type", "email");
                        break;

                    case RoboTextType.Url:
                        MergeHtmlAttribute(htmlAttributes, "data-val-url", string.IsNullOrEmpty(roboAttribute.MessageValidate) ? Constants.Validation.Url : roboAttribute.MessageValidate);
                        MergeHtmlAttribute(htmlAttributes, "type", "url");
                        break;

                    case RoboTextType.Password:
                        MergeHtmlAttribute(htmlAttributes, "type", "password");

                        if (!string.IsNullOrEmpty(roboAttribute.EqualTo))
                        {
                            MergeHtmlAttribute(htmlAttributes, "data-val-equalto", string.IsNullOrEmpty(roboAttribute.MessageValidate) ? Constants.Validation.EqualTo : roboAttribute.MessageValidate);
                            MergeHtmlAttribute(htmlAttributes, "data-val-equalto-other", roboAttribute.EqualTo);
                        }
                        break;
                }
            }

            switch (roboAttribute.Type)
            {
                case RoboTextType.MultiText:
                {
                    MergeHtmlAttribute(htmlAttributes, "rows", roboAttribute.Rows > 0
                        ? roboAttribute.Rows.ToString(CultureInfo.InvariantCulture) : "5");

                    if (roboAttribute.Cols > 0)
                    {
                        MergeHtmlAttribute(htmlAttributes, "cols", roboAttribute.Cols.ToString(CultureInfo.InvariantCulture));
                    }
                    writer.Write(htmlHelper.TextArea(roboAttribute.Name, (string)roboAttribute.Value, htmlAttributes).ToHtmlString());
                    if (!string.IsNullOrEmpty(roboAttribute.HelpText))
                    {
                        writer.Write("<span class=\"help-block\">{0}</span>", roboAttribute.HelpText);
                    }
                    writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", roboAttribute.Name);
                    break;
                }
                case RoboTextType.RichText:
                {
                    if (roboAttribute.IsReadOnly || roboForm.ReadOnly)
                    {
                        writer.Write(roboAttribute.Value);
                    }
                    else
                    {
                        htmlAttributes.Remove("class");
                        MergeHtmlAttribute(htmlAttributes, "class", "richtext ckeditor");

                        writer.Write(htmlHelper.TextArea(roboAttribute.Name, (string)roboAttribute.Value, htmlAttributes).ToHtmlString());

                        if (!string.IsNullOrEmpty(roboAttribute.HelpText))
                        {
                            writer.Write("<span class=\"help-block\">{0}</span>", roboAttribute.HelpText);
                        }
                        writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", roboAttribute.Name);
                    }
                    break;
                }
                default:
                {
                    writer.AddAttributes(htmlAttributes);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(roboAttribute.Name));
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(roboAttribute.Name));
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, Convert.ToString(roboAttribute.Value));
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag(); // input

                    if (!string.IsNullOrEmpty(roboAttribute.HelpText))
                    {
                        writer.Write("<span class=\"help-block\">{0}</span>", roboAttribute.HelpText);
                    }
                    writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", roboAttribute.Name);
                    break;
                }
            }
        }

        private void RenderCheckBox<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboChoiceAttribute attribute) where TModel : class
        {
            if (attribute.PropertyType != typeof(bool) && attribute.PropertyType != typeof(bool?))
            {
                throw new NotSupportedException("Cannot apply robo choice for non boolean property as checkbox.");
            }

            var attributes = new Dictionary<string, object>(attribute.HtmlAttributes);

            if (attribute.IsRequired)
            {
                MergeHtmlAttribute(attributes, "data-val", "true");
                MergeHtmlAttribute(attributes, "data-val-required", Constants.Validation.Required);
            }

            if (!string.IsNullOrEmpty(attribute.OnSelectedIndexChanged))
            {
                MergeHtmlAttribute(attributes, "onchange", attribute.OnSelectedIndexChanged);
            }

            if (attribute.IsReadOnly || roboForm.ReadOnly)
            {
                MergeHtmlAttribute(attributes, "disabled", "disabled");
            }

            var cssClass = attributes.ContainsKey("class") ? attributes["class"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(cssClass))
            {
                writer.Write("<div class=\"checkbox\"><label>");
            }
            else
            {
                writer.Write("<div class=\"{0}\"><label>", cssClass);
                // Remove class attribute
                attributes.Remove("class");
            }

            if (Convert.ToBoolean(attribute.Value))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }

            writer.AddAttributes(attributes);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(attribute.Name));
            writer.AddAttribute(HtmlTextWriterAttribute.Name, htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name));
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "true");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag(); // input

            if (!string.IsNullOrEmpty(attribute.LabelText))
            {
                writer.Write("<span class=\"text\">");
                writer.Write("&nbsp;");
                writer.Write(attribute.LabelText);
                writer.Write("</span>");
            }

            writer.Write("</label>");

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name));
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "false");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag(); // input

            writer.Write("</div>");

            if (!string.IsNullOrEmpty(attribute.HelpText))
            {
                writer.Write("<span class=\"help-block\">{0}</span>", attribute.HelpText);
            }
        }

        private static void RenderCheckBoxList<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboChoiceAttribute attribute) where TModel : class
        {
            if (!typeof(IEnumerable).IsAssignableFrom(attribute.PropertyType))
            {
                throw new NotSupportedException("Cannot use robo choice for non enumerable property as checkbox list.");
            }

            var value = attribute.Value as IEnumerable;
            var values = new List<string>();
            if (value != null)
            {
                values.AddRange(from object item in value select Convert.ToString(item));
            }

            IList<SelectListItem> selectItems;

            if (attribute.SelectListItems == null)
            {
                selectItems = roboForm.GetExternalDataSource(attribute.Name.RemoveBetween('[', ']'));

                if (selectItems == null)
                {
                    // Check if it's enum type
                    var listInterface = attribute.PropertyType.GetTypeInfo().GetInterface(typeof(IEnumerable<>).FullName);
                    if (listInterface != null)
                    {
                        var type = listInterface.GetGenericArguments()[0];
                        if (type.GetTypeInfo().IsEnum)
                        {
                            var enumValues = Enum.GetValues(type);

                            selectItems = (from object enumValue in enumValues
                                select new SelectListItem
                                {
                                    Text = GetEnumValueDescription(type, enumValue),
                                    Value = Convert.ToString(enumValue),
                                }).Where(x => x.Value != "None").ToList();
                        }
                    }

                    if (selectItems == null)
                    {
                        throw new NotSupportedException("You need to register an external data source for " + attribute.Name);
                    }
                }
            }
            else
            {
                selectItems = attribute.SelectListItems.ToList();
            }

            string cssClass = "checkbox";

            if (attribute.HtmlAttributes.ContainsKey("class"))
            {
                cssClass = attribute.HtmlAttributes["class"].ToString();
            }

            if (attribute.GroupedByCategory)
            {
                var items = selectItems.Cast<ExtendedSelectListItem>().ToList();
                var groups = items.GroupBy(x => x.GroupName).ToList();

                if (attribute.Columns > 1)
                {
                    var rows = (int)Math.Ceiling(groups.Count * 1d / attribute.Columns);
                    var columnWidth = 12 / attribute.Columns;
                    for (var i = 0; i < rows; i++)
                    {
                        writer.Write("<div class=\"row\">");

                        for (var j = 0; j < attribute.Columns; j++)
                        {
                            var index = i * attribute.Columns + j;
                            writer.Write("<div class=\"col-xs-{0}\">", columnWidth);

                            if (groups.Count > index)
                            {
                                var group = groups[index];

                                writer.Write("<strong>{0}</strong>", group.Key);

                                foreach (var item in group)
                                {
                                    var isChecked = values.Contains(item.Value);
                                    var checkbox = new TagBuilder("input");
                                    checkbox.TagRenderMode = TagRenderMode.SelfClosing;

                                    string name = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name);
                                    checkbox.MergeAttribute("type", "checkbox");
                                    checkbox.MergeAttribute("name", name);
                                    checkbox.MergeAttribute("id", htmlHelper.GenerateIdFromName(attribute.Name) + "_" + index);
                                    checkbox.MergeAttribute("value", item.Value);
                                    if (isChecked)
                                    {
                                        checkbox.MergeAttribute("checked", "checked");
                                    }

                                    if (attribute.IsReadOnly || roboForm.ReadOnly)
                                    {
                                        checkbox.MergeAttribute("disabled", "disabled");
                                    }

                                    writer.Write("<div class=\"{2}\"><label>{1}<span class=\"text\">{0}</span></label></div>", item.Text, checkbox.ToHtmlString(), cssClass);
                                    index++;
                                }
                            }

                            writer.Write("</div>");
                        }

                        writer.Write("</div>");
                    }
                }
                else
                {
                    int index = 0;

                    foreach (var @group in groups)
                    {
                        writer.Write("<strong>{0}</strong>", group.Key);

                        foreach (var item in group)
                        {
                            var isChecked = values.Contains(item.Value);
                            var checkbox = new TagBuilder("input");
                            checkbox.TagRenderMode = TagRenderMode.SelfClosing;

                            string name = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name);
                            checkbox.MergeAttribute("type", "checkbox");
                            checkbox.MergeAttribute("name", name);
                            checkbox.MergeAttribute("id", htmlHelper.GenerateIdFromName(attribute.Name) + "_" + index);
                            checkbox.MergeAttribute("value", item.Value);
                            checkbox.MergeAttribute("autocomplete", "off");
                            if (isChecked)
                            {
                                checkbox.MergeAttribute("checked", "checked");
                            }

                            if (attribute.IsReadOnly || roboForm.ReadOnly)
                            {
                                checkbox.MergeAttribute("disabled", "disabled");
                            }

                            writer.Write("<label for=\"{3}\" class=\"{2}\">{1}{0}</label>", WebUtility.HtmlEncode(item.Text), checkbox, cssClass, name);
                            index++;
                        }
                    }
                }
            }
            else
            {
                if (attribute.InlineControls)
                {
                    int index = 0;

                    writer.Write("<div class=\"row\">");
                    writer.Write("<div class=\"col-xs-12\">");

                    foreach (var item in selectItems)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "checkbox-inline");
                        writer.RenderBeginTag(HtmlTextWriterTag.Label);

                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                        writer.AddAttribute(HtmlTextWriterAttribute.Name, htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name));
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(attribute.Name) + "_" + index);
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);

                        if (values.Contains(item.Value))
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                        }

                        if (attribute.IsReadOnly || roboForm.ReadOnly)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                        }

                        if (attribute.IsRequired)
                        {
                            writer.AddAttribute("data-val", "true");
                            writer.AddAttribute("data-val-mandatory", Constants.Validation.Required);
                        }

                        writer.AddAttributes(attribute.HtmlAttributes);

                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag(); // input

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "text");
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write(item.Text);
                        writer.RenderEndTag(); // span

                        writer.RenderEndTag(); // label
                        index++;
                    }

                    writer.Write("</div>");
                    writer.Write("</div>");
                }
                else
                {
                    var columns = attribute.Columns > 0 ? attribute.Columns : 1;
                    var columnWidth = (int)Math.Ceiling(12d / columns);
                    int index = 0;

                    writer.Write("<div class=\"row checkboxes\">");

                    foreach (var item in selectItems)
                    {
                        writer.Write("<div class=\"col-xs-{0}\">", columnWidth);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "checkbox");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);

                        writer.RenderBeginTag(HtmlTextWriterTag.Label);

                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                        writer.AddAttribute(HtmlTextWriterAttribute.Name, htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name));
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(attribute.Name) + "_" + index);
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);

                        if (values.Contains(item.Value))
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                        }

                        if (attribute.IsReadOnly || roboForm.ReadOnly)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                        }

                        if (attribute.IsRequired)
                        {
                            writer.AddAttribute("data-val", "true");
                            writer.AddAttribute("data-val-mandatory", Constants.Validation.Required);
                        }

                        writer.AddAttributes(attribute.HtmlAttributes);

                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag(); // input

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "text");
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write(WebUtility.HtmlEncode(item.Text));
                        writer.RenderEndTag(); // span

                        writer.RenderEndTag(); // label

                        writer.RenderEndTag(); // div
                        writer.Write("</div>");
                        index++;
                    }

                    writer.Write("</div>");
                }

                writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", attribute.Name);
            }
        }

        private static void RenderSingleChoice<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboChoiceAttribute attribute) where TModel : class
        {
            var selectedValue = Convert.ToString(attribute.Value);

            IList<SelectListItem> selectItems;
            if (attribute.PropertyType != null && attribute.PropertyType.GetTypeInfo().IsEnum)
            {
                var values = Enum.GetValues(attribute.PropertyType);

                selectItems = (from object value in values
                    select new SelectListItem
                    {
                        Text = GetEnumValueDescription(attribute.PropertyType, value),
                        Value = Convert.ToString(value),
                    }).ToList();
            }
            else
            {
                if (attribute.SelectListItems == null)
                {
                    var tmpSelectItems = roboForm.GetExternalDataSource(attribute.Name);

                    if (tmpSelectItems == null && attribute.Name.Contains('['))
                    {
                        // Long : when Attribute Name contains localization
                        var attrs = attribute.Name.Split('.');
                        if (attrs.Length > 2)
                        {
                            tmpSelectItems = roboForm.GetExternalDataSource(string.Format("{0}.{1}", attrs[0], attrs[2]));
                        }
                        else
                        {
                            tmpSelectItems = roboForm.GetExternalDataSource(attribute.Name.RemoveBetween('[', ']'));
                        }
                        // Long : when Attribute Name contains localization
                    }

                    if (tmpSelectItems == null && attribute.Name.Contains('.'))
                    {
                        tmpSelectItems = roboForm.GetExternalDataSource((attribute.Name + ".").RemoveBetween('.', '.'));
                    }

                    if (tmpSelectItems == null)
                    {
                        throw new NotSupportedException("You need to register an external data source for " + attribute.Name);
                    }
                    selectItems = tmpSelectItems.ToList();
                }
                else
                {
                    selectItems = attribute.SelectListItems.ToList();
                }
            }

            if (attribute.IsReadOnly || roboForm.ReadOnly)
            {
                string selectedText = null;

                var item = selectItems.FirstOrDefault(x => x.Value == selectedValue);
                if (item != null)
                {
                    selectedText = item.Text;
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-control");
                writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, selectedText);
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag(); // input

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(attribute.Name));
                writer.AddAttribute(HtmlTextWriterAttribute.Name, htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name));
                writer.AddAttribute(HtmlTextWriterAttribute.Value, selectedValue);
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag(); // input
                return;
            }

            var attributes = new Dictionary<string, object>(attribute.HtmlAttributes);

            MergeHtmlAttribute(attributes, "class", ControlCssClass);

            if (attribute.IsRequired)
            {
                MergeHtmlAttribute(attributes, "data-val", "true");
                if (attribute.RequiredIfHaveItemsOnly == false || selectItems.Any())
                {
                    MergeHtmlAttribute(attributes, "data-val-required", Constants.Validation.Required);
                }
            }

            if (attribute.Type == RoboChoiceType.DropDownList)
            {
                #region DropDownList

                if (!string.IsNullOrEmpty(attribute.OnSelectedIndexChanged))
                {
                    MergeHtmlAttribute(attributes, "onchange", attribute.OnSelectedIndexChanged);
                }

                var selectedValues = new List<string>();

                if (attribute.AllowMultiple)
                {
                    MergeHtmlAttribute(attributes, "multiple", "multiple");

                    var value = attribute.Value as IEnumerable;
                    if (value != null)
                    {
                        selectedValues.AddRange(from object item in value select Convert.ToString(item));
                    }
                }

                writer.AddAttributes(attributes);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(attribute.Name));
                writer.AddAttribute(HtmlTextWriterAttribute.Name, htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(attribute.Name));
                writer.RenderBeginTag(HtmlTextWriterTag.Select);

                if (!attribute.IsRequired || !string.IsNullOrEmpty(attribute.OptionLabel))
                {
                    writer.Write("<option value=\"\">");
                    writer.Write(attribute.OptionLabel);
                    writer.Write("</option>");
                }

                if (attribute.GroupedByCategory)
                {
                    var items = selectItems.Cast<ExtendedSelectListItem>().ToList();
                    var groups = items.GroupBy(x => x.GroupName);

                    foreach (var group in groups)
                    {
                        writer.Write("<optgroup label=\"{0}\">", group.Key);

                        foreach (var item in group)
                        {
                            var optionTag = new TagBuilder("option");
                            optionTag.InnerHtml.AppendHtml(WebUtility.HtmlEncode(item.Text));
                            if (item.Value != null)
                            {
                                optionTag.Attributes["value"] = item.Value;
                            }

                            if (attribute.AllowMultiple)
                            {
                                if (selectedValues.Contains(item.Value))
                                {
                                    optionTag.Attributes["selected"] = "selected";
                                }
                            }
                            else
                            {
                                if (item.Value == selectedValue)
                                {
                                    optionTag.Attributes["selected"] = "selected";
                                }
                            }

                            if (item.HtmlAttributes != null)
                            {
                                var htmlAttributes = item.HtmlAttributes as IDictionary<string, object>;
                                optionTag.MergeAttributes(htmlAttributes ??
                                                          HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));
                            }

                            writer.Write(optionTag.ToHtmlString());
                        }

                        writer.Write("</optgroup>");
                    }
                }
                else
                {
                    foreach (var selectItem in selectItems)
                    {
                        var optionTag = new TagBuilder("option");
                        optionTag.InnerHtml.AppendHtml(WebUtility.HtmlEncode(selectItem.Text));
                        if (selectItem.Value != null)
                        {
                            optionTag.Attributes["value"] = selectItem.Value;
                        }

                        if (attribute.AllowMultiple)
                        {
                            if (selectedValues.Contains(selectItem.Value))
                            {
                                optionTag.Attributes["selected"] = "selected";
                            }
                        }
                        else
                        {
                            if (selectItem.Value == selectedValue)
                            {
                                optionTag.Attributes["selected"] = "selected";
                            }
                        }

                        var extendedSelectListItem = selectItem as ExtendedSelectListItem;
                        if (extendedSelectListItem != null && extendedSelectListItem.HtmlAttributes != null)
                        {
                            var htmlAttributes = extendedSelectListItem.HtmlAttributes as IDictionary<string, object>;
                            optionTag.MergeAttributes(htmlAttributes ?? HtmlHelper.AnonymousObjectToHtmlAttributes(extendedSelectListItem.HtmlAttributes));
                        }

                        writer.Write(optionTag.ToHtmlString());
                    }
                }

                writer.RenderEndTag(); // select

                if (!string.IsNullOrEmpty(attribute.HelpText))
                {
                    writer.Write("<span class=\"help-block\">{0}</span>", attribute.HelpText);
                }

                writer.Write("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", attribute.Name);

                #endregion DropDownList
            }
            else
            {
                #region Radio Buttons

                if (attribute.InlineControls)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "radios");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    var index = 0;
                    foreach (var selectItem in selectItems)
                    {
                        IDictionary<string, object> htmlAttributes = new Dictionary<string, object>(attributes);
                        htmlAttributes["class"] = "radio-inline";

                        var extendedSelectListItem = selectItem as ExtendedSelectListItem;
                        if (extendedSelectListItem != null && extendedSelectListItem.HtmlAttributes != null)
                        {
                            var routeValueDictionary = new RouteValueDictionary(extendedSelectListItem.HtmlAttributes);
                            foreach (var keyValuePair in routeValueDictionary)
                            {
                                htmlAttributes.Add(keyValuePair.Key, Convert.ToString(keyValuePair.Value));
                            }
                        }

                        writer.AddAttributes(htmlAttributes);
                        writer.RenderBeginTag(HtmlTextWriterTag.Label);

                        writer.AddAttributes(htmlAttributes);
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
                        writer.AddAttribute(HtmlTextWriterAttribute.Name, attribute.Name);
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(attribute.Name + "_" + index));
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, selectItem.Value);

                        if (selectItem.Value == selectedValue)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                        }

                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag(); // input

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "text");
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write(selectItem.Text);
                        writer.RenderEndTag(); // span

                        writer.RenderEndTag(); // label
                        index++;
                    }
                    writer.RenderEndTag(); // div
                }
                else
                {
                    var index = 0;
                    foreach (var selectItem in selectItems)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "radio");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);

                        IDictionary<string, object> htmlAttributes = new Dictionary<string, object>(attributes);

                        if (selectItem is ExtendedSelectListItem extendedSelectListItem && extendedSelectListItem.HtmlAttributes != null)
                        {
                            var routeValueDictionary = new RouteValueDictionary(extendedSelectListItem.HtmlAttributes);
                            foreach (var keyValuePair in routeValueDictionary)
                            {
                                htmlAttributes.Add(keyValuePair.Key, Convert.ToString(keyValuePair.Value));
                            }
                        }

                        writer.RenderBeginTag(HtmlTextWriterTag.Label);

                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
                        writer.AddAttribute(HtmlTextWriterAttribute.Name, attribute.Name);
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, htmlHelper.GenerateIdFromName(attribute.Name + "_" + index));
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, selectItem.Value);

                        if (selectItem.Value == selectedValue)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                        }

                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag(); // input

                        writer.Write(selectItem.Text);

                        writer.RenderEndTag(); // label
                        writer.RenderEndTag(); // div
                        index++;
                    }
                }

                #endregion Radio Buttons
            }
        }

        #endregion IRoboUIFormProvider Members

        protected virtual void RenderActions<TModel>(HtmlTextWriter writer, RoboUIFormResult<TModel> roboForm, params string[] htmlActions) where TModel : class
        {
            if (!string.IsNullOrEmpty(roboForm.FormActionsContainerCssClass) && !string.IsNullOrEmpty(roboForm.FormActionsCssClass))
            {
                writer.Write("<div class=\"{0}\">", roboForm.FormActionsContainerCssClass);

                writer.Write("<div class=\"row\"><div class=\"form-group\">");
                writer.Write("<div class=\"{0}\">", roboForm.FormActionsCssClass);
                var flag = false;
                foreach (var htmlAction in htmlActions)
                {
                    if (flag)
                    {
                        writer.Write("&nbsp;&nbsp;&nbsp;");
                    }
                    flag = true;
                    writer.Write(htmlAction);
                }

                writer.Write("</div></div></div></div>");
            }
            else
            {
                if (roboForm.IsHorizontal)
                {
                    writer.Write("<label class=\"{0}\">&nbsp;</label>", roboForm.HorizontalLabelCssClass ?? HorizontalLabelCssClass);
                    writer.Write("<div class=\"{0}\">", roboForm.HorizontalControlCssClass ?? HorizontalControlCssClass);
                    var flag = false;
                    foreach (var htmlAction in htmlActions)
                    {
                        if (flag)
                        {
                            writer.Write("&nbsp;&nbsp;&nbsp;");
                        }
                        flag = true;
                        writer.Write(htmlAction);
                    }
                    writer.Write("</div>");
                }
                else
                {
                    writer.Write("<div class=\"{0}\">", roboForm.FormActionsContainerCssClass ?? "col-sm-12");
                    var flag = false;
                    foreach (var htmlAction in htmlActions)
                    {
                        if (flag)
                        {
                            writer.Write("&nbsp;&nbsp;&nbsp;");
                        }
                        flag = true;
                        writer.Write(htmlAction);
                    }
                    writer.Write("</div>");
                }
            }
        }

        protected virtual void RenderControls<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, params RoboControlAttribute[] formAttributes) where TModel : class
        {
            if (formAttributes == null || formAttributes.Length == 0)
            {
                return;
            }

            var formAttribute = formAttributes[0];

            if (string.IsNullOrEmpty(formAttribute.ContainerCssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, FormGroupCssClass);
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, FormGroupCssClass + " " + formAttribute.ContainerCssClass);
            }

            if (formAttribute.ContainerHtmlAttributes != null)
            {
                writer.AddAttributes(formAttribute.ContainerHtmlAttributes);
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (roboForm.IsHorizontal)
            {
                if (formAttribute.HasLabelControl)
                {
                    if (formAttribute.HideLabelControl)
                    {
                        writer.Write("<label class=\"{0}\">&nbsp;</label>",
                            roboForm.HorizontalLabelCssClass ?? HorizontalLabelCssClass);
                    }
                    else
                    {
                        writer.Write("<label for=\"{2}\" class=\"{3} {1}\">{0}</label>",
                            formAttribute.LabelText ?? formAttribute.Name,
                            formAttribute.LabelCssClass,
                            formAttribute.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_"),
                            roboForm.HorizontalLabelCssClass ?? HorizontalLabelCssClass);
                    }
                }

                if (string.IsNullOrEmpty(formAttribute.ControlContainerCssClass))
                {
                    writer.Write("<div class=\"{0}\">", roboForm.HorizontalControlCssClass ?? HorizontalControlCssClass);
                    RenderControl(writer, htmlHelper, roboForm, formAttribute);
                    writer.Write("</div>");
                }
                else
                {
                    writer.Write("<div class=\"{0}\">", formAttribute.ControlContainerCssClass);
                    RenderControl(writer, htmlHelper, roboForm, formAttribute);
                    writer.Write("</div>");
                }
            }
            else
            {
                if (formAttribute.HasLabelControl)
                {
                    if (formAttribute.HideLabelControl)
                    {
                        writer.Write("<label>&nbsp;</label>");
                    }
                    else
                    {
                        writer.AddAttributeIfHave(HtmlTextWriterAttribute.Class, formAttribute.LabelCssClass);
                        writer.AddAttribute(HtmlTextWriterAttribute.For, formAttribute.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_"));
                        writer.RenderBeginTag(HtmlTextWriterTag.Label);
                        writer.Write(formAttribute.LabelText ?? formAttribute.Name);
                        writer.Write(":");

                        if (formAttribute.IsRequired)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Em);
                            writer.Write("*");
                            writer.RenderEndTag(); // em
                        }

                        writer.RenderEndTag(); // label
                    }
                }

                if (string.IsNullOrEmpty(formAttribute.ControlContainerCssClass))
                {
                    foreach (var roboControlAttribute in formAttributes)
                    {
                        RenderControl(writer, htmlHelper, roboForm, roboControlAttribute);
                    }
                }
                else
                {
                    writer.Write("<div class=\"{0}\">", formAttribute.ControlContainerCssClass);
                    foreach (var roboControlAttribute in formAttributes)
                    {
                        RenderControl(writer, htmlHelper, roboForm, roboControlAttribute);
                    }
                    writer.Write("</div>");
                }
            }

            writer.RenderEndTag(); // div
        }

        //protected virtual void RenderControl(HtmlTextWriter writer, RoboControlAttribute formAttribute, IList<RoboControlAttribute> inputControls)
        //{
        //    if (inputControls == null || inputControls.Count == 0) return;

        //    var hasLabelControl = false;
        //    var controlsCssClass = string.Empty;

        //    writer.Write(formAttribute != null && !string.IsNullOrEmpty(formAttribute.ContainerDataBind)
        //        ? string.Format("<div class=\"{1}\" data-bind=\"{0}\">", formAttribute.ContainerDataBind, FormGroupCssClass)
        //        : string.Format("<div class=\"{0}\">", FormGroupCssClass));

        //    if (formAttribute.HasLabelControl)
        //    {
        //        hasLabelControl = true;
        //        writer.Write(formAttribute.HideLabelControl
        //            ? string.Format("<label class=\"{0}\">&nbsp;</label>", LabelCssClass)
        //            : string.Format("<label class=\"{2}\" for=\"{1}\">{0}</label>",
        //                formAttribute.LabelText ?? formAttribute.PropertyName ?? formAttribute.Name,
        //                formAttribute.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_"), LabelCssClass));
        //    }

        //    if (hasLabelControl)
        //    {
        //        writer.Write("<div class=\"{0}\">", ControlsCssClass + controlsCssClass);
        //    }

        //    var renderSpaces = false;

        //    foreach (var inputControl in inputControls.Where(x => x != null))
        //    {
        //        if (renderSpaces)
        //        {
        //            writer.Write("&nbsp;&nbsp;&nbsp;");
        //        }
        //        writer.Write(inputControl);
        //        renderSpaces = true;
        //    }

        //    if (hasLabelControl)
        //    {
        //        writer.Write("</div>");
        //    }

        //    writer.Write("</div>");
        //}
    }
}
