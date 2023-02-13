using DpimProject.Models.Data.DataModels;
using DpimProject.Models.DataTools;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Cors;
using System.Web;
using static DpimProject.Models.Course.Course;
using System.Diagnostics;
using DpimProject.Models.Management;

namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class CourseController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Course.Course course;
        private Models.Management.Management manage;
        private Models.Student student;

        public CourseController()
        {
            auth = new Models.Authentication.Authentication();
            course = new Models.Course.Course();
            manage = new Models.Management.Management();
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

        public dynamic check_token()
        {
            string token = "";  
            var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
            if(cookie == null)
            {
                return false;
            }
            else
            {
                token = cookie?.ToString();
                auth.GetAuthentication(token);
                return true;
            }
        }

        #region Admin
        [ActionName("all_course")]
        [HttpGet]
        public dynamic all_course()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_all_course(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return  output;
        }

        [ActionName("new_batch_course")]
        [HttpPost]
        public dynamic new_batch_course()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var model = new
            {
                course_id = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = course.new_batch_course(get_body.course_id, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        #region Course Overview
        [ActionName("remove_course")]
        [HttpPut]
        public dynamic remove_course()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var model = new
            {
                id = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = course.remove_course(get_body.id, auth, ref error);
            if (data == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("overview_course")]
        [HttpGet]
        public dynamic overview_course(string id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_overview_course(id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("add_course")]
        [HttpPost]
        public dynamic add_course()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            #region Model
            var model = new model_course();
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion
            
            var data = course.save_add_course(get_body, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("edit_course")]
        [HttpPut]
        public dynamic edit_course()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            #region Model
            var model = new model_course();
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion
            
            var data = course.save_edit_course(get_body, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course Lesson
        //success
        [ActionName("all_lesson")]
        [HttpGet]
        public dynamic all_lesson(string course_id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_all_lesson(course_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("lesson_by_id")]
        [HttpGet]
        public dynamic lesson_by_id(int id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_lesson_by_id(id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("remove_course_lesson")]
        [HttpPut]
        public dynamic remove_course_lesson()
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

            var data = course.remove_course_lesson(get_body.id, auth, ref error);
            if (data == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("add_course_lesson")]
        [HttpPost]
        public dynamic add_course_lesson()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            #region Model
            var model = new model_course_lesson();
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_add_course_lesson(get_body, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("edit_course_lesson")]
        [HttpPut]
        public dynamic edit_course_lesson()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            #region Model
            var model = new model_course_lesson();
            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_edit_course_lesson(get_body, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course Lesson Interactive
        [ActionName("all_lesson_interactive")]
        [HttpGet]
        public dynamic all_lesson_interactive(int lesson_id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_all_lesson_interactive(lesson_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("remove_lesson_interactive")]
        [HttpPut]
        public dynamic remove_lesson_interactive()
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

            var data = course.remove_lesson_interactive(get_body.id, auth, ref error);
            if (data == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("add_lesson_interactive")]
        [HttpPost]
        public dynamic add_lesson_interactive()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);
            
            #region Model
            dynamic model = new
            {
                interactive = new model_interactive_question(),
                answer = new List<model_interactive_answer>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.interactive == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            }
            #endregion

            var data = course.save_add_lesson_interactive(get_body.interactive, get_body.answer, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("edit_lesson_interactive")]
        [HttpPut]
        public dynamic edit_lesson_interactive()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);
            
            #region Model
            dynamic model = new
            {
                interactive = new model_interactive_question(),
                answer = new List<model_interactive_answer>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.interactive == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            }
            #endregion

            var data = course.save_edit_lesson_interactive(get_body.interactive, get_body.answer, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course Lesson Exercise
        //success
        [ActionName("all_lesson_exercise")]
        [HttpGet]
        public dynamic all_lesson_exercise(int lesson_id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_all_lesson_execrise(lesson_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("remove_lesson_exercise")]
        [HttpPut]
        public dynamic remove_lesson_exercise()
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

            var data = course.remove_lesson_execrise(get_body.id, auth, ref error);
            if (data == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("add_lesson_exercise")]
        [HttpPost]
        public dynamic add_lesson_exercise()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            dynamic model = null;
            bool is_choices = true;
            List<course_lesson_exercise_answer_choices> answer_choices = new List<course_lesson_exercise_answer_choices>();
            List<course_lesson_exercise_answer_match> answer_match = new List<course_lesson_exercise_answer_match>();
            
            #region Model
            // get choices
            model = new
            {
                lesson_exercise = new model_lesson_exercise(),
                answer_choices = new List<course_lesson_exercise_answer_choices>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if(get_body == null || get_body.answer_choices == null)
            {
                //get match
                model = new
                {
                    lesson_exercise = new model_lesson_exercise(),
                    answer_match = new List<course_lesson_exercise_answer_match>()
                };

                get_body = Dtl.json_to_object(Dtl.json_request(), model);

                if (get_body == null || get_body.lesson_exercise == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);
                
                is_choices = false;
                answer_match = get_body.answer_match;
            }
            else
            {
                answer_choices = get_body.answer_choices;
            }
            #endregion

            var data = course.save_add_lesson_exercise(
                get_body.lesson_exercise,
                answer_choices,
                answer_match,
                is_choices,
                auth, 
                ref error
                );

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("edit_lesson_exercise")]
        [HttpPut]
        public dynamic edit_lesson_exercise()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            dynamic model = null;
            bool is_choices = true;
            List<course_lesson_exercise_answer_choices> answer_choices = new List<course_lesson_exercise_answer_choices>();
            List<course_lesson_exercise_answer_match> answer_match = new List<course_lesson_exercise_answer_match>();
            #region Model
            // get choices
            model = new
            {
                lesson_exercise = new model_lesson_exercise(),
                answer_choices = new List<course_lesson_exercise_answer_choices>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.answer_choices == null)
            {
                //get match
                model = new
                {
                    lesson_exercise = new model_lesson_exercise(),
                    answer_match = new List<course_lesson_exercise_answer_match>()
                };

                get_body = Dtl.json_to_object(Dtl.json_request(), model);

                if (get_body == null || get_body.lesson_exercise == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);

                is_choices = false;
                answer_match = get_body.answer_match;
            }
            else
            {
                answer_choices = get_body.answer_choices;
            }
            #endregion

            var data = course.save_edit_lesson_exercise(
                get_body.lesson_exercise,
                answer_choices,
                answer_match,
                is_choices,
                auth,
                ref error
                );

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course Exam
        //success
        [ActionName("all_exam")]
        [HttpGet]
        public dynamic all_exam(string course_id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_all_exam(course_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("course_exam_by_id")]
        [HttpGet]
        public dynamic course_exam_by_id(int id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_course_exam_by_id(id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("remove_course_exam")]
        [HttpPut]
        public dynamic remove_course_exam()
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

            var data = course.remove_course_exam(get_body.id, auth, ref error);
            if (data == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("add_course_exam")]
        [HttpPost]
        public dynamic add_course_exam()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            dynamic model = null;
            #region Model
            model = new
            {
                exam = new model_exam(),
                answer = new List<course_exam_answer>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.exam == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_add_course_exam(
                get_body.exam,
                get_body.answer,
                auth,
                ref error
                );

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success
        [ActionName("edit_course_exam")]
        [HttpPut]
        public dynamic edit_course_exam()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            dynamic model = null;
            #region Model
            model = new
            {
                exam = new model_exam(),
                answer = new List<course_exam_answer>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.exam == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_edit_course_exam(
                get_body.exam,
                get_body.answer,
                auth,
                ref error
                );

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course Evaluation
        [ActionName("all_evaluation")]
        [HttpGet]
        public dynamic all_evaluation(string course_id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken( ref error);

            var data = course.get_all_evaluation(course_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("evaluation_by_id")]
        [HttpGet]
        public dynamic evaluation_by_id(int id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_evaluation_by_id(id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("remove_evaluation")]
        [HttpPut]
        public dynamic remove_evaluation()
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

            var data = course.remove_evaluation(get_body.id, auth, ref error);
            if (data == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("add_evaluation")]
        [HttpPost]
        public dynamic add_evaluation()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            dynamic model = null;
            #region Model
            model = new
            {
                evaluation = new course_evaluation(),
                choices = new List<course_evaluation_choices>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.evaluation == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_add_evaluation(
                get_body.evaluation,
                get_body.choices,
                auth,
                ref error
                );

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("edit_evaluation")]
        [HttpPut]
        public dynamic edit_evaluation()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            dynamic model = null;
            #region Model
            model = new
            {
                evaluation = new course_evaluation(),
                choices = new List<course_evaluation_choices>()
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.evaluation == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_edit_evaluation(
                get_body.evaluation,
                get_body.choices,
                auth,
                ref error
                );

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course Voucher
        [ActionName("all_course_voucher")]
        [HttpGet]
        public dynamic all_course_voucher(string courseId, int pageIndex, int pageSize, int? status = null, string orderBy = "", bool? desc = true, DateTime? startDate = null, DateTime? endDate = null)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_all_course_voucher(
                courseId,
                pageIndex,
                pageSize,
                status,
                orderBy,
                desc,
                startDate,
                endDate,
                ref error
                );
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return data;
        }

        [ActionName("add_voucher")]
        [HttpPost]
        public dynamic add_voucher()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            #region Model
            var model = new model_course_voucher();

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_add_voucher(get_body, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("edit_voucher")]
        [HttpPut]
        public dynamic edit_voucher()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            #region Model
            var model = new model_course_voucher();

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_edit_voucher(get_body, auth, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("remove_voucher")]
        [HttpPut]
        public dynamic remove_voucher()
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

            var data = course.remove_voucher(get_body.id, auth, ref error);
            if (data == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course Student
        [ActionName("all_course_student")]
        [HttpGet]
        public dynamic all_course_student(string courseId, int pageIndex, int pageSize, string search, string orderBy="", bool desc=true)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_all_course_student(
                courseId,
                pageIndex,
                pageSize,
                search,
                orderBy,
                desc,
                ref error
                );
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return data;
        }

        [ActionName("add_student_to_course")]
        [HttpPost]
        public dynamic add_student_to_course()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            dynamic model = null;


            var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
            string token = cookie.ToString();
            
            #region Model
            model = new
            {
                email = "",
                course_id = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            if (get_body == null || get_body.email == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);
            #endregion

            var data = course.save_student_to_course(
                get_body.email,
                get_body.course_id,
                auth,
                ref error
                );

            if(error == "")
            {
                manage.SendingEmail(data.student_id, get_body.course_id, "03",token, auth,null, ref error);
            }

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("student_by_id")]
        [HttpGet]
        public dynamic student_by_id(int id, string course_id)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.get_student_by_id(id, course_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("reset_course")]
        [HttpPut]
        public dynamic reset_course()
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var model = new
            {
                id = 0,
                course_id = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = course.reset_course(get_body.id, get_body.course_id, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("cancel_course")]
        [HttpPut]
        public dynamic cancel_course()
        {
            auth.IsAdmin = true;

            string error = "";
            GetToken(ref error);

            var model = new
            {
                id = 0,
                course_id = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var data = course.cancel_course(get_body.id, get_body.course_id, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion
        #endregion

        #region Frontend
        //success wait test
        [ActionName("list_category")]
        [HttpGet]
        public dynamic list_category()
        {
            string error = "";
            var data = course.get_list_category(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success wait test
        [ActionName("list_course")]
        [HttpGet]
        public dynamic list_course(string search = "", DateTime? register_start_date = null, DateTime? register_end_date = null, string category_id = "", int? learning_online = null, bool? is_free = false, int? price_gte = 0, int? price_lte = 0, int? hasCertificate = null, string sort = "")
        {
            string error = "";
            var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
            if (cookie != null)
            {
                string token = cookie?.ToString();
                auth.GetAuthentication(token);
            }

            var data = course.get_list_course(search, register_start_date, register_end_date, category_id, learning_online, is_free, price_gte, price_lte, hasCertificate, sort, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success wait test
        [ActionName("course_info")]
        [HttpGet]
        public dynamic course_info(string course_id = "")
        {
            check_token();
            string error = "";
            var data = course.get_course_info(course_id, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success wait test
        [ActionName("my_course_progress")]
        [HttpGet]
        public dynamic my_course_progress()
        {
            check_token();
            string error = "";
            var data = course.get_my_course_progress(auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success wait test
        [ActionName("print_course")]
        [HttpPut]
        public dynamic print_course(string course_id = "")
        {
            string error = "";
            var data = course.put_print_course(course_id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success wait test
        [ActionName("extend_study_time")]
        [HttpPut]
        public dynamic extend_study_time(string course_id = "")
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";
                var data = course.put_extend_study_time(course_id, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };
                return output;
            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
                return output;
            }
        }

        //success wait test
        [ActionName("course_by_id")]
        [HttpGet]
        public dynamic course_by_id(string course_id = "")
        {
            check_token();
            string error = "";
            var data = course.get_course_by_id(course_id, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        //success wait test
        [ActionName("stamp_exercise")]
        [HttpPut]
        public dynamic stamp_exercise()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";

                var model = new
                {
                    course_id = "",
                    course_lesson_id = 0
                };

                var get_body = Dtl.json_to_object(Dtl.json_request(), model);
                if (get_body == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);

                var data = course.put_stamp_exercise(get_body.course_id, get_body.course_lesson_id, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };
            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }
            return output;
        }

        //success wait test
        [ActionName("stamp_video_lesson")]
        [HttpPut]
        public dynamic stamp_video_lesson()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";

                var model = new
                {
                    course_id = "",
                    course_lesson_id = 0,
                    video_position = (decimal)0,
                    video_progress = (decimal)0
                };

                var get_body = Dtl.json_to_object(Dtl.json_request(), model);
                if (get_body == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);

                var data = course.put_stamp_video_lesson(get_body.course_id, get_body.course_lesson_id, get_body.video_position, get_body.video_progress, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };
            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }
            return output;

        }

        //success wait test
        [ActionName("send_answer_exam")]
        [HttpPost]
        public dynamic send_answer_exam()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";

                var model = new
                {
                    is_pretest = true,
                    course_id = "",
                    answer = new List<model_exam_logging>()
                };

                var get_body = Dtl.json_to_object(Dtl.json_request(), model);
                if (get_body == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);

                var data = course.post_send_answer_exam(get_body.is_pretest, get_body.course_id, get_body.answer, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };
            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }
            return output;
        }

        //success wait test
        [ActionName("send_answer_evaluation")]
        [HttpPost]
        public dynamic send_answer_evaluation()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";

                var model = new
                {
                    course_id = "",
                    answer = new List<model_evaluation_result>()
                };

                var get_body = Dtl.json_to_object(Dtl.json_request(), model);
                if (get_body == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);

                var data = course.post_send_answer_evaluation(get_body.course_id, get_body.answer, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };

            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }

            return output;
        }

        //success wait test
        [ActionName("my_course")]
        [HttpGet]
        public dynamic my_course()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";
                
                var data = course.get_my_course(auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };

            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }

            return output;
        }

        [ActionName("register_course_free")]
        [HttpPost]
        public dynamic register_course_free()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";

                var model = new
                {
                    course_id = ""
                };

                var get_body = Dtl.json_to_object(Dtl.json_request(), model);
                if (get_body == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);

                var data = course.post_register_course_free(get_body.course_id, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };

            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }

            return output;
        }

        [ActionName("register_course_voucher")]
        [HttpPost]
        public dynamic register_course_voucher()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";

                var model = new
                {
                    course_id = "",
                    voucher_id = ""
                };

                var get_body = Dtl.json_to_object(Dtl.json_request(), model);
                if (get_body == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);

                var data = course.post_register_course_voucher(get_body.course_id, get_body.voucher_id, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };

            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }

            return output;
        }
        

        [ActionName("count_view_lesson")]
        [HttpPost]
        public dynamic count_view_lesson()
        {
            var result = check_token();
            dynamic output = null;
            if (result)
            {
                string error = "";

                var model = new
                {
                    lesson_id = 0
                };

                var get_body = Dtl.json_to_object(Dtl.json_request(), model);
                if (get_body == null)
                    return new System.Web.Mvc.HttpStatusCodeResult(404);


                var data = course.post_count_view_lesson(get_body.lesson_id, auth, ref error);
                output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    data
                };

            }
            else
            {
                output = new
                {
                    success = false,
                    error = "ไม่พบสิทธิในการเข้าใช้งาน กรุณา Login ใหม่อีกครั้ง"
                };
            }
            return output;
        }
        #endregion

        [ActionName("check_file_course")]
        [HttpGet]
        public dynamic check_file_course(string filename, int type_column, int type_file)
        {
            auth.IsAdmin = true;
            string error = "";
            GetToken(ref error);

            var data = course.check_file_course(filename, type_column, type_file, ref error);
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