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
namespace DpimProject.Models.Payment
{
    public class Payment
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        private readonly DateTime now;

        public Payment()
        {
            now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        }

        public dynamic get_all_payment(ref string error)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    var data = db.payment_method.ToList().Select(x => 
                    {
                        payment_method payment = new payment_method();

                        payment.id = x.id;
                        payment.bank_name = x.bank_name;
                        payment.line = x.line;
                        payment.account_name = x.account_name;
                        payment.account_no = x.account_no;
                        payment.is_deleted = x.is_deleted;
                        payment.created_by = x.created_by;
                        payment.created_at = x.created_at;
                        payment.update_by = x.update_by;
                        payment.update_at = x.update_at;
                        payment.qr_code = x.qr_code;// != null && x.qr_code.Length != 0 ? "https://dpimproject.ddns.net/DpimProjectV2/File/load?filename=" + x.qr_code : "";

                        return payment;
                    }).FirstOrDefault();

                    output = new { data };
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
            }

            return output;
        }

        public dynamic update_payment(payment_method payment, ref string error, Authentication.Authentication auth)
        {
            dynamic output = null;

            try
            {
                using (DataContext db = new DataContext())
                {
                    if (db.payment_method.Count() == 0)
                    {
                        payment_method data = new payment_method()
                        {
                            bank_name = payment.bank_name,
                            line = payment.line,
                            account_name = payment.account_name,
                            account_no = payment.account_no,
                            qr_code = payment.qr_code,

                            is_deleted = 0,
                            created_by = auth.user_id,
                            created_at = now,
                            update_by = auth.user_id,
                            update_at = now
                        };

                        if (data.bank_name == null || data.bank_name.Length == 0) throw new Exception("bank_name parameter is required");
                        if (data.line == null || data.line.Length == 0) throw new Exception("line parameter is required");
                        if (data.account_name == null || data.account_name.Length == 0) throw new Exception("account_name parameter is required");
                        if (data.account_no == null || data.account_no.Length == 0) throw new Exception("account_no parameter is required");

                        db.payment_method.Add(data);
                        db.SaveChanges();
                        output = new { data };
                    }
                    else
                    {
                        payment_method data = db.payment_method.Where(x => x.id != null).FirstOrDefault();

                        data.bank_name = payment.bank_name == null ? data.bank_name : payment.bank_name;
                        data.line = payment.line == null ? data.line : payment.line;
                        data.account_name = payment.account_name == null ? data.account_name : payment.account_name;
                        data.account_no = payment.account_no == null ? data.account_no : payment.account_no;
                        data.qr_code = payment.qr_code == null ? data.qr_code : payment.qr_code;

                        data.update_by = auth.user_id;
                        data.update_at = now;

                        //var qr_code = data.qr_code.Split('=');
                        //data.qr_code = qr_code[qr_code.Length - 1];

                        if (data.bank_name == null || data.bank_name.Length == 0) throw new Exception("bank_name parameter is required");
                        if (data.line == null || data.line.Length == 0) throw new Exception("line parameter is required");
                        if (data.account_name == null || data.account_name.Length == 0) throw new Exception("account_name parameter is required");
                        if (data.account_no == null || data.account_no.Length == 0) throw new Exception("account_no parameter is required");

                        db.SaveChanges();
                        output = new { data };
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;ErrorList(ex);
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
        /* Insert payment method is not necessary (2020/12/06 - Dream) */
        //public dynamic update_payment(payment_method payment, ref string error, Authentication.Authentication auth)
        //{
        //    dynamic output = null;

        //    try
        //    {
        //        using (DataContext db = new DataContext())
        //        {
        //            payment_method data = db.payment_method.Where(x => x.id == payment.id).FirstOrDefault();
        //            if (data != null)
        //            {
        //                data.bank_name = payment.bank_name == null ? data.bank_name : payment.bank_name;
        //                data.line = payment.line == null ? data.line : payment.line;
        //                data.qr_code = payment.qr_code == null ? data.qr_code : payment.qr_code;
        //                data.account_name = payment.account_name == null ? data.account_name : payment.account_name;
        //                data.account_no = payment.account_no == null ? data.account_no : payment.account_no;

        //                data.update_by = auth.user_id;
        //                data.update_at = now;
        //            }
        //            else
        //            {
        //                error = "Unable to find payment method with this ID";
        //            }

        //            if (data.bank_name == null || data.bank_name.Length == 0) throw new Exception("bank_name parameter is required");
        //            if (data.line == null || data.line.Length == 0) throw new Exception("line parameter is required");
        //            if (data.account_name == null || data.account_name.Length == 0) throw new Exception("account_name parameter is required");
        //            if (data.account_no == null || data.account_no.Length == 0) throw new Exception("account_no parameter is required");

        //            db.SaveChanges();
        //            output = new { data };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;ErrorList(ex);
        //    }

        //    return output;
        //}

        /* Delete payment method is not necessary (2020/12/06 - Dream) */
        //public dynamic delete_payment(string id, ref string error, Authentication.Authentication auth)
        //{
        //    dynamic output = null;

        //    try
        //    {
        //        using (DataContext db = new DataContext())
        //        {
        //            int? id_new = id == null ? (int?)null : int.Parse(id);

        //            payment_method data = db.payment_method.Where(x => x.id == id_new).FirstOrDefault();
        //            if (data != null)
        //            {
        //                data.is_deleted = 1;
        //                data.update_by = auth.user_id;
        //                data.update_at = now;
        //            }
        //            else
        //            {
        //                error = "Unable to find payment method with this ID";
        //            }

        //            db.SaveChanges();
        //            output = new { data };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;ErrorList(ex);
        //    }

        //    return output;
        //}
    }
}