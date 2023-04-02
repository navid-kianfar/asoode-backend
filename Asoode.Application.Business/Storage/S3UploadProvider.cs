using System.Text.RegularExpressions;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.Extensions;
using Asoode.Application.Core.Helpers;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Storage;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.Storage
{
    internal class S3UploadProvider : IUploadProvider
    {
        #region Internal

        private const string IGNORE_FILE = "ignore.me.txt";
        private readonly Regex _cleanRegex = new Regex("[/]{2,}", RegexOptions.None);
        private readonly string _bucket;
        private readonly string _remote;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServerInfo _serverInfo;
        private readonly IConfiguration _configuration;

        public S3UploadProvider(IServiceProvider serviceProvider, IServerInfo serverInfo, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _serverInfo = serverInfo;
            _configuration = configuration;
            _accessKey = _configuration["Setting:Storage:AccessKey"];
            _secretKey = _configuration["Setting:Storage:SecretKey"];
            _bucket = _configuration["Setting:Storage:Bucket"];
            _remote = $"https://{_configuration["Setting:Storage:Domain"]}";
        }

        private IAmazonS3 GetClient()
        {
            var cnn = new AmazonS3Client(_accessKey, _secretKey, new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                ServiceURL = _remote,
                ForcePathStyle = true,
                UseHttp = false
            });
            cnn.Config.Validate();
            return cnn;
        }

        private string PrepareStore(StoreViewModel model, bool storage = false)
        {
            string destinationPath;
            switch (model.Section)
            {
                case UploadSection.Storage:
                    var userFolder = Path.Combine( "files", model.UserId.ToString());
                    var cleanPath = CleanPath((model.Path ?? "/").Trim('/').Replace(userFolder, "")).Trim('/');
                    destinationPath = Path.Combine( userFolder, cleanPath);
                    break;
                case UploadSection.WorkPackage:
                    destinationPath = Path.Combine(
                        model.PlanId.ToString(),
                        "task",
                        model.RecordId.ToString()
                    );
                    break;
                case UploadSection.Messenger:
                    destinationPath = Path.Combine(
                        model.PlanId.ToString(),
                        "channels",
                        model.RecordId.ToString()
                    );
                    break;
                case UploadSection.Blog:
                    destinationPath = Path.Combine(
                        "blog",
                        model.RecordId.ToString()
                    );
                    break;
                case UploadSection.Pdf:
                    destinationPath = Path.Combine(
                        "pdf",
                        model.RecordId.ToString()
                    );
                    break;
                case UploadSection.UserAvatar:
                    destinationPath = Path.Combine(
                        "avatar",
                        model.UserId.ToString()
                    );
                    break;
                default:
                    destinationPath = string.Empty;
                    break;
            }

            var name = Path.GetFileNameWithoutExtension(model.FormFile.FileName).RemoveSpecialCharacters();
            var ext = Path.GetExtension(model.FormFile.FileName);
            if (storage) return $"{destinationPath}/{name}{ext}";
            return $"{destinationPath}/{Guid.NewGuid()}/{name}{ext}";
        }

        private async Task<string> SaveTempFile(Stream file, string virtualPath)
        {
            var name = Path.GetFileName(virtualPath);
            var destinationFile = Path.Combine(_serverInfo.FilesRootPath, "tmp", Guid.NewGuid().ToString(), name);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
            using (var stream = new FileStream(destinationFile, FileMode.Create)) await file.CopyToAsync(stream);
            file.Seek(0, SeekOrigin.Begin);
            return destinationFile;
        }

        private bool IsValidPath(string path) => !path.Contains("../");

        private string GetUserStoragePath(Guid userId, string path)
        {
            if (path == "/") return CleanPath(Path.Combine("files", userId + path)).TrimEnd('/');

            return CleanPath(path).Trim('/');
        }

        private string CleanPath(string path) => _cleanRegex.Replace(path, "/");

        #endregion

        #region Thumbnail

        // private async Task<string> GetThumbnail(string sourceFile, string virtualPath)
        // {
        //     try
        //     {
        //         var destination = Path.Combine( "thumb", virtualPath + ".png" ).ToLower();
        //         var destinationDirectory = Path.GetDirectoryName(destination);
        //         if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);
        //         
        //         var ext = Path.GetExtension(virtualPath);
        //         string result = string.Empty;
        //         if (IOHelper.IsImage(ext)) result = await GetImageThumbnailPath(sourceFile, destination);
        //         if (IOHelper.IsVideo(ext)) result = await GetVideoThumbnailPath(sourceFile, destination);
        //         if (IOHelper.IsDocument(ext)) result = await GetDocumentThumbnailPath(sourceFile, destination);
        //         if (IOHelper.IsPdf(ext)) result = await GetPdfThumbnailPath(sourceFile, destination);
        //         if (IOHelper.IsPresentation(ext)) result = await GetPresentationThumbnailPath(sourceFile, destination);
        //         if (IOHelper.IsSpreadsheet(ext)) result = await GetSpreadsheetThumbnailPath(sourceFile, destination);
        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //         return null;
        //     }
        // }
        //
        // private async Task<string> GetDocumentThumbnailPath(string sourceFile, string destination)
        // {
        //     try
        //     {
        //         var document = new Spire.Doc.Document();
        //         document.LoadFromFile(sourceFile);
        //         var thumbnail = document.SaveToImages(0, Spire.Doc.Documents.ImageType.Metafile);
        //         thumbnail.Save(destination);
        //         return destination;
        //     }
        //     catch (Exception ex)
        //     {
        //         await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //         return string.Empty;
        //     }
        // }
        //
        // private async Task<string> GetPdfThumbnailPath(string sourceFile, string destination)
        // {
        //     try
        //     {
        //         var document = new Spire.Pdf.PdfDocument();
        //         document.LoadFromFile(sourceFile);
        //         var thumbnail = document.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Bitmap);
        //         thumbnail.Save(destination, ImageFormat.Png);
        //         return destination;
        //     }
        //     catch (Exception ex)
        //     {
        //         await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //         return string.Empty;
        //     }
        // }
        //
        // private async Task<string> GetPresentationThumbnailPath(string sourceFile, string destination)
        // {
        //     try
        //     {
        //         var document = new Spire.Presentation.Presentation();
        //         document.LoadFromFile(sourceFile);
        //         var thumbnail = document.Slides[0].SaveAsImage();
        //         thumbnail.Save(destination, ImageFormat.Png);
        //         return destination;
        //     }
        //     catch (Exception ex)
        //     {
        //         await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //         return string.Empty;
        //     }
        // }
        //
        // private async Task<string> GetSpreadsheetThumbnailPath(string sourceFile, string destination)
        // {
        //     try
        //     {
        //         var document = new Spire.Xls.Workbook();
        //         document.LoadFromFile(sourceFile);
        //         document.Worksheets[0].SaveToImage(destination);
        //         return destination;
        //     }
        //     catch (Exception ex)
        //     {
        //         await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //         return string.Empty;
        //     }
        // }
        //
        // private async Task<string> GetImageThumbnailPath(string sourceFile, string destination)
        // {
        //     try
        //     {
        //         int width = 150;
        //         int height = 150;
        //         using (Image image = Image.FromFile(sourceFile))
        //         using (Image thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero))
        //         {
        //             thumb.Save(destination, ImageFormat.Jpeg);
        //             return destination;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //         return string.Empty;
        //     }
        // }
        //
        // private async Task<string> GetVideoThumbnailPath(string sourceFile, string destination)
        // {
        //     try
        //     {
        //         IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(sourceFile);
        //         if (mediaInfo == null) return null;
        //         var snapshotPosition = Math.Floor(mediaInfo.Duration.TotalSeconds / 2);
        //         IConversion conversion = await FFmpeg.Conversions.FromSnippet
        //             .Snapshot(sourceFile, destination, TimeSpan.FromSeconds(snapshotPosition));
        //         conversion.AddParameter("-vf scale=320:-1");
        //         await conversion.Start();
        //         return destination;
        //     }
        //     catch (Exception ex)
        //     {
        //         await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //         return string.Empty;
        //     }
        // }

        #endregion

        public async Task<OperationResult<bool>> Delete(string file, UploadSection section, Guid? userId = null)
        {
            try
            {
                using (var client = GetClient())
                {
                    var key = file.Replace(_remote, "").Replace(_bucket, "");
                    var op = await client.DeleteObjectAsync(_bucket, key);
                    return OperationResult<bool>.Success();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<UploadViewModel>> Upload(StoreViewModel model)
        {
            try
            {
                using (var client = GetClient())
                {
                    long size = 0;
                    var source = PrepareStore(model, true);
                    var ext = Path.GetExtension(source);

                    if (model.FormFile != null)
                    {
                        model.FileStream = new MemoryStream();
                        await model.FormFile.CopyToAsync(model.FileStream);
                        size = model.FormFile.Length;
                    }

                    await client.PutObjectAsync(new PutObjectRequest
                    {
                        FilePath = model.FilePath,
                        InputStream = model.FileStream,
                        Key = source,
                        BucketName = _bucket,
                        CannedACL = S3CannedACL.PublicRead,
                    });

                    var path = $"{_remote}/{_bucket}/{source}";
                    return OperationResult<UploadViewModel>.Success(new UploadViewModel
                    {
                        Id = Guid.NewGuid(),
                        ThumbnailPath = null, //thumbPath,
                        Path = path,
                        Type = IOHelper.GetFileType(ext),
                        Size = size,
                        Name = Path.GetFileName(source),
                        Extension = ext,
                        Directory = Path.GetDirectoryName($"/{source}"),
                        Public = false,
                        Section = model.Section,
                        CreatedAt = DateTime.Now,
                        RecordId = model.RecordId,
                        UserId = model.UserId
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return null;
            }
        }

        public async Task<OperationResult<string>> Rename(RenameAttachmentViewModel model)
        {
            try
            {
                var key = model.Path.Replace(_remote, "");
                var ext = Path.GetExtension(key);
                var newName = $"{model.Name}{ext}";
                var newKey = key.Replace(Path.GetFileName(key), newName);
                using (var client = GetClient())
                {
                    await client.CopyObjectAsync(_bucket, key, _bucket, newKey);
                    await client.DeleteObjectAsync(_bucket, key);
                }

                var newPath = $"{_remote}/{_bucket}/{newKey}";
                return OperationResult<string>.Success(newPath);
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<string>.Failed();
            }
        }

        public async Task<OperationResult<ExplorerViewModel>> StorageSharedByMe(Guid userId, FileManagerViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var translate = _serviceProvider.GetService<ITranslateBiz>();
                    var result = new ExplorerViewModel();
                    var user = await unit.FindUser(userId);
                    var exception = new Exception("No Access Exception");

                    if (model.Path == "/")
                    {
                        result.Folders = new[]
                        {
                            new ExplorerFolderViewModel
                            {
                                Name = translate.Get("PROJECTS"),
                                Path = "/projects/",
                                CreatedAt = user.CreatedAt
                            },
                            new ExplorerFolderViewModel
                            {
                                Name = translate.Get("CHANNELS"),
                                Path = "/channels/",
                                CreatedAt = user.CreatedAt
                            }
                        };
                    }
                    else if (model.Path.StartsWith("/projects/"))
                    {
                        var groupIds = await unit.FindGroupIds(userId);
                        var projects = await unit.FindProjects(userId, groupIds);
                        result.Folders = projects.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = (p.Complex ? "/project/c/" + p.Id : "/project/s/" + p.Id),
                            Parent = "/",
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/project/c/"))
                    {
                        var guidStr = model.Path.Replace("/project/c/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var groupIds = await unit.FindGroupIds(userId);
                        var canAccess = await unit.ProjectMembers
                            .AnyAsync(p =>
                                p.ProjectId == id &&
                                (p.RecordId == userId ||
                                 groupIds.Contains(p.RecordId)));
                        if (!canAccess) throw exception;

                        var packages = await unit.WorkPackages
                            .AsNoTracking()
                            .Where(i => i.ProjectId == id)
                            .ToArrayAsync();

                        result.Folders = packages.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/package/" + p.Id,
                            Parent = "/project/c/" + p.ProjectId,
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/project/s/"))
                    {
                        var guidStr = model.Path.Replace("/project/s/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var tasks = await (
                            from attach in unit.WorkPackageTaskAttachments
                            join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                            where attach.UserId == userId && attach.ProjectId == id
                            select task
                        ).AsNoTracking().Distinct().ToArrayAsync();

                        result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/task/" + p.Id,
                            Parent = "/project/s/" + p.ProjectId,
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/package/"))
                    {
                        var guidStr = model.Path.Replace("/package/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var tasks = await (
                            from attach in unit.WorkPackageTaskAttachments
                            join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                            where attach.UserId == userId && attach.PackageId == id
                            select task
                        ).AsNoTracking().Distinct().ToArrayAsync();

                        result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/task/" + p.Id,
                            Parent = "/package/" + p.PackageId,
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/task/"))
                    {
                        var guidStr = model.Path.Replace("/task/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var attachments = await (
                            from attach in unit.WorkPackageTaskAttachments
                            where attach.UserId == userId && attach.TaskId == id
                            select attach
                        ).AsNoTracking().ToArrayAsync();

                        result.Files = attachments.Select(p =>
                        {
                            var ext = Path.GetExtension(p.Path);
                            return new ExplorerFileViewModel
                            {
                                Name = p.Title,
                                ExtensionLessName = p.Title,
                                CreatedAt = p.CreatedAt,
                                Extension = ext,
                                Size = 0,
                                Url = p.Path,
                                IsDocument = IOHelper.IsDocument(ext),
                                IsImage = IOHelper.IsImage(ext),
                                IsPdf = IOHelper.IsPdf(ext),
                                IsPresentation = IOHelper.IsPresentation(ext),
                                IsSpreadsheet = IOHelper.IsSpreadsheet(ext),
                                IsArchive = IOHelper.IsArchive(ext),
                                IsExecutable = IOHelper.IsExecutable(ext),
                                IsCode = IOHelper.IsCode(ext),
                                IsOther = IOHelper.IsOther(ext),
                            };
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/channels/"))
                    {
                        var groupIds = await unit.FindGroupIds(userId);
                        var projects = await unit.FindProjectIds(userId, groupIds);
                        var packages = await unit.FindWorkPackageIds(userId, groupIds);
                        var allIds = groupIds.Concat(projects).Concat(packages);
                        var channels = await unit.Channels.Where(c => allIds.Contains(c.Id))
                            .AsNoTracking()
                            .ToArrayAsync();
                        result.Folders = channels.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/channel/" + p.Id,
                            Parent = "/",
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/channel/"))
                    {
                        var guidStr = model.Path.Replace("/channel/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var uploads = await (from con in unit.Conversations
                            join upload in unit.Uploads on con.UploadId equals upload.Id
                            where (con.ChannelId == id &&
                                   con.Type == ConversationType.Upload &&
                                   con.UserId == userId)
                            select upload).AsNoTracking().ToArrayAsync();

                        result.Files = uploads.Select(p =>
                        {
                            var ext = Path.GetExtension(p.Path);
                            return new ExplorerFileViewModel
                            {
                                Name = p.Name,
                                ExtensionLessName = p.Name,
                                CreatedAt = p.CreatedAt,
                                Extension = ext,
                                Size = 0,
                                Url = p.Path,
                                IsDocument = IOHelper.IsDocument(ext),
                                IsImage = IOHelper.IsImage(ext),
                                IsPdf = IOHelper.IsPdf(ext),
                                IsPresentation = IOHelper.IsPresentation(ext),
                                IsSpreadsheet = IOHelper.IsSpreadsheet(ext),
                                IsArchive = IOHelper.IsArchive(ext),
                                IsExecutable = IOHelper.IsExecutable(ext),
                                IsCode = IOHelper.IsCode(ext),
                                IsOther = IOHelper.IsOther(ext),
                            };
                        }).ToArray();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    return OperationResult<ExplorerViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ExplorerViewModel>.Failed();
            }
        }

        public async Task<OperationResult<ExplorerViewModel>> StorageSharedByOthers(Guid userId, FileManagerViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var translate = _serviceProvider.GetService<ITranslateBiz>();
                    var result = new ExplorerViewModel();
                    var user = await unit.FindUser(userId);
                    var exception = new Exception("No Access Exception");

                    if (model.Path == "/")
                    {
                        result.Folders = new[]
                        {
                            new ExplorerFolderViewModel
                            {
                                Name = translate.Get("PROJECTS"),
                                Path = "/projects/",
                                CreatedAt = user.CreatedAt
                            },
                            new ExplorerFolderViewModel
                            {
                                Name = translate.Get("CHANNELS"),
                                Path = "/channels/",
                                CreatedAt = user.CreatedAt
                            }
                        };
                    }
                    else if (model.Path.StartsWith("/projects/"))
                    {
                        var groupIds = await unit.FindGroupIds(userId);
                        var projects = await unit.FindProjects(userId, groupIds);
                        result.Folders = projects.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = (p.Complex ? "/project/c/" + p.Id : "/project/s/" + p.Id),
                            Parent = "/",
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/project/c/"))
                    {
                        var guidStr = model.Path.Replace("/project/c/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var groupIds = await unit.FindGroupIds(userId);
                        var canAccess = await unit.ProjectMembers
                            .AnyAsync(p =>
                                p.ProjectId == id &&
                                (p.RecordId == userId ||
                                 groupIds.Contains(p.RecordId)));
                        if (!canAccess) throw exception;

                        var packages = await unit.WorkPackages
                            .AsNoTracking()
                            .Where(i => i.ProjectId == id)
                            .ToArrayAsync();

                        result.Folders = packages.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/package/" + p.Id,
                            Parent = "/project/c/" + p.ProjectId,
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/project/s/"))
                    {
                        var guidStr = model.Path.Replace("/project/s/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var groupIds = await unit.FindGroupIds(userId);
                        var canAccess = await unit.ProjectMembers
                            .AnyAsync(p =>
                                p.ProjectId == id &&
                                (p.RecordId == userId ||
                                 groupIds.Contains(p.RecordId)));
                        if (!canAccess) throw exception;

                        var tasks = await (
                            from attach in unit.WorkPackageTaskAttachments
                            join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                            where attach.UserId != userId && attach.ProjectId == id
                            select task
                        ).AsNoTracking().Distinct().ToArrayAsync();

                        result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/task/" + p.Id,
                            Parent = "/project/s/" + p.ProjectId,
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/package/"))
                    {
                        var guidStr = model.Path.Replace("/package/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var groupIds = await unit.FindGroupIds(userId);
                        var canAccess = await unit.WorkPackageMembers
                            .AnyAsync(p =>
                                p.PackageId == id &&
                                (p.RecordId == userId ||
                                 groupIds.Contains(p.RecordId)));
                        if (!canAccess) throw exception;

                        var tasks = await (
                            from attach in unit.WorkPackageTaskAttachments
                            join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                            where attach.UserId != userId && attach.PackageId == id
                            select task
                        ).AsNoTracking().Distinct().ToArrayAsync();

                        result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/task/" + p.Id,
                            Parent = "/package/" + p.PackageId,
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/task/"))
                    {
                        var guidStr = model.Path.Replace("/task/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var task = await unit.WorkPackageTasks
                            .AsNoTracking()
                            .SingleOrDefaultAsync(t => t.Id == id);

                        var groupIds = await unit.FindGroupIds(userId);
                        var canAccess = await unit.WorkPackageMembers
                            .AnyAsync(p =>
                                p.PackageId == task.PackageId &&
                                (p.RecordId == userId ||
                                 groupIds.Contains(p.RecordId)));
                        if (!canAccess) throw exception;

                        var attachments = await (
                            from attach in unit.WorkPackageTaskAttachments
                            where attach.UserId != userId && attach.TaskId == id
                            select attach
                        ).AsNoTracking().ToArrayAsync();

                        result.Files = attachments.Select(p =>
                        {
                            var ext = Path.GetExtension(p.Path);
                            return new ExplorerFileViewModel
                            {
                                Name = p.Title,
                                ExtensionLessName = p.Title,
                                CreatedAt = p.CreatedAt,
                                Extension = ext,
                                Size = 0,
                                Url = p.Path,
                                IsDocument = IOHelper.IsDocument(ext),
                                IsImage = IOHelper.IsImage(ext),
                                IsPdf = IOHelper.IsPdf(ext),
                                IsPresentation = IOHelper.IsPresentation(ext),
                                IsSpreadsheet = IOHelper.IsSpreadsheet(ext),
                                IsArchive = IOHelper.IsArchive(ext),
                                IsExecutable = IOHelper.IsExecutable(ext),
                                IsCode = IOHelper.IsCode(ext),
                                IsOther = IOHelper.IsOther(ext),
                            };
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/channels/"))
                    {
                        var groupIds = await unit.FindGroupIds(userId);
                        var projects = await unit.FindProjectIds(userId, groupIds);
                        var packages = await unit.FindWorkPackageIds(userId, groupIds);
                        var allIds = groupIds.Concat(projects).Concat(packages);
                        var channels = await unit.Channels.Where(c => allIds.Contains(c.Id))
                            .AsNoTracking()
                            .ToArrayAsync();
                        result.Folders = channels.Select(p => new ExplorerFolderViewModel
                        {
                            Name = p.Title,
                            Path = "/channel/" + p.Id,
                            Parent = "/",
                            CreatedAt = p.CreatedAt
                        }).ToArray();
                    }
                    else if (model.Path.StartsWith("/channel/"))
                    {
                        var guidStr = model.Path.Replace("/channel/", "");
                        var isGuid = Guid.TryParse(guidStr, out Guid id);
                        if (!isGuid) throw exception;

                        var channel = await unit.Channels
                            .AsNoTracking()
                            .SingleOrDefaultAsync(c => c.Id == id);

                        Guid[] groupIds;
                        bool canAccess = false;
                        switch (channel.Type)
                        {
                            case ChannelType.Bot:
                                throw exception;
                            case ChannelType.Direct:
                                throw exception;
                            case ChannelType.Group:
                                canAccess = await unit.GroupMembers
                                    .AnyAsync(p => p.GroupId == id && p.UserId == userId);
                                break;
                            case ChannelType.Project:
                                groupIds = await unit.FindGroupIds(userId);
                                canAccess = await unit.ProjectMembers
                                    .AnyAsync(p =>
                                        p.ProjectId == id &&
                                        (p.RecordId == userId ||
                                         groupIds.Contains(p.RecordId)));
                                break;
                            case ChannelType.WorkPackage:
                                groupIds = await unit.FindGroupIds(userId);
                                canAccess = await unit.WorkPackageMembers
                                    .AnyAsync(p =>
                                        p.PackageId == id &&
                                        (p.RecordId == userId ||
                                         groupIds.Contains(p.RecordId)));
                                break;
                        }

                        if (!canAccess) throw new Exception();

                        var uploads = await (from con in unit.Conversations
                            join upload in unit.Uploads on con.UploadId equals upload.Id
                            where (con.ChannelId == id &&
                                   con.Type == ConversationType.Upload &&
                                   con.UserId != userId)
                            select upload).AsNoTracking().ToArrayAsync();

                        result.Files = uploads.Select(p =>
                        {
                            var ext = Path.GetExtension(p.Path);
                            return new ExplorerFileViewModel
                            {
                                Name = p.Name,
                                ExtensionLessName = p.Name,
                                CreatedAt = p.CreatedAt,
                                Extension = ext,
                                Size = 0,
                                Url = p.Path,
                                IsDocument = IOHelper.IsDocument(ext),
                                IsImage = IOHelper.IsImage(ext),
                                IsPdf = IOHelper.IsPdf(ext),
                                IsPresentation = IOHelper.IsPresentation(ext),
                                IsSpreadsheet = IOHelper.IsSpreadsheet(ext),
                                IsArchive = IOHelper.IsArchive(ext),
                                IsExecutable = IOHelper.IsExecutable(ext),
                                IsCode = IOHelper.IsCode(ext),
                                IsOther = IOHelper.IsOther(ext),
                            };
                        }).ToArray();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    return OperationResult<ExplorerViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ExplorerViewModel>.Failed();
            }
        }

        public async Task<OperationResult<ExplorerViewModel>> StorageMine(Guid userId, FileManagerViewModel model)
        {
            try
            {
                if (!IsValidPath(model.Path)) return OperationResult<ExplorerViewModel>.Rejected();
                var result = new ExplorerViewModel();
                using (var client = GetClient())
                {
                    var key = GetUserStoragePath(userId, model.Path);
            
                    try
                    {
                        using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                        {
                            var directory = $"/{key}"; 
                            
                            var directories = await unit.Uploads.Where(u => 
                                    u.Path.Contains(key) && 
                                    u.Directory != directory
                                )
                                .AsNoTracking()
                                .ToArrayAsync();

                            var requiredSlashes = directory.Split('/').Length + 1;
                            result.Folders = directories
                                .GroupBy(i => i.Directory)
                                .Where(i => i.Key.Split('/').Length == requiredSlashes)
                                .Select(p =>
                                {
                                    var parent = Path.GetDirectoryName(p.Key);
                                    return new ExplorerFolderViewModel
                                    {
                                        Name = p.Key.Replace(parent, "").Trim('/'),
                                        Path = p.Key,
                                        Parent = parent,
                                        CreatedAt = p.OrderBy(i => i.CreatedAt).First().CreatedAt
                                    };
                                }).ToArray();

                            var files = await unit.Uploads.Where(u =>
                                    u.Directory == directory &&
                                    u.Name != IGNORE_FILE
                                )
                                .AsNoTracking()
                                .ToArrayAsync();
                            
                            result.Files = files.Select(p =>
                            {
                                var ext = Path.GetExtension(p.Path);
                                return new ExplorerFileViewModel
                                {
                                    Name = p.Name,
                                    ExtensionLessName = p.Name,
                                    CreatedAt = p.CreatedAt,
                                    Extension = ext,
                                    Size = 0,
                                    Url = p.Path,
                                    IsDocument = IOHelper.IsDocument(ext),
                                    IsImage = IOHelper.IsImage(ext),
                                    IsPdf = IOHelper.IsPdf(ext),
                                    IsPresentation = IOHelper.IsPresentation(ext),
                                    IsSpreadsheet = IOHelper.IsSpreadsheet(ext),
                                    IsArchive = IOHelper.IsArchive(ext),
                                    IsExecutable = IOHelper.IsExecutable(ext),
                                    IsCode = IOHelper.IsCode(ext),
                                    IsOther = IOHelper.IsOther(ext),
                                };
                            }).ToArray();
                        }
                    }
                    catch (Exception e)
                    {
                        if (!e.Message.Contains("Not Exists")) throw e;
                    }
                }
                
                return OperationResult<ExplorerViewModel>.Success(result);
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ExplorerViewModel>.Failed();
            }
        }
        
        public async Task<OperationResult<bool>> StorageNewFolder(Guid userId, FileManagerNameViewModel model)
        {
            try
            {
                using (var client = GetClient())
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    if (!IsValidPath(model.Path)) return OperationResult<bool>.Rejected();
                    var key = $"{GetUserStoragePath(userId, model.Path)}/{model.Name}/{IGNORE_FILE}";
                    await client.PutObjectAsync(new PutObjectRequest
                    {
                        FilePath = Path.Combine(_serverInfo.ContentRootPath, IGNORE_FILE),
                        Key = key,
                        BucketName = _bucket,
                        CannedACL = S3CannedACL.PublicRead,
                    });
                    await unit.Uploads.AddAsync(new Upload
                    {
                        Directory = $"/{GetUserStoragePath(userId, model.Path)}/{model.Name}",
                        Extension = ".txt",
                        Id = Guid.NewGuid(),
                        Name = IGNORE_FILE,
                        Path = $"{_remote}/{_bucket}/{key}",
                        Public = false,
                        Section = UploadSection.Storage,
                        Size = 1,
                        Type = FileType.Documents,
                        CreatedAt = DateTime.Now,
                        RecordId = Guid.NewGuid(),
                        UserId = userId
                    });
                    await unit.SaveChangesAsync();
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }
        
        public Task<OperationResult<bool>> StorageRename(Guid userId, FileManagerNameViewModel model)
        {
            return Task.FromResult(OperationResult<bool>.Failed());
        }

        public async Task<OperationResult<bool>> StorageDelete(Guid userId, FileManagerDeleteViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<UploadResultViewModel>> StorageUpload(Guid userId, IFormFile file,
            FileManagerViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    var plan = await unit.UserPlanInfo.OrderByDescending(i => i.CreatedAt).FirstAsync();
                    if (plan.AttachmentSize < file.Length)
                    {
                        return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                        {
                            AttachmentSize = true
                        });
                    }

                    if ((plan.UsedSpace + file.Length) > plan.Space)
                    {
                        return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                        {
                            StorageSize = true
                        });
                    }

                    plan.UsedSpace += file.Length;
                    var result = await Upload(new StoreViewModel
                    {
                        FormFile = file,
                        Section = UploadSection.Storage,
                        PlanId = plan.Id,
                        RecordId = userId,
                        UserId = userId,
                        Path = model.Path
                    });
                    if (result.Status != OperationResultStatus.Success)
                        return OperationResult<UploadResultViewModel>.Rejected();
                    await unit.Uploads.AddAsync(new Upload
                    {
                        Directory = result.Data.Directory,
                        Extension = result.Data.Extension,
                        Name = result.Data.Name,
                        Path = result.Data.Path,
                        Public = false,
                        Section = UploadSection.Storage,
                        Size = result.Data.Size,
                        RecordId = result.Data.RecordId,
                        ThumbnailPath = result.Data.ThumbnailPath,
                        UserId = result.Data.UserId,
                        Type = result.Data.Type,
                        Id = result.Data.Id
                    });
                    await unit.SaveChangesAsync();
                    return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                    {
                        Success = true
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<UploadResultViewModel>.Failed();
            }
        }

        public Task<OperationResult<UploadViewModel[]>> BulkUpload(StoreViewModel storeViewModel)
        {
            return Task.FromResult(OperationResult<UploadViewModel[]>.Failed());
        }

        public Task<OperationResult<Stream>> BulkDownload(Guid userId, Dictionary<string, string[]> paths)
        {
            return Task.FromResult(OperationResult<Stream>.Failed());
        }
    }
}


// /*
//  *
//  * 
//     {
//         private readonly IServiceProvider _serviceProvider;
//         private readonly IServerInfo _serverInfo;
//         private readonly Regex _cleanRegex = new Regex("[/]{2,}", RegexOptions.None);
//         private readonly string _urlPrefix;
//
//         public LocalUploadProvider(
//             IServiceProvider serviceProvider,
//             IServerInfo serverInfo,
//             IConfiguration configuration)
//         {
//             _serviceProvider = serviceProvider;
//             _serverInfo = serverInfo;
//             _urlPrefix = $"https://storage.{configuration["Setting:Domain"]}";
//         }
//
//         public string RemoveUrlPrefix(string url)
//         {
//             return url.Replace(_urlPrefix, "");
//         }
//
//         public string RelativeToAbsolute(string path)
//         {
//             return path.Replace(_urlPrefix, _serverInfo.FilesRootPath);
//         }
//
//         public string RelativeToAbsolute(string path, UploadSection section, Guid? userId = null)
//         {
//             path = path.Replace(_urlPrefix, "");
//             string destinationPath = string.Empty;
//             switch (section)
//             {
//                 case UploadSection.Storage:
//                     destinationPath = CleanPath($"{GetUserStorageRoot(userId.Value)}/{path}");
//                     break;
//                 case UploadSection.Blog:
//                 case UploadSection.Messenger:
//                 case UploadSection.WorkPackage:
//                     destinationPath = CleanPath($"{_serverInfo.FilesRootPath}{path}");
//                     break;
//                 case UploadSection.GroupAvatar:
//                     break;
//                 case UploadSection.UserAvatar:
//                     break;
//             }
//
//             return destinationPath;
//         }
//
//
//         public async Task<OperationResult<bool>> Delete(string path, UploadSection section, Guid? userId = null)
//         {
//             try
//             {
//                 if (string.IsNullOrEmpty(path)) return OperationResult<bool>.Success();
//                 path = path.Replace(_urlPrefix, "");
//                 var destinationPath = RelativeToAbsolute(path, section, userId);
//                 if (!File.Exists(destinationPath)) return OperationResult<bool>.Success();
//                 File.Delete(destinationPath);
//                 return OperationResult<bool>.Success();
//             }
//             catch (Exception ex)
//             {
//                 await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
//                 return OperationResult<bool>.Failed();
//             }
//         }
//
//         public string GetStorageAbsolutePath(Guid userId, string path)
//         {
//             return CleanPath($"{GetUserStorageRoot(userId)}/{path}");
//         }
//
//         public OperationResult<RenameResultViewModel> Rename(Guid userId, FileManagerNameViewModel model)
//         {
//             var path = RelativeToAbsolute(model.Path, UploadSection.Storage, userId);
//             var pathParent = Path.GetDirectoryName(path) ?? "";
//             string destination;
//
//             FileAttributes attr = File.GetAttributes(path);
//             if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
//             {
//                 destination = Path.Combine(pathParent, model.Name);
//                 if (destination != path) Directory.Move(path, destination);
//                 return OperationResult<RenameResultViewModel>.Success(new RenameResultViewModel
//                 {
//                     Directory = true,
//                     NewPath = AbsoluteToRelative(destination),
//                     OldPath = AbsoluteToRelative(path),
//                 });
//             }
//
//             destination = Path.Combine(pathParent, model.Name + Path.GetExtension(path));
//
//             if (destination != path) File.Move(path, destination);
//             return OperationResult<RenameResultViewModel>.Success(new RenameResultViewModel
//             {
//                 Directory = false,
//                 NewPath = AbsoluteToRelative(destination),
//                 OldPath = AbsoluteToRelative(path),
//             });
//         }
//
//         public OperationResult<FileManagerDeleteViewModel> Delete(Guid userId, FileManagerDeleteViewModel model)
//         {
//             var result = new List<string>();
//             foreach (var _path in model.Paths)
//             {
//                 var path = RelativeToAbsolute(_path, UploadSection.Storage, userId);
//                 FileAttributes attr = File.GetAttributes(path);
//                 if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
//                     Directory.Delete(path, true);
//                 else File.Delete(path);
//                 result.Add(AbsoluteToRelative(path));
//             }
//
//             return OperationResult<FileManagerDeleteViewModel>.Success(new FileManagerDeleteViewModel
//             {
//                 Paths = result.ToArray()
//             });
//         }
//
//         
//

//
//         private bool IsValidPath(string path)
//         {
//             return !path.Contains("../");
//         }
//
//         public string DocumentFilePath(string path)
//         {
//             return path.Replace(_urlPrefix, _serverInfo.FilesRootPath);
//         }
//
//         public string AbsoluteToRelative(string path, bool prefix = true)
//         {
//             if (prefix) return $"{_urlPrefix}{path.Replace(_serverInfo.FilesRootPath, "")}";
//             return path.Replace(_serverInfo.FilesRootPath, "");
//         }
//
//         private string StorageAbsoluteToRelative(string path, string root)
//         {
//             return path.Replace(root, "");
//         }
//
//         public async Task<OperationResult<UploadViewModel>> Upload(StoreViewModel model)
//         {
//             try
//             {
//                 if (model.Section == UploadSection.Storage && !IsValidPath(model.Path))
//                     return OperationResult<UploadViewModel>.Rejected();
//                 string destinationFile = GetFilePath(model);
//                 string destinationPath = Path.GetDirectoryName(destinationFile);
//
//                 using (var stream = new FileStream(destinationFile, FileMode.Create))
//                     await model.File.CopyToAsync(stream);
//
//                 var thumbnail = await GetThumbnail(destinationFile);
//                 var name = Path.GetFileNameWithoutExtension(model.File.FileName);
//                 var ext = Path.GetExtension(model.File.FileName);
//                 var upload = new UploadViewModel
//                 {
//                     ThumbnailPath = string.IsNullOrEmpty(thumbnail) ? "" : AbsoluteToRelative(thumbnail),
//                     Directory = AbsoluteToRelative(destinationPath),
//                     Path = AbsoluteToRelative(destinationFile),
//                     Type = IOHelper.GetFileType(ext),
//                     RecordId = model.RecordId,
//                     UserId = model.UserId,
//                     Extension = ext,
//                     Name = name,
//                     Public = false,
//                     Section = model.Section,
//                     Size = model.File.Length,
//                 };
//                 return OperationResult<UploadViewModel>.Success(upload);
//             }
//             catch (Exception ex)
//             {
//                 await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
//                 return OperationResult<UploadViewModel>.Failed();
//             }
//         }
//
//         public async Task<OperationResult<UploadViewModel[]>> BulkUpload(StoreViewModel storeViewModel)
//         {
//             try
//             {
//                 var result = new List<UploadViewModel>();
//                 var allowedExtensions = new[] {".mp3", ".mp4", ".mov", ".moho"};
//
//                 var serverInfo = _serviceProvider.GetService<IServerInfo>();
//                 var zipPath = $"{serverInfo.ContentRootPath}/tmp/{Guid.NewGuid()}/bulk-upload.zip";
//                 var extractDirectory = Path.GetDirectoryName(zipPath);
//                 Directory.CreateDirectory(extractDirectory);
//
//                 using (var stream = new FileStream(zipPath, FileMode.Create))
//                     await storeViewModel.File.CopyToAsync(stream);
//                 using (var zip = ZipFile.OpenRead(zipPath))
//                 {
//                     var entries = zip.Entries.Where(e =>
//                     {
//                         var fileName = Path.GetFileName(e.Name.ToLower());
//                         return !fileName.StartsWith(".") && allowedExtensions.Any(ae => fileName.EndsWith(ae));
//                     }).Select(e => e.FullName).ToArray();
//
//                     foreach (var entry in entries)
//                     {
//                         var filename = Path.GetFileNameWithoutExtension(entry.ToLower());
//                         var extension = Path.GetExtension(entry.ToLower());
//                         var sub = storeViewModel.Subs
//                             .SingleOrDefault(s => s.Text.ToLower() == filename);
//                         if (sub == null)
//                         {
//                             sub = storeViewModel.Subs.SingleOrDefault(s =>
//                             {
//                                 var taskTitle = s.Text.ToLower();
//                                 var common = taskTitle.CommonPrefix(filename);
//                                 if (string.IsNullOrEmpty(common))
//                                     throw new Exception("NO Common name in : " + storeViewModel.File.FileName);
//                                 var left = taskTitle.Replace(common, "");
//                                 var right = filename.Replace(common, "");
//                                 var s1 = int.TryParse(left, out int leftCounter);
//                                 var s2 = int.TryParse(right, out int rightCounter);
//                                 return s1 && s2 && leftCounter == rightCounter;
//                             });
//                         }
//
//                         if (sub == null) continue;
//
//                         using (var entryStream = zip.GetEntry(entry).Open())
//                         using (var memoryStream = new MemoryStream())
//                         {
//                             await entryStream.CopyToAsync(memoryStream);
//                             memoryStream.Seek(0, SeekOrigin.Begin);
//                             var name = sub.Text.ToUpper() + extension;
//                             var op = await Upload(new StoreViewModel
//                             {
//                                 File = new FormFile(memoryStream, 0, memoryStream.Length, null, name)
//                                 {
//                                     Headers = new HeaderDictionary(),
//                                     ContentType = IOHelper.GetMimeType(extension)
//                                 },
//                                 Section = storeViewModel.Section,
//                                 PlanId = storeViewModel.PlanId,
//                                 RecordId = sub.Value,
//                                 UserId = storeViewModel.UserId
//                             });
//                             if (op.Status == OperationResultStatus.Success) result.Add(op.Data);
//                         }
//                     }
//                 }
//
//                 Directory.Delete(extractDirectory, true);
//                 return OperationResult<UploadViewModel[]>.Success(result.ToArray());
//             }
//             catch (Exception ex)
//             {
//                 await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
//                 return OperationResult<UploadViewModel[]>.Failed();
//             }
//         }
//
//         public async Task<OperationResult<Stream>> BulkDownload(Guid userId, Dictionary<string, string[]> paths)
//         {
//             var stream = new MemoryStream();
//             var zip = new ZipArchive(stream, ZipArchiveMode.Create);
//             var errorBiz = _serviceProvider.GetService<IErrorBiz>();
//
//             foreach (var key in paths.Keys)
//             {
//                 var dirName = $"{key}/";
//                 zip.CreateEntry(dirName);
//                 foreach (var path in paths[key])
//                 {
//                     try
//                     {
//                         var local = RelativeToAbsolute(path, UploadSection.WorkPackage, userId);
//                         var fileName = Path.Combine(dirName, Path.GetFileName(local));
//                         if (File.Exists(local))
//                         {
//                             var entry = zip.CreateEntry(fileName, CompressionLevel.NoCompression);
//                             using (var zipStream = entry.Open())
//                             using (var fileStream = File.OpenRead(local))
//                             {
//                                 await fileStream.CopyToAsync(zipStream);
//                                 await zipStream.FlushAsync();
//                             }
//                         }
//                         else
//                         {
//                             await errorBiz.LogException(new Exception($"NOT FOUND: {local}"));
//                         }
//                     }
//                     catch (Exception ex)
//                     {
//                         await errorBiz.LogException(ex);
//                     }
//                 }
//             }
//
//             zip.Dispose();
//             stream = new MemoryStream(stream.ToArray());
//             return OperationResult<Stream>.Success(stream);
//         }
//
//         public Task<OperationResult<bool>> StorageRename(Guid userId, FileManagerNameViewModel model)
//         {
//             
//             // try
//             // {
//             //     using (var unit = _serviceProvider.GetService<GeneralDbContext>())
//             //     {
//             //         var op = _uploadProvider.Rename(userId, model);
//             //         
//             //         var uploads = await unit.Uploads
//             //             .Where(u => u.Path.Contains(op.Data.OldPath))
//             //             .ToArrayAsync();
//             //         foreach (var upload in uploads)
//             //         {
//             //             upload.Name = model.Name;
//             //             upload.Path = upload.Path.Replace(op.Data.OldPath, op.Data.NewPath);
//             //             upload.Directory = Path.GetDirectoryName(upload.Path);
//             //         }
//             //         var attachments = await unit.WorkPackageTaskAttachments
//             //             .Where(u => u.Path.Contains(op.Data.OldPath))
//             //             .ToArrayAsync();
//             //         foreach (var attachment in attachments)
//             //         {
//             //             attachment.Path = attachment.Path.Replace(op.Data.OldPath, op.Data.NewPath);
//             //         }
//             //
//             //         await unit.SaveChangesAsync();
//             //         return OperationResult<bool>.Success(true);
//             //     }
//             // }
//             // catch (Exception ex)
//             // {
//             //     await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
//             //     return OperationResult<bool>.Failed();
//             // }
//         }
//
//         public Task<OperationResult<bool>> StorageDelete(Guid userId, FileManagerDeleteViewModel model)
//         {
//             // try
//             // {
//             //     using (var unit = _serviceProvider.GetService<GeneralDbContext>())
//             //     {
//             //         var op = _uploadProvider.Delete(userId, model);
//             //         if (op.Data == null || op.Data.Paths.Length == 0) return OperationResult<bool>.Success(true);
//             //
//             //         var paths = op.Data.Paths;
//             //         var uploads = await unit.Uploads
//             //             .Where(u => u.Section == UploadSection.Storage && u.UserId == userId)
//             //             .ToArrayAsync();
//             //
//             //         var filtered = uploads.Where(u =>
//             //             paths.Any(p => u.Path.Contains(p))).ToArray();
//             //
//             //         var plan = await unit.UserPlanInfo.OrderByDescending(i => i.CreatedAt).FirstAsync();
//             //         plan.UsedSpace -= (filtered.Select(u => u.Size).Sum());
//             //         
//             //         unit.Uploads.RemoveRange(filtered);
//             //         await unit.SaveChangesAsync();
//             //         return OperationResult<bool>.Success(true);
//             //     }
//             // }
//             // catch (Exception ex)
//             // {
//             //     await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
//             //     return OperationResult<bool>.Failed();
//             // }
//         }
//
//     
//
//         public async Task<OperationResult<bool>> StorageNewFolder(Guid userId, FileManagerNameViewModel model)
//         {
//             try
//             {
//                 if (!IsValidPath(model.Path)) return OperationResult<bool>.Rejected();
//                 var root = GetUserStorageRoot(userId);
//                 if (!Directory.Exists(root)) Directory.CreateDirectory(root);
//                 var destination = CleanPath($"{root}{model.Path}/{model.Name}");
//                 if (Directory.Exists(destination)) return OperationResult<bool>.Duplicate();
//                 Directory.CreateDirectory(destination);
//                 return OperationResult<bool>.Success();
//             }
//             catch (Exception ex)
//             {
//                 await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
//                 return OperationResult<bool>.Failed();
//             }
//         }
//
//         public Task<OperationResult<UploadResultViewModel>> StorageUpload(Guid userId, IFormFile file, FileManagerViewModel model)
//         {
//             // try
//             // {
//             //     using (var unit = _serviceProvider.GetService<GeneralDbContext>())
//             //     {
//             //         var plan = await unit.UserPlanInfo.OrderByDescending(i => i.CreatedAt).FirstAsync();
//             //         if (plan.AttachmentSize < file.Length)
//             //         {
//             //             return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
//             //             {
//             //                 AttachmentSize = true
//             //             });
//             //         }
//             //
//             //         if ((plan.UsedSpace + file.Length) > plan.Space)
//             //         {
//             //             return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
//             //             {
//             //                 StorageSize = true
//             //             });
//             //         }
//             //
//             //         plan.UsedSpace += file.Length;
//             //         var result = await _uploadProvider.Upload(new StoreViewModel
//             //         {
//             //             File = file,
//             //             Section = UploadSection.Storage,
//             //             PlanId = plan.Id,
//             //             RecordId = userId,
//             //             UserId = userId,
//             //             Path = model.Path
//             //         });
//             //         if (result.Status != OperationResultStatus.Success)
//             //             return OperationResult<UploadResultViewModel>.Rejected();
//             //         await unit.Uploads.AddAsync(new Upload
//             //         {
//             //             Directory = result.Data.Directory,
//             //             Extension = result.Data.Extension,
//             //             Name = result.Data.Name,
//             //             Path = result.Data.Path,
//             //             Public = false,
//             //             Section = UploadSection.Storage,
//             //             Size = result.Data.Size,
//             //             RecordId = result.Data.RecordId,
//             //             ThumbnailPath = result.Data.ThumbnailPath,
//             //             UserId = result.Data.UserId,
//             //             Type = result.Data.Type,
//             //             Id = result.Data.Id
//             //         });
//             //         await unit.SaveChangesAsync();
//             //         return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
//             //         {
//             //             Success = true
//             //         });
//             //     }
//             // }
//             // catch (Exception ex)
//             // {
//             //     await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
//             //     return OperationResult<UploadResultViewModel>.Failed();
//             // }
//         }
//
//
//     }
//  *
//  *
//  *
//  * 
//  */