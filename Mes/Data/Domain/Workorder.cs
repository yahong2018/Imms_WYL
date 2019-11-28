using System;
using System.Collections.Generic;
using Imms.Data;
using Imms.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imms.Mes.Data.Domain
{
    public partial class Workorder : OrderEntity<long>
    {
        public string LineNo { get; set; }
        public string CustomerNo { get; set; }
        public string PartNo { get; set; }

        public int QtyReq { get; set; }
        public int QtyGood { get; set; }
        public int QtyBad { get; set; }

        public DateTime TimeStartPlan { get; set; }
        public DateTime TimeEndPlan { get; set; }

        public DateTime? TimeStartActual { get; set; }
        public DateTime? TimeEndActual { get; set; }
    }

    public class WorkorderActual : Entity<long>
    {
        public string WorkorderNo { get; set; }
        public string PartNo { get; set; }
        public string WorkstationCode { get; set; }
        public int Qty { get; set; }
        public int RecordType { get; set; }
        public string DefectCode { get; set; }
        public DateTime ReportTime { get; set; }
    }

    // public class LineProductSummaryDate : Entity<long>
    // {
    //     public string LineNo { get; set; }
    //     public string WorkorderNo { get; set; }
    //     public string PartNo { get; set; }
    //     public DateTime ProductDate { get; set; }

    //     public int QtyPlanDate { get; set; }
    //     public int QtyPlan { get; set; }
    //     public int QtyGood { get; set; }
    //     public int QtyBad { get; set; }
    // }

    public class LineProductSummaryDateSpan : Entity<long>
    {
        public string LineNo { get; set; }
        public string WorkorderNo { get; set; }
        public string PartNo { get; set; }
        public DateTime ProductDate { get; set; }

        public int SpanId { get; set; }

        public int QtyPlan { get; set; }
        public int QtyGood { get; set; }
        public int QtyBad { get; set; }
    }

    public class Defect : Entity<long>
    {
        public string DefectCode { get; set; }
        public string DefectName { get; set; }
    }


    public class WorkorderConfigure : EntityConfigure<Workorder>
    {
        protected override void InternalConfigure(EntityTypeBuilder<Workorder> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("mes_workorder");
            ImmsDbContext.RegisterEntityTable<Defect>("mes_workorder");

            builder.Property(e => e.CustomerNo).HasColumnName("customer_no");
            builder.Property(e => e.LineNo).HasColumnName("line_no");
            builder.Property(e => e.PartNo).HasColumnName("part_no");
            builder.Property(e => e.QtyReq).HasColumnName("qty_req");
            builder.Property(e => e.QtyGood).HasColumnName("qty_good");
            builder.Property(e => e.QtyBad).HasColumnName("qty_bad");

            builder.Property(e => e.TimeStartPlan).HasColumnName("time_start_plan");
            builder.Property(e => e.TimeEndPlan).HasColumnName("time_end_plan");
            builder.Property(e => e.TimeStartActual).HasColumnName("time_start_actual");
            builder.Property(e => e.TimeEndPlan).HasColumnName("time_end_actual");
        }
    }

    public class WorkorderActualConfigure : EntityConfigure<WorkorderActual>
    {
        protected override void InternalConfigure(EntityTypeBuilder<WorkorderActual> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("mes_workorder_actual");
            ImmsDbContext.RegisterEntityTable<Defect>("mes_workorder_actual");

            builder.Property(e => e.WorkorderNo).HasColumnName("workorder_no");
            builder.Property(e => e.PartNo).HasColumnName("part_no");
            builder.Property(e => e.WorkstationCode).HasColumnName("workstation_code");
            builder.Property(e => e.Qty).HasColumnName("qty");
            builder.Property(e => e.RecordType).HasColumnName("record_type");
            builder.Property(e => e.DefectCode).HasColumnName("defect_code");
            builder.Property(e => e.ReportTime).HasColumnName("report_time");
        }
    }

    public class LineProductSumaryDateSpanConfigure:EntityConfigure<LineProductSummaryDateSpan>{
        protected override void InternalConfigure(EntityTypeBuilder<LineProductSummaryDateSpan> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("mes_line_product_summary_datespan");
            ImmsDbContext.RegisterEntityTable<Defect>("mes_line_product_summary_datespan");

            builder.Property(e => e.WorkorderNo).HasColumnName("workorder_no");
            builder.Property(e => e.LineNo).HasColumnName("line_no");
            builder.Property(e => e.PartNo).HasColumnName("part_no");            
            builder.Property(e => e.ProductDate).HasColumnName("product_date");

            builder.Property(e => e.SpanId).HasColumnName("span_id");

            builder.Property(e => e.QtyPlan).HasColumnName("qty_plan");
            builder.Property(e => e.QtyGood).HasColumnName("qty_good");
            builder.Property(e => e.QtyBad).HasColumnName("qty_bad");
        }
    }

    public class DefectConfigure : EntityConfigure<Defect>
    {
        protected override void InternalConfigure(EntityTypeBuilder<Defect> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("mes_defect");
            ImmsDbContext.RegisterEntityTable<Defect>("mes_defect");

            builder.Property(e => e.DefectCode).HasColumnName("defect_code");
            builder.Property(e => e.DefectName).HasColumnName("defect_name");
        }
    }
}