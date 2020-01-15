using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Imms.Data;
using Imms.Mes.Data.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace Imms.Mes.Data
{
    public class WorkorderLogic : SimpleCRUDLogic<Workorder>
    {
        private IApplicationBuilder _App;
        private IHostingEnvironment _host;
        private string[] _exts = new string[] { ".xls", ".xlsx" };
        public WorkorderLogic(IApplicationBuilder app, IHostingEnvironment host)
        {
            this._App = app;
            this._host = host;
        }

        public int ImportExcel(int rowStart, int rowEnd, int errorHandle, IFormFile file)
        {
            List<Workorder> orders = Excel2Orders(rowStart, rowEnd, errorHandle, this.SaveExcel(file));
            CommonRepository.UseDbContext(context =>
            {
                foreach (Workorder order in orders)
                {
                    context.Set<Workorder>().Add(order);
                }
                context.SaveChanges();
            });
            return orders.Count;
        }

        private static List<Workorder> Excel2Orders(int rowStart, int rowEnd, int errorHandle, string fileName)
        {
            List<Workorder> orders = new List<Workorder>();
            FileInfo fileInfo = new FileInfo(fileName);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                package.Compatibility.IsWorksheets1Based = false;
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, "文件没有内容");
                }

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int maxRow = worksheet.Dimension.End.Row;
                if (maxRow < rowEnd)
                {
                    rowEnd = maxRow;
                }
                int maxCol = worksheet.Dimension.End.Column;
                if (maxCol < 9)
                {
                    throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, "数据小于9列");
                }

                for (int nRow = rowStart; nRow <= rowEnd; nRow++)
                {
                    try
                    {
                        Workorder order = Row2Workorder(worksheet, nRow);
                        orders.Add(order);
                    }
                    catch (Exception e)
                    {
                        GlobalConstants.DefaultLogger.Error($"导入Excel文件{fileName}出现错误:{e.Message}");
                        GlobalConstants.DefaultLogger.Error(e.StackTrace);

                        if (errorHandle == 0)
                        {
                            throw;
                        }
                    }
                }
            }

            return orders;
        }

        private static Workorder Row2Workorder(ExcelWorksheet worksheet, int nRow)
        {
            string lineNo = worksheet.Cells[nRow, 1].Value.ToString();
            string customer = worksheet.Cells[nRow, 2].Value.ToString();
            string jobNum = worksheet.Cells[nRow, 3].Value.ToString();
            string partNo = worksheet.Cells[nRow, 4].Value.ToString();
            string strBegin = worksheet.Cells[nRow, 5].Value.ToString();
            string strEnd = worksheet.Cells[nRow, 6].Value.ToString();
            string strReq = worksheet.Cells[nRow, 7].Value.ToString();
            string strUph = worksheet.Cells[nRow, 8].Value.ToString();
            string strOperatorCount = worksheet.Cells[nRow, 9].Value.ToString();

            DateTime timeBegin;
            if (!DateTime.TryParse(strBegin, out timeBegin))
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, $"将{nRow + 1}行,5列解析为开始时间失败");
            }
            DateTime timeEnd;
            if (!DateTime.TryParse(strBegin, out timeEnd))
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, $"将{nRow + 1}行,6列解析为结束时间失败");
            }

            int req;
            if (!int.TryParse(strReq, out req))
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, $"将{nRow + 1}行,7列解析为计划数量失败");
            }
            int uph;
            if (!int.TryParse(strReq, out uph))
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, $"将{nRow + 1}行,8列解析为UPH失败");
            }
            int operatorCount;
            if (!int.TryParse(strOperatorCount, out operatorCount))
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, $"将{nRow + 1}行,9列解析为人数失败");
            }

            Workorder order = new Workorder();
            order.LineNo = lineNo;
            order.CustomerNo = customer;
            order.OrderNo = jobNum;
            order.PartNo = partNo;
            order.PartName = "";
            order.TimeStartPlan = timeBegin;
            order.TimeEndPlan = timeEnd;
            order.QtyReq = req;
            order.UPH = uph;
            return order;
        }

        private string SaveExcel(IFormFile file)
        {
            string webRootPath = this._host.WebRootPath;
            string ext = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (!_exts.Contains(ext))
            {
                throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, "只可以上传*.xls、*.xlsx等图片文件");
            }

            string wwwPath = $"upload/workorder/{Guid.NewGuid().ToString()}/{file.FileName}";
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

            return fileName;
        }

        protected override void BeforeDelete(System.Collections.Generic.List<Workorder> items, DbContext dbContext)
        {
            foreach (Workorder order in items)
            {
                if (order.OrderStatus != Workorder.WORKORDER_STATUS_INITED)
                {
                    throw new BusinessException(GlobalConstants.EXCEPTION_CODE_CUSTOM, "已开工、已完工的订单不可以删除！");
                }
            }
        }

        protected override void AfterUpdate(Workorder item, DbContext dbContext)
        {
            if (item.OrderStatus > 0)
            {
                Imms.Mes.Services.Kanban.Line.DataService LineDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.DataService>();
                Task.Run(() => LineDataService.RefreshActiveWorkorders());

                Imms.Mes.Services.Kanban.Workshop.WorkshopDataService workshopDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Workshop.WorkshopDataService>();
                Task.Run(() => workshopDataService.RefreshWorkorder());

                Imms.Mes.Services.Kanban.Factory.FactoryDataService factoryDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Factory.FactoryDataService>();
                Task.Run(() => factoryDataService.RefreshWorkorder());
            }
        }

        public void CompleteWorkder(Workorder item)
        {
            if (item.OrderStatus == Workorder.WORKORDER_SATUS_CLOSED)
            {
                return;
            }

            try
            {
                this.DoCompolete(item);
                GlobalConstants.DefaultLogger.Info("已成功关闭工单:" + item.OrderNo);
            }
            catch (Exception e)
            {
                GlobalConstants.DefaultLogger.Error("关闭工单" + item.OrderNo + "失败:" + e.Message);
                GlobalConstants.DefaultLogger.Error(e.StackTrace);
            }

            this.RefreshKanban();
        }

        private void DoCompolete(Workorder item)
        {
            using (DbContext dbContext = GlobalConstants.DbContextFactory.GetContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    ActiveWorkorder active = dbContext.Set<ActiveWorkorder>().Where(x => x.LineNo == item.LineNo).FirstOrDefault();
                    if (active != null)
                    {
                        GlobalConstants.DefaultLogger.Info("清理旧数据:" + active.RecordId);
                        dbContext.Set<ActiveWorkorder>().Remove(active);
                    }
                    item.OrderStatus = Workorder.WORKORDER_SATUS_CLOSED;
                    item.TimeEndActual = DateTime.Now;
                    GlobalConstants.ModifyEntityStatus<Workorder>(item, dbContext);

                    dbContext.SaveChanges();
                    scope.Complete();
                }
            }
        }

        public void StartWorkder(Workorder workorder)
        {
            if (workorder.OrderStatus > 0)
            {
                return;
            }
            try
            {
                this.DoStart(workorder);
                GlobalConstants.DefaultLogger.Info("已启动工单:" + workorder.OrderNo);
            }
            catch (Exception e)
            {
                GlobalConstants.DefaultLogger.Error("启动工单出现错误:" + e.Message);
                GlobalConstants.DefaultLogger.Error(e.StackTrace);
            }
            this.RefreshKanban();
        }

        public void RefreshKanban()
        {
            Imms.Mes.Services.Kanban.Line.DataService dataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.DataService>();
            Task.Run(() =>
            {
                dataService.RefreshOrgAndSpanData();
                dataService.RefreshActiveWorkorders();
            });

            Imms.Mes.Services.Kanban.Workshop.WorkshopDataService workshopDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Workshop.WorkshopDataService>();
            Task.Run(() =>
            {
                workshopDataService.RefreshOrgData();
                workshopDataService.RefreshWorkorder();
            });

            Imms.Mes.Services.Kanban.Factory.FactoryDataService factoryDataService = _App.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Factory.FactoryDataService>();
            Task.Run(() =>
            {
                factoryDataService.RefreshOrgData();
                factoryDataService.RefreshWorkorder();
            });
        }

        private void DoStart(Workorder workorder)
        {
            workorder.OrderStatus = Workorder.WOKORDER_STATUS_STARTED;
            workorder.TimeStartActual = DateTime.Now;
            using (DbContext dbContext = GlobalConstants.DbContextFactory.GetContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    ActiveWorkorder active = dbContext.Set<ActiveWorkorder>().Where(x => x.LineNo == workorder.LineNo).FirstOrDefault();
                    bool isUpdate = true;
                    if (active == null)
                    {
                        GlobalConstants.DefaultLogger.Info("新增产线:" + workorder.LineNo + "的activeOrder.");
                        active = new ActiveWorkorder();
                        active.LineNo = workorder.LineNo;

                        dbContext.Add(active);
                        isUpdate = false;
                    }
                    else
                    {
                        GlobalConstants.DefaultLogger.Info("产线:" + workorder.LineNo + "已存在ActiveOrder" + active.WorkorderNo + "，先关闭原产线的工单.");
                        Workorder oldOrder = dbContext.Set<Workorder>().Where(x => x.OrderNo == active.WorkorderNo).FirstOrDefault();
                        if (oldOrder != null)
                        {
                            if (oldOrder.OrderStatus != Workorder.WORKORDER_SATUS_CLOSED && oldOrder.OrderNo != workorder.OrderNo)
                            {
                                oldOrder.OrderStatus = Workorder.WORKORDER_SATUS_CLOSED;
                                oldOrder.TimeEndActual = DateTime.Now;

                                GlobalConstants.ModifyEntityStatus<Workorder>(oldOrder, dbContext);
                            }
                        }
                    }
                    active.WorkorderNo = workorder.OrderNo;
                    active.LastUpdateTime = DateTime.Now;
                    active.PartNo = workorder.PartNo;
                    active.GID = 0;
                    active.DID = 0;
                    active.UpdateStatus = 0;

                    if (isUpdate)
                    {
                        GlobalConstants.ModifyEntityStatus<ActiveWorkorder>(active, dbContext);
                    }
                    GlobalConstants.ModifyEntityStatus<Workorder>(workorder, dbContext);

                    dbContext.SaveChanges();

                    scope.Complete();
                }
            }
        }
    }
}