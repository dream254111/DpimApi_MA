using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
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
using System.Diagnostics;

namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class InstructorController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Instructor.Instructor instructor;
        private DataTools dtl;
        private Models.Student student;

        public InstructorController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            instructor = new Models.Instructor.Instructor();
            //student = new Models.Student();
            //student.checkCourseEnddate();

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
        #region Instuctor
        [ActionName("InstuctorReadList")]
        [HttpGet]
        public dynamic InstuctorReadList()
        {
            auth.IsAdmin = true;

            
            string error = "";
            GetToken(ref error );

            var data =(string.IsNullOrEmpty(error))? instructor.InstructorReadlist(ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("InstuctorDelete")]
        [HttpPut]
        public dynamic InstuctorDelete()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);
            var model = new
            {
                id = 0
            };
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = (string.IsNullOrEmpty(error)) ? instructor.InstuctorDelete(get_body.id, auth, ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("InstructorRead")]
        [HttpGet]
        public dynamic InstructorRead(int? instructor_id)
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);
            var data = (string.IsNullOrEmpty(error)) ? instructor.InstructorRead(instructor_id, ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }

        [ActionName("InstructorManage")]
        [HttpPost]
        public dynamic InstructorManage()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);
            dynamic output = null;
            //if (auth.isAuthenticated)
            //{
            //if (auth.isAuthenticated)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            //Form Data
            var f = new Models.Data.DataModels.user();

            ////List<Models.Data.DataModels.user_permission> n = new List<Models.Data.DataModels.user_permission>();

            //var m = new
            //{
            //    instructor = new Models.Data.DataModels.instructor()

            //};
            
            var m = new Models.Data.DataModels.instructor();
            m = Dtl.json_to_object(Dtl.json_request(), m);
            var data = (string.IsNullOrEmpty(error)) ? instructor.InstructorManage(m, auth, ref error):null;
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            //}
            return output;

        }
        #endregion

        #region Course Category
        [ActionName("CategoryReadList")]
        [HttpGet]
        public dynamic CategoryReadList()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);
            var data = (string.IsNullOrEmpty(error)) ? instructor.CategoryReadList(ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }
        [ActionName("CategoryRead")]
        [HttpGet]
        public dynamic CategoryRead(string category_id)
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);
            var data = (string.IsNullOrEmpty(error)) ? instructor.CategoryRead(category_id, ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }
        [ActionName("CategoryManage")]
        [HttpPost]
        public dynamic CategoryManage()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            dynamic output = null;
            //if (auth.isAuthenticated)
            //{
            //if (auth.isAuthenticated)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            //Form Data

            ////List<Models.Data.DataModels.user_permission> n = new List<Models.Data.DataModels.user_permission>();
            
            var m = new Models.Data.DataModels.course_category();
            m = Dtl.json_to_object(Dtl.json_request(), m);
            var data = (string.IsNullOrEmpty(error)) ? instructor.CategoryManage(m, auth, ref error):null;
             output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            //}
            return output;

        }
        #endregion
    }
}
