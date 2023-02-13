using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http.Cors;
using DpimProject.Models.Data;
using DpimProject.Security;
using System.Data;
using System.Collections.Specialized;
using Newtonsoft.Json;
using DpimProject.Models.DataTools;
using System.Text;
using DpimProject.Models.SignalR;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.Authentication
{
    public class Authentication
    {
        public int? user_id { get; set; } = 0;
        public int? student_id { get; set; } = 0;
        public string username { get; set; } = "";
        //public string password { get; set; } = "";
        public string email { get; set; } = "";
        public bool isAuthenticated { get; set; } = false;
        //public string comp_id { get; set; } = "";
        public bool IsAdmin { get; set; } = false;
        //public bool IsSuperAdmin { get; set; } = false;
        //public bool IsInstructor { get; set; } = false;
        //public bool isStudent { get; set; } = false;
        //public dynamic info { get; set; } = null;
        //public string session_id { get; set; } = "";
        public string profile_path { get; set; } = "";
        public string name_th { get; set; } = "";
        public dynamic menu_permission { get; set; } = null;
        public Authentication()
        {
            //req = new DataRequest();
            isAuthenticated = false;
        }
        public dynamic TEST(string username, string password)
        {
            var output = new { username, password };
            return output;
        }
        public bool LogIn(string username, string password, bool remember, ref string token_text, ref string error)
        {
            string ipAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString()?? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            var UserAgent = HttpContext.Current.Request.UserAgent;

            try
            {
                //throw new Exception("Test Exception Error");
                isAuthenticated = false;
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return false;
                }

                string newSession = "";
                Security.Password pwd = new Password();
                newSession = pwd.CreatePassword(16);


                DataContext db = new DataContext();
                //var new_user=   DataTools.Dtl.CreateUserID(db);
                //throw new Exception(new_user);
                var data = db.user
                        .Where(x =>
                        x.username == username || x.email == username
                        );

                if (data.Count() == 0)
                {

                    //error = "ไม่พบผู้ใช้งานนี้ในระบบ";
                    throw new Exception("ไม่พบผู้ใช้งานนี้ในระบบ");
                    //return false;

                }
                var token = new Token();

                var m = data.FirstOrDefault();
                var _password = m?.password.Trim();
                if (_password.IndexOf("/") > 0 || _password.IndexOf("=") > 0 || _password.IndexOf(".") > 0)
                {
                    token.CheckToken(_password, out _password);
                    var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(_password);
                    _password = dataJson["password"].ToString().Trim();

                }
                if (password != _password)
                {
                    //error = "รหัสผ่านไม่ถูกต้อง";
                    //return false;
                    throw new Exception("รหัสผ่านไม่ถูกต้อง");
                }

                if (m.is_deleted != 0)
                {

                    //error = "ไม่สามารถลงชื่อเข้าใช้งานระบบ โปรดติดต่อผู้บริหารระบบ";
                    throw new Exception("ไม่สามารถลงชื่อเข้าใช้งานระบบ โปรดติดต่อผู้บริหารระบบ");
                    //return false;

                }
                if (IsAdmin)
                {
                    if (m.role_id == 2)
                    {

                        //error = "ไม่ได้รับสิทธิให้เข้าระบบ โปรดติดต่อผู้บริหารระบบ";
                        //return false;

                        throw new Exception("ไม่ได้รับสิทธิให้เข้าระบบ โปรดติดต่อผู้บริหารระบบ");

                    }
                }
                //var app_auth = db.app_auth.Where(x => x.user_id == m.id && (x.end_dt > DateTime.Now||(x.active_dt??DateTime.Now)>DateTime.Now.AddHours(6))).FirstOrDefault();
                //if (app_auth != null)
                //{
                //    error = "ท่านลงชื่อเข้าใช้ในระบยเรียบร้อยแล้ว กรุณาลงชื่อออกจากอุปกรณ์ก่อนหน้า";

                //}

                //password expire

                db.SaveChanges();
                username = (m.username == username) ? m.username : m.email;
                Dictionary<string, object> authData = new Dictionary<string, object>();
                authData.Add("session1", newSession);
                authData.Add("username", username);
                authData.Add("permission", "Admin");

                string authDataJson = JsonConvert.SerializeObject(authData);
                //if (remember)
                //{
                authDataJson = token.CreateToken(authDataJson);
                token_text = authDataJson;

                HttpContext.Current.Response.Cookies.Add(new HttpCookie("dpim_auth") { Value = authDataJson, Expires = DateTime.UtcNow.AddYears(1) });
                //}
                this.isAuthenticated = true;
                DateTime? expire_date = DateTime.Now.AddHours(6);
                string session_ref = pwd.CreatePassword(16);
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("dpim_auth_session_ref") { Value = session_ref, Expires = DateTime.UtcNow.AddYears(1) });

                db.app_auth.RemoveRange(db.app_auth.Where(x => x.active_dt > x.end_dt).ToList());
                db.SaveChanges();
                var app = new Data.DataModels.app_auth
                {
                    user_id = m.id,
                    session_id = newSession,
                    start_dt = DateTime.Now,
                    end_dt = DateTime.Now.AddHours(6),
                    active_dt = DateTime.Now,
                    passcode = authDataJson,
                    ip_address=ipAdd,
                    device=UserAgent
                };
                var d = db.app_auth.Where(x => x.user_id == m.id ).FirstOrDefault();
                if (d== null)
                {
                    db.app_auth.Add(app);
                    db.SaveChanges();
                }
                else
                {
                    d.passcode = authDataJson;
                    d.active_dt = DateTime.Now;
                    d.end_dt = DateTime.Now.AddHours(6);
                    d.ip_address = ipAdd;
                    d.device = UserAgent;

                    db.SaveChanges();
                    //throw new Exception("ท่านลงชื่อเข้าใช้ในอุปกรณ์อื่นแล้ว กรุณา Log Out ออกจากระบบก่อน");

                }
                var permiss = db.admin_menu.ToList().Select(a => new
                {
                    a.menu_id,
                    a.menu_name,
                    a.menu_type,
                    status = db.menu_permission.Where(b => b.menu_id == a.menu_id && b.user_id == m.id.ToString()).Select(b => b.status).FirstOrDefault(),
                    disabled = (db.menu_permission.Where(b => b.menu_id == a.menu_id && b.user_id == m.id.ToString()).FirstOrDefault() != null) ? false : true
                }).ToList();
                //this.menu_permission=      db.menu_permission.Where(a => a.user_id == m.id.ToString()).ToList();
                this.menu_permission = permiss;
                this.isAuthenticated = true;
                this.user_id = m.id;
                this.username = m.username;


                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ErrorList(ex, HttpStatusCode.OK);
                return false;
            }

        }
        public bool StundenLogin(string username, string password, bool remember, ref string token_text, ref string error)
        {
            string ipAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString() ?? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            var UserAgent = HttpContext.Current.Request.UserAgent;


            try
            {
                isAuthenticated = false;
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return false;
                }

                string newSession = "";
                Security.Password pwd = new Password();
                newSession = pwd.CreatePassword(16);
                var token = new Token();


                DataContext db = new DataContext();
                //var new_user=   DataTools.Dtl.CreateUserID(db);
                //throw new Exception(new_user);
                var data = db.user
                        .Where(x =>
                        x.username == username || x.email == username
                        );
                if (data.Count() == 0)
                {
                    //error = "ไม่พบผู้ใช้งานนี้ในระบบ";
                    //return false;
                    throw new Exception("ไม่พบผู้ใช้งานนี้ในระบบ");
                }

                var m = data.FirstOrDefault();
                var _password = m?.password.Trim();
                if (_password.IndexOf("/") > 0 || _password.IndexOf("=") > 0 || _password.IndexOf(".") > 0)
                { 
                    token.CheckToken(_password, out _password);
                    var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(_password);
                    _password = dataJson["password"].ToString().Trim();

            }

                if (password != _password)
                {
                    //error = "รหัสผ่านไม่ถูกต้อง";
                    //return false;
                    throw new Exception("รหัสผ่านไม่ถูกต้อง");
                }

                if (m.is_deleted != 0)
                {
                    //error = "ไม่ได้รับสิทธิให้เข้าระบบ โปรดติดต่อผู้บริหารระบบ";
                    //return false;

                    throw new Exception("ไม่ได้รับสิทธิให้เข้าระบบ โปรดติดต่อผู้บริหารระบบ");
                }

                //var app_auth = db.app_auth.Where(x => x.user_id != m.id && (x.end_dt > DateTime.Now || (x.active_dt ?? DateTime.Now) > DateTime.Now.AddHours(6))).FirstOrDefault();
                //if (app_auth != null)
                //{
                //    error = "ท่านลงชื่อเข้าใช้ในระบยเรียบร้อยแล้ว กรุณาลงชื่อออกจากอุปกรณ์ก่อนหน้า";

                //}

                //password expire

                db.SaveChanges();
  
                Dictionary<string, object> authData = new Dictionary<string, object>();
                authData.Add("session1", newSession);
                authData.Add("username",username);
                authData.Add("permission", "Student");
                string authDataJson = JsonConvert.SerializeObject(authData);
                //if (remember)
                //{
                authDataJson = token.CreateToken(authDataJson);
                token_text = authDataJson;
                //HttpCookie cookie = new HttpCookie("Authorization");
                //cookie.Value = authDataJson;
                //cookie.Secure = false;
                //cookie.HttpOnly = false;
                //cookie.Expires = DateTime.Now.AddYears(1);
                //HttpContext.Current.Response.Cookies.Add(cookie);
                token_text = authDataJson;
                //}
                DateTime expire_date = DateTime.Now.AddHours(6);
                string session_ref = pwd.CreatePassword(16);
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("dpim_auth") { Value = authDataJson, Expires = DateTime.UtcNow.AddYears(1) });
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("dpim_auth_session_ref") { Value = session_ref, Expires = DateTime.UtcNow.AddYears(1) });
                db.app_auth.RemoveRange(db.app_auth.Where(x => x.active_dt > x.end_dt).ToList());
                db.SaveChanges();
                var app = new Data.DataModels.app_auth
                {
                    user_id = m.id,
                    session_id = newSession,
                    start_dt = DateTime.Now,
                    end_dt = DateTime.Now.AddHours(6),
                    active_dt = DateTime.Now,
                    passcode = authDataJson,
                    ip_address=ipAdd,
                    device = UserAgent

                };
                var d = db.app_auth.Where(x => x.user_id==m.id).FirstOrDefault();
                if (d == null)
                {
                    db.app_auth.Add(app);
                    db.SaveChanges();
                }
                else
                {
                    d.passcode = authDataJson;
                    d.active_dt = DateTime.Now;
                    d.end_dt = DateTime.Now.AddHours(6);
                    d.ip_address = ipAdd;
                    d.device = UserAgent;

                    db.SaveChanges();
                    //throw new Exception("ท่านลงชื่อเข้าใช้ในอุปกรณ์อื่นแล้ว กรุณา Log Out ออกจากระบบก่อน");

                }
                var stu = db.student.Where(x => x.user_id == m.id).FirstOrDefault();

                this.isAuthenticated = true;
                this.user_id = m?.id??0;
            this.student_id = stu?.student_id;
                this.username = m?.username??"";
                this.profile_path = stu?.profile_image??"";
                this.name_th = $"{stu?.title_name??""}{stu?.firstname??""} {stu?.lastname??""}";
                this.email = m?.email;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ErrorList(ex, HttpStatusCode.OK);
                return false;
            }

        }

        public void LogOut(Authentication auth, string token_text, ref string error)
        {
            try
            {

                DataContext db = new DataContext();

                db.app_auth.RemoveRange(db.app_auth.Where(x => x.user_id == auth.user_id && x.passcode == token_text).ToList());
                db.SaveChanges();
                db.Dispose();
                //db.hr_emp.Where(x => x.maincode == maincode && x.empno == empno).ToList().ForEach(x => x.session1 = null);
                //db.SaveChanges();
                //db.online_user.RemoveRange(db.online_user.Where(x => x.empno == empno));
                //db.SaveChanges();
                //db.Dispose();

                this.isAuthenticated = false;
                //this.IsSuperAdmin = false;
                this.IsAdmin = false;
                //this.IsInstructor = false;
                this.user_id = null;
                this.username = "";
                //this.info = null;
                //HttpContext.Current.Response.Cookies.Clear();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ErrorList(ex, HttpStatusCode.InternalServerError);
            }

        }
  
      
        public dynamic UpdateUsers(Data.DataModels.user u, Authentication auth, ref string error_massage)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    if (u != null)
                    {
                        var users = db.user.Where(x => x.username == u.username).FirstOrDefault();
                        //if (data != null)
                        //{
                        //    throw new Exception("มีชื่ออยู่ในระบบเรียบร้อยแล้ว ไม่สามารถสมัครสมาชิกได้");
                        //}
                        if (users != null)
                        {
                            users.username = u.username;
                            users.password = u.password;
                            users.email = u.email;
                            users.is_deleted = u.is_deleted;
                            users.update_at = DateTime.Now;
                            users.update_by = auth.user_id;
                            db.SaveChanges();
                        }
                        output = new
                        {
                            data = users
                        };


                    }
                }
                catch (Exception ex)
                {
                    error_massage = ex.Message;

                    ErrorList(ex, HttpStatusCode.InternalServerError);


                }
            }
            return output;
        }
        public void GetAuthentication()
        {
            string mg_auth_token = HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim();
            if (string.IsNullOrEmpty(mg_auth_token))
            {
                return;
            }
            GetAuthentication(mg_auth_token);
        }
        public void ChangePassword(Models.Data.DataModels.user u, Authentication auth, ref string errorMsg)
        {
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.user.Where(x => x.id == u.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.password = u.password;
                        data.update_at = DateTime.Now;
                        data.update_by = auth.user_id;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;

                    ErrorList(ex, HttpStatusCode.InternalServerError);
                }

            }
        }
        public dynamic AdminList(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.user.Where(x => x.role_id == 1).Select(x => new
                    {
                        x.id,
                        x.username,
                        x.email,
                        x.password
                    }).ToList();
               
                    output = new
                    {
                        data
                    };
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();                // Get the top stack frame
                    string stackIndent = ex.StackTrace;
                    var errorException = new
                    {
                        ex.Message,
                        FileName = frame.GetFileName(),
                        line = frame.GetFileLineNumber(),
                        Method = frame.GetMethod()
                    };
                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                        ReasonPhrase = ex.Message
                    };

                    errorMsg = ex.Message;
                    throw new HttpResponseException(resp);
                }

            }
            return output;
        }
        public void ForgetPassword(string email,Authentication auth,ref string errorMsg)
        {
            using (var db = new DataContext())
            {

                try
                {
                    var data = db.user.Where(x => x.id == auth.user_id).FirstOrDefault();
                    if (data.email != email)
                    {
                        throw new Exception("Email ไม่ถูกต้อง กรุณาลองใหม่อีกคครั้ง");
                    }
                    else
                    {
                        Management.Management management = new Management.Management();
                        management.SendingEmail(auth.student_id, "", "02", null, auth, 0, ref errorMsg);

                    }

                }
                catch (Exception ex) {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();                // Get the top stack frame
                    string stackIndent = ex.StackTrace;
                    var errorException = new
                    {
                        ex.Message,
                        FileName = frame.GetFileName(),
                        line = frame.GetFileLineNumber(),
                        Method = frame.GetMethod()
                    };
                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                        ReasonPhrase = ex.Message
                    };

                    errorMsg = ex.Message;
                    throw new HttpResponseException(resp);
                }
            }
        }
        public void registerAdmin(Data.DataModels.user u,Authentication auth, ref string errorMsg)
        {
            using (var db = new DataContext())
            {
                try
                {
                    Token token = new Token();

                    var password = u.password.Trim();
                    if (string.IsNullOrEmpty(u.password))
                    {
                        var data = db.user.Where(x => x.id == u.id).FirstOrDefault();
                        if (data != null)
                        {

                            data.email = u.email;
                            data.is_deleted = 0;
                            data.update_by = auth.user_id;
                            data.update_at = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (password.IndexOf("/") > 0 || password.IndexOf("=") > 0 || password.IndexOf(".") > 0)
                        {
                            var pass_ = "";
                            token.CheckToken(password, out pass_);
                            var DataS = JsonConvert.DeserializeObject<Dictionary<string, object>>(pass_);
                            password = DataS["password"].ToString().Trim();
                        }
                        Dictionary<string, object> dataJson = new Dictionary<string, object>();
                        dataJson.Add("password", password);
                        var passJson = JsonConvert.SerializeObject(dataJson);
                      password = token.CreateToken(passJson);

                        var data = db.user.Where(x => x.email== u.email).FirstOrDefault();
                        if (data != null)
                        {
                            data.username = u?.username;
                            data.password = password;
                            data.is_deleted = 0;
                            data.update_by = auth.user_id;
                            data.update_at = DateTime.Now;
                            db.SaveChanges();
                        }
                        else
                        {
                            var itemno = (db.user.Select(x => x.id).Max() ?? 0);
                            u.id = ++itemno;
                            u.created_at = DateTime.Now;
                            u.password = password;
                            u.created_by = auth.user_id;
                            u.role_id = 1;
                            u.is_deleted = 0;

                            db.user.Add(u);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();                // Get the top stack frame
                    string stackIndent = ex.StackTrace;
                    var errorException = new
                    {
                        ex.Message,

                        FileName = frame.GetFileName(),
                        line = frame.GetFileLineNumber(),
                        Method = frame.GetMethod()
                    };
                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                        ReasonPhrase = ex.Message
                    };

                    errorMsg = ex.Message;

                    throw new HttpResponseException(resp);
                }
            }
        }



        public void GetAuthentication(string mg_auth_token)
        {
            using (DataContext db = new DataContext()) {
                //try { 
                var ap = db.app_auth.Where(x => x.passcode == mg_auth_token).FirstOrDefault();
                if (ap == null)
                {
                    throw new Exception("มีการรลงชื่อเข้าใช้งานจากอุปกรณ์เครื่องอื่น กรุณาตรวจสอบข้อมูลของท่าน");
                }
                isAuthenticated = false;

            Token token = new Token();
            Dictionary<string, object> authData = null;
            try
            {
                token.CheckToken(mg_auth_token, out mg_auth_token);
                authData = JsonConvert.DeserializeObject<Dictionary<string, object>>(mg_auth_token);
            }catch(Exception ex) { ErrorList(ex, HttpStatusCode.OK); }

            if (authData == null || authData.Count == 0)
            {
                return;
            }

            string username = authData["username"].ToString();
            string permiissiion = authData["username"].ToString();
            int? user_id = db.user.Where(x => x.username == username || x.email == username).Select(x => x.id).FirstOrDefault();
            DateTime expire_active = DateTime.Now.AddHours(6);
             
                var app = db.app_auth.Where(x => x.user_id == user_id).FirstOrDefault();
            app.active_dt = DateTime.Now;
            app.end_dt = DateTime.Now.AddHours(6);

            db.SaveChanges();
                //var app_auth = db.app_auth.Where(x =>( x.end_dt < DateTime.Now) || ((x.active_dt ?? DateTime.Now) < expire_active)).FirstOrDefault();
                //if (app_auth != null)
                //{
                //    return;
                //}
                //var d = db.app_auth.Where(x => x.user_id == user_id && x.passcode == mg_auth_token).FirstOrDefault();
                //if (d == null)
                //{
                //    throw new Exception("มีการรลงชื่อเข้าใช้งานจากอุปกรณ์เครื่องอื่น กรุณาตรวจสอบข้อมูลของท่าน");
                //}
                var data = db.user.Where(x => x.id == user_id).FirstOrDefault();
            if (this.IsAdmin)
            {
                if (data.role_id == 2)
                {
                    throw new Exception("คุณไม่ได้รับสิทธิเข้าใช้งานหน้านี้ กรุณาติดต่อผู้ดูแลระบบ");

                }
            }
            if (data == null) return;
                if (data != null)
                {



                    var stu = db.student.Where(x => x.user_id == data.id).FirstOrDefault();
                    this.profile_path = stu?.profile_image;
                    this.name_th = $"{stu?.title_name}{stu?.firstname} {stu?.lastname}";

                    this.isAuthenticated = true;
                    this.user_id = data?.id;
                    this.username = data?.username ?? "";
                    this.student_id = stu?.student_id;

                    //this.comp_id = comp_id ?? "";
                    //this.info = data;
                    //var info = new
                    //{
                    //    auth = data,

                    //};
                    //this.info = info;




                    db.app_auth.RemoveRange(db.app_auth.Where(x => x.active_dt > x.end_dt).ToList());
                    db.SaveChanges();
                }
                //}
                //catch (Exception ex)
                //{
                //    var st = new StackTrace(ex, true);
                //    // Get the top stack frame
                //    var frame = st.GetFrame(0);
                //    // Get the line number from the stack frame
                //    var line = frame.GetFileLineNumber();                // Get the top stack frame
                //    string stackIndent = ex.StackTrace;
                //    var errorException = new
                //    {
                //        ex.Message,
                //        FileName = frame.GetFileName(),
                //        line = frame.GetFileLineNumber(),
                //        Method = frame.GetMethod()
                //    };
                //    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                //    {
                //        Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                //        ReasonPhrase = ex.Message
                //    };


                //    throw new HttpResponseException(resp);
                //    return;

                //}
            }

        }
        public bool CheckAuthentication(string mg_auth_token)
        {
            isAuthenticated = false;

            Token token = new Token();
            Dictionary<string, object> authData = null;
            try
            {
                token.CheckToken(mg_auth_token, out mg_auth_token);
                authData = JsonConvert.DeserializeObject<Dictionary<string, object>>(mg_auth_token);
            }
            catch (Exception ex)
            {
                ErrorList(ex, HttpStatusCode.InternalServerError);
                return false;

            }

            if (authData == null || authData.Count == 0)
            {
                return false;
            }
            DataContext db = new DataContext();

            string username = authData["username"].ToString();
            string permiissiion = authData["username"].ToString();
            int? user_id = db.user.Where(x => x.username == username || x.email == username).Select(x => x.id).FirstOrDefault();
            DateTime expire_active = DateTime.Now.AddHours(6);
            var app = db.app_auth.Where(x => x.user_id == user_id).FirstOrDefault();
            app.active_dt = DateTime.Now;
            app.end_dt = DateTime.Now.AddHours(6);


            db.SaveChanges();
            var app_auth = db.app_auth.Where(x => (x.end_dt < DateTime.Now) || ((x.active_dt ?? DateTime.Now) < expire_active)).FirstOrDefault();
            if (app_auth != null)
            {
                return false;
            }
            var data = db.user.Where(x => x.id == user_id).FirstOrDefault();
            if (this.IsAdmin)
            {
                if (data.role_id == 2)
                {
                    return false;

                }
            }
            if (data == null) return false;
            ;
            if (data != null)
            {

                this.isAuthenticated = true;


                var stu = db.student.Where(x => x.user_id == data.id).FirstOrDefault();
                if (stu != null)
                {
                    this.profile_path = stu?.profile_image;
                    this.name_th = $"{stu?.title_name}{stu?.firstname} {stu?.lastname}";
                    this.student_id = stu?.student_id;

                    this.user_id = data?.id;
                    this.username = data?.username ?? "";
                }
                //this.comp_id = comp_id ?? "";
                //this.info = data;
                //var info = new
                //{
                //    auth = data,

                //};
                //this.info = info;




                db.app_auth.RemoveRange(db.app_auth.Where(x => x.active_dt > x.end_dt).ToList());
                db.SaveChanges();
            }
            return true;

        }
        public dynamic UserDpimRegister(Models.Data.DataModels.student s,ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try {
                    var data = db.student.Where(x => x.email == s.email).FirstOrDefault();
                    if (data != null)
                    {




                        data.student_account_type_id = s.student_account_type_id;
                        data.profile_image = s.profile_image;


                        data.gender = s.gender;
                        data.birthday = s.birthday;
                        data.address = s.address;
                        data.sub_district_id = s.sub_district_id;
                        data.district_id = s.district_id;
                        data.province_id = s.province_id;
                        data.zipcode = s.zipcode;
                        data.phone = s.phone;
                        data.educational_id = s.educational_id;
                        data.position = s.position;
                        data.update_at = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {

                        var id = db.user.Select(x => x.id).Max();
                        var user = new Data.DataModels.user
                        { id = id+1,
                            username = s.email,
                            email = s.email,
                            password = s.password,
                            created_at = DateTime.Now,
                            role_id = 2,
                            is_deleted = 0
                        };
                        db.user.Add(user);
                    var student_id = db.student.Select(x => x.id).Max();
                    s.id = student_id + 1;
                    s.user_id = user.id;
                    s.student_id = user.id;
                    db.student.Add(s);
                    db.SaveChanges();
                    output = new
                    {
                        data = user
                    };
                    }
                } catch(Exception ex) { ErrorList(ex, HttpStatusCode.InternalServerError);
                }
            }
            return output;
        }
        private HttpResponseException ErrorList(Exception ex, HttpStatusCode status)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();                // Get the top stack frame
            string stackIndent = ex.StackTrace;
            var errorException = new
            {
                success=string.IsNullOrEmpty(ex.Message),
                error=ex.Message,
                ex.Message,
                FileName = frame.GetFileName(),
                line = frame.GetFileLineNumber(),
                Method = frame.GetMethod(),
            };
            HttpResponseMessage resp = new HttpResponseMessage(status)
            {
                Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                ReasonPhrase = ex.Message
            };

            throw new HttpResponseException(resp);
        }
    }
        public class UserOnline
    {
        public UserOnline()
        {
        }

        //public int UserOnlineCount()
        //{
        //  DataContext db = new DataContext();
        //    var c = db.user_online.Select(x => x.user_id).Distinct().Count();
        //    db.Dispose();
        //    return c;
        //}

        //public void RemoveUserOnline(string user_id)
        //{
        //    DataContext db = new DataContext();
        //    db.user_online.RemoveRange(db.user_online.Where(x => x.user_id == user_id).ToList());
        //    db.SaveChanges();
        //    db.Dispose();
        //}
    }
}