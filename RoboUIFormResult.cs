using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using RoboUI.Extensions;

namespace RoboUI
{
    public enum RoboFormLayout : byte
    {
        Flat,
        Grouped,
        Tab
    }

    public class RoboUIFormResult : RoboUIFormResult<dynamic>
    {
        public RoboUIFormResult(ControllerContext controllerContext)
            : base(new object(), controllerContext)
        {
        }

        public override IEnumerable<RoboControlAttribute> GetProperties()
        {
            return AdditionalFields.Values;
        }
    }

    public class RoboUIFormResult<TModel> : RoboUIResult where TModel : class
    {
        #region Private Members

        private readonly IDictionary<string, string> cascadingCheckboxDataSource;
        private readonly IDictionary<string, RoboCascadingDropDownOptions> cascadingDropDownDataSource;
        private readonly ControllerContext controllerContext;
        private readonly ICollection<string> excludedProperties;
        private readonly IDictionary<string, Func<TModel, IEnumerable<SelectListItem>>> externalDataSources;
        private readonly ICollection<RoboUIGroupedLayout<TModel>> groupedLayouts;
        private readonly TModel model;
        private readonly Type modelType;
        private ICollection<RoboUIFormAction> actions;
        private IDictionary<string, RoboControlAttribute> additionalFields;
        private ICollection<string> readOnlyProperties;
        private Action<RoboUIFormResult<TModel>> setupAction;

        private IDictionary<string, object> htmlAttributes;

        #endregion Private Members

        #region Constructor

        public RoboUIFormResult(TModel model, ControllerContext controllerContext, Type modelType = null)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.modelType = modelType ?? model.GetType();
            this.controllerContext = controllerContext;
            RoboUIFormProvider = RoboUI.DefaultRoboUIFormProvider;

            GridLayouts = new Dictionary<string, GridLayout>();
            groupedLayouts = new List<RoboUIGroupedLayout<TModel>>();
            TabbedLayouts = new List<RoboUITabbedLayout<TModel>>();
            excludedProperties = new List<string>();

            cascadingCheckboxDataSource = new Dictionary<string, string>();
            cascadingDropDownDataSource = new Dictionary<string, RoboCascadingDropDownOptions>();
            externalDataSources = new Dictionary<string, Func<TModel, IEnumerable<SelectListItem>>>();

            AjaxEnabled = true;
            CancelButtonText = "Cancel";
            Layout = RoboFormLayout.Flat;
            ShowCancelButton = true;
            CancelButtonHtmlAttributes = new Dictionary<string, object>();
            ShowSubmitButton = true;
            SubmitButtonHtmlAttributes = new Dictionary<string, object>();
            SubmitButtonText = "Save";
            FormMethod = FormMethod.Post;
            ShowValidationSummary = true;
        }

        #endregion Constructor

        #region Public Properties

        public ICollection<RoboUIFormAction> Actions
        {
            get => actions ?? (actions = new List<RoboUIFormAction>());
            set => actions = value;
        }

        public IDictionary<string, RoboControlAttribute> AdditionalFields
        {
            get => additionalFields ?? (additionalFields = new Dictionary<string, RoboControlAttribute>());
            set => additionalFields = value;
        }

        public bool AjaxEnabled { get; set; }

        public IDictionary<string, object> CancelButtonHtmlAttributes { get; }

        public string CancelButtonText { get; set; }

        public string CancelButtonUrl { get; set; }

        public ControllerContext ControllerContext => controllerContext;

        public string Description { get; set; }

        public bool DisableBlockUI { get; set; }

        public bool DisableGenerateForm { get; set; }

        public ICollection<string> ExcludedProperties => excludedProperties;

        public string CssClass { get; set; }

        public string FormActionsContainerCssClass { get; set; }

        public string FormActionsCssClass { get; set; }

        public string FormActionUrl { get; set; }

        public string FormId { get; set; }

        public FormMethod FormMethod { get; set; }

        public TModel FormModel => model;

        public string FormWrapperEndHtml { get; set; }

        public string FormWrapperStartHtml { get; set; }

        public IDictionary<string, GridLayout> GridLayouts { get; }

        public ICollection<RoboUIGroupedLayout<TModel>> GroupedLayouts => groupedLayouts;

        public IDictionary<string, object> HtmlAttributes
        {
            get => htmlAttributes ?? (htmlAttributes = new Dictionary<string, object>());
            set => htmlAttributes = value;
        }

        public RoboFormLayout Layout { get; set; }

        public bool ReadOnly { get; set; }

        public ICollection<string> ReadOnlyProperties
        {
            get => readOnlyProperties ?? (readOnlyProperties = new List<string>());
            set => readOnlyProperties = value;
        }

        public bool ShowCancelButton { get; set; }

        public bool ShowCloseButton { get; set; }

        public bool ShowSubmitButton { get; set; }

        public bool ShowValidationSummary { get; set; }

        public IDictionary<string, object> SubmitButtonHtmlAttributes { get; private set; }

