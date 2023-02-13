using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ExportController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Export.Export export;
        private Models.Student student;

        public ExportController()
        {
            auth = new Models.Authentication.Authentication();
            export = new Models.Export.Export();
            //student = new Models.Student();
            //student.checkCourseEnddate();

            //string token = HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim() ?? HttpContext.Current.Request.Headers.Get("Authorization");
            //if (!string.IsNullOrEmpty(token))
            //{
            //    auth.GetAuthentication(token);
            //}
        }

        private void GetToken()
        {
            string token = "";
            try
            {
                var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
                if (cookie != null)
                {
                    token = cookie?.ToString();
                    auth.GetAuthentication(token);

                }
                else
                {
                    throw new Exception("Token Not Found");
                }
            }
            catch (Exception ex)
            {

                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();                // Get the top stack frame
                string stackIndent = ex.StackTrace;
                var error = new
                {
                    success = string.IsNullOrEmpty(ex.Message),
                    ex.Message,
                    FileName = frame.GetFileName(),
                    line = frame.GetFileLineNumber(),
                    Method = frame.GetMethod()
                };
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new ObjectContent<object>(error, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };


                throw new HttpResponseException(resp);

            }




        }
        [ActionName("export_report")]
        [HttpGet]
        public dynamic export_report(int page = 1, bool isExcel = true, string course_id = "", string course_category_id = "", int account_type = 0, string search = "")
        {
            //GetToken();
            string error = "";
            var data = export.export_report(page, isExcel, course_id, course_category_id, account_type, search, ref error);
            //var output = new
            //{
            //    success = string.IsNullOrEmpty(error),
            //    error,
            //    data
            //};

            return data;
        }
    }
}
