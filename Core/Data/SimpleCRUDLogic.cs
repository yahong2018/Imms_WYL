using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using Imms.Security.Data.Domain;

namespace Imms.Data
{
    public class SimpleCRUDLogic<T> where T : class, IEntity
    {
        public T Create(T item)
        {
            SystemUser user = GlobalConstants.GetCurrentUser();
            string json = item.ToJson();
            GlobalConstants.DefaultLogger.Info(user.UserCode + "正在创建数据:");
            GlobalConstants.DefaultLogger.Info("\n" + json);

            CommonRepository.UseDbContextWithTransaction(dbContext =>
            {
                this.Create(item, dbContext);
            });

            GlobalConstants.DefaultLogger.Info("数据创建完毕");

            return item;
        }

        public T Update(T item)
        {
            SystemUser user = GlobalConstants.GetCurrentUser();
            string json = item.ToJson();
            GlobalConstants.DefaultLogger.Info(user.UserCode + "正在更新数据:");
            GlobalConstants.DefaultLogger.Info("\n" + json);

            CommonRepository.UseDbContextWithTransaction(dbContext =>
            {
                this.Update(item, dbContext);
            });
            GlobalConstants.DefaultLogger.Info("数据更新完毕");

            return item;
        }

        public int Delete(long[] ids)
        {
            SystemUser user = GlobalConstants.GetCurrentUser();
            string json = ids.ToJson();
            GlobalConstants.DefaultLogger.Info(user.UserCode + "正在删除数据:");
            GlobalConstants.DefaultLogger.Info("\n" + json);
            CommonRepository.UseDbContextWithTransaction(dbContext =>
            {
                Delete(ids, dbContext);
            });
            GlobalConstants.DefaultLogger.Info(user.UserCode + "删除数据完毕");
            return ids.Length;
        }

        public void Create(T item, DbContext dbContext)
        {
            VerifierFactory.Verify(dbContext, item, GlobalConstants.DML_OPERATION_INSERT);
            this.BeforeInsert(item, dbContext);

            dbContext.Set<T>().Add(item);
            dbContext.SaveChanges();

            this.AfterInsert(item, dbContext);
        }

        public void Update(T item, DbContext dbContext)
        {
            VerifierFactory.Verify(dbContext, item, GlobalConstants.DML_OPERATION_UPDATE);
            this.BeforeUpdate(item, dbContext);

            GlobalConstants.ModifyEntityStatus(item, dbContext);
            dbContext.SaveChanges();

            this.AfterUpdate(item, dbContext);
        }

        public void Delete(long[] ids, DbContext dbContext)
        {
            var items = dbContext.Set<T>().Where(x => ids.Contains((long)x.RecordId)).ToList();
            this.BeforeDelete(items, dbContext);

            foreach (var item in items)
            {
                dbContext.Set<T>().Remove(item);
            }
            dbContext.SaveChanges();

            this.AfterDelete(items, dbContext);
        }

        public T GetByOne(FilterExpression[] filterList, GetDataSourceDelegate<T> getDataSourceHandler = null, FilterDataSourceDelegate<T> filterHandler = null)
        {
            GetDataSourceDelegate<T> getDataSource = getDataSourceHandler;
            if (getDataSource == null)
            {
                getDataSource = this.DefaultDataSourceGetHandler;
            }
            FilterDataSourceDelegate<T> filter = filterHandler;
            if (filter == null)
            {
                filter = this.DefaultDataSourceFilter;
            }

            T result = null;
            CommonRepository.UseDbContext(dbContext =>
            {
                IQueryable<T> dataSource = filter(getDataSource(dbContext), filterList);
                result = dataSource.FirstOrDefault();
            });
            return result;
        }


        public ExtJsResult GetAllWithExtResult(QueryParameter queryParameter, GetDataSourceDelegate<T> dataSourceGetHandler = null, FilterDataSourceDelegate<T> filterHandler = null)
        {
            FilterExpression[] filterList = null;
            if (!string.IsNullOrEmpty(queryParameter.FilterStr))
            {
                filterList = queryParameter.FilterStr.ToObject<FilterExpression[]>();
            }
            return this.GetAllWithExtResult(queryParameter, filterList, dataSourceGetHandler, filterHandler);
        }

