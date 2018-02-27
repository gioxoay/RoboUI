using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoboUI.Extensions;

namespace RoboUI
{
    public class RoboUIGridResult<TModel> : RoboUIResult where TModel : class
    {
        #region Private Members

        private readonly ICollection<RoboUIFormAction> actions;
        private readonly ICollection<RoboUIGridColumn<TModel>> columns;
        private readonly ControllerContext controllerContext;
        private readonly IDictionary<string, string> customVariables;
        private readonly ICollection<string> reloadEvents;
        private readonly ICollection<RoboUIGridRowAction<TModel>> rowActions;
        private Action<RoboUIGridResult<TModel>> setupAction;
        private string clientId;

        #endregion Private Members

        #region Constructors

        public RoboUIGridResult(ControllerContext controllerContext)
        {
            this.controllerContext = controllerContext;
            columns = new List<RoboUIGridColumn<TModel>>();
            actions = new List<RoboUIFormAction>();
            rowActions = new List<RoboUIGridRowAction<TModel>>();
            reloadEvents = new List<string>();
            customVariables = new Dictionary<string, string>();
            IsAjaxSupported = true;
            DefaultPageSize = 10;
            EnableSortable = true;
            EnableFilterable = true;
            ActionsHeaderText = "Actions";
            RecordsInfoPosition = "right";
            GetModelId = model => model.GetHashCode();
        }

        #endregion Constructors

        #region Properties

        public ICollection<RoboUIFormAction> Actions => actions;

        public int? ActionsColumnWidth { get; set; }

        public string ActionsHeaderText { get; set; }

        public string ClientId
        {
            get
            {
                if (string.IsNullOrEmpty(clientId))
                {
                    clientId = "roboGrid";
                }
                return clientId;
            }
            set => clientId = value;
        }

        public ICollection<RoboUIGridColumn<TModel>> Columns => columns;

        public ControllerContext ControllerContext => controllerContext;

        public string CssClass { get; set; }

        public IDictionary<string, string> CustomVariables => customVariables;

        public int DefaultPageSize { get; set; }

        public bool EnableCheckboxes { get; set; }

        public bool EnablePageSizeChange { get; set; }

        public bool EnablePaginate { get; set; }

        public bool EnableSearch { get; set; }

        public bool EnableShowHideGrid { get; set; }

        public bool EnableSortable { get; set; }

        public bool EnableFilterable { get; set; }

        public string Height { get; set; }

        public Func<RoboUIGridRequest, Task<RoboUIGridAjaxData<TModel>>> FetchAjaxSource { get; set; }

        public Func<TModel, object> GetModelId { get; set; }

        public string GetRecordsUrl { get; set; }

        public string GridWrapperEndHtml { get; set; }

        public string GridWrapperStartHtml { get; set; }

        public bool HideActionsColumn { get; set; }

        public bool HidePagerWhenEmpty { get; set; }

        public bool IsAjaxSupported { get; set; }

        public string RecordsInfoPosition { get; set; }

        public ICollection<string> ReloadEvents => reloadEvents;

        public ICollection<RoboUIGridRowAction<TModel>> RowActions => rowActions;

        public bool ShowFooterRow { get; set; }

        public string FormActionUrl { get; set; }

        public RoboUIResult FilterForm { get; set; }

        public bool DisableBlockUI { get; set; }

        public bool Editable { get; set; }

        public IRoboUIGridProvider RoboUIGridProvider { get; set; }

        public Action OnAfterRender { get; set; }

        #endregion Properties

        #region Public Methods

        public RoboUIFormAction AddAction(bool isSubmitButton = false, bool isAjaxSupport = true)
        {
            var action = new RoboUIFormAction(isSubmitButton, isAjaxSupport);
            actions.Add(action);
            return action;
        }

        public RoboUIFormAction AddAction(RoboUIFormAction action)
        {
            actions.Add(action);
            return action;
        }

