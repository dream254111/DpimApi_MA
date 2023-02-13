using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net;
using System.Net.Http;

using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using DpimProject.Security;
using System.Data;
using System.Collections.Specialized;
using Newtonsoft.Json;
using DpimProject.Models.DataTools;
using System.IO;
using System.Net.Mail;
using System.Collections;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.Instructor
{
    public class Instructor
    {
        //public dynamic InstructorReadlist(string search_text, int take, int skip, ref string errorMsg)
        public dynamic InstructorReadlist(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.instructor.Where(w => w.is_deleted == 0).ToList().Select(x => new
                    {
                        x.id,
                        x.firstname,
                        x.lastname,
                        name_th = $"{x.firstname} {x.lastname}",
                        name_en = $"{x.firstname_en} {x.lastname_en}",
                        x.position,
                        x.phone,
                        x.email,

                    });

                    //if (!string.IsNullOrEmpty(search_text))
                    //{
                    //    data = data.Where(x => x.firstname == search_text || x.lastname == search_text || x.email == search_text || x.position == search_text || x.phone == search_text || x.email == search_text);

                    //}
                    //data = data.ToList().Skip(skip).Take(take);
                    output = new
                    {
                        data
                    };
                } catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                return output;
            }

        }

        public dynamic get_course_and_instructor_by_id(int id, ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var instructor = db.instructor.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                    if (instructor == null)
                    {
                        error = "ไม่เจอผู้สอนในระบบ";
                        return false;
                    }

                    var course = (from c in db.course
                                  join ls in db.course_lesson.Where(w => w.is_deleted == 0) on c.id equals ls.course_id
                                  where c.is_deleted == 0 && ls.instructor_id == id
                                  select new
                                  {
                                      id = c.id,
                                      name = c.name,
                                      batch = c.batch,
                                      category_name = db.course_category.Where(w => w.id == c.course_category_id).Select(cat_name => cat_name.name).FirstOrDefault(),
                                      category_color = db.course_category.Where(w => w.id == c.course_category_id).Select(cat_color => cat_color.color).FirstOrDefault(),
                                      overview_course = c.overview_course,
                                      cover = c.cover_pic,
                                      total_lesson = db.course_lesson.Where(w => w.course_id == c.id && w.is_deleted == 0).Count(),
                                      lesson_time = db.course_lesson.Where(w => w.course_id == c.id && w.is_deleted == 0).Sum(sl => sl.lesson_time),
                                      is_always_learning = c.is_always_learning == 1 ? true : false,
                                      start_learning = c.learning_startdate,
                                      start_register = c.register_start_date,
                                      is_has_cost = c.is_has_cost == 1 ? true : false,
                                      cost = c.cost,
                                      created_at = c.created_dt,
                                      hasCertificate = c.hasCertificate == 1 ? true : false,
                                      list_instructor = (from cl in db.course_lesson
                                                         join ins in db.instructor.Where(w => w.is_deleted == 0) on cl.instructor_id equals ins.id
                                                         where cl.is_deleted == 0
                                                         select new
                                                         {
                                                             firstname = ins.firstname,
                                                             lastname = ins.lastname,
                                                             profile = ins.profile_image == null || ins.profile_image == "" ? "" : ins.profile_image
                                                         }).ToList()
                                  }).ToList();


                    course = course.GroupBy(g => g.id).Select(s => s.First()).ToList();

                    output = new
                    {
                        instructor,
                        course
                    };
                }
            }
            catch (Exception ex)
            {
                error = ex.InnerException?.Message ?? ex.Message;
            }

            return output;
        }

        public dynamic InstructorRead(int? instructor_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try {
                    var data = db.instructor.Where(x => x.id == instructor_id && x.is_deleted == 0)
                                            .Select(s => new {
                                                id = s.id,
                                                title_name = s.title_name,
                                                firstname = s.firstname,
                                                lastname = s.lastname,
                                                profile_image = s.profile_image == "" || s.profile_image == null ? "" : s.profile_image,
                                                position = s.position,
                                                work = s.work,
                                                email = s.email,
                                                phone = s.phone,
                                                facebook = s.facebook,
                                                twitter = s.twitter,
                                                instagram = s.instagram,
                                                description = s.description
                                            }).FirstOrDefault();
                    if (data != null)
                    {
                        output = new { data };
                    }
                } catch(Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic InstructorManage(instructor ins,Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.instructor.Where(x => x.id == ins.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.id = ins.id;
                        data.firstname = ins.firstname;
                        data.lastname = ins.lastname;
                        data.profile_image = ins.profile_image;
                        data.position = ins.position;
                        data.work = ins.work;
                        data.email = ins.email;
                        data.title_name = ins.title_name;
                        data.phone = ins.phone;
                        data.facebook = ins.facebook;
                        data.twitter = ins.twitter;
                        data.instagram = ins.instagram;
                        data.description = ins.description;
                      
                        data.update_by = auth.user_id;
                        data.update_at =DateTime.Now;
                        data.firstname_en = ins.firstname_en;
                        data.lastname_en = ins.lastname_en;
                        db.SaveChanges();
                        output = data;
                    }
                    else
                    {
                        ins.is_deleted = 0;
                        ins.created_at = DateTime.Now;
                        ins.created_by = auth.user_id;
                        ins.update_by = auth.user_id;
                        ins.update_at = DateTime.Now;
                        db.instructor.Add(ins);
                        db.SaveChanges();
                        output = ins;
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic InstuctorDelete(int id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.instructor.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                    if (data != null)
                    {
                        data.is_deleted = 1;
                        data.update_at = DateTime.Now;
                        data.update_by = 0; //auth.user_id
                        db.SaveChanges();

                        output = new
                        {
                            data
                        };
                    }
                    else
                    {
                        output = null;
                    }
                }
                catch (Exception ex)
                {
                    error_message = ex.Message;
                    ErrorList(ex);
                }
                return output;
            }
        }

        #region Category
        public dynamic CategoryReadList(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.course_category.ToList();
                    output = new
                    {
                        data
                    };
                }catch(Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic CategoryRead(string category_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.course_category.Where(x => x.id == category_id).FirstOrDefault();
                    if (data != null)
                    {
                        output = new
                        {
                            data
                        };
                    }
                }catch(Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic CategoryManage(course_category cat, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.course_category.Where(x => x.id == cat.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.name = cat.name;
                        data.color = cat.color;

                        db.SaveChanges();
                    }else
                    {
                        var get_id = db.course_category.Count();
                        get_id = get_id + 1;
                        cat.id = "0" + get_id.ToString();
                        db.course_category.Add(cat);
                        db.SaveChanges();
                        data = cat;
                    }

                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        #endregion

        public void SendingEmail(int? instructor_id,ref string errorMsg)
        {
            using (var db = new DataContext()) { 
                try
                {
                    var ins = db.instructor.Where(x => x.id == instructor_id).FirstOrDefault();
                    var fromAddress = new MailAddress("from@gmail.com", "From Name");
                    var toAddress = new MailAddress("to@example.com", "To Name");
                    const string fromPassword = "fromPassword";
                    const string subject = "Subject";
                    const string body = "Body";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                }
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