using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;

namespace Asoode.Business.Extensions
{
    public static class DbHelper
    {
        public static async Task<OperationResult<GridResult<TX>>> GetPaginatedData<T, TX>(
            IQueryable<T> query, Func<Tuple<T[], int>, Task<TX[]>> handler,
            int pageNo = -1, int pageSize = 50)
        {
            var result = new GridResult<TX>
                {Page = pageNo, PageSize = pageSize, TotalItems = await query.CountAsync()};
            result.TotalPages = result.TotalItems / pageSize + (result.TotalItems % pageSize == 0 ? 0 : 1);
            var skipped = (pageNo - 1) * pageSize;
            if (pageNo != -1) query = query.Skip(skipped).Take(pageSize);
            var list = await query.ToArrayAsync();
            var tupleParams = new Tuple<T[], int>(list, skipped);
            result.Items = await handler(tupleParams);
            return OperationResult<GridResult<TX>>.Success(result);
        }

        public static async Task<OperationResult<GridResult<TX>>> GetPaginatedData<T, TX>(
            IQueryable<T> query, Func<Tuple<T[], int>, TX[]> handler, int pageNo = -1, int pageSize = 50)
        {
            if (pageNo <= 0) pageNo = 1;
            var result = new GridResult<TX>
                {Page = pageNo, PageSize = pageSize, TotalItems = await query.CountAsync()};
            result.TotalPages = result.TotalItems / pageSize + (result.TotalItems % pageSize == 0 ? 0 : 1);
            var skipped = (pageNo - 1) * pageSize;
            if (pageNo != -1) query = query.Skip(skipped).Take(pageSize);
            var list = await query.ToArrayAsync();
            var tupleParams = new Tuple<T[], int>(list, skipped);
            result.Items = handler(tupleParams);
            return OperationResult<GridResult<TX>>.Success(result);
        }

        public static async Task<OperationResult<GridResult<T>>> GetPaginatedData<T>(
            IQueryable<T> query, int pageNo = -1, int pageSize = 50)
        {
            var result = new GridResult<T>
                {Page = pageNo, PageSize = pageSize, TotalItems = await query.CountAsync()};
            result.TotalPages = result.TotalItems / pageSize + (result.TotalItems % pageSize == 0 ? 0 : 1);
            var skipped = (pageNo - 1) * pageSize;
            if (pageNo != -1) query = query.Skip(skipped).Take(pageSize);
            result.Items = await query.ToArrayAsync();
            return OperationResult<GridResult<T>>.Success(result);
        }

        public static OperationResult<GridResult<T>> EmptyResult<T>()
        {
            return OperationResult<GridResult<T>>.Success(new GridResult<T>
            {
                Items = new T[0],
                Page = 1,
                PageSize = 10,
                TotalItems = 0,
                TotalPages = 1
            });
        }
    }
}