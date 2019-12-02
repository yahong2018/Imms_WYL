using System.Linq;
using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Imms.Mes.Data
{
    public class OperatorLogic : SimpleCRUDLogic<Operator>
    {
        private readonly IHostingEnvironment _host = null;
        private string[] _imgExtentions = new string[] { ".jpg", ".jpeg", ".bmp", ".png", ".gif" };
        public OperatorLogic(IHostingEnvironment host)
        {
            this._host = host;
        }

        protected override void BeforeInsert(Operator item, Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            this.SavePic(item, dbContext);
        }

        protected override void BeforeUpdate(Operator item, Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            this.SavePic(item, dbContext);
        }

        protected override void AfterDelete(System.Collections.Generic.List<Operator> items, Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            //删除图片
            string webRootPath = this._host.WebRootPath;
            foreach (Operator item in items)
            {
                string fullPath = webRootPath + "/" + item.Pic;
                if (!string.IsNullOrEmpty(fullPath) && System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }

        private void SavePic(Operator item, DbContext dbContext)
        {
            if (HttpContext.Current.Request.Form.Files.Count == 0)
            {
                return;
            }

            var file = HttpContext.Current.Request.Form.Files[0];
            string webRootPath = this._host.WebRootPath;
            string ext = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (!_imgExtentions.Contains(ext))
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, "只可以上传JPEG、PNG等图片文件");
            }

            Workline workline = dbContext.Set<Workline>().Where(x => x.OrgCode == item.orgCode).FirstOrDefault();
            if (workline == null)
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, "产线输入错误!");
            }

            string workshopCode = dbContext.Set<Workshop>().Where(x => x.RecordId == workline.ParentId).Select(x => x.OrgCode).Single();
            string wwwPath = $"upload/operators/{workshopCode}/{workline.OrgCode}/{item.EmpId}_{item.EmpName}{ext}";
            string fileName = $"{webRootPath}/{wwwPath}";
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
            string path = System.IO.Path.GetDirectoryName(fileName);
            if (!System.IO.Directory.Exists(path))
            { 
                System.IO.Directory.CreateDirectory(path);
            }
            using (var stream = System.IO.File.Create(fileName))
            {
                file.CopyTo(stream);
            }
            item.Pic = wwwPath;
        }
    }
}