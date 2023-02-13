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
namespace DpimProject.Models.ProfileManagement
{
    public class ProfileManagement
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public ProfileManagement()
        {
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic get_all_profile_manage(ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.manage_profile.ToList().Select(x => new
                    {
                        x.id,

                        x.is_edit_personal_info,
                        x.is_edit_address,
                        x.is_edit_email,
                        x.is_edit_phone,
                        x.is_edit_educational,
                        x.is_edit_career,
                        x.is_edit_know_channel,
                        x.is_edit_business,

                        x.is_deleted,
                        x.created_by,
                        x.created_at,
                        x.update_by,
                        x.update_at
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

        public dynamic update_profile_maange(manage_profile profile, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    if (db.manage_profile.Count() == 0)
                    {
                        manage_profile data = new manage_profile()
                        {
                            is_edit_personal_info = profile.is_edit_personal_info,
                            is_edit_address = profile.is_edit_address,
                            is_edit_email = profile.is_edit_email,
                            is_edit_phone = profile.is_edit_phone,
                            is_edit_educational = profile.is_edit_educational,
                            is_edit_career = profile.is_edit_career,
                            is_edit_know_channel = profile.is_edit_know_channel,
                            is_edit_business = profile.is_edit_business,

                            created_by = auth.user_id,
                            created_at = now,
                            update_by = auth.user_id,
                            update_at = now
                        };

                        db.manage_profile.Add(data);
                        db.SaveChanges();
                        output = new { data };
                    }
                    else
                    {
                        manage_profile data = db.manage_profile.Where(x => x.id != null).FirstOrDefault();

                        data.is_edit_personal_info = profile.is_edit_personal_info;
                        data.is_edit_address = profile.is_edit_address;
                        data.is_edit_email = profile.is_edit_email;
                        data.is_edit_phone = profile.is_edit_phone;
                        data.is_edit_educational = profile.is_edit_educational;
                        data.is_edit_career = profile.is_edit_career;
                        data.is_edit_know_channel = profile.is_edit_know_channel;
                        data.is_edit_business = profile.is_edit_business;

                        data.update_by = auth.user_id;
                        data.update_at = now;

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
    }
}