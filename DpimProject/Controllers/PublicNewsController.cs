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
    public class PublicNewsController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.News.PublicNews news;
        private DataTools dtl;
        private Models.Student student;

        public PublicNewsController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            news = new Models.News.PublicNews();


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

        [ActionName("GetAllNews")]
        [HttpGet]
        public dynamic GetAllNews()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            var data = string.IsNullOrEmpty(error) ? news.get_all_news(ref error) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("InsertNews")]
        [HttpPost]
        public dynamic InsertNews()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            model_public_new news_model = new model_public_new();
            model_public_new body = Dtl.json_to_object(Dtl.json_request(), news_model);
            
            var data = string.IsNullOrEmpty(error) ? news.insert_news(body, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("UpdateNews")]
        [HttpPost]
        public dynamic UpdateNews()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            model_public_new news_model = new model_public_new();
            model_public_new body = Dtl.json_to_object(Dtl.json_request(), news_model);

            var data = string.IsNullOrEmpty(error) ? news.update_news(body, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("DeleteNews")]
        [HttpDelete]
        public dynamic DeleteNews()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            string id = HttpContext.Current.Request.QueryString["id"];

            var data = string.IsNullOrEmpty(error) ? news.delete_news(id, ref error, auth) : null;
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