using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RoboUI
{
    public interface IRoboUIFormProvider
    {
        string GetButtonSizeCssClass(ButtonSize buttonSize);

        string GetButtonStyleCssClass(ButtonStyle buttonStyle);

        string RenderActions(IHtmlHelper htmlHelper, IEnumerable<RoboUIFormAction> actions);

        string RenderAction(IHtmlHelper htmlHelper, RoboUIFormAction action);

        string RenderForm<TModel>(IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm) where TModel : class;

        void RenderControl<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboControlAttribute roboAttribute) where TModel : class;

        void RenderButtonAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboButtonAttribute roboAttribute) where TModel : class;

        void RenderCascadingCheckBoxListAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboCascadingCheckBoxListAttribute roboAttribute) where TModel : class;

        void RenderCascadingDropDownAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboCascadingDropDownAttribute roboAttribute) where TModel : class;

        void RenderChoiceAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboChoiceAttribute roboAttribute) where TModel : class;

        void RenderComplexAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboComplexAttribute roboAttribute) where TModel : class;

        void RenderDatePickerAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboDatePickerAttribute roboAttribute) where TModel : class;

        void RenderDivAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboDivAttribute roboAttribute) where TModel : class;

        void RenderGridAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboGridAttribute roboAttribute) where TModel : class;

        void RenderHiddenFieldAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboHiddenAttribute roboAttribute) where TModel : class;

        void RenderHtmlViewAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboHtmlViewAttribute roboAttribute) where TModel : class;

        void RenderImageAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboImageAttribute roboAttribute) where TModel : class;

        void RenderLabelAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboLabelAttribute roboAttribute) where TModel : class;

        void RenderNumericAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboNumericAttribute roboAttribute) where TModel : class;

        void RenderSlugAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboSlugAttribute roboAttribute) where TModel : class;

        void RenderTextAttribute<TModel>(HtmlTextWriter writer, IHtmlHelper htmlHelper, RoboUIFormResult<TModel> roboForm, RoboTextAttribute roboAttribute) where TModel : class;
    }

    public static class RoboUI
    {
        static RoboUI()
        {
            DefaultRoboUIFormProvider = new Bootstrap3RoboUIFormProvider();
            DefaultRoboUIGridProvider = new KendoRoboUIGridProvider();
        }

        public static IRoboUIFormProvider DefaultRoboUIFormProvider { get; set; }

        public static IRoboUIGridProvider DefaultRoboUIGridProvider { get; set; }
    }
}
