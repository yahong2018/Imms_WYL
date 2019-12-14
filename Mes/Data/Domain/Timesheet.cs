using System.Collections.Generic;
using Imms.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imms.Mes.Data.Domain
{
    public class Workshift : Entity<long>
    {
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public int ShiftStatus { get; set; }

        public virtual List<WorkshiftSpan> Spans { get; set; } = new List<WorkshiftSpan>();
    }

    public class WorkshiftSpan : Entity<long>
    {
        public long WorkshiftId { get; set; }
        public int Seq { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }
        public int IsBreak { get; set; }
        public int isShowOnKanban { get; set; }

        public virtual Workshift Shift { get; set; }
    }

    public class WorkshiftConfigure : EntityConfigure<Workshift>
    {
        protected override void InternalConfigure(EntityTypeBuilder<Workshift> builder)
        {
            base.InternalConfigure(builder);

            builder.ToTable("mes_workshift");
            ImmsDbContext.RegisterEntityTable<Workshift>("mes_workshift");

            builder.Property(e => e.ShiftCode).HasColumnName("shift_code");
            builder.Property(e => e.ShiftName).HasColumnName("shift_name");
            builder.Property(e => e.ShiftStatus).HasColumnName("shift_status");
        }
    }

    public class WorkshiftSpanConfigure : EntityConfigure<WorkshiftSpan>
    {
        protected override void InternalConfigure(EntityTypeBuilder<WorkshiftSpan> builder)
        {
            base.InternalConfigure(builder);

            builder.ToTable("mes_workshift_span");
            ImmsDbContext.RegisterEntityTable<WorkshiftSpan>("mes_workshift_span");

            builder.Property(e => e.WorkshiftId).HasColumnName("workshift_id");
            builder.Property(e => e.Seq).HasColumnName("seq");
            builder.Property(e => e.TimeBegin).HasColumnName("time_begin");
            builder.Property(e => e.TimeEnd).HasColumnName("time_end");
            builder.Property(e => e.IsBreak).HasColumnName("is_break");
            builder.Property(e=>e.isShowOnKanban).HasColumnName("is_show_on_kanban");

            builder.HasOne(e => e.Shift).WithMany(e => e.Spans).HasForeignKey(e => e.WorkshiftId).HasConstraintName("workshift_id");
        }
    }
}