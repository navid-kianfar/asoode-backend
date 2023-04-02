using System.Diagnostics;
using System.Text;
using Asoode.Application.Business.Extensions;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.Logging
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
    }
}