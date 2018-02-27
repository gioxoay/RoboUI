using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoboUI.Extensions;
using RoboUI.Filters;

namespace RoboUI
{
    public class KendoRoboUIGridProvider : BaseRoboUIGridProvider
    {
        public override string Render<TModel>(RoboUIGridResult<TModel> roboUIGrid, IHtmlHelper htmlHelper)
        {
            var sb = new StringBuilder(2048);

            if (!string.IsNullOrEmpty(roboUIGrid.GridWrapperStartHtml))
            {
                Write(sb, roboUIGrid.GridWrapperStartHtml);
            }

            // Start div container
            Write(sb, string.Format("<div class=\"robo-grid-container\" id=\"{0}_Container\">", roboUIGrid.ClientId));

            var form = BeginForm(htmlHelper, roboUIGrid);
            Write(sb, form);

            if (roboUIGrid.FilterForm != null)
            {
                var filterForm = roboUIGrid.FilterForm.GenerateView();
                Write(sb, filterForm);
            }

            Write(sb, string.Format("<div class=\"{0}\" id=\"{1}\"></div>", roboUIGrid.CssClass, roboUIGrid.ClientId));

            // Hidden values
            foreach (var hiddenValue in roboUIGrid.HiddenValues)
            {
                Write(sb, string.Format("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\"/>", hiddenValue.Key, WebUtility.HtmlEncode(hiddenValue.Value)));
            }

            if (!string.IsNullOrEmpty(form))
            {
                Write(sb, "</form>");
            }

            // End div container
            Write(sb, "</div>");

            if (!string.IsNullOrEmpty(roboUIGrid.GridWrapperEndHtml))
            {
                Write(sb, roboUIGrid.GridWrapperEndHtml);
            }

            #region Kendo UI Options

            var getRecordsUrl = string.IsNullOrEmpty(roboUIGrid.GetRecordsUrl)
                ? roboUIGrid.ControllerContext.HttpContext.Request.Path.ToString()
                : roboUIGrid.GetRecordsUrl;

            var columns = new JArray();
            var modelFields = new JObject();

            if (roboUIGrid.EnableCheckboxes)
            {
                columns.Add(new JObject(
                    new JProperty("template", "<input type='checkbox' class='k-checkbox checkbox' name='selectedIds' />"),
                    new JProperty("width", 30)
                ));
            }

            foreach (var column in roboUIGrid.Columns)
            {
                var fieldType = GetFieldType(column.CustomType == false ? column.PropertyType : column.TypeFormat);

                var options = new JObject(
                    new JProperty("field", column.PropertyName),
                    new JProperty("filterable", column.Filterable),
                    new JProperty("encoded", false),
                    new JProperty("title", column.HeaderText),
                    new JProperty("hidden", column.Hidden),
                    new JProperty("sortable", column.Sortable),
                    new JProperty("attributes", new JObject
                    {
                        {"style", "text-align:" + column.Align + ";"}
                    }),
                    new JProperty("headerAttributes", new JObject
                    {
                        {"style", "text-align:" + column.Align + ";"}
                    }));

                if (column.Width.HasValue)
                {
                    options.Add("width", column.Width.Value);
                }

                var fieldModel = new JObject { { "type", fieldType } };

                if (fieldType == "date")
                {
                    fieldModel.Add("parse", new JRaw("function(data){ if(!data || data.length == 0) return null; return new Date(data); }"));
                    options.Add("template", new JRaw(string.Format("function(dataItem){{ return dataItem.{0}Formated; }}", column.PropertyName)));

                    if (!string.IsNullOrEmpty(column.DisplayFormat))
                    {
                        options.Add("format", string.Format("{{0:{0}}}", column.DisplayFormat));
                    }
                    else
                    {
                        options.Add("format", string.Format("{{0:{0}}}", column.DisplayFormat));
                    }
                }
                else if (fieldType == "boolean")
                {
                    fieldModel.Add("parse", new JRaw("function(data){ return data; }"));
                }

                columns.Add(options);
                modelFields.Add(column.PropertyName, fieldModel);
            }
            var postData = new JObject();

            if (roboUIGrid.CustomVariables.Count > 0)
            {
                foreach (var customrVar in roboUIGrid.CustomVariables)
                {
                    postData.Add(customrVar.Key, new JRaw(customrVar.Value));
                }
            }

            var dataSourceOptions = new JObject
            {
                {
                    "type", new JRaw("(function(){if(kendo.data.transports['aspnetmvc-ajax']){return 'aspnetmvc-ajax';} else{throw new Error('The kendo.aspnetmvc.min.js script is not included.');}})()")
                },
                {
                    "transport", new JObject
                    {
                        {
                            "read", new JObject
                            {
                                {"url", getRecordsUrl},
                                {"dataType", "json"},
                                {"type", "POST"},
                                {"data", postData}
                            }
                        },
                        {
                            "update", new JObject
                            {
                                {"url", getRecordsUrl},
                                {"dataType", "json"},
                                {"type", "POST"},
                                {"data", new JObject{{"Save", 1}}}
                            }
                        }
                    }
                },
                {
                    "requestEnd", new JRaw("function(e){ if(e.response.callback){ eval(e.response.callback); } }")
                },
                {
                    "schema", new JObject
                    {
                        {"data", "results"},
                        {"total", "_count"},
                        {
                            "model", new JObject
                            {
                                {"id", "_id"},
                                {"fields", modelFields}
                            }
                        },
                        {
                            "errors", new JRaw("function(response){if(response.errors){ if(response.redirectUrl){window.location = response.redirectUrl;}else{ return response.errors; }} return null;}")
                        }
                    }
                },
                {
                    "error", new JRaw("function(e){ alert(e.errors); }")
                },
                {"serverPaging", roboUIGrid.EnablePaginate},
                {"pageSize", roboUIGrid.EnablePaginate ? roboUIGrid.DefaultPageSize : int.MaxValue},
                {"serverFiltering", true},
                {"serverSorting", true}
            };

            if (roboUIGrid.RowActions.Count > 0 && !roboUIGrid.HideActionsColumn)
            {
                var options = new JObject(
                    new JProperty("field", "_RowActions"),
                    new JProperty("filterable", false),
                    new JProperty("sortable", false),
                    new JProperty("groupable", false),
                    new JProperty("encoded", false),
                    new JProperty("menu", false),
                    new JProperty("title", roboUIGrid.ActionsHeaderText),
                    new JProperty("headerAttributes", new JObject
                    {
                        {"style", "text-align: center;"}
                    }),
                    new JProperty("attributes", new JObject
                    {
                        {"style", "text-align: center; overflow: visible;"}
                    }));

                if (roboUIGrid.ActionsColumnWidth.HasValue)
                {
                    options.Add("width", roboUIGrid.ActionsColumnWidth.Value);
                }

                columns.Add(options);
            }

            var dataTableOptions = new JObject
            {
                {"dataSource", dataSourceOptions},
                {"sortable", roboUIGrid.EnableSortable},
                {"resizable", false},
                {"scrollable", true},
                {"columnMenu", false},
                {"columns", columns}
            };

            if (roboUIGrid.EnableFilterable && roboUIGrid.Columns.Any(x => x.Filterable))
            {
                dataTableOptions["filterable"] = new JObject
                {
                    {
                        "operators", new JObject
                        {
                            {
                                "string", new JObject
                                {
                                    { "contains", "Contains" },
                                    { "doesnotcontain", "Does not contain" },
                                    { "eq", "Is equal to" },
                                    { "neq", "Is not equal to" },
                                    { "startswith", "Starts with" },
                                    { "endswith", "Ends with" }
                                }
                            }
                        }
                    }
                };
            }

            if (roboUIGrid.EnablePaginate)
            {
                dataTableOptions.Add("pageable", new JObject
                {
                    {"refresh", true},
                    {"pageSize", roboUIGrid.DefaultPageSize},
                    {"buttonCount", 10}
                });
            }
            else
            {
                dataTableOptions.Add("pageable", false);
            }

            if (!string.IsNullOrEmpty(roboUIGrid.Height))
            {
                dataTableOptions.Add("height", roboUIGrid.Height);
            }

            // Toolbar
            if (roboUIGrid.Actions.Count > 0)
            {
                var toolbarBuilder = new StringBuilder();
                foreach (var action in roboUIGrid.Actions.OrderBy(x => x.Order))
                {
                    action.HasButtonSize(ButtonSize.Small);
                    toolbarBuilder.Append(RenderAction(htmlHelper, action));
                }
                dataTableOptions.Add("toolbar", toolbarBuilder.ToString().Replace("#", "\\#"));
            }

            var scriptRegister = roboUIGrid.ControllerContext.HttpContext.RequestServices.GetService<IScriptRegister>();
            scriptRegister.AddInlineScript(string.Format("var grid = $('#{0}').kendoGrid({1}).data('kendoGrid');", roboUIGrid.ClientId, dataTableOptions.ToString(Formatting.None)));

            if (roboUIGrid.EnableCheckboxes)
            {
                scriptRegister.AddInlineScript(string.Format("grid.table.on('click', '.k-checkbox' , function(){{ var checked = this.checked, row = $(this).closest('tr'), grid = $('#{0}').data('kendoGrid'), dataItem = grid.dataItem(row); if(checked){{ row.addClass('k-state-selected'); this.value = dataItem.id; }}else{{ row.removeClass('k-state-selected'); }} }});", roboUIGrid.ClientId));
            }

            if (roboUIGrid.ReloadEvents.Count > 0)
            {
                scriptRegister.AddInlineScript(string.Format("$('body').bind('SystemMessageEvent', function(event){{ var events = [{1}]; if(events.indexOf(event.SystemMessage) > -1){{ $('#{0}').data('kendoGrid').dataSource.read(); }} }});", roboUIGrid.ClientId, string.Join(", ", roboUIGrid.ReloadEvents.Select(x => "'" + x + "'"))));
            }

            #endregion Kendo UI Options

            return sb.ToString();
        }

