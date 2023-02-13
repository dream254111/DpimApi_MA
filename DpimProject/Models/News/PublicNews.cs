using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.News
{
    public class PublicNews
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public PublicNews()
        {
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic get_all_news(ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.public_new.Where(w => w.is_deleted == 0).OrderBy(x => x.created_at).ToList().Select(x => 
                    {
                        model_public_new news = new model_public_new();

                        news.id = x.id;
                        news.name = x.name;
                        news.image = x.image;// != null && x.image.Length != 0 ? "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.image : "";
                        news.description = x.description;
                        news.active = x.active == 1 ? true : false;

                        return news;
                    }).ToList();
                    
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic insert_news(model_public_new news, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    public_new data = new public_new()
                    {
                        name = news.name,
                        image = news.image,
                        description = news.description,
                        active = news.active ? 1 : 0,

                        is_deleted = 0,
                        created_by = auth.user_id,
                        created_at = now,
                        update_by = auth.user_id,
                        update_at = now
                    };

                    if (data.name == null || data.name.Length == 0) throw new Exception("name parameter is required");

                    db.public_new.Add(data);
                    db.SaveChanges();
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic update_news(model_public_new news, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    public_new data = db.public_new.Where(x => x.id == news.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.name = news.name == null ? data.name : news.name;
                        data.image = news.image == null ? data.image : news.image;
                        data.description = news.description == null ? data.description : news.description;
                        data.active = news.active ? 1 : 0;

                        data.update_by = auth.user_id;
                        data.update_at = now;
                    }
                    else
                    {
                        error = "Unable to find public news with this ID";
                    }

                    if (data.name == null || data.name.Length == 0) throw new Exception("name parameter is required");

                    db.SaveChanges();
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic delete_news(string id, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    int? id_new = id == null ? (int?)null : int.Parse(id);

                    public_new data = db.public_new.Where(x => x.id == id_new).FirstOrDefault();
                    if (data != null)
                    {
                        data.is_deleted = 1;
                        data.update_by = auth.user_id;
                        data.update_at = now;
                    }
                    else
                    {
                        error = "Unable to find public news with this ID";
                    }

                    db.SaveChanges();
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic get_news_by_id (string id, ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    int? id_new = id == null ? (int?)null : int.Parse(id);

                    public_new data = db.public_new.Where(x => x.id == id_new).FirstOrDefault();
                    if (data != null)
                    {
                        output = new { data };
                    }
                }
            }
            catch (Exception ex)
            {

                error = ex.Message;ErrorList(ex);
            }

            return output;
        }
        private HttpResponseException ErrorList(Exception ex)
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


            throw new HttpResponseException(resp);
        }
    }
}