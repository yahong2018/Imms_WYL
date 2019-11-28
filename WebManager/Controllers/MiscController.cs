using System;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Imms.WebManager.Controllers
{   
    public class BarCodeController : Controller
    {
        [Route("misc/qrcode/create")]
        public void GetQRCode(string base64Content, int pixel)
        {
            Response.ContentType = "image/jpeg";

            string content = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Content.Replace("$_$_$_$_$","+")));

            var bitmap = Imms.Core.QRCoder.GetQRCode(content, pixel);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);

            Response.Body.WriteAsync(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            Response.Body.Close();                        
        }
    }
}