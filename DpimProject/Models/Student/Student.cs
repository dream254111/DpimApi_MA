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
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models
{
    public class Student
    {
        public dynamic StudentCreate(model_register s, ref string errorMsg)
        {
            dynamic output = null;
            bool status_email = true;
            bool status_id_card = true;
            bool status_register = false;
            using (var db = new DataContext())
            {
                try
                {
                    Token token = new Token();
                    

                    #region Check email
                    var check_email = db.user.Where(w => w.username == s.email || w.email == s.email).FirstOrDefault();
                    if(check_email != null)
                    {
                        status_email = false;
                    }
                    #endregion

                    #region Check Id card
                    var check_id_card = db.student.Where(w => w.is_deleted == 0 && w.id_card == s.id_card).FirstOrDefault();
                    if(check_id_card != null)
                    {
                        status_id_card = false;
                    }
                    #endregion

                    #region Create User
                    if(status_email && status_id_card)
                    {
                        Authentication.Authentication auth = new Authentication.Authentication();
                        Dictionary<string, object> authData = new Dictionary<string, object>();
                        authData.Add("password", s.password);
                        var authenJson = JsonConvert.SerializeObject(authData);
                        var password = token.CreateToken(authenJson);
                        var itemno = db.user.Select(x => x.id).Max() ?? 0;
                        var user_id = ++itemno;
                        var id = db.student.Select(x => x.id).Max() ?? 0;
                        var student_id = ++id;

                        var u = new user
                        {
                            id = user_id,
                            username = s.email,
                            email = s.email,
                            role_id = 2,
                            password = password,
                            is_deleted = 0,
                            created_by = itemno,
                            created_at = DateTime.Now,
                            update_by = itemno,
                            update_at = DateTime.Now
                        };
                        db.user.Add(u);
                        db.SaveChanges();

                        #region Create Student
                        var student = new student
                        {
                            user_id = user_id,
                            id = user_id,
                            student_id = user_id,
                            username = s.email,
                            password = password,
                            career_id = s.career_id,
                            career_name = s.career_name,
                            business_type_id = 0,
                            student_account_type_id = 2,
                            profile_image = "",
                            firstname = s.firstname,
                            lastname = s.lastname,
                            firstname_en = s.firstname_en,
                            lastname_en = s.lastname_en,
                            id_card = s.id_card,
                            gender = s.gender,
                            birthday = s.birthday,
                            address = s.address,
                            sub_district_id = s.sub_district_id,
                            district_id = s.district_id,
                            province_id = s.province_id,
                            zipcode = s.zipcode,
                            email = s.email,
                            phone = s.phone,
                            educational_id = s.educational_id,
                            business_name = "",
                            business_province_id = 0,
                            business_register = null,
                            business_no = "",
                            is_internal_student = 0,
                            front_id_card = "",
                            back_id_card = "",
                            straight_face_image = "",
                            business_attachment = "",
                            is_deleted = 0,
                            created_by = user_id,
                            created_at = DateTime.Now,
                            update_by = user_id,
                            update_at = DateTime.Now,
                            know_channel_name = s.know_channel_name
                    };
                        db.student.Add(student);
                        db.SaveChanges();
                        #endregion

                        #region Create know channel
                        var know_channel = s.know_channel;
                        foreach (var item in know_channel)
                        {
                            mapping_student_know_channel mapping_student_know_channel = new mapping_student_know_channel();
                            mapping_student_know_channel.know_channel_id = item;
                            mapping_student_know_channel.student_id = user_id;
                            db.mapping_student_know_channel.Add(mapping_student_know_channel);
                            db.SaveChanges();
                        }
                        #endregion

                        var manage = new Management.Management();
                        manage.SendingEmail(user_id, null, "01", null, auth,null, ref errorMsg);
                        output = student;
                        status_register = true;
                    }
                    #endregion

                    output = new
                    {
                        status_email,
                        status_id_card,
                        status_register
                    };
                }
                catch (Exception ex) {
                    errorMsg = ex.Message;
                    ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic StudentUpdate(model_update_profile s, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            bool status_id_card = true;
            bool status_update_profile = false;
            using (var db = new DataContext())
            {
                try
                {
                    var get_student = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                    if(get_student == null)
                    {
                        errorMsg = "ไม่มีพบนักเรียนคนนี้ในระบบ";
                        return false;
                    }

                    #region Check Id card
                    var check_id_card = db.student.Where(w => w.is_deleted == 0 && w.id_card == s.id_card && w.user_id != auth.user_id).FirstOrDefault();
                    if (check_id_card != null)
                    {
                        status_id_card = false;
                    }
                    #endregion

                    if (status_id_card)
                    {
                        get_student.profile_image = s.profile_image;
                        get_student.firstname = s.firstname;
                        get_student.lastname = s.lastname;
                        get_student.firstname_en = s.firstname_en;
                        get_student.lastname_en = s.lastname_en;
                        get_student.id_card = s.id_card;
                        get_student.gender = s.gender;
                        get_student.birthday = s.birthday;
                        get_student.province_id = s.province_id;
                        get_student.district_id = s.district_id;
                        get_student.sub_district_id = s.sub_district_id;
                        get_student.zipcode = s.zipcode;
                        get_student.address = s.address;
                        get_student.phone = s.phone;
                        get_student.educational_id = s.educational_id;
                        get_student.career_id = s.career_id;
                        get_student.career_name = s.career_name;
                        get_student.business_name = s.business_name;
                        get_student.business_type_id = s.business_type_id;
                        get_student.business_province_id = s.business_province_id;
                        get_student.business_register = s.business_register;
                        get_student.business_no = s.business_no;
                        get_student.front_id_card = s.front_id_card;
                        get_student.back_id_card = s.back_id_card;
                        get_student.straight_face_image = s.straight_face_image;
                        get_student.business_attachment = s.business_attachment;
                        get_student.know_channel_name = s.know_channel_name;
                        db.SaveChanges();

                        #region know channel
                        foreach (var item in db.mapping_student_know_channel.Where(w => w.student_id == get_student.id).ToList())
                        {
                            db.mapping_student_know_channel.Remove(item);
                            db.SaveChanges();
                        }

                        if (s.know_channel != null)
                        {
                            foreach (var item_mapp in s.know_channel)
                            {
                                mapping_student_know_channel mapping_student_know_channel = new mapping_student_know_channel();
                                mapping_student_know_channel.know_channel_id = item_mapp;
                                mapping_student_know_channel.student_id = (int)get_student.id;
                                db.mapping_student_know_channel.Add(mapping_student_know_channel);
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        status_update_profile = true;
                    }


                    output = new
                    {
                        status_id_card,
                        status_update_profile
                    };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic StudentReadList(string search_text, int take, int skip, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.student.ToList().Select(x => new
                    {
                        x.course_id,
                        x.user_id,
                        x.student_id,
                        x.career_id,
                        x.business_type_id,
                        x.student_account_type_id,
                        x.profile_image,
                        x.firstname,
                        x.lastname,
                        x.firstname_en,
                        x.lastname_en,
                        x.id_card,
                        x.gender,
                        x.birthday,
                        x.address,
                        x.sub_district_id,
                        x.district_id,
                        x.province_id,
                        x.zipcode,
                        x.email,
                        x.phone,
                        x.educational_id,
                        x.position,
                        x.know_channel,
                        x.business_name,
                        x.business_province_id,
                        x.business_register,
                        x.business_no,
                        x.is_internal_student,
                        x.front_id_card,
                        x.back_id_card,
                        x.straight_face_image,
                        x.business_attachment,
                        x.is_deleted,

                        x.username,
                        x.password,

                    });
                    if (!string.IsNullOrEmpty(search_text))
                    {
                        data = data.Where(x => x.firstname.Contains(search_text) || x.lastname.Contains(search_text) || x.email.Contains(search_text) || x.id_card.Contains(search_text));
                    }
                    data = data.ToList().Skip(skip).Take(take).ToList();
                    output = new
                    {
                        data
                    };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;

        }
        public dynamic StudentRead(int? student_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.student.Where(x => x.student_id == student_id).ToList().Select(x => new
                    {
                        x.course_id,
                        x.user_id,
                        x.student_id,
                        x.career_id,
                        x.business_type_id,
                        x.student_account_type_id,
                        x.profile_image,
                        x.firstname,
                        x.lastname,
                        x.firstname_en,
                        x.lastname_en,
                        x.id_card,
                        x.gender,
                        x.birthday,
                        x.address,
                        x.sub_district_id,
                        x.district_id,
                        x.province_id,
                        x.zipcode,
                        x.email,
                        x.phone,
                        x.educational_id,
                        x.position,
                        x.know_channel,
                        x.business_name,
                        x.business_province_id,
                        x.business_register,
                        x.business_no,
                        x.is_internal_student,
                        x.front_id_card,
                        x.back_id_card,
                        x.straight_face_image,
                        x.business_attachment,
                        x.is_deleted,

                        x.username,
                        x.password,

                    }).FirstOrDefault();

                    output = new
                    {
                        data
                    };
                }
                catch (Exception ex)
                {
                    errorMsg = JsonConvert.SerializeObject(ex);
                }
            }
            return output;

        }
        public dynamic StudentProfile(Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var student = db.student.Where(x => x.user_id == auth.user_id).ToList().Select(x => new
                    {
                        x.profile_image,
                        x.firstname,
                        x.lastname,
                        x.firstname_en,
                        x.lastname_en,
                        x.id_card,
                        x.gender,
                        x.birthday,
                        x.province_id,
                        x.district_id,
                        x.sub_district_id,
                        x.zipcode,
                        x.address,
                        x.email,
                        x.phone,
                        x.educational_id,
                        x.career_id,
                        x.career_name,
                        know_channel = db.mapping_student_know_channel.Where(w => w.student_id == x.id).Select(s => s.know_channel_id).ToList(),
                        x.know_channel_name,
                        x.business_name,
                        x.business_type_id,
                        x.business_province_id,
                        x.business_register,
                        x.business_no,
                        x.front_id_card,
                        x.back_id_card,
                        x.straight_face_image,
                        x.business_attachment
                    }).FirstOrDefault();

                    var profile_setting = db.manage_profile.Select(s => new
                    {
                        is_edit_personal_info = s.is_edit_personal_info == 1 ? true : false,
                        is_edit_address = s.is_edit_address == 1 ? true : false,
                        is_edit_email = s.is_edit_email == 1 ? true : false,
                        is_edit_phone = s.is_edit_phone == 1 ? true : false,
                        is_edit_educational = s.is_edit_educational == 1 ? true : false,
                        is_edit_career = s.is_edit_career == 1 ? true : false,
                        is_edit_know_channel = s.is_edit_know_channel == 1 ? true : false,
                        is_edit_business = s.is_edit_business == 1 ? true : false
                    }).FirstOrDefault();

                    output = new
                    {
                        student,
                        profile_setting
                    };
                }
                catch (Exception ex)
                {
                    errorMsg = JsonConvert.SerializeObject(ex);
                }
            }
            return output;

        }
        public dynamic StudentCourseReadList(int? student_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.student
                                join b1 in db.student_course_info on new { a.student_id } equals new { b1.student_id } into b2
                                from b in b2.DefaultIfEmpty()
                                join c1 in db.course on new { b.course_id } equals new { course_id = c1.id } into c2
                                from c in c2.DefaultIfEmpty()
                                select new
                                {
                                    a.course_id,
                                    c.cover_pic,
                                    c.course_category_id,
                                    c.name,
                                    c.is_learning_online,
                                    c.is_has_cost,
                                    c.overview_course,
                                    c.objective_course,
                                    c.print_count,
                                    c.benefits,
                                    c.batch,
                                    c.is_always_learning,
                                    c.learning_startdate,
                                    c.learning_enddate,
                                    c.video_sample,
                                    c.contact_name,
                                    c.contact_phone,
                                    c.contact_email,
                                    a.student_id

                                }).ToList().Select(x => new
                                {
                                    x.course_id,
                                    x.course_category_id,
                                    course_name = x.name,
                                    x.cover_pic,
                                    x.is_learning_online,
                                    x.is_has_cost,
                                    x.overview_course,
                                    x.objective_course,
                                    x.print_count,
                                    x.benefits,
                                    x.batch,
                                    x.is_always_learning,
                                    x.learning_startdate,
                                    x.learning_enddate,
                                    x.video_sample,
                                    x.contact_name,
                                    x.contact_phone,
                                    x.contact_email,
                                    x.student_id

                                }).ToList();
                    if (student_id != null)
                    {
                        data = data.Where(x => x.student_id == student_id).ToList();
                    }

                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic LessonReadList(int? student_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.student
                                join b1 in db.student_course_info on new { a.student_id } equals new { b1.student_id } into b2
                                from b in b2.DefaultIfEmpty()
                                join c1 in db.course on new { b.course_id } equals new { course_id = c1.id } into c2
                                from c in c2.DefaultIfEmpty()

                                join d1 in db.course_lesson on new { b.course_id } equals new { d1.course_id } into d2
                                from d in d2.DefaultIfEmpty()
                                select new
                                {
                                    d.id,
                                    d.course_id,
                                    d.instructor_id,
                                    d.order,
                                    d.name,
                                    d.main_video,
                                    d.count_view,
                                    d.is_interactive,
                                    //d.interactive_time,
                                    //d.interactive_video_1,
                                    //d.interactive_video_2,
                                    d.description,
                                    d.lesson_time,
                                    d.is_deleted,
                                    d.created_by,
                                    d.created_dt,
                                    d.update_by,
                                    d.update_dt,
                                    d.main_p_480,
                                    d.main_p_720,
                                    d.main_p_1080,
                                    d.main_p_original,
                                    d.main_cover_video,


                                }).ToList().Select(x => new
                                {
                                    x.id,
                                    x.course_id,
                                    x.instructor_id,
                                    x.order,
                                    x.name,
                                    x.main_video,
                                    x.count_view,
                                    x.is_interactive,
                                    //x.interactive_time,
                                    //x.interactive_video_1,
                                    //x.interactive_video_2,
                                    x.description,
                                    x.lesson_time,
                                    x.is_deleted,
                                    x.created_by,
                                    x.created_dt,
                                    x.update_by,
                                    x.update_dt,
                                    x.main_p_480,
                                    x.main_p_720,
                                    x.main_p_1080,
                                    x.main_p_original,
                                    x.main_cover_video,


                                }).ToList();


                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic LessonRead(int? student_id, int? lesson_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.student
                                join b1 in db.student_course_info on new { a.student_id } equals new { b1.student_id } into b2
                                from b in b2.DefaultIfEmpty()
                                join c1 in db.course on new { b.course_id } equals new { course_id = c1.id } into c2
                                from c in c2.DefaultIfEmpty()

                                join d1 in db.course_lesson on new { b.course_id } equals new { d1.course_id } into d2
                                from d in d2.DefaultIfEmpty()
                                where a.student_id == student_id && d.id == lesson_id
                                select new
                                {
                                    d.id,
                                    d.course_id,
                                    d.instructor_id,
                                    d.order,
                                    d.name,
                                    d.main_video,
                                    d.count_view,
                                    d.is_interactive,
                                    //d.interactive_time,
                                    //d.interactive_video_1,
                                    //d.interactive_video_2,
                                    d.description,
                                    d.lesson_time,
                                    d.is_deleted,
                                    d.created_by,
                                    d.created_dt,
                                    d.update_by,
                                    d.update_dt,
                                    d.main_p_480,
                                    d.main_p_720,
                                    d.main_p_1080,
                                    d.main_p_original,
                                    d.main_cover_video,


                                }).ToList().Select(x => new
                                {
                                    x.id,
                                    x.course_id,
                                    x.instructor_id,
                                    x.order,
                                    x.name,
                                    x.main_video,
                                    x.count_view,
                                    x.is_interactive,
                                    //x.interactive_time,
                                    //x.interactive_video_1,
                                    //x.interactive_video_2,
                                    x.description,
                                    x.lesson_time,
                                    x.is_deleted,
                                    x.created_by,
                                    x.created_dt,
                                    x.update_by,
                                    x.update_dt,
                                    x.main_p_480,
                                    x.main_p_720,
                                    x.main_p_1080,
                                    x.main_p_original,
                                    x.main_cover_video,


                                }).FirstOrDefault();


                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic StudentCourseRead(int? student_id, string course_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.student
                                join b1 in db.student_course_info on new { a.student_id } equals new { b1.student_id } into b2
                                from b in b2.DefaultIfEmpty()
                                join c1 in db.course on new { b.course_id } equals new { course_id = c1.id } into c2
                                from c in c2.DefaultIfEmpty()
                                join d1 in db.course_lesson on new { course_id = c.id } equals new { d1.course_id } into d2
                                from d in d2.DefaultIfEmpty()
                                where a.student_id == student_id && c.id == course_id
                                select new
                                {
                                    a.course_id,
                                    c.cover_pic,
                                    c.course_category_id,
                                    c.name,
                                    c.is_learning_online,
                                    c.is_has_cost,
                                    c.overview_course,
                                    c.objective_course,
                                    c.print_count,
                                    c.benefits,
                                    c.batch,
                                    c.is_always_learning,
                                    c.learning_startdate,
                                    c.learning_enddate,
                                    c.video_sample,
                                    c.contact_name,
                                    c.contact_phone,
                                    c.contact_email,
                                    d.main_p_480,
                                    d.main_p_720,
                                    d.main_p_1080,
                                    d.main_cover_video

                                }).ToList().Select(x => new
                                {
                                    x.course_id,
                                    x.course_category_id,
                                    course_name = x.name,
                                    x.cover_pic,
                                    x.is_learning_online,
                                    x.is_has_cost,
                                    x.overview_course,
                                    x.objective_course,
                                    x.print_count,
                                    x.benefits,
                                    x.batch,
                                    x.is_always_learning,
                                    x.learning_startdate,
                                    x.learning_enddate,
                                    x.video_sample,
                                    x.contact_name,
                                    x.contact_phone,
                                    x.contact_email,
                                    x.main_p_480,
                                    x.main_p_720,
                                    x.main_p_1080,
                                    x.main_cover_video

                                }).FirstOrDefault();
                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic VideoOnDemandReadList(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.course_category
                                join b1 in db.video_on_demand on new { course_category_id = a.id } equals new { b1.course_category_id } into b2
                                from b in b2.DefaultIfEmpty()

                                select new
                                {
                                    b.id,
                                    b.course_category_id,
                                    b.name,
                                    b.video,
                                    b.count_view,
                                    b.description,
                                    b.producer_name,
                                    b.phone,
                                    b.email,
                                    b.attachment,
                                    b.is_deleted,
                                    b.created_by,
                                    b.created_dt,
                                    b.update_by,
                                    b.update_dt,
                                    b.cover_thumbnail,
                                    catagory_name = a.name,
                                    a.color,


                                }).ToList().Select(x => new
                                {
                                    x.id,
                                    x.course_category_id,
                                    x.name,
                                    x.video,
                                    x.count_view,
                                    x.description,
                                    x.producer_name,
                                    x.phone,
                                    x.email,
                                    x.attachment,
                                    x.is_deleted,
                                    x.created_by,
                                    x.created_dt,
                                    x.update_by,
                                    x.update_dt,
                                    x.cover_thumbnail,
                                    x.catagory_name,
                                    x.color,

                                }).ToList();
                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic VideoOnDemandCatagory(string course_catagory_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.course_category
                                join b1 in db.video_on_demand on new { course_category_id = a.id } equals new { b1.course_category_id } into b2
                                from b in b2.DefaultIfEmpty()
                                where a.id == course_catagory_id
                                select new
                                {
                                    b.id,
                                    b.course_category_id,
                                    b.name,
                                    b.video,
                                    b.count_view,
                                    b.description,
                                    b.producer_name,
                                    b.phone,
                                    b.email,
                                    b.attachment,
                                    b.is_deleted,
                                    b.created_by,
                                    b.created_dt,
                                    b.update_by,
                                    b.update_dt,
                                    b.cover_thumbnail,
                                    catagory_name = a.name,
                                    a.color,


                                }).ToList().Select(x => new
                                {
                                    x.id,
                                    x.course_category_id,
                                    x.name,
                                    x.video,
                                    x.count_view,
                                    x.description,
                                    x.producer_name,
                                    x.phone,
                                    x.email,
                                    x.attachment,
                                    x.is_deleted,
                                    x.created_by,
                                    x.created_dt,
                                    x.update_by,
                                    x.update_dt,
                                    x.cover_thumbnail,
                                    x.catagory_name,
                                    x.color,

                                }).ToList();
                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic VideoOnDemandRead(string course_catagory_id, int? video_on_demand_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.course_category
                                join b1 in db.video_on_demand on new { course_category_id = a.id } equals new { b1.course_category_id } into b2
                                from b in b2.DefaultIfEmpty()
                                where a.id == course_catagory_id && b.id == video_on_demand_id
                                select new
                                {
                                    b.id,
                                    b.course_category_id,
                                    b.name,
                                    b.video,
                                    b.count_view,
                                    b.description,
                                    b.producer_name,
                                    b.phone,
                                    b.email,
                                    b.attachment,
                                    b.is_deleted,
                                    b.created_by,
                                    b.created_dt,
                                    b.update_by,
                                    b.update_dt,
                                    b.cover_thumbnail,
                                    catagory_name = a.name,
                                    a.color,


                                }).ToList().Select(x => new
                                {
                                    x.id,
                                    x.course_category_id,
                                    x.name,
                                    x.video,
                                    x.count_view,
                                    x.description,
                                    x.producer_name,
                                    x.phone,
                                    x.email,
                                    x.attachment,
                                    x.is_deleted,
                                    x.created_by,
                                    x.created_dt,
                                    x.update_by,
                                    x.update_dt,
                                    x.cover_thumbnail,
                                    x.catagory_name,
                                    x.color,

                                }).FirstOrDefault();
                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic LessonExerciseReadList(string course_id, int? lesson_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.course_lesson_exercise.Where(x => x.course_id == course_id && x.course_lesson_id == lesson_id).ToList().Select(x => new
                    {
                        header = new
                        {
                            x.id,
                            x.course_id,
                            x.course_lesson_id,
                            x.question,
                            x.image,
                            x.video,
                            choice = db.course_lesson_exercise_answer_choices.Where(a => a.course_lesson_exercise_id == x.id).ToList().Select(a => new
                            {
                                a.id,
                                a.course_lesson_exercise_id,
                                a.answer,
                                a.order,


                            }).ToList().OrderBy(a => a.order),
                            match = db.course_lesson_exercise_answer_match.Where(a => a.course_lesson_exercise_id == x.id).ToList().Select(a => new
                            {
                                a.id,
                                a.course_lesson_exercise_id,
                                a.question,
                                a.answer,



                            }).ToList().OrderBy(a => a.id)
                        }
                    }).ToList();

                    output = new { data };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic ExamResult(int? student_id,string course_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                //try
                //{
              
                int? answer_ = 1;
               
                var score_total = db.course_exam.Where(x => x.course_id == course_id).ToList().Count();
                var score_pass = db.course_exam_logging.Where(x => x.course_id == course_id && x.student_id == student_id && x.score == 1).ToList().Count();
                var data1 = db.course_exam_logging.Where(x => x.course_id == course_id && x.student_id == student_id).ToList();
                var percent = ((Math.Ceiling((decimal) score_pass) / Math.Ceiling((decimal)score_total)) * 100);
                output = new
                {
                    data = data1,
                    score_total,
                    score_pass,
                    percent


                };
                //}
                //catch (Exception ex)
                //{
                //    errorMsg= ex.Message;ErrorList(ex);
                //}
            }
            return output;
        }
        public dynamic ExamAnswer(List<course_exam_logging> ans, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                //try
                //{
               string course_id = "";
                int? is_prestest = 0;
                foreach (var a in ans)
                {
                    var data = db.course_exam_logging.Where(x => x.course_id == a.course_id && x.course_exam_id == a.course_exam_id && x.student_id == auth.student_id).FirstOrDefault();
                    if (data != null)
                    {
                        data.student_id = a.student_id;
                        data.course_id = a.course_id;
                        data.is_pretest = a.is_pretest;
                        data.score = (db.course_exam.Where(x => x.course_id == a.course_id && x.id == a.course_exam_id && x.answer == a.course_exam_answer_id).FirstOrDefault() != null) ? 1 : 0;


                        data.update_by = auth.user_id;
                        data.update_dt = DateTime.Now;
                        data.course_exam_answer_id = a.course_exam_answer_id;
                        data.course_exam_id = a.course_exam_id;
                        db.SaveChanges();
                    }
                    var itemno = (db.course_exam_logging.Where(x => x.course_id == a.course_id && x.student_id == auth.student_id).Select(x => x.id).Max());
                    //a.id = itemno;
                    a.id = ++itemno;
                    a.score = (db.course_exam.Where(x => x.course_id == a.course_id && x.id == a.course_exam_id && x.answer == a.course_exam_answer_id).FirstOrDefault() != null) ? 1 : 0;
                    a.created_dt = DateTime.Now;
                    a.created_by = auth.user_id;
                    db.course_exam_logging.Add(a);
                    db.SaveChanges();
                    course_id = a.course_id;
                    is_prestest = a.is_pretest;
                }
                var scoretotal = db.course_exam.Where(x => x.course_id == course_id).ToList().Sum(x=>x.score);
                var percent_pass = db.course_exam.Where(x => x.course_id == course_id).Select(x => x.percent_pass).FirstOrDefault() ?? 80;
                var scorepass = (from a in db.course_exam_logging
                                 join b1 in db.course_exam on new { a.course_exam_id, a.course_id } equals new { course_exam_id = b1.id, b1.course_id } into b2
                                 from b in b2.DefaultIfEmpty()
                                 where a.course_id==course_id&&a.student_id== auth.student_id
                                 select new
                                 {
                                     a.score,

                                 }).ToList().Sum(x => x.score);
                var data1 = db.course_exam_logging.Where(x => x.course_id == course_id && x.student_id == auth.student_id).ToList();
                var percent = Math.Ceiling(((decimal)scorepass / (decimal)scoretotal) * 100);
                if (percent > percent_pass && is_prestest == 0)
                {

                
                    var manage = new Management.Management();
                    GenereteCertificate(auth.student_id, course_id, ref errorMsg);
                    manage.SendingEmail(auth.student_id, course_id, "04", null, auth,null, ref errorMsg);

                }
                output = new
                {
                    data = data1,
                    score_total=scoretotal,
                    score_pass=scorepass,
                    percent=$"{percent.ToString("0.##")} %"


                };
                //}
                //catch (Exception ex)
                //{
                //    errorMsg= ex.Message;ErrorList(ex);
                //}
            }
            return output;
        }
        public dynamic ExamPerCheck(string course_id,int? student_id ,ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var get_exam_logging = db.course_exam_logging.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == student_id).ToList();
                    var get_exam = db.course_exam.Where(x => x.course_id == course_id).ToList();
                    var scorepass = (from a in db.course_exam_logging
                                     join b1 in db.course_exam on new { a.course_exam_id, a.course_id } equals new { course_exam_id = b1.id, b1.course_id } into b2
                                     from b in b2.DefaultIfEmpty()
                                     where a.course_id == course_id && a.student_id == student_id && a.is_pretest == 0
                                     select new
                                     {
                                         a.score,

                                     }).ToList().Sum(x => x.score);
                    var                    score = get_exam_logging.Where(w => w.is_pretest == 0 && get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0).Count();
                    var total = db.course_exam.Where(x => x.course_id == course_id).Count();
                    var percent = ((decimal)scorepass / (decimal)total) * 100;
                    var percent_course = ((decimal)score / (decimal)total) * 100;
                    var data = new
                    {
                        percent_pete = percent_course,
                        scorepass_pete = score,
                        percent_nick = percent,
                        scorepass_nick = scorepass,
                    };
                    output = new
                    {
                        data 
                    };
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    ErrorList(ex);
                }
            }
                return output;
        }
        public dynamic ExamRead(string course_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var random = new Random();
                    //dynamic data = null;
                    //var setting = (from a in db.course_exam_setting
                    //               join b1 in db.course_exam_type on new { type_exam_id = a.type_of_exam } equals new { b1.type_exam_id } into b2
                    //               from b in b2.DefaultIfEmpty()
                    //               where a.course_id == course_id &&
                    //               select new
                    //               {
                    //                   a.exam_id,
                    //                   a.course_id,
                    //                   a.random_answer,
                    //                   a.course_answer,
                    //                   a.percent_pass,
                    //                   b.type_exam_id,
                    //                   b.type_exam_desc
                    //               }).FirstOrDefault();

                    var data = db.course_exam.Where(x => x.course_id == course_id).ToList().Select(x =>

                    new
                    {
                          exam_id = x.id,
                            x.course_id,
                            x.order,
                            x.question,
                            x.image,
                            x.video,

                            detail = db.course_exam_answer.Where(a =>/* a.exam_id == x.exam_id &&*/ a.course_id == x.course_id && a.course_exam_id == x.id).Select(a => new
                            {
                                a.id,
                                a.course_exam_id,
                                a.answer,
                                a.order,
                                

                            }).ToList().OrderBy(a => random.Next(0, db.course_exam_answer.Where(n =>/* a.exam_id == x.exam_id &&*/ n.course_id == x.course_id && n.course_exam_id == x.id).ToList().Count())).ToList()
                        
                    }).ToList().OrderBy(a => random.Next(0, db.course_exam.Where(x => x.course_id == course_id).ToList().Count())).ToList();
                
               
                    //if (setting.random_answer.Contains("Y"))
                    //{
                    //var random = new Random();
                    //List<object> data2 = new List<object>();
                    //for (int i = 0; i < data1.Count(); i++)
                    //        {
                    //            int j = random.Next(0, data.Count());
                    //            data2.Add(data1[j].header);
                    //        }
                    //        data = data2;

                    //    }
                    //else
                    //{
                    //    data = data1;
                    //}
                    output = new
                    {
                        //data,
                        data
                        


                    };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic ExamReadList(string course_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.course_exam.Where(x => x.course_id == course_id).ToList().Select(x => new
                    {
                        exam_id = x.id,
                        x.course_id,
                        x.order,
                        x.question,
                        x.image,
                        x.video,
                    }).ToList();

                    output = new
                    {
                        data

                    };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic ExamPassCourseAll(int? student_id, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.student
                                join b1 in db.student_course_info on new { a.id } equals new { id = b1.student_id } into b2
                                from b in b2.DefaultIfEmpty()
                                join c1 in db.course on new { b.course_id } equals new { course_id = c1.id } into c2
                                from c in c2.DefaultIfEmpty()
                                join d1 in db.course_exam on new { b.course_id } equals new { d1.course_id } into d2
                                from d in d2.DefaultIfEmpty()
                                join f1 in db.course_exam_logging on new { b.course_id, b.student_id, course_exam_id = d.id } equals new { f1.course_id, f1.student_id, f1.course_exam_id } into f2
                                from f in f2.DefaultIfEmpty()
                                where d.answer == f.course_exam_answer_id
                                select new
                                {
                                    f.student_id,
                                    f.course_id,
                                    f.is_pretest,
                                    f.score,
                                    f.is_deleted,
                                    f.created_by,
                                    f.created_dt,
                                    f.update_by,
                                    f.update_dt,
                                    f.course_exam_answer_id,
                                    f.course_exam_id,
                                    f.id,


                                }).ToList().GroupBy(g => (g.course_id), (k, v) => new
                                {
                                    student_id = v.Select(x => x.student_id).FirstOrDefault(),
                                    course_id = v.Select(x => x.course_id).FirstOrDefault(),
                                    is_pretest = v.Select(x => x.is_pretest).FirstOrDefault(),
                                    score = v.Select(x => x.score).FirstOrDefault(),

                                    course_exam_answer_id = v.Select(x => x.course_exam_answer_id).FirstOrDefault(),
                                    course_exam_id = v.Select(x => x.course_exam_id).FirstOrDefault(),
                                    exam_loggin_id = v.Select(x => x.id).FirstOrDefault(),
                                    course_pass_score = v.Count()
                                }).ToList();
                    output = new
                    {
                        data
                    };
                }
                catch(Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
             
            }

            return output;
        }
        public void StudentVideoProgress(student_video_progress s,Authentication.Authentication auth,ref string errorMsg)
        {
            using (var db = new DataContext())
            {
                var data = db.student_video_progress.Where(x => x.student_id == auth.user_id && x.course_id == s.course_id && x.video_progress != 100).FirstOrDefault();
                if (data != null)
                {
                    data.video_path = s.video_path;
                    data.video_position = s.video_position;
                    data.video_progress = s.video_progress;
                 
                    data.update_by = auth.user_id;
                    data.update_dt =DateTime.Now;
                    db.SaveChanges();
                }
                //s.student_id = auth.user_id;
                s.create_at = DateTime.Now;
                s.create_by = auth.user_id;
                db.student_video_progress.Add(s);
                db.SaveChanges();
            }
        }

        public dynamic CertificateReadList(int? student_id ,ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {try
                {
                    var data = (from a in db.certificate_info
                                join b1 in db.course on new { a.course_id } equals new { course_id = b1.id } into b2
                                from b in b2.DefaultIfEmpty()
                                where a.student_id == student_id && b.is_deleted == 0
                                select new
                                {
                                    a.certificate_id,
                                    a.certificate_dt,
                                    a.course_id,
                                    course_name = b.name,
                                    b.batch,
                                    a.path,
                                    b.cover_pic,
                                    
                                }).ToList().Select(x=>new {
                                    x.certificate_dt,
                                    x.certificate_id,
                                    x.course_id,
                                    x.course_name,
                                    x.batch,
                                    x.cover_pic,
                                    count_lesson=db.course_lesson.Where(a=>a.course_id==x.course_id && a.is_deleted == 0).ToList().Count(),
                                    lesson_time=(db.course_lesson.Where(a=>a.course_id==x.course_id && a.is_deleted == 0).Sum(a=>a.lesson_time)),
                                 
                                }).ToList();
                    output = new
                    {
                        data
                    };
                }catch(Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
       public dynamic CheckCoursePermission(string course_id,Authentication.Authentication auth ,ref string error)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try {
                    var student = db.student.Where(x => x.user_id == auth.user_id).FirstOrDefault();
                    var data = (from a in db.student
                                join b1 in db.course_permission on new { a.student_account_type_id } equals new { student_account_type_id = b1.student_type_permission } into b2
                                from b in b2.DefaultIfEmpty()
                                where a.user_id == auth.user_id
                                select new
                                {
                                    a.student_id,
                                    a.user_id,
                                    a.student_account_type_id
                                }).FirstOrDefault();
                 var course_permission=       db.course_permission.Where(x => x.course_id == course_id&&x.student_type_permission==data.student_account_type_id).FirstOrDefault();
                    List<object> d = new List<object>();
                    dynamic course = null;
                    if (course_permission != null)
                    {
                        if (data != null)
                        {
                            course = db.course.Where(x => x.id == course_id).FirstOrDefault();

                        }
                        else
                        {
                            throw new Exception("ไม่พบวิชานี้ โปรดติดต่อผู้ดูแลระบบ");
                        }
                    }
                    else
                    {
                        throw new Exception("ไม่ได้กำหนดสิทธิการมองเห็น");
                    }
                    output = new
                    {
                        data = course
                    };
                } catch(Exception ex)
                {
                    error = ex.Message;
                    ErrorList(ex);
                }

            }
            return output;
        }
        public dynamic CertificateRead(string certificate_id,int? student_id,ref string errorMsg)
        {
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["CertUrl"];

            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from a in db.certificate_info
                                join b1 in db.course on new { a.course_id } equals new { course_id = b1.id } into b2
                                from b in b2.DefaultIfEmpty()
                                where a.certificate_id == certificate_id&&a.student_id==student_id && b.is_deleted == 0
                                select new
                                {
                                    a.certificate_id,
                                    a.certificate_dt,
                                    a.course_id,
                                    course_name = b.name,
                                    b.batch,
                                    a.path,
                                    b.cover_pic,
                                    a.student_id
                                }).ToList().Select(x => new {
                                    x.certificate_dt,
                                    x.certificate_id,
                                    x.course_id,
                                    x.course_name,
                                    x.batch,
                                    x.cover_pic,
                                    count_lesson = db.course_lesson.Where(a => a.course_id == x.course_id && a.is_deleted == 0).Count(),
                                    lesson_time = (db.course_lesson.Where(a => a.course_id == x.course_id && a.is_deleted == 0).Sum(a => a.lesson_time)),
                                    print_count = (db.certificate_print.Where(a => a.certificate_id == x.certificate_id && a.course_id == x.course_id )).Count(),
                                    cert_path = virtual_dir + x.path + "&course_id=" + x.course_id
                                }).FirstOrDefault();
                    output = new
                    {
                        data
                    };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public dynamic PrintCertificate(int? student_id,string course_id ,ref string errorMsg,ref string fileName)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {try
                {
                  
                    var data = db.certificate_info.Where(x => x.student_id == student_id && x.course_id == course_id).FirstOrDefault();
                    var item_no = db.certificate_print.Where(x => x.course_id == course_id && x.student_id == student_id && x.certificate_id == data.certificate_id).Select(x => x.itemno).Max() ?? 0;
                    var id = db.certificate_print.Select(x => x.id).Max() ?? 0;
                    var c = new certificate_print
                    {
                        id = ++id,
                        itemno = ++item_no,
                        student_id = student_id,
                        course_id = course_id,
                        certificate_id = data.certificate_id,
                        certificate_dt = data.certificate_dt,
                        print_dt = DateTime.Now,
                        print_by = student_id
                    };
                    db.certificate_print.Add(c);
                    db.SaveChanges();
                    fileName = data.course_id + "\\" + data.path;
                    //GenereteCertificate(student_id, course_id, ref errorMsg);
                    errorMsg =data.course_id+"\\"+ data.path;
                    output = new
                    {
                        data
                    };
                }catch(Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
            }
            return output;
        }
        public void GenereteCertificate(int? student_id, string course_id,ref string errorMsg)
        {
            var virtual_dir = System.Configuration.ConfigurationManager.AppSettings["Certificate"];
            var photo = new Bitmap(virtual_dir + "template.png");
            string CertID = "";
            DateTime? CertDate = DateTime.Now;
            Bitmap bitmap = new Bitmap(photo.Width,photo.Height);
            var new_path = "";
            using (var db = new DataContext())
            {
                //try
                //{
                System.Globalization.CultureInfo th_format = new System.Globalization.CultureInfo("th-Th");
                var course = db.course.Where(x => x.id == course_id).FirstOrDefault();
                var student = db.student.Where(x => x.student_id == student_id).FirstOrDefault();
                var data = (from a in db.student_course_info
                            join b1 in db.student on new { a.student_id } equals new { student_id = b1.id } into b2
                            from b in b2.DefaultIfEmpty()
                            join c1 in db.course on new { a.course_id } equals new { course_id = c1.id } into c2
                            from c in c2.DefaultIfEmpty()
                            where a.student_id == student_id && a.course_id == course_id
                            select new
                            {
                                //a.certificate_id,
                                //a.certificate_dt,
                                b.student_id,
                                b.title_name,
                                b.firstname,
                                b.lastname,
                                c.name,
                                c.id,
                                //a.path
                            }).ToList().Select(x => new
                            {
                                x.firstname,
                                //x.certificate_dt,
                                //x.certificate_id,
                                x.lastname,
                                x.title_name,
                                course_name = x.name,
                                course_id = x.id,
                                x.student_id,
                                //x.path
                            }).FirstOrDefault();
                var path = virtual_dir + course.name.ToString() + "\\";
                //errorMsg = path;
                //throw new Exception(path);
                if (!Directory.Exists(virtual_dir + course_id))
                {
                    Directory.CreateDirectory(virtual_dir + course_id);
                }
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    CertID = CreateCertificateID(db.certificate_info.Where(x => x.course_id == course_id).ToList().Count() + 1);

                      var ft = new PrivateFontCollection();
                    ImageAttributes attributes = new ImageAttributes();
                    //var url =
                    //$"D:\\DpimProjectApi\\";
                    var url = System.Configuration.ConfigurationManager.AppSettings["defaultpath"] ?? $"D:\\DpimProjectApi\\";
                    ft.AddFontFile(url + "fonts\\NotoSansThai\\NotoSansThai-Regular.ttf");
                    ft.AddFontFile(url + "fonts\\NotoSansThai\\NotoSansThai-Bold.ttf");
                    ft.AddFontFile(url + "fonts\\THSarabun\\THSarabunNew.ttf");
                    ft.AddFontFile(url + "fonts\\THSarabun\\THSarabunNew Bold.ttf");
                    ft.AddFontFile(url + "fonts\\Sarabun\\Sarabun-Regular.ttf");
                    ft.AddFontFile(url + "fonts\\Sarabun\\Sarabun-Bold.ttf");
                    FontFamily[] fonts = ft.Families;

                    graphics.Clear(Color.White);
                    graphics.DrawImage(photo, new Rectangle(0, 0, photo.Width, photo.Height), 0, 0, photo.Width, photo.Height, GraphicsUnit.Pixel, attributes);
                    Font noto = new Font(ft.Families[0].Name,
                        12,
                        FontStyle.Regular,
                        GraphicsUnit.Pixel);
                    //var notoB = ft.Families[1].Name;
                    Font thSarabun = new Font(ft.Families[1].Name,
                        12,
                        FontStyle.Regular,
                        GraphicsUnit.Pixel);
                    Font sarabun = new Font(ft.Families[2].Name,
                        14,
                        FontStyle.Regular,
                        GraphicsUnit.Pixel);
                    //var thBold = ft.Families[3].Name;


                    using (var font = new Font("Sarabun", 10, FontStyle.Regular))
                    {

                        Font sarabun_name = new Font(ft.Families[2].Name,
                                                14,
                                                FontStyle.Bold,
                                                GraphicsUnit.Pixel);
                        var name_color = Color.FromArgb(27, 159, 136);
                        SolidBrush b = new SolidBrush(name_color);
                        var default_color = Color.FromArgb(120, 120, 120);
                        SolidBrush d = new SolidBrush(default_color);
                        // draw some text on image
                        var name = $"{student?.title_name}{student?.firstname}  {student?.lastname}";
                        SizeF nameSize = graphics.MeasureString(name, sarabun_name);
                        var course_ = "เป็นผู้ผ่านการฝึกอบรม" + course?.name;
                        var year = CertDate.Value.Year + 543;
                        SizeF size = graphics.MeasureString(course_.ToString(), sarabun);
                        //var date = $"ออกให้ ณ วันที่ {Dtl.arabic_to_thai(data.certificate_dt.Value.Day.ToString())} {Dtl.thai_month(data.certificate_dt.Value.Month)} พุุทธศักราช " + Dtl.arabic_to_thai((data.certificate_dt.Value.Year + 543).ToString().Trim());
                        var date = $"ออกให้ ณ วันที่ {Dtl.arabic_to_thai(CertDate.Value.Day.ToString() ?? "")} {Dtl.thai_month(CertDate.Value.Month)} พุุทธศักราช " + Dtl.arabic_to_thai(year.ToString()?? "");
                        SizeF dateSize = graphics.MeasureString(date, sarabun);
                        var cert = $"เลขที่บัตร {Dtl.arabic_to_thai(CertID)}/{Dtl.arabic_to_thai(CertDate.Value.Year.ToString())}";
                        SizeF certSize = graphics.MeasureString(cert, sarabun);
                        float course_h = size.Height;
                        if (bitmap.Width < size.Width)
                        {
                            course_h = size.Height * 2;
                        }
                       
                        RectangleF drawRect = new RectangleF(5, 196, bitmap.Width - 10, course_h);
                        RectangleF dateRect = new RectangleF((bitmap.Width - dateSize.Width) / 2, drawRect.Y+40, dateSize.Width, dateSize.Height);
                        RectangleF certRect = new RectangleF((bitmap.Width - certSize.Width) / 2, dateRect.Y + 17, certSize.Width, certSize.Height);
                        StringFormat drawFormat = new StringFormat();
                        drawFormat.Alignment = StringAlignment.Center;
                        Pen blackPen = new Pen(Color.Black);

                        //graphics.DrawRectangle(blackPen, 5, 196, bitmap.Width - 10, course_h);
                        graphics.DrawString(name, sarabun_name, b, (photo.Width - nameSize.Width) / 2, 170);
                        graphics.DrawString(course_, sarabun, d,drawRect,drawFormat);

                        graphics.DrawString(date, sarabun, d,dateRect,drawFormat);
                        graphics.DrawString(cert, sarabun, d,certRect,drawFormat);
                        //var license = db.certificate.Where(x => x.is_delete == 0).FirstOrDefault();
                        //var license_path = new Bitmap(license.path);
                        //graphics.DrawImage(license_path,new PointF((bitmap.Width-license_path.Width)/2,certRect.Y+15));
                        // etc
                    }
                }

                new_path = virtual_dir + course_id + "\\" + CertID+"_"+CertDate.Value.Year.ToString() + ".png";
                var d1 = db.certificate_info.Where(x => x.student_id == student_id && x.course_id == course_id && x.cert_status == "Y").FirstOrDefault();

                if (d1 != null)
            {
                    d1.path = CertID + "_" + CertDate.Value.Year.ToString() + ".png";
                    db.SaveChanges();
                }
                else
                {
                    var s = new certificate_info
                    {
                        certificate_id = $"{CertID}",
                        student_id = student_id,
                        course_id = course_id,
                        certificate_dt = CertDate,
                        created_dt = CertDate,
                        path = CertID+"_"+CertDate.Value.Year.ToString() + ".png",
                        cert_status = "Y"
                    };
                    db.certificate_info.Add(s);
                    db.SaveChanges();
                }
             



                //}catch(Exception ex) { errorMsg= ex.Message;ErrorList(ex); }
                FileInfo fi = new FileInfo(new_path);
                if (!fi.Exists)
                {
                    bitmap.Save(new_path, ImageFormat.Png);
                }
                bitmap.Dispose();
            }
        


        }
        public void StudentRequestForgetPassword(string email, Authentication.Authentication auth, ref string errorMsg, ref string token)
        {
            using (var db = new DataContext())
            {
                try
                {
                    var stu = db.user.Where(x => x.email == email).FirstOrDefault();

                    if (stu != null)
                    {
                        if (stu.role_id == 2) { 
                            Dictionary<string, object> stuData = new Dictionary<string, object>();
                            stuData.Add("student_id", stu.id);
                            stuData.Add("username", stu.username);
                            stuData.Add("password", stu.password);
                            stuData.Add("enddate", DateTime.Now.AddMinutes(10));

                            string authDataJson = JsonConvert.SerializeObject(stuData);
                            Token token_s = new Token();
                            token = token_s.CreateToken(authDataJson);
                            var f = new student_forget_password_token
                            {
                                user_id = stu.id,
                                token = token,
                                create_at = DateTime.Now,
                                create_by = stu.id
                            };

                            db.student_forget_password_token.Add(f);
                            db.SaveChanges();
                            var management = new Models.Management.Management();
                            token = WebUtility.UrlEncode(token);
                            management.SendingEmail(stu.id, null, "02", token, auth,null, ref errorMsg);
                        }
                        else
                        {
                            errorMsg = "ไม่พบ email นี้ในระบบ";
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;ErrorList(ex);
                }
            }

        }
        public dynamic StudentForgetPassword(string password, string token, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    Dictionary<string, object> authData = null;
                    Token token_ = new Token();

                    //token = WebUtility.UrlDecode(token);
                    var token2_ = token;
                    token_.CheckToken(token2_, out token2_);
                    authData = JsonConvert.DeserializeObject<Dictionary<string, object>>(token2_);
                    if(authData == null)
                    {
                        errorMsg = "Token ไม่ถูกต้อง";
                        return false;
                    }

                    var enddate = DateTime.Parse(authData["enddate"].ToString());
                    if (enddate > DateTime.Now)
                    {
                        var forget = db.student_forget_password_token.Where(x => x.token == token).FirstOrDefault();
                        if (forget != null)
                        {
                            var get_student = db.student.Where(w => w.id == forget.user_id).FirstOrDefault();
                            if (get_student == null)
                            {
                                errorMsg = "ไม่พบนักเรียนในระบบ";
                                return false;
                            }



                            Token _token = new Token();
                            Dictionary<string, object> _authData = new Dictionary<string, object>();
                            _authData.Add("password", password);
                            var authenJson = JsonConvert.SerializeObject(_authData);
                            var hash_password = _token.CreateToken(authenJson);


                            var stu = db.user.Where(x => x.id == forget.user_id).FirstOrDefault();
                            if (stu != null)
                            {
                                stu.password = hash_password;
                                stu.update_at = DateTime.Now;
                                stu.update_by = auth.user_id;
                                db.SaveChanges();
                            }
                            forget.update_by = auth.user_id;
                            forget.update_at = DateTime.Now;
                            db.SaveChanges();

                            get_student.password = hash_password;
                            db.SaveChanges();

                            output = stu;
                        }
                        else
                        {
                            errorMsg = "ไม่พบ Token ในระบบ กรุณาติดต่อผู้ดูแลระบบ";
                            return false;
                        }
                    }
                    else
                    {
                        errorMsg = "Token หมดอายุ กรุณาลองใหม่อีกครั้ง";
                        return false;
                    }
                    
                }catch(Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }
                
            }
            return output;

        }
        public dynamic checkCourseEnddate()
        {
            string error = "";
            dynamic output = null;
            Dictionary<string, object> d1 = new Dictionary<string, object>();

            using (var db = new DataContext())
            {
                //try
                //{
                    //var manage = new Management.Management(); manage.SendingEmail(3, 3018, "05", null, auth, ref error);

                    var auth = new Authentication.Authentication();
                    var data = db.student_course_info.Where(x=>x.learning_enddate!=null).ToList().Select(x=>new
                    {
                        x.student_id,
                        x.course_id,
enddate=((x.learning_enddate??DateTime.Now.AddDays(30))-DateTime.Now).TotalDays<30?true : false
                }).ToList();
                    var manage = new Models.Management.Management();
                if (data != null)
                {
                    foreach (var j in data)
                    {
                        var checkEmail = db.email_sending.Where(x => x.student_id == j.student_id && x.course_id == j.course_id && x.email_type == "05").FirstOrDefault();
                        if (checkEmail == null)
                        {
                            if (j.enddate)
                            {
                                manage.SendingEmail(j.student_id, j.course_id, "05", null, auth,null, ref error);

                            }
                        }
                        //d1.Add("student_id", j.student_id);
                        //d1.Add("course_id", j.course_id);


                    }
                }
                    output = new
                    {
                        data,
                        error
                    };
                //}
                //catch (Exception ex)
                //{
                //   throw new Exception(ex.Message);
                //}
               
                return output;
            }
        }

        public dynamic get_all_video(string category_id, string sort, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = new List<object>();
            try
            {
                using (var db = new DataContext())
                {
                    var student_type = auth.user_id == 0 ? 1 : db.student.Where(w => w.user_id == auth.user_id).Select(s => s.student_account_type_id).FirstOrDefault();
                    var get_data = db.video_on_demand.Where(w => w.is_deleted == 0 && db.content_permission.Where(ww => ww.video_id == w.id && ww.student_type_permission == student_type).Count() > 0).ToList();

                    #region Check category
                    category_id = category_id.Replace("\"", "");
                    if (category_id != "" && category_id != null && category_id != "0" && category_id != "''")
                    {
                        var arr_category = category_id.Split(',');
                        get_data = get_data.Where(w => arr_category.Contains(w.course_category_id.ToString())).ToList();
                    }
                    #endregion

                    if (get_data.Count > 0)
                    {
                        var data = get_data.Select(s => new
                        {
                            id = s.id,
                            course_category_id = s.course_category_id,
                            category_nane = db.course_category.Where(w => w.id == s.course_category_id).Select(n => n.name).FirstOrDefault(),
                            category_color = db.course_category.Where(w => w.id == s.course_category_id).Select(cat_color => cat_color.color).FirstOrDefault(),
                            name = s.name,
                            link_video = s.video,
                            cover_thumbnail = s.cover_thumbnail,
                            created_at = s.created_dt,
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
                        }).OrderByDescending(o => o.created_at).ToList();

                        output = data;
                        if (sort != "" && sort != null)
                        {
                            switch (sort)
                            {
                                case "newest":
                                    output = data.OrderByDescending(o => o.created_at).ToList();
                                    break;
                                case "oldest":
                                    output = data.OrderBy(o => o.created_at).ToList();
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrorList(ex);
            }
            return output;
        }

        public dynamic get_all_banner(ref string errorMsg)
        {
            dynamic output = new List<object>();
            try
            {
                using (var db = new DataContext())
                {
                    var data = db.banner.Where(w => w.is_deleted == 0 && w.active == 1).OrderByDescending(o => o.update_at).ToList();
                    output = data;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrorList(ex);
            }
            return output;
        }

        public dynamic post_send_email_cert(string course_id, string token, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                using (var db = new DataContext())
                {
                    var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                    if (get_student_id == null)
                    {
                        error_message = "ไม่พบนักเรียนในระบบ";
                        return false;
                    }

                    var check_course_info = db.student_course_info.Where(w => w.course_id == course_id && w.student_id == get_student_id.id).FirstOrDefault();
                    if (check_course_info == null)
                    {
                        error_message = "นักเรียนคนนี้ไม่ได้ลงทะเบียนคอร์สนี้ไว้";
                        return false;
                    }

                    var management = new Models.Management.Management();
                    management.SendingEmail(get_student_id.id, course_id, "04", token, auth, null, ref error_message);

                    output = "ส่ง email เรียบร้อยแล้ว";
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic get_profile_minimal(Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.student.Where(x => x.user_id == auth.user_id).ToList().Select(x => new
                    {
                        x.profile_image,
                        x.firstname,
                        x.lastname,
                        x.student_account_type_id,
                        account_type_name = db.student_account_type.Where(w => w.id == x.student_account_type_id).Select(s => s.name).FirstOrDefault()
                    }).FirstOrDefault();


                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = JsonConvert.SerializeObject(ex);
                }
            }
            return output;
        }

        public dynamic get_tutorial(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = (from th in db.tutorial_header
                                select new
                                {
                                    id = th.tutorial_id,
                                    order = th.order,
                                    tutorial_text = th.tutorial_text,
                                    image = th.image,
                                    link = th.link,
                                    detail = db.tutorial_detail.Where(w => w.tutorial_id == th.tutorial_id).ToList()
                                }).ToList();


                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = JsonConvert.SerializeObject(ex);
                }
            }
            return output;
        }

        #region master data
        public dynamic get_master_data(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var career = db.career.ToList();
                    var business_type = db.business_type.ToList();
                    var course_category = db.course_category.ToList();
                    var educational = db.educational.ToList();
                    var faq_type = db.faq_type.ToList();
                    var know_channel = db.know_channel.ToList();
                    var student_account_type = db.student_account_type.ToList();
                    var problem_of_use = db.problem_of_use.ToList();

                    output = new
                    {
                        career,
                        business_type,
                        course_category,
                        educational,
                        faq_type,
                        know_channel,
                        student_account_type,
                        problem_of_use
                    };
                }
                catch (Exception ex)
                {
                    errorMsg= ex.Message;ErrorList(ex);
                }

            }
            return output;
        }
        #endregion

        public dynamic get_all_site(ref string error)
        {
            dynamic output = new List<object>();

            try
            {
                using (DataContext db = new DataContext())
                {
                    var recommend_site = db.recommend_site.ToList();
                    if(recommend_site.Count() > 0)
                    {
                        var data = new List<object>();
                        data.Add(recommend_site.Select(s => new { name = s.name1, link = s.link1, cover = s.cover1 }).FirstOrDefault());
                        data.Add(recommend_site.Select(s => new { name = s.name2, link = s.link2, cover = s.cover2 }).FirstOrDefault());
                        data.Add(recommend_site.Select(s => new { name = s.name3, link = s.link3, cover = s.cover3 }).FirstOrDefault());
                        data.Add(recommend_site.Select(s => new { name = s.name4, link = s.link4, cover = s.cover4 }).FirstOrDefault());
                        output = data;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message; ErrorList(ex);
            }

            return output;
        }

        public dynamic get_all_news(ref string error)
        {
            dynamic output = new List<object>();

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.public_new.Where(w => w.is_deleted == 0 && w.active == 1).OrderByDescending(o => o.update_at).ToList();
                    output = data;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message; ErrorList(ex);
            }

            return output;
        }

        public dynamic get_special_days(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.special_days.Where(w => w.is_deleted == 0 && w.start_date <= DateTime.Now && w.end_date >= DateTime.Now).FirstOrDefault();
                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
                }
                return output;
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
        private string CreateCertificateID(int? certificate_id)
        {
            string new_cert_id = "";
            var count_ = certificate_id.ToString().Length;
            if (count_ == 3)
            {
                new_cert_id = certificate_id.ToString();

            }
            else if (count_ == 2)
            {
                new_cert_id = $"0{certificate_id}";
            }
            else if (count_ == 1)
            {
                new_cert_id = $"00{certificate_id}";
            }
            return new_cert_id;
        }

    }
}
