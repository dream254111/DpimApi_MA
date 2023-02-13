using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit.Util;
using System.Web.Mvc;
using System.Threading.Tasks;
using NReco.VideoConverter;
using System.Diagnostics;
using System.Net.Http.Formatting;
namespace DpimProject.Models.DataTools
{

        public static class Dtl
        {
            private static DataTools d;

            static Dtl()
            {
                d = new DataTools();
            }

            public static string convert_pre_event2(string pre_event)
            {
                pre_event = pre_event ?? "";
                pre_event = pre_event.Trim();
                if (pre_event.Length > 7)
                {
                    return d.Convert2PreEvent2(pre_event);
                }
                else
                {
                    return null;
                }
            }

            public static string convert_pre_event0(string pre_event2)
            {
                pre_event2 = pre_event2 ?? "";
                pre_event2 = pre_event2.Trim();
                if (pre_event2.Length == 7)
                {
                    return d.Convert2PreEvent(pre_event2);
                }
                else if (pre_event2.Length > 7)
                {
                    return d.Convert2PreEvent(d.Convert2PreEvent2(pre_event2));
                }
                else
                {
                    return null;
                }
            }

            public static decimal? parse_dec(object n)
            {
                return d.ObjToDecimal(n);
            }

            public static int? parse_int(object n)
            {
                return d.ObjToInt(n);
            }

            public static DateTime? parse_date(object date)
            {
                return d.ObjToDateTime(date);
            }

            public static string json_stringify(object o)
            {
                var json = "";
                try
                {
                    json = JsonConvert.SerializeObject(o);
                }
                catch { }
                return json;
            }

            public static string json_request()
            {
                var json = "";
                try
                {
                    json = d.JsonRequest();
                }
                catch { }
                return json;
            }

