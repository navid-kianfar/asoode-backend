// using System;
// using System.IO;
// using System.Threading;
// using System.Linq;
// using System.Threading.Tasks;
// using Amazon;
// using Amazon.S3;
// using Amazon.S3.Model;
// using Asoode.Core.Contracts.General;
// using Asoode.Core.Contracts.Logging;
// using Asoode.Data.Contexts;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Z.EntityFramework.Plus;
//
// namespace Asoode.Background.Services
// {
//     public class StorageBackgroundTask : IHostedService
//     {
//         private readonly IServiceProvider _serviceProvider;
//         private Timer _timer;
//         private bool _lock;
//
//         public StorageBackgroundTask(IServiceProvider serviceProvider)
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
//                 // TimeSpan.FromMinutes(1),
//                 TimeSpan.FromSeconds(10),
//                 TimeSpan.FromSeconds(30)
//             );
//             return Task.CompletedTask;
//         }
//
//         private async Task DoWork(object state)
//         {
//             if (_lock) 
//             {
//                 Console.WriteLine("StorageBackgroundTask : progress!!!!");
//                 return;
//             }
//             var host = _serviceProvider.GetService<IHost>();
//             using (var scope = host.Services.CreateScope())
//             {
//                 try
//                 {
//                     _lock = true;
//                     Console.WriteLine("StorageBackgroundTask : started");
//                     var now = DateTime.UtcNow;
//                     var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
//                     var _serverInfo = scope.ServiceProvider.GetService<IServerInfo>();
//                     using (var unit = scope.ServiceProvider.GetService<ProjectManagementDbContext>())
//                     {
//                         var uploads = await (
//                                 from upld in unit.Uploads
//                                 where upld.UpdatedAt == null && upld.DeletedAt == null
//                                 orderby upld.CreatedAt 
//                                 select new {Upload = upld}
//                             )
//                             .Take(500)
//                             .ToArrayAsync();
//
//                         if (uploads.Any())
//                         {
//                             var _accessKey = _configuration["Setting:Storage:AccessKey"];
//                             var _secretKey = _configuration["Setting:Storage:SecretKey"];
//                             var _bucket = _configuration["Setting:Storage:Bucket"];
//                             var _remote = $"https://{_configuration["Setting:Storage:Domain"]}";
//                             var _oldRemote = "https://storage.asoode.com"; 
//                             var cnn = new AmazonS3Client(_accessKey, _secretKey, new AmazonS3Config
//                             {
//                                 RegionEndpoint = RegionEndpoint.USEast1,
//                                 ServiceURL = _remote,
//                                 ForcePathStyle = true,
//                                 UseHttp = false
//                             });
//                             
//                             cnn.Config.Validate();
//
//                             var counter = 0;
//                             foreach (var record in uploads)
//                             {
//                                 if (counter++ % 10 == 0) await unit.SaveChangesAsync();
//                                 if (record.Upload.Path.StartsWith(_remote))
//                                 {
//                                     record.Upload.UpdatedAt = now;
//                                     continue;
//                                 }
//
//                                 var localFile = record.Upload.Path.Replace(_oldRemote, _serverInfo.FilesRootPath);
//                                 var newPath = record.Upload.Path.Replace(_oldRemote, _remote);
//                                 var key = record.Upload.Path.Replace(_oldRemote, "").TrimStart('/').Replace("//", "/");
//                                 
//                                 if (!File.Exists(localFile))
//                                 {
//                                     record.Upload.DeletedAt = now;
//                                     continue;
//                                 }
//                                 await cnn.PutObjectAsync(new PutObjectRequest
//                                 {
//                                     BucketName = _bucket,
//                                     Key = key,
//                                     CannedACL = S3CannedACL.PublicRead,
//                                     FilePath = localFile
//                                 });
//
//                                 record.Upload.ThumbnailPath = null;
//                                 record.Upload.Directory = Path.GetDirectoryName($"/{key}");
//                                 record.Upload.Path = newPath;
//                                 record.Upload.UpdatedAt = now;
//                                 await unit.SaveChangesAsync();
//                                 File.Delete(localFile);
//                             }
//
//                             await unit.SaveChangesAsync();
//                             await unit.Uploads.Where(i => i.DeletedAt.HasValue).DeleteAsync();
//                             await unit.WorkPackageTaskAttachments.Where(i => i.DeletedAt.HasValue).DeleteAsync();
//                             cnn.Dispose();
//                         }
//
//                     }
//
//                     Console.WriteLine("StorageBackgroundTask : finished");
//                 }
//                 catch (Exception ex)
//                 {
//                     await scope.ServiceProvider.GetService<IErrorBiz>().LogException(ex);
//                     Console.WriteLine("StorageBackgroundTask : failed!!!!");
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