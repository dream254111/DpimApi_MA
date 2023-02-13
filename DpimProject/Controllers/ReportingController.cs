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

    public class ReportingController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Report.ReportStudent reportStudent;
        private DataTools dtl;
        private Models.Student student;

        public ReportingController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            reportStudent = new Models.Report.ReportStudent();
            //student = new Models.Student();
            //student.checkCourseEnddate();
            //string token = System.Web.HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim();

            //if (!auth.CheckAuthentication(token))
            //{
            //    RedirectToAction("Index", "Home");
            //}
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
        [ActionName("overview_website")]
        [HttpGet]
        public dynamic overview_website()
        {
            //GetToken();

            string error = "";
            var data = reportStudent.get_overview_website(ref error);
            if (data == null)
            {
                error = "Data Is Empty";
            }

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("report_overview_student")]
        [HttpGet]
        public dynamic report_overview_student(string course_id, int account_type, string search, int pageIndex, int pageSize, DateTime? start_date = null, DateTime? end_date = null, string orderBy = "", bool desc = true)
        {
        

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error))? reportStudent.get_overview_student(
                course_id,
                account_type,
                search,
                pageIndex,
                pageSize,
                start_date,
                end_date,
                orderBy,
                desc,
                ref error
                ):null;
         

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("change_account_type_student")]
        [HttpPut]
        public dynamic change_account_type_student()
        {

            string error = "";
            GetToken(ref error);

            var model = new
            {
                student_id = 0,
                account_type = 0
                //is_internal_student = false
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.save_account_type_student(get_body.student_id, get_body.account_type, auth, ref error):null;

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }


        [ActionName("report_desc_student")]
        [HttpGet]
        public dynamic report_desc_student(int filter, string course_id)
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.get_desc_student(filter, course_id, ref error):null;
            if (data == null)
            {
                error = "Data Is Empty";
            }

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("report_student_by_id")]
        [HttpGet]
        public dynamic report_student_by_id(int student_id)
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.get_student_by_id(student_id, ref error):null;

            if (data == null)
            {
                error = "Data Is Empty";
            }

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }


        //รายงานรายชื่อนักเรียน
        [ActionName("StudentOfCourse")]
        [HttpGet]
        public dynamic StudentOfCourse(string search_text, int skip)
        {

            int take = 16;
            skip = (skip > 0) ? skip : 0;
            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.ReportStudentCourse(search_text, take, skip, ref error):null;
            if (data == null)
            {
                error = "Data Is Empty";
            }
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }

        //Report
        //แบบประเมิน 
        [ActionName("EvaluationReport")]    
        [HttpGet]
        public dynamic EvaluationReport(string course_id)
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.EvaluationReport(course_id, ref error):null;

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        //Report
        //นักเรียน
        [ActionName("StudentReport")]
        [HttpGet]
        public dynamic StudentReport(string search_text, string course_id, int skip)
        {

            int take = 16; skip = (skip > 0) ? skip : 0;

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.StudentReport(search_text, course_id, take, skip, ref error):null;
            if (data == null)
            {
                error = "Data Is Empty";
            }
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        //Report
        //การทำข้อสอบ
        [ActionName("StudentExamPressReport")]
        [HttpGet]
        public dynamic StudentExamPressReport(string id, DateTime? start_date = null, DateTime? end_date = null)
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.StudentExamPressReport(id, start_date, end_date, ref error):null;
            if (data == null)
            {
                error = "Data Is Empty";
            }
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        //Report
        //พืมพ์ Certificate Category
        [ActionName("CertificatePrintReportByCategory")]
        [HttpGet]
        public dynamic CertificatePrintReportByCategory(string category_id, DateTime? start_date = null, DateTime? end_date = null)
        {
          

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.get_certificate_print_report_by_category(category_id, start_date, end_date, ref error):null;

            if (data == null)
            {
                error = "Data Is Empty";
            }

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        
        //Report
        //พืมพ์ Certificate Course
        [ActionName("CertificatePrintReportByCourse")]
        [HttpGet]
        public dynamic CertificatePrintReportByCourse(string course_id, DateTime? start_date = null, DateTime? end_date = null)
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.get_certificate_print_report_by_course(course_id, start_date, end_date, ref error):null;

            if (data == null)
            {
                error = "Data Is Empty";
            }

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //Report
        //พื้นใน Harddisk 
        [ActionName("DriveInfoReport")]
        [HttpGet]
        public dynamic DriveInfoReport()
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.DriveSpecReport(ref error):null;
            //if (data == null)
            //{
            //    error = "Data Is Empty";
            //}
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        //Report
        //Video On demain View
        [ActionName("ViewCountVideoReport")]
        [HttpGet]
        public dynamic ViewCountVideoReport(string id)
        {

            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.ViewCountVideoReport(id, ref error):null;
            if (data == null)
            {
                error = "Data Is Empty";
            }
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }

        [ActionName("ProblemReport")]
        [HttpGet]
        public dynamic ProblemReport(string search_text, int skip)
        {

            string error = "";
            GetToken(ref error);

            int take = 16; skip = (skip > 0) ? skip : 0;

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.ProblemReport(search_text, take, skip, ref error):null;
            if (data == null)
            {
                error = "Data Is Empty";
            }
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;

        }

        /* การพิมพ์หน้าคอร์สเรียน */
        [ActionName("CoursePrint")]
        [HttpGet]
        public dynamic CoursePrint(string id)
        {

            var error = string.Empty;
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.CoursePrint(id, ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        /* การรัยชมคอร์สเรียน (course) */
        [ActionName("CourseView")]
        [HttpGet]
        public dynamic CourseView(string id)
        {

            var error = string.Empty;
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.CourseView(id, ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        /* การรัยชมคอร์สเรียน (lesson) */
        [ActionName("LessonView")]
        [HttpGet]
        public dynamic LessonView(string id)
        {

            var error = string.Empty;
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.LessonView(id, ref error):null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            return output;
        }

        [ActionName("evaluation_raw")]
        [HttpGet]
        public dynamic evaluation_raw(string course_id)
        {
            string error = "";
            GetToken(ref error);

            var data = (string.IsNullOrEmpty(error)) ? reportStudent.evaluation_raw(course_id, ref error) : null;

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
