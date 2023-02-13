using System;
using Newtonsoft.Json;
using DpimProject.Security;
using System.Web;
namespace DpimProject
{
    public static class Licence
    {
        private static int maxuser = 2;
        private static string conn_str = "";

        public static int MaxUser
        {
            get
            {
                //var t = System.Configuration.ConfigurationManager.AppSettings["KeyPath"];
                var t = System.Configuration.ConfigurationManager.ConnectionStrings["vmConnection"]?.ConnectionString ?? "";
                var tmp = new { connection_string = "", max_user = 20 };

                //try
                //{
                new Token().CheckToken(t, out t);
                tmp = JsonConvert.DeserializeAnonymousType(t, tmp);
                maxuser = tmp?.max_user ?? 2;
                //}
                //catch (Exception ex)
                //{
                //}

                return maxuser;
            }
        }

        //public static string ConnectionString
        //{
        //    get
        //    {
        //        var t = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "";
        //        //var t = System.Configuration.ConfigurationManager.AppSettings["KeyPath"];

        //        var tmp = new { connection_string = "", max_user = 20 };

        //        try
        //        {
        //            new Token().CheckToken(t, out t);

        //            tmp = JsonConvert.DeserializeAnonymousType(t, tmp);

        //            conn_str = tmp?.connection_string ?? "";
        //        }
        //        catch (Exception ex)
        //        {
        //        }

        //        return conn_str;
        //    }
        //}
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