using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Configuration;
using System.Net.Http.Formatting;
using System.Web.Http.Cors;
using DpimProject.Models.DataTools;
using DpimProject.Models.Data;
using Newtonsoft.Json;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Caching;
using NReco.VideoConverter;
using OfficeOpenXml;
using OfficeOpenXml.Style;
namespace DpimProject.Controllers
{
    /// Controller For Upload All File ///
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FileUploadController : ApiController
    {
        private Models.Authentication.Authentication auth;
        private Models.Student student;
        private string fileUrl = System.Configuration.ConfigurationManager.AppSettings["FileUrl"]??"";
        public FileUploadController()
        {
            auth = new Models.Authentication.Authentication();
            //student = new Models.Student();
            //student.checkCourseEnddate();
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
            auth.IsAdmin = true;




        }
        [ActionName("VoucherImport")]
        [HttpPost]
        public dynamic VoucherImport(string course_id)
        {
            dynamic output = null;
           
                auth.IsAdmin = true;

                //if (auth.isAuthenticated)
                //{
                string error = "";
                GetToken(ref error);

                var file = HttpContext.Current.Request.Files.Count > 0 ?
                    HttpContext.Current.Request.Files[0] : null;

                if (file != null && file.ContentLength > 0)
                {

                    //UploadFile fu = new UploadFile(file, false, false, false, 0, 0);
                    //var path= fu.FileSave();
                    new Models.Document().VoucherImport(file, auth, course_id, ref error);




                    output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error


                    };
                    //}
                }


           
            return output;
        }
        [ActionName("ExamImport")]
        [HttpPost]
        public dynamic ExamImport(string course_id)
        {
            dynamic output = null;
            auth.IsAdmin = true;

            //if (auth.isAuthenticated)
            //{
            string error = "";
            GetToken(ref error);

            var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;

                if (file != null && file.ContentLength > 0)
                {

                    //UploadFile fu = new UploadFile(file, false, false, false, 0, 0);
                    //var path= fu.FileSave();
                    var data = new Models.Document().ExamImport(file, auth, course_id, ref error);




                    output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error,
                        data

                    };
                    //}
                }


           
            return output;
        }
        [ActionName("UserImport")]
        [HttpPost]
        public dynamic UserImport( )
        {
            dynamic output = null;
            try {
            auth.IsAdmin = true;

            //if (auth.isAuthenticated)
            //{
            string error = "";
            GetToken(ref error);

            var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;

                if (file != null && file.ContentLength > 0)
                {

                    //UploadFile fu = new UploadFile(file, false, false, false, 0, 0);
                    //var path= fu.FileSave();
                    var data = new Models.Document().UserImport(file, auth, ref error);




                    output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error,
                        data

                    };
                    //}
                }


            }catch(Exception ex)
            {
                ErrorList(ex);
            }
            return output;
        }
        [ActionName("EvaluationImport")]
        [HttpPost]
        public dynamic EvaluationImport(string courseId)
        {
            dynamic output = null;
            try
            {
                auth.IsAdmin = true;
                
                string error = "";
                GetToken(ref error);

                var file = HttpContext.Current.Request.Files.Count > 0 ?
                    HttpContext.Current.Request.Files[0] : null;

                if (file != null && file.ContentLength > 0)
                {
                    var data = new Models.Document().EvaluationImport(file, auth, courseId, ref error);

                    output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error,
                        data

                    };
                }
            }
            catch (Exception ex)
            {
                ErrorList(ex);
            }
            return output;
        }
        [ActionName("FileUpload")]
        [HttpPost]
        public dynamic FileUpload(int max_w=0,int max_h=0)
        {
          
            bool imgResize = false;
            if (max_w >0 && max_h>0)
            {
                imgResize = true;
            }
            var filePath = new List<object>();
            string error = "";
            GetToken(ref error);

            var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;
            if (string.IsNullOrEmpty(error))
            {
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {

                        string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

                        UploadFile fu = new UploadFile(file, imgResize, false, false, max_w, max_h);
                        var path = fu.FileSave();
                        var token = new Security.Token().CreateToken(path);
                        var token_ = "";
                        if (new Security.Token().CheckToken(token, out token_)) ;
                        var files = new
                        {
                            success = string.IsNullOrEmpty(error),
                            error,
                            path = fileUrl + path
                        };
                        filePath.Add(files);
                        //string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        //Directory.CreateDirectory(virtual_dir);

                        //var path = Path.Combine(
                        //            HttpContext.Current.Server.MapPath("~/FileUpload/")+DateTime.Now.ToString("yyyyMMdd") + "\\",
                        //    fileName
                        //);

                    }
                }
                catch (Exception ex)
                {

                    ErrorList(ex);
                }
            }
            return filePath;
        }   [ActionName("LicenseUpload")]
        [HttpPost]
        public dynamic LicenseUpload()
        {
          
          
            var filePath = new List<object>();
            string error = "";
            GetToken(ref error);

            var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;
          
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {

                    string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

                    UploadFile fu = new UploadFile(file, true, false, false, 293, 92);
                        var path = fu.FileSave();
                        var token = new Security.Token().CreateToken(path);
                        var token_ = "";
                        if (new Security.Token().CheckToken(token, out token_)) ;
                        var files = new
                        {
                            success = string.IsNullOrEmpty(error),
                            error,
                            path =virtual_dir+ path
                        };
                        filePath.Add(files);
                        //string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        //Directory.CreateDirectory(virtual_dir);

                        //var path = Path.Combine(
                        //            HttpContext.Current.Server.MapPath("~/FileUpload/")+DateTime.Now.ToString("yyyyMMdd") + "\\",
                        //    fileName
                        //);

                    }
                }
                catch (Exception ex)
                {
                ErrorList(ex);
                }
            
            return filePath;
        }
        [ActionName("VideoUpload")]
        [HttpPost]
        public dynamic VideoUpload()
        {
            auth.IsAdmin = true;

            //GetToken();
            dynamic output = null;
            //if (auth.isAuthenticated) { 
            var filePath = new List<object>();
            string error = "";
            GetToken(ref error);

            var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;
            try
            {
                if (file != null && file.ContentLength > 0)
                {

                    string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

                    UploadFile fu = new UploadFile(file, false, false, true, 500, 500);
                    var data = fu.ConverterFile(ref error);
                    output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error,
                        data
                    };
                    //string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
                    //Directory.CreateDirectory(virtual_dir);

                    //var path = Path.Combine(
                    //            HttpContext.Current.Server.MapPath("~/FileUpload/")+DateTime.Now.ToString("yyyyMMdd") + "\\",
                    //    fileName
                    //);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //}
            return output;
        }
        [ActionName("FilesUpload")]
        [HttpPost]
        public dynamic FilesUpload()
        {
            string error = "";
            GetToken(ref error);

            var filePath = new List<object>();

            var file = HttpContext.Current.Request.Files;
           
                for (var i = 0; i < file.Count; i++)
                {
                    try
                    {
                        if (file != null && file[i].ContentLength > 0)
                        {

                            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";

                            UploadFile fu = new UploadFile(file[i], false, false, false, 500, 500);
                            var path = fu.FileSave();
                            var token = new Security.Token().CreateToken(path);
                            var token_ = "";
                            if (new Security.Token().CheckToken(token, out token_)) ;
                            var files = new
                            {
success=string.IsNullOrEmpty(error),
error,
                                path = fileUrl + path
                            };
                            filePath.Add(files);
                            //string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
                            //Directory.CreateDirectory(virtual_dir);

                            //var path = Path.Combine(
                            //            HttpContext.Current.Server.MapPath("~/FileUpload/")+DateTime.Now.ToString("yyyyMMdd") + "\\",
                            //    fileName
                            //);

                        }
                    }
                    catch (Exception ex)
                    {
                    ErrorList(ex);
                    }
                }
            
            return filePath;

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
    /// Controller For Download And Open File ///

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FileController : ApiController
    {
       
        private HttpContent HttpContent;
        private TransportContext TransportContext;
        private Stream stream;
        private static Security.Token token_;
        private static Models.Authentication.Authentication auth;
        #region Fields
        public const int ReadStreamBufferSize = 1024 * 1024;
        public static readonly IReadOnlyDictionary<string, string> MimeName;
        public static readonly IReadOnlyCollection<char> InvalidFileNameChars;
        // Where are your videos located at? Change the value to any folder you want.
        public static readonly string InitialDirectory;

        #endregion
        #region Constructors

        static FileController()
        {
            token_ = new Security.Token();
            auth = new Models.Authentication.Authentication();
            var mimeNames = new Dictionary<string, string>();

            mimeNames.Add(".mp3", "audio/mpeg");    // List all supported media types; 
            mimeNames.Add(".mp4", "video/mp4");
            mimeNames.Add(".ogg", "application/ogg");
            mimeNames.Add(".ogv", "video/ogg");
            mimeNames.Add(".oga", "audio/ogg");
            mimeNames.Add(".wav", "audio/x-wav");
            mimeNames.Add(".webm", "video/webm");

            MimeName = new ReadOnlyDictionary<string, string>(mimeNames);

            InvalidFileNameChars = Array.AsReadOnly(Path.GetInvalidFileNameChars());
            InitialDirectory = WebConfigurationManager.AppSettings["InitialDirectory"];
        }
        #endregion
        #region Actions

        // Later we will do something around here.

        #endregion

        #region Others

        private static bool AnyInvalidFileNameChars(string fileName)
        {
            return InvalidFileNameChars.Intersect(fileName).Any();
        }

        private static MediaTypeHeaderValue GetMimeNameFromExt(string ext)
        {
            string value;

            if (MimeName.TryGetValue(ext.ToLowerInvariant(), out value))
                return new MediaTypeHeaderValue(value);
            else
                return new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);
        }

        private static bool TryReadRangeItem(RangeItemHeaderValue range, long contentLength,
            out long start, out long end)
        {
            if (range.From != null)
            {
                start = range.From.Value;
                if (range.To != null)
                    end = range.To.Value;
                else
                    end = contentLength - 1;
            }
            else
            {
                end = contentLength - 1;
                if (range.To != null)
                    start = contentLength - range.To.Value;
                else
                    start = 0;
            }
            return (start < contentLength && end < contentLength);
        }

        private static void CreatePartialContent(Stream inputStream, Stream outputStream,
            long start, long end)
        {
            int count = 0;
            long remainingBytes = end - start + 1;
            long position = start;
            byte[] buffer = new byte[ReadStreamBufferSize];

            inputStream.Position = start;
            do
            {
                try
                {
                    if (remainingBytes > ReadStreamBufferSize)
                        count = inputStream.Read(buffer, 0, ReadStreamBufferSize);
                    else
                        count = inputStream.Read(buffer, 0, (int)remainingBytes);
                    outputStream.Write(buffer, 0, count);
                }
                catch (Exception error)
                {
                    Debug.WriteLine(error);
                    break;
                }
                position = inputStream.Position;
                remainingBytes = end - position + 1;
            } while (position <= end);
        }

        #endregion

        [ActionName("Stream")]
        [HttpGet]
        public HttpResponseMessage Play(string fileName)
        {
            // This can prevent some unnecessary accesses. 
            // These kind of file names won't be existing at all. 
            if (string.IsNullOrWhiteSpace(fileName) || AnyInvalidFileNameChars(fileName))
                throw new HttpResponseException(HttpStatusCode.NotFound);
            var path = fileName.Substring(0, 8);
            var st = fileName.Substring(0, fileName.IndexOf("_"));
            var dir = path + "\\" + st + "\\" + fileName;
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];

            FileInfo fileInfo = new FileInfo(virtual_dir + dir);

            if (!fileInfo.Exists)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            long totalLength = fileInfo.Length;

            RangeHeaderValue rangeHeader = base.Request.Headers.Range;
            HttpResponseMessage response = new HttpResponseMessage();

            response.Headers.AcceptRanges.Add("bytes");

            // The request will be treated as normal request if there is no Range header.
            if (rangeHeader == null || !rangeHeader.Ranges.Any())
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
                =>
                {
                    using (outputStream) // Copy the file to output stream straightforward. 
                    using (Stream inputStream = fileInfo.OpenRead())
                    {
                        try
                        {
                            inputStream.CopyTo(outputStream, ReadStreamBufferSize);
                        }
                        catch (Exception error)
                        {
                            Debug.WriteLine(error);
                        }
                    }
                }, GetMimeNameFromExt(fileInfo.Extension));

                response.Content.Headers.ContentLength = totalLength;
                return response;
            }

            long start = 0, end = 0;

            // 1. If the unit is not 'bytes'.
            // 2. If there are multiple ranges in header value.
            // 3. If start or end position is greater than file length.
            if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 ||
                !TryReadRangeItem(rangeHeader.Ranges.First(), totalLength, out start, out end))
            {
                Stream inputStream = fileInfo.OpenRead();
                response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Content = new StreamContent(inputStream);  // No content for this status.
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType = GetMimeNameFromExt(fileInfo.Extension);

                return response;
            }

            var contentRange = new ContentRangeHeaderValue(start, end, totalLength);

            // We are now ready to produce partial content.
            response.StatusCode = HttpStatusCode.PartialContent;
            response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
            =>
            {
                using (outputStream) // Copy the file to output stream in indicated range.
                using (Stream inputStream = fileInfo.OpenRead())
                    CreatePartialContent(inputStream, outputStream, start, end);

            }, GetMimeNameFromExt(fileInfo.Extension));

            response.Content.Headers.ContentLength = end - start + 1;
            response.Content.Headers.ContentRange = contentRange;

            return response;
        }
        private void GetToken()
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





        //Use the ReadToEnd method to read all the content from file

        [ActionName("Load2")]
        [HttpGet]
        public HttpResponseMessage Load2(string fileName,string course_id)
        {
            //GetToken();

           
            string file_;

            //token_.CheckToken(fileName, out file_);
            var path = fileName.Substring(0, 8);
            var dir = course_id + "\\" + fileName;
            //Create HTTP Response.       
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["Certificate"];
            
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                if (File.Exists(virtual_dir + dir))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + dir));

                        response.Headers.AcceptRanges.Add("bytes");
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(memoryStream);
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = fileName;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
                        response.Content.Headers.ContentLength = memoryStream.Length;
                    }
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("File Not Found"),
                        ReasonPhrase = "File Not Found"
                    };
                }
            }catch(Exception ex)
            {
               
                ErrorList(ex);
            }
            return response;
        }
        [ActionName("Load")]
        [HttpGet]

        public HttpResponseMessage Load(string fileName)
        {
            //GetToken();
            string file_;
            var dir = "";
            if (fileName.Contains("logo.png"))
            {
                dir = fileName;
            }
            else { 
                //token_.CheckToken(fileName, out file_);
                var path = fileName.Substring(0, 8);
         dir = path + "\\" + fileName;
        } //Create HTTP Response.       
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                if (File.Exists(virtual_dir + dir))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + dir));

                        response.Headers.AcceptRanges.Add("bytes");
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(memoryStream);
                        
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = dir;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(dir));
                        response.Content.Headers.ContentLength = memoryStream.Length;
                    }
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("File not Found"),
                        ReasonPhrase = "File Not Found"
                    };
                }
            }catch(Exception ex)
            {
               
                ErrorList(ex);
            }
            return response;
        }
        //[ActionName("LoadCert")]
        //[HttpGet]

        //public HttpResponseMessage LoadCert(string fileName)
        //{
        //    //GetToken();
        //    string file_;
        //    //token_.CheckToken(fileName, out file_);
        //    var path = fileName.Substring(0, 8);
        //    var dir = path + "\\" + fileName;
        //    //Create HTTP Response.       
        //    string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["Certificate"];

        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //    try
        //    {
        //        if(File.Exists(virtual_dir+dir))
        //        using (WebClient webClient = new WebClient())
        //        {
        //            var memoryStream = new MemoryStream(webClient.DownloadData(fileName));

        //            response.Headers.AcceptRanges.Add("bytes");
        //            response.StatusCode = HttpStatusCode.OK;
        //            response.Content = new StreamContent(memoryStream);
        //            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //            response.Content.Headers.ContentDisposition.FileName = fileName;
        //            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
        //            response.Content.Headers.ContentLength = memoryStream.Length;
        //        }
        //    }catch(Exception ex)
        //    {
        //        ErrorList(ex);
        //    }
        //    return response;
        //}
        [ActionName("ExamDownLoadTemp")]
        [HttpGet]

        public HttpResponseMessage ExamDownLoadTemp()
        {
            //GetToken();
            string file_;
            //token_.CheckToken(fileName, out file_);
            var filename = "single-choice_template.xlsx";
            //Create HTTP Response.       
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + "TmpImport\\";

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                if (File.Exists(virtual_dir + filename))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + filename));

                        response.Headers.AcceptRanges.Add("bytes");
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(memoryStream);
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = filename;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
                        response.Content.Headers.ContentLength = memoryStream.Length;
                    }
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("File not Found"),
                        ReasonPhrase = "File Not Found"
                    };
                }
            }
            catch(Exception ex)
            {
                ErrorList(ex);
            }
            return response;
        }  [ActionName("UserDownLoadTemp")]
        [HttpGet]

        public HttpResponseMessage UserDownLoadTemp()
        {
            //GetToken();
            string file_;
            //token_.CheckToken(fileName, out file_);
            var filename = "import-user.xlsx";
            //Create HTTP Response.       
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + "TmpImport\\";

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                if (File.Exists(virtual_dir + filename))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + filename));

                        response.Headers.AcceptRanges.Add("bytes");
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(memoryStream);
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = filename;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
                        response.Content.Headers.ContentLength = memoryStream.Length;
                    }
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("File not Found"),
                        ReasonPhrase = "File Not Found"
                    };
                }
            }
            catch(Exception ex)
            {
                ErrorList(ex);
            }
            return response;
        }
        [ActionName("EvaluationDownLoadTemp")]
        [HttpGet]
        public HttpResponseMessage EvaluationDownLoadTemp()
        {
            //GetToken();
            string file_;
            //token_.CheckToken(fileName, out file_);
            var filename = "EvaluationDownLoadTemp.xlsx";
            //Create HTTP Response.       
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + "TmpImport\\";

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                if (File.Exists(virtual_dir + filename))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + filename));

                        response.Headers.AcceptRanges.Add("bytes");
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(memoryStream);
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = filename;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
                        response.Content.Headers.ContentLength = memoryStream.Length;
                    }
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("File not Found"),
                        ReasonPhrase = "File Not Found"
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorList(ex);
            }
            return response;
        }

        [ActionName("VoucherDownLoadTemp")]
        [HttpGet]
        public HttpResponseMessage VoucherDownLoadTemp()
        {
            //GetToken();
            string file_;
            //token_.CheckToken(fileName, out file_);
            var filename = "import_voucher.xlsx";
            //Create HTTP Response.       
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"] + "TmpImport\\";

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                if (File.Exists(virtual_dir + filename))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + filename));

                        response.Headers.AcceptRanges.Add("bytes");
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(memoryStream);
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = filename;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
                        response.Content.Headers.ContentLength = memoryStream.Length;
                    }
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("File not Found"),
                        ReasonPhrase = "File Not Found"
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorList(ex);
            }
            return response;
        }


        [ActionName("LoadCover")]
        [HttpGet]

        public HttpResponseMessage LoadCover(string fileName)
        {
            //GetToken();
            string file_;
            //token_.CheckToken(fileName, out file_);
            var path = fileName.Substring(0, 8);
            var st = fileName.Substring(0, fileName.IndexOf("_"));
            var dir = path + "\\" + st + "\\" + fileName;
            //Create HTTP Response.       
            string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                if (File.Exists(virtual_dir + dir))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + dir));

                        response.Headers.AcceptRanges.Add("bytes");
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(memoryStream);
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = dir;
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(dir));
                        response.Content.Headers.ContentLength = memoryStream.Length;
                    }
                }
                else
                {
                    var data = new
                    {
                        success = false,
                        error = "File Not Found"
                    };
                    response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new ObjectContent<object>(data,new JsonMediaTypeFormatter()),
                        ReasonPhrase = "File Not Found"
                    };
                }
            }catch(Exception ex)
            {
              
                ErrorList(ex);
            }
            return response;
        }
        #region NotUse

        //[ActionName("Stream2")]
        //[HttpGet]
        //public HttpResponseMessage StreamVDO(string fileName)
        //{
        //    //GetToken();        
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //    try
        //    {

        //        using (var db = new DataContext())
        //        {
        //            var data = db.video_status.Where(x => x.filename == fileName).FirstOrDefault();
        //            if (data.status == "Y")
        //            {
        //                string file_;
        //                //token_.CheckToken(fileName, out file_);
        //                var path = fileName.Substring(0, 8);
        //                var st = fileName.Substring(0, fileName.IndexOf("_"));
        //                var dir = path + "\\" + st + "\\" + fileName;
        //                //Create HTTP Response.       
        //                string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
        //                if (File.Exists(virtual_dir + dir))
        //                {
        //                    using (WebClient webClient = new WebClient())
        //                    {
        //                        var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + dir)) ;

        //                        //response.Headers.AcceptRanges.Add("bytes");
        //                        response.StatusCode = HttpStatusCode.OK;
        //                        response.Content = new StreamContent(memoryStream);
        //                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //                        response.Content.Headers.ContentDisposition.FileName = dir;
        //                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
        //                        response.Content.Headers.ContentLength = memoryStream.Length;
        //                    }
        //                }
        //                else {
        //                    throw new Exception("File Not Found");
        //            }
        //            }
        //            else
        //            {
        //                throw new Exception("Video Not Ready");
        //            }
        //        }

        //    }catch(Exception ex)
        //    {

        //        ErrorList(ex);
        //    }
        //    return response;
        //}
        //  [ActionName("Stream")]
        //[HttpGet]
        //public async Task<HttpResponseMessage> StreamVDO2(string fileName)
        //{
        //    MemoryCache cache = MemoryCache.Default; 

        ////GetToken();        
        //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //    try
        //    {                var buffer = new byte[65536];

        //        using (var db = new DataContext())
        //        {
        //            var data = db.video_status.Where(x => x.filename == fileName).FirstOrDefault();
        //            if (data.status == "Y")
        //            {
        //                string file_;
        //                //token_.CheckToken(fileName, out file_);
        //                var path = fileName.Substring(0, 8);
        //                var st = fileName.Substring(0, fileName.IndexOf("_"));
        //                var dir = path + "\\" + st + "\\" + fileName;
        //                //Create HTTP Response.       
        //                string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
        //                if (File.Exists(virtual_dir + dir))
        //                {
        //                    var expire = DateTimeOffset.Now.AddMinutes(60);
        //                    using (WebClient webClient = new WebClient())
        //                    {

        //                        var range = Request.Headers.Range?.Ranges?.FirstOrDefault();
        //                        if (range == null)
        //                        {


        //                            var stream = new MemoryStream();
        //                            using (var video = new MemoryStream(webClient.DownloadData(virtual_dir + dir))) await video.CopyToAsync(stream);

        //                            //response.Content = new ByteArrayContent(stream.ToArray());
        //                            response.Content = new StreamContent(stream);
        //                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
        //                            response.Content.Headers.ContentLength = File.OpenRead(virtual_dir + dir).Length;
        //                            //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //                            //response.Content.Headers.ContentDisposition.FileName = dir;
        //                        }
        //                        else
        //                        {
        //                            var stream = new MemoryStream();
        //                            //using (var video = File.OpenRead(virtual_dir + dir)) await video.CopyToAsync(stream);
        //                            using (var video = new MemoryStream(webClient.DownloadData(virtual_dir + dir))) await video.CopyToAsync(stream);

        //                            response.Content = new ByteRangeStreamContent(stream,
        //                                new RangeHeaderValue(range.From, range.To),
        //                                new MediaTypeHeaderValue("video/mp4"));
        //                            //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //                            //response.Content.Headers.ContentDisposition.FileName = dir;
        //                            response.Content.Headers.ContentLength = (range.To.HasValue ? range.To.Value + 1 :stream.Length) - (range.From ?? 0);
        //                            /* byte[] video = webClient.DownloadData(virtual_dir + dir);*//* File.ReadAllBytes(virtual_dir + dir);*/
        //                                                                                          //    Stream stream;
        //                                                                                          //MemoryStream memoryStream = new MemoryStream();
        //                                                                                          //string value = "";
        //                                                                                          //if (cache.Contains(fileName)) {
        //                                                                                          //        video = GetMemoryCache(fileName);
        //                                                                                          //        //memoryStream =(MemoryStream)cache.Get(fileName);
        //                                                                                          //        //    cache.Set(fileName, memoryStream, expire);
        //                                                                                          //        //throw new Exception(video.ToString());
        //                                                                                          //        value = "Get Cach";
        //                                                                                          //}
        //                                                                                          //else
        //                                                                                          //{
        //                                                                                          //        //video = webClient.DownloadData(virtual_dir + dir);
        //                                                                                          //        var cacheItemPolicy = new CacheItemPolicy
        //                                                                                          //    {
        //                                                                                          //        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60)
        //                                                                                          //    };

        //                            //    //var cacheItem = new CacheItem(fileName, video,expire);
        //                            //    cache.Add(fileName, video, expire);

        //                            //        value = "Add Cach";
        //                            //    }

        //                            //}


        //                        }
        //                    }
        //                            //response.Headers.AcceptRanges.Add("bytes");
        //                            //response.StatusCode = HttpStatusCode.OK;

        //                            //response.Content = new StreamContent(memoryStream);

        //                            //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("video");
        //                            //response.Content.Headers.ContentDisposition.FileName = dir;
        //                            //response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(dir));
        //                            //response.Content.Headers.ContentLength = memoryStream.Length;
        //                            //response.Dispose();

        //                            //}




        //                    }

        //                else
        //                {
        //                        throw new Exception("File Not Found");
        //                    }
        //                }
        //                else
        //                {
        //                    throw new Exception("Video Not Ready");
        //                }

        //        }
        //    }catch(Exception ex)
        //    {

        //        ErrorList(ex);
        //    }
        //    return response;
        //}
        //[ActionName("Stream3")]
        //[HttpGet]
        //public async Task<HttpResponseMessage> StreamVDO3(string fileName)
        //{
        //    //GetToken();        
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //    try
        //    {
        //        var buffer = new byte[65536];
        //        using (var db = new DataContext())
        //        {
        //            var data = db.video_status.Where(x => x.filename == fileName).FirstOrDefault();
        //            if (data.status == "Y")
        //            {
        //                int count;
        //                byte[] byteArray;
        //                string file_;
        //                //token_.CheckToken(fileName, out file_);
        //                var path = fileName.Substring(0, 8);
        //                var st = fileName.Substring(0, fileName.IndexOf("_"));
        //                var dir = path + "\\" + st + "\\" + fileName;
        //                //Create HTTP Response.       
        //                string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
        //                if (File.Exists(virtual_dir + dir))
        //                {
        //                    using (WebClient webClient = new WebClient())
        //                    {

        //                        var range = Request.Headers.Range?.Ranges?.FirstOrDefault();
        //                        if (range == null)
        //                        {
        //                            var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + dir));



        //                                response.Content = new ByteArrayContent(memoryStream.ToArray());
        //                                response.Content.Headers.ContentLength = memoryStream.Length;


        //                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //                            response.Content.Headers.ContentDisposition.FileName = dir;
        //                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
        //                        }
        //                        else
        //                        {

        //                            var memoryStream = new MemoryStream(webClient.DownloadData(virtual_dir + dir));
        //                            response.Content = new ByteRangeStreamContent(memoryStream,
        //                        new RangeHeaderValue(range.From, range.To),
        //                        new MediaTypeHeaderValue("video/mp4")
        //                        );

        //                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //                            response.Content.Headers.ContentDisposition.FileName = dir;
        //                            response.Content.Headers.ContentLength = (range.To.HasValue ? range.To.Value + 1 : webClient.DownloadData(virtual_dir + dir).Length) - (range.From ?? 0);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    throw new Exception("File Not Found");
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("Video Not Ready");
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        ErrorList(ex);
        //    }
        //    return response;
        //}
        #endregion
        [ActionName("ImageDelete")]
        [HttpGet]
        public HttpResponseMessage ImageDelete(string FileName)
        {
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            try
            {
                string error = "";
                var dir = FileName.Substring(0, 8);
                var path = dir + "\\" + FileName;
                string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
                auth.IsAdmin = true;
                GetToken();
                if (File.Exists(virtual_dir + path))
                {
                    File.Delete(virtual_dir + path);

                    output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error
                    };

                    resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
                }
                else
                {
                    throw new Exception("File Not Found");

                }

            }
            catch(Exception ex)
            {
                ErrorList(ex);
            }
            return resp;
        }
        [ActionName("VideoDelete")]
        [HttpGet]
        public HttpResponseMessage VideoDelete(string FileName)
        {
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            dynamic output = null;
            try
            {
                string error = "";
                var dir = FileName.Substring(0, 8);
                var sub_dir = FileName.Substring(0, FileName.IndexOf("_"));
                var path = dir + "\\" + sub_dir;
                string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
                auth.IsAdmin = true;

                GetToken();
                if (Directory.Exists(virtual_dir + path))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(virtual_dir + path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir_ in di.GetDirectories())
                    {
                        dir_.Delete(true);
                    }
                    Directory.Delete(virtual_dir + path);
                    output = new
                    {
                        success = string.IsNullOrEmpty(error),
                        error
                    };

                    resp.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());
                }
                else
                {
                    throw new Exception("File Not Found");

                }

            }
            catch(Exception ex)
            {
                ErrorList(ex);
            }
            return resp;
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
                success=string.IsNullOrEmpty(ex.Message),
                error = ex.Message,
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

        /////++++Stream Live +++++//////
        //public HttpResponseMessage StreamVDO(string filename)
        //    {
        //        string error = "";

        //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //        using (var db = new DataContext())
        //        {

        //            var status = db.video_status.Where(x => x.filename == filename&&x.status.Contains("Y")).FirstOrDefault();
        //            if (status == null)
        //            {

        //                response = new HttpResponseMessage(HttpStatusCode.NoContent);

        //                //GetToken();
        //                //if(token_.CheckToken(filename, out filename));
        //                var output = new
        //                {
        //                    success = false,

        //                    error = "Video นี้กำลังทำการ Transcoder อยู่ยังไม่สามารถเล่นได้ขณะนี้ กรุณารอสักครู่"
        //                };
        //                response.Content = new ObjectContent<object>(output, new JsonMediaTypeFormatter());


        //            }


        //            else
        //            {
        //                response = new HttpResponseMessage(HttpStatusCode.OK);

        //                var video = new VideoStream(filename);
        //                var contentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
        //                response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)video.WriteToStream, contentType);
        //            }
        //                //}

        //            //}
        //        }
        //        return response;
        //}
        [ActionName("GetMemoryCache")]
        [HttpGet]
        public byte[] GetMemoryCache(string filename)
        {
            //MemoryStream a = new MemoryStream();
            byte[] a = null;
            try
            { 
            MemoryCache cache = MemoryCache.Default;
            if (cache.Contains(filename))
            {
                a = (byte[])cache.Get(filename, null);
            }
            }catch(Exception ex) { }
            return a;
        }
    }
  
    public class VideoStream
    {

        private readonly string _filename;
        private string virtual_dir = System.Configuration.ConfigurationManager.AppSettings["UploadPath"];
        private MemoryCache cache = new MemoryCache("StreamCache");
        public VideoStream(string filename)
        {

            var st = filename.Substring(0, DateTime.Now.ToString("ddMMYYYY").Length);
            var pt = filename.Substring(0, filename.IndexOf("_"));
            var dir = st + "\\" + pt + "\\" + filename;
            // string _file ;
            // Security.Token token = new Security.Token();
            //token.CheckTokenStream(filename,out _file);
            _filename = virtual_dir + dir;
        }

        public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];

                using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var length = (int)video.Length;
                    var bytesRead = 1;

                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                    var cacheItem = new CacheItem(_filename, outputStream);
                    var cacheItemPolicy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60)
                    };
                    cache.Set(cacheItem, cacheItemPolicy);
                }
            }
            catch (HttpException ex)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }

        }
   
        public async void WriteContentToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            try
            {
                //path of file which we have to read//  
                var filePath = HttpContext.Current.Server.MapPath("~/MicrosoftBizSparkWorksWithStartups.mp4");
                //here set the size of buffer, you can set any size  
                int bufferSize = 65536;
                byte[] buffer = new byte[65536];
                //here we re using FileStream to read file from server//  
                using (var fileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int totalSize = (int)fileStream.Length;
                    /*here we are saying read bytes from file as long as total size of file 

                    is greater then 0*/
                    while (totalSize > 0)
                    {
                        int count = totalSize > bufferSize ? bufferSize : totalSize;
                        //here we are reading the buffer from orginal file  
                        int sizeOfReadedBuffer = fileStream.Read(buffer, 0, count);
                        //here we are writing the readed buffer to output//  
                        await outputStream.WriteAsync(buffer, 0, sizeOfReadedBuffer);

                        //and finally after writing to output stream decrementing it to total size of file.  
                        totalSize -= sizeOfReadedBuffer;
                    }
                }
                if (outputStream == null)
                {
                    outputStream.Close();
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }

    }
}
