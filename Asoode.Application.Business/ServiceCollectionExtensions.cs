using Asoode.Application.Business.Collaboration;
using Asoode.Application.Business.Communication;
using Asoode.Application.Business.General;
using Asoode.Application.Business.Import;
using Asoode.Application.Business.Logging;
using Asoode.Application.Business.Membership;
using Asoode.Application.Business.ProjectManagement;
using Asoode.Application.Business.Reports;
using Asoode.Application.Business.Storage;
using Asoode.Application.Business.TimeSpent;
using Asoode.Application.Core.Contracts.Collaboration;
using Asoode.Application.Core.Contracts.Communication;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Import;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Contracts.Membership;
using Asoode.Application.Core.Contracts.ProjectManagement;
using Asoode.Application.Core.Contracts.Reports;
using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.Contracts.TimeSpent;
using Asoode.Application.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupApplicationBusiness(
            this IServiceCollection services)
        {
            
            services.AddSingleton<IValidateBiz, ValidateBiz>();
            services.AddSingleton<IGeneralBiz, GeneralBiz>();
            services.AddTransient<IUploadProvider, S3UploadProvider>();
            services.AddTransient<ITrelloBiz, TrelloBiz>();
            services.AddTransient<ITaskBiz, TaskBiz>();
            services.AddTransient<IWorkPackageBiz, WorkPackageBiz>();
            services.AddTransient<IStorageBiz, StorageBiz>();
            services.AddTransient<IMessengerBiz, MessengerBiz>();
            services.AddTransient<IActivityBiz, ActivityBiz>();
            services.AddTransient<IErrorBiz, ErrorBiz>();
            services.AddTransient<IAccountBiz, AccountBiz>();
            services.AddTransient<IReportBiz, ReportBiz>();
            services.AddTransient<ITimeSpentBiz, TimeSpentBiz>();
            services.AddTransient<ISearchBiz, SearchBiz>();
            services.AddTransient<ICalendarBiz, CalendarBiz>();
            services.AddTransient<IDataBiz, DataBiz>();
            services.AddTransient<INotificationBiz, NotificationBiz>();
            services.AddTransient<IProjectBiz, ProjectBiz>();
            services.AddTransient<IGroupBiz, GroupBiz>();
            return services;
        }
    }
}