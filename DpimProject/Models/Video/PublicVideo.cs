using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.Video
{
    public class PublicVideo
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public PublicVideo()
        {
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic get_all_video(ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.video_on_demand.Where(w => w.is_deleted == 0).Select(s => new
                    {
                        id = s.id,
                        course_category_id = s.course_category_id,
                        name = s.name,
                        link_video = s.video,
                        count_view = s.count_view,
                        description = s.description,
                        producer_name = s.producer_name,
                        phone = s.phone,
                        email = s.email,
                        attachment = s.attachment,
                        is_deleted = s.is_deleted,
                        created_by = s.created_by,
                        created_dt = s.created_dt,
                        update_by = s.update_by,
                        update_dt = s.update_dt,
                        cover_thumbnail = s.cover_thumbnail,
                        content_permission = db.content_permission.Where(w => w.video_id == s.id).Select(ss => ss.student_type_permission).ToList(),
                        video = new
                        {
                            original = s.p_original,
                            p_480 = s.p_480,
                            p_720 = s.p_720,
                            p_1080 = s.p_1080,
                            thumbnail = s.cover_video
                        },
                        type = s.type,
                        youtube = s.youtube
                    }).ToList();

                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error =ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic get_all_video_filter(string categories, string order, ref string error)
        {
            dynamic output = null;

            try
            {
                var categories_trim = categories?.Replace("[", "").Replace("]", "") ?? "";
                var categories_list = categories_trim.Split(',');
                var order_new = order?.ToLower() ?? "";

                using (DataContext db = new DataContext())
                {
                    if (categories_list.Length == 0 || string.IsNullOrEmpty(categories_list[0]))
                    {
                        var data = db.video_on_demand.ToList().Select(x =>
                        {
                            video_on_demand video = new video_on_demand();

                            video.id = x.id;
                            video.course_category_id = x.course_category_id;
                            video.name = x.name;
                            video.video = x.video;
                            video.count_view = x.count_view;
                            video.description = x.description;
                            video.producer_name = x.producer_name;
                            video.phone = x.phone;
                            video.email = x.email;
                            video.attachment = x.attachment;// != null && x.attachment.Length != 0 ? "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.attachment : "";
                            video.attachment = x.attachment;
                            video.is_deleted = x.is_deleted;
                            video.created_by = x.created_by;
                            video.created_dt = x.created_dt;
                            video.update_by = x.update_by;
                            video.update_dt = x.update_dt;
                            video.cover_thumbnail = x.cover_thumbnail;// != null && x.cover_thumbnail.Length != 0 ? "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.cover_thumbnail : "";

                            return video;
                        }).ToList();

                        data.RemoveAll(x => x.is_deleted == 1);

                        if (order_new == "desc")
                            data = data.OrderByDescending(x => x.created_dt).ToList();
                        else
                            data = data.OrderBy(x => x.update_dt).ToList();

                        output = new { data };
                    }
                    else
                    {
                        var id_tmp = categories_list[0];
                        var data = db.video_on_demand.Where(x => x.course_category_id == id_tmp).ToList();

                        for (int i = 1; i < categories_list.Length; i++)
                        {
                            id_tmp = categories_list[i];
                            data = data.Union(db.video_on_demand.Where(x => x.course_category_id == id_tmp)).ToList();
                        }

                        data = data.Select(x =>
                        {
                            video_on_demand video = new video_on_demand();

                            video.id = x.id;
                            video.course_category_id = x.course_category_id;
                            video.name = x.name;
                            video.video = x.video;
                            video.count_view = x.count_view;
                            video.description = x.description;
                            video.producer_name = x.producer_name;
                            video.phone = x.phone;
                            video.email = x.email;
                            video.attachment = x.attachment;// != null && x.attachment.Length != 0 ? "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.attachment : "";
                            video.is_deleted = x.is_deleted;
                            video.created_by = x.created_by;
                            video.created_dt = x.created_dt;
                            video.update_by = x.update_by;
                            video.update_dt = x.update_dt;
                            video.cover_thumbnail = x.cover_thumbnail;// != null && x.cover_thumbnail.Length != 0 ? "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.cover_thumbnail : "";

                            return video;
                        }).ToList();

                        data.RemoveAll(x => x.is_deleted == 1);

                        if (order_new == "desc")
                            data = data.OrderByDescending(x => x.created_dt).ToList();
                        else
                            data = data.OrderBy(x => x.created_dt).ToList();

                        output = new { data };
                    }
                }
            }
            catch (Exception ex)
            {
                error =ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic insert_video(model_video_on_demand v, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var is_video = (v.video == null || v.video.original == null) ? false : true;
                    video_on_demand data = new video_on_demand();
                    data.course_category_id = v.course_category_id;
                    data.name = v.name;
                    data.video = "";
                    data.count_view = 0;
                    data.description = v.description;
                    data.producer_name = v.producer_name;
                    data.phone = v.phone;
                    data.email = v.email;
                    data.attachment = v.attachment;
                    data.cover_thumbnail = v.cover_thumbnail;

                    if(v.type == 1)
                    {
                        data.video_sample = is_video ? v.video.original : "";
                        data.p_480 = is_video ? v.video.p_480 : "";
                        data.p_720 = is_video ? v.video.p_720 : "";
                        data.p_1080 = is_video ? v.video.p_1080 : "";
                        data.p_original = is_video ? v.video.original : "";
                        data.cover_video = is_video ? v.video.thumbnail : "";
                        data.youtube = "";
                    }
                    else
                    {
                        data.video_sample = "";
                        data.p_480 = "";
                        data.p_720 = "";
                        data.p_1080 = "";
                        data.p_original = "";
                        data.cover_video = "";
                        data.youtube = v.youtube;
                    }

                    data.type = v.type;
                    data.is_deleted = 0;
                    data.created_by = auth.user_id;
                    data.created_dt = now;
                    data.update_by = auth.user_id;
                    data.update_dt = now;
                
                    db.video_on_demand.Add(data);
                    db.SaveChanges();

                    foreach (var item in v.content_permission)
                    {
                        content_permission content_permission = new content_permission();
                        content_permission.video_id = data.id;
                        content_permission.student_type_permission = item;

                        db.content_permission.Add(content_permission);
                        db.SaveChanges();
                    }
                    
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic update_video(model_video_on_demand v, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    video_on_demand data = db.video_on_demand.Where(x => x.id == v.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.course_category_id = v.course_category_id;
                        data.name = v.name;
                        data.video = "";
                        data.count_view = v.count_view;
                        data.description = v.description;
                        data.producer_name = v.producer_name;
                        data.phone = v.phone;
                        data.email = v.email;
                        
                        var is_video = (v.video == null || v.video.original == null) ? false : true;
                        if (is_video && v.type == 1)
                        {
                            data.video_sample = v.video.original;
                            data.p_480 = v.video.p_480;
                            data.p_720 = v.video.p_720;
                            data.p_1080 = v.video.p_1080;
                            data.p_original = v.video.original;
                            data.cover_video = v.video.thumbnail;
                            data.youtube = "";
                        }

                        if(v.type == 2)
                        {
                            data.youtube = v.youtube;
                            data.video_sample = "";
                            data.p_480 = "";
                            data.p_720 = "";
                            data.p_1080 = "";
                            data.p_original = "";
                            data.cover_video = "";
                        }

                        data.type = v.type;
                        data.update_by = auth.user_id;
                        data.update_dt = now;
                        
                        data.attachment = v.attachment == null ? data.attachment : v.attachment;
                        data.cover_thumbnail = v.cover_thumbnail == null ? data.cover_thumbnail : v.cover_thumbnail;

                        db.SaveChanges();
                        
                        #region remove old permission
                        foreach(var deleted in db.content_permission.Where(w => w.video_id == data.id).ToList())
                        {
                            db.content_permission.Remove(deleted);
                            db.SaveChanges();
                        }
                        #endregion

                        #region add new permission
                        foreach (var item in v.content_permission)
                        {
                            content_permission content_permission = new content_permission();
                            content_permission.video_id = data.id;
                            content_permission.student_type_permission = item;

                            db.content_permission.Add(content_permission);
                            db.SaveChanges();
                        }
                        #endregion
                    }
                    else
                    {
                        error = "Unable to find video with this ID";
                    }
                    
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error =ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic delete_video(string id, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    int? id_new = id == null ? (int?)null : int.Parse(id);

                    video_on_demand data = db.video_on_demand.Where(x => x.id == id_new).FirstOrDefault();
                    if (data != null)
                    {
                        data.is_deleted = 1;
                        data.update_by = auth.user_id;
                        data.update_dt = now;
                        db.SaveChanges();

                        foreach (var deleted in db.content_permission.Where(w => w.video_id == data.id).ToList())
                        {
                            db.content_permission.Remove(deleted);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        error = "Unable to find video with this ID";
                    }

                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error =ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic get_video_by_id(string id, Authentication.Authentication auth, ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    int? id_new = id == null ? (int?)null : int.Parse(id);

                    var student_type = auth.user_id == 0 ? 1 : db.student.Where(w => w.user_id == auth.user_id).Select(s => s.student_account_type_id).FirstOrDefault();
                    
                    var get_data = db.video_on_demand.Where(w => w.id == id_new && w.is_deleted == 0 && db.content_permission.Where(ww => ww.video_id == w.id && ww.student_type_permission == student_type).Count() > 0).FirstOrDefault();
                    if(get_data != null)
                    {
                        get_data.count_view = get_data.count_view == null ? 1 : (get_data.count_view + 1);
                        db.SaveChanges();

                        var data = new
                        {
                            id = get_data.id,
                            attachment = get_data.attachment,
                            count_view = get_data.count_view,
                            course_category_id = get_data.course_category_id,
                            category_nane = db.course_category.Where(w => w.id == get_data.course_category_id).Select(n => n.name).FirstOrDefault(),
                            category_color = db.course_category.Where(w => w.id == get_data.course_category_id).Select(cat_color => cat_color.color).FirstOrDefault(),
                            name = get_data.name,
                            link_video = get_data.video,
                            cover_thumbnail = get_data.cover_thumbnail,
                            created_dt = get_data.created_dt,
                            created_by = get_data.created_by,
                            description = get_data.description,
                            email = get_data.email,
                            is_deleted = get_data.is_deleted,
                            phone = get_data.phone,
                            producer_name = get_data.producer_name,
                            update_by = get_data.update_by,
                            update_dt = get_data.update_dt,
                            video = new
                            {
                                original = get_data.p_original,
                                p_480 = get_data.p_480,
                                p_720 = get_data.p_720,
                                p_1080 = get_data.p_1080,
                                thumbnail = get_data.cover_video
                            },
                            type = get_data.type,
                            youtube = get_data.youtube
                        };
                        
                        output = new { data };
                    }
                }
            }
            catch (Exception ex)
            {
                error =ex.Message;ErrorList(ex);
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