            private static bool IsGenericList(this object o)
            {
                var oType = o.GetType();
                return (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)));
            }

            private static bool CheckIfAnonymousType(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");

                // HACK: The only way to detect anonymous types right now.
                return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                    && type.IsGenericType && type.Name.Contains("AnonymousType")
                    && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                    && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
            }

            public static T json_to_object<T>(string json, T obj) where T : class
            {
                var cc = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                {
                    Converters = new[] { new JsonCoverter1() }
                });
                return cc;
            }

            public static string thai_baht_text(decimal? amount)
            {
                return d.ThaiBathText(amount);
            }

            public static T TrimUpString<T>(T obj) where T : class
            {
                var obj2 = new List<object>();
                obj.GetType().GetProperties().ToList().ForEach(x =>
                {
                    if (x.PropertyType == typeof(string))
                    {
                        obj2.Add(x.GetValue(obj)?.ToString().Trim());
                    }
                    else
                    {
                        obj2.Add(x.GetValue(obj));
                    }
                });
                return (T)Activator.CreateInstance(obj.GetType(), obj2.ToArray());
            }

            private class JsonCoverter1 : JsonConverter
            {
                public override bool CanConvert(Type objectType)
                {
                    var _types = new[] {
                typeof(string),
                typeof(int),
                typeof(int?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(DateTime),
                typeof(DateTime?),
                //typeof(bool?),
                //typeof(bool),
            };
                    return _types.Any(x => x == objectType);
                }

                public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
                {
                    var val = reader.Value?.ToString();
                    if (objectType == typeof(string))
                    {
                        return val;
                    }
                    else if (objectType == typeof(decimal?) || objectType == typeof(decimal))
                    {
                        return objectType == typeof(decimal?) ? Dtl.parse_dec(val) : Dtl.parse_dec(val) ?? 0;
                    }
                    else if (objectType == typeof(double?) || objectType == typeof(double))
                    {
                        return objectType == typeof(double?) ? (double?)Dtl.parse_dec(val) : (double?)Dtl.parse_dec(val) ?? 0;
                    }
                    else if (objectType == typeof(int?) || objectType == typeof(int))
                    {
                        return objectType == typeof(int?) ? Dtl.parse_int(val) : Dtl.parse_int(val) ?? 0;
                    }
                    else if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
                    {
                        return objectType == typeof(DateTime?) ? Dtl.parse_date(val) : Dtl.parse_date(val) ?? DateTime.Now;
                    }
                    else
                        return null;
                }
          
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                {
                    throw new NotImplementedException();
                }
            }
        public static string CreateUserID(Data.DataContext db)
        {
            string userid = "";
            var user_max = db.user.ToList().Count() + 1;
            string run_user = "";
            if (user_max < 10)
            {
                run_user = "0" + user_max++;
            }
            else
            {
                run_user = user_max++.ToString();
            }
            //var last_user=  user_max.ToString();
            //var run_user=    Int32.Parse(last_user);

            userid = "U" + DateTime.Today.Year + DateTime.Today.Month.ToString("d2") + run_user;
            return userid;
        }

        public static string CreateCourseID(Data.DataContext db, string department_id, string course_category_id)
        {
            string course_id = "";
            string running = "";
            var count = db.course.Where(x => x.course_category_id == course_category_id).ToList().Count();
            if (count < 10)
            {
                running = $"00{count}" ;
            }
            else if (count < 100)
            {
                running = $"0{count}" ;
            }
            else
            {
                running = count.ToString();
            }
            course_id = $"{department_id}{course_category_id}{running}";
            return course_id;

        }
        //public static string CreateCompanyID(Data.DataContext db)
        //{
        //    string userid = "";
        //    var user_max = db.maincomp.ToList().Count() + 1;
        //    string run_user = "";
        //    if (user_max < 10)
        //    {
        //        run_user = "0" + user_max++;
        //    }
        //    else
        //    {
        //        run_user = user_max++.ToString();
        //    }
        //    //var last_user=  user_max.ToString();
        //    //var run_user=    Int32.Parse(last_user);

        //    userid = "M" + DateTime.Today.ToString("yy") + DateTime.Today.Month.ToString("d2") + run_user;
        //    return userid;
        //}
        //public static string CreateCategoryID(Data.DataContext db)
        //{
        //    string userid ="";
        //    var user_max = db.category.ToList().Count()+1;
        //    string run_user = "";
        //    if(user_max < 10)
        //    {
        //        run_user = "0" + user_max++;
        //    }
        //    else
        //    {
        //        run_user = user_max++.ToString();
        //    }
        //    //var last_user=  user_max.ToString();
        //    //var run_user=    Int32.Parse(last_user);

        //    userid = "C" + DateTime.Today.Year + DateTime.Today.Month.ToString("d2") + run_user;
        //    return userid;
        //}

        //public static string CreateSkillID(Data.DataContext db)
        //{
        //    string userid ="";
        //    var user_max = db.skill.ToList().Count()+1;
        //    string run_user = "";
        //    if(user_max < 10)
        //    {
        //        run_user = "0" + user_max++;
        //    }
        //    else
        //    {
        //        run_user = user_max++.ToString();
        //    }
        //    //var last_user=  user_max.ToString();
        //    //var run_user=    Int32.Parse(last_user);

        //    userid = "K" + DateTime.Today.Year + DateTime.Today.Month.ToString("d2") + run_user;
        //    return userid;
        //}
        //public static string CreateGradeID(Data.DataContext db)
        //{
        //    string userid ="";
        //    var user_max = db.grade.ToList().Count()+1;
        //    string run_user = "";
        //    if(user_max < 10)
        //    {
        //        run_user = "0" + user_max++;
        //    }
        //    else
        //    {
        //        run_user = user_max++.ToString();
        //    }
        //    //var last_user=  user_max.ToString();
        //    //var run_user=    Int32.Parse(last_user);

        //    userid = "G" + DateTime.Today.Year + DateTime.Today.Month.ToString("d2") + run_user;
        //    return userid;
        //}
        //public static string CreateLectureID(Data.DataContext db)
        //{
        //    string userid ="";
        //    var user_max = db.lecture.ToList().Count()+1;
        //    string run_user = "";
        //    if(user_max < 10)
        //    {
        //        run_user = "0" + user_max++;
        //    }
        //    else
        //    {
        //        run_user = user_max++.ToString();
        //    }
        //    //var last_user=  user_max.ToString();
        //    //var run_user=    Int32.Parse(last_user);

        //    userid = "L" + DateTime.Today.Year + DateTime.Today.Month.ToString("d2") + run_user;
        //    return userid;
        //}
        //public static string CreateCourseID(Data.DataContext db)
        //{
        //    string userid ="";
        //    var user_max = db.course.ToList().Count()+1;
        //    string run_user = "";
        //    if(user_max < 10)
        //    {
        //        run_user = "0" + user_max++;
        //    }
        //    else
        //    {
        //        run_user = user_max++.ToString();
        //    }
        //    //var last_user=  user_max.ToString();
        //    //var run_user=    Int32.Parse(last_user);

        //    userid = "S" + DateTime.Today.Year + DateTime.Today.Month.ToString("d2") + run_user;
        //    return userid;
        //}
        //public static string CreateSectionID(Data.DataContext db)
        //{
        //    string userid ="";
        //    var user_max = db.sections.ToList().Count()+1;
        //    string run_user = "";
        //    if(user_max < 10)
        //    {
        //        run_user = "0" + user_max++;
        //    }
        //    else
        //    {
        //        run_user = user_max++.ToString();
        //    }
        //    //var last_user=  user_max.ToString();
        //    //var run_user=    Int32.Parse(last_user);

        //    userid = "E" + DateTime.Today.Year + DateTime.Today.Month.ToString("d2") + run_user;
        //    return userid;
        //}
        //public static string CreateUserID(Data.DataContext db)
        //{
        //    var user_id = "";

        //    var int_id = db.register.Where(x => x.add_date.Value.Year == DateTime.Now.Year).Count()+1;
        //    var max_id = "";
        //    var year = DateTime.Today.Year.ToString();
        //    var month = DateTime.Today.ToString("MM");

        //    if (int_id < 10)
        //    {
        //        max_id = "00" + int_id;
        //    }
        //    if (int_id > 10 && int_id < 100)
        //    {
        //        max_id = "0" + int_id;

        //    }
        //    if (int_id > 100 && int_id < 1000)
        //    {
        //        max_id = int_id.ToString();
        //    }

        //    user_id = $@"U{year}{month}{max_id}";
        //    return user_id;


        //}
        //public static string CreateTopicID(Data.DataContext db,DataModels.topic_header m)
        //{
        //    var user_id = "";

        //    var m_id = db.topic_header.Where(x => x.add_date.Value.Year == DateTime.Now.Year).Select(x=>x.subject_id).Max();
        //    var int_id = "";
        //    if (m_id != null)
        //    {
        //        int_id = m_id.Substring(6, 3);
        //    }
        //    else
        //    {
        //        int_id = "001";
        //    }
        //   var max=  db.topic_header.Where(x => x.subject_id == m.subject_id).Count()+1 ;
        //    var max_id = "";
        //    var year = DateTime.Today.Year.ToString();
        //    var month = DateTime.Today.ToString("MM");

        //    if (max < 10)
        //    {
        //        max_id = "00" + max;
        //    }
        //    if (max > 10 && max < 100)
        //    {
        //        max_id = "0" + max;

        //    }
        //    if (max > 100 && max < 1000)
        //    {
        //        max_id = max.ToString();
        //    }

        //    user_id = $@"T{m.topic_type}{year}{month}{int_id}{max_id}";
        //    return user_id;


        //}
        //public static string CreateSubjectID(Data.DataContext db,DataModels.subject m)
        //{
        //    var user_id = "";

        //    var int_id = db.subject.Where(x => x.add_date.Value.Year == DateTime.Now.Year).Count() + 1;
        //    var max_id = "";
        //    var year = DateTime.Today.Year.ToString();
        //    var month = DateTime.Today.ToString("MM");

        //    if (int_id < 10)
        //    {
        //        max_id = "00" + int_id;
        //    }
        //    if (int_id > 10 && int_id < 100)
        //    {
        //        max_id = "0" + int_id;

        //    }
        //    if (int_id > 100 && int_id < 1000)
        //    {
        //        max_id = int_id.ToString();
        //    }

        //    user_id = $@"S{m.subject_id}{year}{month}{max_id}";
        //    return user_id;


        //}
        //public static string CreateFileID(Data.DataContext db)
        //{
        //    var user_id = "";

        //    var int_id = db.attachfile.Where(x => x.add_date.Value.Year == DateTime.Now.Year).Count() + 1;
        //    var max_id = "";
        //    var year = DateTime.Today.Year.ToString();
        //    var month = DateTime.Today.ToString("MM");

        //    if (int_id < 10)
        //    {
        //        max_id = "00" + int_id;
        //    }
        //    if (int_id > 10 && int_id < 100)
        //    {
        //        max_id = "0" + int_id;

        //    }
        //    if (int_id > 100 && int_id < 1000)
        //    {
        //        max_id = int_id.ToString();
        //    }

        //    user_id = $@"F{year}{month}{max_id}";
        //    return user_id;


        //}
        public static string ConvertToVirtualPath(string RealPath)
        {
            string vPath = "";
            try
            {
                vPath = System.IO.Path.GetFileName(RealPath);
                vPath = new UrlHelper(HttpContext.Current.Request.RequestContext).Content("~/file/load?filename=") + RealPath;
            }
            catch { }
            return vPath;
        }
        //public static string CreatePreEvent(Data.DataContext db)
        //{
        //    var pre_event = "";

        //    var max_id = db.chq_proj.Where(x=>x.add_dt.Value.Year==DateTime.Now.Year).Count()+1;

        //    var int_id = "";
        //    var year = DateTime.Today.Year.ToString();
        //    var month = DateTime.Today.ToString("MM");

        //    if (max_id < 10)
        //    {
        //        int_id = "00" + max_id;
        //    }
        //    if (max_id >10&&max_id < 100)
        //    {
        //        int_id = "0" + max_id;

        //    }
        //    if (max_id >100&&max_id < 1000)
        //    {
        //        int_id = max_id.ToString();
        //    }

        //    pre_event = $@"P{year}{month}{int_id}";
        //    return pre_event;


        //}
        //public static string CreateTopicID(Data.DataContext db)
        //{
        //    var topic_id = "";

        //    var max_id = db.topic.Where(x => x.add_dt.Value.Year == DateTime.Now.Year).Count()+1;
        //    var id = "";
        //    var year = DateTime.Today.Year.ToString();
        //    var month = DateTime.Today.ToString("MM");

        //    if (max_id < 10)
        //    {
        //        id = "00" + max_id;
        //    }
        //    if (max_id > 10 && max_id < 100)
        //    {
        //        id = "0" + max_id;

        //    }
        //    if (max_id > 100 && max_id < 1000)
        //    {
        //        id = max_id.ToString();
        //    }

        //    topic_id = $@"T{year}{month}{id}";
        //    return topic_id;


        //}
        public static string thai_month(int? month)
        {
            var output = "";
            switch (month)
            {
                case 1:
                    {
                        output = "มกราคม";
                    }break;
                case 2:
                    {
                        output = "กุมภาพันธ์";
                    }break; case 3:
                    {
                        output = "มีนาคม";
                    }break; case 4:
                    {
                        output = "เมษายน";
                    }break; case 5:
                    {
                        output = "พฤษภาคม";
                    }break; case 6:
                    {
                        output = "มิถุนายน";
                    }break; case 7:
                    {
                        output = "กรกฎาคม";
                    }break; case 8:
                    {
                        output = "สิงหาคม";
                    }break;case 9:
                    {
                        output = "กันยายน";
                    }break;case 10:
                    {
                        output = "ตุลาคม";
                    }break;case 11:
                    {
                        output = "พฤศจิกายน";
                    }break;case 12:
                    {
                        output = "ธันวาคม";
                    }break;
            }
        
            return output;
        }
        public static string arabic_to_thai(string num)
        {
            var output = "";
            StringBuilder s = new StringBuilder(num);
            if (num.Contains("1"))
            {
                s.Replace("1", "๑");
            }
            if (num.Contains("2"))
            {
                s.Replace("2", "๒");
            }
            if (num.Contains("3"))
            {
                s.Replace("3", "๓");
            }
            if (num.Contains("4"))
            {
                s.Replace("4", "๔");
            }
            if (num.Contains("5"))
            {
                s.Replace("5", "๕");
            }
            if (num.Contains("6"))
            {
                s.Replace("6", "๖");
            }
            if (num.Contains("7"))
            {
                s.Replace("7", "๗");
            }
            if (num.Contains("8"))
            {
                s.Replace("8", "๘");
            }
            if (num.Contains("9"))
            {
                s.Replace("9", "๙");
            }
            if (num.Contains("0"))
            {
                s.Replace("0", "๐");
            }
            output = s.ToString();
            return output;
        }

        public static DataTools X { get; } = new DataTools();
        }

        public class DataTools
        {
            public DataTools()
            {
            }

            public DateTime? DateFromExcelTime(object data)
            {
                if (data == null)
                {
                    return null;
                }

                if (ObjToDecimal(data) != null)
                {
                    try
                    {
                        return DateTime.FromOADate((double)(ObjToDecimal(data) ?? 0));
                    }
                    catch { }
                }

                return ObjToDateTime(data);
            }

            public List<int> CreateRange(int start, int end)
            {
                if (start < end)
                {
                    return Enumerable.Range(start, end + 1).ToList();
                }
                else if (end > start)
                {
                    return Enumerable.Range(end, start + 1).Reverse().ToList();
                }
                else
                {
                    return new int[] { start }.ToList();
                }
            }

            public List<object> JsonToModels(string JsonString, object ModelClass)
            {
                List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

                try
                {
                    dataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(JsonString);
                }
                catch
                {
                }

                if (dataList.Count == 0)
                {
                    try
                    {
                        dataList.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonString));
                    }
                    catch
                    {
                    }
                }

                return DictionaryToModel(dataList, ModelClass);
            }

            public List<object> DictionaryToModel(List<Dictionary<string, object>> dataList, object ModelClass)
            {
                var mList = new List<object>();
                foreach (var data in dataList)
                {
                    var m = Activator.CreateInstance(ModelClass.GetType());
                    foreach (var prop in (m.GetType()).GetProperties())
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            string val = data.ContainsKey(prop.Name) ? data[prop.Name]?.ToString() : null;
                            prop.SetValue(m, val);
                        }
                        else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                        {
                            decimal? val = null;
                            if (data.ContainsKey(prop.Name))
                            {
                                val = ObjToDecimal(data[prop.Name]);
                            }
                            prop.SetValue(m, val);
                        }
                        else if (prop.GetType() == typeof(double) || prop.GetType() == typeof(double?))
                        {
                            double? val = null;
                            if (data.ContainsKey(prop.Name)) val = (double?)ObjToDecimal(data[prop.Name]);
                            prop.SetValue(m, val);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            int? val = null;
                            if (data.ContainsKey(prop.Name)) val = ObjToInt(data[prop.Name]);
                            prop.SetValue(m, val);
                        }
                        else if (prop.PropertyType == typeof(DateTime?))
                        {
                            string dateStr = data.ContainsKey(prop.Name) && data[prop.Name] != null ? data[prop.Name].ToString() : "";
                            prop.SetValue(m, this.ObjToDateTime(dateStr));
                        }
                        else if (prop.PropertyType == typeof(TimeSpan?))
                        {
                            TimeSpan? val = null;
                            string dateStr = data.ContainsKey(prop.Name) && data[prop.Name] != null ? data[prop.Name].ToString() : "";
                            if (dateStr != "")
                            {
                                DateTime dt;
                                TimeSpan ts;
                                if (DateTime.TryParseExact(dateStr,
                                                       "HH:mm:ss",
                                                       CultureInfo.InvariantCulture,
                                                       DateTimeStyles.None,
                                                       out dt))
                                {
                                    dt.ToLocalTime();
                                    val = dt.TimeOfDay;
                                }
                                else if (TimeSpan.TryParseExact(dateStr,
                                                     "HH:mm:ss",
                                                     CultureInfo.InvariantCulture,
                                                     TimeSpanStyles.None,
                                                     out ts))
                                {
                                    val = ts;
                                }
                            }
                            prop.SetValue(m, val);
                        }
                    }
                    mList.Add(m);
                }
                return mList;
            }

            public List<object> DictionaryToModel(Dictionary<string, object> dataList, object ModelClass)
            {
                var o = new List<Dictionary<string, object>>();
                o.Add(dataList);
                return DictionaryToModel(o, ModelClass);
            }

            public string JsonRequest()
            {
                Stream req = HttpContext.Current.Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                var outp = new StreamReader(req).ReadToEnd();
                return outp;
            }

            public object PostToModel(NameValueCollection Form, object m)
            {
                m = Activator.CreateInstance(m.GetType());

                foreach (var prop in (m.GetType()).GetProperties())
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        string val = null;
                        if (Form[prop.Name] != null && Form[prop.Name] != "") val = Form[prop.Name].Trim();
                        prop.SetValue(m, val);
                    }
                    else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                    {
                        decimal? val = null;

                        val = ObjToDecimal(Form[prop.Name] ?? "");

                        prop.SetValue(m, val);
                    }
                    else if (prop.GetType() == typeof(double) || prop.GetType() == typeof(double?))
                    {
                        double? val = null;
                        val = (double?)ObjToDecimal(Form[prop.Name] ?? "");
                        prop.SetValue(m, val);
                    }
                    else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                    {
                        int? val = null;
                        val = ObjToInt(Form[prop.Name] ?? "");
                        prop.SetValue(m, val);
                    }
                    else if (prop.PropertyType == typeof(TimeSpan?))
                    {
                        TimeSpan? val = null;
                        string dateStr = Form[prop.Name] != null ? Form[prop.Name].ToString() : "";
                        if (dateStr != "")
                        {
                            DateTime dt;
                            TimeSpan ts;
                            if (DateTime.TryParseExact(dateStr,
                                                   "HH:mm:ss",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out dt))
                            {
                                dt.ToLocalTime();
                                val = dt.TimeOfDay;
                            }
                            else if (TimeSpan.TryParseExact(dateStr,
                                                 "HH:mm:ss",
                                                 CultureInfo.InvariantCulture,
                                                 TimeSpanStyles.None,
                                                 out ts))
                            {
                                val = ts;
                            }
                        }
                        prop.SetValue(m, val);
                    }
                    else
                    {
                        prop.SetValue(m, null);
                    }
                }

                return m;
            }

            public string Compress(string s)
            {
                var bytes = Encoding.Unicode.GetBytes(s);
                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        msi.CopyTo(gs);
                    }
                    return Convert.ToBase64String(mso.ToArray());
                }
            }

            public string Decompress(string s)
            {
                var bytes = Convert.FromBase64String(s);
                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        gs.CopyTo(mso);
                    }
                    return Encoding.Unicode.GetString(mso.ToArray());
                }
            }

            public NameValueCollection GetFormData()
            {
                NameValueCollection data = new NameValueCollection();

                foreach (string key in HttpContext.Current.Request.Form.Keys)
                {
                    data.Add(key, HttpContext.Current.Request.Form[key]);
                }

                foreach (string key in HttpContext.Current.Request.QueryString.Keys)
                {
                    if (data.AllKeys.Contains(key))
                    {
                        data[key] = HttpContext.Current.Request.QueryString[key];
                    }
                    else
                    {
                        data.Add(key, HttpContext.Current.Request.QueryString[key]);
                    }
                }

                return data;
            }

            public List<Dictionary<string, object>> ListModelsToDictionary(dynamic listModel)
            {
                var table = new List<Dictionary<string, object>>();

                if (listModel.Count == 0) return table;

                var m = listModel[0];

                foreach (var row in listModel)
                {
                    var rowObject = new Dictionary<string, object>();
                    foreach (var prop in m.GetType().GetProperties())
                    {
                        rowObject.Add(prop.Name, prop.GetValue(row));
                    }
                    table.Add(rowObject);
                }

                return table;
            }

            public NameValueCollection ModelToPostData(object m)
            {
                NameValueCollection postData = new NameValueCollection();
                foreach (var prop in m.GetType().GetProperties())
                {
                    if (prop.GetValue(m) == null || string.IsNullOrEmpty(prop.GetValue(m).ToString())) continue;
                    if (prop.GetType() == typeof(Nullable<DateTime>))
                    {
                        postData.Add(prop.Name, ((DateTime)prop.GetValue(m)).ToString("o"));
                    }
                    else
                    {
                        postData.Add(prop.Name, prop.GetValue(m).ToString().Trim());
                    }
                }
                return postData;
            }

            public int? ObjToInt(object o)
            {
                if (o == null)
                {
                    return null;
                }
                var D = ObjToDecimal(o);
                return D == null ? (int?)null : (int?)D;
            }

            public decimal? ObjToDecimal(object o)
            {
                if (o == null) return null;
                decimal? D = null;
                try
                {
                    D = Convert.ToDecimal(o);
                }
                catch {; }

                if (D == null)
                {
                    try
                    {
                        D = Decimal.Parse(o.ToString().Replace(" ", "").Replace(",", "").Trim());
                    }
                    catch {; }
                }
                return D == null ? D : Math.Round((decimal)D, 4);
            }

            public DateTime? ObjToDateTime(object x)
            {
                DateTime dt;
                if (x == null) return null;
                if (DateTime.TryParseExact(x.ToString(),
                                                   "d/M/yyyy",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out dt) || DateTime.TryParseExact(x.ToString(),
                                                   "dd/MM/yyyy",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out dt) ||
                                                   DateTime.TryParseExact(x.ToString(),
                                                   "dd/MM/yyyy HH:mm:ss",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out dt) ||
                                                   DateTime.TryParseExact(x.ToString(),
                                                   "dd/MM/yyyy HH:mm",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None, out dt
                                                   ) || DateTime.TryParse(x.ToString(), out dt))
                {
                    return dt;
                }

                return null;
            }

            public string Convert2PreEvent2(string pre_event)
            {
                if (pre_event != null && pre_event.Length > 6)
                {
                    var x = pre_event;
                    return x.Substring((x.Length - 4), 4) + x.Substring(0, 3);
                }
                else
                    return "";
            }

            public string Convert2PreEvent(string pre_event2)
            {
                if (pre_event2.Length > 5)
                {
                    var x = pre_event2;
                    return x.Substring(x.Length - 3, 3) + "0" + x.Substring(0, 4);
                }
                else
                    return "";
            }

            private string StringToCSVCell(string str, Type obj)
            {
                if (str == null) str = "";
                str = str.Trim();
                bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
                if (mustQuote)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\"");
                    foreach (char nextChar in str)
                    {
                        sb.Append(nextChar);
                        if (nextChar == '"')
                            sb.Append("\"");
                    }
                    sb.Append("\"");
                    return sb.ToString();
                }

                //if (str.IndexOf("0") == 0) {
                //    str = "=\"" + str + "\"";
                //}
                return str;
            }
        public  string CreateCourseID(Data.DataContext db, string department_id, string course_category_id)
        {
            string course_id = "";
            string running = "";
            var count = db.course.Where(x => x.course_category_id == course_category_id).ToList().Count()+1;
            if (count < 10)
            {
                running = $"00{count}";
            }
            else if (count < 100)
            {
                running = $"0{count}";
            }
            else
            {
                running = count.ToString();
            }
            course_id = $"{department_id}{course_category_id}{running}";
            return course_id;

        }

        public string CsvFromList(dynamic x)
            {
                string outp = "";
                if (x != null && x.Count > 0)
                {
                    foreach (object n in x)
                    {
                        outp += string.Join(",", n.GetType().GetProperties().ToList().Select(y => StringToCSVCell(y.GetValue(n)?.ToString() ?? "", y.GetType())).ToArray()) + System.Environment.NewLine;
                    }
                }

                return outp.Trim();
            }

            public dynamic MergeObject(object item1, object item2)
            {
                if (item1 == null || item2 == null)
                    return item1 ?? item2 ?? new ExpandoObject();

                dynamic expando = new ExpandoObject();
                var result = expando as IDictionary<string, object>;
                foreach (System.Reflection.PropertyInfo fi in item1.GetType().GetProperties())
                {
                    result[fi.Name] = fi.GetValue(item1, null);
                }
                foreach (System.Reflection.PropertyInfo fi in item2.GetType().GetProperties())
                {
                    result[fi.Name] = fi.GetValue(item2, null);
                }
                return result;
            }

            public string ConvertToVirtualPath(string RealPath)
            {
                string vPath = "";
                try
                {
                    vPath = System.IO.Path.GetFileName(RealPath);
                    vPath = new UrlHelper(HttpContext.Current.Request.RequestContext).Content("~/file/load?filename=") + vPath;
                }
                catch { }
                return vPath;
            }

            public string ThaiBathText(decimal? amount)
            {
                if (amount == 0)
                {
                    return " - ";
                }

                amount = amount ?? 0;
                var TxtNumArr = new string[] { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" }.ToList();
                var TxtDigitArr = new string[] { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน" }.ToList();

                amount = Math.Round((decimal)amount, 2);

                string[] amount_text = amount?.ToString("0.00").Split('.');
                string bath = amount_text.First();
                string stang = amount_text.Last();

                int digit = -1;
                int ml = -1;

                List<string> bathText = new List<string>();

                foreach (var x in bath.ToCharArray().Reverse())
                {
                    digit++;
                    if (x == '-')
                    {
                        bathText.Add("ลบ");
                    }
                    else if (new System.Text.RegularExpressions.Regex("[0-9]").IsMatch(x.ToString()))
                    {
                        var t = TxtNumArr[int.Parse(x.ToString())];

                        var d = digit % 6;

                        if (x == '0')
                        {
                            t = "";
                        }

                        if (d == 0)
                        {
                            ml++;
                            for (int i = 1; i <= ml; i++)
                            {
                                t += "ล้าน";
                            }
                        }
                        else if (x != '0')
                        {
                            t += TxtDigitArr[d];
                        }

                        bathText.Add(t);
                    }
                }
                bathText.Reverse();
                bathText = bathText.Where(x => !string.IsNullOrEmpty(x)).ToList();

                string b = string.Join("", bathText) + "บาท";
                if (b == "ลบบาท") b = "ลบ";
                b = b.Replace("หนึ่งสิบ", "สิบ");
                b = b.Replace("สองสิบ", "ยี่สิบ");
                b = b.Replace("สิบหนึ่ง", "สิบเอ็ด");

                b = bath == "0" ? "ศูนย์บาท" : b;

                List<string> stangText = new List<string>();
                digit = -1;
                foreach (var x in stang.ToCharArray().Reverse())
                {
                    digit++;
                    var d = digit % 2;
                    if (new System.Text.RegularExpressions.Regex("[0-9]").IsMatch(x.ToString()))
                    {
                        var t = TxtNumArr[int.Parse(x.ToString())];
                        if (x == '0')
                        {
                            t = "";
                        }
                        if (x != '0')
                        {
                            t += TxtDigitArr[d];
                        }
                        stangText.Add(t);
                    }
                }
                stangText.Reverse();
                stangText = stangText.Where(x => !string.IsNullOrEmpty(x)).ToList();
                string s = stangText.Count > 0 ? string.Join("", stangText) + "สตางค์" : "ถ้วน";
                s = s.Replace("หนึ่งสิบ", "สิบ");
                s = s.Replace("สองสิบ", "ยี่สิบ");
                s = s.Replace("สิบหนึ่ง", "สิบเอ็ด");

                return b + s;
            }

            public string ThaiLandText(decimal? land_space)
            {
                if (land_space == 0)
                {
                    return " - ";
                }

                land_space = land_space ?? 0;
                var TxtNumArr = new string[] { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" }.ToList();
                var TxtDigitArr = new string[] { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน" }.ToList();

                land_space = Math.Round((decimal)land_space, 2);

                string[] amount_text = land_space?.ToString("0.00").Split('.');
                string bath = amount_text.First();
                string stang = amount_text.Last();

                int digit = -1;
                int ml = -1;

                List<string> landText = new List<string>();

                foreach (var x in bath.ToCharArray().Reverse())
                {
                    digit++;
                    if (x == '-')
                    {
                        landText.Add("ลบ");
                    }
                    else if (new System.Text.RegularExpressions.Regex("[0-9]").IsMatch(x.ToString()))
                    {
                        var t = TxtNumArr[int.Parse(x.ToString())];

                        var d = digit % 6;

                        if (x == '0')
                        {
                            t = "";
                        }

                        if (d == 0)
                        {
                            ml++;
                            for (int i = 1; i <= ml; i++)
                            {
                                t += "ล้าน";
                            }
                        }
                        else if (x != '0')
                        {
                            t += TxtDigitArr[d];
                        }

                        landText.Add(t);
                    }
                }
                landText.Reverse();
                landText = landText.Where(x => !string.IsNullOrEmpty(x)).ToList();

                string b = string.Join("", landText) + "ตารางวา";
                if (b == "ลบบาท") b = "ลบ";
                b = b.Replace("หนึ่งสิบ", "สิบ");
                b = b.Replace("สองสิบ", "ยี่สิบ");
                b = b.Replace("สิบหนึ่ง", "สิบเอ็ด");

                b = bath == "0" ? "ศูนย์ตารางวา" : b;

                List<string> stangText = new List<string>();
                digit = -1;
                foreach (var x in stang.ToCharArray().Reverse())
                {
                    digit++;
                    var d = digit % 2;
                    if (new System.Text.RegularExpressions.Regex("[0-9]").IsMatch(x.ToString()))
                    {
                        var t = TxtNumArr[int.Parse(x.ToString())];
                        if (x == '0')
                        {
                            t = "";
                        }
                        if (x != '0')
                        {
                            t += TxtDigitArr[d];
                        }
                        stangText.Add(t);
                    }
                }

                return b;
            }

            public string DateThai(DateTime? date)
            {
                var th_cl = new CultureInfo("th-TH");
                var en_cl = new CultureInfo("en-US");
                if (date == null) return "";
                else
                {
                    return date.Value.ToString("d MMMM", th_cl) + " พ.ศ. " + ((date?.Year ?? 0) + 543);
                }
            }

            public string DateThai2(DateTime? date)
            {
                var th_cl = new CultureInfo("th-TH");
                if (date == null) return "";
                else
                {
                    return date.Value.ToString("d MMMM", th_cl) + " " + ((date?.Year ?? 0) + 543);
                }
            }

            public string DateEn(DateTime? date)
            {
                var th_cl = new CultureInfo("th-TH");
                var en_cl = new CultureInfo("en-US");
                if (date == null) return "";
                else
                {
                    return date.Value.ToString("d MMMM yyyy", en_cl);
                }
            }

            public string DateFormatYearTH(DateTime? date)
            {
                var th_cl = new CultureInfo("th-TH");
                if (date == null) return "";
                else
                {
                    return date.Value.ToString("dd/MM", th_cl) + "/" + ((date?.Year ?? 0) + 543);
                }
            }

            public int WeekOfYear(DateTime time, DayOfWeek day)
            {
                int w = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, day);
                if (time.Month == 1 && w >= 52)
                {
                    w = 0;
                }
                return w;
            }

            public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
            {
                int diff = dt.DayOfWeek - startOfWeek;
                if (diff < 0)
                {
                    diff += 7;
                }
                return dt.AddDays(-1 * diff).Date;
            }

            public class LandConvert
            {
                //private decimal land_space{ get; set; } = 0;
                public decimal sqr { get; set; } = 0;

                public int work { get; set; } = 0;
                public int rai { get; set; } = 0;

                public LandConvert(decimal land_space)
                {
                    rai = (int)(land_space / 400);
                    work = (int)(((land_space / 400) - rai) * 4);
                    sqr = (decimal)(land_space % 100M);
                }
            }

            public bool FileUpload(HttpPostedFileBase file, string path, string accept, bool create_sub_directory, ref string error_message, ref FileInfo file_info, bool img_resize = false)
            {
                file_info = null;
                if (file == null || file.ContentLength == 0)
                {
                    error_message = "No file upload";
                    return false;
                }
                if (!accept.Split(',').Select(x => (x ?? "").Trim().ToLower()).Where(x => !string.IsNullOrEmpty(x)).ToArray().Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    error_message = "File not support";
                    return false;
                }

                try
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    DateTime d = DateTime.Now;
                    //List<string> video_ext = new List<string>();
                    //video_ext.Add(".mp4");
                    //video_ext.Add(".flv");
                    //video_ext.Add(".3gp");
                    //video_ext.Add(".wma");

                    if (create_sub_directory)
                    {
                        path = (path + $"/{d.ToString("yyyyMMdd")}/");
                    }

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var file_name = "";

                    file_name = (path + CreateRandomFilename(Path.GetExtension(file.FileName).ToLower()));

                    var file_name2 = "";

                    file.SaveAs(file_name);
                    if (img_resize)
                    {
                        if (!file.ContentType.ToLower().StartsWith("image/"))
                        {
                            throw new Exception("File must be an image file");
                        }

                        using (System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(file_name))
                        {
                            if (objGraphic.Width > 800 || objGraphic.Height > 800)
                            {
                                int new_w = 1;
                                int new_h = 1;

                                if (objGraphic.Width > objGraphic.Height)
                                {
                                    new_w = 800;
                                    new_h = (int)Math.Ceiling((1.0 * objGraphic.Height * new_w) / (objGraphic.Width == 0 ? 1 : objGraphic.Width));
                                }
                                else
                                {
                                    new_h = 800;
                                    new_w = (int)Math.Ceiling((1.0 * objGraphic.Width * new_h) / (objGraphic.Height == 0 ? 1 : objGraphic.Height));
                                }

                                file_name2 = path = (path + CreateRandomFilename(Path.GetExtension(file.FileName).ToLower()));

                                using (var objBitmap = new System.Drawing.Bitmap(objGraphic, new_w, new_h))
                                {
                                    objBitmap.Save(file_name2, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(file_name2))
                        {
                            File.Delete(file_name);
                            file_name = file_name2;
                        }
                    }

                    file_info = new FileInfo(file_name);
                    //MimeMapping.GetMimeMapping
                }
                catch (Exception ex)
                {
                    error_message = ex.Message;
                    return false;
                }

                return true;
            }

            public bool FileUpload(HttpPostedFile file, string path, string accept, bool create_sub_directory, ref string error_message, ref FileInfo file_info, bool img_resize = false)
            {
                file_info = null;
                if (file == null || file.ContentLength == 0)
                {
                    error_message = "No file upload";
                    return false;
                }
                if (!accept.Split(',').Select(x => (x ?? "").Trim().ToLower()).Where(x => !string.IsNullOrEmpty(x)).ToArray().Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    error_message = "File not support";
                    return false;
                }

                try
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    DateTime d = DateTime.Now;
                    //List<string> video_ext = new List<string>();
                    //video_ext.Add(".mp4");
                    //video_ext.Add(".flv");
                    //video_ext.Add(".3gp");
                    //video_ext.Add(".wma");

                    if (create_sub_directory)
                    {
                        path = (path + $"/{d.ToString("yyyyMMdd")}/");
                    }

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var file_name = "";

                    file_name = (path + CreateRandomFilename(Path.GetExtension(file.FileName).ToLower()));
                    //throw new Exception(file_name);

                    var file_name2 = "";

                    file.SaveAs(file_name);
                    if (img_resize)
                    {
                        if (!file.ContentType.ToLower().StartsWith("image/"))
                        {
                            throw new Exception("File must be an image file");
                        }

                        using (System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(file_name))
                        {
                            if (objGraphic.Width > 800 || objGraphic.Height > 800)
                            {
                                int new_w = 1;
                                int new_h = 1;

                                if (objGraphic.Width > objGraphic.Height)
                                {
                                    new_w = 800;
                                    new_h = (int)Math.Ceiling((1.0 * objGraphic.Height * new_w) / (objGraphic.Width == 0 ? 1 : objGraphic.Width));
                                }
                                else
                                {
                                    new_h = 800;
                                    new_w = (int)Math.Ceiling((1.0 * objGraphic.Width * new_h) / (objGraphic.Height == 0 ? 1 : objGraphic.Height));
                                }

                                file_name2 = path = (path + CreateRandomFilename(Path.GetExtension(file.FileName).ToLower()));

                                using (var objBitmap = new System.Drawing.Bitmap(objGraphic, new_w, new_h))
                                {
                                    objBitmap.Save(file_name2, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(file_name2))
                        {
                            File.Delete(file_name);
                            file_name = file_name2;
                        }
                    }

                    file_info = new FileInfo(file_name);
                    //MimeMapping.GetMimeMapping
                }
                catch (Exception ex)
                {
                    error_message = ex.Message;
                    return false;
                }

                return true;
            }

            public string CreateRandomFilename(string ext)
            {
                return DateTime.Now.ToString("yyyyMMddHHmmss") + CreateMD5(Path.GetRandomFileName()).Substring(0, 8) + ext;
            }

            public string CreateMD5(string input)
            {
                // Use input string to calculate MD5 hash
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // Convert the byte array to hexadecimal string
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }

            public void YouMustUpdateDataToolsToV1_0_8()
            {

            }
        }

        //public static class DocxExtended {
        //    public static DocX ReplaceText(this DocX docx, object el) {
        //        return
        //    }
        //}

        public static class JsonExtened
        {
            public static string ToJsonString(this object obj)
            {
                try
                {
                    return Dtl.json_stringify(obj);
                }
                catch
                {
                    return null;
                }
            }

            public static T FromJson<T>(this T obj, string json) where T : class
            {
                return Dtl.json_to_object(json, obj);
            }

            public static string Json(this HttpRequestBase Request)
            {
                return Dtl.json_request();
            }

            public static dynamic ToDynamic<T>(this T obj)
            {
                IDictionary<string, object> expando = new ExpandoObject();

                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    var currentValue = propertyInfo.GetValue(obj);
                    expando.Add(propertyInfo.Name, currentValue);
                }
                return expando as ExpandoObject;
            }
        }

        public static class DateTimeExtended
        {
            public static int Quarter(this DateTime dt)
            {
                var month = dt.Month;
                return new int?[] { 1, 2, 3 }.Contains(month) ? 1 : new int?[] { 4, 5, 6 }.Contains(month) ? 2 : new int?[] { 7, 8, 9 }.Contains(month) ? 3 : new int?[] { 10, 11, 12 }.Contains(month) ? 4 : 0;
            }

            public static int WeekOfYear(this DateTime time, DayOfWeek day)
            {
                int w = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, day);
                if (time.Month == 1 && w >= 52)
                {
                    w = 0;
                }
                return w;
            }

            public static int WeekOfYearStartMonday(this DateTime time)
            {
                return time.WeekOfYear(DayOfWeek.Monday);
            }

            public static int WeekOfYearStartSunday(this DateTime time)
            {
                return time.WeekOfYear(DayOfWeek.Sunday);
            }

            public static int Diff(this DateTime time, DateTime comparer, TimeUnit timeUnit)
            {
                if (timeUnit == TimeUnit.Day)
                {
                    return (int)(time - comparer).TotalDays;
                }
                else if (timeUnit == TimeUnit.Hour)
                {
                    return (int)(time - comparer).TotalHours;
                }
                else if (timeUnit == TimeUnit.Minute)
                {
                    return (int)(time - comparer).TotalMinutes;
                }
                else if (timeUnit == TimeUnit.Second)
                {
                    return (int)(time - comparer).TotalSeconds;
                }
                else if (timeUnit == TimeUnit.Millisecond)
                {
                    return (int)(time - comparer).TotalMilliseconds;
                }
                else
                {
                    return 0;
                }
            }
        }

        public enum TimeUnit
        {
            Day,
            Hour,
            Minute,
            Second,
            Millisecond
        }

    public class FileUpload
    {
        private string virtual_dir = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UploadPath"]) ? @"C:\fileuploads\" : System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

        //private string virtual_dir = HttpContext.Current.Server.MapPath("~/FileUploads/");
        private bool img_resize = false;

        private string[] except_extentions = new string[] { "jpg", "jpeg", "png", "gif", "bmp", "doc", "docx", "xls", "xlsx", "pdf", "csv" ,"pptx", "ppt" ,"mp4", "mpeg", "3gp" };
        private HttpPostedFileBase file;
        private string  path_name = "";

        private int max_w = 0;
        private int max_h = 0;
        private bool auto_resize;
        public FileUpload(HttpPostedFileBase file)
        {
            var extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();
          
          
            this.file = file;
            if (!Directory.Exists(virtual_dir))
            {
                Directory.CreateDirectory(virtual_dir);
            }
        }

        public FileUpload(HttpPostedFileBase file, bool img_resize,bool auto_resize, int max_w, int max_h)
        {
            this.file = file;
            if (img_resize)
            {
                this.img_resize = img_resize;
                this.max_w = max_w;
                this.max_h = max_h;
            }
            if (auto_resize)
            {
                this.auto_resize = auto_resize;
            }
                if (!Directory.Exists(virtual_dir))
            {
                Directory.CreateDirectory(virtual_dir);
            }
        }

        public string FileSave()
        {
            var extention = "";
            var new_file_name = "";

            if (file == null || file.ContentLength <= 0)
            {
                throw new Exception("No file uploaded.");
            }

            extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();

            if (!except_extentions.Contains(extention))
            {
                throw new Exception("File not accept.");
            }
            if (img_resize || auto_resize)
            {
                if (!file.ContentType.ToLower().StartsWith("image/"))
                {
                    throw new Exception("File must be an image file");
                }
            }

            var tmpfile = "";
            do
            {
                tmpfile = CreateFileName() + "." + extention;
            }
            while (File.Exists(virtual_dir + tmpfile));
            file.SaveAs(virtual_dir + tmpfile);
            if (img_resize)
            {
              
                using (System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(virtual_dir + tmpfile))
                {
                    if (objGraphic.Width > max_w || objGraphic.Height > max_h)
                    {
                        int new_w = 1;
                        int new_h = 1;

                        if (objGraphic.Width > objGraphic.Height)
                        {
                            new_w = max_w;
                            new_h = (int)Math.Ceiling((1.0 * objGraphic.Height * new_w) / (objGraphic.Width == 0 ? 1 : objGraphic.Width));
                        }
                        else
                        {
                            new_h = max_h;
                            new_w = (int)Math.Ceiling((1.0 * objGraphic.Width * new_h) / (objGraphic.Height == 0 ? 1 : objGraphic.Height));
                        }

                        using (var objBitmap = new System.Drawing.Bitmap(objGraphic, new_w, new_h))
                        {
                            do
                            {
                                new_file_name = CreateFileName() + ".jpg";
                            }
                            while (File.Exists((virtual_dir) + new_file_name));

                            objBitmap.Save((virtual_dir) + new_file_name, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                    else
                    {
                        new_file_name = tmpfile;
                    }
                }

                if (tmpfile != new_file_name)
                {
                    File.Delete((virtual_dir) + tmpfile);
                }
            }
            //else
            //{
            //    do
            //    {
            //        new_file_name = CreateFileName() + "." + extention;
            //    }
            //    while (File.Exists((virtual_dir) + new_file_name));

            //    file.SaveAs((virtual_dir) + new_file_name);
            //}
            else if (auto_resize)
            {
                
                  

                    using (System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(virtual_dir + tmpfile))
                    {
                        if (objGraphic.Width > max_w || objGraphic.Height > max_h)
                        {
                            int new_w = objGraphic.Width/2;
                            int new_h = objGraphic.Height/2;

                            if (objGraphic.Width > objGraphic.Height)
                            {
                                new_w = max_w;
                                new_h = (int)Math.Ceiling((1.0 * objGraphic.Height * new_w) / (objGraphic.Width == 0 ? 1 : objGraphic.Width));
                            }
                            else
                            {
                                new_h = max_h;
                                new_w = (int)Math.Ceiling((1.0 * objGraphic.Width * new_h) / (objGraphic.Height == 0 ? 1 : objGraphic.Height));
                            }

                            using (var objBitmap = new System.Drawing.Bitmap(objGraphic, new_w, new_h))
                            {
                                do
                                {
                                    new_file_name = CreateFileName() + ".jpg";
                                }
                                while (File.Exists((virtual_dir) + new_file_name));

                                objBitmap.Save((virtual_dir) + new_file_name, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        else
                        {
                            new_file_name = tmpfile;
                        }
                    }
                if (tmpfile != new_file_name)
                {
                    File.Delete((virtual_dir) + tmpfile);
                }


            }
            else
            {
                do
                {
                    new_file_name = CreateFileName() + "." + extention;
                }
                while (File.Exists((virtual_dir) + new_file_name));

                file.SaveAs((virtual_dir) + new_file_name);
            }
            return new_file_name;
        }

        private string CreateFileName()
        {
            var new_file_name = DateTime.Now.ToString("yyyyMMddHHmmss");
            new_file_name += "_";

            var rnd = new Random();
            for (int i = 1; i <= 6; i++)
            {
                new_file_name += rnd.Next(0, 10);
            }
            return new_file_name;
        }
    }
    public class UploadFile
    {
        private string virtual_dir = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UploadPath"]) ? @"C:\fileuploads\" : System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

        //private string virtual_dir = HttpContext.Current.Server.MapPath("~/FileUploads/");
        private bool img_resize = false;

        private string[] except_extentions = new string[] {"mp3","wma", "jpg", "jpeg", "png", "gif", "bmp", "doc", "docx", "xls", "xlsx", "pdf", "csv" ,"pptx", "ppt" ,"mp4", "mpeg", "3gp" };
        private HttpPostedFile file;
        private string truePath;
        private int max_w = 0;
        private int max_h = 0;
        private bool auto_resize;
        private bool convertfile;
        private string path_name ="";

        public UploadFile(HttpPostedFile file)
        {
            var extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();
           

            this.file = file;
            if (!Directory.Exists(virtual_dir))
            {
                Directory.CreateDirectory(virtual_dir);
            }
        }

        public UploadFile(HttpPostedFile file, bool img_resize,bool auto_resize,bool convertfile, int max_w, int max_h)
        {
            this.file = file;
            if (img_resize)
            {
                this.img_resize = img_resize;
                this.max_w = max_w;
                this.max_h = max_h;
            }
            if (auto_resize)
            {
                this.auto_resize = auto_resize;
            }
                if (!Directory.Exists(virtual_dir))
            {
                Directory.CreateDirectory(virtual_dir);
            }
            if (convertfile)
            {
                this.convertfile = convertfile;
            }
        }

        public string FileSave()
        {
            var new_file_name = "";

            try
            {
                var extention = "";

                if (file == null || file.ContentLength <= 0)
                {
                    throw new Exception("No file uploaded.");
                }

                extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();

                if (!except_extentions.Contains(extention))
                {
                    throw new Exception("File not accept.");
                }
                if (img_resize || auto_resize)
                {
                    if (!file.ContentType.ToLower().StartsWith("image/"))
                    {
                        throw new Exception("File must be an image file");
                    }
                }

                var tmpfile = "";
                do
                {
                    tmpfile = CreateFileName() + "." + extention;
                }
                while (File.Exists(virtual_dir + tmpfile));
                file.SaveAs(virtual_dir + tmpfile);
                path_name = tmpfile;
                if (img_resize)
                {

                    using (System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(virtual_dir + tmpfile))
                    {
                        if (objGraphic.Width > max_w || objGraphic.Height > max_h)
                        {
                            int new_w = 1;
                            int new_h = 1;

                            if (objGraphic.Width > objGraphic.Height)
                            {
                                new_w = max_w;
                                new_h = (int)Math.Ceiling((1.0 * objGraphic.Height * new_w) / (objGraphic.Width == 0 ? 1 : objGraphic.Width));
                            }
                            else
                            {
                                new_h = max_h;
                                new_w = (int)Math.Ceiling((1.0 * objGraphic.Width * new_h) / (objGraphic.Height == 0 ? 1 : objGraphic.Height));
                            }

                            using (var objBitmap = new System.Drawing.Bitmap(objGraphic, new_w, new_h))
                            {
                                do
                                {
                                    new_file_name = CreateFileName() + ".jpg";
                                }
                                while (File.Exists((virtual_dir) + new_file_name));

                                objBitmap.Save((virtual_dir) + new_file_name, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        else
                        {
                            new_file_name = tmpfile;
                        }
                    }

                    if (tmpfile != new_file_name)
                    {
                        File.Delete((virtual_dir) + tmpfile);
                    }
                }
                //else
                //{
                //    do
                //    {
                //        new_file_name = CreateFileName() + "." + extention;
                //    }
                //    while (File.Exists((virtual_dir) + new_file_name));

                //    file.SaveAs((virtual_dir) + new_file_name);
                //}
                else if (auto_resize)
                {



                    using (System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(virtual_dir + tmpfile))
                    {
                        if (objGraphic.Width > max_w || objGraphic.Height > max_h)
                        {
                            int new_w = objGraphic.Width / 2;
                            int new_h = objGraphic.Height / 2;

                            if (objGraphic.Width > objGraphic.Height)
                            {
                                new_w = max_w;
                                new_h = (int)Math.Ceiling((1.0 * objGraphic.Height * new_w) / (objGraphic.Width == 0 ? 1 : objGraphic.Width));
                            }
                            else
                            {
                                new_h = max_h;
                                new_w = (int)Math.Ceiling((1.0 * objGraphic.Width * new_h) / (objGraphic.Height == 0 ? 1 : objGraphic.Height));
                            }

                            using (var objBitmap = new System.Drawing.Bitmap(objGraphic, new_w, new_h))
                            {
                                do
                                {
                                    new_file_name = CreateFileName() + ".jpg";
                                }
                                while (File.Exists((virtual_dir) + new_file_name));

                                objBitmap.Save((virtual_dir) + new_file_name, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        else
                        {
                            new_file_name = tmpfile;
                        }
                    }
                    if (tmpfile != new_file_name)
                    {
                        File.Delete((virtual_dir) + tmpfile);
                    }


                }


                else
                {
                    File.Delete(virtual_dir + tmpfile);

                    do
                    {
                        new_file_name = CreateFileName() + "." + extention;
                    }
                    while (File.Exists((virtual_dir) + new_file_name));
                    truePath = new_file_name;



                    file.SaveAs((virtual_dir) + new_file_name);
                }
                Security.Token token_ = new Security.Token();
            }catch(Exception ex)
            {
                ErrorList(ex);
            }
            return /*token_.CreateToken(DateTime.Now.ToString("yyyyMMdd") + "\\"+*/ new_file_name/*)*/;
        }
        private string new_folder;
        private string file_;
        private string tmp_;
        private string p_480;
        private string original;
        private string p_720;
        private string p_1080;
        private string thumbnail;
        private string videoUrl = System.Configuration.ConfigurationManager.AppSettings["VideoUrl"]??"";
        private string coverUrl = System.Configuration.ConfigurationManager.AppSettings["CoverUrl"]??"";

        public dynamic ConverterFile(ref string errorMsg)
        {
            dynamic output = null;
            try
            {
                string[] media = new string[] { "mp4", "mpeg", "3gp" };
                var extention = "";
                var new_file_name = "";
                extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();

                var tmp = CreateFileName();
                new_folder = virtual_dir + tmp + "\\";
                Directory.CreateDirectory(new_folder);
                tmp = tmp + "." + extention;


                 original = tmp.Substring(0, tmp.Length - 4) + "_original." + extention;
                file_ = original;
                tmp_ = tmp;
                file.SaveAs(new_folder + tmp);

                file.SaveAs(new_folder + original);
                thumbnail =/* token.CreateToken(*/file_.Substring(0, file_.IndexOf("_")) + "_cover.jpg"/*)*/;
                using (var db = new Data.DataContext())
                {
                    var f4 = new Data.DataModels.video_status
                    {
                        filename = original,
                        status = "Y"
                    };

                    db.video_status.Add(f4);
                    db.SaveChanges();
                }
                Task.Run(async () =>
                {
                    await GetThumnail(new_folder + file_, file_);
                  await  ConvertStream();
                });

                //task.Add(
                /*);*/
                //Task t=       Task.WhenAll(task);
                //       try
                //       {
                //           t.Wait();
                //       } catch { };

                Security.Token token = new Security.Token();

                 p_480 = /*token.CreateToken(*/file_.Substring(0, file_.IndexOf("_")) + "_480." + extention/*)*/;
                 p_720 = /*token.CreateToken(*/ file_.Substring(0, file_.IndexOf("_")) + "_720." + extention/*)*/;
                p_1080 = /*token.CreateToken(*/ file_.Substring(0, file_.IndexOf("_")) + "_1080." + extention/*)*/;
                                 
             
                output = new
                {
                    original= videoUrl+original,
                    p_480= videoUrl+p_480,
                    p_720=videoUrl+p_720,
                    p_1080=videoUrl+p_1080,
                    thumbnail=coverUrl+ thumbnail,
                };

              
            }
            catch(Exception ex)
            {
                errorMsg = ex.Message;
                ErrorList(ex);
            }


           
            return output;
        }
        public async Task ConvertStream()
        {
            try
            {
                List<Task> t = new List<Task>();

                //        await Task.WhenAll(
                //         GetThumnail(new_folder + file_, file_),
                //         Converter1080(new_folder + file_, file_),

                //         Converter480(new_folder + file_, file_),
                //         Converter720(new_folder + file_, file_)



                //);
                /* t.Add(*/
                await Task.WhenAll(Task.Run(async () =>
                {
                    await Task.Run(() =>
                       {
                           using (var db = new Data.DataContext())
                           {
                               List<Data.DataModels.video_status> v = new List<Data.DataModels.video_status>();
                               var f1 = new Data.DataModels.video_status
                               {
                                   filename = p_480,
                                   status = "N"
                               }; var f2 = new Data.DataModels.video_status
                               {
                                   filename = p_720,
                                   status = "N"
                               }; var f3 = new Data.DataModels.video_status
                               {
                                   filename = p_1080,
                                   status = "N"
                               };

                               v.Add(f1);
                               v.Add(f2);
                               v.Add(f3);
                               db.video_status.AddRange(v);
                               db.SaveChanges();
                           }
                       });
                    await Task.Run(() =>
                     {
                         using (var db = new Data.DataContext())
                         {
                             if (Converter480(new_folder + file_, file_).IsCompleted)
                             {
                                 db.video_status.Remove(db.video_status.Where(x => x.filename == p_480).FirstOrDefault());

                                 var m = new Data.DataModels.video_status
                                 {
                                     filename = p_480,
                                     status = "Y"
                                 };
                                 db.video_status.Add(m);
                                 db.SaveChanges();
                             }

                         }
                     });
                    await Task.Run(() =>
                    {
                        using (var db = new Data.DataContext())
                        {

                            if (Converter720(new_folder + file_, file_).IsCompleted)
                            {
                                db.video_status.Remove(db.video_status.Where(x => x.filename == p_720).FirstOrDefault());

                                var m = new Data.DataModels.video_status
                                {
                                    filename = p_720,
                                    status = "Y"
                                };
                                db.video_status.Add(m);
                                db.SaveChanges();
                            }

                        }
                    });

                    await Task.Run(() =>
                       {
                           using (var db = new Data.DataContext())
                           {

                               if (Converter1080(new_folder + file_, file_).IsCompleted)
                               {
                                   db.video_status.Remove(db.video_status.Where(x => x.filename == p_1080).FirstOrDefault());

                                   var m = new Data.DataModels.video_status
                                   {
                                       filename = p_1080,
                                       status = "Y"
                                   };
                                   db.video_status.Add(m);
                                   db.SaveChanges();
                               }

                           }

                       });
                }));

                //);
                File.Delete((new_folder) + tmp_);
            }catch(Exception ex)
            {
                ErrorList(ex);
            }
        }

        public Task Converter480(string path,string tmp)
        {
            try
            {
                string[] media = new string[] { "mp4", "mpeg", "3gp" };
                var extention = "";
                var new_file_name = "";
                extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();


                var file_new = tmp.Substring(0, tmp.IndexOf("_")) + "_480." + extention;
                var inputFile = new MediaFile { Filename = path };
                var outputFile = new MediaFile { Filename = new_folder + file_new };
                var option = new ConversionOptions
                {
                    VideoAspectRatio = VideoAspectRatio.Default,
                    VideoSize = VideoSize.Hd480,
                    AudioSampleRate = AudioSampleRate.Hz48000
                };

                //Task.Run(async () =>
                //{

                var engine = new MediaToolkit.Engine();
                /*await*/
                Task.Delay(5000);

                engine.Convert(inputFile, outputFile, option);
                //});

            }catch(Exception ex)
            {
                ErrorList(ex);
            }
            return Task.CompletedTask;/* DateTime.Now.ToString("yyyyMMdd") + "\\" +tmp.Substring(0, tmp.IndexOf("_")) +"\\"+file_new;*/
        }
          public Task Converter720(string path,string tmp)
        {
            try
            {
                string[] media = new string[] { "mp4", "mpeg", "3gp" };
                var extention = "";
                var new_file_name = "";
                extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();


                var file_new = tmp.Substring(0, tmp.IndexOf("_")) + "_720." + extention;
                var inputFile = new MediaFile { Filename = path };
                var outputFile = new MediaFile { Filename = new_folder + file_new };
                var option = new ConversionOptions
                {
                    VideoAspectRatio = VideoAspectRatio.Default,
                    VideoSize = VideoSize.Hd720,
                    AudioSampleRate = AudioSampleRate.Hz48000
                };

                var engine = new MediaToolkit.Engine();
                //await Task.Delay(5000);

                engine.Convert(inputFile, outputFile, option);
            } catch (Exception ex)
            {
                ErrorList(ex);
            }
            return Task.CompletedTask;
            //return /*DateTime.Now.ToString("yyyyMMdd") + "\\"+tmp.Substring(0, tmp.IndexOf("_")) +"\\"+ file_new;*/
        }
             public Task Converter1080(string path,string tmp)
        {
            try
            {
                string[] media = new string[] { "mp4", "mpeg", "3gp" };
                var extention = "";
                var new_file_name = "";
                extention = Path.GetExtension(file.FileName).Replace(".", "").Trim().ToLower();

                var file_new = tmp.Substring(0, tmp.IndexOf("_")) + "_1080." + extention;
                var inputFile = new MediaFile { Filename = path };
                var outputFile = new MediaFile { Filename = new_folder + file_new };
                var option = new ConversionOptions
                {
                    VideoAspectRatio = VideoAspectRatio.Default,
                    VideoSize = VideoSize.Hd1080,
                    AudioSampleRate = AudioSampleRate.Hz48000
                };
                //Task.Run(async () =>
                //{
                var engine = new MediaToolkit.Engine();
                //await Task.Delay(5000);
                engine.Convert(inputFile, outputFile, option);
                //});
            }catch(Exception ex)
            {
                ErrorList(ex);
            }
            return Task.CompletedTask;

            //return DateTime.Now.ToString("yyyyMMdd") + "\\" +tmp.Substring(0, tmp.IndexOf("_")) +"\\"+ file_new;
        }
        public Task GetThumnail(string path,string thumpnail)
        {
            try
            {
                var thumnail_path = new_folder + thumbnail;
                //Task.Run(async () =>
                //{
                //await Task.Delay(5000);
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                ffMpeg.GetVideoThumbnail(path, thumnail_path, 70.00f);
                //});
            }catch(Exception ex)
            {
                ErrorList(ex);
            }
            return Task.CompletedTask;

            //return DateTime.Now.ToString("yyyyMMdd") + "\\"+thumpnail.Substring(0, thumpnail.IndexOf("_")) + "\\" + "thumbnail.jpg";
        }

        private string CreateFileName()
        {
            var new_file_name = DateTime.Now.ToString("yyyyMMddHHmmss");
          

            var rnd = new Random();
            for (int i = 1; i <= 6; i++)
            {
                new_file_name += rnd.Next(0, 10);
            }
            return new_file_name;
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