using System.Data.Entity;
using DpimProject.Models.Data.DataModels;
using DpimProject.Security;
using Newtonsoft.Json;
using System;
namespace DpimProject.Models.Data
{
    public class Ef6Config : DbConfiguration
    {
        //public Ef6Config()
        //{
        //    if (System.Configuration.ConfigurationManager.ConnectionStrings["dbProvider"]?.ConnectionString.ToLower() == "sap")
        //    {
        //        this.SetDefaultConnectionFactory(new Sap.Data.SQLAnywhere.SAConnectionFactory());

        //        this.SetProviderServices("Sap.Data.SQLAnywhere", Sap.Data.SQLAnywhere.SAProviderServices.Instance);
        //        this.SetProviderFactory("Sap.Data.SQLAnywhere", Sap.Data.SQLAnywhere.SAFactory.Instance);

        //        SetDatabaseInitializer<DataContext>(null);
        //    }
        //    //else
        //    //if (System.Configuration.ConfigurationManager.ConnectionStrings["dbProvider"]?.ConnectionString.ToLower() == "sap")
        //    //{
        //    //    this.SetDefaultConnectionFactory(new iAnywhere.Data.SQLAnywhere.SAConnectionFactory());

        //    //    this.SetProviderServices("iAnywhere.Data.SQLAnywhere", iAnywhere.Data.SQLAnywhere.SAProviderServices.Instance);
        //    //    this.SetProviderFactory("iAnywhere.Data.SQLAnywhere", iAnywhere.Data.SQLAnywhere.SAFactory.Instance);

        //    //    SetDatabaseInitializer<Database>(null);
        //    //}
        //}
    }

    [DbConfigurationType(typeof(Ef6Config))]
    public partial class DataContext : DbContext
    {
        private static string connectionS = Licence.ConnectionString;

        public DataContext()
        : base(ConnectionString)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //if (System.Configuration.ConfigurationManager.ConnectionStrings["dbProvider"]?.ConnectionString.ToLower() == "sap")
            //{
            //    //System.Data.Entity.Database.DefaultConnectionFactory = new Sap.Data.SQLAnywhere.SAConnectionFactory();
            //    System.Data.Entity.Database.SetInitializer<DataContext>(null);
            //}
        }

