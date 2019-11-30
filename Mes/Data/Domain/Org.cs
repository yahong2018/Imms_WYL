using Imms.Data;
using Imms.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imms.Mes.Data.Domain
{
    public class Workshop : Imms.Data.Domain.Org
    {
        public string WorkshopCode { get { return base.OrgCode; } set { base.OrgCode = value; } }
        public string WorkshopName { get { return base.OrgName; } set { base.OrgName = value; } }
    }

    public class Workline : Imms.Data.Domain.Org
    {
        public string LineCode { get { return base.OrgCode; } set { base.OrgCode = value; } }
        public string LineName { get { return base.OrgName; } set { base.OrgName = value; } }

        public int GID { get; set; }
        public int DID { get; set; }
    }

    public class Workstation : Imms.Data.Domain.Org
    {
        public string WorkStaitonCode { get { return base.OrgCode; } set { base.OrgCode = value; } }
        public string WorkStationName { get { return base.OrgName; } set { base.OrgName = value; } }

        public int GID { get; set; }
        public int DID { get; set; }
        public int Seq { get; set; }
        public int WorkstationType { get; set; }
    }

    public partial class Operator : Entity<long>
    {
        public string orgCode { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string Title { get; set; }
        public string Pic { get; set; }

        public static readonly string ROLE_WORKSHOP_OPERATOR = "WORKSHOP_OPERATOR";
    }


    public class OperatorConfigure : EntityConfigure<Operator>
    {
        protected override void InternalConfigure(EntityTypeBuilder<Operator> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("mes_operator");
            ImmsDbContext.RegisterEntityTable<Operator>("mes_operator");

            builder.Property(e => e.orgCode).HasColumnName("org_code");
            builder.Property(e => e.EmpId).HasColumnName("emp_id");
            builder.Property(e => e.EmpName).HasColumnName("emp_name");
            builder.Property(e => e.Title).HasColumnName("title");
            builder.Property(e => e.Pic).HasColumnName("pic");
        }
    }

    public class WorkshopConfigure : IEntityTypeConfiguration<Workshop>
    {
        public void Configure(EntityTypeBuilder<Workshop> builder)
        {
            ImmsDbContext.RegisterEntityTable<Workshop>("mes_org");
            builder.Ignore(e => e.WorkshopCode);
            builder.Ignore(e => e.WorkshopName);
        }
    }

    public class WorklineConfigure : IEntityTypeConfiguration<Workline>
    {
        public void Configure(EntityTypeBuilder<Workline> builder)
        {
            ImmsDbContext.RegisterEntityTable<Workline>("mes_org");
            builder.Ignore(e => e.LineCode);
            builder.Ignore(e => e.LineName);

            builder.Property(e => e.GID).HasColumnName("gid");
            builder.Property(e => e.DID).HasColumnName("did");            
        }
    }

    public class WorkstationConfigure : IEntityTypeConfiguration<Workstation>
    {
        public void Configure(EntityTypeBuilder<Workstation> builder)
        {
            ImmsDbContext.RegisterEntityTable<Workstation>("mes_org");

            builder.Ignore(e => e.WorkStaitonCode);
            builder.Ignore(e => e.WorkStationName);

            builder.Property(e => e.GID).HasColumnName("gid");
            builder.Property(e => e.DID).HasColumnName("did");
            builder.Property(e => e.Seq).HasColumnName("seq");
            builder.Property(e => e.WorkstationType).HasColumnName("workstation_type");
        }
    }

    public class OrgConfigure : IEntityTypeConfiguration<Imms.Data.Domain.Org>
    {
        public void Configure(EntityTypeBuilder<Imms.Data.Domain.Org> builder)
        {
            builder.HasDiscriminator("org_type", typeof(string))
               .HasValue<Workstation>(GlobalConstants.TYPE_ORG_WORK_STATETION)
               .HasValue<Workline>(GlobalConstants.TYPE_ORG_WORK_LINE)
               .HasValue<Workshop>(GlobalConstants.TYPE_ORG_WORK_SHOP)
               ;
        }
    }
}
