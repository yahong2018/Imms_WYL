using System;
using System.Collections.Generic;
using Imms.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imms.Security.Data.Domain
{
    public partial class SystemUser : Entity<long>
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public byte UserStatus { get; set; }
        public string Email { get; set; }
        // public string PhoneNumber { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastLoginTime { get; set; }

        public virtual List<RoleUser> Roles { get; set; } = new List<RoleUser>();


        public const byte USER_STATUS_DISABLED = 1;
        public const byte USER_STATUS_ENABLED = 0;
    }

    public partial class SystemRole : Entity<long>
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }

        public virtual List<RoleUser> Users { get; set; } = new List<RoleUser>();
        public virtual List<RolePrivilege> Privileges { get; set; } = new List<RolePrivilege>();
    }

    public class BaseProgram : Entity<string>
    {
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public string Url { get; set; }
        public string Glyph { get; set; }
        public int ShowOrder { get; set; }
        public string Parameters { get; set; }
        public string ParentId { get; set; }
        public int ProgramStatus{get;set;}

        public virtual void Assign(BaseProgram other)
        {
            this.RecordId = other.RecordId;
            this.ProgramCode = other.ProgramCode;
            this.ProgramName = other.ProgramName;
            this.Url = other.Url;
            this.Glyph = other.Glyph;
            this.ShowOrder = other.ShowOrder;
            this.Parameters = other.Parameters;
            this.ParentId = other.ParentId;
        }
    }

    public partial class SystemProgram : BaseProgram
    {
        public virtual List<ProgramPrivilege> Privielges { get; set; } = new List<ProgramPrivilege>();

        public virtual List<SystemProgram> Children { get; set; } = new List<SystemProgram>();
        public virtual SystemProgram Parent { get; set; }
    }

    public partial class RoleUser : Entity<long>
    {
        public long RoleId { get; set; }
        public long UserId { get; set; }

        public virtual SystemRole Role { get; set; }
        public virtual SystemUser User { get; set; }
    }

    public partial class RolePrivilege : Entity<long>
    {
        public long ProgramPrivilegeId { get; set; }
        public long RoleId { get; set; }
        public string ProgramId { get; set; }
        public string PrivilegeCode { get; set; }

        public virtual ProgramPrivilege PorgramPrivielge { get; set; }
        public virtual SystemRole Role { get; set; }
    }

    public partial class ProgramPrivilege : Entity<long>
    {
        public string ProgramId { get; set; }
        public string PrivilegeCode { get; set; }
        public string PrivilegeName { get; set; }

        public virtual SystemProgram Program { get; set; }

        public const string PRIVILEGE_RUN = "RUN";
        public const string PRIVILEGE_INSERT = "INSERT";
        public const string PRIVILEGE_UPDATE = "UPDATE";
        public const string PRIVILEGE_DELETE = "DELETE";
    }

    public class ProgramPrivilegeConfigure : EntityConfigure<ProgramPrivilege>
    {
        protected override void InternalConfigure(EntityTypeBuilder<ProgramPrivilege> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("program_privilege");
            ImmsDbContext.RegisterEntityTable<ProgramPrivilege>("program_privilege");

            builder.Property(e => e.PrivilegeCode).IsRequired().HasColumnName("privilege_code").HasMaxLength(50).IsUnicode(false);
            builder.Property(e => e.PrivilegeName).IsRequired().HasColumnName("privilege_name").HasMaxLength(120).IsUnicode(false);
            builder.Property(e => e.ProgramId).IsRequired().HasColumnName("program_id").HasMaxLength(50).IsUnicode(false);

            builder.HasOne(e => e.Program).WithMany(e => e.Privielges).HasForeignKey(x => x.ProgramId).HasConstraintName("program_id");
        }
    }

    public class RolePrivilegeConfigure : EntityConfigure<RolePrivilege>
    {
        protected override void InternalConfigure(EntityTypeBuilder<RolePrivilege> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("role_privilege");
            ImmsDbContext.RegisterEntityTable<RolePrivilege>("role_privilege");

            builder.Property(e => e.PrivilegeCode).IsRequired().HasColumnName("privilege_code").HasMaxLength(50).IsUnicode(false);
            builder.Property(e => e.ProgramId).IsRequired().HasColumnName("program_id").HasMaxLength(50).IsUnicode(false);
            builder.Property(e => e.ProgramPrivilegeId).HasColumnName("program_privilege_id").HasColumnType("bigint(20)");
            builder.Property(e => e.RoleId).HasColumnName("role_id").HasColumnType("bigint(20)");

            builder.HasOne(e => e.Role).WithMany(e => e.Privileges).HasForeignKey(e => e.RoleId).HasConstraintName("role_id");
            builder.HasOne(e => e.PorgramPrivielge).WithMany().HasForeignKey(e => e.ProgramPrivilegeId).HasConstraintName("program_privilege_id");
        }
    }
    public class RoleUserConfigure : EntityConfigure<RoleUser>
    {
        protected override void InternalConfigure(EntityTypeBuilder<RoleUser> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("role_user");
            ImmsDbContext.RegisterEntityTable<RoleUser>("role_user");

            builder.Property(e => e.RoleId).HasColumnName("role_id").HasColumnType("bigint(20)");
            builder.Property(e => e.UserId).HasColumnName("user_id").HasColumnType("bigint(20)");

            builder.HasOne(e => e.User).WithMany(e => e.Roles).HasForeignKey(e => e.UserId).HasConstraintName("user_id");
            builder.HasOne(e => e.Role).WithMany(e => e.Users).HasForeignKey(e => e.RoleId).HasConstraintName("role_id");
        }
    }

    public class SystemProgramConfigure : EntityConfigure<SystemProgram>
    {
        protected override void InternalConfigure(EntityTypeBuilder<SystemProgram> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("system_program");
            ImmsDbContext.RegisterEntityTable<SystemProgram>("system_program");

            builder.Property(e => e.Glyph).HasColumnName("glyph").HasMaxLength(100).IsUnicode(false);
            builder.Property(e => e.Parameters).IsRequired().HasColumnName("parameters").HasMaxLength(255).IsUnicode(false);
            builder.Property(e => e.ParentId).IsRequired().HasColumnName("parent_id").HasMaxLength(50).IsUnicode(false);
            builder.Property(e => e.ProgramCode).IsRequired().HasColumnName("program_code").HasMaxLength(50).IsUnicode(false);
            builder.Property(e => e.ProgramName).IsRequired().HasColumnName("program_name").HasMaxLength(120).IsUnicode(false);
            builder.Property(e => e.ShowOrder).HasColumnName("show_order").HasColumnType("int(11)");
            builder.Property(e => e.Url).IsRequired().HasColumnName("url").HasMaxLength(255).IsUnicode(false);
            builder.Property(e=>e.ProgramStatus).HasColumnName("program_status");

            builder.HasMany(e => e.Children).WithOne(e => e.Parent).HasForeignKey(e => e.ParentId).HasConstraintName("parent_id").HasPrincipalKey(x => x.RecordId);
        }
    }

    public class SystemRoleConfigure : EntityConfigure<SystemRole>
    {
        protected override void InternalConfigure(EntityTypeBuilder<SystemRole> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("system_role");
            ImmsDbContext.RegisterEntityTable<SystemRole>("system_role");

            builder.Property(e => e.RoleCode).IsRequired().HasColumnName("role_code").HasMaxLength(20).IsUnicode(false);
            builder.Property(e => e.RoleName).IsRequired().HasColumnName("role_name").HasMaxLength(50).IsUnicode(false);
        }
    }

    public class SystemUserConfigure : EntityConfigure<SystemUser>
    {
        protected override void InternalConfigure(EntityTypeBuilder<SystemUser> builder)
        {
            base.InternalConfigure(builder);
            builder.ToTable("system_user");
            ImmsDbContext.RegisterEntityTable<SystemUser>("system_user");

            builder.Property(e => e.Email).IsRequired().HasColumnName("email").HasMaxLength(255).IsUnicode(false);
            builder.Property(e => e.IsOnline).HasColumnName("is_online").HasColumnType("bit(1)").HasDefaultValueSql("b'0'");
            builder.Property(e => e.LastLoginTime).HasColumnName("last_login_time");
            builder.Property(e => e.Pwd).IsRequired().HasColumnName("pwd").HasMaxLength(50).IsUnicode(false);
            builder.Property(e => e.UserCode).IsRequired().HasColumnName("user_code").HasMaxLength(20).IsUnicode(false);
            builder.Property(e => e.UserName).IsRequired().HasColumnName("user_name").HasMaxLength(50).IsUnicode(false);
            builder.Property(e => e.UserStatus).HasColumnName("user_status").HasColumnType("tinyint(4)");
            // builder.Property(e=>e.PhoneNumber).HasColumnName("phone_number");
        }
    }
}
