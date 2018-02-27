using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RoboUI
{
    public interface IRoboUIGridProvider
    {
        string Render<TModel>(RoboUIGridResult<TModel> roboUIGrid, IHtmlHelper htmlHelper) where TModel : class;

        //void GetAdditionalResources(ScriptRegister scriptRegister, StyleRegister styleRegister);

        RoboUIGridRequest CreateGridRequest(ControllerContext controllerContext);

        Task ExecuteGridRequest<TModel>(RoboUIGridResult<TModel> roboUIGrid, RoboUIGridRequest request, ControllerContext controllerContext) where TModel : class;

        void HandingError(ControllerContext controllerContext, Exception ex);

        string GetReloadClientScript(string clientId);
    }
}
