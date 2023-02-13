using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using DpimProject.Models.DataTools;
using DpimProject.Models.Data;
using Newtonsoft.Json;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DpimProject.Controllers
{
    //******** Access-Control-Allow-Orgin *********//\
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthenticationController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Student student;
      
        private DataTools dtl;
        private string token_text = "";
        private dynamic m = new
        {
            username = "",
            password = "",
            email=""

        };
        public AuthenticationController()
        {
            var token = "";
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            student = new Models.Student();
            //student.checkCourseEnddate();
            //string token_ = System.Web.HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim();
            //if (!string.IsNullOrEmpty(token_))
            //{
            //    auth.GetAuthentication(token_);
            //}
            //CookieHeaderValue cookie = Request.Headers.GetCookies("Authorization").FirstOrDefault();
            //if (cookie != null)
            //{
            //    token = cookie["Authorization"].Value;
            //}

            //if (!auth.CheckAuthentication(token))
            //{
            //     new HttpStatusCodeResult(403,"Access Denied");
            //}
            //auth.GetAuthentication(token);

        }

        //async protected override Task<HttpResponseMessage> SendAsync(
        //  HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        //{
        //    string sessionId;

        //    // Try to get the session ID from the request; otherwise create a new ID.
        //    var cookie = request.Headers.GetCookies("dpim_auth").FirstOrDefault();

        //    // Store the session ID in the request property bag.

        //    // Continue processing the HTTP request.
        //    HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        //    // Set the session ID as a cookie in the response message.
        //    response.Headers.AddCookies(new CookieHeaderValue[] {
        //    new CookieHeaderValue(SessionIdToken, sessionId)
        //});

        //    return response;
        //}
        [ActionName("Login")]
        [HttpPost]
              public HttpResponseMessage Login()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            auth.IsAdmin = true;
            string token = "";
            string error = "";
            var f = new Models.Data.DataModels.user();
            f = Dtl.json_to_object(Dtl.json_request(), f);
          
                auth.LogIn(f.username, f.password, true, ref token, ref error);


                CookieHeaderValue cookie = new CookieHeaderValue("dpim_auth", token);
                cookie.Secure = false;
                cookie.HttpOnly = false;
                cookie.Expires = DateTime.Now.AddYears(1);
            //token_text = token;
         var   output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                auth.menu_permission,
                token
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());

            return resp;
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
                    auth.IsAdmin = true;

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
                    error=ex.Message,
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

        [ActionName("ForgetPassword")]
        [HttpGet]
        public HttpResponseMessage ForgetPassword(string email)
        {
            string error = "";
            GetToken(ref error);
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);

            auth.ForgetPassword(email, auth, ref  error);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        
        }
        [ActionName("CheckToken")]
        [HttpGet]
        public HttpResponseMessage CheckToken()
        {
            var error = "";
            auth.IsAdmin = true;
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            GetToken(ref error);
            if (!string.IsNullOrEmpty(error))
            {
                auth = null;
            }

            var output = new

            {
                sucess = string.IsNullOrEmpty(error),
                error,
                auth
            };
                resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
                  
            return resp;
        }
        [ActionName("LogOut")]
        [HttpGet]
public void LogOut()
        {
            string token = "";
            string error = "";
            GetToken(ref error);

            auth.LogOut(auth, token_text,ref error);
          
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            CookieHeaderValue cookie = new CookieHeaderValue("dpim_auth",token);
           
            //token_text = token;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,

            };
        }
 
       
        [ActionName("CheckAuthen")]
        [HttpPost]
        public HttpResponseMessage CheckAuthen()
        {
            string errorMsg = "";
            GetToken(ref errorMsg);
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);

            try
            {
              
                var f = new
                {
                    //token_admin = "",
                    token = ""
                };
                f = Dtl.json_to_object(Dtl.json_request(), f);
                var t = f?.token.Trim();
                Security.Token t1 = new Security.Token();
                var password = "";

                if (t.IndexOf("/") > 0 || t.IndexOf("=") > 0 || t.IndexOf(".") > 0)
                {

                    t1.CheckToken(t, out t);
                    var DataS = JsonConvert.DeserializeObject<Dictionary<string, object>>(t);

                    password = DataS["password"].ToString().Trim();
                }
                else
                {
                    password = f.token.Trim();
                }
                //if (f.token_admin != null)
                //{
                //    var t2 = f?.token_admin.Trim();
                //    t1.CheckToken(t2, out t2);

                //var DataA = JsonConvert.DeserializeObject<Dictionary<string, object>>(t2);   }
                //var username = DataA["username"].ToString().Trim();
                var output = new
                {
                    password,
                    //username
                };
                resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            }catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();                // Get the top stack frame
                string stackIndent = ex.StackTrace;
                var error = new
                {

                    FileName = frame.GetFileName(),
                    line = frame.GetFileLineNumber(),
                    Method = frame.GetMethod(),
                    ex.Message
                };
               resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new ObjectContent<object>(error, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };


                throw new HttpResponseException(resp);
            }
            return resp;
        }
        [ActionName("ChangePassword")]
        [HttpPost]
        public void ChangePassword()
        {

            var f = new Models.Data.DataModels.user();
            string error = "";
            GetToken(ref error);

            ////List<Models.Data.DataModels.user_permission> n = new List<Models.Data.DataModels.user_permission>();

            f = Dtl.json_to_object(Dtl.json_request(), f);
            auth.ChangePassword(f,auth,ref error);     
        }
        [ActionName("RegisterAdmin")]
        [HttpPost]
        public HttpResponseMessage RegisterAdmin()
        {
            string error = "";
            var f = new
            {
                user = new Models.Data.DataModels.user(),
                token = ""
            };
            f = Dtl.json_to_object(Dtl.json_request(), f);
            auth.IsAdmin = true;
           
            if (!string.IsNullOrEmpty(f.token))
            {
                if (auth.CheckAuthentication(f.token))
                {
                    //var token = new Security.Token();
                    //var password = f.user.password;
                    //if (password.IndexOf("+") > 0)
                    //{
                    //    token.CheckToken(password, out password);
                    //}
                    //Dictionary<string, object> dataJson = new Dictionary<string, object>();
                    //dataJson.Add("password", password);
                    //var passJson = JsonConvert.SerializeObject(dataJson);
                    //password = token.CreateToken(passJson);

                    //f.user.password = password;
                    auth.registerAdmin(f.user, auth, ref error);

                }
                else
                {
                    error = "ท่านไม่ได้รับสิทธิในการเพิ่มข้อมูล";

                }
            }
            else
            {
                error = "ท่านไม่ได้รับสิทธิในการเพิ่มข้อมูล";
            }
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
              

            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("AdminList")]
        [HttpGet]
        public HttpResponseMessage AdminList()
        {
            string error = "";
            //GetToken(ref error);

            var data = auth.AdminList(ref error);
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
                
            };
            resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
            return resp;
        }
        [ActionName("GETIpAddress")]
        [HttpGet]
        public HttpResponseMessage GetIpValue()
        {
            var UserAgent = HttpContext.Current.Request.UserAgent;

            string ipAdd = "";
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
               ipAdd = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAdd))
            {
                ipAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            var output = new
            {
                ipAdd,
                UserAgent
            };
            resp.Content = new ObjectContent<object>(output,new JsonMediaTypeFormatter());
            return resp;
        }
    
    }
}
