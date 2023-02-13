using DpimProject.Models.Data;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
namespace DpimProject.Models.Export
{
    public class Export
    {
        private readonly DataContext db;
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public Export()
        {
            db = new DataContext();
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic export_report(int page, bool isExcel, string course_id, string course_category_id, int account_type, string search, ref string error_message)
        {
            dynamic output = null;
            dynamic data = null;
            try
            {
                if (page == 1)
                {
                    
                }
                else if(page == 2)
                {
                    //var get_eva_result = db.course_evaluation_result.Where(w => w.is_deleted == 0 && w.course_id == course_id);
                    //var get_choices = db.course_evaluation_choices;
                    //data = (from c_eva in db.course_evaluation
                    //        where c_eva.course_id == course_id
                    //        select new
                    //        {
                    //            name = c_eva.question,
                    //            total_score = get_choices.Where(w => w.course_evaluation_id == c_eva.id).Select(s => s.score).Sum(),
                    //            score = (from r in get_eva_result
                    //                     where r.course_evaluation_id == c_eva.id
                    //                     select new
                    //                     {
                    //                         score2 = get_choices.Where(w => w.id == r.course_evaluation_choices_id).Select(s => s.score).FirstOrDefault()
                    //                     }).ToList()
                    //        }).ToList();

                    //if (isExcel)
                    //{
                    //    ExcelPackage excel = new ExcelPackage();
                    //    var workSheet = excel.Workbook.Worksheets.Add("รายงานการขาย");

                    //    workSheet.Cells[1, 1].Value = "ลำดับ";
                    //    workSheet.Cells[1, 1].Style.Font.Bold = true;
                    //    workSheet.Cells[1, 2].Value = "แบบสอบถาม";
                    //    workSheet.Cells[1, 2].Style.Font.Bold = true;
                    //    workSheet.Cells[1, 3].Value = "คะแนนเฉลี่ย";
                    //    workSheet.Cells[1, 3].Style.Font.Bold = true;





                    //    var name_file = "Student Overview.xlsx";
                    //    var stream = new MemoryStream();
                    //    excel.SaveAs(stream);
                    //    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    //    {
                    //        Content = new ByteArrayContent(stream.ToArray())
                    //    };
                    //    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    //    {
                    //        FileName = name_file
                    //    };
                    //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                    //    output = result;
                    //}
                    //else
                    //{

                    //    output = data;
                    //}
                }
                else if(page == 3)
                {

                }
                // การพิมพ์ใบประกาศ by category course
                else if(page == 4)
                {
                    var student_info = db.student_course_info.Where(w => w.is_deleted == 0);
                    data = (from c in db.course
                            where c.course_category_id == course_category_id && c.is_deleted == 0
                            select new
                            {
                                name = c.name,
                                cert_print_count = student_info.Where(w => w.course_id == c.id).Select(s => s.cert_print_count).Sum()
                            }).ToList();

                    var category_name = db.course_category.Where(w => w.id == course_category_id).Select(s => s.name).FirstOrDefault();
                    category_name = category_name == null || category_name == "" ? "category" : category_name;
                    if (isExcel)
                    {
                        ExcelPackage excel = new ExcelPackage();
                        var workSheet = excel.Workbook.Worksheets.Add(category_name);

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[1, 1].Value = "ลำดับ";
                        workSheet.Cells[1, 2].Value = "ชื่อคอร์ส";
                        workSheet.Cells[1, 3].Value = "จำนวนการพิมพ์";

                        int recordIndex = 2;
                        foreach (var val in data)
                        {
                            workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1);
                            workSheet.Cells[recordIndex, 2].Value = val.name;
                            workSheet.Cells[recordIndex, 3].Value = val.cert_print_count == null ? 0 : val.cert_print_count;

                            recordIndex++;
                        }

                        for (int i = 1; i <= 3; i++)
                        {
                            workSheet.Column(i).AutoFit();
                        }

                        var name_file = "Certificate Category " + category_name + ".xlsx";
                        var stream = new MemoryStream();
                        excel.SaveAs(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = name_file
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        output = result;
                    }
                    else
                    {

                        output = data;
                    }
                }
                // การพิมพ์ใบประกาศ by course
                else if (page == 5)
                {
                    data = (from s_info in db.student_course_info
                            join s in db.student.Where(w => w.is_deleted == 0) on s_info.student_id equals s.id
                            where s_info.is_deleted == 0 && s_info.course_id == course_id
                            select new
                            {
                                firstname = s.firstname,
                                lastname = s.lastname,
                                cert_print_count = s_info.cert_print_count
                            }).ToList();

                    var course_name = db.course.Where(w => w.id == course_id).Select(s => s.name).FirstOrDefault();
                    course_name = course_name == null || course_name == "" ? "course" : course_name;
                    if (isExcel)
                    {
                        ExcelPackage excel = new ExcelPackage();
                        var workSheet = excel.Workbook.Worksheets.Add(course_name);

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[1, 1].Value = "ลำดับ";
                        workSheet.Cells[1, 2].Value = "ชื่อ";
                        workSheet.Cells[1, 3].Value = "จำนวนการพิมพ์";

                        int recordIndex = 2;
                        foreach (var val in data)
                        {
                            workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1);
                            workSheet.Cells[recordIndex, 2].Value = val.firstname + ' ' + val.lastname;
                            workSheet.Cells[recordIndex, 3].Value = val.cert_print_count == null ? 0 : val.cert_print_count;

                            recordIndex++;
                        }

                        for (int i = 1; i <= 3; i++)
                        {
                            workSheet.Column(i).AutoFit();
                        }

                        var name_file = "Certificate Category " + course_name + ".xlsx";
                        var stream = new MemoryStream();
                        excel.SaveAs(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = name_file
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        output = result;
                    }
                    else
                    {

                        output = data;
                    }
                }
                // video on demand
                else if (page == 6)
                {
                    data = db.video_on_demand.Where(w => w.is_deleted == 0 && w.course_category_id == course_category_id)
                                             .Select(s => new { name = s.name, count_view = s.count_view })
                                             .ToList();

                    var category_name = db.course_category.Where(w => w.id == course_category_id).Select(s => s.name).FirstOrDefault();
                    category_name = category_name == null || category_name == "" ? "category" : category_name;

                    if (isExcel)
                    {
                        ExcelPackage excel = new ExcelPackage();
                        var workSheet = excel.Workbook.Worksheets.Add(category_name);

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[1, 1].Value = "ลำดับ";
                        workSheet.Cells[1, 2].Value = "ชื่อ Video on demand";
                        workSheet.Cells[1, 3].Value = "จำนวนการเข้า";

                        int recordIndex = 2;
                        foreach (var val in data)
                        {
                            workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1);
                            workSheet.Cells[recordIndex, 2].Value = val.name;
                            workSheet.Cells[recordIndex, 3].Value = val.count_view == null ? 0 : val.count_view;

                            recordIndex++;
                        }

                        for (int i = 1; i <= 3; i++)
                        {
                            workSheet.Column(i).AutoFit();
                        }

                        var name_file = "Video on demand " + category_name + ".xlsx";
                        var stream = new MemoryStream();
                        excel.SaveAs(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = name_file
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        output = result;
                    }
                    else
                    {

                        output = data;
                    }
                }
                // การรับชมบทเรียน by category course
                else if (page == 7)
                {
                    var get_lesson = db.course_lesson.Where(w => w.is_deleted == 0);

                    data = (from c in db.course
                            where c.is_deleted == 0 && c.course_category_id == course_category_id
                            select new
                            {
                                name = c.name,
                                count_view = get_lesson.Where(w => w.course_id == c.id).Select(s => s.count_view).Sum()
                            }).ToList();

                    var category_name = db.course_category.Where(w => w.id == course_category_id).Select(s => s.name).FirstOrDefault();
                    category_name = category_name == null || category_name == "" ? "category" : category_name;

                    if (isExcel)
                    {
                        ExcelPackage excel = new ExcelPackage();
                        var workSheet = excel.Workbook.Worksheets.Add(category_name);

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[1, 1].Value = "ลำดับ";
                        workSheet.Cells[1, 2].Value = "ชื่อคอร์ส";
                        workSheet.Cells[1, 3].Value = "จำนวนการรับชมคอร์ส";

                        int recordIndex = 2;
                        foreach (var val in data)
                        {
                            workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1);
                            workSheet.Cells[recordIndex, 2].Value = val.name;
                            workSheet.Cells[recordIndex, 3].Value = val.count_view == null ? 0 : val.count_view;

                            recordIndex++;
                        }

                        for (int i = 1; i <= 3; i++)
                        {
                            workSheet.Column(i).AutoFit();
                        }

                        var name_file = "Watch Course " + category_name + ".xlsx";
                        var stream = new MemoryStream();
                        excel.SaveAs(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = name_file
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        output = result;
                    }
                    else
                    {

                        output = data;
                    }
                }
                // การรับชมบทเรียน by course
                else if (page == 8)
                {
                    data = db.course_lesson.Where(w => w.is_deleted == 0 && w.course_id == course_id)
                                           .Select(s => new { name = s.name, count_view = s.count_view })
                                           .ToList();

                    var course_name = db.course.Where(w => w.id == course_id).Select(s => s.name).FirstOrDefault();
                    course_name = course_name == null || course_name == "" ? "course" : course_name;

                    if (isExcel)
                    {
                        ExcelPackage excel = new ExcelPackage();
                        var workSheet = excel.Workbook.Worksheets.Add(course_name);

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[1, 1].Value = "ลำดับ";
                        workSheet.Cells[1, 2].Value = "ชื่อบทเรียน";
                        workSheet.Cells[1, 3].Value = "จำนวนการรับชม";

                        int recordIndex = 2;
                        foreach (var val in data)
                        {
                            workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1);
                            workSheet.Cells[recordIndex, 2].Value = val.name;
                            workSheet.Cells[recordIndex, 3].Value = val.count_view == null ? 0 : val.count_view;

                            recordIndex++;
                        }

                        for (int i = 1; i <= 3; i++)
                        {
                            workSheet.Column(i).AutoFit();
                        }

                        var name_file = "Watch Course " + course_name + ".xlsx";
                        var stream = new MemoryStream();
                        excel.SaveAs(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = name_file
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        output = result;
                    }
                    else
                    {

                        output = data;
                    }
                }
                // การรับชมบทเรียน
                else if (page == 9)
                {
                    data = db.course.Where(w => w.is_deleted == 0 && w.course_category_id == course_category_id)
                                    .Select(s => new { name = s.name, print_count = s.print_count })
                                    .ToList();

                    var category_name = db.course_category.Where(w => w.id == course_category_id).Select(s => s.name).FirstOrDefault();
                    category_name = category_name == null || category_name == "" ? "category" : category_name;

                    if (isExcel)
                    {
                        ExcelPackage excel = new ExcelPackage();
                        var workSheet = excel.Workbook.Worksheets.Add(category_name);

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[1, 1].Value = "ลำดับ";
                        workSheet.Cells[1, 2].Value = "ชื่อคอร์ส";
                        workSheet.Cells[1, 3].Value = "จำนวนที่พิมพ์เอกสาร";

                        int recordIndex = 2;
                        foreach (var val in data)
                        {
                            workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1);
                            workSheet.Cells[recordIndex, 2].Value = val.name;
                            workSheet.Cells[recordIndex, 3].Value = val.print_count == null ? 0 : val.print_count;

                            recordIndex++;
                        }

                        for (int i = 1; i <= 3; i++)
                        {
                            workSheet.Column(i).AutoFit();
                        }

                        var name_file = "Print Course " + category_name + ".xlsx";
                        var stream = new MemoryStream();
                        excel.SaveAs(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = name_file
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        output = result;
                    }
                    else
                    {

                        output = data;
                    }
                }
                else if (page == 10)
                {

                }
                else if (page == 11)
                {

                }
                else if (page == 12)
                {

                }
            }
            catch (Exception ex)
            {
error_message = ex.Message;ErrorList(ex);
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