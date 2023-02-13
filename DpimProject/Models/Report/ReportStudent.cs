using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using DpimProject.Security;
using System.Data;
using System.Collections.Specialized;
using Newtonsoft.Json;
using DpimProject.Models.DataTools;
using System.IO;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.Report
{
    public class ReportStudent
    {
        private string[] pic_mapping = new string[] { "jpg", "jpeg", "png", "gif", "bmp" };
        private string[] doc_mapping = new string[] { "doc", "docx", "xls", "xlsx", "pdf", "csv", "pptx", "ppt" };
        private string[] video_mapping = new string[] { "mp4", "mpeg", "3gp" };
        private string[] audio_mapping = new string[] { "mp3", "wma" };
        private string virtual_dir = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UploadPath"]) ? @"C:\fileuploads\" : System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
        /*  private string virtual_dir = @"C:\DpimProject\DpimApi\DpimProject\FileUpload";  */    // DEBUG ONLY

        public dynamic get_overview_website(ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_auth = db.app_auth;
                    var total_desktop = get_auth.Where(w => w.device.ToString().ToLower().IndexOf("windows") >= 0 || 
                                                            w.device.ToString().ToLower().IndexOf("mac") >= 0 && 
                                                            w.device.ToString().ToLower().IndexOf("iphone") == -1 && 
                                                            w.device.ToString().ToLower().IndexOf("ipad") == -1 && 
                                                            w.device.ToString().ToLower().IndexOf("android") == -1).Count();
                    var total_mobile = (get_auth.Count() - total_desktop);
                    var data = new
                    {
                        total_student = db.student.Where(w => w.is_deleted == 0).Count(),
                        total_visit_website = db.website_info.Select(s => s.visit_count).FirstOrDefault(),
                        total_course = db.course.Where(w => w.is_deleted == 0).Count(),
                        total_instructor = db.instructor.Where(w => w.is_deleted == 0).Count(),
                        total_desktop = total_desktop,
                        total_mobile = total_mobile,
                        total_time_course = (from c in db.course
                                             where c.is_deleted == 0
                                             select new {
                                                 time = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == c.id).Sum(s => s.lesson_time)
                                             }).Sum(s => s.time)
                    };

                    output = data;
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic save_account_type_student(int student_id, int account_type, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.student.Where(w => w.id == student_id && w.is_deleted == 0).FirstOrDefault();
                    if(data == null)
                    {
                        error_message = "Id Invalid";
                        return false;
                    }

                    data.student_account_type_id = account_type;
                    //data.is_internal_student = is_internal_student ? 1 : 0;
                    db.SaveChanges();

                    output = data;
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic get_desc_student(int filter, string course_id, ref string error_message)
        {
            dynamic output = null;
            dynamic data = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_info = db.student_course_info.Where(w => w.course_id == course_id);

                    if (filter == 1)
                    {
                        data = (from p in db.provinces
                                select new
                                {
                                    name = p.province_name,
                                    count = (from s in db.student
                                             join s_info in get_info on s.id equals s_info.student_id
                                             where s.province_id == p.id && s.is_deleted == 0
                                             select new
                                             {
                                                 s.id
                                             }).Count(),
                                }).ToList()
                                .Select(s =>
                                {
                                    return new
                                    {
                                        name = s.name,
                                        count = (get_info.Count() == 0 ? 0 : s.count),
                                        percent = (get_info.Count() == 0 ? 0 : (((decimal)s.count / get_info.Count()) * 100)).ToString("0.##")
                                    };
                                }).ToList();
                    }
                    else if (filter == 2)
                    {
                        data = (from b in db.business_type
                                select new
                                {
                                    name = b.name,
                                    count = (from s in db.student
                                             join s_info in get_info on s.id equals s_info.student_id
                                             where s.business_type_id == b.id && s.is_deleted == 0
                                             select new
                                             {
                                                 s.id
                                             }).Count(),
                                }).ToList()
                                .Select(s =>
                                {
                                    return new
                                    {
                                        name = s.name,
                                        count = (get_info.Count() == 0 ? 0 : s.count),
                                        percent = (get_info.Count() == 0 ? 0 : (((decimal)s.count / get_info.Count()) * 100)).ToString("0.##")
                                    };
                                }).ToList();

                    }
                    else if (filter == 3)
                    {
                        data = (from e in db.educational
                                select new
                                {
                                    name = e.name,
                                    count = (from s in db.student
                                             join s_info in get_info on s.id equals s_info.student_id
                                             where s.educational_id == e.id && s.is_deleted == 0
                                             select new
                                             {
                                                 s.id
                                             }).Count(),
                                }).ToList()
                                .Select(s =>
                                {
                                    return new
                                    {
                                        name = s.name,
                                        count = (get_info.Count() == 0 ? 0 : s.count),
                                        percent = (get_info.Count() == 0 ? 0 : (((decimal)s.count / get_info.Count()) * 100)).ToString("0.##")
                                    };
                                }).ToList();

                    }
                    else if (filter == 4)
                    {
                        data = (from e in db.career
                                select new
                                {
                                    name = e.name,
                                    count = (from s in db.student
                                             join s_info in get_info on s.id equals s_info.student_id
                                             where s.career_id == e.id && s.is_deleted == 0
                                             select new
                                             {
                                                 s.id
                                             }).Count(),
                                }).ToList()
                                .Select(s =>
                                {
                                    return new
                                    {
                                        name = s.name,
                                        count = (get_info.Count() == 0 ? 0 : s.count),
                                        percent = (get_info.Count() == 0 ? 0 : (((decimal)s.count / get_info.Count()) * 100)).ToString("0.##")
                                    };
                                }).ToList();
                    }
                    else if (filter == 5)
                    {
                        data = (from k in db.know_channel
                                select new
                                {
                                    name = k.name,
                                    count = db.mapping_student_know_channel.Where(w => w.know_channel_id == k.id).Count(),
                                }).ToList()
                                .Select(s =>
                                {
                                    return new
                                    {
                                        name = s.name,
                                        count = (get_info.Count() == 0 ? 0 : s.count),
                                        percent = (get_info.Count() == 0 ? 0 : (((decimal)s.count / get_info.Count()) * 100)).ToString("0.##")
                                    };
                                }).ToList();
                    }

                    output = data;
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic get_student_by_id(int? student_id, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from s in db.student
                                where s.id == student_id && s.is_deleted == 0
                                select new
                                {
                                    id = s.id,
                                    cover = s.profile_image == null || s.profile_image == "" ? "" : s.profile_image,
                                    firstname = s.firstname,
                                    lastname = s.lastname,
                                    gender = s.gender,
                                    birthday = s.birthday,
                                    id_card = s.id_card,
                                    email = s.email,
                                    phone = s.phone,
                                    is_internal_student = s.is_internal_student,

                                    business_name = s.business_name,
                                    business_type = db.business_type.Where(w => w.id == s.business_type_id).Select(n => n.name).FirstOrDefault(),
                                    business_province = db.provinces.Where(w => w.id == s.business_province_id).Select(p => p.province_name).FirstOrDefault(),
                                    business_register = s.business_register,
                                    business_no = s.business_no,

                                    front_id_card = s.front_id_card == null || s.front_id_card == "" ? "" : s.front_id_card,
                                    back_id_card = s.back_id_card == null || s.back_id_card == "" ? "" : s.back_id_card,
                                    straight_face_image = s.straight_face_image == null || s.straight_face_image == "" ? "" : s.straight_face_image,
                                    business_attachment = s.business_attachment == null || s.business_attachment == "" ? "" : s.business_attachment,
                                    account_type = s.student_account_type_id,

                                    course = (from s_info in db.student_course_info
                                              join c in db.course on s_info.course_id equals c.id
                                              where s_info.student_id == s.id 
                                              select new
                                              {
                                                  id = c.id,
                                                  created_by = s_info.created_by,
                                                  name = c.name,
                                                  sub_name = c.sub_name,
                                                  batch = c.batch,
                                                  cert_count = (from cp in db.certificate_print
                                                                join ci in db.certificate_info on cp.certificate_id equals ci.certificate_id
                                                                where ci.course_id == c.id && ci.cert_status == "Y" && ci.student_id == student_id && cp.student_id == student_id && cp.course_id == c.id && ci.course_id == c.id
                                                                select new {
                                                                    cp.id
                                                                }).Count(),
                                                  pre_test = db.course_exam_logging.Where(w => w.student_id == s.id && 
                                                                                               w.course_id == c.id && 
                                                                                               w.is_pretest == 1 &&
                                                                                               w.is_deleted == 0
                                                                                               && (db.course_exam.Where(wx => wx.is_deleted == 0 &&
                                                                                                                                  wx.course_id == c.id &&
                                                                                                                                  wx.id == w.course_exam_id &&
                                                                                                                                  wx.answer == w.course_exam_answer_id).Count() > 0)).Count(),

                                                  after_class = db.course_exam_logging.Where(w => w.student_id == s.id && 
                                                                                                  w.course_id == c.id && 
                                                                                                  w.is_pretest == 0 &&
                                                                                                  w.is_deleted == 0
                                                                                                  && (db.course_exam.Where(wx => wx.is_deleted == 0 &&
                                                                                                                                  wx.course_id == c.id &&
                                                                                                                                  wx.id == w.course_exam_id &&
                                                                                                                                  wx.answer == w.course_exam_answer_id).Count() > 0)).Count(),
                                                  total_exam = db.course_exam.Where(w => w.course_id == c.id && w.is_deleted == 0).Count()
                                              }).OrderByDescending(o => o.created_by).ToList()
                                }).FirstOrDefault();
                    
                    output = data;
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic get_overview_student(string course_id, int account_type, string search, int pageIndex, int pageSize, DateTime? start_date, DateTime? end_date, string orderBy, bool desc, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    if(start_date != null && end_date != null && start_date == end_date)
                    {
                        end_date = end_date.Value.AddDays(1);
                    }

                    var get_account_type = db.student_account_type;
                    var get_info = db.student_course_info.Where(w => w.is_deleted == 0).ToList();

                    var get_student = (start_date != null && end_date != null) ? db.student.Where(w => w.created_at >= start_date && 
                                                                                                       w.created_at <= end_date && 
                                                                                                       w.is_deleted == 0 
                                                                                                  ).ToList() 
                                                                                                  : 
                                                                                                  db.student.Where(w => w.is_deleted == 0).ToList();

                    if (course_id != "" && course_id != null)
                    {
                        get_info = get_info.Where(w => w.course_id == course_id).ToList();
                        get_student = get_student.Where(w => get_info.Where(ww => ww.student_id == w.id).Count() > 0).ToList();
                    }
                    
                    #region Filter - Search
                    //search by firstname, lastname, email
                    if (search != null && search != "")
                    {
                        get_student = get_student.Where(w => w.firstname.Contains(search) || w.lastname.Contains(search) || w.email.Contains(search)).ToList();
                    }

                    //filter account type
                    if (account_type != 0)
                    {
                        get_student = get_student.Where(w => w.student_account_type_id == account_type).ToList();
                    }
                    #endregion

                    var count = get_student.Count();

                    var data = (from s in get_student
                                select new
                                {
                                    id = s.id,
                                    name = s.firstname + " " + s.lastname,
                                    account_type = get_account_type.Where(w => w.id == s.student_account_type_id).Select(n => n.name).FirstOrDefault(),
                                    address = s.address,
                                    sub_district = db.sub_districts.Where(w => w.id == s.sub_district_id).Select(ss => ss.sub_districts_name).FirstOrDefault(),
                                    district = db.districts.Where(w => w.id == s.district_id).Select(ss => ss.districts_name).FirstOrDefault(),
                                    province = db.provinces.Where(w => w.id == s.province_id).Select(ss => ss.province_name).FirstOrDefault(),
                                    zipcode = (s.zipcode == null ? "" : s.zipcode),
                                    email = s.email,
                                    phone = (s.phone == null ? "" : s.phone),
                                    business_type = db.business_type.Where(w => w.id == s.business_type_id).Select(ss => ss.name).FirstOrDefault(),
                                    count_course = db.student_course_info.Where(w => w.is_deleted == 0 && w.student_id == s.id).Count(),
                                    create_at = s.created_at
                                }).OrderByDescending(o => o.create_at).ToList();

                    #region order by
                    switch (orderBy)
                    {
                        case "name":
                            data = desc ? data.OrderByDescending(o => o.name).ToList() : data.OrderBy(o => o.name).ToList();
                            break;
                        case "email":
                            data = desc ? data.OrderByDescending(o => o.email).ToList() : data.OrderBy(o => o.email).ToList();
                            break;
                        case "count_course":
                            data = desc ? data.OrderByDescending(o => o.count_course).ToList() : data.OrderBy(o => o.count_course).ToList();
                            break;
                        case "create_at":
                            data = desc ? data.OrderByDescending(o => o.create_at).ToList() : data.OrderBy(o => o.create_at).ToList();
                            break;
                        default: break;
                    }
                    #endregion

                    var skip = (pageIndex - 1) * pageSize;
                    output = new
                    {
                        data = data.Skip(skip).Take(pageSize).ToList(),
                        count = count
                    };

                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        //รายงานรายชื่อนักเรียน
        public dynamic ReportStudentCourse(string search_text, int take, int skip, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    System.Globalization.CultureInfo th_format = new System.Globalization.CultureInfo("th-Th");
                    var data = (from a in db.student
                                join b1 in db.student_account_type on new { a.student_account_type_id } equals new { student_account_type_id = b1.id } into b2
                                from b in b2.DefaultIfEmpty()

                                select new
                                {
                                    a.course_id,
                                    a.firstname,
                                    a.firstname_en,
                                    a.lastname,
                                    a.lastname_en,

                                    a.email,
                                    a.student_account_type_id,
                                    student_type_name = b.name,
                                    course_row_total = db.student_course_info.Where(x => x.student_id == a.user_id).ToList().Count(),
                                    a.phone,
                                    a.position,
                                    add_date = a.update_at,
                                    a.province_id,
                                }).ToList().Select(x => new
                                {
                                    x.course_id,
                                    x.firstname,
                                    x.firstname_en,
                                    x.lastname,
                                    x.lastname_en,
                                    name_th = $"{x.firstname} {x.lastname}",
                                    name_en = $"{x.firstname_en} {x.lastname_en}",
                                    x.email,
                                    x.student_account_type_id,
                                    x.student_type_name,
                                    x.course_row_total,
                                    x.phone,
                                    x.position,
                                    add_date = x.add_date?.ToString("dd MMMM YYYY", th_format),
                                    x.province_id,
                                });
                    if (search_text != null)
                    {
                        data = data.Where(x => x.name_th == search_text || x.name_en == search_text).ToList();
                    }
                    data = data.ToList().Skip(skip).Take(take);
                    output = new
                    {
                        data,
                        page_total = data.Count(),

                    };
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        //รายงานผลแบบประเมิน

        public dynamic EvaluationReport(string course_id, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_eva_result = db.course_evaluation_result.Where(w => w.course_id == course_id && w.is_deleted == 0);
                    var data = (from c_eva in db.course_evaluation
                                where c_eva.course_id == course_id && c_eva.is_deleted == 0
                                select new
                                {
                                    id = c_eva.id,
                                    score = (from ss in get_eva_result
                                                      where ss.course_evaluation_id == c_eva.id
                                                      select new
                                                      {
                                                          a = db.course_evaluation_choices.Where(w => w.course_evaluation_id == c_eva.id && w.id == ss.course_evaluation_choices_id).Select(s => s.score).FirstOrDefault()
                                                      }).Sum(ssc => ssc.a),
                                    name = c_eva.question,
                                    order= c_eva.order,
                                    count_student = get_eva_result.Where(w => w.course_evaluation_id == c_eva.id).Count()
                                }).OrderBy(o => o.order).ToList()
                                .Select(x =>
                                {
                                    return new
                                    {
                                        id = x.id,
                                        order = x.order,
                                        name = x.name,
                                        score = x.score > 0 && x.score != null ? ((decimal)x.score / x.count_student) : 0
                                    };
                                });

                    output = data.ToList();
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic StudentReport(string search_text, string course_id, int take, int skip, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var i = 1;
                    System.Globalization.CultureInfo th_format = new System.Globalization.CultureInfo("th-Th");
                    var data = (from a in db.student
                                join b1 in db.student_course_info on new { a.student_id } equals new { b1.student_id } into b2
                                from b in b2.DefaultIfEmpty()
                                join c1 in db.course on new { b.course_id } equals new { course_id = c1.id } into c2
                                from c in c2.DefaultIfEmpty()
                                join e1 in db.provinces on new { a.province_id } equals new { province_id = e1.id } into e2
                                from e in e2.DefaultIfEmpty()
                                join d1 in db.course_category on new { id = c.course_category_id } equals new { d1.id } into d2
                                from d in d2.DefaultIfEmpty()
                                where b.course_id == course_id
                                select new
                                {
                                    a.province_id,
                                    e.province_name,
                                    e.province_code,
                                    e.province_name_eng,
                                    a.user_id,
                                    b.course_id,
                                    c.name,
                                    c.course_category_id,
                                    course_category_name = d.name
                                }).GroupBy(g => g.province_id, (k, v) => new
                                {
                                    provice_id = v.Select(x => x.province_id).FirstOrDefault(),
                                    course_id = v.Select(x => x.course_id).FirstOrDefault(),
                                    course_name = v.Select(x => x.course_id).FirstOrDefault(),
                                    province_code = v.Select(x => x.province_code).FirstOrDefault(),
                                    province_name = v.Select(x => x.province_name).FirstOrDefault(),
                                    province_name_eng = v.Select(x => x.province_name_eng).FirstOrDefault(),
                                    course_category_id = v.Select(x => x.course_category_id).FirstOrDefault(),
                                    course_category_name = v.Select(x => x.course_category_name).FirstOrDefault(),
                                    count_provice = v.Count()

                                }).ToList().Select(x =>
                                {
                                    decimal? province_result = db.student.Where(a => a.province_id == x.provice_id).ToList().Count();
                                    decimal? total = db.student.Count();
                                    return new
                                    {
                                        x.provice_id,
                                        x.province_code,
                                        x.province_name,
                                        x.province_name_eng,
                                        x.course_id,
                                        x.course_name,
                                        x.count_provice,
                                        x.course_category_id,
                                        x.course_category_name,
                                        province_percent = province_result - x.count_provice / total
                                    };





                                });


                    if (search_text != null)
                    {
                        data = data.Where(x => x.course_category_name == search_text).ToList();
                    }

                    data = data.ToList().Skip(skip).Take(take);

                    output = new
                    {
                        data,
                        page_total = data.Count(),

                    };
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic StudentExamPressReport(string id, DateTime? start_date, DateTime? end_date, ref string error_message)
        {
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["CertUrl"];
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    if (start_date != null && end_date != null && start_date == end_date)
                    {
                        end_date = end_date.Value.AddDays(1);
                    }

                    var get_exam = db.course_exam.Where(w => w.is_deleted == 0 && w.course_id == id);
                    var get_percent = db.course.Where(w => w.id == id).Select(s => s.passed_percent).FirstOrDefault();
                    var passed_percent = ((double)get_percent / 100);
                    var exam_pass = Math.Floor(get_exam.Count() * passed_percent);
                    var data = (from s_info in db.student_course_info
                                join s in db.student.Where(w => w.is_deleted == 0) on s_info.student_id equals s.id
                                where s_info.course_id == id
                                select new
                                {
                                    id = s_info.student_id,
                                    name = s.firstname + " " + s.lastname,
                                    pass_at = db.course_exam_logging.Where(w => w.is_deleted == 0 && 
                                                                                w.is_pretest == 0 && 
                                                                                w.course_id == id &&
                                                                                (start_date != null && end_date != null ? w.created_dt >= start_date && w.created_dt <= w.created_dt : w.id != 0)
                                                                          ).Select(d => d.created_dt)
                                                                          .OrderByDescending(o => o)
                                                                          .FirstOrDefault(),
                                    score = db.course_exam_logging.Where(w => w.student_id == s.id &&
                                                                              w.is_deleted == 0 && 
                                                                              w.is_pretest == 0 && 
                                                                              w.course_id == id &&
                                                                              (start_date != null && end_date != null ? w.created_dt >= start_date && w.created_dt <= w.created_dt : w.id != 0)
                                                                              && (get_exam.Where(wx => wx.id == w.course_exam_id && wx.answer == w.course_exam_answer_id).Count() > 0)).Count(),
                                                                         //&& w.score == 1 
                                                                         //).Select(ss => get_exam.Where(w => w.id == ss.course_exam_id && w.answer == ss.course_exam_answer_id).Count() > 0).Count(),
                                    exam_total = get_exam.Count(),
                                    cert_info = db.certificate_info.Where(w => w.student_id == s_info.student_id && w.course_id == id && w.cert_status == "Y").Select(ss => new { path = ss.path, course_id = ss.course_id }).FirstOrDefault()
                                }).Where(w => w.score >= exam_pass && exam_pass > 0).OrderByDescending(o => o.pass_at).ToList();

                    var real_data = new List<object>();
                    foreach(var mock in data)
                    {
                        string path = mock.cert_info == null ? "" : virtual_dir + mock.cert_info.path + "&course_id=" + mock.cert_info.course_id;
                        real_data.Add(new
                        {
                            mock.id,
                            mock.name,
                            mock.pass_at,
                            mock.score,
                            mock.exam_total,
                            cert_link = path
                        });
                    }

                    output = real_data;
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic get_certificate_print_report_by_category(string category_id, DateTime? start_date, DateTime? end_date, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    if (start_date != null && end_date != null && start_date == end_date)
                    {
                        end_date = end_date.Value.AddDays(1);
                    }

                    var get_certificate_print = (start_date != null && end_date != null) ? db.certificate_print.Where(w => w.certificate_dt >= start_date && w.certificate_dt <= end_date) : db.certificate_print;
                    var data = (from c in db.course
                                where c.course_category_id == category_id && c.is_deleted == 0
                                select new
                                {
                                    id = c.id,
                                    name = c.name,
                                    sub_name = c.sub_name,
                                    batch = c.batch,
                                    cert_print = get_certificate_print.Where(w => w.course_id == c.id).Count(),
                                    created_at = c.created_dt
                                }).OrderByDescending(o => o.created_at).ToList();

                    output = data;
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic get_certificate_print_report_by_course(string course_id, DateTime? start_date, DateTime? end_date, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_certificate_print = (start_date != null && end_date != null) ? db.certificate_print.Where(w => w.certificate_dt >= start_date && w.certificate_dt <= end_date) : db.certificate_print;
                    var get_course = db.course.Where(w => w.id == course_id && w.is_deleted == 0).Select(s => new { name = s.name, batch = s.batch }).FirstOrDefault();
                    var data = (from s_info in db.student_course_info
                                join s in db.student on s_info.student_id equals s.id
                                where s.is_deleted == 0 && s_info.course_id == course_id
                                select new
                                {
                                    course_name = get_course.name,
                                    batch = get_course.batch,
                                    id = s.id,
                                    name = s.firstname + " " + s.lastname,
                                    email = s.email,
                                    phone = (s.phone == null ? "" : s.phone),
                                    address = (s.address == null ? "" : s.address),
                                    sub_district = db.sub_districts.Where(w => w.id == s.sub_district_id).Select(s => s.sub_districts_name).FirstOrDefault(),
                                    district = db.districts.Where(w => w.id == s.district_id).Select(s => s.districts_name).FirstOrDefault(),
                                    province = db.provinces.Where(w => w.id == s.province_id).Select(s => s.province_name).FirstOrDefault(),
                                    zipcode = (s.zipcode == null ? "" : s.address),
                                    business_type = db.business_type.Where(w => w.id == s.business_type_id).Select(s => s.name).FirstOrDefault(),
                                    cert_print = get_certificate_print.Where(w => w.course_id == course_id && w.student_id == s.id).Count(),
                                    created_at = s.created_at
                                }).OrderByDescending(o => o.cert_print).ToList();

                    output = data;
                }
                catch (Exception ex)
                {
                    error_message= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }

        public dynamic DriveSpecReport(ref string errorMsg)
        {
            double freeSpec = 0;
            double useSpec = 0;
            double totalSpec = 0;
            double freeSpec_per = 0;
            double totalSpec_per = 0;
            double useSpec_per = 0;
            double cal_mb = 1048576;
            double cal_kb = 1024;
            double cal_gb = 1073741824;
            dynamic output = null;
            var totalSpec_txt = "";
            double total = 0;
            var freeSpec_txt = "";
            var useSpec_txt = "";
            var total_per_txt = "";
            var free_per_txt = "";
            var use_per_txt = "";
            try
            {
                dynamic driveSpec = null;
            //DriveInfo driveInfo = new DriveInfo("D");
            DriveInfo driveInfo = new DriveInfo("C");
            if (driveInfo.IsReady)
            {
                //total = driveInfo.TotalSize;
                freeSpec_per = (driveInfo.AvailableFreeSpace / (double)driveInfo.TotalSize) * 100;
                totalSpec_per = ((float)driveInfo.TotalSize / 100) * 100;
                useSpec_per = (((float)driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / (float)driveInfo.TotalSize) * 100;
                totalSpec = (driveInfo.TotalSize / cal_gb);
                freeSpec = driveInfo.AvailableFreeSpace / cal_gb;
                free_per_txt = $"{freeSpec_per.ToString("0.##")} %";
                use_per_txt = $"{useSpec_per.ToString("0.##")} %";
                totalSpec_per = freeSpec_per + useSpec_per;
                total_per_txt = $"{totalSpec_per.ToString("0.##")} %";
                totalSpec_txt = $"{totalSpec.ToString("0.##")} GB";
                freeSpec_txt = $"{freeSpec.ToString("0.##")} GB";
                if (Math.Round(totalSpec) < 0)
                {
                    totalSpec = driveInfo.TotalSize / cal_mb;
                    totalSpec_txt = $"{totalSpec.ToString("0.##")} MB";

                }
                if (Math.Round(freeSpec) < 0)
                {
                    freeSpec = driveInfo.AvailableFreeSpace / cal_mb;
                    freeSpec_txt = $"{freeSpec.ToString("0.##")} MB";

                }
                useSpec = (driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / cal_gb;
                useSpec_txt = $"{useSpec.ToString("0.##")} GB";

                if (Math.Round(useSpec) < 0)
                {
                    useSpec = (driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / cal_mb;
                    useSpec_txt = $"{useSpec.ToString("0.##")} MB";

                }
            }


            var driveData = new
            {
                //freeSpec,
                //useSpec,
                //totalSpec,
                totalSpec_txt,
                freeSpec_txt,
                useSpec_txt,
                free_per_txt,
                use_per_txt,
                total_per_txt
            };
            var pic_count = 0;
            var audio_count = 0;
            var doc_count = 0;
            var video_count = 0;
            double img_size = 0;
            double doc_size = 0;
            double video_size = 0;
            double audio_size = 0;
            double total_size = 0;

            dynamic test = null;
            var dir = Directory.GetDirectories(virtual_dir);
            String[] docs = null;
            String[] pics = null;
            //List<object> pics = new List<object>();

            for (var j = 0; j < dir.Length; j++)
            {

                foreach (var x in pic_mapping)
                {

                    //throw new Exception(JsonConvert.SerializeObject(dir));

                    var file_path = Directory.GetFiles(dir[j], $"*.{x}");
                    for (var i = 0; i < file_path.Length; i++)
                    {
                        FileInfo fileInfo = new FileInfo(file_path[i]);
                        img_size += fileInfo.Length;

                    }
                    DirectoryInfo info = new DirectoryInfo(dir[j]);
                    var files = Directory.GetFiles(dir[j], $"*.{x}");
                    if (files.Length > 0)
                    {
                        pic_count += files.Length;
                    }
                }
                foreach(var x in audio_mapping)
                    {
                        var file_path = Directory.GetFiles(dir[j], $"*.{x}");
                        for (var i = 0; i < file_path.Length; i++)
                        {
                            FileInfo fileInfo = new FileInfo(file_path[i]);
                            audio_size += fileInfo.Length;

                        }
                        DirectoryInfo info = new DirectoryInfo(dir[j]);
                        var files = Directory.GetFiles(dir[j], $"*.{x}");
                        if (files.Length > 0)
                        {
                            audio_count += files.Length;
                        }
                    }
                foreach (var x in doc_mapping)
                {
                    //throw new Exception(JsonConvert.SerializeObject(dir));

                    var file_path = Directory.GetFiles(dir[j], $"*.{x}");
                    for (var i = 0; i < file_path.Length; i++)
                    {
                        FileInfo fileInfo = new FileInfo(file_path[i]);
                        doc_size += fileInfo.Length;
                    }
                    var files = Directory.GetFiles(dir[j], $"*.{x}");
                    if (files.Length > 0)
                    {
                        docs = files;
                    }


                }

            }

            doc_count += (docs != null) ? docs.Length : 0;
            doc_count += (docs != null) ? docs.Length : 0;

            //throw new Exception(JsonConvert.SerializeObject(pics));



            var cover = 0;
            List<object> pics_path = new List<object>();
            List<object> video_c = new List<object>();
            using (var db = new DataContext())
            {
                var video = db.video_status.ToList().Select(x => new
                {
                    path_h = x.filename.Substring(0, 8),
                    path_m = x.filename.Substring(0, x.filename.IndexOf("_")),
                    x.filename
                }).ToList();
                foreach (var x in video)
                {
                    var dt = x.filename.Substring(0, 8);
                    var ft = x.filename.Substring(0, x.filename.IndexOf("_"));
                    var direc = dt + "\\" + ft;
                    //throw new Exception(dir);
                    if (Directory.Exists(virtual_dir + direc))
                    {
                        //var file_path = Directory.GetFiles(virtual_dir + direc, $"*.mp4");

                        //for (var i = 0; i < file_path.Length; i++)
                        //{
                        //    if (file_path[i] != null)
                        //    {
                        //        FileInfo fileInfo = new FileInfo(file_path[i]);
                        //        total_size += fileInfo.Length;

                        //    }
                        //}



                        //var pic_path = Directory.GetFiles(virtual_dir + direc, $"*.jpg");
                        // cover += pic_path.Length;

                        //for (var i = 0; i < pic_path.Length; i++) {
                        //    if (pic_path[i] != null) {
                        //        //pics_path.Add(pic_path[i]);
                        //        FileInfo fileInfo = new FileInfo(pic_path[i]);
                        //        img_size += fileInfo.Length;
                        //    }

                        //}


                    }

                }
                video_count = video_c.Count();
                video = db.video_status.Where(x => x.status == "Y").ToList().Select(x => new
                {
                    path_h = x.filename.Substring(0, 8),
                    path_m = x.filename.Substring(0, x.filename.IndexOf("_")),
                    x.filename
                }).ToList().GroupBy(g => (g.path_m), (k, v) => new
                {
                    path_h = v.Select(a => a.path_h).FirstOrDefault(),
                    path_m = v.Select(a => a.path_m).FirstOrDefault(),
                    filename = v.Select(a => a.filename).FirstOrDefault()
                }).ToList();
                foreach (var v in video)
                {
                    if (Directory.Exists(virtual_dir + v.path_h + "\\" + v.path_m))
                    {
                        var root = v.path_h + "\\" + v.path_m;

                        var file_path = Directory.GetFiles(virtual_dir + root, $"*.mp4");
                        for (var i = 0; i < file_path.Length; i++)
                        {
                            if (file_path[i] != null)
                            {
                                var f = new
                                {
                                    filename = file_path[i]
                                };

                                video_c.Add(f);

                                FileInfo fileInfo = new FileInfo(file_path[i]);
                                video_size += fileInfo.Length;

                                total_size += fileInfo.Length;

                            }
                        }

                        var co_n = v.filename.Substring(0, v.filename.IndexOf("_")) + "_cover.jpg";
                        if (File.Exists(virtual_dir + v.path_h + "\\" + v.path_m + "\\" + co_n))
                        {
                            pics_path.Add(co_n);
                            var pic_path = Directory.GetFiles(virtual_dir + root, $"*.jpg");
                            for (var i = 0; i < pic_path.Length; i++)
                            {
                                if (pic_path[i] != null)
                                {
                                    FileInfo fileInfo = new FileInfo(pic_path[i]);
                                    img_size += fileInfo.Length;
                                    total_size += fileInfo.Length;
                                }
                            }
                        }
                    }
                }

                video_count = video_c.Count();
                pic_count += pics_path.Count();

            }
            pic_count += 1;
            var result_count = pic_count + doc_count + video_count;
            var total_count = pic_count + doc_count + video_count;
            var other_count = (total_count > result_count) ? total_count - result_count : 0;
            total_count += other_count;
            var result_size = img_size + doc_size + video_size;
            var total_path = Directory.GetFiles(virtual_dir);
            for (var xx = 0; xx < total_path.Length; xx++)
            {

                FileInfo fileInfo = new FileInfo(total_path[xx]);
                total_size += fileInfo.Length;
                img_size += fileInfo.Length;

            }
            var total_file_use = (total_size - totalSpec);
            var size_use = Math.Ceiling((total_size / total) * 100);
            var img_size_txt = "";
            var doc_size_txt = "";
            var video_size_txt = "";
            var audio_size_txt = "";
            var other_size_txt = "";
            var total_size_txt = "";
            var other_size = (total_size > result_size) ? total_size - result_size : 0;
            var img_gb = img_size / cal_gb;
            var img_mb = img_size / cal_mb;
            var img_kb = img_size / cal_kb;
            var doc_gb = doc_size / cal_gb;
            var doc_mb = doc_size / cal_mb;
            var doc_kb = doc_size / cal_kb;
            double pic_per = 0;
            var pic_per_txt = "";
            double doc_per = 0;
            var doc_per_txt = "";
            double video_per = 0;
            double audio_per = 0;
            var video_per_txt = "";
            var audio_per_txt = "";
            double other_per = 0;
            var other_per_txt = "";

            if (Math.Round(img_gb) > 0)
            {
                img_size_txt = $"{(img_size / cal_gb).ToString("0.##")} GB";
            }
            else if (Math.Round(img_mb) > 0)
            {
                img_size_txt = $"{(img_size / cal_mb).ToString("0.##")} MB";
            }
            else
            {
                img_size_txt = $"{(img_size / cal_kb).ToString("0.##")} KB";
            }
            if (Math.Round(doc_gb) > 0)
            {
                doc_size_txt = $"{(doc_size / cal_gb).ToString("0.##")} GB";
            }
            else if (Math.Round(doc_mb) > 0)
            {
                doc_size_txt = $"{(doc_size / cal_mb).ToString("0.##")} MB";
            }
            else
            {
                doc_size_txt = $"{(doc_size / cal_kb).ToString("0.##")} KB";
            }
            if (Math.Round(video_size / cal_gb) > 0)
            {
                video_size_txt = $"{(video_size / cal_gb).ToString("0.##")} GB";
            }
            else if (Math.Round(video_size / cal_mb) > 0)
            {
                video_size_txt = $"{(video_size / cal_mb).ToString("0.##")} MB";
            }
            else { video_size_txt = $"{(video_size / cal_kb).ToString("0.##")} KB"; }
            if (Math.Round(other_size / cal_gb) > 0)
            {
                other_size_txt = $"{(other_size / cal_gb).ToString("0.##")} GB";
            }
            else if (Math.Round(other_size / cal_mb) > 0)
            {
                other_size_txt = $"{(other_size / cal_mb).ToString("0.##")} MB";
            }
            else
            {
                other_size_txt = $"{(other_size / cal_kb).ToString("0.##")} KB";
            }
         
            if(Math.Round(audio_size/ cal_gb) > 0)
                {
                    audio_size_txt = $"{(audio_size / cal_gb).ToString("0.##")} GB";
                } else if(Math.Round(audio_size/ cal_mb) > 0)
                {
                    audio_size_txt = $"{(audio_size / cal_mb).ToString("0.##")} MB";
                }
                else
                {
                    audio_size_txt = $"{(audio_size / cal_kb).ToString("0.##")} KB";

                }
                total_size = video_size + audio_size + img_size + doc_size + other_size;
                if (Math.Round(total_size / cal_gb) > 0)
                {
                    total_size_txt = $"{(total_size / cal_gb).ToString("0.##")} GB";
                }
                else if (Math.Round(total_size / cal_mb) > 0)
                {
                    total_size_txt = $"{(total_size / cal_mb).ToString("0.##")} MB";
                }
                else
                {
                    total_size_txt = $"{(total_size / cal_kb).ToString("0.##")} KB";
                }
                pic_per = Math.Ceiling((img_size / total_size) * 100);
            pic_per_txt = $"{pic_per.ToString("0.##")} %";
            doc_per = Math.Ceiling((doc_size / total_size) * 100);
            doc_per_txt = $"{doc_per.ToString("0.##")} %";

                other_per = Math.Ceiling((other_size / total_size) * 100);
            other_per_txt = $"{other_per.ToString("0.##")} %";
                audio_per = (audio_size > video_size + img_size + doc_size + other_size) ? Math.Ceiling((audio_size / total_size) * 100) : 0;

                video_per = (video_size>img_size+doc_size+other_size)? Math.Ceiling((video_size / total_size) * 100) - (pic_per +audio_per+ doc_per + other_per):0;
            video_per_txt = $"{video_per.ToString("0.##")} %";
                audio_per_txt = $"{audio_per.ToString("0.##")} %";
            var fileCount = new
            {
                pic_count,
                doc_count,
                video_count,
                other_count,
                total_count,
                audio_count,
                fileSize = new
                {
                    pic_size = img_size_txt,
                    doc_size = doc_size_txt,
                    video_size = video_size_txt,
                    other_size = other_size_txt,
                    audio_size=audio_size_txt,
                    total_size = total_size_txt,
                    pic_per_txt,
                    doc_per_txt,
                    video_per_txt,
                    other_per_txt,
                    audio_per_txt,
                    total_per_txt = "100%"
                    //totalSpec=total,
                    //total_size_=total_size
                    //pic_per
                }

            };
            output = new { driveData, fileCount };
            }
            catch (Exception ex)
            {
                errorMsg= ex.Message;ErrorList(ex);
            }
            return output;
        }

        public dynamic ViewCountVideoReport(string search_text, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.video_on_demand
                                join b1 in db.course_category on new { a.course_category_id } equals new { course_category_id = b1.id } into b2
                                from b in b2.DefaultIfEmpty()
                                select new
                                {
                                    a.id,
                                    a.count_view,
                                    a.name,
                                    a.course_category_id,
                                    course_category_name = b.name
                                }).ToList().Select(x => new
                                {
                                    x.id,
                                    x.course_category_id,
                                    x.course_category_name,
                                    x.count_view,
                                    video_name = x.name
                                });
                    if (search_text != null)
                    {
                        data = data.Where(x => x.course_category_id == search_text);
                    }
                    data = data.OrderByDescending(x => x.count_view);
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

        public dynamic ProblemReport(string search_text, int take, int skip, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.report_problem.ToList().Select(x => new
                    {
                        x.id,
                        x.problem_of_use_id,
                        x.firstname,
                        x.lastname,
                        x.description,
                        x.phone,
                        x.email,
                        x.status,
                        x.is_deleted,
                        x.created_by,
                        x.created_at,
                        x.update_by,
                        x.update_at,
                    });
                    if (search_text != null)
                    {
                        data = data.Where(x => x.description == search_text || x.firstname == search_text || x.lastname == search_text);
                    }
                    data = data.ToList().OrderByDescending(x => x.id).Skip(skip).Take(take);
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

        /* การพิมพ์หน้าคอร์สเรียน */
        public dynamic CoursePrint(string id, ref string error)
        {
            dynamic output = null;

            try
            {
                string id_new = id ?? "";

                using (DataContext db = new DataContext())
                {
                    var data = db.course.Where(x => x.course_category_id == id_new && x.is_deleted == 0).OrderByDescending(x => x.print_count).Select(x => new
                    {
                        x.name,
                        x.print_count,
                        x.batch,
                        x.id
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

        /* การรับชมคอร์สเรียน (course) */
        public dynamic CourseView(string id, ref string error)
        {
            dynamic output = null;

            try
            {
               string id_new = id == null ? "" : id;

                using (DataContext db = new DataContext())
                {
                    var filter_course = db.course.Where(x => x.course_category_id == id_new && x.is_deleted == 0);
                    var join_course_lesson = filter_course.Join(db.course_lesson, x => x.id, y => y.course_id, (x, y) => new {
                        x.id,
                        view = y.count_view
                    });

                    var sum_lesson = join_course_lesson.GroupBy(x => x.id, x => x.view, (a, b) => new {
                        id = a,
                        sum = b.Sum()
                    });

                    var final = sum_lesson.Join(filter_course, x => x.id, y => y.id, (x, y) => new {
                        y.id,
                        y.name,
                        y.batch,
                        count_view = x.sum
                    });

                    var data = final.OrderByDescending(x => x.count_view).ToList();

                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        /* การรับชมคอร์สเรียน (lesson) */
        public dynamic LessonView(string id, ref string error)
        {
            dynamic output = null;

            try
            {
                string id_new = id == null ? "" : id;

                using (DataContext db = new DataContext())
                {
                    var filter_lesson = db.course_lesson.Where(x => x.course_id == id_new && x.is_deleted == 0);
                    var final = filter_lesson.Join(db.course, x => x.course_id, y => y.id, (x, y) => new {
                        x.id,
                        x.count_view,
                        lesson_name = x.name,
                        course_name = y.name,
                        y.batch
                    });

                    var data = final.OrderByDescending(x => x.count_view).ToList();

                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic evaluation_raw(string course_id, ref string error_message)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_eva = db.course_evaluation.Where(w => w.course_id == course_id && w.is_deleted == 0);
                    var data = (from r in db.course_evaluation_result
                                where r.is_deleted == 0 && r.course_id == course_id
                                select new
                                {
                                    id = r.id,
                                    question_id = r.course_evaluation_id,
                                    question = get_eva.Where(w => w.id == r.course_evaluation_id).Select(s => s.question).FirstOrDefault(),
                                    choice_answer_id = r.course_evaluation_choices_id,
                                    choice_answer = db.course_evaluation_choices.Where(w => w.id == r.course_evaluation_choices_id).Select(s => s.choice).FirstOrDefault(),
                                    write_header = get_eva.Where(w => w.id == r.course_evaluation_id).Select(s => s.free_fill_text).FirstOrDefault(),
                                    write_answer = r.answer
                                }).OrderBy(o => o.question_id).ToList();
                    output = data;
                }
                catch (Exception ex)
                {
                    error_message = ex.Message; ErrorList(ex);
                }
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