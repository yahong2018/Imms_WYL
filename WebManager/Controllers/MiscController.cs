using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Imms.WebManager.Controllers
{
    [Route("api/misc/qrcode")]
    public class BarCodeController : Controller
    {
        [Route("create")]
        public void GetQRCode(string base64Content, int pixel)
        {
            Response.ContentType = "image/jpeg";

            string content = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Content.Replace("$_$_$_$_$", "+")));

            var bitmap = Imms.Core.QRCoder.GetQRCode(content, pixel);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);

            Response.Body.WriteAsync(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            Response.Body.Close();
        }
    }

    [Route("api/misc/excel")]
    public class ExcelImortController : Controller
    {
        public ImportSession StartImport(string target)
        {
            ImportSession result = new ImportSession();
            result.SessionId = Guid.NewGuid();
            result.Worksheets = new string[] { "sheet1", "sheet2", "sheet3" };
            lock (sessionList)
            {
                sessionList.Add(result.SessionId, result);
            }
            return result;
        }

        static readonly SortedList<Guid, ImportSession> sessionList = new SortedList<Guid, ImportSession>();
    }

    public class ImportSession
    {        
        public Guid SessionId { get; set; }
        public int ActiveSheet { get; set; }
        public string[] Worksheets { get; set; }
        public int FieldRow { get; set; }
    }
}