        public ExtJsResult GetAllWithExtResult(QueryParameter queryParameter, FilterExpression[] filterList, GetDataSourceDelegate<T> dataSourceGetHandler = null, FilterDataSourceDelegate<T> filterHandler = null)
        {
            GetDataSourceDelegate<T> getDataSource = dataSourceGetHandler;
            if (getDataSource == null)
            {
                getDataSource = this.DefaultDataSourceGetHandler;
            }
            FilterDataSourceDelegate<T> filter = filterHandler;
            if (filter == null)
            {
                filter = this.DefaultDataSourceFilter;
            }

            ExtJsResult result = new ExtJsResult();
            CommonRepository.UseDbContext(dbContext =>
            {
                IQueryable<T> dataSource = filter(getDataSource(dbContext), filterList);
                IQueryable<T> countSource = filter(getDataSource(dbContext), filterList);
                if (!string.IsNullOrEmpty(queryParameter.SortExpr))
                {
                    dataSource = dataSource.OrderBy(queryParameter.SortExpr);
                }
                if (queryParameter.Start > 0)
                {
                    dataSource = dataSource.Skip(queryParameter.Start);
                }
                if (queryParameter.Limit > 0)
                {
                    dataSource = dataSource.Take(queryParameter.Limit);
                }

                try
                {
                    List<T> list = dataSource.ToList();
                    int count = this.DefaultDataSourceFilter(this.DefaultDataSourceGetHandler(dbContext), filterList).Count();
                    // if (filterList != null && filterList.Length > 0)
                    // {
                    //     count = this.FilterDataSource(this.GetDataSource(dbContext), filterList).Count();
                    // }
                    // else
                    // {
                    //     count = list.Count;
                    // }

                    result.RootProperty = list;
                    result.total = count;
                }
                catch (Exception e)
                {
                    GlobalConstants.DefaultLogger.Error(e.Message);
                }
            });
            return result;
        }

        public IQueryable<T> DefaultDataSourceGetHandler(DbContext dbContext)
        {
            return dbContext.Set<T>();
        }

        public IQueryable<T> DefaultDataSourceFilter(IQueryable<T> query, FilterExpression[] expressions)
        {
            if (expressions != null)
            {
                string whereString = this.BuildWhereString(expressions);
                string[] values = expressions.Select(x => x.R).ToArray();

                return query.Where(whereString, values);
            }
            return query;
        }

        private string BuildWhereString(FilterExpression[] filterList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (FilterExpression filter in filterList)
            {
                stringBuilder.Append(filter.ToWhereString());
            }
            return stringBuilder.ToString();
        }

        protected virtual void BeforeInsert(T item, DbContext dbContext) { }
        protected virtual void AfterInsert(T item, DbContext dbContext) { }

        protected virtual void BeforeUpdate(T item, DbContext dbContext) { }
        protected virtual void AfterUpdate(T item, DbContext dbContext) { }

        protected virtual void BeforeDelete(List<T> items, DbContext dbContext) { }
        protected virtual void AfterDelete(List<T> items, DbContext dbContext) { }

        protected virtual List<T> DoGetData(string sql, DbContext dbContext)
        {
            return dbContext.Set<T>().FromSql(sql).ToList();
        }
    }

    public class FilterExpression
    {
        public string L { get; set; }
        public string O { get; set; }
        public string R { get; set; }
        public string J { get; set; }

        public string ToWhereString()
        {
            if ("like" == this.O)
            {
                return $" {this.J} ({this.L}.contains(@0))";
            }

            return $" {this.J} ({this.L} {this.O} @0)";
        }
    }

    public class QueryParameter
    {
        public int Page { get; set; }
        public int Start { get; set; }
        public int Limit { get; set; }
        public string FilterStr { get; set; }
        public string SortExpr { get; set; }
    }

    public delegate IQueryable<T> GetDataSourceDelegate<T>(DbContext dbContext) where T : class, IEntity;
    public delegate IQueryable<T> FilterDataSourceDelegate<T>(IQueryable<T> query, FilterExpression[] expressions) where T : class, IEntity;
}