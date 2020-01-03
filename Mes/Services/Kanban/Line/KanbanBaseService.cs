using System.Collections.Generic;
using System.Linq;
using Imms.Mes.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace Imms.Mes.Services.Kanban.Line
{
    public class KanbanBaseService : BaseService
    {
        protected DbContext _DbContext = null;
        protected List<Imms.Mes.Data.Domain.Workshop> _Workshops = new List<Imms.Mes.Data.Domain.Workshop>();
        protected List<Workline> _Worklines = new List<Workline>();
        protected SortedList<string, List<WorkshiftSpan>> _LineSpans = new SortedList<string, List<WorkshiftSpan>>();

        public override bool Config()
        {
            base.Config();

            this.ThreadIntervals = 1000 * 1;
            this._DbContext = GlobalConstants.DbContextFactory.GetContext();
            this.RefreshOrgAndSpanData();

            return true;
        }

        public virtual void RefreshOrgAndSpanData()
        {
            lock (this)
            {
                this._LineSpans.Clear();
                this._Workshops = this._DbContext.Set<Imms.Mes.Data.Domain.Workshop>().ToList();
                this._Worklines = this._DbContext.Set<Workline>().ToList();
                foreach (Imms.Mes.Data.Domain.Workshop workshop in this._Workshops)
                {
                    long workshiftId = this._DbContext.Set<Workshift>().Where(x => x.ShiftCode == workshop.WorkshiftCode).Single().RecordId;
                    foreach (Workline line in this._Worklines)
                    {
                        this._LineSpans.Add(line.LineCode, this._DbContext.Set<WorkshiftSpan>()
                                     .Where(x => x.WorkshiftId == workshiftId)
                                     .OrderBy(x => x.Seq)
                                     .ToList());
                    }
                }
            }
        }
    }
}