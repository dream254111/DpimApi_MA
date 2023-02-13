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
    public class VideoController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Video.PublicVideo video;
        private DataTools dtl;

        public VideoController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            video = new Models.Video.PublicVideo();


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

        //fix
        [ActionName("GetAllVideo")]
        [HttpGet]
        public dynamic GetAllVideo()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            var data = string.IsNullOrEmpty(error) ? video.get_all_video(ref error) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //fix
        [ActionName("InsertVideo")]
        [HttpPost]
        public dynamic InsertVideo()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            model_video_on_demand video_model = new model_video_on_demand();
            model_video_on_demand body = Dtl.json_to_object(Dtl.json_request(), video_model);

            var data = string.IsNullOrEmpty(error) ? video.insert_video(body, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        
        //fix
        [ActionName("UpdateVideo")]
        [HttpPost]
        public dynamic UpdateVideo()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            var m = new model_video_on_demand();
            m = Dtl.json_to_object(Dtl.json_request(), m);

            var data = string.IsNullOrEmpty(error) ? video.update_video(m, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        
        [ActionName("DeleteVideo")]
        [HttpDelete]
        public dynamic DeleteVideo()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            string id = HttpContext.Current.Request.QueryString["id"];

            var data = string.IsNullOrEmpty(error) ? video.delete_video(id, ref error, auth) : null;
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