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
using DpimProject.Models.Data.DataModels;
using System.Diagnostics;
namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ManagementController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Management.Management manage;
        private DataTools dtl;
        private Models.Student student;

        public ManagementController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            manage = new Models.Management.Management();
            //string token = System.Web.HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim();
            ////student = new Models.Student();
            ////student.checkCourseEnddate();
            //if (!auth.CheckAuthentication(token))
            //{
            //    new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Access Denied"),
            //        StatusCode = HttpStatusCode.Unauthorized
            //    };
            //    }
            //auth.GetAuthentication(token);
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
        
        #region Tutorial
        [ActionName("TutorialReadList")]
        [HttpGet]
        public dynamic TutorialReadList()
        {
            auth.IsAdmin = true;


            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? manage.TutorialReadList(ref error):null;

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }

        [ActionName("TutorialManage")]
        [HttpPost]
        public dynamic TutorialManage()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            dynamic output = null;
            var model = new List<model_tutorial>();
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = (string.IsNullOrEmpty(error)) ? manage.TutorialManage(get_body, auth, ref error):null;

            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;

        }

        [ActionName("TutorialDelete")]
        [HttpGet]
        public dynamic TutorialDelete(int tutorial_id)
        {
            auth.IsAdmin = true;


            string error = "";
            GetToken(ref error);

            manage.TutorialDelete(tutorial_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error
            };
            return output;

        }
        #endregion

        [ActionName("SendingEmail")]
        [HttpGet]
        public dynamic SendingEmail(int? student_id=0,string course_id="",string email_type="",int? problem_id=0)
        {
            string token = "";
            string error = "";
            GetToken(ref error);

            //var m = new
            //{
            //    student = new Models.Data.DataModels.student { id = 1 },
            //    course = new Models.Data.DataModels.course { id = 1009 },
            //    student_course_info = new Models.Data.DataModels.student_course_info(),
            //    certificate_info = new Models.Data.DataModels.certificate_info { certificate_id = "1234/2563" },
            //    email_type="04"
            //};
            //m = Dtl.json_to_object(Dtl.json_request(), m);
            manage.SendingEmail(student_id, course_id, email_type, token, auth, problem_id, ref error);
            //manage.testSendEmail();
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error
            };
            return output;
        } [ActionName("TestSendingEmail")]
        [HttpGet]
        public dynamic TestSendingEmail()
        {
            //GetToken();
            string token = "";
            string error = "";

            manage.testSendEmail(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error
            };
            return output;
        }

        #region FAQ
        [ActionName("ReadListFAQ")]
        [HttpGet]
        public dynamic ReadListFAQ()
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? manage.ReadListFAQ(ref error):null;

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("FAQManagement")]
        [HttpPost]
        public dynamic FAQManagement()
        {

            string error = "";
            GetToken(ref error);

            dynamic output = null;
            //if (auth.isAuthenticated)
            //{

            //}
            var model = new faq();
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = (string.IsNullOrEmpty(error)) ? manage.FAQManagement(get_body, auth, ref error):null;

            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        } 

        [ActionName("DeleteFAQ")]
        [HttpPut]
        public dynamic DeleteFAQ()
        {
            var model = new
            {
                id = 0
            };
            var error = "";
            GetToken(ref error);
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = (string.IsNullOrEmpty(error)) ? manage.DeleteFAQ(get_body.id, ref error, auth):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("GetAllFAQType")]
        [HttpGet]
        public dynamic GetAllFAQType()
        {
            var error = "";
            GetToken(ref error);
            var data = manage.GetAllFAQType(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region ReportProblem
        [ActionName("ReportProblemReadList")]
        [HttpGet]
        public dynamic ReportProblemReadList(int pageIndex, int pageSize, bool? is_done = null, int problem_type = 0, string orderBy = "", bool desc = true)
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? manage.ReadListReportProblem(problem_type, pageIndex, pageSize, is_done, orderBy, desc, ref error) : null ;

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("DoneProblem")]
        [HttpPut]
        public dynamic DoneProblem()
        {

            string error = "";
            GetToken(ref error);

            var model = new
            {
                id = 0,
                response = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = (string.IsNullOrEmpty(error)) ? manage.DoneReportProblem(get_body.id, get_body.response, auth, ref error):null;

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }

        [ActionName("ReportProblemManage")]
        [HttpPost]
        public dynamic ReportProblemManage()
        {

            string error = "";
            GetToken(ref error);

            dynamic output = null;

            var m = new report_problem();
            m = Dtl.json_to_object(Dtl.json_request(), m);
            var data = (string.IsNullOrEmpty(error)) ? manage.ReportProblemManage(m, auth, ref error):null;
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }
        #endregion

        #region Special day
        [ActionName("special_days")]
        [HttpGet]
        public dynamic special_days()
        {
            var error = "";
            GetToken(ref error);
            var data = manage.get_special_days(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("manage_special_days")]
        [HttpPost]
        public dynamic manage_special_days()
        {
            var error = "";
            GetToken(ref error);
            var model = new
            {
                cover = "",
                start_date = (DateTime?)null,
                end_date = (DateTime?)null
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            
            var data = manage.manage_special_days(get_body.cover, get_body.start_date, get_body.end_date, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        [ActionName("VisitUpdate")]
        [HttpPut]
        public dynamic VisitUpdate()
        {
            string error = "";

            var data =  manage.VisitUpdate(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }
        [ActionName("GetVideo")]
        [HttpGet]
        public HttpResponseMessage GetVideo()
        {
            //GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            var data = manage.getVideo();
            var output = new
            {
                data
            };
            resp.Content = new ObjectContent<object>(output, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("LoadVideo")]
        [HttpGet]
        public HttpResponseMessage LoadVideo(string filename)
        {
            string error = "";
            GetToken(ref error);

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            var data = (string.IsNullOrEmpty(error)) ? manage.Loadvide(filename):null;
            var output = new
            {
                data
            };
            resp.Content = new ObjectContent<object>(output, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("CheckCourseFile")]
        [HttpGet]
        public HttpResponseMessage CheckCourseFile(int? course_id)
        {
            string error = "";
            GetToken(ref error);

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
           manage.CheckCourseFile(course_id, ref error);
            var output = new
            {
                success=string.IsNullOrEmpty(error),
                error
            };
            resp.Content = new ObjectContent<object>(output, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
            return resp;
        }
        //[ActionName("LicenseCertificate")]
        //[HttpPost]
        //public HttpResponseMessage LicenseCertificate()
        //{
        //    string error = "";
        //    GetToken(ref error);
        //    var m = new certificate();
        //     m = Dtl.json_to_object(Dtl.json_request(), m);
        //    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
        //   manage.LicenseCertificate(m,auth,ref error);
        //    var output = new
        //    {
        //        success=string.IsNullOrEmpty(error),
        //        error
        //    };
        //    resp.Content = new ObjectContent<object>(output, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
        //    return resp;
        //}
    }
}
