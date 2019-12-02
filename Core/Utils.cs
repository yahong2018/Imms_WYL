using System;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using QRCoder;

namespace Imms.Core
{
    public class QRCoder
    {
        public static Bitmap GetQRCode(string content, int pixel)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData codeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);
            global::QRCoder.QRCode qrcode = new global::QRCoder.QRCode(codeData);

            Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, true);

            return qrImage;
        }
    }

    public class ExcelHelper
    {
        public static void ImportExcel(string fileName, string worksheetName, int firstRowNum, int lastRowNum, int firstColumn, int lastColumn, ExcelRowProcessHandler proessHandler)
        {
            if (proessHandler == null)
            {
                return;
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                string fileType = Path.GetExtension(fileName).ToLower();
                ExcelHelper.ImportExcel(fs, fileType, worksheetName, firstRowNum, lastRowNum, firstColumn, lastColumn, proessHandler);
                fs.Close();
            }
        }

        public static void ImportExcel(FileStream fs, string fileType, string worksheetName, int firstRowNum, int lastRowNum, int firstColumn, int lastColumn, ExcelRowProcessHandler proessHandler)
        {
            using (ExcelPackage package = new ExcelPackage(fs))
            {
                try
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[worksheetName];
                    ExcelHelper.ImportExcel(worksheet, firstRowNum, lastRowNum, firstColumn, lastColumn, proessHandler);
                }
                catch (Exception e)
                {
                    GlobalConstants.DefaultLogger.Error("导入Excel出错：" + e.Message);
                    GlobalConstants.DefaultLogger.Debug(e.StackTrace);
                }
            }
        }

        public static void ImportExcel(ExcelWorksheet sheet, int firstRowNum, int lastRowNum, int firstColumn, int lastColumn, ExcelRowProcessHandler proessHandler)
        {
            for (int i = firstRowNum; i < lastRowNum; i++)
            {
                ExcelCellValue[] cellValues = new ExcelCellValue[lastColumn - firstColumn];
                for (int j = firstColumn, n = 0; j < lastColumn; j++, n++)
                {
                    cellValues[n].Column = j;
                    cellValues[n].Value = sheet.Cells[i, j].Value;
                }

                if (proessHandler != null)
                {
                    proessHandler(cellValues);
                }
            }
        }
    }

    public delegate void ExcelRowProcessHandler(ExcelCellValue[] cellValues);

    public class ExcelCellValue
    {
        public int Column { get; set; }
        public object Value { get; set; }
    }
}