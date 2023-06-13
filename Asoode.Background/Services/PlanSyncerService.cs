// using System;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Asoode.Core.Contracts.Logging;
// using Asoode.Core.Primitives.Enums;
// using Asoode.Data.Contexts;
// using Asoode.Data.Models.Junctions;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
//
// namespace Asoode.Background.Services
// {
//     public class PlanSyncerService : IHostedService, IDisposable
//     {
//         private readonly IServiceProvider _serviceProvider;
//         private Timer _timer;
//         private bool _lock;
//
//         public PlanSyncerService(IServiceProvider serviceProvider)
//         {
//             _serviceProvider = serviceProvider;
//         }
//
//         public Task StartAsync(CancellationToken cancellationToken)
//         {
//             _lock = false;
//             _timer = new Timer(
//                 async o => await DoWork(o),
//                 null,
//                 TimeSpan.FromSeconds(5),
//                 // TimeSpan.FromMinutes(1),
//                 TimeSpan.FromMinutes(1)
//             );
//             return Task.CompletedTask;
//         }
//
//         private async Task DoWork(object state)
//         {
//             if (_lock) 
//             {
//                 Console.WriteLine("PlanSyncerService : progress!!!!");
//                 return;
//             }
//             var host = _serviceProvider.GetService<IHost>();
//             using (var scope = host.Services.CreateScope())
//             {
//                 try
//                 {
//                     _lock = true;
//                     Console.WriteLine("PlanSyncerService : started");
//                     var now = DateTime.UtcNow;
//                     using (var unit = scope.ServiceProvider.GetService<ProjectManagementDbContext>())
//                     {
//                         var projectIds = await unit.ProjectMembers
//                             .Where(i => i.Access == AccessType.Owner)
//                             .Select(i => i.ProjectId)
//                             .Distinct()
//                             .ToArrayAsync();
//                         
//                         var packageIds = await unit.WorkPackageMembers
//                             .Where(i => i.Access == AccessType.Owner)
//                             .Select(p => p.PackageId)
//                             .Distinct()
//                             .ToArrayAsync();
//                         
//                         
//                         var projects = await unit.Projects.Where(p => !projectIds.Contains(p.Id)).Select(p => new
//                         {
//                             p.Id,
//                             p.UserId
//                         }).ToArrayAsync();
//                         
//                         var packages = await unit.WorkPackages.Where(p => !packageIds.Contains(p.Id)).Select(p => new
//                         {
//                             p.Id,
//                             p.ProjectId,
//                             p.UserId
//                         }).ToArrayAsync();
//
//                         foreach (var project in projects)
//                         {
//                             await unit.ProjectMembers.AddAsync(new ProjectMember
//                             {
//                                 Access = AccessType.Owner,
//                                 ProjectId = project.Id,
//                                 RecordId = project.UserId
//                             });
//                         }
//                         
//                         foreach (var pkg in packages)
//                         {
//                             await unit.WorkPackageMembers.AddAsync(new WorkPackageMember
//                             {
//                                 Access = AccessType.Owner,
//                                 ProjectId = pkg.ProjectId,
//                                 RecordId = pkg.UserId,
//                                 PackageId = pkg.Id
//                             });
//                         }
//
//                         await unit.SaveChangesAsync();
//
//                         // var planInfos = (await unit.UserPlanInfo
//                         //     .OrderByDescending(i => i.CreatedAt)
//                         //     .ToArrayAsync())
//                         //     .GroupBy(i => i.UserId)
//                         //     .Select(i => i.First())
//                         //     .ToArray();
//                         //
//                         // var groupInfos = (await unit.Groups
//                         //         .OrderByDescending(i => i.CreatedAt)
//                         //         .Select(i => new {i.PlanInfoId, i.Complex})
//                         //         .AsNoTracking()
//                         //         .ToArrayAsync())
//                         //     .GroupBy(i => i.PlanInfoId)
//                         //     .Select(i => new
//                         //     {
//                         //         Id = i.Key,
//                         //         Complex = i.Count(j => j.Complex),
//                         //         Simple = i.Count(j => !j.Complex),
//                         //     })
//                         //     .ToDictionary(k => k.Id, v => new
//                         //     {
//                         //         v.Complex,
//                         //         v.Simple
//                         //     });
//                         //
//                         // var projectsInfos = (await unit.Projects
//                         //     .OrderByDescending(i => i.CreatedAt)
//                         //     .Select(i => new { i.PlanInfoId, i.Complex })
//                         //     .AsNoTracking()
//                         //     .ToArrayAsync())
//                         //     .GroupBy(i => i.PlanInfoId)
//                         //     .Select(i => new
//                         //     {
//                         //         Id = i.Key,
//                         //         Complex = i.Count(j => j.Complex),
//                         //         Simple = i.Count(j => !j.Complex),
//                         //     })
//                         //     .ToDictionary(k => k.Id, v => new
//                         //     {
//                         //         v.Complex,
//                         //         v.Simple
//                         //     });
//                         //
//                         // var packageInfos = await (
//                         //     from wp in unit.WorkPackages
//                         //     join proj in unit.Projects on wp.ProjectId equals proj.Id
//                         //     orderby wp.CreatedAt descending 
//                         //     group proj by proj.PlanInfoId into grouped
//                         //     select new
//                         //     {
//                         //         grouped.Key,
//                         //         Count = grouped.Count()
//                         //     }
//                         // ).ToDictionaryAsync(k => k.Key, v => v.Count);
//                         //
//                         // int counter = 0;
//                         // foreach (var info in planInfos.Where(k => k.Id == Guid.Parse("5af146c2-9dce-410a-a526-7057bb3242c7")))
//                         // {
//                         //     if (++counter % 10 == 0) Console.WriteLine("counter = {0}, of = {1}", counter, planInfos.Length);
//                         //
//                         //     if (groupInfos.ContainsKey(info.Id))
//                         //     {
//                         //         info.UsedComplexGroup = groupInfos[info.Id].Complex;
//                         //         info.UsedSimpleGroup = groupInfos[info.Id].Simple;
//                         //     }
//                         //     
//                         //     if (projectsInfos.ContainsKey(info.Id))
//                         //     {
//                         //         info.UsedProject = projectsInfos[info.Id].Complex;
//                         //     }
//                         //     
//                         //     if (packageInfos.ContainsKey(info.Id))
//                         //     {
//                         //         info.UsedWorkPackage = packageInfos[info.Id];
//                         //     }
//                         //     info.UpdatedAt = now;
//                         // }
//                     }
//
//                     Console.WriteLine("PlanSyncerService : finished");
//                 }
//                 catch (Exception ex)
//                 {
//                     await scope.ServiceProvider.GetService<IErrorBiz>().LogException(ex);
//                     Console.WriteLine("PlanSyncerService : failed!!!!");
//                 }
//                 finally
//                 {
//                     _lock = false;
//                 }
//             }
//         }
//
//         public Task StopAsync(CancellationToken stoppingToken)
//         {
//             _timer?.Change(Timeout.Infinite, 0);
//             return Task.CompletedTask;
//         }
//
//         public void Dispose()
//         {
//             _timer?.Dispose();
//         }
//     }
// }

