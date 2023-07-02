using Asoode.Business.Admin;
using Asoode.Business.Collaboration;
using Asoode.Business.Communication;
using Asoode.Business.General;
using Asoode.Business.Import;
using Asoode.Business.Logging;
using Asoode.Business.Membership;
using Asoode.Business.ProjectManagement;
using Asoode.Business.Reports;
using Asoode.Business.Storage;
using Asoode.Business.TimeSpent;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Contracts.Collaboration;
using Asoode.Core.Contracts.Communication;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Import;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Contracts.ProjectManagement;
using Asoode.Core.Contracts.Reports;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Contracts.TimeSpent;
using Asoode.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupApplicationBusiness(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILoggerService, ConsoleLogger>();
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<IQueueBiz, QueueBiz>();
        services.AddSingleton<ITranslateBiz, TranslateBiz>();
        services.AddSingleton<IValidateBiz, ValidateBiz>();
        services.AddSingleton<IGeneralBiz, GeneralBiz>();
        services.AddSingleton<IPdfBiz, PdfBiz>();
        services.AddTransient<IMarketerBiz, MarketerBiz>();
        services.AddTransient<IDiscountBiz, DiscountBiz>();
        services.AddTransient<IPaymentBiz, PaymentBiz>();
        services.AddTransient<IOrderBiz, OrderBiz>();
        services.AddTransient<ITrelloBiz, TrelloBiz>();
        services.AddTransient<ITaskuluBiz, TaskuluBiz>();
        services.AddTransient<ITransactionBiz, TransactionBiz>();
        services.AddTransient<IBlogBiz, BlogBiz>();
        services.AddTransient<ITaskBiz, TaskBiz>();
        services.AddTransient<IWorkPackageBiz, WorkPackageBiz>();
        services.AddTransient<IEmailBiz, EmailBiz>();
        services.AddTransient<ISmsBiz, SmsBiz>();
        services.AddTransient<IMessengerBiz, MessengerBiz>();
        services.AddTransient<IActivityBiz, ActivityBiz>();
        services.AddTransient<IErrorBiz, ErrorBiz>();
        services.AddTransient<IAccountBiz, AccountBiz>();
        services.AddTransient<IPlanBiz, PlanBiz>();
        services.AddTransient<IReportBiz, ReportBiz>();
        services.AddTransient<ITimeSpentBiz, TimeSpentBiz>();
        services.AddTransient<ISearchBiz, SearchBiz>();
        services.AddTransient<ICalendarBiz, CalendarBiz>();
        services.AddTransient<IDataBiz, DataBiz>();
        services.AddTransient<INotificationBiz, NotificationBiz>();
        services.AddTransient<IPostmanBiz, PostmanBiz>();
        services.AddTransient<IProjectBiz, ProjectBiz>();
        services.AddTransient<IGroupBiz, GroupBiz>();
        services.AddTransient<IContactBiz, ContactBiz>();
        services.AddScoped<IStorageBiz, StorageBiz>();
        return services;
    }
}