using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Admin.Business.Implementation;

internal class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly ILoggerService _loggerService;
    private readonly IStorageService _storageService;

    public BlogService(
        IBlogRepository blogRepository, 
        ILoggerService loggerService, 
        IStorageService storageService)
    {
        _blogRepository = blogRepository;
        _loggerService = loggerService;
        _storageService = storageService;
    }

    public Task<OperationResult<GridResult<BlogDto>>> BlogsList(Guid userId, GridFilterWithParams<GridQuery> model)
        => _blogRepository.BlogsList(userId, model);

    public Task<OperationResult<GridResult<PostDto>>> BlogPosts(Guid userId, Guid id,
        GridFilterWithParams<GridQuery> model)
        => _blogRepository.BlogPosts(userId, id, model);

    public Task<OperationResult<bool>> EditBlog(Guid userId, Guid id, BlogEditDto model)
        => _blogRepository.EditBlog(userId, id, model);

    public Task<OperationResult<bool>> CreateBlog(Guid userId, BlogEditDto model)
        => _blogRepository.CreateBlog(userId, model);

    public async Task<OperationResult<bool>> CreatePost(Guid userId, Guid id, BlogPostEditDto model, StorageItemDto[] files)
    {
        try
        {
            await PrepareFiles(files, model);
            return await _blogRepository.CreatePost(userId, id, model);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogService.CreatePost", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> EditPost(Guid userId, Guid id, BlogPostEditDto model, StorageItemDto[] files)
    {
        try
        {
            var post = await _blogRepository.GetPost(id);
            if (post.Status != OperationResultStatus.Success)
                return OperationResult<bool>.Failed();

            model.ThumbImage = post.Data!.ThumbImage;
            model.MediumImage = post.Data!.MediumImage;
            model.LargeImage = post.Data!.LargeImage;
            
            await PrepareFiles(files, model);
            
            return await _blogRepository.EditPost(userId, id, model);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogService.EditPost", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> DeletePost(Guid userId, Guid id)
    {
        try
        {
            var post = await _blogRepository.GetPost(id);
            if (post.Status != OperationResultStatus.Success)
                return OperationResult<bool>.Failed();

            if (!string.IsNullOrEmpty(post.Data!.ThumbImage))
                await _storageService.Delete(post.Data!.ThumbImage, SharedConstants.PublicBucket);

            if (!string.IsNullOrEmpty(post.Data!.MediumImage))
                await _storageService.Delete(post.Data!.MediumImage, SharedConstants.PublicBucket);

            if (!string.IsNullOrEmpty(post.Data!.LargeImage))
                await _storageService.Delete(post.Data!.LargeImage, SharedConstants.PublicBucket);
            
            return await _blogRepository.DeletePost(userId, id);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "BlogService.DeletePost", e);
            return OperationResult<bool>.Failed();
        }
    }

    private async Task PrepareFiles(StorageItemDto[] files, BlogPostEditDto model)
    {
        foreach (var file in files)
        {
            var path = $"blog/{file.FileField}/{Guid.NewGuid()}/{file.FileName}";
            var upload = await _storageService.Upload(file, SharedConstants.PublicBucket, path);
            if (upload.Status == OperationResultStatus.Success)
            {
                file.Url = upload.Data!.Url;
                file.Path = upload.Data!.Path;
                await file.Stream!.DisposeAsync();
                file.Stream = null;
            }
        }
        
        var thumbImage = files.FirstOrDefault(f => f.FileField == "thumbImage");
        if (!string.IsNullOrEmpty(model.ThumbImage) && thumbImage != null)
            await _storageService.Delete(model.ThumbImage, SharedConstants.PublicBucket);
        model.ThumbImage = thumbImage?.Url ?? string.Empty;
        
        var mediumImage = files.FirstOrDefault(f => f.FileField == "mediumImage");
        if (!string.IsNullOrEmpty(model.MediumImage) && mediumImage != null)
            await _storageService.Delete(model.MediumImage, SharedConstants.PublicBucket);
        model.MediumImage = mediumImage?.Url ?? string.Empty;
        
        var largeImage = files.FirstOrDefault(f => f.FileField == "largeImage");
        if (!string.IsNullOrEmpty(model.LargeImage) && largeImage != null)
            await _storageService.Delete(model.LargeImage, SharedConstants.PublicBucket);
        model.LargeImage = largeImage?.Url ?? string.Empty;
    }
}