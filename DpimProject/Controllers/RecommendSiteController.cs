using DpimProject.Models.Data.DataModels;
using DpimProject.Models.DataTools;
using DpimProject.Models.RecommendSite;
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
    public class RecommendSiteController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.RecommendSite.RecommendSite site;
        private DataTools dtl;
        private Models.Student student;

        public RecommendSiteController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            site = new Models.RecommendSite.RecommendSite();


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

        [ActionName("GetAllSite")]
        [HttpGet]
        public dynamic GetAllSite()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            var data = string.IsNullOrEmpty(error) ? site.get_all_site(ref error) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        /* Insert funtion is not necessary for recommend site (2020/12/06 - Dream) */
        //[ActionName("InsertSite")]
        //[HttpPost]
        //public dynamic InsertSite()
        //{
        //    recommend_site site_model = new recommend_site();
        //    recommend_site body = Dtl.json_to_object(Dtl.json_request(), site_model);

        //    var error = "";
        //    var data = site.insert_site(body, ref error, auth);
        //    var output = new
        //    {
        //        success = string.IsNullOrEmpty(error),
        //        error,
        //        data
        //    };

        //    return output;
        //}

        [ActionName("UpdateSite")]
        [HttpPost]
        public dynamic UpdateSite()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);


            dynamic model = new
            {
                site_model = new List<recommend_site_single>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            
            var data = string.IsNullOrEmpty(error) ? site.update_site(get_body.site_model, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        /* Delete funtion is not necessary for recommend site (2020/12/06 - Dream) */
        //[ActionName("DeleteSite")]
        //[HttpDelete]
        //public dynamic DeleteSite()
        //{
        //    string id = HttpContext.Current.Request.QueryString["id"];

        //    var error = "";
        //    var data = site.delete_site(id, ref error, auth);
        //    var output = new
        //    {
        //        success = string.IsNullOrEmpty(error),
        //        error,
        //        data
        //    };

        //    return output;
        //}
    }
}