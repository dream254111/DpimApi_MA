using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.Banner
{
    public class Banner
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public Banner()
        {
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic get_all_banner(ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.banner.ToList().Select(x =>
                    {
                        if (x.is_deleted != 1)
                        {
                            model_banner banner = new model_banner();

                            banner.id = x.id;
                            banner.order = x.order;
                            banner.image_pc = x.image_pc;// "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.image_pc;
                            banner.image_mobile = x.image_mobile;// "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.image_mobile;
                            banner.link = x.link;
                            //banner.is_deleted = x.is_deleted;
                            //banner.created_at = x.created_at;
                            //banner.created_by = x.created_by;
                            //banner.update_at = x.update_at;
                            //banner.update_by = x.update_by;
                            banner.active = x.active == 1 ? true : false;

                            return banner;
                        }

                        return null;
                    }).ToList();

                    data.RemoveAll(x => x == null);

                    output = new { data };
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

                    FileName = frame.GetFileName(),
                    line = frame.GetFileLineNumber(),
                    Method = frame.GetMethod()
                };
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };

                error = ex.Message;
                throw new HttpResponseException(resp);
            }

            return output;
        }

        public dynamic insert_banner(model_banner banner, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var link = banner.link.ToString() == "" ? "" : (banner.link.ToString().IndexOf("https://") > -1 ? banner.link : "https://" + banner.link);
                    banner data = new banner()
                    {
                        order = banner.order,
                        image_pc = banner.image_pc,
                        image_mobile = banner.image_mobile,
                        link = link,
                        active = banner.active == true ? 1 : 0,

                        is_deleted = 0,
                        created_by = auth.user_id,
                        created_at = now,
                        update_by = auth.user_id,
                        update_at = now
                    };

                    db.banner.Add(data);
                    db.SaveChanges();
                    output = new { data };
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

                    FileName = frame.GetFileName(),
                    line = frame.GetFileLineNumber(),
                    Method = frame.GetMethod()
                };
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };


                error = ex.Message;
                throw new HttpResponseException(resp);
            }

            return output;
        }

        public dynamic update_banner(model_banner banner, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    banner data = db.banner.Where(x => x.id == banner.id).FirstOrDefault();
                    var link = banner.link == null ? data.link : (banner.link.ToString() == "" ? "" : banner.link.ToString().IndexOf("https://") > -1 ? banner.link : "https://" + banner.link);

                    if (data != null)
                    {
                        data.order = banner.order == null ? data.order : banner.order;
                        data.image_pc = banner.image_pc == null ? data.image_pc : banner.image_pc;
                        data.image_mobile = banner.image_mobile == null ? data.image_mobile : banner.image_mobile;
                        data.link = link;
                        data.active = banner.active == true ? 1 : 0;

                        data.update_by = auth.user_id;
                        data.update_at = now;
                    }
                    else
                    {
                        error = "Unable to find banner with this ID";
                    }

                    db.SaveChanges();
                    output = new { data };
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

                    FileName = frame.GetFileName(),
                    line = frame.GetFileLineNumber(),
                    Method = frame.GetMethod()
                };
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };


                error = ex.Message;
                throw new HttpResponseException(resp);
            }

            return output;
        }

        public dynamic delete_banner(string id, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    int? id_new = id == null ? (int?)null : int.Parse(id);

                    banner data = db.banner.Where(x => x.id == id_new).FirstOrDefault();
                    if (data != null)
                    {
                        data.is_deleted = 1;
                        data.update_by = auth.user_id;
                        data.update_at = now;
                    }
                    else
                    {
                        error = "Unable to find banner with this ID";
                    }

                    db.SaveChanges();
                    output = new { data };
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
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ObjectContent<object>(errorException, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };

                error = ex.Message;
                throw new HttpResponseException(resp);
            }

            return output;
        }
    }
}