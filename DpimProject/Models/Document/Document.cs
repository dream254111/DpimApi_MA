using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Web.Mvc;
using System.Drawing;
using DpimProject.Models.DataTools;
using DpimProject.Models.Data;
using System.Diagnostics;
using System.Net.Http.Formatting;
using DpimProject.Models.Data.DataModels;
using System.Globalization;

namespace DpimProject.Models
{
    public class Document
    {
        public dynamic ExamImport(HttpPostedFile file,Authentication.Authentication auth,string course_id, ref string errorMsg)
        {
            var dtl = new Models.DataTools.DataTools();
            int total = 0;
            dynamic output = null;
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

            string tmp_f = "";
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    throw new Exception("No file upload.");
                }
                if (System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "") != "xlsx")
                // if (System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "") != "xlsx" || file.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    throw new Exception("File not supported (.xlsx only)");
                }
                tmp_f = new UploadFile(file).FileSave();

                int line = 0;
                //throw new Exception(tmp_f);

                List<Data.DataModels.course_exam> header = new List<Data.DataModels.course_exam>();
                List<Data.DataModels.course_exam_answer> detail = new List<Data.DataModels.course_exam_answer>();
            using (var db = new DataContext())
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(virtual_dir+ tmp_f);
                using (var excel = new ExcelPackage(fi))
                {
                    var ws = excel.Workbook.Worksheets.First();
                    int? id =( db.course_exam.Select(x => x.id).Max()??0) + 1;
                    int? course_exam_id = (db.course_exam_answer.Select(x => x.id).Max() ?? 0) + 1;
                    var start_row = 2;
                    string[] stringSeparators = new string[] { "," };
                var j = 0;
                    while ((ws.Cells[start_row, 1].Value?.ToString().Trim() ?? "") != "")
                    {

                        var exam_h = new Data.DataModels.course_exam();
                        var d= new List<Data.DataModels.course_exam_answer>();
                        exam_h.order = j+1;
                        exam_h.is_deleted = 0;
                        exam_h.score= 1;
                        exam_h.question = ws.Cells[start_row, 2].Value?.ToString().Trim();
                        exam_h.answer = Int32.Parse(ws.Cells[start_row, 3].Value?.ToString().Trim());
                        exam_h.created_dt = DateTime.Now;
                        exam_h.course_id = course_id;
                        exam_h.created_by = auth.user_id;
                        db.course_exam.Add(exam_h);
                        db.SaveChanges();
                        for (int i=0; i < 5; i++)
                    {
                        var exam_d = new Data.DataModels.course_exam_answer();

                        if (!string.IsNullOrEmpty(ws.Cells[start_row, 4+i].Value?.ToString().Trim())|| ws.Cells[start_row, 4 + i].Value?.ToString().Trim()!=null)
                        {
                            exam_d.answer = ws.Cells[start_row, 4+i].Value?.ToString().Trim();
                                exam_d.order = i + 1;
                                exam_d.course_id = course_id;
                            exam_d.course_exam_id = exam_h.id;
                            //detail.Add(exam_d);
                                db.course_exam_answer.Add(exam_d);
                                db.SaveChanges();
                            }
                    }


                    header.Add(exam_h);
                        start_row++;
                    }

             
                    //          throw new Exception(JsonConvert.SerializeObject(work));
                }
            //var pre_event2_l = m_array.Select(x => x.pre_event2).Distinct().ToList();

           


              
                //throw new Exception(p.Count.ToString() + " ");

                line = 0;
           var d1  =   header.ToList().Select(x => new
                {
                    x.id,
                    x.course_id,
                    x.order,
                    x.question,
                    x.image,
                    x.video,
                    x.is_deleted,
                    x.created_by,
                    x.created_dt,
                    x.update_by,
                    x.update_dt,
                    x.score,
                    x.answer,
                    detail = detail.Where(a => a.course_id == x.course_id && a.course_exam_id == x.id).ToList()

                }).ToList();
                output = new
                {
                   data=d1,
                };

            }

                //db.rd_mas_defect.AddRange(defect_array);
                //db.SaveChanges();
                //db.rd_mas_defect_group.AddRange(defect_group_array);
                //db.SaveChanges();
                //db.rd_mas_defect_type.AddRange(defect_type_array);
                //db.SaveChanges();

            }


            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrorList(ex);
            }
            finally
            {
                System.IO.File.Delete(tmp_f);

            }
            return output;

        }
        public void VoucherImport(HttpPostedFile file,Authentication.Authentication auth,string course_id, ref string errorMsg)
        {
            var dtl = new Models.DataTools.DataTools();
            int total = 0;
            //dynamic output = null;
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
            CultureInfo provider = CultureInfo.InvariantCulture;

            string tmp_f = "";
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    throw new Exception("No file upload.");
                }
                if (System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "") != "xlsx")
                {
                    throw new Exception("File not supported (.xlsx only)");
                }
                tmp_f = new UploadFile(file).FileSave();

                int line = 0;

                List<Data.DataModels.course_voucher> v_ = new List<Data.DataModels.course_voucher>();
                using (var db = new DataContext())
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(virtual_dir+ tmp_f);

                    using (var excel = new ExcelPackage(fi))
                    {
                        var ws = excel.Workbook.Worksheets.First();
                        int? id =( db.course_exam.Select(x => x.id).Max()??0) + 1;
                        int? course_exam_id = (db.course_exam_answer.Select(x => x.id).Max() ?? 0) + 1;
                        var start_row = 2;
                        var format = "dd/MM/yyyy";
                        string[] stringSeparators = new string[] { "," };
                        var j = 0;
                        List<Data.DataModels.course_voucher> d = new List<Data.DataModels.course_voucher>();

                        while ((ws.Cells[start_row, 1].Value?.ToString().Trim() ?? "") != "")
                        {
                            var start_dt = ws.Cells[start_row, 2].Text;
                            var end_dt = ws.Cells[start_row, 3].Text;
                            var v = new Data.DataModels.course_voucher();
                            v.voucher_id = ws.Cells[start_row, 1].Value.ToString().Trim();
                            v.course_id = course_id;
                            v.start_dt = start_dt == null || start_dt.ToString() == "" ? (DateTime?)null : DateTime.ParseExact(start_dt.ToString().Trim(), format, provider);
                            v.end_dt = end_dt == null || end_dt.ToString() == "" ? (DateTime?)null : DateTime.ParseExact(end_dt.ToString().Trim(), format, provider);
                            v.created_at = DateTime.Now;
                            v.created_by = auth.user_id;
                            v.status = 0;
                            v.is_delete = 0;
                            v.update_by = auth.user_id;
                            v.update_dt = DateTime.Now;

                            if (db.course_voucher.Where(w => w.voucher_id == v.voucher_id && w.course_id == v.course_id).Count() == 0)
                            {
                                db.course_voucher.Add(v);
                                db.SaveChanges();
                            }
                            start_row++;
                        }
                    }

                    line = 0;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrorList(ex);
            }
            finally
            {
                System.IO.File.Delete(tmp_f);
            }

        }
        public dynamic UserImport(HttpPostedFile file,Authentication.Authentication auth, ref string errorMsg)
        {
            var dtl = new Models.DataTools.DataTools();
            int total = 0;
            dynamic output = null;
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

            string tmp_f = "";
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    throw new Exception("No file upload.");
                }
                if (System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "") != "xlsx")
                // if (System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "") != "xlsx" || file.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    throw new Exception("File not supported (.xlsx only)");
                }
                tmp_f = new UploadFile(file).FileSave();

                int line = 0;
                //throw new Exception(tmp_f);

              var user = new Data.DataModels.user();
              var users = new List<Data.DataModels.user>();
                //List<Data.DataModels.> detail = new List<Data.DataModels.course_exam_answer>();
            using (var db = new DataContext())
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(virtual_dir+ tmp_f);
                using (var excel = new ExcelPackage(fi))
                {
                    var ws = excel.Workbook.Worksheets.First();
                    int? id =( db.user.Select(x=>x.id).Max()??0);
                    var start_row = 2;
                    string[] stringSeparators = new string[] { "," };
                var j = 0;
                    while ((ws.Cells[start_row, 1].Value?.ToString().Trim() ?? "") != "")
                    {
                            user.id = ++id;
                            user.username = ws.Cells[start_row, 1].Value?.ToString().Trim();
                            user.password = ws.Cells[start_row, 2].Value?.ToString().Trim();
                            user.email = ws.Cells[start_row, 3].Value?.ToString().Trim();
                            user.role_id = 1;
                            db.user.Add(user);
                            db.SaveChanges();
                            users.Add(user);
                    }


                    
             
                    //          throw new Exception(JsonConvert.SerializeObject(work));
                }
            //var pre_event2_l = m_array.Select(x => x.pre_event2).Distinct().ToList();

           


              
                //throw new Exception(p.Count.ToString() + " ");

                line = 0;
          
                output = new
                {
                   data=users,
                };

            }

                //db.rd_mas_defect.AddRange(defect_array);
                //db.SaveChanges();
                //db.rd_mas_defect_group.AddRange(defect_group_array);
                //db.SaveChanges();
                //db.rd_mas_defect_type.AddRange(defect_type_array);
                //db.SaveChanges();

            }


            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrorList(ex);
            }
            finally
            {
                System.IO.File.Delete(tmp_f);

            }
            return output;

        }

        public dynamic EvaluationImport(HttpPostedFile file, Authentication.Authentication auth, string course_id, ref string errorMsg)
        {
            dynamic output = null;
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

            string tmp_f = "";
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    errorMsg = "No file upload.";
                    return false;
                }
                
                if (System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "") != "xlsx")
                {
                    errorMsg = "File not supported (.xlsx only)";
                    return false;
                }
                
                tmp_f = new UploadFile(file).FileSave();
                System.IO.FileInfo fi = new System.IO.FileInfo(virtual_dir + tmp_f);
                var data = new List<dynamic>();
                using (var excel = new ExcelPackage(fi))
                {
                    var ws = excel.Workbook.Worksheets.First();
                    var total_row = ws.Dimension.End.Row;
                    for(int row = 2; row <= total_row; row++)
                    {
                        var order = ws.Cells[row, 1].Text;
                        if(order != "")
                        {
                            if (!int.TryParse(order, out int n))
                            {
                                errorMsg = "ข้อมูลไม่ถูกต้อง";
                                return false;
                            }

                            var question = ws.Cells[row, 2].Text;
                            var choice_1 = ws.Cells[row, 3].Text;
                            var score_1 = ws.Cells[row, 4].Text;
                            var choice_2 = ws.Cells[row, 5].Text;
                            var score_2 = ws.Cells[row, 6].Text;
                            var choice_3 = ws.Cells[row, 7].Text;
                            var score_3 = ws.Cells[row, 8].Text;
                            var choice_4 = ws.Cells[row, 9].Text;
                            var score_4 = ws.Cells[row, 10].Text;
                            var choice_5 = ws.Cells[row, 11].Text;
                            var score_5 = ws.Cells[row, 12].Text;
                            var free_fill_text = ws.Cells[row, 13].Text;

                            if (order == "" || question == "")
                            {
                                errorMsg = "ข้อมูลไม่ครบ";
                                return false;
                            }

                            var data_choices = new List<dynamic>();
                            if ((choice_1 != "" && score_1 == "") || (choice_1 == "" && score_1 != ""))
                            {
                                errorMsg = "ข้อมูลไม่ครบ";
                                return false;
                            }
                            else if (choice_1 != "" && score_1 != "")
                            {
                                if (int.TryParse(score_1, out int n1))
                                {
                                    data_choices.Add(new
                                    {
                                        choice = choice_1,
                                        score = score_1,
                                        order = 1
                                    });
                                }
                                else
                                {
                                    errorMsg = "ข้อมูลไม่ถูกต้อง";
                                    return false;
                                }
                            }

                            if ((choice_2 != "" && score_2 == "") || (choice_2 == "" && score_2 != ""))
                            {
                                errorMsg = "ข้อมูลไม่ครบ";
                                return false;
                            }
                            else if (choice_2 != "" && score_2 != "")
                            {
                                if (int.TryParse(score_2, out int n2))
                                {
                                    data_choices.Add(new
                                    {
                                        choice = choice_2,
                                        score = score_2,
                                        order = 2
                                    });
                                }
                                else
                                {
                                    errorMsg = "ข้อมูลไม่ถูกต้อง";
                                    return false;
                                }
                            }

                            if ((choice_3 != "" && score_3 == "") || (choice_3 == "" && score_3 != ""))
                            {
                                errorMsg = "ข้อมูลไม่ครบ";
                                return false;
                            }
                            else if (choice_3 != "" && score_3 != "")
                            {
                                if (int.TryParse(score_3, out int n3))
                                {
                                    data_choices.Add(new
                                    {
                                        choice = choice_3,
                                        score = score_3,
                                        order = 3
                                    });
                                }
                                else
                                {
                                    errorMsg = "ข้อมูลไม่ถูกต้อง";
                                    return false;
                                }
                            }

                            if ((choice_4 != "" && score_4 == "") || (choice_4 == "" && score_4 != ""))
                            {
                                errorMsg = "ข้อมูลไม่ครบ";
                                return false;
                            }
                            else if (choice_4 != "" && score_4 != "")
                            {
                                if (int.TryParse(score_4, out int n4))
                                {
                                    data_choices.Add(new
                                    {
                                        choice = choice_4,
                                        score = score_4,
                                        order = 4
                                    });
                                }
                                else
                                {
                                    errorMsg = "ข้อมูลไม่ถูกต้อง";
                                    return false;
                                }
                            }

                            if ((choice_5 != "" && score_5 == "") || (choice_5 == "" && score_5 != ""))
                            {
                                errorMsg = "ข้อมูลไม่ครบ";
                                return false;
                            }
                            else if (choice_5 != "" && score_5 != "")
                            {
                                if (int.TryParse(score_5, out int n5))
                                {
                                    data_choices.Add(new
                                    {
                                        choice = choice_5,
                                        score = score_5,
                                        order = 5
                                    });
                                }
                                else
                                {
                                    errorMsg = "ข้อมูลไม่ถูกต้อง";
                                    return false;
                                }
                            }

                            data.Add(new
                            {
                                order = order,
                                question = question,
                                is_free_fill_text = (free_fill_text != "" ? 1 : 0),
                                free_fill_text = free_fill_text,
                                data_choices = data_choices
                            });
                        }
                    }
                }

                using (var db = new DataContext())
                {
                    foreach (var item in data)
                    {
                        course_evaluation course_evaluation = new course_evaluation();
                        course_evaluation.course_id = course_id .ToString();
                        course_evaluation.order = int.Parse(item.order);
                        course_evaluation.question = item.question;
                        course_evaluation.is_free_fill_text = item.is_free_fill_text;
                        course_evaluation.free_fill_text = item.free_fill_text;
                        course_evaluation.is_deleted = 0;
                        course_evaluation.created_by = auth.user_id;
                        course_evaluation.created_dt = DateTime.Now;
                        course_evaluation.update_by = auth.user_id;
                        course_evaluation.update_dt = DateTime.Now;

                        db.course_evaluation.Add(course_evaluation);
                        db.SaveChanges();

                        var course_evaluation_id = course_evaluation.id;
                        foreach (var choice in item.data_choices)
                        {
                            course_evaluation_choices course_evaluation_choices = new course_evaluation_choices();
                            course_evaluation_choices.course_evaluation_id = course_evaluation_id;
                            course_evaluation_choices.choice = choice.choice;
                            course_evaluation_choices.score = int.Parse(choice.score);
                            course_evaluation_choices.order = choice.order;

                            db.course_evaluation_choices.Add(course_evaluation_choices);
                            db.SaveChanges();
                        }
                    }
                }
                
                output = data;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrorList(ex);
            }
            finally
            {
                System.IO.File.Delete(tmp_f);

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