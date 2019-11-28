using System;
using System.Collections.Generic;
using System.Linq;
using Imms.Data.Domain;
using Imms.Security.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Imms.Data
{
    public class ImmsDbContext : DbContext
    {
        public ImmsDbContext()
        {
        }

        public ImmsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (ConfigurationManager.ConnectionString.ProviderType == ProviderType.SqlServer)
                {
                    optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionString.ConnectionUrl);
                }
                else if (ConfigurationManager.ConnectionString.ProviderType == ProviderType.MySql)
                {
                    optionsBuilder.UseMySQL(ConfigurationManager.ConnectionString.ConnectionUrl);
                }
            }

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new EFLoggerProvider());
            optionsBuilder.UseLoggerFactory(loggerFactory);

            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            var modifiedEntities = this.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified
                     || x.State == EntityState.Added
                     || x.State == EntityState.Deleted
                ).ToList();

            List<DataChangedNotifyEvent> eventList = new List<DataChangedNotifyEvent>();
            foreach (EntityEntry entry in modifiedEntities)
            {
                this.FillTracableData(entry);                

                int dmlType = this.GetDmlType(entry);
                IEntity entity = entry.Entity as IEntity;

                eventList.Add(new DataChangedNotifyEvent() { Entity = entity, DMLType = dmlType });
            }
            int result = base.SaveChanges();
            foreach (DataChangedNotifyEvent e in eventList)
            {
                DataChangedNotifier.Instance.Notify(e);
            }
            return result;
        }
        

        private int GetDmlType(EntityEntry e)
        {
            int dmlType = 0;
            if (e.State == EntityState.Added)
            {
                dmlType = GlobalConstants.DML_OPERATION_INSERT;
            }
            else if (e.State == EntityState.Deleted)
            {
                dmlType = GlobalConstants.DML_OPERATION_DELETE;
            }
            else if (e.State == EntityState.Modified)
            {
                dmlType = GlobalConstants.DML_OPERATION_UPDATE;
            }

            return dmlType;
        }

        private void FillTracableData(EntityEntry entry)
        {
            if (!(entry.Entity is ITrackableEntity))
                return;

            // ITrackableEntity trackableEntity = entry.Entity as ITrackableEntity;
            // SystemUser currentUser = GlobalConstants.GetCurrentUser();
            // if (entry.State == EntityState.Added)
            // {
            //     trackableEntity.CreateById = currentUser.RecordId;
            //     trackableEntity.CreateByCode = currentUser.UserCode;
            //     trackableEntity.CreateByName = currentUser.UserName;
            //     trackableEntity.CreateTime = DateTime.Now;                
            // }
            // else if (entry.State == EntityState.Modified)
            // {
            //     trackableEntity.UpdateById = currentUser.RecordId;
            //     trackableEntity.UpdateByCode = currentUser.UserCode;
            //     trackableEntity.UpdateByName = currentUser.UserName;
            //     trackableEntity.UpdateTime = DateTime.Now;
            // }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrgConfigure());              
            modelBuilder.ApplyConfiguration(new SystemParameterConfigure());

            foreach (ICustomModelBuilder customModelBuilder in customModelBuilders)
            {
                customModelBuilder.BuildModel(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                }
            }
        }


        public class BoolToIntConverter : ValueConverter<bool, int>
        {
            public BoolToIntConverter(ConverterMappingHints mappingHints = null)
                : base(
                      v => Convert.ToInt32(v),
                      v => Convert.ToBoolean(v),
                      mappingHints)
            {
            }

            public static ValueConverterInfo DefaultInfo { get; }
                = new ValueConverterInfo(typeof(bool), typeof(int), i => new BoolToIntConverter(i.MappingHints));
        }

        private static List<ICustomModelBuilder> customModelBuilders = new List<ICustomModelBuilder>();

        public static void RegisterModelBuilders(ICustomModelBuilder builder)
        {
            customModelBuilders.Add(builder);
        }

        private static SortedList<Guid, string> entityTables = new SortedList<Guid, string>();
        public static void RegisterEntityTable<T>(string tableName)
        {
            Guid key = typeof(T).GUID;
            if (entityTables.ContainsKey(key))
            {
                return;
            }
            entityTables.Add(key, tableName);
        }
        public static string GetEntityTableName<T>()
        {
            return entityTables[typeof(T).GUID];
        }
    }

    public interface ICustomModelBuilder
    {
        void BuildModel(ModelBuilder modelBuilder);
    }
}