        public RoboUIGridColumn<TModel> AddColumn<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return AddColumn(expression, null);
        }

        public RoboUIGridColumn<TModel> AddColumn<TValue>(Expression<Func<TModel, TValue>> expression, string headerText)
        {
            var column = new RoboUIGridColumn<TModel>();
            column.SetValueGetter(expression.Compile());

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                var property = memberExpression.Member as PropertyInfo;
                if (property != null)
                {
                    column.SetValueSetter((model, value) => property.SetValue(model, value));
                }
            }

            if (!string.IsNullOrEmpty(headerText))
            {
                column.HeaderText = headerText;
            }
            else
            {
                //var modelMetadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TModel>());
                //column.HeaderText = modelMetadata.DisplayName ?? modelMetadata.PropertyName;
            }
            column.PropertyName = Utils.GetFullPropertyName(expression);
            column.PropertyType = typeof(TValue);

            columns.Add(column);
            return column;
        }

        public RoboUIGridColumn<TModel> AddColumn(string columnName)
        {
            var column = new RoboUIGridColumn<TModel> { PropertyName = columnName };
            columns.Add(column);
            return column;
        }

        public void AddCustomVariable(string name, object value, bool isFunction = false)
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }

            if (isFunction || !(value is string))
            {
                if (isFunction)
                {
                    customVariables.Add(name, "function(){ return " + value + " }");
                }
                else
                {
                    customVariables.Add(name, value.ToString());
                }
            }
            else
            {
                customVariables.Add(name, value.ToString().JsEncode());
            }
        }

        public void AddReloadEvent(string eventName)
        {
            reloadEvents.Add(eventName);
        }

        public RoboUIGridRowAction<TModel> AddRowAction(bool isSubmitButton = false, bool isValidationSupported = true)
        {
            var action = new RoboUIGridRowAction<TModel>(isSubmitButton, isValidationSupported);
            rowActions.Add(action);
            return action;
        }

        public void AddRowAction(RoboUIGridRowAction<TModel> action)
        {
            rowActions.Add(action);
        }

        public override string GenerateView()
        {
            var gridProvider = RoboUIGridProvider ?? RoboUI.DefaultRoboUIGridProvider;

            var htmlHelper = (IHtmlHelper)controllerContext.HttpContext.RequestServices.GetService(typeof(IHtmlHelper));

            try
            {
                var gridResult = gridProvider.Render(this, htmlHelper);
                OnAfterRender?.Invoke();
                return gridResult;
            }
            catch (Exception ex)
            {
                return ex.GetBaseException().Message;
            }
        }

        public override async Task<bool> OverrideExecuteResult()
        {
            if (controllerContext.HttpContext.Request.IsAjaxRequest())
            {
                var gridProvider = RoboUIGridProvider ?? RoboUI.DefaultRoboUIGridProvider;

                try
                {
                    // Return data only
                    var request = gridProvider.CreateGridRequest(controllerContext);

                    if (request.PageIndex <= 0)
                    {
                        request.PageIndex = 1;
                    }

                    if (request.PageSize <= 0)
                    {
                        request.PageSize = DefaultPageSize;
                    }

                    await gridProvider.ExecuteGridRequest(this, request, controllerContext);
                }
                catch (Exception ex)
                {
                    gridProvider.HandingError(controllerContext, ex);
                }
                return true;
            }
            return await base.OverrideExecuteResult();
        }

        #endregion Public Methods

        public RoboUIGridResult<TModel> Setup(Action<RoboUIGridResult<TModel>> action)
        {
            setupAction = action;
            return this;
        }

        public string GetReloadClientScript()
        {
            return (RoboUIGridProvider ?? RoboUI.DefaultRoboUIGridProvider).GetReloadClientScript(ClientId);
        }

        public IHtmlContent Render()
        {
            setupAction?.Invoke(this);
            var html = GenerateView();
            var builder = new HtmlContentBuilder().AppendHtml(html);
            return builder;
        }
    }
}
