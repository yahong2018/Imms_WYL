using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imms.Data
{
    public interface IEntity : IComparable
    {
        IComparable RecordId { get; set; }
        void Clone(IEntity other, params string[] excludes);
    }

    public class Entity<T> : IEntity where T : IComparable
    {
        public T RecordId { get; set; }
        IComparable IEntity.RecordId { get => this.RecordId; set => this.RecordId = (T)value; }

        public void Clone(IEntity other, params string[] excludes)
        {
            if (other == null)
            {
                return;
            }

            PropertyInfo[] otherProperties = GetProperties(other.GetType());
            PropertyInfo[] thisProperties = GetProperties(this.GetType());
            foreach (PropertyInfo property in thisProperties)
            {
                if (excludes.Contains(property.Name) || property.Name == "RecordId")
                {
                    continue;
                }

                PropertyInfo otherProperty = otherProperties.Where(x => x.Name == property.Name).FirstOrDefault();
                Type propertyType = property.PropertyType;
                if (otherProperty == null || (!propertyType.IsPrimitive && propertyType != typeof(string)))
                {
                    continue;
                }
                property.SetValue(this, otherProperty.GetValue(other));

                //object otherValue = otherProperty.GetValue(other);
                //if(otherValue is IEntity)
                //{
                //    object thisValue = Activator.CreateInstance(propertyType);
                //    property.SetValue(this, thisValue);
                //    (thisValue as IEntity).Clone(otherValue as IEntity);
                //}
                //else
                //{
                //    property.SetValue(this, otherValue);
                //}
            }
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (((IEntity)this).RecordId == null)
            {
                return -1;
            }

            if (!(obj is IEntity))
            {
                return -1;
            }

            if (((IEntity)obj).RecordId == null)
            {
                return -1;
            }

            return ((IComparable)this.RecordId).CompareTo(((IEntity)obj).RecordId);
        }

        public override string ToString()
        {
            PropertyInfo[] properties = GetProperties(this.GetType());
            StringBuilder builder = new StringBuilder("{");
            foreach (PropertyInfo property in properties)
            {
                Type propertyType = property.PropertyType;
                if (!propertyType.IsPrimitive && propertyType != typeof(string))
                {
                    continue;
                }

                object value = property.GetValue(this);
                if (value == null)
                {
                    value = "[null]";
                }
                builder.Append($"{property.Name}={value.ToString()};");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append("}");
            return builder.ToString();
            //string result = stringBuilder.ToString();

            //if (string.IsNullOrEmpty(GlobalConstants.DEFAULT_CHARSET) || GlobalConstants.DEFAULT_CHARSET == Encoding.UTF8.BodyName)
            //{
            //    return result;
            //}
            //return GlobalConstants.DEFAULT_ENCODING.GetString(Encoding.UTF8.GetBytes(result));
        }

        private static readonly SortedList<Guid, PropertyInfo[]> _Properties = new SortedList<Guid, PropertyInfo[]>();


        private static PropertyInfo[] GetProperties(Type type)
        {
            Guid key = type.GUID;
            lock (_Properties)
            {
                if (!_Properties.ContainsKey(key))
                {
                    _Properties.Add(key, type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                }
            }
            return _Properties[key];
        }
    }

    public interface ITrackableEntity
    {
        // long CreateById { get; set; }
        // string CreateByCode{get;set;}
        // string CreateByName{get;set;}
        // DateTime CreateTime { get; set; }
        // long? UpdateById { get; set; }
        // string UpdateByCode{get;set;}
        // string UpdateByName{get;set;}
        // DateTime? UpdateTime { get; set; }
        // int OptFlag { get; set; }
    }

    public class TrackableEntity<T> : Entity<T>, ITrackableEntity where T : IComparable
    {
        // public long CreateById { get ; set; }
        // public string CreateByCode { get ; set ; }
        // public string CreateByName { get ; set ; }
        // public DateTime CreateTime { get ; set ; }
        // public long? UpdateById { get ; set ; }
        // public string UpdateByCode { get ; set ; }
        // public string UpdateByName { get ; set ; }
        // public DateTime? UpdateTime { get ; set ; }
        // public int OptFlag { get ; set ; }
    }

    public interface IOrderEntity
    {
        string OrderNo { get; set; }
    }

    public class OrderEntity<T> : TrackableEntity<T>, IOrderEntity where T : IComparable
    {
        public string OrderNo { get; set; }
        public int OrderStatus { get; set; }

        public void PlusOrderStatus(int status)
        {
            this.OrderStatus |= status;
        }

        public bool ReachStatus(int status)
        {
            return (this.OrderStatus & status) == status;
        }
    }

    public abstract class EntityConfigure<E> : IEntityTypeConfiguration<E> where E : class
    {
        public void Configure(EntityTypeBuilder<E> builder)
        {
            this.InternalConfigure(builder);
        }

        protected virtual void InternalConfigure(EntityTypeBuilder<E> builder)
        {
            Type entityType = typeof(E);
            PropertyInfo propertyInfo = entityType.GetProperty("RecordId", BindingFlags.Public | BindingFlags.Instance);
            builder.HasKey("RecordId");
            builder.Property(propertyInfo.PropertyType, "RecordId").HasColumnName("record_id");
        }
    }

    public abstract class TrackableEntityConfigure<E> : EntityConfigure<E> where E : class
    {
        protected override void InternalConfigure(EntityTypeBuilder<E> builder)
        {
            base.InternalConfigure(builder);

            // builder.Property(typeof(long), "CreateById").HasColumnName("create_by_id");
            // builder.Property(typeof(string),"CreateByCode").HasColumnName("create_by_code");
            // builder.Property(typeof(string),"CreateByName").HasColumnName("create_by_name");            
            // builder.Property(typeof(DateTime), "CreateTime").HasColumnName("create_time");

            // builder.Property(typeof(long?), "UpdateById").HasColumnName("update_by_id");
            // builder.Property(typeof(string),"UpdateByCode").HasColumnName("update_by_code");
            // builder.Property(typeof(string),"UpdateByName").HasColumnName("update_by_name");
            // builder.Property(typeof(DateTime?), "UpdateTime").HasColumnName("update_time");
            
            // builder.Property(typeof(int), "OptFlag").HasColumnName("opt_flag");
        }
    }

    public abstract class OrderEntityConfigure<E> : TrackableEntityConfigure<E> where E : class
    {
        protected override void InternalConfigure(EntityTypeBuilder<E> builder)
        {
            base.InternalConfigure(builder);

            builder.Property(typeof(int), "OrderStatus").HasColumnName("order_status");
            builder.Property(typeof(string), "OrderNo").HasColumnName("order_no").HasMaxLength(12);
        }
    }
}