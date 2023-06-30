using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Business.Implementation;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business;

public static class Startup
{
    public static IServiceCollection RegisterApplicationBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();
        services.AddSingleton<IGeneralService, IGeneralService>();
        
        services.AddScoped<IStorageManager, StorageManager>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IMessengerService, MessengerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITimeSpentService, TimeSpentService>();
        services.AddScoped<IWorkPackageService, WorkPackageService>();
        return services;
    }
}