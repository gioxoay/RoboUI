using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace RoboUI
{
    public abstract class BaseRoboUIFormProvider : IRoboUIFormProvider
    {
        #region IRoboUIFormProvider Members

        public virtual string GetButtonSizeCssClass(ButtonSize buttonSize)
        {
            throw new NotSupportedException();
        }

        public virtual string GetButtonStyleCssClass(ButtonStyle buttonStyle)
        {
            throw new NotSupportedException();
        }

        public virtual string RenderAction(IHtmlHelper htmlHelper, RoboUIFormAction action)
        {
            throw new NotSupportedException();
        }

        public virtual string RenderActions(IHtmlHelper htmlHelper, IEnumerable<RoboUIFormAction> actions)
        {
            throw new NotSupportedException();
        }

        public virtual void RenderControl<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboControlAttribute roboAttribute) where TModel : class
        {
            if (roboAttribute is RoboTextAttribute)
            {
                RenderTextAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboTextAttribute);
            }
            else if (roboAttribute is RoboNumericAttribute)
            {
                RenderNumericAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboNumericAttribute);
            }
            else if (roboAttribute is RoboChoiceAttribute)
            {
                RenderChoiceAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboChoiceAttribute);
            }
            else if (roboAttribute is RoboHiddenAttribute)
            {
                RenderHiddenFieldAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboHiddenAttribute);
            }
            else if (roboAttribute is RoboButtonAttribute)
            {
                RenderButtonAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboButtonAttribute);
            }
            else if (roboAttribute is RoboCascadingCheckBoxListAttribute)
            {
                RenderCascadingCheckBoxListAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboCascadingCheckBoxListAttribute);
            }
            else if (roboAttribute is RoboCascadingDropDownAttribute)
            {
                RenderCascadingDropDownAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboCascadingDropDownAttribute);
            }
            else if (roboAttribute is RoboComplexAttribute)
            {
                RenderComplexAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboComplexAttribute);
            }
            else if (roboAttribute is RoboDatePickerAttribute)
            {
                RenderDatePickerAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboDatePickerAttribute);
            }
            else if (roboAttribute is RoboTimePickerAttribute)
            {
                RenderTimePickerAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboTimePickerAttribute);
            }
            else if (roboAttribute is RoboDivAttribute)
            {
                RenderDivAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboDivAttribute);
            }
            else if (roboAttribute is RoboEditableGridAttribute)
            {
                RenderEditableGridAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboEditableGridAttribute);
            }
            else if (roboAttribute is RoboGridAttribute)
            {
                RenderGridAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboGridAttribute);
            }
            else if (roboAttribute is RoboHtmlViewAttribute)
            {
                RenderHtmlViewAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboHtmlViewAttribute);
            }
            else if (roboAttribute is RoboImageAttribute)
            {
                RenderImageAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboImageAttribute);
            }
            else if (roboAttribute is RoboLabelAttribute)
            {
                RenderLabelAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboLabelAttribute);
            }
            else if (roboAttribute is RoboSlugAttribute)
            {
                RenderSlugAttribute(writer, htmlHelper, roboForm, roboAttribute as RoboSlugAttribute);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void RenderButtonAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboButtonAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderCascadingCheckBoxListAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboCascadingCheckBoxListAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderCascadingDropDownAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboCascadingDropDownAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderChoiceAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboChoiceAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderComplexAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboComplexAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderDatePickerAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboDatePickerAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderTimePickerAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboTimePickerAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderDivAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboDivAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual string RenderForm<TModel>(IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderGridAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboGridAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderEditableGridAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboEditableGridAttribute roboEditableGridAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderHiddenFieldAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboHiddenAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderHtmlViewAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboHtmlViewAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderImageAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboImageAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderLabelAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboLabelAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderNumericAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboNumericAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderSlugAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboSlugAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        public virtual void RenderTextAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboTextAttribute roboAttribute) where TModel : class
        {
            throw new NotSupportedException();
        }

        #endregion IRoboUIFormProvider Members

        protected static string ConvertDateFormat(string format)
        {
            /* 
                d, dd: Numeric date, no leading zero and leading zero, respectively.Eg, 5, 05.
                D, DD: Abbreviated and full weekday names, respectively.Eg, Mon, Monday.
                m, mm: Numeric month, no leading zero and leading zero, respectively.Eg, 7, 07.
                M, MM: Abbreviated and full month names, respectively.Eg, Jan, January
                yy, yyyy: 2 - and 4 - digit years, respectively.Eg, 12, 2012.
            */

            var currentFormat = format;

            // Convert the date
            currentFormat = currentFormat.Replace("dddd", "DD");
            currentFormat = currentFormat.Replace("ddd", "D");

            // Convert month
            if (currentFormat.Contains("MMMM"))
            {
                currentFormat = currentFormat.Replace("MMMM", "MM");
            }
            else if (currentFormat.Contains("MMM"))
            {
                currentFormat = currentFormat.Replace("MMM", "M");
            }
            else if (currentFormat.Contains("MM"))
            {
                currentFormat = currentFormat.Replace("MM", "mm");
            }
            else
            {
                currentFormat = currentFormat.Replace("M", "m");
            }

            return currentFormat;
        }

        protected static bool DetectSpanCells(GridLayout gridLayout, int row, int column)
        {
            if (gridLayout.Column == 0 && gridLayout.Row == 0)
            {
                return false;
            }

            if (gridLayout.ColumnSpan == 1 && gridLayout.RowSpan == 1)
            {
                return false;
            }

            if (gridLayout.Column == column && gridLayout.Row == row)
            {
                return false;
            }

            if (gridLayout.Row > row)
            {
                return false;
            }

            if (gridLayout.Column > column)
            {
                return false;
            }

            if (gridLayout.Row + gridLayout.RowSpan > row && gridLayout.Column + gridLayout.ColumnSpan > column)
            {
                return true;
            }

            return false;
        }

        protected static string GenerateStyleAttribute(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return string.Concat(name, ":", value, ";");
            }
            return null;
        }

        protected static string GetEnumValueDescription(Type type, object value)
        {
            var field = type.GetField(Convert.ToString(value));
            var attrs = field.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attrs.Any())
            {
                return ((DisplayAttribute)attrs.ElementAt(0)).GetName();
            }

            return Convert.ToString(value);
        }

        protected static void MergeHtmlAttribute(IDictionary<string, object> htmlAttributes, string key, string value)
        {
            if (htmlAttributes.ContainsKey(key))
            {
                if (key == "class")
                {
                    if (!htmlAttributes[key].ToString().Contains(value))
                    {
                        htmlAttributes[key] = value + " " + htmlAttributes[key];
                    }
                }
                else if (key == "style")
                {
                    htmlAttributes[key] += ";" + value;
                }
                else
                {
                    htmlAttributes[key] = value;
                }
            }
            else
            {
                htmlAttributes.Add(key, value);
            }
        }

        protected virtual string BeginForm<TModel>(IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, string formId) where TModel : class
        {
            var formActionUrl = string.IsNullOrEmpty(roboForm.FormActionUrl)
                ? htmlHelper.ViewContext.HttpContext.Request.Path.ToString()
                : roboForm.FormActionUrl;

            var form = new FluentTagBuilder("form", TagRenderMode.StartTag)
                .MergeAttribute("role", "form")
                .MergeAttribute("action", formActionUrl)
                .MergeAttribute("id", formId)
                .MergeAttribute("method", roboForm.FormMethod.ToString().ToLowerInvariant());

            if (roboForm.IsHorizontal)
            {
                form.MergeAttribute("class", "form-horizontal");
            }

            var formIdAsName = formId.Replace("-", "_");

            if (roboForm.AjaxEnabled && roboForm.Layout == RoboFormLayout.Tab)
            {
                form.MergeAttribute("data-ajax-begin", formIdAsName + "_ValidateTabs");
            }

            if (roboForm.AjaxEnabled)
            {
                form.MergeAttribute("data-ajax", "true");
                form.MergeAttribute("data-ajax-failure", formIdAsName + "_OnFailure");
                var scriptRegister = roboForm.ControllerContext.HttpContext.RequestServices.GetService<IScriptRegister>();
                scriptRegister.AddInlineScript(string.Format("function {0}_OnFailure(xhr, status, error){{ var ct = xhr.getResponseHeader('content-type') || ''; if(ct.indexOf('application/x-javascript') == -1) {{ alert(xhr.responseText); }} }}", formIdAsName));
            }
            else
            {
                form.MergeAttribute("enctype", "multipart/form-data");
            }

            form.MergeAttribute("method", roboForm.FormMethod.ToString().ToLowerInvariant());
            form.MergeAttributes(roboForm.HtmlAttributes);

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
    }
}
