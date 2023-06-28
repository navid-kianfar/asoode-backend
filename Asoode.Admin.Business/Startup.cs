using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Business.Implementation;
using Asoode.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Admin.Business;

public static class Startup
{
    public static IServiceCollection RegisterAdminBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IErrorService, ErrorService>();
        services.AddScoped<IMarketerService, MarketerService>();
        services.AddScoped<ITransactionService, TransactionService>();
        
        return services;
    }
}