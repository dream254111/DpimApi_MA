using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using DpimProject.Security;
using System.Data;
using System.Collections.Specialized;
using Newtonsoft.Json;
using DpimProject.Models.DataTools;
using System.IO;
using System.Net.Mail;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.Management
{
    public class Management
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        //private readonly string path_picture = "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=";

        public Management()
        {
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic TutorialReadList(ref string error)
        {
            dynamic output = null;
                using (var db = new DataContext()) {
                try
                {
                    var get_sub = db.tutorial_detail;
                    var data = db.tutorial_header.Select(s => new
                    {
                        header = new
                        {
                            tutorial_id = s.tutorial_id,
                            order = s.order,
                            tutorial_text = s.tutorial_text,
                            image = (s.image == null || s.image == "") ? "" : s.image,
                            link = s.link,
                        },
                        detail = get_sub.Where(w => w.tutorial_id == s.tutorial_id)
                                                   .Select(sub => new {
                                                        detail_id = sub.detail_id,
                                                        tutorial_id = sub.tutorial_id,
                                                        order = sub.order,
                                                        img_path = (sub.img_path == null || sub.img_path == "") ? "" : sub.img_path,
                                                        title = sub.title
                                                   }).ToList()
                    }).ToList();

                    output = data;
                }catch(Exception ex)
                {
                    error = ex.Message;
                }
                
            }
            return output;
        }

        public dynamic TutorialManage(List<model_tutorial> data, Authentication.Authentication auth,ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    #region Delete old data
                    foreach(var header in db.tutorial_header.ToList())
                    {
                        db.tutorial_header.Remove(header);
                        db.SaveChanges();
                    }

                    foreach (var detail in db.tutorial_detail.ToList())
                    {
                        db.tutorial_detail.Remove(detail);
                        db.SaveChanges();
                    }
                    #endregion

                    foreach (var item in data)
                    {
                        tutorial_header tutorial_header = new tutorial_header();
                        tutorial_header.order = item.header.order;
                        tutorial_header.tutorial_text = item.header.tutorial_text;
                        tutorial_header.link = item.header.link;

                        var check_image_header = item.header.image;
                        if(check_image_header != null)
                        {
                            tutorial_header.image = item.header.image;
                        }
                        db.tutorial_header.Add(tutorial_header);
                        db.SaveChanges();

                        var get_id = tutorial_header.tutorial_id;

                        foreach(var sub in item.detail)
                        {
                            tutorial_detail tutorial_detail = new tutorial_detail();
                            tutorial_detail.tutorial_id = get_id;
                            tutorial_detail.order = sub.order;
                            tutorial_detail.title = sub.title;

                            var check_image_sub = sub.img_path;
                            if (check_image_sub != null)
                            {
                                tutorial_detail.img_path = sub.img_path;
                            }
                            db.tutorial_detail.Add(tutorial_detail);
                            db.SaveChanges();
                        }
                    }

                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }

                return output;
            }
        }

        public void TutorialDelete(int tutorial_id,ref string errorMsg)
        {
            using (var db = new DataContext())
            {
                try
                {
                    db.tutorial_header.Remove(db.tutorial_header.Where(a => a.tutorial_id == tutorial_id).FirstOrDefault());
                    db.tutorial_detail.RemoveRange(db.tutorial_detail.Where(a => a.tutorial_id == tutorial_id).ToList());
                    db.SaveChanges();
                }catch(Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                }
        }

        public void SendingEmail(int? student_id,string course_id, string email_type,string token, Authentication.Authentication auth,int? problem_id,ref string error)
        {
            using (var db = new DataContext())
            {
                try
                {

                    var dir = System.Configuration.ConfigurationManager.AppSettings["EmailTemplate"];
                    var templatePath = System.Configuration.ConfigurationManager.AppSettings["EmailTemplate"] + "\\";
                    var from = db.email_admin.Where(x => x.email_type.Contains("gmail")).FirstOrDefault();
                    var student = db.student.Where(x => x.student_id == student_id).FirstOrDefault();
                    //throw new Exception(student.email);
                    var emailType = db.email_type.Where(x => x.email_type_id == email_type).FirstOrDefault();
                    //throw new Exception(dir+emailType.path);

                    var fromAddress = new MailAddress(from.email, from.admin_name);
                    //var fromAddress = new MailAddress("dpimacademy@gmail.com", "Dpim Adim");
                    var imgPath = System.Configuration.ConfigurationManager.AppSettings["EmailTemplate"] + "img\\logo.png";
                    var certificatePath = "";
                    LinkedResource inline = new LinkedResource(imgPath, MediaTypeNames.Image.Jpeg);
                    inline.ContentId = Guid.NewGuid().ToString();
                    Attachment att = new Attachment(imgPath);
                    att.ContentDisposition.Inline = true;
                    MailAddress toAddress = null;
                    var token_ = "";
                    if (student != null)
                    {
                        var title_name = (student.title_name == null) ? "คุณ" : student.title_name;
                        var student_name = $"{title_name}{student.firstname} {student.lastname}";
                         toAddress = new MailAddress(student.email, student_name);
                    }
                    string load_dir = System.Configuration.ConfigurationManager.AppSettings["FileUrl"];
                    string default_url = System.Configuration.ConfigurationManager.AppSettings["DefaultUrl"];
                    var load_url = load_dir + "logo.png";
                    //var toAddress = new MailAddress("harizadoz@gmail.com","คุณทดสอบ");
                    var subject_title = emailType.subject;
                    //var subject_title = "ทดสอบ ChangePassword";
                    var body = "";
                    string body_text = System.IO.File.ReadAllText(dir + emailType.path);
                var certificatePrint = false;
                    var cert_url = "";

                    if (!string.IsNullOrEmpty(email_type))
                    {
                        switch (email_type)
                        {
                            case "01":
                                {
                                    subject_title = emailType.subject;
                                    //templatePath = templatePath + emailType.path;
                                    var name_th = $"{student.firstname} {student.lastname}";
                                    var url = default_url+"course";
                                    var a = @"<a style=""display: flex;
        flex-direction: row;
        align-items: center;
        padding: 8px 16px;
        position: absolute;
        width: 86px;
        height: 18px;
        left: 16px;
        top: 286px;
        background-color: #00937B;
        border-radius: 4px;
text-decoration:none;font-family: Noto Sans Thai;font-style: normal;font-weight: bold;font-size: 12px;line-height: 18px;text-align: center;color: #FFFFFF;"" href=" + url+@" target=""_blank"">ดูรายการคอร์ส</a>";

                                    body_text = body_text.Replace("{name_th}", name_th);
                                    body_text = body_text.Replace("{urlLink}", a);

                                }
                                break;
                            case "02":
                                {
                                    subject_title = emailType.subject;
                                    //templatePath = templatePath + emailType.path;
                                    var student_learn = db.student.Where(x => x.student_id == student_id).FirstOrDefault();
                                    if (student_learn != null)
                                    {
                                        var url= default_url+"resetpassword?token="+token;
                                        token_ = token;
                                        var name_th = $"{student.firstname} {student.lastname}";
                                        var a = @"<a style="" display: flex;
        flex-direction: row;
        align-items: center;
        padding: 8px 16px;
        position: absolute;
        width: 86px;
        height: 18px;
        left: 16px;
        top: 256px;
        background-color: #00937B;
        border-radius: 4px;

  text-decoration:none;font-family: Noto Sans Thai;font-style: normal;font-weight: bold;font-size: 12px;line-height: 18px;text-align: center;color: #FFFFFF;"" href=" + url+@" target=""_blank"">  ตั้งรหัสผ่านใหม่</a>";


                                        body_text = body_text.Replace("{urlLink}", a);
                                        body_text = body_text.Replace("{name_th}", name_th);
                                    }
                                    else
                                    {
                                        throw new Exception("ไม่พบข้อมูลในระบบ");
                                    }
                                }
                                break;
                            case "03":
                                {
                                    //templatePath = templatePath + emailType.path;
                                    var f = (from a in db.student_course_info
                                             join b1 in db.course on new { a.course_id } equals new { course_id = b1.id } into b2
                                             from b in b2.DefaultIfEmpty()
                                             where a.student_id == student_id && a.course_id == course_id
                                             select new
                                             {

                                                 a.student_id,
                                                 a.course_id,
                                                 b.name
                                             }).ToList().Select(x => new
                                             {
                                                 x.student_id,
                                                 x.course_id,
                                                 course_name = x.name
                                             }).FirstOrDefault();
                                    subject_title = emailType.subject;
                                    if (f != null)
                                    {
                                        //var name_th = $"{student.firstname} {student.lastname}";
                                        var course_name = $"{f.course_name}";
                                        //var course_start = $"{course.learning_startdate}";
                                        //var course_end = $"{course?.learning_enddate.Value.ToString("dd/MMMM/YYYY H:i:s")}";
                                        var url = default_url+"course/" + f.course_id;
                                        var a = @"<a style=""display: flex;
        flex-direction: row;
        align-items: center;
        padding: 8px 16px;
        position: absolute;
        width: 86px;
        height: 18px;
        left: 16px;
        top: 276px;
        background-color: #00937B;
        border-radius: 4px;
text-decoration:none;font-family: Noto Sans Thai;font-style: normal;font-weight: bold;font-size: 12px;line-height: 18px;text-align: center;color: #FFFFFF;"" href=" + url + @" target=""_blank"">เข้าเรียนคอร์สนี้</a>";

                                        body_text = body_text.Replace("{course_name}", course_name);
                                        body_text = body_text.Replace("{urlLink}", a);
                                    }
                                    else
                                    {
                                        throw new Exception("ไม่พบข้อมูลในระบบ");

                                    }
                                }
                                break;
                            case "04":
                                {
                                    var cert_dir = System.Configuration.ConfigurationManager.AppSettings["Certificate"];

                                    certificatePrint = true;
                                    //templatePath = templatePath + emailType.path;
                                    var student_learn = db.student.Where(x => x.student_id == student_id).FirstOrDefault();
                                    var cert = db.certificate_info.Where(x => x.student_id == student_id && x.course_id == course_id).FirstOrDefault();
                                    var course = db.course.Where(x => x.id == course_id).FirstOrDefault();
                                    var s = (from a in db.certificate_info
                                             join b1 in db.course on new { a.course_id } equals new { course_id = b1.id } into b2
                                             from b in b2.DefaultIfEmpty()
                                             where a.student_id == student_id&&a.course_id==course_id
                                             select new
                                             {
                                                 b.name,
                                                 b.learning_startdate,
                                                 b.learning_enddate,
                                                 a.certificate_id,
                                                 a.certificate_dt,
                                                 a.path
                                             }).ToList().Select(x => new
                                             {
                                                 course_name = x.name,
                                                 x.learning_startdate,
                                                 x.learning_enddate,
                                                 x.certificate_id,
                                                 x.certificate_dt,
                                                 x.path
                                             }).FirstOrDefault();
                                    if (s != null)
                                    {
                                        subject_title = emailType.subject + " " + course.name;

                                        var name_th = $"{student.firstname} {student.lastname}";
                                        var course_name = $"{s.course_name}";
                                        var course_start = $"{s.learning_startdate}";
                                        var cert_dt = $"{s.certificate_dt.Value.ToString("dd MMMM YYYY")}";
                                        var cert_id = $"{s.certificate_id}";
                                        var course_end = s.learning_enddate == null ? "คอร์สนี้เปิดให้เรียนตลอด" : $"{s.learning_enddate.Value.ToString("dd/MMMM/YYYY H:i:s")}";
                                        certificatePath =cert_dir+ course_id + "\\"+ s.path;
                                        string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["CertUrl"];

                                        cert_url = virtual_dir + s.path + "&course_id=" + course.id;

                                        body_text = body_text.Replace("{name_th}", name_th);
                                        body_text = body_text.Replace("{course_name}", course_name);
                                        body_text = body_text.Replace("{cert_dt}", cert_dt);
                                        body_text = body_text.Replace("{cert_id}", cert_id);
                                        var item_no = db.certificate_print.Where(x => x.course_id == course_id && x.student_id == student_id && x.certificate_id == cert.certificate_id).Select(x => x.itemno).Max() ?? 0;
                                        var id = db.certificate_print.Select(x => x.id).Max() ?? 0;
                                        var c = new certificate_print
                                        {
                                            itemno = ++item_no,
                                            student_id = student_id,
                                            course_id = course_id,
                                            certificate_id = cert.certificate_id,
                                            certificate_dt = cert.certificate_dt,
                                            print_dt = DateTime.Now,
                                            print_by = auth.user_id
                                        };
                                        db.certificate_print.Add(c);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        throw new Exception("ไม่พบข้อมูลในระบบ");

                                    }
                                }
                                break;
                            case "05":
                                {
                                    subject_title = emailType.subject;
                                    //templatePath = templatePath + emailType.path;
                                    var student_learn = db.student.Where(x => x.student_id == student_id).FirstOrDefault();
                                    var f = (from a in db.student_course_info
                                             join b1 in db.course on new { a.course_id } equals new { course_id = b1.id } into b2
                                             from b in b2.DefaultIfEmpty()
                                             where a.student_id == student_id && a.course_id == course_id && a.learning_enddate!=null
                                             select new
                                             {

                                                 a.student_id,
                                                 a.course_id,
                                                 a.learning_enddate,
                                                 a.learning_startdate,
                                                 b.name
                                             }).ToList().Select(x => new
                                             {
                                                 x.student_id,
                                                 x.course_id,
                                                 x.learning_startdate,
                                                 x.learning_enddate,
                                                 course_name = x.name
                                             }).FirstOrDefault();
                                    //    var course = db.course.Where(x => x.id == course_id).FirstOrDefault();
                                    //var student_info = db.student_course_info.Where(x => x.course_id == course_id && x.student_id == student_id && x.learning_enddate!=null).FirstOrDefault();
                                    if (f != null)
                                    {
                                        var title_name = (student.title_name == null) ? "คุณ" : student.title_name;

                                        var name_th = $"{title_name}{student.firstname} {student.lastname}";
                                        var course_name = $"{f.course_name}";

                                        var course_start = $"{f.learning_startdate}";
                                        var course_end = f.learning_enddate == null ? "คอร์สนี้เปิดให้เรียนตลอด" : $"{f.learning_enddate.Value.ToString("dd MMMM yyyy")}";
                                        body_text = body_text.Replace("{name_th}", name_th);
                                        body_text = body_text.Replace("{course_name}", course_name);

                                        body_text = body_text.Replace("{course_end_dt}", course_end);
                                    }
                                    else
                                    {
                                        throw new Exception("ไม่พบข้อมูลในระบบ");

                                    }
                                }
                                break;
                            case "06":
                                {
                                    var title_name = (student?.title_name == null) ? "คุณ" : student?.title_name;

                                    var data = db.report_problem.Where(x => x.id == problem_id).FirstOrDefault();
                                    var desc = $"{data?.description}";
                                    //student_id = 0;
                                    //course_id = 0;
                                    if (data != null)
                                    {
                                        var name_th = $"{title_name}{data?.firstname} {data?.lastname}";

                                        toAddress = new MailAddress(data.email, name_th);
                                        body_text = body_text.Replace("{name_th}", name_th);
                                        body_text = body_text.Replace("{problem_id}", problem_id.ToString());
                                        body_text = body_text.Replace("{problem_desc}", desc);
                                    }
                                    else
                                    {
                                        throw new Exception("ไม่พบข้อมูลในระบบ");
                                    }
                                }
                                break;
                        }
                    }
                 
                    var smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new System.Net.NetworkCredential(fromAddress.Address, from.password.Trim());
                    smtp.EnableSsl = true;
                    //smtp.Timeout = 10000;

                    //var smtp = new SmtpClient
                    //{
                    //    Host = "smtp.gmail.com",
                    //    Port = 587,
                    //    UseDefaultCredentials = false,

                    //    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //    Credentials = new System.Net.NetworkCredential("dpimacademy@gmail.com", "dpimaca2020"),
                    //    EnableSsl = false,

                    //    //Timeout = 20000
                    //};
                    var message = new MailMessage();
                    message.From = fromAddress;
                    message.To.Add(toAddress);
                    message.Subject = subject_title;

                    var imgReplace = String.Format(
                                        @"<img src='" + load_url+ @"' style=""   position: absolute;
                                        width: 47px;
                                        height: 58px;
                                        left: 32px;
                                        top: 12px;
                                        ""/>", att.ContentId);
                    body_text = body_text.Replace("{image_path}", imgReplace);


                    if (certificatePrint)
                {
                    LinkedResource certline = new LinkedResource(certificatePath, MediaTypeNames.Image.Jpeg);
                    certline.ContentId = Guid.NewGuid().ToString();
                    Attachment certt = new Attachment(certificatePath);
                    certt.ContentDisposition.Inline = true;

                        var certReplace =  String.Format(
                                      @"<img src='" + cert_url + @"' style=""  position: absolute;
                                          width: 520px;
                                          height: 364px;
                                          left: 16px;
                                          top: 50px;
                                          ""/>", certt.ContentId);
                        body_text = body_text.Replace("{certificate_path}", certReplace);

                        ContentType mimeType = new System.Net.Mime.ContentType("text/html");

                        message.Attachments.Add(certt);

                    }
                    message.Body = body_text;

                    message.Attachments.Add(att);
                    message.IsBodyHtml = true;
                    try
                    {
                        smtp.Send(message);
                    } catch (Exception ex)
                    {
                        error = ex.Message;
                        ErrorList(ex);
                    }
                    var itemno = db.email_sending.Select(x => x.email_id).Max()??0;
                    var email_sending = new email_sending
                    {
                        email_id = ++itemno,
                        from_email = from.email,
                        subject = subject_title,
                        email_type = email_type,
                        send_by = auth.user_id,
                        send_dt = DateTime.Now,
                        student_id = student_id??0,
                        course_id =course_id??"",
                        problem_id=problem_id??0,
                        token=token_??""
                    };
                    db.email_sending.Add(email_sending);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    ErrorList(ex);
                }
            }
        }


        public dynamic ReadListFAQ(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.faq.Where(w => w.is_deleted == 0).ToList();
                    output = new
                    {
                        data
                    };
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                return output;
            }
        }
        public dynamic FAQManagement(faq faq,Authentication.Authentication auth,ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.faq.Where(x => x.id == faq.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.faq_type_id = faq.faq_type_id;
                        data.question = faq.question;
                        data.answer = faq.answer;
                        data.update_by = auth.user_id;
                        data.update_at = DateTime.Now;
                        db.SaveChanges();
                        output = data;
                    }
                    else
                    {
                        faq.is_deleted = 0;
                        faq.created_at = DateTime.Now;
                        faq.created_by = auth.user_id;
                        faq.update_by = auth.user_id;
                        faq.update_at = DateTime.Now;
                        db.faq.Add(faq);
                        db.SaveChanges();
                        output = faq;
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                return output;
            }
        }

        public dynamic DeleteFAQ(int id, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.faq.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                    if (data != null)
                    {
                        data.is_deleted = 1;
                        data.update_at = DateTime.Now;
                        data.update_by = auth.user_id;
                        db.SaveChanges();
                        output = data;
                    }
                    else
                    {
                        throw new Exception("Unable to find banner with this ID");
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return output;
        }

        public dynamic get_faq_by_type(string type, ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    int? type_new = type == null ? (int?)null : int.Parse(type);

                    var data = db.faq.Where(x => x.faq_type_id == type_new).ToList();
                    data.RemoveAll(x => x.is_deleted == 1);
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.InnerException?.Message ?? ex.Message;
            }

            return output;
        }

        public dynamic GetAllFAQType(ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.faq_type.ToList().Select(x => new
                    {
                        x.id,
                        x.name
                    }).ToList();

                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return output;
        }

        #region ReportProblem
        public dynamic ReadListReportProblem(int problem_type, int pageIndex, int pageSize, bool? is_done, string orderBy, bool desc, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_problem_of_use = db.problem_of_use;
                    var get_data = (problem_type == 0 ? db.report_problem.Where(w => w.is_deleted == 0).ToList() : db.report_problem.Where(w => w.is_deleted == 0 && w.problem_of_use_id == problem_type).ToList());
                    var count_edited = get_data.Where(w => w.status == 1).Count();
                    var count_progress = get_data.Where(w => w.status == 0).Count();
                    if (is_done == true)
                    {
                        get_data = get_data.Where(w => w.status == 1).ToList();
                    } else if (is_done == false)
                    {
                        get_data = get_data.Where(w => w.status == 0).ToList();
                    }

                    var data = get_data.Select(s => new {
                        id = s.id,
                        problem_of_use_id = s.problem_of_use_id,
                        problem_of_use_name = get_problem_of_use.Where(w => w.id == s.problem_of_use_id).Select(n => n.name).FirstOrDefault(),
                        firstname = s.firstname,
                        lastname = s.lastname,
                        description = s.description,
                        phone = s.phone,
                        email = s.email,
                        status = s.status,
                        created_at = s.created_at,
                        response = s.response,
                        admin_name = db.user.Where(w => w.id == s.update_by && w.role_id == 1).Select(ss => ss.email).FirstOrDefault()
                    }).OrderByDescending(o => o.created_at).ToList();

                    #region order by
                    switch (orderBy)
                    {
                        case "name":
                            data = desc ? data.OrderByDescending(o => o.firstname).ToList() : data.OrderBy(o => o.firstname).ToList();
                            break;
                        case "problem_of_use_name":
                            data = desc ? data.OrderByDescending(o => o.problem_of_use_name).ToList() : data.OrderBy(o => o.problem_of_use_name).ToList();
                            break;
                        case "email":
                            data = desc ? data.OrderByDescending(o => o.email).ToList() : data.OrderBy(o => o.email).ToList();
                            break;
                        case "phone":
                            data = desc ? data.OrderByDescending(o => o.phone).ToList() : data.OrderBy(o => o.phone).ToList();
                            break;
                        case "status":
                            data = desc ? data.OrderByDescending(o => o.status).ToList() : data.OrderBy(o => o.status).ToList();
                            break;
                        case "created_at":
                            data = desc ? data.OrderByDescending(o => o.created_at).ToList() : data.OrderBy(o => o.created_at).ToList();
                            break;
                        case "admin_name":
                            data = desc ? data.OrderByDescending(o => o.admin_name).ToList() : data.OrderBy(o => o.admin_name).ToList();
                            break;
                        default: break;
                    }
                    #endregion

                    var skip = (pageIndex - 1) * pageSize;
                    var count = data.Count();

                    var get_count_type = db.report_problem.Where(w => w.is_deleted == 0).ToList();
                    var count_type = new
                    {
                        total = get_count_type.Count(),
                        common = get_count_type.Where(w => w.problem_of_use_id == 1).Count(),
                        course = get_count_type.Where(w => w.problem_of_use_id == 2).Count(),
                        register = get_count_type.Where(w => w.problem_of_use_id == 3).Count(),
                        payment = get_count_type.Where(w => w.problem_of_use_id == 4).Count(),
                        other = get_count_type.Where(w => w.problem_of_use_id == 5).Count(),
                    };

                    output = new
                    {
                        data = data.Skip(skip).Take(pageSize).ToList(),
                        count = count,
                        count_edited = count_edited,
                        count_progress = count_progress,
                        count_type = count_type
                    };
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                return output;
            }
        }

        public dynamic DoneReportProblem(int id, string response, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.report_problem.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                    if(data == null)
                    {
                        errorMsg = "Id Invalid";
                        return false;
                    }

                    data.status = 1;
                    data.response = response;
                    data.update_by = auth.user_id;
                    data.update_at = DateTime.Now;
                    db.SaveChanges();
                    
                    SendingEmail(null, null, "06", null, auth, id, ref errorMsg);

                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                return output;
            }
        }

        public dynamic ReportProblemManage(report_problem rep, Authentication.Authentication auth ,ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {

                    var data = db.report_problem.Where(x => x.id == rep.id && x.email == rep.email).FirstOrDefault();
                    if (data != null)
                    {
                        data.status = rep.status;
                        rep.update_by = auth.user_id;
                        rep.update_at = DateTime.Now;
                        db.SaveChanges();
                    }
                    var itemno = db.report_problem.Select(x => x.id).Max() ?? 0;
                    rep.id = ++itemno;
                    rep.created_by = auth.user_id;
                    rep.created_at = DateTime.Now;
                    db.report_problem.Add(rep);
                    db.SaveChanges();
                    output = new { data };

                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
                return output;
            }
        }

        public dynamic get_problem_of_use (ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.problem_of_use.ToList();
                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.InnerException?.Message ?? ex.Message;
            }

            return output;
        }

        public dynamic add_report_problem (model_report_problem body, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = new report_problem()
                    {
                        problem_of_use_id = body.problem_of_use_id,
                        firstname = body.firstname,
                        lastname = body.lastname,
                        description = body.description,
                        phone = body.phone,
                        email = body.email,
                        response = "",
                        status = 0,
                        is_deleted = 0,
                        created_by = auth.user_id,
                        created_at = now,
                        update_by = auth.user_id,
                        update_at = now
                    };

                    db.report_problem.Add(data);
                    db.SaveChanges();
                    output = data;
                }
            }
            catch (Exception ex)
            {
                error = ex.InnerException?.Message ?? ex.Message;
            }

            return output;
        }

        #endregion

        public dynamic VisitUpdate(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    //string ipAdd = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString() ?? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();

                    var data = db.website_info.FirstOrDefault();
                    var visitor = 0;
                    if (data == null)
                    {
                        website_info website_info = new website_info();
                        website_info.visit_count = 1;
                        db.website_info.Add(website_info);
                        db.SaveChanges();
                        visitor = 1;
                    }
                    else
                    {
                        data.visit_count = (data.visit_count + 1);
                        db.SaveChanges();
                        visitor = data.visit_count;
                    }

                    var course = db.course.Where(w => w.is_deleted == 0).Count();
                    var student = db.student.Where(w => w.is_deleted == 0).Count();
                    var instructor = db.instructor.Where(w => w.is_deleted == 0).Count();
                    output = new
                    {
                        visitor,
                        course,
                        student,
                        instructor
                    };
                }catch(Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
   public dynamic getVideo()
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                var vir_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
                List<object> data1 = new List<object>();
                var data = db.video_status.Where(x => x.filename.Contains("original")).Select(x => new
                {
                    filename = x.filename.Substring(0, x.filename.IndexOf("_")) + "_original.mp4",
                    cover = x.filename.Substring(0, x.filename.IndexOf("_")) + "_cover.jpg",
                    folder=x.filename.Substring(0,x.filename.IndexOf("_"))
                }
                    ).ToList().OrderByDescending(x=>x.filename).GroupBy(g => (g.filename), (k, v) => new
                    {
                        filename = v.Select(x => x.filename).FirstOrDefault(),
                        cover = v.Select(x => x.cover).FirstOrDefault(),
                        folder=v.Select(x=>x.folder).FirstOrDefault()
                    }).ToList() ;
                foreach (var d in data)
                {
                    var folder = d.folder;

                    if (File.Exists(vir_dir + d.folder.Substring(0, 8) + "\\" + d.folder + "\\" + d.cover))
                    {
                        var fileInfo = new FileInfo(vir_dir + d.folder.Substring(0, 8) + "\\" + d.folder + "\\" + d.cover);
                        if (fileInfo.Length > 0)
                        {
                            data1.Add(d);
                        }
                    }
                }
                //var data1 = data.Where(x => File.Exists(vir_dir + x.filename.Substring(0, 8) + "\\" + x.folder + "\\" + x.cover)).ToList();

                output = new
                {
                    data=data1,
                    count=data.Count()
                };
            }
            return output;
        }
        public dynamic Loadvide(string filename)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                var p_480 = filename.Substring(0, filename.IndexOf("_")) + "_480.mp4";
                var p_720 = filename.Substring(0, filename.IndexOf("_")) + "_720.mp4";
                var p_1080 = filename.Substring(0, filename.IndexOf("_")) + "_1080.mp4";

                var p480 = db.video_status.Where(x => x.filename == p_480).Select(x=>x.filename).FirstOrDefault();
                var p720 = db.video_status.Where(x => x.filename == p_720).Select(x => x.filename).FirstOrDefault();
                var p1080 = db.video_status.Where(x => x.filename == p_1080).Select(x => x.filename).FirstOrDefault();

                output = new
                {
                
                      original=filename,
                      p_480 = p480,
                      p_720 = p720,
                      p_1080,
                      cover=filename.Substring(0,filename.IndexOf("_"))+"_cover.jpg"
                  
                };
            }
            return output;
        }
        public void testSendEmail(ref string errorMsg)
        {
            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                try
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new System.Net.NetworkCredential("dpimacademy@gmail.com", "dpimaca2020");
                    smtp.EnableSsl = true;
                    smtp.Timeout = 30000;

                    //var smtp = new SmtpClient
                    //{
                    //    Host = "smtp.gmail.com",
                    //    Port = 587,
                    //    UseDefaultCredentials = false,

                    //    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //    Credentials = new System.Net.NetworkCredential("dpimacademy@gmail.com", "dpimaca2020"),
                    //    EnableSsl = false,

                    //    //Timeout = 20000
                    //};
                    var fromAddress = new MailAddress("dpimacademy@gmail.com", "admin@dpim.go.th");
                    var toAddress = new MailAddress("harizadoz@gmail.com", "นายพงษ์สันต์ เหล่ามงคลนิมิต");

                    var message = new MailMessage();
                    message.From = fromAddress;
                    message.To.Add(toAddress);
                    message.Subject = "ทดสอบส่ง SMTP";
                    message.Body = "TEST SEND SMTP";
                    smtp.Send(message);
                }catch(Exception ex)
                {
                  errorMsg = ex.Message;
                    ErrorList(ex);
                }
            }
        }
        public void CheckCourseFile(int? course_id, ref string errogMsg)
        {
            using (var db = new DataContext())
            {
                try
                {

                    //var data = (from a in db.course
                    //            join b1 in db.course_lesson on new { a.parent_id } equals new { parent_id = b1.course_id } into b2
                    //            from b in b2.DefaultIfEmpty()
                    //            join c1 in db.course_lesson_exercise on new { a.parent_id } equals new { parent_id = c1.course_id } into c2
                    //            from c in c2.DefaultIfEmpty()
                    //            join d1 in db.course_exam on new { a.parent_id } equals new { parent_id = d1.course_id } into d2
                    //            from d in d2.DefaultIfEmpty()
                    //            where a.id == course_id
                    //            select new
                    //            {
                    //                a,
                    //                b,
                    //                c,
                    //                d
                    //            }).ToList();
                    //if (data.Count > 1)
                    //{
                    //    throw new Exception("ไม่สามารถลบไฟล์นี้ออกจาก Server ได้ เนื่องจากมี Course อื่นใช้งานอยู่");
                    //}

                }
                catch (Exception ex)
                {
                    errogMsg = ex.Message;
                    ErrorList(ex);
                }
            }
        }

        public dynamic get_special_days(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.special_days.Where(w => w.is_deleted == 0).FirstOrDefault();
                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
                }
                return output;
            }
        }

        public dynamic manage_special_days(string cover, DateTime? start_date, DateTime? end_date, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.special_days.Where(w => w.is_deleted == 0).FirstOrDefault();
                    if(data == null)
                    {
                        special_days special_days = new special_days();
                        special_days.cover = cover;
                        special_days.start_date = start_date;
                        special_days.end_date = end_date;
                        special_days.is_deleted = 0;
                        special_days.created_by = auth.user_id;
                        special_days.created_at = DateTime.Now;
                        special_days.update_by = auth.user_id;
                        special_days.update_at = DateTime.Now;

                        db.special_days.Add(special_days);
                        db.SaveChanges();
                        output = special_days;
                    }
                    else
                    {
                        data.cover = cover;
                        data.start_date = start_date;
                        data.end_date = end_date;
                        data.update_by = auth.user_id;
                        data.update_at = DateTime.Now;
                        db.SaveChanges();
                        output = data;
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
                }
                return output;
            }
        }

        //public void LicenseCertificate(certificate m,Authentication.Authentication auth, ref string errorMsg)
        //{
        //    using (var db = new DataContext())
        //    {
        //        try {
        //            var cert = db.certificate.Where(x => x.is_delete == 0).ToList();
        //            foreach(var x in cert)
        //            {
        //                x.is_delete = 1;
                        
        //            }
        //            db.SaveChanges();
        //            var itemno = db.certificate.Select(x => x.id).Max() ?? 0;
        //            m.id = ++itemno;
        //            m.create_by = auth.user_id;
        //            m.create_dt = DateTime.Now;
        //            m.is_delete = 0;
        //            db.certificate.Add(m);
        //            db.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            errorMsg = ex.Message;
        //            ErrorList(ex);
        //        }
        //    }
        //}

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