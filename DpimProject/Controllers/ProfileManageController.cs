using DpimProject.Models.Data.DataModels;
using DpimProject.Models.DataTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProfileManageController : ApiController
    {
        
        private Models.Authentication.Authentication auth;
        private Models.ProfileManagement.ProfileManagement profile;
        private DataTools dtl;
        private Models.Student student;

        public ProfileManageController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            profile = new Models.ProfileManagement.ProfileManagement();


            //string token = HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim() ?? HttpContext.Current.Request.Headers.Get("Authorization");
            //if (!string.IsNullOrEmpty(token))
            //{
            //    auth.GetAuthentication(token);
            //}
        }

        private void GetToken(ref string errorMsg)
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

        [ActionName("GetProfileManage")]
        [HttpGet]
        public dynamic GetProfileManage()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            var data = string.IsNullOrEmpty(error) ? profile.get_all_profile_manage(ref error) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("UpdateProfileManage")]
        [HttpPost]
        public dynamic UpdateProfileManage()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            manage_profile profile_model = new manage_profile();
            manage_profile body = Dtl.json_to_object(Dtl.json_request(), profile_model);

            var data = string.IsNullOrEmpty(error) ? profile.update_profile_maange(body, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }


    }
}