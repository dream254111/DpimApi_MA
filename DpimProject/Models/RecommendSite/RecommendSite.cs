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
namespace DpimProject.Models.RecommendSite
{
    public class RecommendSite
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public RecommendSite()
        {
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic get_all_site(ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.recommend_site.ToList().Select(x => new
                    {
                        x.id,
                        x.name1,
                        x.link1,
                        x.cover1,
                        x.name2,
                        x.link2,
                        x.cover2,
                        x.name3,
                        x.link3,
                        x.cover3,
                        x.name4,
                        x.link4,
                        x.cover4
                    }).FirstOrDefault();

                    output = new { data };
                }
            }
            catch (Exception ex)
            {
              error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        /* Insert funtion is not necessary for recommend site (2020/12/06 - Dream) */
        //public dynamic insert_site(recommend_site site, ref string error, Authentication.Authentication auth)
        //{
        //    dynamic output = null;

        //    try
        //    {
        //        using (DataContext db = new DataContext())
        //        {
        //            recommend_site data = new recommend_site()
        //            {
        //                order = site.order,
        //                name = site.name,
        //                link = site.link,

        //                is_deleted = 0,
        //                created_by = auth.user_id,
        //                created_at = now,
        //                update_by = auth.user_id,
        //                update_at = now
        //            };

        //            db.recommend_site.Add(data);
        //            db.SaveChanges();
        //            output = new { data };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;
        //    }

        //    return output;
        //}

        public dynamic update_site(List<recommend_site_single> site, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    if (db.recommend_site.Count() == 0)
                    {
                        recommend_site data = new recommend_site()
                        {
                            name1 = site[0].name,
                            link1 = site[0].link,
                            cover1 = site[0].cover,

                            name2 = site[1].name,
                            link2 = site[1].link,
                            cover2 = site[1].cover,

                            name3 = site[2].name,
                            link3 = site[2].link,
                            cover3 = site[2].cover,

                            name4 = site[3].name,
                            link4 = site[3].link,
                            cover4 = site[3].cover
                        };

                        if (data.name1 == null || data.name1.Length == 0) throw new Exception("name1 parameter is required");
                        if (data.link1 == null || data.link1.Length == 0) throw new Exception("link1 parameter is required");
                        if (data.cover1 == null || data.cover1.Length == 0) throw new Exception("cover1 parameter is required");

                        if (data.name2 == null || data.name2.Length == 0) throw new Exception("name2 parameter is required");
                        if (data.link2 == null || data.link2.Length == 0) throw new Exception("link2 parameter is required");
                        if (data.cover2 == null || data.cover2.Length == 0) throw new Exception("cover2 parameter is required");

                        if (data.name3 == null || data.name3.Length == 0) throw new Exception("name3 parameter is required");
                        if (data.link3 == null || data.link3.Length == 0) throw new Exception("link3 parameter is required");
                        if (data.cover3 == null || data.cover3.Length == 0) throw new Exception("cover3 parameter is required");

                        if (data.name4 == null || data.name4.Length == 0) throw new Exception("name4 parameter is required");
                        if (data.link4 == null || data.link4.Length == 0) throw new Exception("link4 parameter is required");
                        if (data.cover4 == null || data.cover4.Length == 0) throw new Exception("cover4 parameter is required");

                        db.recommend_site.Add(data);
                        db.SaveChanges();
                        output = new { data };
                    }
                    else
                    {
                        recommend_site data = db.recommend_site.Where(x => x.id != null).FirstOrDefault();
                        data.name1 = site[0].name;
                        data.link1 = site[0].link;
                        data.cover1 = site[0].cover;

                        data.name2 = site[1].name;
                        data.link2 = site[1].link;
                        data.cover2 = site[1].cover;

                        data.name3 = site[2].name;
                        data.link3 = site[2].link;
                        data.cover3 = site[2].cover;

                        data.name4 = site[3].name;
                        data.link4 = site[3].link;
                        data.cover4 = site[3].cover;

                        if (data.name1 == null || data.name1.Length == 0) throw new Exception("name1 parameter is required");
                        if (data.link1 == null || data.link1.Length == 0) throw new Exception("link1 parameter is required");
                        if (data.cover1 == null || data.cover1.Length == 0) throw new Exception("cover1 parameter is required");

                        if (data.name2 == null || data.name2.Length == 0) throw new Exception("name2 parameter is required");
                        if (data.link2 == null || data.link2.Length == 0) throw new Exception("link2 parameter is required");
                        if (data.cover2 == null || data.cover2.Length == 0) throw new Exception("cover2 parameter is required");

                        if (data.name3 == null || data.name3.Length == 0) throw new Exception("name3 parameter is required");
                        if (data.link3 == null || data.link3.Length == 0) throw new Exception("link3 parameter is required");
                        if (data.cover3 == null || data.cover3.Length == 0) throw new Exception("cover3 parameter is required");

                        if (data.name4 == null || data.name4.Length == 0) throw new Exception("name4 parameter is required");
                        if (data.link4 == null || data.link4.Length == 0) throw new Exception("link4 parameter is required");
                        if (data.cover4 == null || data.cover4.Length == 0) throw new Exception("cover4 parameter is required");

                        db.SaveChanges();
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
        /* Delete funtion is not necessary for recommend site (2020/12/06 - Dream) */
        //public dynamic delete_site(string id, ref string error, Authentication.Authentication auth)
        //{
        //    dynamic output = null;

        //    try
        //    {
        //        using (DataContext db = new DataContext())
        //        {
        //            int? id_new = id == null ? (int?)null : int.Parse(id);

        //            recommend_site data = db.recommend_site.Where(x => x.id == id_new).FirstOrDefault();
        //            if (data != null)
        //            {
        //                data.is_deleted = 1;
        //                data.update_by = auth.user_id;
        //                data.update_at = now;
        //            }
        //            else
        //            {
        //                error = "Unable to find reccommend site with this ID";
        //            }

        //            db.SaveChanges();
        //            output = new { data };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;
        //    }

        //    return output;
        //}
    }
}