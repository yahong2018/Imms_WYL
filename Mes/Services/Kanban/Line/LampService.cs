using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Imms.Mes.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace Imms.Mes.Services.Kanban.Line
{
    public class LampService : KanbanBaseService
    {
        private DataService _DataService = null;
        private DateTime _Yestoday = DateTime.Now;
        private SortedList<string, DateTime> _FirstTimeList = new SortedList<string, DateTime>();
        private SortedList<string, int> _LampStatusList = new SortedList<string, int>();
        private SortedList<string, DateTime> _LowRateTimeList = new SortedList<string, DateTime>();
        private SortedList<string,DateTime> _LastLightTimeList=new SortedList<string, DateTime>();

        public LampService(DataService dataService)
        {
            this._DataService = dataService;
        }

        public override bool Config()
        {
            base.Config();           

            this.RefreshTimes();
            return true;
        }

        public override void RefreshOrgAndSpanData()
        {
            lock (this)
            {
                base.RefreshOrgAndSpanData();
                this._LampStatusList.Clear();

                foreach (Workline line in this._Worklines)
                {
                    this._LampStatusList.Add(line.LineCode, 0);
                }
            }
        }

        public void RefreshTimes()
        {
            lock (this)
            {
                this._FirstTimeList.Clear();
                DateTime currentTime = DateTime.Now;
                foreach (string lineCode in this._LineSpans.Keys)
                {
                    List<WorkshiftSpan> spanList = this._LineSpans[lineCode];
                    string strTimeBegin = spanList.OrderBy(x => x.Seq).Select(x => x.TimeBegin).First();
                    DateTime timeBegin = DateTime.Parse(this._Yestoday.Date.ToString("yyyy/MM/dd") + " " + strTimeBegin).AddHours(1);  // 上班的第一个小时

                    this._FirstTimeList.Add(lineCode, timeBegin);
                    this._LowRateTimeList.Add(lineCode, new DateTime(9999, 12, 31));

                    this._LastLightTimeList.Add(lineCode,currentTime);
                }
            }
        }

        protected override void DoInternalThreadProc()
        {
            if (DateTime.Now.Day != this._Yestoday.Day)
            {
                this._Yestoday = DateTime.Now;
                this.RefreshTimes();
            }

            lock (this)
            {
                foreach (Workline line in this._Worklines)
                {
                    string lineNo = line.LineCode;
                    KanbanLineData lineData = this._DataService.GetLineData(lineNo);
                    if (lineData != null)
                    {
                        this.Light(lineData, line);
                    }
                }
            }
        }

        private void Light(KanbanLineData lineData, Workline line)
        {
            /*
            亮绿灯条件： 
                1. 每天上班第一个小时
                2. 生产率达标
                3. 初始化启动

            亮红灯的条件
                1. 不是每天上班的第一个小时
                2. 连续15分钟生产率不达标                
            */

            string lineNo = lineData.line_code;
            if (this._LampStatusList[lineNo] == 0)
            {
                this.DoLight(line, LAMP_GREEN);
                return;
            }

            DateTime firstTime = this._FirstTimeList[lineNo];

            DateTime currentTime = DateTime.Now;
            int targetLamp = this._LampStatusList[lineNo];
            if (currentTime < firstTime)   //如果是还没有上班或者上班的第一个小时内，亮绿灯
            {
                targetLamp = LAMP_GREEN;
            }
            if (lineData.is_break)    //中途休息时间不更改等的状态
            {
                return;
            }

            if (lineData.line_detail_data == null)
            {
                return;
            }
            Detail currentItem = lineData.line_detail_data.Where(x => x.is_current_item).Single();
            if (currentItem.qty_good >= currentItem.qty_plan)   //生产率达标，亮绿灯
            {
                targetLamp = LAMP_GREEN;
                this._LowRateTimeList[lineNo] = new DateTime(9999, 12, 31);  //生产率不达标的时间清零
            }
            else
            {
                DateTime lastCheckTime = this._LowRateTimeList[lineNo];
                if (currentTime < lastCheckTime)
                {
                    this._LowRateTimeList[lineNo] = currentTime;
                    lastCheckTime = this._LowRateTimeList[lineNo];
                }

                TimeSpan span = currentTime.Subtract(lastCheckTime);
               // if (span.TotalMinutes >= 1)   //连续15分钟检查亮绿灯
                {
                    targetLamp = LAMP_RED;
                }
            }

            if (this._LampStatusList[lineNo] != targetLamp || currentTime.Subtract(this._LastLightTimeList[lineNo]).TotalMinutes>=3)
            {
                this.DoLight(line, targetLamp);

                this._LastLightTimeList[lineNo] = currentTime;
            }
        }

        private void DoLight(Workline line, int curLamp)
        {
            this._LampStatusList[line.LineCode] = curLamp;
            var gidParam = new SqlParameter("GID", line.GID);
            var didParam = new SqlParameter("DID", line.DID);
            var lampParam = new SqlParameter("Lamp", curLamp);
            this._DbContext.Database.ExecuteSqlCommand("MES_Light @GID,@DID,@Lamp", gidParam, didParam, lampParam);
        }

        private const int LAMP_GREEN = 3;
        private const int LAMP_YELLOW = 2;
        private const int LAMP_RED = 1;
    }
}