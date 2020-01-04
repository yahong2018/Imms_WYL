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
        [Route("startImport")]
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

        [Route("getExcelFields")]
        public ImportSession GetExcelFields([FromBody]ImportSession session)
        {
            lock (sessionList)
            {
                sessionList[session.SessionId] = session;
            }

            session.FieldMappings = new FieldMapping[]{
                new FieldMapping(){ColumnIndex = 1, ExcelFieldCode="OrderNo",SystemFieldCode="order_no",SystemFieldLabel="订单号"},
                new FieldMapping(){ColumnIndex = 2, ExcelFieldCode="PartNo",SystemFieldCode="production_no",SystemFieldLabel="产品编号"}
            };

            return session;
        }

        static readonly SortedList<Guid, ImportSession> sessionList = new SortedList<Guid, ImportSession>();
    }

    public class ImportSession
    {
        public Guid SessionId { get; set; }
        public int ActiveSheet { get; set; }
        public string[] Worksheets { get; set; }
        public int FieldRowIndex { get; set; }
        public int ColumnStartIndex { get; set; }
        public int ColumnEndIndex { get; set; }
        public FieldMapping[] FieldMappings { get; set; }
    }

    public class FieldMapping
    {
        public string ExcelFieldCode { get; set; }
        public string SystemFieldCode { get; set; }
        public string SystemFieldLabel{get;set;}
        public int ColumnIndex { get; set; }
    }
}