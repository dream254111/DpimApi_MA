using DpimProject.Models.Data.DataModels;
using DpimProject.Models.DataTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DpimProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PaymentController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Payment.Payment payment;
        private DataTools dtl;
        private Models.Student student;

        public PaymentController()
        {
            dtl = new DataTools();
            auth = new Models.Authentication.Authentication();
            payment = new Models.Payment.Payment();

            //string token = HttpContext.Current.Request.Cookies["dpim_auth"]?.Value?.Trim() ?? HttpContext.Current.Request.Headers.Get("Authorization");
            //if (!string.IsNullOrEmpty(token))
            //{
            //    auth.GetAuthentication(token);
            //}
        }

        private void GetToken(ref string errorMsg)
        {
            string token = "";
            try
            {
                var cookie = HttpContext.Current.Request.Headers.Get("Authorization");
                if (cookie != null)
                {
                    token = cookie?.ToString();
                    auth.GetAuthentication(token);

                }
                else
                {
                    throw new Exception("Token Not Found");
                }
            }
            catch (Exception ex)
            {

                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();                // Get the top stack frame
                string stackIndent = ex.StackTrace;
                var error = new
                {
                    success = string.IsNullOrEmpty(ex.Message),
                    ex.Message,
                    FileName = frame.GetFileName(),
                    line = frame.GetFileLineNumber(),
                    Method = frame.GetMethod()
                };
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new ObjectContent<object>(error, new JsonMediaTypeFormatter()),
                    ReasonPhrase = ex.Message
                };


                throw new HttpResponseException(resp);

            }




        }

        [ActionName("GetAllPayment")]
        [HttpGet]
        public dynamic GetAllPayment()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            var data = string.IsNullOrEmpty(error) ? payment.get_all_payment(ref error) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        /* Insert payment method is not necessary (2020/12/06 - Dream) */
        //[ActionName("InsertPayment")]
        //[HttpPost]
        //public dynamic InsertPayment()
        //{
        //    payment_method payment_model = new payment_method();
        //    payment_method body = Dtl.json_to_object(Dtl.json_request(), payment_model);

        //    var error = "";
        //    var data = payment.insert_payment(body, ref error, auth);
        //    var output = new
        //    {
        //        success = string.IsNullOrEmpty(error),
        //        error,
        //        data
        //    };

        //    return output;
        //}

        [ActionName("UpdatePayment")]
        [HttpPost]
        public dynamic UpdatePayment()
        {
            auth.IsAdmin = true;
            var error = "";
            GetToken(ref error);

            payment_method payment_model = new payment_method();
            payment_method body = Dtl.json_to_object(Dtl.json_request(), payment_model);

            var data = string.IsNullOrEmpty(error) ? payment.update_payment(body, ref error, auth) : null;
            var output = new
            {
                success = string.IsNullOrEmpty(error),
                error,
                data
            };

            return output;
        }

        /* Delete payment method is not necessary (2020/12/06 - Dream) */
        //[ActionName("DeletePayment")]
        //[HttpDelete]
        //public dynamic DeletePayment()
        //{
        //    string id = HttpContext.Current.Request.QueryString["id"];

        //    var error = "";
        //    var data = payment.delete_payment(id, ref error, auth);
        //    var output = new
        //    {
        //        success = string.IsNullOrEmpty(error),
        //        error,
        //        data
        //    };

        //    return output;
        //}
    }
}