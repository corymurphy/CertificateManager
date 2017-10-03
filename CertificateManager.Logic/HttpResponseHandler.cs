using Microsoft.AspNetCore.Mvc;

namespace CertificateManager.Logic
{
    public class HttpResponseHandler
    {
        Controller controller;
        public HttpResponseHandler(Controller controller)
        {
            this.controller = controller;
        }
        public JsonResult RespondConflict(string msg)
        {
            controller.Response.StatusCode = 409;
            return controller.Json(new { message = msg, status = "error" });
        }

        public JsonResult RespondNotFound(string msg)
        {
            controller.Response.StatusCode = 404;
            return controller.Json(new { message = msg, status = "error" });
        }

        public JsonResult RespondBadRequest(string msg)
        {
            controller.Response.StatusCode = 400;
            return controller.Json(new { message = msg });
        }

        public JsonResult RespondPreconditionFailed(string msg)
        {
            controller.Response.StatusCode = 412;
            return controller.Json(new { message = msg });
        }

        public JsonResult RespondSuccess()
        {
            controller.Response.StatusCode = 200;
            return controller.Json(new { status = "success" });
        }

        public JsonResult RespondSuccess(object data)
        {
            controller.Response.StatusCode = 200;
            return controller.Json(new { payload = data, status = "success", message = string.Empty });
        }

        public JsonResult RespondServerError(string msg)
        {
            controller.Response.StatusCode = 500;
            return controller.Json(new { status = "error", message = msg });
        }

        public JsonResult RespondServerError()
        {
            controller.Response.StatusCode = 500;
            return controller.Json(new { status = "error", message = "Error occured while performing the requested operation." });
        }
    }
}