        public override RoboUIGridRequest CreateGridRequest(ControllerContext controllerContext)
        {
            var result = new RoboUIGridRequest();
            var request = controllerContext.HttpContext.Request;

            var page = request.Form["page"];
            if (page.Count > 0)
            {
                result.PageIndex = Convert.ToInt32(page);
            }

            var pageSize = request.Form["pageSize"];
            if (pageSize.Count > 0)
            {
                result.PageSize = Convert.ToInt32(pageSize);
            }

            var sort = (string)request.Form["sort"];
            if (!string.IsNullOrEmpty(sort))
            {
                result.Sorts = GridDescriptorSerializer.Deserialize<SortDescriptor>(sort);
            }

            var value = (string)request.Form["filter"];

            if (!string.IsNullOrEmpty(value))
            {
                // Process [today], [beginWeek], [endWeek], [beginMonth], [endMonth], [beginPrevMonth] tokens
                var dtNow = DateTime.UtcNow.Date;
                int startIndex;
                var endIndex = 0;
                while ((startIndex = value.IndexOf("[today", endIndex, StringComparison.Ordinal)) != -1)
                {
                    endIndex = value.IndexOf("]", startIndex, StringComparison.Ordinal);
                    var days = value.Substring(startIndex + 6, endIndex - startIndex - 6);
                    value = value.Replace("[today" + days + "]", dtNow.AddDays(Convert.ToInt32(days)).ToString("O"));
                }

                //value = value.Replace("[beginWeek]", dtNow.StartOfWeek(DayOfWeek.Monday).ToString("O"));
                //value = value.Replace("[endWeek]", dtNow.EndOfWeek(DayOfWeek.Sunday).AddDays(1).ToString("O"));
                value = value.Replace("[beginMonth]", new DateTime(dtNow.Year, dtNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O"));
                value = value.Replace("[endMonth]", dtNow.Month == 12
                    ? new DateTime(dtNow.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O")
                    : new DateTime(dtNow.Year, dtNow.Month + 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O"));
                value = value.Replace("[beginPrevMonth]", new DateTime(dtNow.Year, dtNow.Month == 1 ? 12 : dtNow.Month - 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O"));
                value = value.Replace("[beginYear]", new DateTime(dtNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O"));
                value = value.Replace("[endYear]", new DateTime(dtNow.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O"));

                result.Filters = FilterDescriptorFactory.Create(value);
            }

            return result;
        }

        public override async Task ExecuteGridRequest<TModel>(RoboUIGridResult<TModel> roboUIGrid, RoboUIGridRequest request,
            ControllerContext controllerContext)
        {
            var formProvider = roboUIGrid.RoboUIFormProvider ?? RoboUI.DefaultRoboUIFormProvider;

            var response = controllerContext.HttpContext.Response;
            response.ContentType = "application/json";

            var data = await roboUIGrid.FetchAjaxSource(request);
            await WriteJsonData(response, data,
                data.TotalRecords > 0 ? data.TotalRecords : data.Count,
                formProvider,
                roboUIGrid.Columns.Select(x => (RoboUIGridColumn)x).ToList(),
                roboUIGrid.RowActions.Count > 0 && !roboUIGrid.HideActionsColumn
                    ? roboUIGrid.RowActions.Select(x => (IRoboUIGridRowAction)x).ToList()
                    : new List<IRoboUIGridRowAction>(),
                roboUIGrid.GetModelId, roboUIGrid.Editable);
        }

        public override void HandingError(ControllerContext controllerContext, Exception ex)
        {
            var response = controllerContext.HttpContext.Response;
            response.Clear();
            response.ContentType = "application/json";

            var obj = new JObject();

            //if (ex is NotAuthorizedException)
            if (false)
            {
                //var workContext = controllerContext.GetWorkContext();
                //var shellSettings = workContext.Resolve<ShellSettings>();
                //var loginUrl = FormsAuthentication.LoginUrl;
                //if (!string.IsNullOrEmpty(shellSettings.RequestUrlPrefix))
                //{
                //    var indexOf = loginUrl.IndexOf('/');
                //    if (indexOf <= -1) return;
                //    loginUrl = loginUrl.Insert(indexOf + 1, shellSettings.RequestUrlPrefix + "/");
                //}

                //obj["errors"] = "NotAuthorizedException";
                //// ReSharper disable PossibleNullReferenceException
                //obj["redirectUrl"] = loginUrl + "?ReturnUrl=" + HttpUtility.UrlEncode(controllerContext.RequestContext.HttpContext.Request.Url.PathAndQuery);
                //// ReSharper restore PossibleNullReferenceException
            }
            else
            {
                obj["errors"] = ex.GetBaseException().Message;
            }

            response.WriteAsync(obj.ToString());
        }

        public override string GetReloadClientScript(string clientId)
        {
            return string.Format("$(\"#{0}\").data('kendoGrid').dataSource.page(1); return false;", clientId);
        }

        private static async Task WriteJsonData<TModelRecord>(
            HttpResponse response,
            RoboUIGridAjaxData<TModelRecord> data,
            int totalRecords,
            IRoboUIFormProvider formProvider,
            IEnumerable<RoboUIGridColumn> columns,
            IEnumerable<IRoboUIGridRowAction> rowActions,
            Func<TModelRecord, object> getModelId,
            bool isEditable)
        {
            var stringBuilder = new StringBuilder();

            using (var stringWriter = new StringWriter(stringBuilder))
            {
                using (var writer = new JsonTextWriter(stringWriter) { Formatting = Formatting.None })
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("_count");
                    writer.WriteValue(totalRecords);

                    if (data.Callbacks.Count > 0)
                    {
                        writer.WritePropertyName("callback");
                        writer.WriteValue(string.Join("", data.Callbacks));
                    }

                    writer.WritePropertyName("results");

                    writer.WriteStartArray();

                    var needWriteValueDelimiter = false;
                    foreach (var item in data)
                    {
                        var jsonObject = new JObject { { "_id", Convert.ToString(getModelId(item)) } };

                        foreach (var column in columns)
                        {
                            if (isEditable)
                            {
                                //column.PropertyName
                            }
                            else
                            {
                                var value = column.GetValue(item);
                                if (value is DateTime)
                                {
                                    var dt = (DateTime)value;

                                    if (dt == DateTime.MinValue)
                                    {
                                        jsonObject.Add(column.PropertyName, string.Empty);
                                        jsonObject.Add(column.PropertyName + "Formated", string.Empty);
                                    }
                                    else
                                    {
                                        if (dt.Kind == DateTimeKind.Utc)
                                        {
                                            column.SetValue(item, dt.ToLocalTime());
                                        }

                                        jsonObject.Add(column.PropertyName, dt.ToString("O", CultureInfo.InvariantCulture));
                                        if (!string.IsNullOrEmpty(column.DisplayFormat))
                                        {
                                            jsonObject.Add(column.PropertyName + "Formated", dt.ToString(column.DisplayFormat));
                                        }
                                        else
                                        {
                                            jsonObject.Add(column.PropertyName + "Formated", dt.ToString(CultureInfo.CurrentCulture));
                                        }
                                    }
                                }
                                else if (value is DateTimeOffset)
                                {
                                    var dt = (DateTimeOffset)value;

                                    if (dt.DateTime == DateTime.MinValue)
                                    {
                                        jsonObject.Add(column.PropertyName, string.Empty);
                                        jsonObject.Add(column.PropertyName + "Formated", string.Empty);
                                    }
                                    else
                                    {
                                        jsonObject.Add(column.PropertyName, dt.ToString("O", CultureInfo.InvariantCulture));
                                        if (!string.IsNullOrEmpty(column.DisplayFormat))
                                        {
                                            jsonObject.Add(column.PropertyName + "Formated", column.BuildHtml(item));
                                        }
                                        else
                                        {
                                            jsonObject.Add(column.PropertyName + "Formated", dt.LocalDateTime.ToString(CultureInfo.CurrentCulture));
                                        }
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(column.DisplayFormat))
                                    {
                                        jsonObject.Add(column.PropertyName, column.BuildHtml(item));
                                        jsonObject.Add(column.PropertyName + "Formated", string.Empty);
                                    }
                                    else
                                    {
                                        jsonObject.Add(column.PropertyName, column.BuildHtml(item));
                                    }
                                }
                            }
                        }

                        if (rowActions.Any())
                        {
                            var sb = new StringBuilder();
                            sb.Append("<div style=\"white-space: nowrap;\">");
                            sb.Append("<ul class=\"list-inline\" style=\"margin-bottom: 0;\">");

                            foreach (var action in rowActions)
                            {
                                BuildAction(sb, item, action, formProvider, true);
                            }

                            sb.Append("</ul>");
                            sb.Append("</div>");

                            jsonObject.Add("_RowActions", sb.ToString());
                        }
                        else
                        {
                            jsonObject.Add("_RowActions", null);
                        }

                        if (needWriteValueDelimiter)
                        {
                            writer.WriteRaw(",");
                        }

                        writer.WriteRaw(jsonObject.ToString());
                        needWriteValueDelimiter = true;
                    }

                    writer.WriteEndArray();

                    if (data.UserData.Count > 0)
                    {
                        writer.WritePropertyName("userdata");

                        writer.WriteStartObject();

                        foreach (var item in data.UserData)
                        {
                            writer.WritePropertyName(item.Key);
                            writer.WriteValue(item.Value);
                        }

                        writer.WriteEndObject();
                    }

                    writer.WriteEndObject();
                    writer.Flush();
                }
            }

            await response.WriteAsync(stringBuilder.ToString());
        }

        private static void BuildAction<TModelRecord>(StringBuilder sb, TModelRecord item, IRoboUIGridRowAction action, IRoboUIFormProvider formProvider, bool addLiTag)
        {
            if (!action.IsVisible(item))
            {
                return;
            }

            var enabled = action.IsEnabled(item);
            var attributes = new RouteValueDictionary(action.GetAttributes(item));

            if (addLiTag)
            {
                sb.Append("<li>");
            }

            if (action.ChildActions.Count > 0)
            {
                var dropdownId = Guid.NewGuid().ToString();

                sb.AppendFormat("<div id=\"dropdown-content-{0}\" style=\"display: none;\">", dropdownId);

                foreach (var childAction in action.ChildActions)
                {
                    BuildAction(sb, item, childAction, formProvider, false);
                }

                sb.Append("</div>");

                var cssClass =
                (formProvider.GetButtonSizeCssClass(action.ButtonSize) + " " +
                 formProvider.GetButtonStyleCssClass(action.ButtonStyle)).Trim();

                sb.AppendFormat("<button id=\"dropdown-button-{2}\" type=\"button\" class=\"{0}\" data-container=\"body\" data-toggle=\"popover\" data-placement=\"bottom\" data-html=\"true\" data-trigger=\"focus\" data-template=\"<div class='popover popover-dropdown' role='tooltip'><div class='arrow'></div><div class='popover-content'></div></div>\" onclick=\"if(!$('#dropdown-button-{2}').data('bs.popover')){{ $('#dropdown-button-{2}').popover({{ content: function(){{ return $('#dropdown-content-{2}').html(); }} }}).popover('show'); }}\">{1} <span class=\"caret\"></span></button>", cssClass, action.Text, dropdownId);

                return;
            }

            if (action.IsSubmitButton)
            {
                var value = action.GetValue(item);

                var cssClass =
                (formProvider.GetButtonSizeCssClass(action.ButtonSize) + " " +
                 formProvider.GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass).Trim();

                if (!string.IsNullOrEmpty(cssClass))
                {
                    attributes.Add("class", cssClass);
                }

                if (!enabled)
                {
                    attributes.Add("disabled", "disabled");
                }

                if (!string.IsNullOrEmpty(action.ClientClickCode))
                {
                    attributes.Add("onclick", action.ClientClickCode);
                }
                else
                {
                    if (!string.IsNullOrEmpty(action.ConfirmMessage))
                    {
                        attributes.Add("onclick", string.Format("return confirm('{0}');", action.ConfirmMessage));
                    }
                }

                attributes.Add("id", "btn" + Guid.NewGuid().ToString("N").ToLowerInvariant());
                attributes.Add("style", "white-space: nowrap;");

                var tagBuilder = new TagBuilder("button");
                tagBuilder.MergeAttribute("type", "submit");
                tagBuilder.MergeAttribute("name", action.Name);
                tagBuilder.InnerHtml.AppendHtml(action.Text);
                tagBuilder.MergeAttribute("value", Convert.ToString(value));
                tagBuilder.MergeAttributes(attributes, true);
                sb.Append(tagBuilder.ToHtmlString());
            }
            else
            {
                var href = action.GetUrl(item);

                var cssClass =
                (formProvider.GetButtonSizeCssClass(action.ButtonSize) + " " +
                 formProvider.GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass).Trim();

                if (!string.IsNullOrEmpty(cssClass))
                {
                    if (enabled)
                    {
                        attributes.Add("class", cssClass);
                    }
                    else
                    {
                        attributes.Add("class", cssClass + " disabled");
                    }
                }
                else
                {
                    if (!enabled)
                    {
                        attributes.Add("class", "disabled");
                    }
                }

                attributes.Add("style", "white-space: nowrap;");

                if (action.IsShowModalDialog && enabled)
                {
                    attributes.Add("data-toggle", "fancybox");
                    attributes.Add("data-fancybox-type", "iframe");
                    attributes.Add("data-fancybox-width", action.ModalDialogWidth);
                }

                var tagBuilder = new TagBuilder("a");
                if (enabled)
                {
                    tagBuilder.MergeAttribute("href", href ?? "javascript:void(0)");
                }
                else
                {
                    tagBuilder.MergeAttribute("href", "javascript:void(0)");
                }
                tagBuilder.InnerHtml.AppendHtml(action.Text);
                tagBuilder.MergeAttributes(attributes, true);
                sb.Append(tagBuilder.ToHtmlString());
            }

            if (addLiTag)
            {
                sb.Append("</li>");
            }
        }

        private static string GetFieldType(Type propertyType)
        {
            var localType = propertyType;

            LabelBegin:

            if (localType.GetTypeInfo().IsEnum)
            {
                return "string";
            }

            switch (Type.GetTypeCode(localType))
            {
                case TypeCode.Boolean:
                    return "boolean";

                case TypeCode.DateTime:
                    return "date";

                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "number";

                case TypeCode.Object:
                    if (localType.FullName == "System.DateTimeOffset")
                    {
                        return "date";
                    }
                    localType = Nullable.GetUnderlyingType(localType);
                    if (localType == null || localType.FullName == "System.Guid")
                    {
                        return "string";
                    }
                    goto LabelBegin;
                default:
                    return "string";
            }
        }

        private static class GridDescriptorSerializer
        {
            public static IList<T> Deserialize<T>(string from) where T : IDescriptor, new()
            {
                var list = new List<T>();
                if (string.IsNullOrEmpty(from))
                    return list;

                foreach (var source in from.Split("~".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    var obj = new T();
                    obj.Deserialize(source);
                    list.Add(obj);
                }
                return list;
            }
        }
    }
}
