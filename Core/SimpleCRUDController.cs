using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Imms.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Imms
{
    [Authorize]
    public abstract class SimpleCRUDController<T> : Controller where T : class, IEntity
    {
        public SimpleCRUDLogic<T> Logic { get; set; }
        public GetDataSourceDelegate<T> DataSourceGetHandler { get; protected set; }
        public FilterDataSourceDelegate<T> DataSourceFilterHandler { get; protected set; }

        [Route("create"), HttpPost]
        public T Create(T item)
        {
            this.Verify(item, GlobalConstants.DML_OPERATION_INSERT);
            return Logic.Create(item);
        }

        [Route("update"), HttpPost]
        public T Update(T item)
        {
            this.Verify(item, GlobalConstants.DML_OPERATION_UPDATE);
            return Logic.Update(item);
        }

        [Route("delete"), HttpPost]
        public int Delete([FromBody]long[] ids)
        {
            return Logic.Delete(ids);
        }

        [Route("getAll"), HttpGet]
        public ExtJsResult GetAll()
        {
            int page, start, limit;
            string filterStr;
            GetQueryParameters(out page, out start, out limit, out filterStr);
            ExtJsResult result = Logic.GetAllWithExtResult(page, start, limit, filterStr, this.DataSourceGetHandler, this.DataSourceFilterHandler);

            return result;
        }

        protected void GetQueryParameters(out int page, out int start, out int limit, out string filterStr)
        {
            page = -1;
            start = -1;
            limit = -1;
            IQueryCollection query = this.HttpContext.Request.Query;
            if (query.ContainsKey("page"))
            {
                int.TryParse(query["page"][0], out page);
            }
            if (query.ContainsKey("start"))
            {
                int.TryParse(query["start"][0], out start);
            }
            if (query.ContainsKey("limit"))
            {
                int.TryParse(query["limit"][0], out limit);
            }

            filterStr = this.GetFilterString();
        }

        protected virtual void Verify(T item, int operation) { }

        protected virtual string GetFilterString()
        {
            IQueryCollection query = this.HttpContext.Request.Query;
            string filterStr = "";
            if (query.ContainsKey("filterExpr"))
            {
                filterStr = query["filterExpr"][0];
                byte[] bytes = Convert.FromBase64String(filterStr);
                filterStr = Encoding.UTF8.GetString(bytes);
            }
            return filterStr;
        }
    }
}