        #region New DB
        public virtual DbSet<course> course { get; set; }
        public virtual DbSet<group_department> group_department { get; set; }
        public virtual DbSet<admin_menu> admin_menu { get; set; }
        public virtual DbSet<menu_permission> menu_permission { get; set; }
        public virtual DbSet<video_status> video_status { get; set; }
        public virtual DbSet<app_auth> app_auth { get; set; }
        public virtual DbSet<online_user> online_user { get; set; }
        public virtual DbSet<course_lesson> course_lesson { get; set; }
        public virtual DbSet<instructor> instructor { get; set; }
        public virtual DbSet<course_voucher> course_voucher { get; set; }
        public virtual DbSet<course_lesson_exercise> course_lesson_exercise { get; set; }
        public virtual DbSet<course_lesson_exercise_answer_match> course_lesson_exercise_answer_match { get; set; }
        public virtual DbSet<course_lesson_exercise_answer_choices> course_lesson_exercise_answer_choices { get; set; }
        public virtual DbSet<course_exam> course_exam { get; set; }
        public virtual DbSet<certificate_print> certificate_print { get; set; }
        public virtual DbSet<certificate_type> certificate_type { get; set; }
        public virtual DbSet<certificate_info> certificate_info { get; set; }
        public virtual DbSet<email_admin> email_admin { get; set; }
        public virtual DbSet<email_sending> email_sending { get; set; }
        public virtual DbSet<email_type> email_type { get; set; }
        public virtual DbSet<course_exam_answer> course_exam_answer { get; set; }
        public virtual DbSet<course_permission> course_permission { get; set; }
        public virtual DbSet<course_evaluation> course_evaluation { get; set; }
        public virtual DbSet<course_evaluation_choices> course_evaluation_choices { get; set; }
        public virtual DbSet<course_evaluation_result> course_evaluation_result { get; set; }
        public virtual DbSet<student_course_info> student_course_info { get; set; }
        public virtual DbSet<student_video_progress> student_video_progress { get; set; }
        public virtual DbSet<student_course_lesson_progress> student_course_lesson_progress { get; set; }
        public virtual DbSet<course_exam_setting> course_exam_setting { get; set; }
        public virtual DbSet<course_exam_type> course_exam_type { get; set; }
        public virtual DbSet<course_exam_logging> course_exam_logging { get; set; }
        public virtual DbSet<student> student { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<role> role { get; set; }
        public virtual DbSet<student_forget_password_token> student_forget_password_token { get; set; }
        public virtual DbSet<file_upload> file_upload { get; set; }
        public virtual DbSet<faq_type> faq_type { get; set; }
        public virtual DbSet<course_category> course_category { get; set; }
        public virtual DbSet<sub_districts> sub_districts { get; set; }
        public virtual DbSet<districts> districts { get; set; }
        public virtual DbSet<provinces> provinces { get; set; }
        public virtual DbSet<zipcode> zipcode { get; set; }
        public virtual DbSet<business_type> business_type { get; set; }
        public virtual DbSet<career> career { get; set; }
        public virtual DbSet<know_channel> know_channel { get; set; }
        public virtual DbSet<student_account_type> student_account_type { get; set; }
        public virtual DbSet<educational> educational { get; set; }
        public virtual DbSet<problem_of_use> problem_of_use { get; set; }
        public virtual DbSet<website_info> website_info { get; set; }
        public virtual DbSet<tutorial_header> tutorial_header { get; set; }
        public virtual DbSet<tutorial_detail> tutorial_detail { get; set; }
        public virtual DbSet<faq> faq { get; set; }
        public virtual DbSet<banner> banner { get; set; }
        public virtual DbSet<recommend_site> recommend_site { get; set; }
        public virtual DbSet<video_on_demand> video_on_demand { get; set; }
        public virtual DbSet<public_new> public_new { get; set; }
        public virtual DbSet<manage_profile> manage_profile { get; set; }
        public virtual DbSet<payment_method> payment_method { get; set; }
        public virtual DbSet<report_problem> report_problem { get; set; }
        public virtual DbSet<mapping_student_know_channel> mapping_student_know_channel { get; set; }
        public virtual DbSet<certificate> certificate { get; set; }

        //new 08-01-2021
        public virtual DbSet<interactive_question> interactive_question { get; set; }
        public virtual DbSet<interactive_answer> interactive_answer { get; set; }
        public virtual DbSet<special_days> special_days { get; set; }
        public virtual DbSet<department> department { get; set; }

        //new 23-01-2021
        public virtual DbSet<content_permission> content_permission { get; set; }
        #endregion




        public virtual DbSet<permission> permission { get; set; }
        public virtual DbSet<user_permission> user_permission { get; set; }
        public virtual DbSet<user_online> user_online { get; set; }
        //public virtual DbSet<catagory> catagory { get; set; }
        //public virtual DbSet<certificate> certificate { get; set; }
        //public virtual DbSet<course_exam_detail> course_exam_detail { get; set; }
        //public virtual DbSet<course_exam_form> course_exam_form { get; set; }
        //public virtual DbSet<course_exam_header> course_exam_header { get; set; }
        //public virtual DbSet<course_module> course_module { get; set; }
        //public virtual DbSet<faq_group> faq_group { get; set; }
        //public virtual DbSet<faq_header> faq_header { get; set; }
        //public virtual DbSet<learn_status> learn_status { get; set; }
        //public virtual DbSet<lesson_test_detail> lesson_test_detail { get; set; }
        //public virtual DbSet<lesson_test_form> lesson_test_form { get; set; }
        //public virtual DbSet<lesson_test_header> lesson_test_header { get; set; }
        //public virtual DbSet<monitoring> monitoring { get; set; }
        //public virtual DbSet<monitoring_status> monitoring_status { get; set; }
        //public virtual DbSet<priority_type> priority_type { get; set; }
        //public virtual DbSet<question_type> question_type { get; set; }
        //public virtual DbSet<questionnaire_detail> questionnaire_detail { get; set; }
        //public virtual DbSet<questionnaire_form> questionnaire_form { get; set; }
        //public virtual DbSet<questionnaire_header> questionnaire_header { get; set; }
        //public virtual DbSet<student_course> student_course { get; set; }
        //public virtual DbSet<student_course_exam> student_course_exam { get; set; }
        //public virtual DbSet<student_group> student_group { get; set; }
        //public virtual DbSet<student_lesson_learn> student_lesson_learn { get; set; }
        //public virtual DbSet<student_lesson_test> student_lesson_test { get; set; }
        //public virtual DbSet<student_questionnaire> student_questionnaire { get; set; }




        private static string conn_str = "";

        public static string ConnectionString
        {

            get
            {
                var t = System.Configuration.ConfigurationManager.ConnectionStrings["vmConnection"]?.ConnectionString ?? "";
                //throw new Exception(t);
                var tmp = new { connection_string = "", max_user = 20 };

                try
                {
                    new Token().CheckToken(t, out t);
                    tmp = JsonConvert.DeserializeAnonymousType(t, tmp);
                    conn_str = tmp?.connection_string ?? "";
                }
                catch (Exception ex)
                {
                }

                return conn_str;
            }
        }

    }
}