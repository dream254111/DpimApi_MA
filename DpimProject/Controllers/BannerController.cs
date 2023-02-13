using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using DpimProject.Models.DataTools;
using DpimProject.Models.Data;
using Newtonsoft.Json;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using DpimProject.Models.Data.DataModels;
using System.Diagnostics;
using System.Net.Http.Formatting;

namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BannerController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Banner.Banner banner;
        private DataTools dtl;
        private Models.Student student;

        public BannerController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            banner = new Models.Banner.Banner();

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

        /* Old GetToken
        private void GetToken(ref string errorMsg)
        {
            string token = "";
            try
            {
                var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
                if (cookie != null)
                {
                    token = cookie.ToString();
                }
                else
                {
                    throw new Exception("ไม่พบข้อมูล Token");
                }
                //auth.IsAdmin = true;

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            auth.GetAuthentication(token);


        }
        */

        /* Test succeed (2020/11/23) */
        /*
        public ActionResult TestResult()
        {
            return Content("test555", "application/json");
        }
        */

        [ActionName("GetAllBanner")]
        [HttpGet]
        public dynamic GetAllBanner()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            var data = string.IsNullOrEmpty(error) ? banner.get_all_banner(ref error) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("InsertBanner")]
        [HttpPost]
        public dynamic InsertBanner()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            model_banner banner_model = new model_banner();
            model_banner body = Dtl.json_to_object(Dtl.json_request(), banner_model);

            var data = string.IsNullOrEmpty(error) ? banner.insert_banner(body, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("UpdateBanner")]
        [HttpPost]
        public dynamic UpdateBanner()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            model_banner banner_model = new model_banner();
            model_banner body = Dtl.json_to_object(Dtl.json_request(), banner_model);

            var data = string.IsNullOrEmpty(error) ? banner.update_banner(body, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("DeleteBanner")]
        [HttpDelete]
        public dynamic DeleteBanner()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            string id = HttpContext.Current.Request.QueryString["id"];

            var data = string.IsNullOrEmpty(error) ? banner.delete_banner(id, ref error, auth) : null;
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
