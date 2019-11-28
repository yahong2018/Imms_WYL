using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Imms.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imms.Data.Domain
{
    public partial class Org : Entity<long>
    {
        public string OrgCode { get; set; }
        public string OrgName { get; set; }

        public long ParentId { get; set; }

        public virtual List<Org> Children { get; set; } = new List<Org>();
        public virtual Org Parent { get; set; }
    }

    public class OrgConfigure : EntityConfigure<Org>
    {
        protected override void InternalConfigure(EntityTypeBuilder<Org> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("mes_org");
            ImmsDbContext.RegisterEntityTable<Org>("mes_org");

            builder.Property(e => e.OrgCode).HasColumnName("org_code");
            builder.Property(e => e.OrgName).HasColumnName("org_name");
            builder.Property(e => e.ParentId).HasColumnName("parent_id");

            builder.HasMany(e => e.Children).WithOne(e => e.Parent).HasForeignKey(e => e.ParentId).HasConstraintName("parent_id");
        }
    }
}
