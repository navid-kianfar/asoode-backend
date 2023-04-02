using Asoode.Core.Contracts.Logging;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Business.Extensions;
using Asoode.Core.Contracts.General;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Data.Models.Base;
using Z.EntityFramework.Plus;

namespace Asoode.Business.Logging
{
    internal class ErrorBiz : IErrorBiz
    {
        private const string Dash = "---------------------------------------------------";
        private readonly IServiceProvider _serviceProvider;
        private readonly IJsonBiz _jsonBiz;

        public ErrorBiz(IServiceProvider serviceProvider, IJsonBiz jsonBiz)
        {
            _serviceProvider = serviceProvider;
            _jsonBiz = jsonBiz;
        }

        public string ExtractError(Exception ex, ErrorLogPayload payload = null)
        {
            var builder = new StringBuilder();
            builder.Append(ex);
            builder.Append("\r\n");
            builder.Append("\r\n");
            builder.Append(ex.StackTrace);
            builder.Append(Dash);
            if (ex.InnerException != null)
            {
                builder.Append(ex.InnerException);
                builder.Append("\r\n");
                builder.Append("\r\n");
                builder.Append(ex.StackTrace);
            }
            if (payload != null)
            {
                builder.Append("\r\n");
                builder.Append("\r\n");
                _jsonBiz.Serialize(payload);
            }

            return builder.ToString();
        }

        public async Task LogException(Exception ex, ErrorLogPayload payload = null)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<LoggerDbContext>())
                {
                    await unit.ErrorLogs.AddAsync(new ErrorLog
                    {
                        Description = ex.Message,
                        ErrorBody = ExtractError(ex, payload)
                    });
                    await unit.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        public async Task<OperationResult<GridResult<ErrorViewModel>>> AdminErrorsList(Guid userId, GridFilter model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<LoggerDbContext>())
                {
                    var query = unit.ErrorLogs.OrderByDescending(i => i.CreatedAt);
                    return await DbHelper.GetPaginatedData(query, tuple =>
                    {
                        var (items, startIndex) = tuple;
                        return items.Select((i, index) =>
                        {
                            var vm = i.ToViewModel();
                            vm.Index = startIndex + index + 1;
                            return vm;
                        }).ToArray();
                    }, model.Page, model.PageSize);
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                return OperationResult<GridResult<ErrorViewModel>>.Failed();
            }
        }

        public async Task<OperationResult<bool>> AdminErrorsDelete(Guid userId, Guid errorId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<LoggerDbContext>())
                {
                    await unit.ErrorLogs.Where(i => i.Id == errorId).DeleteAsync();
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }
    }
}