        public string SubmitButtonText { get; set; }

        public ICollection<RoboUITabbedLayout<TModel>> TabbedLayouts { get; }

        public string ValidationSummary { get; set; }

        public bool IsHorizontal { get; set; }

        public string HorizontalLabelCssClass { get; set; }

        public string HorizontalControlCssClass { get; set; }

        public bool RequireAntiForgeryToken { get; set; }

        #endregion Public Properties

        #region Public Methods

        public RoboUIFormAction AddAction(bool isSubmitButton = false, bool isValidationSupported = true)
        {
            var action = new RoboUIFormAction(isSubmitButton, isValidationSupported);
            Actions.Add(action);
            return action;
        }

        public RoboUIGroupedLayout<TModel> AddGroupedLayout(string title, bool getExisting = false)
        {
            if (getExisting)
            {
                var existing = groupedLayouts.FirstOrDefault(x => x.Title == title);
                if (existing != null)
                {
                    return existing;
                }
            }

            var layout = new RoboUIGroupedLayout<TModel>(title);
            GroupedLayouts.Add(layout);
            return layout;
        }

        public virtual void AddProperty(string name, RoboControlAttribute attribute, object value = null)
        {
            attribute.Name = name;
            attribute.Value = value;
            AdditionalFields[name] = attribute;
        }

        public RoboUITabbedLayout<TModel> AddTabbedLayout(string title)
        {
            var layout = new RoboUITabbedLayout<TModel>(title);
            TabbedLayouts.Add(layout);
            return layout;
        }

        public void AssignGridLayout<TValue>(Expression<Func<TModel, TValue>> expression, int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            AssignGridLayout(ExpressionHelper.GetExpressionText(expression), col, row, colSpan, rowSpan);
        }

        public void AssignGridLayout(string property, int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            if (colSpan < 1)
            {
                throw new ArgumentOutOfRangeException("colSpan");
            }

            GridLayouts.Add(property, new GridLayout(col, row, colSpan, rowSpan));
        }

        public void ExcludeProperty(string name)
        {
            excludedProperties.Add(name);
        }

