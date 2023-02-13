using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using DpimProject.Models.DataTools;
using DpimProject.Models.Data;
using DpimProject.Models;
using Newtonsoft.Json;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

using DpimProject.Models.Banner;
using DpimProject.Models.Video;
using DpimProject.Models.RecommendSite;
using DpimProject.Models.News;
using DpimProject.Models.Payment;
using DpimProject.Models.Management;
using DpimProject.Models.Instructor;
using DpimProject.Models.Data.DataModels;
using System.Diagnostics;
using System.Threading.Tasks;
namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class StudentController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Student student;
        private Models.Management.Management management;
        public StudentController()
        {
            string error = "";
            auth = new Models.Authentication.Authentication();
            student = new Models.Student();
            management = new Models.Management.Management();

            //student.checkCourseEnddate();
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
                    success=string.IsNullOrEmpty(ex.Message),
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
            auth.IsAdmin = true;




        }
        [ActionName("ProfileUpdate")]
        [HttpPost]
        public HttpResponseMessage ProfileUpdate()
        {
            var error = "";
            GetToken();

            var f = new
            {
                student = new Models.Data.DataModels.model_update_profile()
            };

            f = Dtl.json_to_object(Dtl.json_request(), f);
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);

            dynamic data = null;
            if (string.IsNullOrEmpty(error)) {
                data = student.StudentUpdate(f.student, auth, ref error);
            }

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("RegisterStudent")]
        [HttpPost]
        public HttpResponseMessage RegisterStudent()
        {
            //GetToken();
            ////List<Models.Data.DataModels.user_permission> n = new List<Models.Data.DataModels.user_permission>();
            string error = "";
            var m = new
            {
                student = new Models.Data.DataModels.model_register()
            };
            m = Dtl.json_to_object(Dtl.json_request(), m);

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
          
            var data = student.StudentCreate(m.student, ref error);
            
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }

        [ActionName("StudentReadList")]
        [HttpGet]
        public dynamic StudentReadList(string search_text, int skip)
        {
            string error = "";

            GetToken();

            int take = 16;
            skip = (skip > 0) ? skip : 0;
            dynamic output = null;
            dynamic data = null;

                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (auth.isAuthenticated)
                {

                     data = student.StudentReadList(search_text, take, skip, ref error);

                  
                }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data,
            };
            return output;
        }
        [ActionName("StudentRead")]
        [HttpGet]
        public HttpResponseMessage StudentRead()
        {
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);

            string error = "";
            GetToken();

            dynamic output = null;
            dynamic data = null;

            if (string.IsNullOrEmpty(error))
                {

                     data = student.StudentRead(auth.student_id, ref error);

                  
                }
            output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error,
                        data
                    };
            
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("StudentProfile")]
        [HttpGet]
        public HttpResponseMessage StudentProfile()
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (auth.isAuthenticated)
            {

                data = student.StudentProfile(auth, ref error);

               
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("StudentCourseReadList")]
        [HttpGet]
        public HttpResponseMessage StudentCourseReadList()
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {

                 data = student.StudentCourseReadList(auth.student_id, ref error);

                
            }; output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }

        [ActionName("VideoOnDemandReadList")]
        [HttpGet]
        public HttpResponseMessage VideoOnDemandReadList()
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {

                 data = student.VideoOnDemandReadList(ref error);

               
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("StudentCourseRead")]
        [HttpGet]
        public HttpResponseMessage StudentCourseRead(string course_id)
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {

                 data = student.StudentCourseRead(auth.student_id, course_id, ref error);

               
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());

            return resp;
        }
        [ActionName("VideoOnDemandRead")]
        [HttpGet]
        public HttpResponseMessage VideoOnDemandRead(string course_catagory_id, int? video_on_demand_id)
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {

                 data = student.VideoOnDemandRead(course_catagory_id, video_on_demand_id, ref error);

               
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("VideoOnDemandCatagory")]
        [HttpGet]
        public HttpResponseMessage VideoOnDemandCatagory(string course_catagory_id)
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {

                 data = student.VideoOnDemandCatagory(course_catagory_id, ref error);

                
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
           
            return resp;
        }
        [ActionName("LessonReadList")]
        [HttpGet]
        public HttpResponseMessage LessonReadList()
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.LessonReadList(auth.student_id, ref error);

           
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());

            return resp;
        }
        [ActionName("LessonRead")]
        [HttpGet]
        public HttpResponseMessage LessonReadList(int? lesson_id)
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.LessonRead(auth.student_id, lesson_id, ref error);

              
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("LessonExerciseReadList")]
        [HttpGet]
        public HttpResponseMessage LessonExerciseReadList(string course_id, int? lesson_id)
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.LessonExerciseReadList(course_id, lesson_id, ref error);

              
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("ExamRead")]
        [HttpGet]
        public HttpResponseMessage ExamRead(string course_id)
        {
            //GetToken();

            string error = "";

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.ExamRead(course_id, ref error);


            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            //}
            return resp;
        } [ActionName("ExamPerCheck")]
        [HttpGet]
        public HttpResponseMessage ExamPerCheck(string course_id)
        {
            GetToken();

            string error = "";

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.ExamPerCheck(course_id,auth.user_id, ref error);


            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            //}
            return resp;
        }
        [ActionName("ExamReadList")]
        [HttpGet]
        public HttpResponseMessage ExamReadList(string course_id)
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.ExamReadList(course_id, ref error);

              
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());

            return resp;
        }
        [ActionName("ExamAnswer")]
        [HttpPost]
        public HttpResponseMessage ExamAnswer()
        {
            //GetToken();

            string error = "";
            GetToken();

            var m =new List<Models.Data.DataModels.course_exam_logging>();
            //    new
            //{
            //    course_exam_logging = new List<Models.Data.DataModels.course_exam_logging>()

            //};

            m = Dtl.json_to_object(Dtl.json_request(), m);
            //throw new Exception(JsonConvert.SerializeObject(m));
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.ExamAnswer(m, auth, ref error);

                
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            //}
            return resp;
        }
        [ActionName("ExamResult")]
        [HttpGet]
        public HttpResponseMessage ExamResult( string course_id)
        {

            string error = "";
            GetToken();

            ////var m = new
            ////{
            ////    course_exam_logging = new List<Models.Data.DataModels.course_exam_logging>()

            ////};

            ////m = Dtl.json_to_object(Dtl.json_request(), m);

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.ExamResult(auth.student_id, course_id, ref error);

               
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            //}
            return resp;
        }
        [ActionName("ExamPassCourseAll")]
        [HttpGet]
        public HttpResponseMessage ExamPassCourseAll()
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.ExamPassCourseAll(auth.student_id, ref error);

               
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            
            return resp;
        }
        [ActionName("StudentVideoProgress")]
        [HttpPost]
        public HttpResponseMessage StudentVideoProgress()
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            //if (auth.isAuthenticated)
            //{

            var m = new
            {
                video_progress = new Models.Data.DataModels.student_video_progress()

            };

            m = Dtl.json_to_object(Dtl.json_request(), m);
            if (string.IsNullOrEmpty(error))
            {
                student.StudentVideoProgress(m.video_progress, auth, ref error);
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,

            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            //}
            return resp;
        }
        [ActionName("Login")]
        [HttpPost]
     
        public HttpResponseMessage Login()
        {

            string token = "";
            string error = "";
            var f = new Models.Data.DataModels.user();
            f = Dtl.json_to_object(Dtl.json_request(), f);
            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            if (f.username.Contains("@dpim.go.th"))
            {
                var username = f.username.Substring(0, f.username.IndexOf("@dpim.go.th"));
                Task.Run(async () => { error = await DpimAuthentication(username, f.password); });
                auth.StundenLogin(f.username, f.password, true, ref token, ref error);


                //token_text = token;
                var output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    token = (string.IsNullOrEmpty(error)) ? token : "",
                    auth

                };
                resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());

            }
            else
            {
                auth.StundenLogin(f.username, f.password, true, ref token, ref error);


                //token_text = token;
                var output = new
                {
                    success = string.IsNullOrEmpty(error),
                    error,
                    token = (string.IsNullOrEmpty(error)) ? token : "",
                    auth

                };
                resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            }
            return resp;
        }
        [ActionName("AuthenticationDpim")]
        [HttpPost]
        public async Task<string> DpimAuthentication(string email, string password)
        {
            string error = "";
            var f = new
            {
                username = "",
                password = ""
            };
            f = Dtl.json_to_object(Dtl.json_request(), f);
            var m = new
            {
                username = email,
                password
            };
            HttpClient client = new HttpClient();
            DataTools dataTools = new DataTools();
            string str = JsonConvert.SerializeObject(m);
            var keyValue = new List<KeyValuePair<string, string>>();
            keyValue.Add(new KeyValuePair<string, string>("username", m.username));
            keyValue.Add(new KeyValuePair<string, string>("password", m.password));
            HttpContent content =
                new FormUrlEncodedContent(keyValue);

            content.Headers.Add("Secret-Key", "9d3f6ca6-61bf-ecc2-29f7-f25e68f7ff5d");

            //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

            var response = await client.PostAsync("http://dcen.dpim.go.th/api/auth/logon", content);
            dynamic resp = await response.Content.ReadAsStringAsync();
            var d1 = JsonConvert.DeserializeObject(resp);
            //if (d1["user"] == null)
            //{
            //    error = "ไม่พบข้อมูลในระบบ";
            //}
            var s = new Models.Data.DataModels.student
            {
                id_dpim = d1["user"]["id"],
                email = d1["user"]["email"],
                //title_name = d1["user"]["prefix_th"],
                //title_name_en = d1["user"]["prefix_en"],
                firstname = d1["user"]["name_th"],
                firstname_en = d1["user"]["name_en"],
                lastname = d1["user"]["lastname_th"],
                lastname_en = d1["user"]["lastname_en"],
                username = m.username,
                password = m.password

            };

            var data1 = auth.UserDpimRegister(s, ref error);
            var data = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data = data1
            };

            var output = new
            {
                data = data1
            };
            //return output;
            HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.OK);
            res.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return error;
        }
       
        [ActionName("CertificateReadList")]
        [HttpGet]
        public HttpResponseMessage CertificateReadList()
        {

            string error = "";
            GetToken();
            dynamic output = null;
            dynamic data = null;

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (string.IsNullOrEmpty(error))
            {
                 data = student.CertificateReadList(auth.student_id, ref error);
              
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("CertificateRead")]
        [HttpGet]
        public HttpResponseMessage CertificateRead(string certificate_id)
        {

            string error = "";
            GetToken();
            dynamic output = null;
            dynamic data = null;
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (string.IsNullOrEmpty(error))
            {
                 data = student.CertificateRead(certificate_id,auth.student_id, ref error);

            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("CheckCoursePermission")]
        [HttpGet]
        public HttpResponseMessage CheckCoursePermission(string course_id)
        {
            string error = "";
            GetToken();
            dynamic output = null;
            dynamic data = null;
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            if (string.IsNullOrEmpty(error))
            {
                data = student.CheckCoursePermission(course_id,auth, ref error);

            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("PrintCertificate")]
        [HttpGet]
        public HttpResponseMessage PrintCertificate(string course_id)
        {

            string error = "";
            GetToken();
            string fileName = "";
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.PrintCertificate(auth.student_id, course_id, ref error, ref fileName);
                string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["Certificate"];

                using (WebClient webClient = new WebClient())
                {
                    var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + fileName));

                    resp.Headers.AcceptRanges.Add("bytes");
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StreamContent(memoryStream);
                    resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    resp.Content.Headers.ContentDisposition.FileName = fileName;
                    resp.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
                    resp.Content.Headers.ContentLength = memoryStream.Length;
                }


            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            //resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("GenereteCertificate")]
        [HttpGet]
        public HttpResponseMessage GenereteCertificate(string course_id)
        {
            string error = "";

            GetToken();
            dynamic output = null;
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            //student_id = 1;
            //course_id = 2;   
            if (string.IsNullOrEmpty(error))
            {
                student.GenereteCertificate(auth.student_id, course_id, ref error);

            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("RequestForgetPassword")]
        [HttpPost]
        public HttpResponseMessage RequestForgetPassword()
        {
            string error = "";
            string token = "";
            dynamic output = null;
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);

            var m = new
            {
                email = ""
            };
            m = Dtl.json_to_object(Dtl.json_request(), m);
            if (string.IsNullOrEmpty(error))
            {
                student.StudentRequestForgetPassword(m.email, auth, ref error, ref token);

            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                token
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("ForgetPassword")]
        [HttpPost]
        public HttpResponseMessage ForgetPassword()
        {
            string error = "";

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);

            var m = new
            {
                password = "",
                token = ""
            };

            m = Dtl.json_to_object(Dtl.json_request(), m);
            dynamic output = null;
            dynamic data = null;
            if (string.IsNullOrEmpty(error))
            {
                 data = student.StudentForgetPassword(m.password, m.token, auth, ref error);
                
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("CheckEnddate")]
        [HttpGet]
        public HttpResponseMessage CheckEnddate()
        {
            //GetToken();

            string error = "";
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
          
    var data= student.checkCourseEnddate();
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }

        #region Manage API (Banner) [Dream]
        [ActionName("GetAllBanner")]
        [HttpGet]
        public dynamic GetAllBanner()
        {
            var error = "";
            var data = student.get_all_banner(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Manage API (Video On Demand) [Dream]

        [ActionName("GetVideo")]
        [HttpGet]
        public dynamic GetVideo()
        {
            string id = HttpContext.Current.Request.QueryString["id"];

            var video = new PublicVideo();

            var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
            if (cookie != null)
            {
                string token = cookie?.ToString();
                auth.GetAuthentication(token);
            }

            var error = "";
            var data = video.get_video_by_id(id, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("GetAllVideo")]
        [HttpGet]
        public dynamic GetAllVideo(string category_id = "", string sort = "")
        {
            var error = "";
            var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
            if (cookie != null)
            {
                string token = cookie?.ToString();
                auth.GetAuthentication(token);
            }
            
            var data = student.get_all_video(category_id, sort, auth, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Manage API (Recommended Website) [Dream]
        [ActionName("GetAllSite")]
        [HttpGet]
        public dynamic GetAllSite()
        {
            var error = "";
            var data = student.get_all_site(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Manage API (News) [Dream]
        [ActionName("GetAllNews")]
        [HttpGet]
        public dynamic GetAllNews()
        {
            var error = "";
            var data = student.get_all_news(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        [ActionName("GetNews")]
        [HttpGet]
        public dynamic GetNews()
        {
            string id = HttpContext.Current.Request.QueryString["id"];

            var news = new PublicNews();

            var error = "";
            var data = news.get_news_by_id(id, ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Manage API (Payment) [Dream]
        [ActionName("GetAllPayment")]
        [HttpGet]
        public dynamic GetAllPayment()
        {
            var payment = new Payment();

            var error = "";
            var data = payment.get_all_payment(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Other API (FAQ) [Dream]
        [ActionName("GetFAQ")]
        [HttpGet]
        public dynamic GetFAQ()
        {
            var manage = new Management();

            var error = "";
            var data = manage.ReadListFAQ(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Other API (Report Issue) [Dream]
        [ActionName("AddReportProblem")]
        [HttpPost]
        public dynamic AddReportProblem()
        {
            var model = new model_report_problem();

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);

            var manage = new Management();

            var error = string.Empty;
            var data = manage.add_report_problem(get_body, ref error, auth);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Other API (Tutorial) [Dream]
        [ActionName("TutorialReadList")]
        [HttpGet]
        public dynamic TutorialReadList()
        {
            string error = "";
            var data = student.get_tutorial(ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        #region Course API (Instructor) [Dream]
        [ActionName("GetInstructor")]
        [HttpGet]
        public dynamic GetInstructor(int id)
        {
            var instructor = new Instructor();

            string error = "";
            var data = instructor.get_course_and_instructor_by_id(id, ref error);

            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        [ActionName("special_days")]
        [HttpGet]
        public dynamic special_days()
        {
            var error = "";
            var data = student.get_special_days(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        #region master data
        [ActionName("master_data")]
        [HttpGet]
        public dynamic master_data()
        {
            string error = "";
            var data = student.get_master_data(ref error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }
        #endregion

        [ActionName("send_email_cert")]
        [HttpPost]
        public dynamic send_email_cert()
        {
            GetToken();
            dynamic output = null;
            string error = "";

            var model = new
            {
                course_id = ""
            };

            var get_body = Dtl.json_to_object(Dtl.json_request(), model);
            if (get_body == null)
                return new System.Web.Mvc.HttpStatusCodeResult(404);

            var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
            string token = cookie.ToString();
            var data = student.post_send_email_cert(get_body.course_id, token, auth, ref error);
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            
            return output;
        }

        [ActionName("profile_minimal")]
        [HttpGet]
        public HttpResponseMessage profile_minimal()
        {

            string error = "";
            GetToken();

            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            dynamic data = null;
            if (auth.isAuthenticated)
            {
                data = student.get_profile_minimal(auth, ref error);
            }
            output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }

        #region API I-Industry;
        public async Task AuthenticationIIndrustry()
        {
            var m = new
            {
                ClientID = "4nDMM9MnDZzUpbH",
                ClientSecrect = "jiVXXD0w0QK7P6u",
                AgentID = "3400101426101"
            };
            var keyValue = new List<KeyValuePair<string, string>>();
            keyValue.Add(new KeyValuePair<string, string>("ClientID", m.ClientID));
            keyValue.Add(new KeyValuePair<string, string>("ClientSecrect", m.ClientSecrect));
            keyValue.Add(new KeyValuePair<string, string>("AgentID", m.AgentID));
            var content = new FormUrlEncodedContent(keyValue);
            HttpClient client = new HttpClient();
            var response = await client.PostAsync("https://dpimsv.industry.go.th/api/v1/Authentication", content);


        }
        #endregion;
    }
}
