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
using System.Globalization;

namespace DpimProject.Models.Course
{
    public class Course
    {
        private DataTools.DataTools Dtl;
        private readonly DataContext db;
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public Course()
        {
            db = new DataContext();
            Dtl = new DataTools.DataTools();
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        #region Admin
        public dynamic get_all_course(ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_lesson_course = db.course_lesson.Where(w => w.is_deleted == 0);
                output = (from c in db.course
                          where c.is_deleted == 0
                          select new
                          {
                              id = c.id,
                              name = c.name,
                              batch = c.batch,
                              cover_img = c.cover_pic,
                              count_lesson = get_lesson_course.Where(w => w.course_id == c.id).Count(),
                              lesson_time = get_lesson_course.Where(w => w.course_id == c.id).Sum(s => s.lesson_time)
                          }).ToList();
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic new_batch_course(string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_course = db.course.Where(w => w.id == course_id && w.is_deleted == 0).FirstOrDefault();
                if (get_course == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                #region Add course
                DataTools.DataTools Dtl = new DataTools.DataTools();

                string gen_course_id = Dtl.CreateCourseID(db, get_course.department_id, get_course.course_category_id);

                course course = new course();
                course.id = gen_course_id;
                course.department_id = get_course.department_id;
                course.passed_percent = get_course.passed_percent;
                course.course_category_id = get_course.course_category_id;
                course.name = get_course.name;
                course.sub_name = get_course.sub_name;
                course.cover_pic = get_course.cover_pic;
                course.info_cover = get_course.info_cover;
                course.is_learning_online = get_course.is_learning_online;
                course.is_has_cost = get_course.is_has_cost;
                course.cost = get_course.cost;
                course.overview_course = get_course.overview_course;
                course.objective_course = get_course.objective_course;
                course.print_count = 0;
                course.benefits = get_course.benefits;
                course.batch = get_course.batch + 1;
                course.hasCertificate = get_course.hasCertificate;
                course.isAlwaysRegister = get_course.isAlwaysRegister;
                course.register_start_date = get_course.register_start_date;
                course.register_end_date = get_course.register_end_date;
                course.is_always_learning = get_course.is_always_learning;
                course.learning_startdate = get_course.learning_startdate;
                course.learning_enddate = get_course.learning_enddate;
                course.video_sample = get_course.video_sample;
                course.p_480 = get_course.p_480;
                course.p_720 = get_course.p_720;
                course.p_1080 = get_course.p_1080;
                course.p_original = get_course.p_original;
                course.cover_video = get_course.cover_video;
                course.contact_name = get_course.contact_name;
                course.contact_phone = get_course.contact_phone;
                course.contact_email = get_course.contact_email;
                course.is_deleted = 0;
                course.created_by = auth.user_id;
                course.created_dt = now;
                course.update_by = auth.user_id;
                course.update_dt = now;

                db.course.Add(course);
                db.SaveChanges();

                foreach(var item_permission in db.content_permission.Where(w => w.course_id == course_id).ToList())
                {
                    content_permission content_permission = new content_permission();
                    content_permission.course_id = gen_course_id;
                    content_permission.student_type_permission = item_permission.student_type_permission;

                    db.content_permission.Add(content_permission);
                    db.SaveChanges();
                }
                
                var get_course_id = course.id;
                #endregion

                #region Add lesson and exercise
                var get_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == course_id).ToList();
                foreach (var item_lesson in get_lesson)
                {
                    course_lesson course_lesson = new course_lesson();
                    course_lesson.course_id = get_course_id;
                    course_lesson.instructor_id = item_lesson.instructor_id;
                    course_lesson.attachment = item_lesson.attachment;
                    course_lesson.order = item_lesson.order;
                    course_lesson.name = item_lesson.name;
                    course_lesson.main_video = item_lesson.main_video;
                    course_lesson.main_p_480 = item_lesson.main_p_480;
                    course_lesson.main_p_720 = item_lesson.main_p_720;
                    course_lesson.main_p_1080 = item_lesson.main_p_1080;
                    course_lesson.main_p_original = item_lesson.main_p_original;
                    course_lesson.main_cover_video = item_lesson.main_cover_video;
                    course_lesson.count_view = 0;
                    course_lesson.is_interactive = item_lesson.is_interactive;
                    course_lesson.description = item_lesson.description;
                    course_lesson.lesson_time = item_lesson.lesson_time;
                    course_lesson.is_deleted = 0;
                    course_lesson.created_by = auth.user_id;
                    course_lesson.created_dt = now;
                    course_lesson.update_by = auth.user_id;
                    course_lesson.update_dt = now;

                    db.course_lesson.Add(course_lesson);
                    db.SaveChanges();

                    var lesson_id = course_lesson.id;
                    
                    for (var i = 0; i < 5; i++)
                    {
                        var file = new file_upload
                        {
                            itemno = (db.file_upload.Count() + 1),
                            filepath = (i == 0) ? item_lesson.main_p_480 : (i == 1) ? item_lesson.main_p_720 : (i == 2) ? item_lesson.main_p_1080 : (i == 3) ? item_lesson.main_p_original : item_lesson.main_cover_video,
                            main_id = lesson_id.ToString(),
                            seconde_id = get_course_id.ToString(),
                            third_id = course_lesson.order.ToString()
                        };
                        db.file_upload.Add(file);
                        db.SaveChanges();
                    }

                    var get_interactive = db.interactive_question.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.course_lesson_id == item_lesson.id).ToList();
                    #region Add interactive
                    foreach (var item_interactive in get_interactive)
                    {
                        interactive_question interactive_question = new interactive_question();
                        interactive_question.course_id = get_course_id;
                        interactive_question.course_lesson_id = lesson_id;
                        interactive_question.name = item_interactive.name;
                        interactive_question.order = item_interactive.order;
                        interactive_question.interactive_time = item_interactive.interactive_time;
                        interactive_question.is_deleted = 0;
                        interactive_question.created_by = auth.user_id;
                        interactive_question.created_at = now;
                        interactive_question.update_by = auth.user_id;
                        interactive_question.update_at = now;

                        db.interactive_question.Add(interactive_question);
                        db.SaveChanges();

                        var interactive_id = interactive_question.id;
                        var get_interactive_answer = db.interactive_answer.Where(w => w.course_lesson_id == item_lesson.id && w.interactive_question_id == item_interactive.id).ToList();
                        foreach (var item_interactive_answer in get_interactive_answer)
                        {
                            interactive_answer interactive_answer = new interactive_answer();
                            interactive_answer.interactive_question_id = interactive_id;
                            interactive_answer.course_lesson_id = lesson_id;
                            interactive_answer.name = item_interactive_answer.name;
                            interactive_answer.order = item_interactive_answer.order;
                            interactive_answer.correct = item_interactive_answer.correct;

                            db.interactive_answer.Add(interactive_answer);
                            db.SaveChanges();
                        }
                    }
                    #endregion

                    var get_exercise = db.course_lesson_exercise.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.course_lesson_id == item_lesson.id).ToList();

                    #region Add exercise
                    foreach (var item_exercise in get_exercise)
                    {
                        course_lesson_exercise course_lesson_exercise = new course_lesson_exercise();
                        course_lesson_exercise.course_id = get_course_id;
                        course_lesson_exercise.order = item_exercise.order;
                        course_lesson_exercise.course_lesson_id = lesson_id;
                        course_lesson_exercise.question = item_exercise.question;
                        course_lesson_exercise.image = item_exercise.image;
                        course_lesson_exercise.video = item_exercise.video;
                        course_lesson_exercise.p_480 = item_exercise.p_480;
                        course_lesson_exercise.p_720 = item_exercise.p_720;
                        course_lesson_exercise.p_1080 = item_exercise.p_1080;
                        course_lesson_exercise.p_original = item_exercise.p_original;
                        course_lesson_exercise.cover_video = item_exercise.cover_video;
                        course_lesson_exercise.is_answer_match = item_exercise.is_answer_match;
                        course_lesson_exercise.is_answer_choice = item_exercise.is_answer_choice;
                        course_lesson_exercise.is_deleted = 0;
                        course_lesson_exercise.created_by = auth.user_id;
                        course_lesson_exercise.created_dt = now;
                        course_lesson_exercise.update_by = auth.user_id;
                        course_lesson_exercise.update_dt = now;

                        db.course_lesson_exercise.Add(course_lesson_exercise);
                        db.SaveChanges();

                        var lesson_exercise_id = course_lesson_exercise.id;
                        if (course_lesson_exercise.is_answer_match == 1)
                        {
                            var get_exercise_match = db.course_lesson_exercise_answer_match.Where(w => w.course_lesson_id == item_lesson.id && w.course_lesson_exercise_id == item_exercise.id).ToList();
                            foreach (var item_exercise_match in get_exercise_match)
                            {
                                course_lesson_exercise_answer_match course_lesson_exercise_answer_match = new course_lesson_exercise_answer_match();
                                course_lesson_exercise_answer_match.course_lesson_exercise_id = lesson_exercise_id;
                                course_lesson_exercise_answer_match.course_lesson_id = lesson_id;
                                course_lesson_exercise_answer_match.question = item_exercise_match.question;
                                course_lesson_exercise_answer_match.answer = item_exercise_match.answer;

                                db.course_lesson_exercise_answer_match.Add(course_lesson_exercise_answer_match);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var get_exercise_choices = db.course_lesson_exercise_answer_choices.Where(w => w.course_lesson_id == item_lesson.id && w.course_lesson_exercise_id == item_exercise.id).ToList();
                            foreach (var item_exercise_choices in get_exercise_choices)
                            {
                                course_lesson_exercise_answer_choices course_lesson_exercise_answer_choices = new course_lesson_exercise_answer_choices();
                                course_lesson_exercise_answer_choices.course_lesson_exercise_id = lesson_exercise_id;
                                course_lesson_exercise_answer_choices.course_lesson_id = lesson_id;
                                course_lesson_exercise_answer_choices.answer = item_exercise_choices.answer;
                                course_lesson_exercise_answer_choices.correct = item_exercise_choices.correct;
                                course_lesson_exercise_answer_choices.order = item_exercise_choices.order;

                                db.course_lesson_exercise_answer_choices.Add(course_lesson_exercise_answer_choices);
                                db.SaveChanges();
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                
                #region Add exam
                var get_exam = db.course_exam.Where(w => w.is_deleted == 0 && w.course_id == course_id).ToList();
                foreach (var item_exam in get_exam)
                {
                    course_exam course_exam = new course_exam();
                    course_exam.course_id = get_course_id;
                    course_exam.order = item_exam.order;
                    course_exam.question = item_exam.question;
                    course_exam.image = item_exam.image;
                    course_exam.video = item_exam.video;
                    course_exam.p_480 = item_exam.p_480;
                    course_exam.p_720 = item_exam.p_720;
                    course_exam.p_1080 = item_exam.p_1080;
                    course_exam.p_original = item_exam.p_original;
                    course_exam.cover_video = item_exam.cover_video;
                    course_exam.percent_pass = item_exam.percent_pass;
                    course_exam.score = item_exam.score;
                    course_exam.answer = item_exam.answer;
                    course_exam.is_deleted = 0;
                    course_exam.created_by = auth.user_id;
                    course_exam.created_dt = now;
                    course_exam.update_by = auth.user_id;
                    course_exam.update_dt = now;

                    db.course_exam.Add(course_exam);
                    db.SaveChanges();

                    var exam_id = course_exam.id;

                    var get_exam_answer = db.course_exam_answer.Where(w => w.course_id == course_id && w.course_exam_id == item_exam.id).ToList();
                    foreach (var item_course_exam_answer in get_exam_answer)
                    {
                        course_exam_answer course_exam_answer = new course_exam_answer();
                        course_exam_answer.course_exam_id = exam_id;
                        course_exam_answer.course_id = get_course_id;
                        course_exam_answer.correct = item_course_exam_answer.correct;
                        course_exam_answer.order = item_course_exam_answer.order;
                        course_exam_answer.answer = item_course_exam_answer.answer;

                        db.course_exam_answer.Add(course_exam_answer);
                        db.SaveChanges();
                    }
                }
                #endregion

                #region Add Evaluation
                var get_evaluation = db.course_evaluation.Where(w => w.is_deleted == 0 && w.course_id == course_id).ToList();
                foreach (var item_eva in get_evaluation)
                {
                    course_evaluation course_evaluation = new course_evaluation();
                    course_evaluation.course_id = get_course_id;
                    course_evaluation.order = item_eva.order;
                    course_evaluation.question = item_eva.question;
                    course_evaluation.is_free_fill_text = item_eva.is_free_fill_text;
                    course_evaluation.free_fill_text = item_eva.free_fill_text;

                    course_evaluation.is_deleted = 0;
                    course_evaluation.created_by = auth.user_id;
                    course_evaluation.created_dt = now;
                    course_evaluation.update_by = auth.user_id;
                    course_evaluation.update_dt = now;

                    db.course_evaluation.Add(course_evaluation);
                    db.SaveChanges();

                    var get_eva_id = course_evaluation.id;
                    var get_eva_choices = db.course_evaluation_choices.Where(w => w.course_evaluation_id == item_eva.id).ToList();
                    foreach (var item_eva_choices in get_eva_choices)
                    {
                        course_evaluation_choices course_evaluation_choices = new course_evaluation_choices();
                        course_evaluation_choices.course_evaluation_id = get_eva_id;
                        course_evaluation_choices.choice = item_eva_choices.choice;
                        course_evaluation_choices.score = item_eva_choices.score;
                        course_evaluation_choices.order = item_eva_choices.order;

                        db.course_evaluation_choices.Add(course_evaluation_choices);
                        db.SaveChanges();
                    }
                }
                #endregion

                output = new
                {
                    course = course
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        #region Course Overview
        public dynamic remove_course(string id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var data = db.course.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (data != null)
                {
                    data.is_deleted = 1;
                    data.update_dt = now;
                    data.update_by = auth.user_id;
                    db.SaveChanges();

                    foreach (var deleted in db.content_permission.Where(w => w.course_id == id).ToList())
                    {
                        db.content_permission.Remove(deleted);
                        db.SaveChanges();
                    }

                    output = new
                    {
                        data
                    };
                }
                else
                {
                    error_message = "Id Invalid";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic get_overview_course(string id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_data = db.course.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (get_data == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                output = new model_course
                {
                    id = get_data.id,
                    batch = get_data.batch,
                    passed_percent = get_data.passed_percent,
                    benefits = get_data.benefits,
                    categoryId = get_data.course_category_id,
                    departmentId = get_data.department_id,
                    contactEmail = get_data.contact_email,
                    contactName = get_data.contact_name,
                    contactPhone = get_data.contact_phone,
                    cost = get_data.cost,
                    cover = get_data.cover_pic,
                    info_cover = get_data.info_cover,
                    hasCost = get_data.is_has_cost == 1 ? true : false,
                    hasCertificate = get_data.hasCertificate == 1 ? true : false,
                    isAlwaysRegister = get_data.isAlwaysRegister == 1 ? true : false,
                    register_start_date = get_data.register_start_date,
                    register_end_date = get_data.register_end_date,
                    isAlwaysLearning = get_data.is_always_learning == 1 ? true : false,
                    learning_enddate = get_data.learning_enddate,
                    learning_startdate = get_data.learning_startdate,
                    name = get_data.name,
                    sub_name = get_data.sub_name,
                    objective = get_data.objective_course,
                    overview = get_data.overview_course,
                    type = get_data.is_learning_online,
                    content_permission = db.content_permission.Where(w => w.course_id == get_data.id).Select(s => s.student_type_permission).ToList(),
                    video = new video
                    {
                        p_480 = get_data.p_480,
                        p_720 = get_data.p_720,
                        p_1080 = get_data.p_1080,
                        original = get_data.p_original,
                        thumbnail = get_data.cover_video,
                    }
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic save_add_course(model_course data, Authentication.Authentication auth, ref string error_message)
        {
            DataTools.DataTools Dtl = new DataTools.DataTools();

            string course_id = Dtl.CreateCourseID(db, data.departmentId, data.categoryId);
            dynamic output = null;
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                course course = new course();
                course.id = course_id;
                course.department_id = data.departmentId;
                course.passed_percent = data.passed_percent;
                course.batch = data.batch;
                course.benefits = data.benefits;
                course.course_category_id = data.categoryId;
                course.contact_email = data.contactEmail;
                course.contact_name = data.contactName;
                course.contact_phone = data.contactPhone;
                course.cost = data.cost;
                course.cover_pic = data.cover;
                course.info_cover = data.info_cover;
                course.is_has_cost = data.hasCost == true ? 1 : 0;
                course.hasCertificate = data.hasCertificate == true ? 1 : 0;
                course.isAlwaysRegister = data.isAlwaysRegister == true ? 1 : 0;
                course.register_start_date = data.isAlwaysRegister == true ? (DateTime?)null : DateTime.ParseExact(data.register_start_date.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.register_start_date;
                course.register_end_date = data.isAlwaysRegister == true ? (DateTime?)null : DateTime.ParseExact(data.register_end_date.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.register_end_date;
                course.is_always_learning = data.isAlwaysLearning == true ? 1 : 0;
                course.learning_enddate = data.isAlwaysLearning == true ? (DateTime?)null : DateTime.ParseExact(data.learning_enddate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.learning_enddate;
                course.learning_startdate = data.isAlwaysLearning == true ? (DateTime?)null : DateTime.ParseExact(data.learning_startdate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.learning_startdate;
                course.name = data.name;
                course.sub_name = data.sub_name;
                course.objective_course = data.objective;
                course.overview_course = data.overview;
                course.is_learning_online = data.type;

                var is_video = (data.video == null || data.video.original == null) ? false : true;

                course.video_sample = is_video ? data.video.original : "";
                course.p_480 = is_video ? data.video.p_480 : "";
                course.p_720 = is_video ? data.video.p_720 : "";
                course.p_1080 = is_video ? data.video.p_1080 : "";
                course.p_original = is_video ? data.video.original : "";
                course.cover_video = is_video ? data.video.thumbnail : "";

                course.print_count = 0;
                course.is_deleted = 0;
                course.created_by = auth.user_id;
                course.created_dt = now;
                course.update_by = auth.user_id;
                course.update_dt = now;
                db.course.Add(course);
                db.SaveChanges();

                foreach (var item in data.content_permission)
                {
                    content_permission content_permission = new content_permission();
                    content_permission.course_id = course_id;
                    content_permission.student_type_permission = item;

                    db.content_permission.Add(content_permission);
                    db.SaveChanges();
                }

                output = course;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic save_edit_course(model_course data, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            bool check_learning_enddate = false;
            try
            {
                var get_data = db.course.Where(x => x.id == data.id).FirstOrDefault();
                CultureInfo provider = CultureInfo.InvariantCulture;
                if (get_data != null)
                {
                    DateTime? learning_startdate = data.isAlwaysLearning == true ? (DateTime?)null : DateTime.ParseExact(data.learning_startdate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.learning_startdate;
                    DateTime? learning_enddate = data.isAlwaysLearning == true ? (DateTime?)null : DateTime.ParseExact(data.learning_enddate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.learning_enddate;
                    check_learning_enddate = get_data.learning_enddate == learning_enddate ? true : false;

                    get_data.passed_percent = data.passed_percent;
                    get_data.course_category_id = data.categoryId;
                    get_data.name = data.name;
                    get_data.sub_name = data.sub_name;
                    get_data.cover_pic = data.cover == null ? get_data.cover_pic : data.cover;
                    get_data.info_cover = data.info_cover == null ? get_data.info_cover : data.info_cover;
                    get_data.is_learning_online = data.type;
                    get_data.is_has_cost = data.hasCost == true ? 1 : 0;
                    get_data.cost = data.hasCost == true ? data.cost : 0;
                    get_data.hasCertificate = data.hasCertificate == true ? 1 : 0;
                    get_data.isAlwaysRegister = data.isAlwaysRegister == true ? 1 : 0;
                    get_data.register_start_date = data.isAlwaysRegister == true ? (DateTime?)null : DateTime.ParseExact(data.register_start_date.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.register_start_date;
                    get_data.register_end_date = data.isAlwaysRegister == true ? (DateTime?)null : DateTime.ParseExact(data.register_end_date.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);//data.register_end_date;
                    get_data.overview_course = data.overview;
                    get_data.objective_course = data.objective;
                    get_data.benefits = data.benefits;
                    get_data.batch = data.batch;
                    get_data.is_always_learning = data.isAlwaysLearning == true ? 1 : 0;
                    get_data.learning_startdate = data.isAlwaysLearning == true ? (DateTime?)null : learning_startdate;
                    get_data.learning_enddate = data.isAlwaysLearning == true ? (DateTime?)null : learning_enddate;

                    var is_video = (data.video == null || data.video.original == null) ? false : true;
                    if (is_video)
                    {
                        get_data.video_sample = data.video.original;
                        get_data.p_480 = data.video.p_480;
                        get_data.p_720 = data.video.p_720;
                        get_data.p_1080 = data.video.p_1080;
                        get_data.p_original = data.video.original;
                        get_data.cover_video = data.video.thumbnail;
                    }

                    get_data.contact_name = data.contactName;
                    get_data.contact_phone = data.contactPhone;
                    get_data.contact_email = data.contactEmail;
                    get_data.update_by = auth.user_id;
                    get_data.update_dt = now;
                    db.SaveChanges();
                    output = get_data;

                    #region remove old permission
                    foreach (var deleted in db.content_permission.Where(w => w.course_id == get_data.id).ToList())
                    {
                        db.content_permission.Remove(deleted);
                        db.SaveChanges();
                    }
                    #endregion

                    #region add new permission
                    foreach (var item in data.content_permission)
                    {
                        content_permission content_permission = new content_permission();
                        content_permission.course_id = get_data.id;
                        content_permission.student_type_permission = item;

                        db.content_permission.Add(content_permission);
                        db.SaveChanges();
                    }
                    #endregion

                    if (!check_learning_enddate)
                    {
                        change_learning_enddate(learning_startdate, learning_enddate, get_data.id);
                    }
                }
                else
                {
                    error_message = "Id Invalid";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion

        #region Course Lesson
        //success
        public dynamic get_all_lesson(string course_id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                output = db.course_lesson.Where(w => w.course_id == course_id && w.is_deleted == 0).OrderBy(o => o.order).ToList();
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic get_lesson_by_id(int id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_data = db.course_lesson.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (get_data == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                output = new
                {
                    id = get_data.id,
                    course_id = get_data.course_id,
                    order = get_data.order,
                    name = get_data.name,
                    main_video = new video
                    {
                        p_480 = get_data.main_p_480,
                        p_720 = get_data.main_p_720,
                        p_1080 = get_data.main_p_1080,
                        original = get_data.main_p_original,
                        thumbnail = get_data.main_cover_video
                    },
                    lesson_time = get_data.lesson_time,
                    description = get_data.description,
                    instructor_id = get_data.instructor_id,
                    attachment = get_data.attachment
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic remove_course_lesson(int id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var data = db.course_lesson.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (data != null)
                {
                    data.is_deleted = 1;
                    data.update_dt = now;
                    data.update_by = auth.user_id;
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
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic save_add_course_lesson(model_course_lesson data, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                course_lesson course_lesson = new course_lesson();

                //var check_order = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == data.course_id && w.order == data.order).Count();
                //if (check_order > 0)
                //{
                //    foreach (var item in db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == data.course_id && w.order >= data.order).ToList())
                //    {
                //        item.order = (item.order + 1);
                //        db.SaveChanges();
                //    }
                //}

                course_lesson.course_id = data.course_id;
                course_lesson.instructor_id = data.instructor_id;
                course_lesson.order = data.order;
                course_lesson.name = data.name;
                course_lesson.attachment = data.attachment;

                var is_main_video = (data.main_video == null || data.main_video.original == null) ? false : true;
                course_lesson.main_video = is_main_video ? data.main_video.original : "";
                course_lesson.main_p_480 = is_main_video ? data.main_video.p_480 : "";
                course_lesson.main_p_720 = is_main_video ? data.main_video.p_720 : "";
                course_lesson.main_p_1080 = is_main_video ? data.main_video.p_1080 : "";
                course_lesson.main_p_original = is_main_video ? data.main_video.original : "";
                course_lesson.main_cover_video = is_main_video ? data.main_video.thumbnail : "";
                
                course_lesson.description = data.description;
                course_lesson.lesson_time = data.lesson_time;

                course_lesson.count_view = 0;
                course_lesson.is_deleted = 0;
                course_lesson.created_by = auth.user_id;
                course_lesson.created_dt = now;
                course_lesson.update_by = auth.user_id;
                course_lesson.update_dt = now;
                db.course_lesson.Add(course_lesson);
                db.SaveChanges();

                for (var i = 0; i < 5; i++)
                {
                    var file = new file_upload
                    {
                        itemno = db.file_upload.ToList().Count() + 1,
                        filepath = (i == 0) ? course_lesson.main_p_480 : (i == 1) ? course_lesson.main_p_720 : (i == 2) ? course_lesson.main_p_1080 : (i == 3) ? course_lesson.main_p_original : course_lesson.main_cover_video,
                        main_id = course_lesson.id.ToString(),
                        seconde_id = course_lesson.course_id.ToString(),
                        third_id= course_lesson.order.ToString()

                    };
                    db.file_upload.Add(file);
                    db.SaveChanges();
                }

                output = course_lesson;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic save_edit_course_lesson(model_course_lesson data, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_data = db.course_lesson.Where(w => w.id == data.id && w.is_deleted == 0).FirstOrDefault();
                if (get_data != null)
                {
                    //var check_order = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == data.course_id && w.order == data.order).Count();
                    //if (check_order > 0)
                    //{
                    //    foreach (var item in db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == data.course_id && w.order >= data.order).ToList())
                    //    {
                    //        item.order = (item.order + 1);
                    //        db.SaveChanges();
                    //    }
                    //}

                    get_data.instructor_id = data.instructor_id;
                    get_data.order = data.order;
                    get_data.name = data.name;
                    get_data.attachment = (data.attachment == null ? get_data.attachment : data.attachment);

                    var is_main_video = (data.main_video == null || data.main_video.original == null) ? false : true;
                    if (is_main_video)
                    {
                        get_data.main_video = data.main_video.original;
                        get_data.main_p_480 = data.main_video.p_480;
                        get_data.main_p_720 = data.main_video.p_720;
                        get_data.main_p_1080 = data.main_video.p_1080;
                        get_data.main_p_original = data.main_video.original;
                        get_data.main_cover_video = data.main_video.thumbnail;


                        var d1 = db.file_upload.Where(x => x.main_id == data.course_id.ToString() && x.seconde_id == data.id.ToString() && x.third_id == data.order.ToString()).FirstOrDefault();

                        if (d1 != null)
                        {
                            db.file_upload.RemoveRange(db.file_upload.Where(x => x.main_id == data.course_id.ToString() && x.seconde_id == data.id.ToString()).ToList());
                            db.SaveChanges();
                        }
                        
                        for (var i = 0; i < 5; i++)
                        {
                            var file = new file_upload
                            {
                                itemno = db.file_upload.ToList().Count() + 1,
                                filepath = (i == 0) ? get_data.main_p_480 : (i == 1) ? get_data.main_p_720 : (i == 2) ? get_data.main_p_1080 : (i == 3) ? get_data.main_p_original : get_data.main_cover_video,
                                main_id = get_data.id.ToString(),
                                seconde_id = get_data.course_id.ToString(),
                                third_id = get_data.order.ToString()

                            };
                            db.file_upload.Add(file);
                            db.SaveChanges();
                        }
                    }

                    get_data.description = data.description;
                    get_data.lesson_time = data.lesson_time;
                    get_data.update_by = auth.user_id;
                    get_data.update_dt = now;
                    db.SaveChanges();
                    output = data; 
                }
                else
                {
                    error_message = "Id Invalid";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion

        #region Course Lesson Interactive
        public dynamic get_all_lesson_interactive(int lesson_id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_answer = db.interactive_answer.Where(w => w.course_lesson_id == lesson_id);
                output = (from i in db.interactive_question
                          where i.course_lesson_id == lesson_id && i.is_deleted == 0
                          select new
                          {
                              id = i.id,
                              course_id = i.course_id,
                              course_lesson_id = i.course_lesson_id,
                              order = i.order,
                              question = i.name,
                              interactive_time = i.interactive_time,
                              answer = get_answer.Where(w => w.interactive_question_id == i.id).Select(s => new
                              {
                                  id = s.id,
                                  interactive_question_id = s.interactive_question_id,
                                  course_lesson_id = s.course_lesson_id,
                                  answer = s.name,
                                  order = s.order,
                                  correct = s.correct,
                              }).OrderBy(o => o.order).ToList()
                          }).OrderBy(o => o.order).ToList();
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic remove_lesson_interactive(int id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var data = db.interactive_question.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (data != null)
                {
                    var course_lesson = data.course_lesson_id;
                    data.is_deleted = 1;
                    data.update_at = now;
                    data.update_by = auth.user_id;
                    db.SaveChanges();

                    #region Delete answer
                    foreach (var get_answer in db.interactive_answer.Where(w => w.interactive_question_id == id).ToList())
                    {
                        db.interactive_answer.Remove(get_answer);
                        db.SaveChanges();
                    }
                    #endregion

                    var check_interactive = db.interactive_question.Where(w => w.course_lesson_id == course_lesson && w.is_deleted == 0).Count();
                    var get_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.id == course_lesson).FirstOrDefault();
                    if(get_lesson != null)
                    {
                        get_lesson.is_interactive = (check_interactive > 0 ? 1 : 0);
                        db.SaveChanges();
                    }

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
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_add_lesson_interactive(model_interactive_question interactive, List<model_interactive_answer> answer, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            dynamic list_answer = new List<object>();
            try
            {
                interactive_question interactive_question = new interactive_question();
                interactive_question.course_id = interactive.course_id;
                interactive_question.course_lesson_id = interactive.course_lesson_id;
                interactive_question.name = interactive.question;
                interactive_question.order = interactive.order;
                interactive_question.interactive_time = interactive.interactive_time;
                interactive_question.is_deleted = 0;
                interactive_question.created_by = auth.user_id;
                interactive_question.created_at = now;
                interactive_question.update_by = auth.user_id;
                interactive_question.update_at = now;
                db.interactive_question.Add(interactive_question);
                db.SaveChanges();

                var get_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.id == interactive.course_lesson_id).FirstOrDefault();
                if(get_lesson != null)
                {
                    get_lesson.is_interactive = 1;
                    db.SaveChanges();
                }

                var get_id = interactive_question.id;
                foreach (var item in answer)
                {
                    if (item.answer != "" && item.answer != null)
                    {
                        interactive_answer interactive_answer = new interactive_answer();
                        interactive_answer.interactive_question_id = get_id;
                        interactive_answer.course_lesson_id = interactive.course_lesson_id;
                        interactive_answer.name = item.answer;
                        interactive_answer.order = item.order;
                        interactive_answer.correct = item.correct;

                        db.interactive_answer.Add(interactive_answer);
                        db.SaveChanges();
                        list_answer.Add(item);
                    }
                }
                output = new
                {
                    interactive_question = interactive_question,
                    answer = list_answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_edit_lesson_interactive(model_interactive_question interactive, List<model_interactive_answer> answer, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            dynamic list_answer = new List<object>();
            try
            {
                var get_data = db.interactive_question.Where(w => w.id == interactive.id && w.is_deleted == 0).FirstOrDefault();
                if (get_data != null)
                {
                    get_data.name = interactive.question;
                    get_data.order = interactive.order;
                    get_data.interactive_time = interactive.interactive_time;
                    get_data.update_by = auth.user_id;
                    get_data.update_at = now;
                    db.SaveChanges();

                    var get_id = interactive.id;

                    var get_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.id == interactive.course_lesson_id).FirstOrDefault();
                    if (get_lesson != null)
                    {
                        get_lesson.is_interactive = 1;
                        db.SaveChanges();
                    }

                    #region Delete old answer
                    foreach (var get_answer in db.interactive_answer.Where(w => w.interactive_question_id == get_id).ToList())
                    {
                        db.interactive_answer.Remove(get_answer);
                        db.SaveChanges();
                    }
                    #endregion
                    
                    #region add new answer
                    foreach (var item in answer)
                    {
                        if (item.answer != "" && item.answer != null)
                        {
                            interactive_answer interactive_answer = new interactive_answer();
                            interactive_answer.interactive_question_id = get_id;
                            interactive_answer.course_lesson_id = get_data.course_lesson_id;
                            interactive_answer.name = item.answer;
                            interactive_answer.order = item.order;
                            interactive_answer.correct = item.correct;

                            db.interactive_answer.Add(interactive_answer);
                            db.SaveChanges();
                            list_answer.Add(item);
                        }
                    }
                    #endregion
                }
                else
                {
                    error_message = "Id Invalid";
                    return false;
                }

                output = new
                {
                    interactive_question = get_data,
                    answer = list_answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion

        #region Course Lesson Exercise
        //success
        public dynamic get_all_lesson_execrise(int lesson_id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_choices = db.course_lesson_exercise_answer_choices.Where(w => w.course_lesson_id == lesson_id);
                var get_match = db.course_lesson_exercise_answer_match.Where(w => w.course_lesson_id == lesson_id);

                output = (from lex in db.course_lesson_exercise
                          where lex.course_lesson_id == lesson_id && lex.is_deleted == 0
                          select new
                          {
                              id = lex.id,
                              course_id = lex.course_id,
                              order = lex.order,
                              course_lesson_id = lex.course_lesson_id,
                              question = lex.question,
                              image = (lex.image == null || lex.image == "") ? "" : lex.image,
                              video = new video
                              {
                                  p_480 = (lex.p_480 == null || lex.p_480 == "") ? "" : lex.p_480,
                                  p_720 = (lex.p_720 == null || lex.p_720 == "") ? "" : lex.p_720,
                                  p_1080 = (lex.p_1080 == null || lex.p_1080 == "") ? "" : lex.p_1080,
                                  original = (lex.p_original == null || lex.p_original == "") ? "" : lex.p_original,
                                  thumbnail = (lex.cover_video == null || lex.cover_video == "") ? "" : lex.cover_video,
                              },
                              is_answer_match = lex.is_answer_match,
                              is_answer_choice = lex.is_answer_choice,
                              choices = get_choices.Where(w => w.course_lesson_exercise_id == lex.id).ToList(),
                              match = get_match.Where(w => w.course_lesson_exercise_id == lex.id).ToList()
                          }).OrderBy(o => o.order).ToList();
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic remove_lesson_execrise(int id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var data = db.course_lesson_exercise.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (data != null)
                {
                    data.is_deleted = 1;
                    data.update_dt = now;
                    data.update_by = auth.user_id;
                    db.SaveChanges();

                    #region Delete old choices
                    foreach (var get_choices in db.course_lesson_exercise_answer_choices.Where(w => w.course_lesson_exercise_id == id).ToList())
                    {
                        db.course_lesson_exercise_answer_choices.Remove(get_choices);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Delete old match
                    foreach (var get_match in db.course_lesson_exercise_answer_match.Where(w => w.course_lesson_exercise_id == id).ToList())
                    {
                        db.course_lesson_exercise_answer_match.Remove(get_match);
                        db.SaveChanges();
                    }
                    #endregion

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
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic save_add_lesson_exercise(
            model_lesson_exercise lesson_exercise,
            List<course_lesson_exercise_answer_choices> answer_choices,
            List<course_lesson_exercise_answer_match> answer_match,
            bool is_choices,
            Authentication.Authentication auth,
            ref string error_message
            )
        {
            dynamic output = null;
            dynamic answer = null;
            try
            {
                course_lesson_exercise course_lesson_exercise = new course_lesson_exercise();
                course_lesson_exercise.course_id = lesson_exercise.course_id;
                course_lesson_exercise.order = lesson_exercise.order;
                course_lesson_exercise.course_lesson_id = lesson_exercise.course_lesson_id;
                course_lesson_exercise.question = lesson_exercise.question;
                course_lesson_exercise.image = lesson_exercise.image == null ? "" : lesson_exercise.image;

                var is_video = (lesson_exercise.video == null || lesson_exercise.video.original == null) ? false : true;
                course_lesson_exercise.video = is_video ? lesson_exercise.video.original : "";
                course_lesson_exercise.p_480 = is_video ? lesson_exercise.video.p_480 : "";
                course_lesson_exercise.p_720 = is_video ? lesson_exercise.video.p_720 : "";
                course_lesson_exercise.p_1080 = is_video ? lesson_exercise.video.p_1080 : "";
                course_lesson_exercise.p_original = is_video ? lesson_exercise.video.original : "";
                course_lesson_exercise.cover_video = is_video ? lesson_exercise.video.thumbnail : "";

                course_lesson_exercise.is_answer_match = lesson_exercise.is_answer_match;
                course_lesson_exercise.is_answer_choice = lesson_exercise.is_answer_choice;

                course_lesson_exercise.is_deleted = 0;
                course_lesson_exercise.created_by = auth.user_id;
                course_lesson_exercise.created_dt = now;
                course_lesson_exercise.update_by = auth.user_id;
                course_lesson_exercise.update_dt = now;
                db.course_lesson_exercise.Add(course_lesson_exercise);
                db.SaveChanges();

                var get_id = course_lesson_exercise.id;
                if (is_choices)
                {
                    foreach (var item in answer_choices)
                    {
                        if (item.answer != "" && item.answer != null)
                        {
                            item.course_lesson_id = lesson_exercise.course_lesson_id;
                            item.course_lesson_exercise_id = get_id;
                            db.course_lesson_exercise_answer_choices.Add(item);
                            db.SaveChanges();
                        }
                    }
                    answer = answer_choices;
                }
                else
                {
                    foreach (var item in answer_match)
                    {
                        if ((item.question != "" && item.question != null) && (item.answer != "" && item.answer != null))
                        {
                            item.course_lesson_id = lesson_exercise.course_lesson_id;
                            item.course_lesson_exercise_id = get_id;
                            db.course_lesson_exercise_answer_match.Add(item);
                            db.SaveChanges();
                        }
                    }
                    answer = answer_match;
                }


                output = new
                {
                    lesson_exercise = course_lesson_exercise,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success
        public dynamic save_edit_lesson_exercise(
            model_lesson_exercise lesson_exercise,
            List<course_lesson_exercise_answer_choices> answer_choices,
            List<course_lesson_exercise_answer_match> answer_match,
            bool is_choices,
            Authentication.Authentication auth,
            ref string error_message
            )
        {
            dynamic output = null;
            dynamic answer = null;
            try
            {
                var get_data = db.course_lesson_exercise.Where(w => w.id == lesson_exercise.id && w.is_deleted == 0).FirstOrDefault();
                if (get_data != null)
                {
                    get_data.question = lesson_exercise.question;
                    get_data.image = lesson_exercise.image == null ? get_data.image : lesson_exercise.image;
                    get_data.order = lesson_exercise.order;

                    var is_video = (lesson_exercise.video == null || lesson_exercise.video.original == null) ? false : true;
                    if (is_video)
                    {
                        get_data.video = lesson_exercise.video.original == "" ? "" : lesson_exercise.video.original;
                        get_data.p_480 = lesson_exercise.video.p_480 == "" ? "" : lesson_exercise.video.p_480;
                        get_data.p_720 = lesson_exercise.video.p_720 == "" ? "" : lesson_exercise.video.p_720;
                        get_data.p_1080 = lesson_exercise.video.p_1080 == "" ? "" : lesson_exercise.video.p_1080;
                        get_data.p_original = lesson_exercise.video.original == "" ? "" : lesson_exercise.video.original;
                        get_data.cover_video = lesson_exercise.video.thumbnail == "" ? "" : lesson_exercise.video.thumbnail;
                    }

                    get_data.is_answer_match = is_choices ? 0 : 1;
                    get_data.is_answer_choice = is_choices ? 1 : 0;

                    get_data.update_by = auth.user_id;
                    get_data.update_dt = now;
                    db.SaveChanges();

                    var get_id = lesson_exercise.id;

                    #region Delete old choices
                    foreach (var get_choices in db.course_lesson_exercise_answer_choices.Where(w => w.course_lesson_exercise_id == get_id).ToList())
                    {
                        db.course_lesson_exercise_answer_choices.Remove(get_choices);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Delete old match
                    foreach (var get_match in db.course_lesson_exercise_answer_match.Where(w => w.course_lesson_exercise_id == get_id).ToList())
                    {
                        db.course_lesson_exercise_answer_match.Remove(get_match);
                        db.SaveChanges();
                    }
                    #endregion


                    if (is_choices)
                    {
                        #region add new choices
                        foreach (var item in answer_choices)
                        {
                            if (item.answer != "" && item.answer != null)
                            {
                                item.course_lesson_id = lesson_exercise.course_lesson_id;
                                item.course_lesson_exercise_id = get_id;
                                db.course_lesson_exercise_answer_choices.Add(item);
                                db.SaveChanges();
                            }
                        }
                        answer = answer_choices;
                        #endregion
                    }
                    else
                    {
                        #region add new match
                        foreach (var item in answer_match)
                        {
                            if ((item.question != "" && item.question != null) && (item.answer != "" && item.answer != null))
                            {
                                item.course_lesson_id = lesson_exercise.course_lesson_id;
                                item.course_lesson_exercise_id = get_id;
                                db.course_lesson_exercise_answer_match.Add(item);
                                db.SaveChanges();
                            }
                        }
                        answer = answer_match;
                        #endregion
                    }
                }
                else
                {
                    error_message = "Id Invalid";
                    return false;
                }

                output = new
                {
                    lesson_exercise = get_data,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion

        #region Course Exam
        //success
        public dynamic get_all_exam(string course_id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                output = db.course_exam.Where(w => w.course_id == course_id && w.is_deleted == 0).OrderBy(o => o.order).ToList();
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic get_course_exam_by_id(int id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_data = db.course_exam.Where(w => w.id == id)
                                             .Select(s => new model_exam
                                             {
                                                 id = s.id,
                                                 course_id = s.course_id,
                                                 order = s.order,
                                                 question = s.question,
                                                 image = (s.image == null || s.image == "") ? "" : s.image,
                                                 video = new video
                                                 {
                                                     p_480 = (s.p_480 == null || s.p_480 == "") ? "" : s.p_480,
                                                     p_720 = (s.p_720 == null || s.p_720 == "") ? "" : s.p_720,
                                                     p_1080 = (s.p_1080 == null || s.p_1080 == "") ? "" : s.p_1080,
                                                     original = (s.p_original == null || s.p_original == "") ? "" : s.p_original,
                                                     thumbnail = (s.cover_video == null || s.cover_video == "") ? "" : s.cover_video
                                                 },
                                                 percent_pass = s.percent_pass,
                                                 score = s.score,
                                                 answer = s.answer
                                             }).FirstOrDefault();
                if (get_data == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                var answer = db.course_exam_answer.Where(w => w.course_exam_id == id).Select(s => new
                {
                    id = s.id,
                    course_exam_id = s.course_exam_id,
                    course_id = s.course_id,
                    correct = get_data.answer == s.order ? 1 : 0,
                    order = s.order,
                    answer = s.answer
                }).ToList();

                output = new
                {
                    exam = get_data,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic remove_course_exam(int id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var data = db.course_exam.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (data != null)
                {
                    data.is_deleted = 1;
                    data.update_dt = now;
                    data.update_by = auth.user_id;
                    db.SaveChanges();

                    foreach (var item in db.course_exam_answer.Where(w => w.course_exam_id == id).ToList())
                    {
                        db.course_exam_answer.Remove(item);
                        db.SaveChanges();
                    }

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
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_add_course_exam(
            model_exam exam,
            List<course_exam_answer> answer,
            Authentication.Authentication auth,
            ref string error_message
            )
        {
            dynamic output = null;
            try
            {
                course_exam course_exam = new course_exam();

                course_exam.course_id = exam.course_id;
                course_exam.order = exam.order;
                course_exam.question = exam.question;
                course_exam.image = exam.image == null ? "" : exam.image;

                var is_video = (exam.video == null || exam.video.original == null) ? false : true;
                course_exam.video = is_video ? exam.video.original : "";
                course_exam.p_480 = is_video ? exam.video.p_480 : "";
                course_exam.p_720 = is_video ? exam.video.p_720 : "";
                course_exam.p_1080 = is_video ? exam.video.p_1080 : "";
                course_exam.p_original = is_video ? exam.video.original : "";
                course_exam.cover_video = is_video ? exam.video.thumbnail : "";

                course_exam.percent_pass = exam.order;
                course_exam.score = exam.order;
                course_exam.answer = exam.order;
                //course_exam.question_random = "";

                course_exam.is_deleted = 0;
                course_exam.created_by = auth.user_id;
                course_exam.created_dt = now;
                course_exam.update_by = auth.user_id;
                course_exam.update_dt = now;
                db.course_exam.Add(course_exam);
                db.SaveChanges();

                var get_id = course_exam.id;
                int? get_id_answer = 0;
                foreach (var item in answer)
                {
                    if (item.answer != "" && item.answer != null)
                    {
                        item.course_exam_id = get_id;
                        item.course_id = exam.course_id;
                        db.course_exam_answer.Add(item);
                        db.SaveChanges();

                        get_id_answer = get_id_answer == 0 ? (item.correct == 1 ? item.order : get_id_answer) : get_id_answer;
                    }
                }

                course_exam.answer = get_id_answer;
                db.SaveChanges();

                output = new
                {
                    exam = course_exam,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_edit_course_exam(
            model_exam exam,
            List<course_exam_answer> answer,
            Authentication.Authentication auth,
            ref string error_message
            )
        {
            dynamic output = null;
            try
            {
                var get_data = db.course_exam.Where(w => w.id == exam.id && w.is_deleted == 0).FirstOrDefault();
                if (get_data != null)
                {
                    get_data.question = exam.question;
                    get_data.image = exam.image == null ? get_data.image : exam.image;
                    get_data.order = exam.order;

                    var is_video = (exam.video == null || exam.video.original == null) ? false : true;
                    if (is_video)
                    {
                        get_data.video = exam.video.original == "" ? "" : exam.video.original;
                        get_data.p_480 = exam.video.p_480 == "" ? "" : exam.video.p_480;
                        get_data.p_720 = exam.video.p_720 == "" ? "" : exam.video.p_720;
                        get_data.p_1080 = exam.video.p_1080 == "" ? "" : exam.video.p_1080;
                        get_data.p_original = exam.video.original == "" ? "" : exam.video.original;
                        get_data.cover_video = exam.video.thumbnail == "" ? "" : exam.video.thumbnail;
                    }

                    get_data.update_by = auth.user_id;
                    get_data.update_dt = now;
                    db.SaveChanges();

                    var get_id = exam.id;
                    #region Delete old answer
                    foreach (var get_choices in db.course_exam_answer.Where(w => w.course_exam_id == get_id).ToList())
                    {
                        db.course_exam_answer.Remove(get_choices);
                        db.SaveChanges();
                    }
                    #endregion

                    #region add new answer
                    int? get_id_answer = 0;
                    foreach (var item in answer)
                    {
                        if (item.answer != "" && item.answer != null)
                        {
                            item.course_exam_id = get_id;
                            item.course_id = exam.course_id;
                            db.course_exam_answer.Add(item);
                            db.SaveChanges();

                            get_id_answer = get_id_answer == 0 ? (item.correct == 1 ? item.order : get_id_answer) : get_id_answer;
                        }
                    }
                    #endregion

                    get_data.answer = get_id_answer;
                    db.SaveChanges();
                }
                else
                {
                    error_message = "Id Invalid";
                    return false;
                }

                output = new
                {
                    exam = get_data,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion

        #region Course Evaluation
        public dynamic get_all_evaluation(string course_id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                output = db.course_evaluation.Where(w => w.course_id == course_id && w.is_deleted == 0).OrderBy(o => o.order).ToList();
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic get_evaluation_by_id(int id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_data = db.course_evaluation.Where(w => w.id == id).FirstOrDefault();
                if (get_data == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                var answer = db.course_evaluation_choices.Where(w => w.course_evaluation_id == id).ToList();

                output = new
                {
                    evaluation = get_data,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic remove_evaluation(int id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var data = db.course_evaluation.Where(w => w.id == id && w.is_deleted == 0).FirstOrDefault();
                if (data != null)
                {
                    data.is_deleted = 1;
                    data.update_dt = now;
                    data.update_by = auth.user_id;
                    db.SaveChanges();

                    #region Delete Choices
                    foreach (var item in db.course_evaluation_choices.Where(w => w.course_evaluation_id == id).ToList())
                    {
                        db.course_evaluation_choices.Remove(item);
                        db.SaveChanges();
                    }
                    #endregion

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
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_add_evaluation(
            course_evaluation evaluation,
            List<course_evaluation_choices> answer,
            Authentication.Authentication auth,
            ref string error_message
            )
        {
            dynamic output = null;
            try
            {
                evaluation.is_deleted = 0;
                evaluation.created_by = auth.user_id;
                evaluation.created_dt = now;
                evaluation.update_by = auth.user_id;
                evaluation.update_dt = now;
                db.course_evaluation.Add(evaluation);
                db.SaveChanges();

                var get_id = evaluation.id;
                foreach (var item in answer)
                {
                    if (item.choice != "" && item.choice != null)
                    {
                        item.course_evaluation_id = get_id;
                        db.course_evaluation_choices.Add(item);
                        db.SaveChanges();
                    }
                }

                output = new
                {
                    evaluation = evaluation,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_edit_evaluation(
            course_evaluation evaluation,
            List<course_evaluation_choices> answer,
            Authentication.Authentication auth,
            ref string error_message
            )
        {
            dynamic output = null;
            try
            {
                var get_data = db.course_evaluation.Where(w => w.id == evaluation.id && w.is_deleted == 0).FirstOrDefault();
                if (get_data != null)
                {
                    get_data.question = evaluation.question;
                    get_data.order = evaluation.order;
                    get_data.is_free_fill_text = evaluation.is_free_fill_text;
                    get_data.free_fill_text = evaluation.is_free_fill_text == 0 ? "" : evaluation.free_fill_text;
                    get_data.update_by = auth.user_id;
                    get_data.update_dt = now;
                    db.SaveChanges();

                    var get_id = evaluation.id;
                    #region Delete old answer
                    foreach (var get_answer in db.course_evaluation_choices.Where(w => w.course_evaluation_id == get_id).ToList())
                    {
                        db.course_evaluation_choices.Remove(get_answer);
                        db.SaveChanges();
                    }
                    #endregion

                    #region add new answer
                    foreach (var item in answer)
                    {
                        if (item.choice != "" && item.choice != null)
                        {
                            item.course_evaluation_id = get_id;
                            db.course_evaluation_choices.Add(item);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                }
                else
                {
                    error_message = "Id Invalid";
                    return false;
                }

                output = new
                {
                    evaluation = get_data,
                    answer = answer
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion

        #region Course Voucher
        public dynamic get_all_course_voucher(string courseId, int pageIndex, int pageSize, int? status, string orderBy, bool? desc, DateTime? startDate, DateTime? endDate, ref string error_message)
        {
            dynamic output = null;
            try
            {
                if (startDate != null && endDate != null && startDate == endDate)
                {
                    endDate = endDate.Value.AddDays(1);
                }

                var get_voucher = (startDate != null && endDate != null ? db.course_voucher.Where(w => w.is_delete == 0 && w.course_id == courseId && ((w.start_dt == null && w.end_dt == null) || (w.end_dt >= startDate && w.end_dt <= endDate))).ToList() : db.course_voucher.Where(w => w.course_id == courseId && w.is_delete == 0).ToList());

                var used = get_voucher.Where(w => w.status == 1).Count();
                var not_used = get_voucher.Where(w => w.status == 0).Count();

                get_voucher = (status == null ? get_voucher : get_voucher.Where(w => w.status == status).ToList());
                var data = get_voucher.Select(s => new
                {
                    id = s.id,
                    voucher_id = s.voucher_id,
                    course_id = s.course_id,
                    start_dt = s.start_dt,
                    end_dt = s.end_dt,
                    status = s.status
                }).ToList();
                
                #region order by
                switch (orderBy)
                {
                    case "voucher_id":
                        data = desc == true ? data.OrderByDescending(o => o.voucher_id).ToList() : data.OrderBy(o => o.voucher_id).ToList();
                        break;
                    case "status":
                        data = desc == true ? data.OrderByDescending(o => o.status).ToList() : data.OrderBy(o => o.status).ToList();
                        break;
                    case "start_dt":
                        data = desc == true ? data.OrderByDescending(o => o.start_dt).ToList() : data.OrderBy(o => o.start_dt).ToList();
                        break;
                    case "end_dt":
                        data = desc == true ? data.OrderByDescending(o => o.end_dt).ToList() : data.OrderBy(o => o.end_dt).ToList();
                        break;
                    default: break;
                }
                #endregion

                var skip = (pageIndex - 1) * pageSize;
                var count = data.Count();

                output = new
                {
                    data = data.Skip(skip).Take(pageSize).ToList(),
                    count = count,
                    used = used,
                    not_used = not_used
                };

            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_add_voucher(model_course_voucher data, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var check = db.course_voucher.Where(w => w.is_delete == 0 && w.course_id == data.course_id && w.voucher_id == data.voucher_id).FirstOrDefault();
                if(check != null)
                {
                    error_message = "Voucher ซ้ำกับในระบบ";
                    return false;
                }
                CultureInfo provider = CultureInfo.InvariantCulture;
                
                course_voucher course_voucher = new course_voucher();
                course_voucher.voucher_id = data.voucher_id;
                course_voucher.course_id = data.course_id;
                course_voucher.start_dt = data.start_dt == null ? (DateTime?)null : DateTime.ParseExact(data.start_dt.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);
                course_voucher.end_dt = data.end_dt == null ? (DateTime?)null : DateTime.ParseExact(data.end_dt.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);
                course_voucher.status = 0;
                course_voucher.is_delete = 0;
                course_voucher.created_by = auth.user_id;
                course_voucher.created_at = now;
                course_voucher.update_by = auth.user_id;
                course_voucher.update_dt = now;

                db.course_voucher.Add(course_voucher);
                db.SaveChanges();

                output = course_voucher;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_edit_voucher(model_course_voucher data, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_voucher = db.course_voucher.Where(w => w.is_delete == 0 && w.id == data.id).FirstOrDefault();
                if (get_voucher == null)
                {
                    error_message = "ไม่พบ Voucher ในระบบ";
                    return false;
                }

                CultureInfo provider = CultureInfo.InvariantCulture;
                get_voucher.voucher_id = data.voucher_id;
                get_voucher.start_dt = data.start_dt == null ? (DateTime?)null : DateTime.ParseExact(data.start_dt.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);
                get_voucher.end_dt = data.end_dt == null ? (DateTime?)null : DateTime.ParseExact(data.end_dt.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);
                get_voucher.update_by = auth.user_id;
                get_voucher.update_dt = now;

                db.SaveChanges();

                output = get_voucher;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic remove_voucher(int id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_voucher = db.course_voucher.Where(w => w.is_delete == 0 && w.id == id).FirstOrDefault();
                if (get_voucher == null)
                {
                    error_message = "ไม่พบ Voucher ในระบบ";
                    return false;
                }

                get_voucher.is_delete = 1;
                get_voucher.update_by = auth.user_id;
                get_voucher.update_dt = now;

                db.SaveChanges();

                output = get_voucher;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion

        #region Course Student
        public dynamic get_all_course_student(string courseId, int pageIndex, int pageSize, string search, string orderBy, bool desc, ref string error_message)
        {
            dynamic output = null;
            try
            {
                #region get data from db
                var count_lesson = db.course_lesson.Where(w => w.course_id == courseId && w.is_deleted == 0).Count();
                var get_exam_logging = db.course_exam_logging.Where(w => w.course_id == courseId && w.is_deleted == 0);
                var get_lesson_progress = db.student_course_lesson_progress.Where(w => w.course_id == courseId && w.is_done_exercise == 1 && w.is_done_lesson == 1);
                var get_account_type = db.student_account_type;
                var get_exam = db.course_exam.Where(w => w.is_deleted == 0 && w.course_id == courseId);
                var get_info_course = db.course.Where(w => w.id == courseId && w.is_deleted == 0).Select(s => new { name = s.name, batch = s.batch, percent_pass = s.passed_percent, sub_name = s.sub_name }).FirstOrDefault();
                #endregion
                
                var count_exam = Math.Floor(((double)get_info_course.percent_pass / 100) * get_exam.Count());

                var data = (from s_info in db.student_course_info
                            join s in db.student.Where(w => w.is_deleted == 0) on s_info.student_id equals s.student_id
                            where s_info.is_deleted == 0 && s_info.course_id == courseId
                            select new
                            {
                                id = s.student_id,
                                name = s.firstname + " " + s.lastname,
                                course_name = get_info_course.name,
                                sub_name = get_info_course.sub_name,
                                course_batch = get_info_course.batch,
                                email = s.email,
                                account_type = get_account_type.Where(w => w.id == s.student_account_type_id).Select(s => s.name).FirstOrDefault(),
                                progress = (count_lesson > 0 ? (((double)get_lesson_progress.Where(w => w.student_id == s.student_id).Count() / count_lesson) * 100) : 0),

                                score_pretest = get_exam_logging.Where(w => w.student_id == s.student_id &&
                                                                            w.is_deleted == 0 &&
                                                                            w.is_pretest == 1 &&
                                                                            (get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0)).Count(),


                                score_after_class = get_exam_logging.Where(w => w.student_id == s.student_id &&
                                                                                w.is_deleted == 0 &&
                                                                                w.is_pretest == 0 &&
                                                                                (get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0)).Count(),


                                last_test_at = get_exam_logging.Where(w => w.student_id == s.student_id && 
                                                                           w.is_deleted == 0 && 
                                                                           w.is_pretest == 0 && 
                                                                           w.score == 1).Select(s => s.score).Sum() >= count_exam ? get_exam_logging.Where(w => w.student_id == s.student_id && w.is_deleted == 0 && w.is_pretest == 0 && w.score == 1).OrderByDescending(o => o.created_dt).Select(s => s.created_dt).FirstOrDefault() : null,
                                create_at = s_info.created_dt,
                                total_exam = get_exam.Count()
                            }).ToList();



                if (search != "" && search != null)
                {
                    data = data.Where(w => w.name.Contains(search) || w.email.Contains(search)).ToList();
                }

                #region order by
                switch (orderBy)
                {
                    case "name":
                        data = desc ? data.OrderByDescending(o => o.name).ToList() : data.OrderBy(o => o.name).ToList();
                        break;
                    case "email":
                        data = desc ? data.OrderByDescending(o => o.email).ToList() : data.OrderBy(o => o.email).ToList();
                        break;
                    case "progress":
                        data = desc ? data.OrderByDescending(o => o.progress).ToList() : data.OrderBy(o => o.progress).ToList();
                        break;
                    case "pre_test":
                        data = desc ? data.OrderByDescending(o => o.score_pretest).ToList() : data.OrderBy(o => o.score_pretest).ToList();
                        break;
                    case "post_test":
                        data = desc ? data.OrderByDescending(o => o.score_after_class).ToList() : data.OrderBy(o => o.score_after_class).ToList();
                        break;
                    case "passed_date":
                        data = desc ? data.OrderByDescending(o => o.last_test_at).ToList() : data.OrderBy(o => o.last_test_at).ToList();
                        break;
                    default: break;
                }
                #endregion

                var skip = (pageIndex - 1) * pageSize;
                var count = data.Count();

                output = new
                {
                    data = data.Skip(skip).Take(pageSize).ToList(),
                    count = count
                };

            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic save_student_to_course(string email, string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var check_student = db.student.Where(w => w.email == email && w.is_deleted == 0).FirstOrDefault();
                if (check_student == null)
                {
                    error_message = "ไม่พบผู้เรียนที่ใช้ email นี้";
                    return false;
                }

                var check_info = db.student_course_info.Where(w => w.course_id == course_id && w.student_id == check_student.id && w.is_deleted == 0).FirstOrDefault();
                if (check_info != null)
                {
                    error_message = "email นี้ลงทะเบียน course นี้ไปแล้ว";
                    return false;
                }

                var get_course = db.course.Where(w => w.is_deleted == 0 && w.id == course_id).FirstOrDefault();
                student_course_info student_course_info = new student_course_info();
                student_course_info.student_id = check_student.id;
                student_course_info.course_id = course_id;
                student_course_info.course_lesson_id = 0;
                student_course_info.cert_print_count = 0;

                student_course_info.is_extend_study_time = 0;
                student_course_info.learning_startdate = get_course.learning_startdate;
                student_course_info.learning_enddate = get_course.learning_enddate;

                student_course_info.is_deleted = 0;
                student_course_info.created_by = auth.user_id;
                student_course_info.created_dt = now;
                student_course_info.update_by = auth.user_id;
                student_course_info.update_dt = now;
                db.student_course_info.Add(student_course_info);
                db.SaveChanges();

                output = student_course_info;

            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic get_student_by_id(int id, string course_id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_exam = db.course_exam.Where(w => w.is_deleted == 0 && w.course_id == course_id);
                var get_logging = db.course_exam_logging.Where(w => w.course_id == course_id && w.student_id == id && w.is_deleted == 0);
                var get_cert_print = db.certificate_print.Where(w => w.course_id == course_id && w.student_id == id);
                var get_cert_info = db.certificate_info.Where(w => w.student_id == id && w.course_id == course_id && w.cert_status.ToString().ToLower() == "y")
                                                       .Select(s => new
                                                       {
                                                           student_id = s.student_id,
                                                           print_count = get_cert_print.Where(w => w.certificate_id == s.certificate_id).Count()
                                                       }).FirstOrDefault();

                var cert_print_count = get_cert_info == null ? 0 : get_cert_info.print_count;
                var get_course_lesson_progress = db.student_course_lesson_progress.Where(w => w.student_id == id && w.course_id == course_id);

                var get_data = (from s in db.student
                                where s.student_id == id && s.is_deleted == 0
                                select new
                                {
                                    id = s.student_id,
                                    profile_image = (s.profile_image == null || s.profile_image == "") ? "" : s.profile_image,
                                    firstname = s.firstname,
                                    lastname = s.lastname,
                                    address = s.address,
                                    sub_district = s.sub_district_id,
                                    districts = s.district_id,
                                    province = s.province_id,
                                    email = s.email,
                                    phone = s.phone,
                                    progress = (from sv in db.student_video_progress
                                                join cl in db.course_lesson on sv.course_lesson_id equals cl.id
                                                where sv.course_id == course_id && sv.student_id == id
                                                select new
                                                {
                                                    id = cl.id,
                                                    order = cl.order,
                                                    name = cl.name,
                                                    date = sv.update_dt
                                                }).OrderByDescending(o => o.date).FirstOrDefault(),
                                    //progress = (from cl in db.course_lesson
                                    //                 where cl.is_deleted == 0 && cl.course_id == course_id
                                    //                 orderby cl.order
                                    //                 select new
                                    //                 {
                                    //                     id = cl.id,
                                    //                     order = cl.order,
                                    //                     name = cl.name,
                                    //                     is_done = get_course_lesson_progress.Where(w => w.course_lesson_id == cl.id && w.is_done_exercise == 1 && w.is_done_lesson == 1).Count() > 0 ? true : false,
                                    //                     is_done_exercise = get_course_lesson_progress.Where(w => w.course_lesson_id == cl.id).Select(ss => ss.is_done_exercise == 1 ? true : false).FirstOrDefault(),
                                    //                     is_done_lesson = get_course_lesson_progress.Where(w => w.course_lesson_id == cl.id).Select(ss => ss.is_done_lesson == 1 ? true : false).FirstOrDefault(),
                                    //                 }).OrderBy(o => o.is_done).FirstOrDefault(),
                                    business_type = s.business_type_id,
                                    batch = db.course.Where(w => w.id == course_id && w.is_deleted == 0).Select(b => b.batch).FirstOrDefault(),
                                    count_exam = get_exam.Count(),
                                    pretest = get_logging.Where(w => w.is_pretest == 1 && get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0).Count(),
                                    after_class = get_logging.Where(w => w.is_pretest == 0 && get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0).Count(),
                                    cert_print = cert_print_count
                                }).FirstOrDefault();

                if (get_data == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                output = get_data;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic reset_course(int id, string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                #region reset student info
                var get_info = db.student_course_info.Where(w => w.student_id == id && w.course_id == course_id).FirstOrDefault();
                if (get_info != null)
                {
                    get_info.cert_print_count = 0;
                    get_info.update_dt = now;
                    get_info.update_by = auth.user_id;

                    db.SaveChanges();
                }
                #endregion

                #region reset course progress
                foreach (var progress in db.student_course_lesson_progress.Where(w => w.course_id == course_id && w.student_id == id).ToList())
                {
                    db.student_course_lesson_progress.Remove(progress);
                    db.SaveChanges();
                }
                #endregion

                #region reset video progress
                foreach (var video_progress in db.student_video_progress.Where(w => w.course_id == course_id && w.student_id == id).ToList())
                {
                    db.student_video_progress.Remove(video_progress);
                    db.SaveChanges();
                }
                #endregion

                #region reset exam logging
                foreach (var logging in db.course_exam_logging.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == id).ToList())
                {
                    logging.is_deleted = 1;
                    logging.update_dt = now;
                    logging.update_by = auth.user_id;

                    db.SaveChanges();
                }
                #endregion

                #region reset certificate
                var get_cert = db.certificate_info.Where(w => w.student_id == id && w.course_id == course_id).FirstOrDefault();
                if (get_cert != null)
                {
                    get_cert.cert_status = "N";
                    get_cert.update_by = auth.user_id;
                    get_cert.update_dt = now;

                    db.SaveChanges();
                }
                #endregion

                #region reset evaluation
                foreach (var eva in db.course_evaluation_result.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == id).ToList())
                {
                    eva.is_deleted = 1;
                    eva.update_dt = now;
                    eva.update_by = auth.user_id;

                    db.SaveChanges();
                }
                #endregion

                output = get_info;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic cancel_course(int id, string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                #region reset student info
                var get_info = db.student_course_info.Where(w => w.student_id == id && w.course_id == course_id).FirstOrDefault();
                if (get_info != null)
                {
                    get_info.is_deleted = 1;
                    get_info.update_dt = now;
                    get_info.update_by = auth.user_id;

                    db.SaveChanges();
                }
                #endregion

                #region reset course progress
                foreach (var progress in db.student_course_lesson_progress.Where(w => w.course_id == course_id && w.student_id == id).ToList())
                {
                    db.student_course_lesson_progress.Remove(progress);
                    db.SaveChanges();
                }
                #endregion

                #region reset video progress
                foreach (var video_progress in db.student_video_progress.Where(w => w.course_id == course_id && w.student_id == id).ToList())
                {
                    db.student_video_progress.Remove(video_progress);
                    db.SaveChanges();
                }
                #endregion

                #region reset exam logging
                foreach (var logging in db.course_exam_logging.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == id).ToList())
                {
                    logging.is_deleted = 1;
                    logging.update_dt = now;
                    logging.update_by = auth.user_id;

                    db.SaveChanges();
                }
                #endregion

                #region reset certificate
                var get_cert = db.certificate_info.Where(w => w.student_id == id && w.course_id == course_id).FirstOrDefault();
                if (get_cert != null)
                {
                    get_cert.cert_status = "N";
                    get_cert.update_by = auth.user_id;
                    get_cert.update_dt = now;

                    db.SaveChanges();
                }
                #endregion

                #region reset evaluation
                foreach (var eva in db.course_evaluation_result.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == id).ToList())
                {
                    eva.is_deleted = 1;
                    eva.update_dt = now;
                    eva.update_by = auth.user_id;

                    db.SaveChanges();
                }
                #endregion

                output = get_info;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        #endregion
        #endregion

        public dynamic change_learning_enddate(DateTime? learning_startdate, DateTime? learning_enddate, string course_id)
        {
            var get_info = db.student_course_info.Where(w => w.course_id == course_id && w.is_deleted == 0).ToList();
            foreach (var item in get_info)
            {
                if (item.is_extend_study_time == 1)
                {
                    DateTime? extend_end_date = learning_enddate == null ? null : (DateTime?)learning_enddate.Value.AddDays(30);
                    item.learning_enddate = learning_enddate;
                    item.learning_startdate = learning_startdate;
                }
                else
                {
                    item.learning_enddate = learning_enddate;
                    item.learning_startdate = learning_startdate;
                }

                db.SaveChanges();
            }

            return "";
        }

        #region Frontend
        //success wait test
        public dynamic get_list_category(ref string error_message)
        {
            dynamic output = null;
            try
            {
                var course_category = db.course_category.ToList();

                output = course_category;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic get_list_course(string search, DateTime? register_start_date, DateTime? register_end_date, string category_id, int? learning_online, bool? is_free, int? price_gte, int? price_lte, int? hasCertificate, string sort, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = new List<object>();
            try
            {
                var student_type = auth.user_id == 0 ? 1 : db.student.Where(w => w.user_id == auth.user_id).Select(s => s.student_account_type_id).FirstOrDefault();
                var get_data = db.course.Where(w => w.is_deleted == 0 && db.content_permission.Where(ww => ww.course_id == w.id && ww.student_type_permission == student_type).Count() > 0).ToList();

                #region Check Search
                if (search != "" && search != null)
                {
                    get_data = get_data.Where(w => w.name.Contains(search)).ToList();
                }
                #endregion

                #region Check Register range
                if (register_start_date != null && register_end_date != null)
                {
                    if (register_start_date == register_end_date)
                    {
                        register_end_date = register_end_date.Value.AddDays(1);
                    }
                    get_data = get_data.Where(w => w.isAlwaysRegister == 1 || (w.register_start_date >= register_start_date && w.register_end_date <= register_end_date)).ToList();
                }
                #endregion

                #region Check category
                category_id = category_id.Replace("\"", "");
                if (category_id != "" && category_id != null && category_id != "0" && category_id != "''")
                {
                    var arr_category = category_id.Split(',');
                    get_data = get_data.Where(w => arr_category.Contains(w.course_category_id.ToString())).ToList();
                }
                #endregion

                #region Check Type learning
                if (learning_online != null)
                {
                    get_data = learning_online == 1 ? get_data.Where(w => w.is_learning_online == 1).ToList() : get_data.Where(w => w.is_learning_online == 2).ToList();
                }
                #endregion
                
                #region Check Price And Course free
                if ((price_lte != 0 && price_lte != null && price_gte != null) || is_free == true)
                {
                    get_data = price_lte != 0 && price_lte != null && price_gte != null && is_free == true ? get_data.Where(w => (w.cost >= price_gte && w.cost <= price_lte) || w.is_has_cost == 0).ToList() :
                               price_lte != 0 && price_lte != null && price_gte != null && is_free != true ? get_data.Where(w => w.cost >= price_gte && w.cost <= price_lte).ToList() :
                               get_data.Where(w => w.is_has_cost == 0).ToList();
                }
                #endregion

                #region Check Has Certificate
                if (hasCertificate != null)
                {
                    get_data = hasCertificate == 1 ? get_data.Where(w => w.hasCertificate == 1).ToList() : get_data.Where(w => w.hasCertificate == 0).ToList();
                }
                #endregion

                if (get_data.Count > 0)
                {
                    var data = get_data.Select(s => new
                    {
                        id = s.id,
                        name = s.name,
                        sub_name = s.sub_name,
                        batch = s.batch,
                        category_name = db.course_category.Where(w => w.id == s.course_category_id).Select(cat_name => cat_name.name).FirstOrDefault(),
                        category_color = db.course_category.Where(w => w.id == s.course_category_id).Select(cat_color => cat_color.color).FirstOrDefault(),
                        overview_course = s.overview_course,
                        cover = s.cover_pic,
                        total_lesson = db.course_lesson.Where(w => w.course_id == s.id && w.is_deleted == 0).Count(),
                        lesson_time = db.course_lesson.Where(w => w.course_id == s.id && w.is_deleted == 0).Sum(sl => sl.lesson_time),
                        is_always_learning = s.is_always_learning == 1 ? true : false,
                        start_learning = s.learning_startdate,
                        start_register = s.register_start_date,
                        is_has_cost = s.is_has_cost == 1 ? true : false,
                        cost = s.cost,
                        created_at = s.created_dt,
                        hasCertificate = s.hasCertificate == 1 ? true : false,
                        list_instructor = (from cl in db.course_lesson
                                           join ins in db.instructor.Where(w => w.is_deleted == 0) on cl.instructor_id equals ins.id
                                           where cl.is_deleted == 0 && cl.course_id == s.id
                                           select new
                                           {
                                               firstname = ins.firstname,
                                               lastname = ins.lastname,
                                               profile = ins.profile_image == null || ins.profile_image == "" ? "" : ins.profile_image
                                           }).Distinct().ToList()
                    }).OrderByDescending(o => o.created_at).ToList();

                    output = data;
                    if (sort != "" && sort != null)
                    {
                        switch (sort)
                        {
                            case "newest":
                                output = data.OrderByDescending(o => o.created_at).ToList();
                                break;
                            case "cheapest":
                                output = data.OrderBy(o => o.cost).ToList();
                                break;
                            case "expensive":
                                output = data.OrderByDescending(o => o.cost).ToList();
                                break;
                            case "letters":
                                output = data.OrderBy(o => o.name).ToList();
                                break;
                            default: break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic get_releted_course(string own_course, string category_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = new List<object>();
            try
            {
                var student_type = auth.user_id == 0 ? 1 : db.student.Where(w => w.user_id == auth.user_id).Select(s => s.student_account_type_id).FirstOrDefault();
                var get_data = db.course.Where(w => w.is_deleted == 0 && w.course_category_id == category_id && w.id != own_course && db.content_permission.Where(ww => ww.course_id == w.id && ww.student_type_permission == student_type).Count() > 0).ToList();

                if (get_data.Count > 0)
                {
                    var data = get_data.Select(s => new
                    {
                        id = s.id,
                        name = s.name,
                        sub_name = s.sub_name,
                        batch = s.batch,
                        category_name = db.course_category.Where(w => w.id == s.course_category_id).Select(cat_name => cat_name.name).FirstOrDefault(),
                        category_color = db.course_category.Where(w => w.id == s.course_category_id).Select(cat_color => cat_color.color).FirstOrDefault(),
                        overview_course = s.overview_course,
                        cover = s.cover_pic,
                        total_lesson = db.course_lesson.Where(w => w.course_id == s.id && w.is_deleted == 0).Count(),
                        lesson_time = db.course_lesson.Where(w => w.course_id == s.id && w.is_deleted == 0).Sum(sl => sl.lesson_time),
                        start_learning = s.learning_startdate,
                        start_register = s.register_start_date,
                        is_has_cost = s.is_has_cost == 1 ? true : false,
                        cost = s.cost,
                        created_at = s.created_dt,
                        hasCertificate = s.hasCertificate == 1 ? true : false,
                        list_instructor = (from cl in db.course_lesson
                                           join ins in db.instructor.Where(w => w.is_deleted == 0) on cl.instructor_id equals ins.id
                                           where cl.is_deleted == 0 && cl.course_id == s.id
                                           select new
                                           {
                                               firstname = ins.firstname,
                                               lastname = ins.lastname,
                                               profile = ins.profile_image == null || ins.profile_image == "" ? "" : ins.profile_image
                                           }).ToList()
                    }).OrderByDescending(o => o.created_at).ToList();

                    output = data;
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic get_my_course_progress(Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = new List<object>();
            try
            {
                var get_student = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student != null)
                {
                    var get_info = db.student_course_info.Where(w => w.student_id == get_student.id && w.is_deleted == 0).ToList();
                    //var get_progress = db.student_course_lesson_progress.Where(w => w.student_id == get_student.id && w.is_done_lesson == 0 && w.is_done_exercise == 0).ToList();
                    foreach (var item in get_info)
                    {
                        var get_course_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == item.course_id).OrderBy(o => o.order);

                        var continue_learning = (from sp in db.student_course_lesson_progress
                                                 join cl in get_course_lesson on sp.course_lesson_id equals cl.id
                                                 where sp.course_id == item.course_id && sp.student_id == get_student.id
                                                 select new
                                                 {
                                                     id = cl.id,
                                                     name = cl.name,
                                                     order = cl.order,
                                                     is_done = (sp.is_done_lesson == 1 && sp.is_done_exercise == 1 ? true : false),
                                                     progress = (((double)db.student_course_lesson_progress.Where(w => w.student_id == get_student.id && w.course_id == item.course_id && w.is_done_exercise == 1 && w.is_done_lesson == 1).Count() / get_course_lesson.Count()) * 100)
                                                 }).OrderBy(o => o.is_done).FirstOrDefault();

                        if (continue_learning == null || !continue_learning.is_done)
                        {
                            var order_lesson = continue_learning == null ? 1 : continue_learning.order;
                            var progress_lesson = continue_learning == null ? 0 : continue_learning.progress;
                            var get_course = db.course.Where(w => w.id == item.course_id && w.is_deleted == 0)
                                                      .Select(s => new
                                                      {
                                                          id = s.id,
                                                          name = s.name,
                                                          sub_name = s.sub_name,
                                                          batch = s.batch,
                                                          cover = s.cover_pic,
                                                          order_lesson = order_lesson,
                                                          lesson_name = get_course_lesson.Where(w => w.order == order_lesson).Select(lsn => lsn.name).FirstOrDefault(),
                                                          progress_lesson = progress_lesson,
                                                          learning_end = item.learning_enddate
                                                      }).FirstOrDefault();

                            output.Add(get_course);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic get_course_info(string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).Select(s => s.id).FirstOrDefault();
                var student_type = auth.user_id == 0 ? 1 : db.student.Where(w => w.user_id == auth.user_id).Select(s => s.student_account_type_id).FirstOrDefault();
                var check_my_own = db.student_course_info.Where(w => w.student_id == get_student_id && w.is_deleted == 0 && w.course_id == course_id).FirstOrDefault();
                var get_course_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == course_id).OrderBy(o => o.order);

                #region Get course
                var course = db.course.Where(w => w.is_deleted == 0 && w.id == course_id && (db.student_course_info.Where(ww => ww.student_id == get_student_id && ww.is_deleted == 0 && ww.course_id == course_id).Count() > 0 || db.content_permission.Where(ww => ww.course_id == w.id && ww.student_type_permission == student_type).Count() > 0)).Select(s => new
                {
                    id = s.id,
                    passed_percent = s.passed_percent,
                    name = s.name,
                    sub_name = s.sub_name,
                    batch = s.batch,
                    cover = s.cover_pic,
                    info_cover = s.info_cover,
                    category = db.course_category.Where(w => w.id == s.course_category_id).FirstOrDefault(),
                    category_color = db.course_category.Where(w => w.id == s.course_category_id).Select(cat_color => cat_color.color).FirstOrDefault(),
                    hasCertificate = s.hasCertificate == 1 ? true : false,
                    is_has_cost = s.is_has_cost == 1 ? true : false,
                    cost = s.cost,
                    overview = s.overview_course,
                    total_lesson = get_course_lesson.Count(),
                    lesson_time = get_course_lesson.Sum(sl => sl.lesson_time),

                    video = new video
                    {
                        p_480 = s.p_480,
                        p_720 = s.p_720,
                        p_1080 = s.p_1080,
                        original = s.p_original,
                        thumbnail = s.cover_video,
                    },

                    isAlwaysRegister = s.isAlwaysRegister == 1 ? true : false,
                    register_start_date = s.register_start_date,
                    register_end_date = s.register_end_date,
                    is_always_learning = s.is_always_learning == 1 ? true : false,
                    learning_start_date = s.learning_startdate,
                    learning_end_date = s.learning_enddate,

                    objective_course = s.objective_course,
                    benefits = s.benefits,
                    contact_name = s.contact_name,
                    contact_phone = s.contact_phone,
                    contact_email = s.contact_email
                }).FirstOrDefault();

                if(course == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }
                #endregion

                var list_instructor = (from cl in get_course_lesson
                                       join ins in db.instructor.Where(w => w.is_deleted == 0) on cl.instructor_id equals ins.id
                                       select new
                                       {
                                           id = ins.id,
                                           firstname = ins.firstname,
                                           lastname = ins.lastname,
                                           description = ins.description,
                                           profile = ins.profile_image == null || ins.profile_image == "" ? "" : ins.profile_image
                                       }).ToList();

                list_instructor = list_instructor.GroupBy(g => g.id).Select(s => s.First()).ToList();

                #region Get Lesson
                dynamic course_lesson = null;
                dynamic can_extend_study_time = null;
                dynamic is_own_course = null;
                dynamic student_learning_enddate = course.learning_end_date;
                dynamic continue_learning = null;
                dynamic can_enroll = null;
                dynamic is_timeup = null;
                if (check_my_own == null)
                {
                    course_lesson = get_course_lesson.OrderBy(o => o.order).Select(s => s.name).ToList();
                    can_extend_study_time = false;
                    is_own_course = false;
                    can_enroll = course.isAlwaysRegister == true ? true : (course.register_start_date <= now && course.register_end_date >= now ? true : false);
                    is_timeup = course.is_always_learning == true ? false : (course.learning_start_date <= now && course.learning_end_date >= now ? false : true);
                }
                else
                {
                    var get_lesson_exercise = db.course_lesson_exercise.Where(w => w.is_deleted == 0 && w.course_id == course_id);
                    var get_exam = db.course_exam.Where(w => w.is_deleted == 0 && w.course_id == course_id);
                    continue_learning = (from sp in db.student_course_lesson_progress
                                         join cl in get_course_lesson on sp.course_lesson_id equals cl.id
                                         where sp.course_id == course_id && sp.student_id == get_student_id
                                         select new
                                         {
                                             id = cl.id,
                                             name = cl.name,
                                             order = cl.order,
                                             is_done = (sp.is_done_lesson == 1 && sp.is_done_exercise == 1 ? true : false),
                                             progress = (((double)db.student_course_lesson_progress.Where(w => w.student_id == get_student_id && w.course_id == course_id && w.is_done_exercise == 1 && w.is_done_lesson == 1).Count() / get_course_lesson.Count()) * 100)
                                         }).OrderBy(o => o.is_done).FirstOrDefault();
                    if (continue_learning == null)
                    {
                        continue_learning = null;
                    }

                    is_timeup = check_my_own.learning_startdate == null && check_my_own.learning_enddate == null ? false : (check_my_own.learning_startdate <= now && check_my_own.learning_enddate >= now ? false : true);
                    course_lesson = get_course_lesson.Select(s => new
                    {
                        id = s.id,
                        name = s.name,
                        time = s.lesson_time,
                        is_done_lesson = db.student_course_lesson_progress.Where(w => w.student_id == check_my_own.student_id && w.course_lesson_id == s.id && w.course_id == course_id).Select(ss => ss.is_done_lesson).FirstOrDefault(),
                        progress = db.student_video_progress.Where(w => w.course_id == course_id && w.course_lesson_id == s.id && w.student_id == check_my_own.student_id).Select(sv => sv.video_position).FirstOrDefault(),
                        exercise = get_lesson_exercise.Where(w => w.course_lesson_id == s.id).Count(),
                        exam = get_exam.Count(),
                        order = s.order,
                        attachment = s.attachment
                    }).OrderBy(o => o.order).ToList();

                    can_extend_study_time = (course.learning_end_date != null && check_my_own.is_extend_study_time == 0 && course.learning_end_date <= now ? true : false);
                    is_own_course = true;
                    student_learning_enddate = check_my_own.learning_enddate;

                    can_enroll = false;
                }
                #endregion

                #region Get Related Course
                var error = "";
                var related_course = get_releted_course(course_id, course.category.id, auth, ref error);
                #endregion

                output = new
                {
                    course,
                    list_instructor,
                    related_course,
                    course_lesson,
                    can_extend_study_time,
                    is_own_course,
                    student_learning_enddate,
                    continue_learning,
                    can_enroll,
                    is_timeup
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic put_print_course(string course_id, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var course = db.course.Where(w => w.is_deleted == 0 && w.id == course_id).FirstOrDefault();
                if (course == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                course.print_count = (course.print_count + 1);
                db.SaveChanges();
                output = course;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic put_extend_study_time(string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                var student_info = db.student_course_info.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == get_student_id.id).FirstOrDefault();
                if (student_info == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }

                if (student_info.is_extend_study_time == 1)
                {
                    error_message = "เคยขยายเวลาเรียนไปแล้ว";
                    return false;
                }


                DateTime? learning_enddate = student_info.learning_enddate == null ? null : (DateTime?)student_info.learning_enddate.Value.AddDays(30);
                CultureInfo provider = CultureInfo.InvariantCulture;

                student_info.is_extend_study_time = 1;
                student_info.learning_enddate = learning_enddate == null ? learning_enddate : DateTime.ParseExact(learning_enddate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);
                db.SaveChanges();
                output = student_info;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic get_course_by_id(string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var student_type = auth.user_id == 0 ? 1 : db.student.Where(w => w.user_id == auth.user_id).Select(s => s.student_account_type_id).FirstOrDefault();
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).Select(s => s.id).FirstOrDefault();
                var check_my_own = db.student_course_info.Where(w => w.student_id == get_student_id && w.is_deleted == 0 && w.course_id == course_id).FirstOrDefault();
                CultureInfo provider = CultureInfo.InvariantCulture;

                #region Check Course
                var course = db.course.Where(w => w.is_deleted == 0 && w.id == course_id && (db.student_course_info.Where(ww => ww.student_id == get_student_id && ww.is_deleted == 0 && ww.course_id == course_id).Count() > 0 || db.content_permission.Where(ww => ww.course_id == w.id && ww.student_type_permission == student_type).Count() > 0))
                                      .Select(s => new
                                      {
                                          id = s.id,
                                          cover = s.cover_pic,
                                          info_cover = s.info_cover,
                                          name = s.name,
                                          sub_name = s.sub_name,
                                          passed_percent = s.passed_percent,
                                          batch = s.batch,
                                          overview_course = s.overview_course,
                                          objective_course = s.objective_course,

                                          video = new video
                                          {
                                              p_480 = s.p_480,
                                              p_720 = s.p_720,
                                              p_1080 = s.p_1080,
                                              original = s.p_original,
                                              thumbnail = s.cover_video,
                                          },
                                      }).FirstOrDefault();
                if (course == null)
                {
                    error_message = "Id Invalid";
                    return false;
                }
                #endregion

                #region Check own course
                var get_course_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == course_id).OrderBy(o => o.order).ToList();

                #region dynamic data
                dynamic course_lesson = new List<dynamic>();
                dynamic trial_class = true;
                dynamic can_use_pre_test = false;
                dynamic exam_pre_test = new List<dynamic>();
                dynamic exam_post_test = new List<model_exam_platform>();
                dynamic evaluation = new List<object>();
                dynamic can_use_evaluation = false;
                dynamic can_use_post_test = false;
                dynamic time_up_post_test = false;
                dynamic pre_test_pass = false;
                dynamic post_test_pass = false;
                dynamic total_exam = 0;
                dynamic score_pre_test = 0;
                dynamic score_post_test = 0;
                dynamic percent_pre_test = 0;
                dynamic percent_post_test = 0;
                dynamic continue_learning = null;
                dynamic not_yet_learn = false;
                #endregion

                var date_now = DateTime.ParseExact(now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);
                //if (check_my_own != null && ((check_my_own.learning_startdate == null && check_my_own.learning_enddate == null) || (check_my_own.learning_startdate <= date_now && check_my_own.learning_enddate >= date_now)))
                if ((check_my_own != null && check_my_own.learning_startdate == null && check_my_own.learning_enddate == null) || (check_my_own != null && check_my_own.learning_startdate <= date_now && check_my_own.learning_enddate >= date_now))
                {
                    var get_student_course_progress = db.student_course_lesson_progress.Where(w => w.course_id == course_id && w.student_id == check_my_own.student_id).ToList();
                    trial_class = false;
                    var get_exam = db.course_exam.Where(w => w.is_deleted == 0 && w.course_id == course_id).ToList();
                    total_exam = get_exam.Count();
                    var passed_percent = ((double)course.passed_percent / 100);
                    var exam_pass = (total_exam == 1 ? 1 : Math.Floor(total_exam * passed_percent));

                    #region get exam pre test
                    exam_pre_test = get_exam.Select(s => new model_exam_platform
                    {
                        id = s.id,
                        course_id = s.course_id,
                        order = s.order,
                        question = s.question,
                        image = (s.image == null || s.image == "") ? "" : s.image,
                        video = new video
                        {
                            p_480 = (s.p_480 == null || s.p_480 == "") ? "" : s.p_480,
                            p_720 = (s.p_720 == null || s.p_720 == "") ? "" : s.p_720,
                            p_1080 = (s.p_1080 == null || s.p_1080 == "") ? "" : s.p_1080,
                            original = (s.p_original == null || s.p_original == "") ? "" : s.p_original,
                            thumbnail = (s.cover_video == null || s.cover_video == "") ? "" : s.cover_video
                        },
                        list_answer = db.course_exam_answer.Where(w => w.course_exam_id == s.id).Select(ans => new model_answer_exam_platform
                        {
                            id = ans.id,
                            course_exam_id = ans.course_exam_id,
                            course_id = ans.course_id,
                            order = ans.order,
                            answer = ans.answer
                        }).ToList()
                    }).ToList();

                    exam_pre_test = ramdom_exam(exam_pre_test);
                    #endregion

                    #region get lesson
                    course_lesson = get_course_lesson.Select(s => new
                    {
                        id = s.id,
                        name = s.name,
                        order = s.order,
                        description = s.description,
                        time = s.lesson_time,
                        attachment = s.attachment,
                        main_video = new video
                        {
                            p_480 = s.main_p_480,
                            p_720 = s.main_p_720,
                            p_1080 = s.main_p_1080,
                            original = s.main_p_original,
                            thumbnail = s.main_cover_video,
                        },
                        is_interactive = s.is_interactive == 1 ? true : false,
                        interactive = (from i in db.interactive_question
                                       where i.is_deleted == 0 && i.course_id == course_id && i.course_lesson_id == s.id
                                       select new
                                       {
                                           id = i.id,
                                           course_id = i.course_id,
                                           course_lesson_id = i.course_lesson_id,
                                           name = i.name,
                                           order = i.order,
                                           interactive_time = i.interactive_time,
                                           answer = db.interactive_answer.Where(w => w.interactive_question_id == i.id && w.course_lesson_id == s.id).ToList()
                                       }).ToList(),
                        done_lesson = get_student_course_progress.Where(w => w.course_lesson_id == s.id).Select(done_lesson => (done_lesson.is_done_lesson == 1 ? true : false)).FirstOrDefault(),
                        done_exercise = get_student_course_progress.Where(w => w.course_lesson_id == s.id).Select(done_exercise => (done_exercise.is_done_exercise == 1 ? true : false)).FirstOrDefault(),
                        video_position = db.student_video_progress.Where(w => w.course_id == course_id && w.course_lesson_id == s.id && w.student_id == check_my_own.student_id).Select(sv => sv.video_position).FirstOrDefault(),
                        video_progress = db.student_video_progress.Where(w => w.course_id == course_id && w.course_lesson_id == s.id && w.student_id == check_my_own.student_id).Select(sp => sp.video_progress).FirstOrDefault(),
                        instructor = db.instructor.Where(w => w.id == s.instructor_id)
                                                     .Select(ins => new
                                                     {
                                                         firstname = ins.firstname,
                                                         lastname = ins.lastname
                                                     }).FirstOrDefault(),
                        exercise = (from lex in db.course_lesson_exercise
                                    where lex.course_lesson_id == s.id && lex.is_deleted == 0
                                    select new
                                    {
                                        id = lex.id,
                                        course_id = lex.course_id,
                                        order = lex.order,
                                        course_lesson_id = lex.course_lesson_id,
                                        question = lex.question,
                                        image = (lex.image == null || lex.image == "") ? "" : lex.image,
                                        video = new video
                                        {
                                            p_480 = (lex.p_480 == null || lex.p_480 == "") ? "" : lex.p_480,
                                            p_720 = (lex.p_720 == null || lex.p_720 == "") ? "" : lex.p_720,
                                            p_1080 = (lex.p_1080 == null || lex.p_1080 == "") ? "" : lex.p_1080,
                                            original = (lex.p_original == null || lex.p_original == "") ? "" : lex.p_original,
                                            thumbnail = (lex.cover_video == null || lex.cover_video == "") ? "" : lex.cover_video,
                                        },
                                        is_answer_match = lex.is_answer_match == 1 ? true : false,
                                        is_answer_choice = lex.is_answer_choice == 1 ? true : false,
                                        choices = db.course_lesson_exercise_answer_choices.Where(w => w.course_lesson_id == s.id && w.course_lesson_exercise_id == lex.id)
                                                                                          .Select(ch => new
                                                                                          {
                                                                                              id = ch.id,
                                                                                              course_lesson_exercise_id = ch.course_lesson_exercise_id,
                                                                                              course_lesson_id = ch.course_lesson_id,
                                                                                              answer = ch.answer,
                                                                                              order = ch.order,
                                                                                              correct = ch.correct
                                                                                          }).ToList(),
                                        match = db.course_lesson_exercise_answer_match.Where(w => w.course_lesson_id == s.id && w.course_lesson_exercise_id == lex.id)
                                                                                      .Select(match => new
                                                                                      {
                                                                                          id = match.id,
                                                                                          course_lesson_exercise_id = match.course_lesson_exercise_id,
                                                                                          course_lesson_id = match.course_lesson_id,
                                                                                          question = match.question,
                                                                                          answer = match.answer
                                                                                      }).ToList()
                                    }).OrderBy(o => o.order).ToList()
                    }).OrderBy(o => o.order).ToList();
                    #endregion

                    #region get exam post test
                    var get_exam_logging = db.course_exam_logging.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == check_my_own.student_id).ToList();
                    var get_exam_post_test = get_exam_logging.Where(w => w.is_pretest == 0).ToList();
                    if (get_exam_post_test.Count() > 0)
                    {

                        var get_list_exam_post_test = (from ex in get_exam
                                                       select new model_exam_platform
                                                       {
                                                           id = ex.id,
                                                           course_id = ex.course_id,
                                                           order = ex.order,
                                                           question = ex.question,
                                                           image = (ex.image == null || ex.image == "") ? "" : ex.image,
                                                           video = new video
                                                           {
                                                               p_480 = (ex.p_480 == null || ex.p_480 == "") ? "" : ex.p_480,
                                                               p_720 = (ex.p_720 == null || ex.p_720 == "") ? "" : ex.p_720,
                                                               p_1080 = (ex.p_1080 == null || ex.p_1080 == "") ? "" : ex.p_1080,
                                                               original = (ex.p_original == null || ex.p_original == "") ? "" : ex.p_original,
                                                               thumbnail = (ex.cover_video == null || ex.cover_video == "") ? "" : ex.cover_video
                                                           },
                                                           list_answer = db.course_exam_answer.Where(w => w.course_exam_id == ex.id).Select(ans => new model_answer_exam_platform
                                                           {
                                                               id = ans.id,
                                                               course_exam_id = ans.course_exam_id,
                                                               course_id = ans.course_id,
                                                               order = ans.order,
                                                               answer = ans.answer
                                                           }).ToList()
                                                       }).ToList();

                        foreach (var item_exam in get_list_exam_post_test)
                        {
                            var get_exam_pass = get_exam.Where(w => w.id == item_exam.id).Select(s => s.answer).FirstOrDefault();
                            var check_exam_not_pass = get_exam_post_test.Where(w => w.course_exam_id == item_exam.id && w.course_exam_answer_id == get_exam_pass).FirstOrDefault();
                            if (check_exam_not_pass == null)
                            {
                                exam_post_test.Add(item_exam);
                            }
                        }
                    }
                    else
                    {
                        exam_post_test = exam_pre_test;
                    }
                    exam_post_test = ramdom_exam(exam_post_test);
                    #endregion

                    #region get Evaluation
                    var get_evaluation_result = db.course_evaluation_result.Where(w => w.course_id == course_id && w.student_id == check_my_own.student_id && w.is_deleted == 0).Count();
                    can_use_evaluation = get_evaluation_result > 0 ? false : true;
                    evaluation = db.course_evaluation.Where(w => w.is_deleted == 0 && w.course_id == course_id)
                                                     .Select(s => new
                                                     {
                                                         id = s.id,
                                                         order = s.order,
                                                         question = s.question,
                                                         is_free_fill_text = s.is_free_fill_text == 1 ? true : false,
                                                         free_fill_text = s.free_fill_text,
                                                         answer = db.course_evaluation_choices.Where(w => w.course_evaluation_id == s.id)
                                                                                              .Select(ans => new
                                                                                              {
                                                                                                  id = ans.id,
                                                                                                  course_evaluation_id = ans.course_evaluation_id,
                                                                                                  choice = ans.choice,
                                                                                                  score = ans.score,
                                                                                                  order = ans.order
                                                                                              }).ToList()
                                                     }).ToList();
                    #endregion

                    #region Check Can Use Post test
                    var get_student_progress = get_student_course_progress.Where(w => w.is_done_exercise == 1 && w.is_done_lesson == 1).Count();
                    can_use_post_test = (get_student_progress == get_course_lesson.Count() ? true : false);
                    time_up_post_test = (check_my_own.learning_enddate == null || check_my_own.learning_enddate >= DateTime.Now) ? false : true;
                    #endregion

                    #region Check Score Pre test, Post test
                    can_use_pre_test = get_exam_logging.Where(w => w.is_pretest == 1).Count() > 0 ? false : true;
                    score_pre_test = get_exam_logging.Where(w => w.is_pretest == 1 && get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0).Count();
                    //score_pre_test = get_exam_logging.Where(w => w.is_pretest == 1).Count();
                    pre_test_pass = get_exam_logging.Where(w => w.is_pretest == 1).Count() > 0 ? true : false;
                    percent_pre_test = (((double)score_pre_test / total_exam) * 100);


                    score_post_test = get_exam_post_test.Where(w => get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0).Count();
                    post_test_pass = (score_post_test >= exam_pass ? true : false);
                    percent_post_test = (((double)score_post_test / total_exam) * 100);
                    exam_post_test = percent_post_test >= 80 ? new List<object>() : exam_post_test;
                    #endregion

                    continue_learning = (from sp in db.student_course_lesson_progress
                                         join cl in db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == course_id).OrderBy(o => o.order) on sp.course_lesson_id equals cl.id
                                         where sp.course_id == course_id && sp.student_id == get_student_id && ((sp.is_done_lesson == 0 && sp.is_done_exercise == 1) || (sp.is_done_lesson == 1 && sp.is_done_exercise == 0) || (sp.is_done_lesson == 0 && sp.is_done_exercise == 0))
                                         select new
                                         {
                                             id = cl.id,
                                             name = cl.name,
                                             order = cl.order,
                                             continue_lern = true,
                                         }).OrderBy(o => o.continue_lern).FirstOrDefault();
                    if (continue_learning == null)
                    {
                        continue_learning = null;
                    }

                    not_yet_learn = false;
                }
                else
                {
                    course_lesson = get_course_lesson.Select(s => new
                    {
                        id = s.id,
                        name = s.name,
                        order = s.order,
                        description = s.description,
                        time = s.lesson_time,
                        main_video = new video
                        {
                            p_480 = "",
                            p_720 = "",
                            p_1080 = "",
                            original = "",
                            thumbnail = "",
                        },
                        is_interactive = s.is_interactive == 1 ? true : false,
                        interactive = (from i in db.interactive_question
                                       where i.is_deleted == 0 && i.course_id == course_id && i.course_lesson_id == s.id
                                       select new
                                       {
                                           id = i.id,
                                           course_id = i.course_id,
                                           course_lesson_id = i.course_lesson_id,
                                           name = i.name,
                                           order = i.order,
                                           interactive_time = i.interactive_time,
                                           answer = db.interactive_answer.Where(w => w.interactive_question_id == i.id && w.course_lesson_id == s.id).ToList()
                                       }).ToList(),
                        done_lesson = false,
                        done_exercise = false,
                        video_position = 0,
                        video_progress = 0,
                        instructor = db.instructor.Where(w => w.id == s.instructor_id)
                                                     .Select(ins => new
                                                     {
                                                         firstname = ins.firstname,
                                                         lastname = ins.lastname
                                                     }).FirstOrDefault(),
                        exercise = new List<object>()
                    }).OrderBy(o => o.order).ToList();

                    if (course_lesson.Count > 0)
                    {
                        var get_video_lesson_1 = get_course_lesson.Where(w => w.id == course_lesson[0].id).FirstOrDefault();
                        course_lesson[0].main_video.p_480 = get_video_lesson_1.main_p_480;
                        course_lesson[0].main_video.p_720 = get_video_lesson_1.main_p_720;
                        course_lesson[0].main_video.p_1080 = get_video_lesson_1.main_p_1080;
                        course_lesson[0].main_video.original = get_video_lesson_1.main_p_original;
                        course_lesson[0].main_video.thumbnail = get_video_lesson_1.main_cover_video;
                    }

                    if(check_my_own != null)
                    {
                        not_yet_learn = true;
                    }
                }
                #endregion
                
                #region output
                output = new
                {
                    not_yet_learn,

                    continue_learning,
                    trial_class,
                    course,

                    can_use_pre_test,
                    total_exam,
                    exam_pre_test,
                    pre_test_pass,
                    score_pre_test,
                    percent_pre_test,
                    
                    course_lesson,

                    exam_post_test,
                    can_use_post_test,
                    time_up_post_test,
                    post_test_pass,
                    score_post_test,
                    percent_post_test,

                    evaluation,
                    can_use_evaluation
                };
                #endregion
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic put_stamp_exercise(string course_id, int course_lesson_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student_id == null)
                {
                    error_message = "ไม่มีนักเรียนในคอร์สนี้";
                    return false;
                }


                var get_student_progress = db.student_course_lesson_progress.Where(w => w.course_id == course_id && w.course_lesson_id == course_lesson_id && w.student_id == get_student_id.id).FirstOrDefault();
                if (get_student_progress == null)
                {
                    student_course_lesson_progress student_course_lesson_progress = new student_course_lesson_progress();
                    student_course_lesson_progress.student_id = (int)get_student_id.id;
                    student_course_lesson_progress.course_id = course_id;
                    student_course_lesson_progress.course_lesson_id = course_lesson_id;
                    student_course_lesson_progress.is_done_lesson = 0;
                    student_course_lesson_progress.is_done_exercise = 1;

                    db.student_course_lesson_progress.Add(student_course_lesson_progress);
                    db.SaveChanges();
                    output = student_course_lesson_progress;
                }
                else
                {
                    get_student_progress.is_done_exercise = 1;
                    db.SaveChanges();
                    output = get_student_progress;
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic put_stamp_video_lesson(string course_id, int course_lesson_id, decimal video_position, decimal video_progress, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student_id == null)
                {
                    error_message = "ไม่มีนักเรียนในคอร์สนี้";
                    return false;
                }

                dynamic output_video = null;
                dynamic output_progress = null;

                #region student_video_progress
                var get_video_progress = db.student_video_progress.Where(w => w.student_id == get_student_id.id && w.course_id == course_id && w.course_lesson_id == course_lesson_id).FirstOrDefault();
                if (get_video_progress == null)
                {
                    student_video_progress student_video_progress = new student_video_progress();
                    student_video_progress.student_id = get_student_id.id;
                    student_video_progress.course_id = course_id;
                    student_video_progress.course_lesson_id = course_lesson_id;
                    student_video_progress.video_path = "";
                    student_video_progress.video_position = video_position;
                    student_video_progress.video_progress = video_progress;
                    student_video_progress.create_by = auth.user_id;
                    student_video_progress.create_at = now;
                    student_video_progress.update_by = auth.user_id;
                    student_video_progress.update_dt = now;
                    db.student_video_progress.Add(student_video_progress);
                    db.SaveChanges();
                    output_video = student_video_progress;
                }
                else
                {
                    get_video_progress.video_position = video_position;
                    get_video_progress.video_progress = video_progress;
                    get_video_progress.update_by = auth.user_id;
                    get_video_progress.update_dt = now;
                    db.SaveChanges();
                    output_video = get_video_progress;
                }
                #endregion

                #region student_course_lesson_progress
                var check_course_exercise = db.course_lesson_exercise.Where(w => w.course_id == course_id && w.course_lesson_id == course_lesson_id && w.is_deleted == 0).Count();
                var get_student_progress = db.student_course_lesson_progress.Where(w => w.course_id == course_id && w.course_lesson_id == course_lesson_id && w.student_id == get_student_id.id).FirstOrDefault();
                var is_done_lesson = video_progress >= 90 ? 1 : 0;
                if (get_student_progress == null)
                {
                    student_course_lesson_progress student_course_lesson_progress = new student_course_lesson_progress();
                    student_course_lesson_progress.student_id = (int)get_student_id.id;
                    student_course_lesson_progress.course_id = course_id;
                    student_course_lesson_progress.course_lesson_id = course_lesson_id;
                    student_course_lesson_progress.is_done_lesson = is_done_lesson;
                    student_course_lesson_progress.is_done_exercise = check_course_exercise > 0 ? 0 : 1;

                    db.student_course_lesson_progress.Add(student_course_lesson_progress);
                    db.SaveChanges();
                    output_progress = student_course_lesson_progress;
                }
                else
                {
                    is_done_lesson = (get_student_progress.is_done_lesson == 1 ? 1 : is_done_lesson);
                    get_student_progress.is_done_lesson = is_done_lesson;
                    get_student_progress.is_done_exercise = check_course_exercise > 0 ? get_student_progress.is_done_exercise : 1;
                    db.SaveChanges();
                    output_progress = get_student_progress;
                }
                #endregion

                output = new
                {
                    video_progress = output_video,
                    progress = output_progress
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic post_send_answer_exam(bool is_pretest, string course_id, List<model_exam_logging> answer, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student_id == null)
                {
                    error_message = "ไม่มีนักเรียนในคอร์สนี้";
                    return false;
                }

                var check_exam = db.course_exam_logging.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.is_pretest == 1 && w.student_id == get_student_id.id).ToList();
                if (is_pretest && check_exam.Count > 0)
                {
                    error_message = "เคยทำแบบทดสอบก่อนเรียนไปแล้ว";
                    return false;
                }

                var score = 0;
                dynamic percent = 0;
                var get_exam = db.course_exam.Where(w => w.is_deleted == 0 && w.course_id == course_id).ToList();
                var get_check_exam_logging = db.course_exam_logging.Where(w => w.is_deleted == 0 && w.student_id == get_student_id.id && w.course_id == course_id).ToList();
                var list_result = new List<object>();
                var is_pass = false;
                foreach (var item in answer)
                {
                    var check_correct = get_exam.Where(w => w.id == item.course_exam_id && w.answer == item.answer).FirstOrDefault();
                    var check_duplicate = is_pretest ? true : (get_check_exam_logging.Where(w => w.is_pretest == 0 && w.score == 1 && w.course_exam_id == item.course_exam_id && w.course_exam_answer_id == item.answer).Count() > 0 ? false : true);

                    if (check_duplicate)
                    {
                        course_exam_logging course_exam_logging = new course_exam_logging();
                        course_exam_logging.student_id = get_student_id.id;
                        course_exam_logging.course_id = course_id;
                        course_exam_logging.is_pretest = is_pretest ? 1 : 0;
                        course_exam_logging.course_exam_answer_id = item.answer;
                        course_exam_logging.course_exam_id = item.course_exam_id;
                        course_exam_logging.score = (check_correct == null ? 0 : 1);
                        course_exam_logging.is_deleted = 0;
                        course_exam_logging.created_by = auth.user_id;
                        course_exam_logging.created_dt = now;
                        course_exam_logging.update_by = auth.user_id;
                        course_exam_logging.update_dt = now;

                        db.course_exam_logging.Add(course_exam_logging);
                        db.SaveChanges();

                        list_result.Add(new { status = (check_correct == null ? false : true), course_exam_id = item.course_exam_id });
                    }
                }

                #region Check score
                var get_exam_logging = db.course_exam_logging.Where(w => w.is_deleted == 0 && w.course_id == course_id && w.student_id == get_student_id.id).ToList();
                if (is_pretest)
                {
                    score = get_exam_logging.Where(w => w.is_pretest == 1 && get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0).Count();
                    percent = (((double)score / get_exam.Count()) * 100);
                    is_pass = true;
                }
                else
                {
                    score = get_exam_logging.Where(w => w.is_pretest == 0 && get_exam.Where(wex => wex.id == w.course_exam_id && wex.answer == w.course_exam_answer_id).Count() > 0).Count();
                    percent = (((double)score / get_exam.Count()) * 100);
                    var get_percent = db.course.Where(w => w.id == course_id).Select(s => s.passed_percent).FirstOrDefault();
                    if (percent >= get_percent)
                    {
                        is_pass = true;
                        var check_cert = db.certificate_info.Where(w => w.course_id == course_id && w.student_id == get_student_id.id && w.cert_status == "Y").FirstOrDefault();
                        if (check_cert == null)
                        {
                            var student = new Student();
                            var manage = new Management.Management();

                            var errorMsg = "";
                            student.GenereteCertificate(auth.student_id, course_id, ref errorMsg);
                            manage.SendingEmail(auth.student_id, course_id, "04", null, auth, null, ref errorMsg);
                        }
                    }
                }
                #endregion

                output = new
                {
                    score,
                    total_exam = get_exam.Count(),
                    percent,
                    list_result,
                    is_pass
                };
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        //success wait test
        public dynamic post_send_answer_evaluation(string course_id, List<model_evaluation_result> answer, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student_id == null)
                {
                    error_message = "ไม่มีนักเรียนในคอร์สนี้";
                    return false;
                }

                var check_evaluation = db.course_evaluation_result.Where(w => w.is_deleted == 0 && w.student_id == get_student_id.id && w.course_id == course_id).ToList();
                if (check_evaluation.Count > 0)
                {
                    error_message = "เคยทำแบบประเมินหลักสูตรไปแล้ว";
                    return false;
                }

                var data = new List<object>();
                foreach (var item in answer)
                {
                    course_evaluation_result course_evaluation_result = new course_evaluation_result();
                    course_evaluation_result.course_evaluation_id = item.course_evaluation_id;
                    course_evaluation_result.student_id = get_student_id.id;
                    course_evaluation_result.course_id = course_id;
                    course_evaluation_result.course_evaluation_choices_id = item.course_evaluation_choices_id;
                    course_evaluation_result.answer = item.answer;
                    course_evaluation_result.is_deleted = 0;
                    course_evaluation_result.created_by = get_student_id.id;
                    course_evaluation_result.created_dt = now;
                    course_evaluation_result.update_by = get_student_id.id;
                    course_evaluation_result.update_dt = now;

                    db.course_evaluation_result.Add(course_evaluation_result);
                    db.SaveChanges();
                    data.Add(course_evaluation_result);
                }

                output = data;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic ramdom_exam(List<model_exam_platform> exam)
        {
            var random = new Random();
            var data = exam.OrderBy(o => Guid.NewGuid().ToString() + random.Next(exam.Count()).ToString()).ToList();
            for (var i = 0; i < data.Count(); i++)
            {
                var get_data = data[i];
                get_data.list_answer = get_data.list_answer.OrderBy(o => Guid.NewGuid().ToString() + random.Next(get_data.list_answer.Count()).ToString()).ToList();

                data[i] = get_data;
            }
            return data;
        }

        public dynamic get_my_course(Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student_id == null)
                {
                    error_message = "ไม่พบนักเรียนคนนี้ในระบบ";
                    return false;
                }

                var data = (from s_info in db.student_course_info
                            join c in db.course.Where(w => w.is_deleted == 0) on s_info.course_id equals c.id
                            where s_info.student_id == get_student_id.id && s_info.is_deleted == 0
                            select new
                            {
                                id = c.id,
                                name = c.name,
                                sub_name = c.sub_name,
                                cover = c.cover_pic,
                                batch = c.batch,
                                count_lesson = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == c.id).Count(),
                                course_time = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == c.id).Sum(sum => sum.lesson_time),
                                learning_end_date = c.learning_enddate,
                                progress = (db.course_lesson.Where(w => w.is_deleted == 0 && 
                                                                        w.course_id == c.id).Count() > 0 ? 
                                                                            (((double)db.student_course_lesson_progress.Where(w => w.student_id == get_student_id.id && 
                                                                                                                                   w.course_id == c.id && 
                                                                                                                                   w.is_done_exercise == 1 && 
                                                                                                                                   w.is_done_lesson == 1).Count() / db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == c.id).Count()) * 100) 
                                                                            : 0)
                            }).ToList();

                output = data;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }
        
        public dynamic post_register_course_free(string course_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student_id == null)
                {
                    error_message = "ไม่พบนักเรียนคนนี้ในระบบ";
                    return false;
                }

                if (get_student_id.student_account_type_id == 1)
                {
                    error_message = "กรุณาอัพโหลดเอกสาร";
                    return false;
                }

                var check_student_info = db.student_course_info.Where(w => w.is_deleted == 0 && w.student_id == get_student_id.id && w.course_id == course_id).FirstOrDefault();
                if (check_student_info != null)
                {
                    error_message = "นักเรียนคนนี้ลงทะเบียน course นี้ไปแล้ว";
                    return false;
                }

                var get_course = db.course.Where(w => w.is_deleted == 0 && w.id == course_id).FirstOrDefault();
                if(get_course == null)
                {
                    error_message = "ไม่มี course นี้ในระบบ";
                    return false;
                }

                student_course_info student_course_info = new student_course_info();
                student_course_info.student_id = get_student_id.id;
                student_course_info.course_id = course_id;
                student_course_info.course_lesson_id = 0;
                student_course_info.cert_print_count = 0;

                student_course_info.is_extend_study_time = 0;
                student_course_info.learning_startdate = get_course.learning_startdate;
                student_course_info.learning_enddate = get_course.learning_enddate;

                student_course_info.is_deleted = 0;
                student_course_info.created_by = auth.user_id;
                student_course_info.created_dt = now;
                student_course_info.update_by = auth.user_id;
                student_course_info.update_dt = now;
                db.student_course_info.Add(student_course_info);
                db.SaveChanges();

                output = student_course_info;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic post_register_course_voucher(string course_id, string voucher_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                var datenow = DateTime.ParseExact(now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", provider);

                var check_voucher = db.course_voucher.Where(w => w.is_delete == 0 && w.course_id == course_id && w.voucher_id == voucher_id).FirstOrDefault();
                if(check_voucher == null)
                {
                    error_message = "ไม่พบ Voucher นี้ในระบบ";
                    return false;
                }

                if(check_voucher.start_dt != null && check_voucher.end_dt != null && check_voucher.start_dt > datenow)
                {
                    error_message = "ยังไม่ถึงช่วงเวลาใช้ Voucher นี้";
                    return false;
                }

                if (check_voucher.start_dt != null && check_voucher.end_dt != null && check_voucher.end_dt < datenow)
                {
                    error_message = "Voucher นี้หมดอายุแล้ว";
                    return false;
                }

                if (check_voucher.status == 1)
                {
                    error_message = "Voucher นี้ถูกใช้ไปแล้ว";
                    return false;
                }

                var get_student_id = db.student.Where(w => w.user_id == auth.user_id && w.is_deleted == 0).FirstOrDefault();
                if (get_student_id == null)
                {
                    error_message = "ไม่พบนักเรียนคนนี้ในระบบ";
                    return false;
                }

                if (get_student_id.student_account_type_id == 1)
                {
                    error_message = "กรุณาอัพโหลดเอกสาร";
                    return false;
                }

                var check_student_info = db.student_course_info.Where(w => w.is_deleted == 0 && w.student_id == get_student_id.id && w.course_id == course_id).FirstOrDefault();
                if (check_student_info != null)
                {
                    error_message = "นักเรียนคนนี้ลงทะเบียน course นี้ไปแล้ว";
                    return false;
                }

                var get_course = db.course.Where(w => w.is_deleted == 0 && w.id == course_id).FirstOrDefault();
                if (get_course == null)
                {
                    error_message = "ไม่มี course นี้ในระบบ";
                    return false;
                }

                student_course_info student_course_info = new student_course_info();
                student_course_info.student_id = get_student_id.id;
                student_course_info.course_id = course_id;
                student_course_info.course_lesson_id = 0;
                student_course_info.cert_print_count = 0;

                student_course_info.is_extend_study_time = 0;
                student_course_info.learning_startdate = get_course.learning_startdate;
                student_course_info.learning_enddate = get_course.learning_enddate;

                student_course_info.is_deleted = 0;
                student_course_info.created_by = auth.user_id;
                student_course_info.created_dt = now;
                student_course_info.update_by = auth.user_id;
                student_course_info.update_dt = now;
                db.student_course_info.Add(student_course_info);
                db.SaveChanges();

                check_voucher.status = 1;
                check_voucher.update_by = auth.user_id;
                check_voucher.update_dt = now;
                db.SaveChanges();

                output = student_course_info;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        public dynamic post_count_view_lesson(int lesson_id, Authentication.Authentication auth, ref string error_message)
        {
            dynamic output = null;
            try
            {
                var get_course = db.course_lesson.Where(w => w.is_deleted == 0 && w.id == lesson_id).FirstOrDefault();
                if (get_course == null)
                {
                    error_message = "ไม่มีบทเรียนนี้ในระบบ";
                    return false;
                }

                get_course.count_view = get_course.count_view == null ? 1 : (get_course.count_view + 1);
                db.SaveChanges();

                output = get_course.count_view;
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
            }
            return output;
        }

        #endregion

        public dynamic check_file_course(string filename, int type_column, int type_file, ref string error_message)
        {
            dynamic output = null;
            dynamic get_data = null;
            try
            {
                switch (type_column)
                {
                    case 1:
                        get_data = type_file == 1 ? (db.course.Where(w => w.is_deleted == 0 && (w.cover_pic == filename || w.info_cover == filename)).Count()) : (db.course.Where(w => w.is_deleted == 0 && w.video_sample == filename).Count());
                        output = get_data > 1 ? false : true;
                        break;
                    case 2:
                        get_data = db.course_lesson.Where(w => w.is_deleted == 0 && w.main_video == filename).Count();
                        output = get_data > 1 ? false : true;
                        break;
                    case 3:
                        get_data = type_file == 1 ? (db.course_lesson_exercise.Where(w => w.is_deleted == 0 && w.image == filename).Count()) : (db.course_lesson_exercise.Where(w => w.is_deleted == 0 && w.video == filename).Count());
                        output = get_data > 1 ? false : true;
                        break;
                    case 4:
                        get_data = type_file == 1 ? (db.course_exam.Where(w => w.is_deleted == 0 && w.image == filename).Count()) : (db.course_exam.Where(w => w.is_deleted == 0 && w.video == filename).Count());
                        output = get_data > 1 ? false : true;
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message; ErrorList(ex);
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