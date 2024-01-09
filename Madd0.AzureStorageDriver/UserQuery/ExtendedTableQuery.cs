using Azure;
using Azure.Data.Tables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Madd0.UserQuery
{
    public class TableQuery<TElement> : IEnumerable<TElement>
        where TElement : class, new()
    {
        public TableClient TableClient { get; }

        public TableQuery(TableClient table)
        {
            this.TableClient = table;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return TableClient.Query<TableEntity>()
                .Select(x =>
                {
                    var e = new EntityAdapter<TElement>();
                    e.ReadEntity(x);
                    return e.Entity;
                })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return TableClient.Query<TableEntity>()
                .Select(x =>
                {
                    var e = new EntityAdapter<TElement>();
                    e.ReadEntity(x);
                    return e.Entity;
                })
                .GetEnumerator();
        }
    }

    public interface IEntityAdapter : ITableEntity
    {
        public void ReadEntity(TableEntity tableEntity);
    }

    public class EntityAdapter<T> : IEntityAdapter
        where T : class, new()
    {
        public T Entity { get; set; } = new T();

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public void ReadEntity(TableEntity tableEntity)
        {
            if (tableEntity == null) throw new ArgumentNullException(nameof(tableEntity), "Table entity cannot be null");
    
            Timestamp = tableEntity.Timestamp;
            PartitionKey = tableEntity.PartitionKey;
            RowKey = tableEntity.RowKey;
            ETag = tableEntity.ETag;
    
            ReadProperties(tableEntity);
        }
    
        protected virtual void ReadProperties(TableEntity tableEntity)
        {
            foreach (var property in Entity.GetType().GetProperties())
            {
                if (property.Name != nameof(ETag))
                {
                    ReadProperty(property, tableEntity);
                }
            }
        }

        protected virtual void ReadProperty(PropertyInfo property, TableEntity tableEntity)
        {
            try
            {
                property.ConvertFromTableEntityProperty(Entity, tableEntity);
            }
            catch (Exception e)
            {
                throw new NotSupportedException($"Failed to read  the: '{property}' property.", e);
            }
        }
    }

    public static class PropertyInfoExtensions
    {
        public static void ConvertFromTableEntityProperty(this PropertyInfo property, object entity, TableEntity tableEntity)
        {
            if (property == null)
                return;
            
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(entity, tableEntity.GetString(property.Name));
            }
            else if (property.PropertyType == typeof(int) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int))
            {
                property.SetValue(entity, tableEntity.GetInt32(property.Name));
            }
            else if (property.PropertyType == typeof(bool) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(bool))
            {
                property.SetValue(entity, tableEntity.GetBoolean(property.Name));
            }
            else if (property.PropertyType == typeof(DateTimeOffset) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTimeOffset))
            {
                property.SetValue(entity, tableEntity.GetDateTimeOffset(property.Name));
            }
            else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
            {
                property.SetValue(entity, tableEntity.GetDateTime(property.Name));
            }
            else if (property.PropertyType == typeof(long) | Nullable.GetUnderlyingType(property.PropertyType) == typeof(long))
            {
                property.SetValue(entity, tableEntity.GetInt64(property.Name));
            }
            else if (property.PropertyType == typeof(double) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double))
            {
                property.SetValue(entity, tableEntity.GetDouble(property.Name));
            }
            else if (property.PropertyType == typeof(Guid) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(Guid))
            {
                property.SetValue(entity, tableEntity.GetGuid(property.Name));
            }
            else
            {
                property.SetValue(entity, tableEntity[property.Name]);
            }
        }
    }
}
