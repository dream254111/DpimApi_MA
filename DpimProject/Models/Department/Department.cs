using DpimProject.Models.Data;
using DpimProject.Models.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace DpimProject.Models.Department
{
    public class Department
    {
        public dynamic DepartmentReadList(ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.department.OrderByDescending(o => o.update_dt).ToList();
                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
                }
            }
            return output;
        }
        
        public dynamic DepartmentManage(department d, Authentication.Authentication auth, ref string errorMsg)
        {
            dynamic output = null;
            using (var db = new DataContext())
            {
                try
                {
                    var data = db.department.Where(x => x.id == d.id).FirstOrDefault();
                    if (data != null)
                    {
                        data.department_name = d.department_name;
                        data.department_name_short = d.department_name_short;
                        data.update_by = auth.user_id;
                        data.update_dt = DateTime.Now;

                        db.SaveChanges();
                    }
                    else
                    {
                        department department = new department();
                        department.id = d.id;
                        department.department_name = d.department_name;
                        department.department_name_short = d.department_name_short;
                        department.create_by = auth.user_id;
                        department.create_dt = DateTime.Now;
                        department.update_by = auth.user_id;
                        department.update_dt = DateTime.Now;
                        

                        db.department.Add(department);
                        db.SaveChanges();
                    }

                    output = data;
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message; ErrorList(ex);
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