        public void ExcludeProperty<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            excludedProperties.Add(ExpressionHelper.GetExpressionText(expression));
        }

        public override string GenerateView()
        {
            var htmlHelper = (IHtmlHelper)controllerContext.HttpContext.RequestServices.GetService(typeof(IHtmlHelper));
            var viewContextAware = htmlHelper as IViewContextAware;

            var viewContext = new ViewContext
            {
                HttpContext = controllerContext.HttpContext
            };

            viewContextAware?.Contextualize(viewContext);

            return RoboUIFormProvider.RenderForm(htmlHelper, this);
        }

        public virtual IEnumerable<RoboControlAttribute> GetProperties()
        {
            var cacheManager = (IStaticCacheManager)controllerContext.HttpContext.RequestServices.GetService(typeof(IStaticCacheManager));

            var attributes = cacheManager.Get("RoboForms_Properties_" + modelType.FullName, key =>
            {
                var propertyInfos = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var result = new List<RoboControlAttribute>();

                foreach (var propertyInfo in propertyInfos)
                {
                    var controlAttribute = propertyInfo.GetCustomAttribute<RoboControlAttribute>(false);

                    if (controlAttribute != null)
                    {
                        controlAttribute.Name = propertyInfo.Name;
                        controlAttribute.PropertyName = propertyInfo.Name;
                        controlAttribute.PropertyType = propertyInfo.PropertyType;
                        controlAttribute.PropertyInfo = propertyInfo;

                        if (controlAttribute.LabelText == null)
                        {
                            controlAttribute.LabelText = propertyInfo.Name;
                        }

                        var propertyHtmlAttributes = propertyInfo.GetCustomAttributes<RoboHtmlAttributeAttribute>().ToList();
                        if (propertyHtmlAttributes.Any())
                        {
                            var containerHtmlAttributes = new Dictionary<string, object>();
                            foreach (var htmlAttribute in propertyHtmlAttributes)
                            {
                                if (!htmlAttribute.IsContainer)
                                {
                                    controlAttribute.HtmlAttributes.Add(htmlAttribute.Name, htmlAttribute.Value);
                                }
                                else
                                {
                                    containerHtmlAttributes.Add(htmlAttribute.Name, htmlAttribute.Value);
                                }
                            }

                            if (containerHtmlAttributes.Count > 0)
                            {
                                controlAttribute.ContainerHtmlAttributes = containerHtmlAttributes;
                            }
                        }

                        result.Add(controlAttribute);
                    }
                }
                return result.OrderBy(x => x.Order).ToList();
            });

            var properties = new List<RoboControlAttribute>();

            foreach (var attribute in attributes)
            {
                var property = attribute.ShallowCopy();
                property.Value = GetPropertyValue(model, property.Name);
                properties.Add(property);
            }

            return properties.Concat(AdditionalFields.Values).OrderBy(x => x.Order).ToList();
        }

        public virtual object GetPropertyValue(object modelObject, string property)
        {
            if (modelObject == null)
            {
                return null;
            }

            var type = modelObject.GetType();
            var propertyInfo = type.GetProperty(property);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(modelObject);
            }

            var provider = modelObject as IDynamicMetaObjectProvider;
            if (provider != null)
            {
                dynamic dynamicObject = provider;
                return dynamicObject[property];
            }

            return null;
        }

        public void MakePropertyReadOnly<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            ReadOnlyProperties.Add(ExpressionHelper.GetExpressionText(expression));
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void SetPropertyValue(string property, object value)
        {
            var attributes = GetProperties();
            var propertyInfo = attributes.FirstOrDefault(x => x.Name == property);
            if (propertyInfo != null)
            {
                propertyInfo.PropertyInfo.SetValue(model, value);
            }
        }

        protected virtual void TryUpdateModel(TModel modelObject)
        {
            throw new NotImplementedException();
            //var binder = new DefaultModelBinder();
            //var bindingContext = new ModelBindingContext
            //{
            //    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => modelObject, typeof(TModel)),
            //    ModelState = controllerContext.Controller.ViewData.ModelState,
            //    ValueProvider = controllerContext.Controller.ValueProvider
            //};

            //binder.BindModel(controllerContext, bindingContext);
        }

        #endregion Protected Methods

        #region DataSources

        public virtual string GetCascadingCheckBoxDataSource(string property)
        {
            if (cascadingCheckboxDataSource.ContainsKey(property))
            {
                return cascadingCheckboxDataSource[property];
            }

            throw new NotSupportedException(string.Format("You must register a cascading dropdown data source for '{0}'.", property));
        }

        public virtual RoboCascadingDropDownOptions GetCascadingDropDownDataSource(string property)
        {
            if (cascadingDropDownDataSource.ContainsKey(property))
            {
                return cascadingDropDownDataSource[property];
            }

            throw new NotSupportedException(string.Format("You must register a cascading dropdown data source for '{0}'.", property));
        }

        public virtual IList<SelectListItem> GetExternalDataSource(string property)
        {
            if (!externalDataSources.ContainsKey(property))
            {
                return null;
            }

            var dataSource = externalDataSources[property];
            return dataSource.Invoke(model).ToList();
        }

        public void RegisterCascadingCheckboxDataSource(string property, string sourceUrl)
        {
            cascadingCheckboxDataSource.Add(property, sourceUrl);
        }

        public void RegisterCascadingDropDownDataSource<TValue>(Expression<Func<TModel, TValue>> expression, string sourceUrl)
        {
            cascadingDropDownDataSource.Add(ExpressionHelper.GetExpressionText(expression), new RoboCascadingDropDownOptions { SourceUrl = sourceUrl });
        }

        public void RegisterCascadingDropDownDataSource<TValue>(Expression<Func<TModel, TValue>> expression, RoboCascadingDropDownOptions options)
        {
            cascadingDropDownDataSource.Add(ExpressionHelper.GetExpressionText(expression), options);
        }

        public void RegisterCascadingDropDownDataSource(string property, string sourceUrl)
        {
            cascadingDropDownDataSource.Add(property, new RoboCascadingDropDownOptions { SourceUrl = sourceUrl });
        }

        public void RegisterCascadingDropDownDataSource(string property, RoboCascadingDropDownOptions options)
        {
            cascadingDropDownDataSource.Add(property, options);
        }

        public void RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, Func<TModel, IEnumerable<SelectListItem>> items)
        {
            var str = ExpressionHelper.GetExpressionText(expression);
            externalDataSources[str] = items;
        }

        public void RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> items)
        {
            var str = ExpressionHelper.GetExpressionText(expression);
            externalDataSources[str] = x => items;
        }

        public void RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, params string[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            var func = new Func<TModel, List<SelectListItem>>(item => values.Select(value => new SelectListItem
            {
                Text = value,
                Value = value
            }).ToList());
            externalDataSources.Add(ExpressionHelper.GetExpressionText(expression), func);
        }

        public void RegisterExternalDataSource(string property, Func<TModel, IEnumerable<SelectListItem>> items)
        {
            externalDataSources.Add(property, items);
        }

        public void RegisterExternalDataSource(string property, params string[] values)
        {
            RegisterExternalDataSource(property, values.ToSelectList(k => k, v => v));
        }

        public void RegisterExternalDataSource(string property, IEnumerable<string> values)
        {
            RegisterExternalDataSource(property, values.ToSelectList(k => k, v => v));
        }

        public void RegisterExternalDataSource(string property, IEnumerable<SelectListItem> items)
        {
            externalDataSources[property] = m => items;
        }

        #endregion DataSources

        public RoboUIFormResult<TModel> Setup(Action<RoboUIFormResult<TModel>> action)
        {
            setupAction = action;
            return this;
        }

        public string ToHtmlString()
        {
            setupAction?.Invoke(this);

            return GenerateView();
        }
    }
}
