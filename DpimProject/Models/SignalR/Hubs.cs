using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
//using DpimProject.Models.DataModels;
using DpimProject.Models;
namespace DpimProject.Models.SignalR
{
    public class UserHubModels
    {
        public string session_id { get; set; }
        public int? user_id { get; set; }
        public string ConectionIds { get; set; }
        public DateTime? session_active { get; set; }
        public DateTime? session_expired { get; set; }
    }
    public class GlobalVar
    {
        //public static List<UserHubModels> UserHubModelsList;
        public static List<Data.DataModels.online_user> UserOnline { get; set; } = new List<Data.DataModels.online_user>();
    }
    public class authHub : Hub
    {
      

      
        private string dpim_token = "";
        private Models.Authentication.Authentication auth = new Models.Authentication.Authentication();
        public override Task OnConnected()
        {
            updateAuth();
            var context = new UserHubModels();
            context.ConectionIds = Context.ConnectionId;
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            updateAuth();
           
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            GetAuthen();
            if (auth.isAuthenticated)
            {
                using (var db = new Models.Data.DataContext())
                {
                    try
                    {
                        if (SignalR.GlobalVar.UserOnline.Where(x => x.user_id == auth.user_id).Count() < 2)
                        {
                            db.online_user.RemoveRange(db.online_user.Where(x => x.user_id == auth.user_id));
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            GlobalVar.UserOnline.RemoveAll(x => x.session_id == Context.ConnectionId);
            
            return base.OnDisconnected(stopCalled);
        }

        public void GetAuthen()
        {
            //easy_token = Context.QueryString["easy_token"]?.Trim() ?? "";
            //var dpim_token = HttpContext.Current.Request.Cookies["dpim_auth"]?.Value ?? "";
          var  dpim_token = Context.QueryString["dpim_auth"]?.Trim() ?? "";
            //throw new Exception(dpim_token);
            auth = new Models.Authentication.Authentication();
            auth.GetAuthentication(dpim_token);
        }

        public void updateAuth()
        {
            GetAuthen();
            using (var db = new Models.Data.DataContext())
            {


                db.online_user.RemoveRange(db.online_user.Where(x => x.session_expired < DateTime.Now));
                var session_data = GlobalVar.UserOnline.Where(x => x.session_id == Context.ConnectionId).FirstOrDefault();
                if (session_data != null)
                {
                    session_data.user_id = auth.user_id;
                    session_data.session_active = DateTime.Now;
                    session_data.session_expired = session_data.session_active?.AddSeconds(60);
                }
                else
                {

                    var session = new Models.Data.DataModels.online_user();
                    session.session_id = Context.ConnectionId;
                    session.user_id = auth.user_id;
                    GlobalVar.UserOnline.Add(session);

                }
                var ip_address = Context.Request.Environment["server.RemoteIpAddress"]?.ToString() ?? "";
                if (auth.user_id != null)
                {
                    var data = db.online_user.Where(x => x.user_id == auth.user_id).FirstOrDefault();                       //throw new Exception(auth.user_id);
                    if (data != null)
                    {
                        //db.online_user.RemoveRange(db.online_user.Where(x => x.ip_address != ip_address));
                        //db.SaveChanges();
                        data.ip_address = Context.Request.Environment["server.RemoteIpAddress"]?.ToString() ?? "";
                        //data.empno = empno;
                        data.session_active = DateTime.Now;
                        data.user_id = auth.user_id;
                        data.session_expired = DateTime.Now.AddMinutes(3);
                        data.session_id = Context.ConnectionId;
                        db.SaveChanges();


                    }
                    else
                    {

                        var m = new Models.Data.DataModels.online_user();
                        m.ip_address = Context.Request.Environment["server.RemoteIpAddress"]?.ToString() ?? "";
                        m.session_active = DateTime.Now;
                        m.session_start = DateTime.Now;
                        m.session_expired = DateTime.Now.AddMinutes(3);
                        m.user_id = auth.user_id;
                        m.session_id = Context.ConnectionId;
                        db.online_user.Add(m);

                        db.SaveChanges();

                        //Clients.Caller.updateUserOnline(db.app_authen.Select(x => x.user_id).Distinct().Count(), Licence.MaxUser);
                    }
                }

                //var d = db.chq_proj.Where(x => x.start_dt <= DateTime.Now).ToList();
                //d.ForEach(x => {

                //    x.active = "Y";
                //    db.SaveChanges();
                //});


                db.SaveChanges();

                HttpContext.Current.Response.Cookies.Add(new HttpCookie("dpim_auth_session_ref")
                {
                    Value =/* Context.Request.Environment["server.RemoteIpAddress"]?.ToString() ?? "",*/ new Security.Token().CreateToken(Context.ConnectionId),
                    Expires = DateTime.Now.AddMinutes(60)
                });






            }
        }

    }
}