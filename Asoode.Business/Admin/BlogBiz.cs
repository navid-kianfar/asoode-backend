using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Business.Extensions;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Extensions;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.Admin;

internal class BlogBiz : IBlogBiz
{
    private readonly IServiceProvider _serviceProvider;

    public BlogBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<GridResult<BlogViewModel>>> AdminBlogsList(Guid userId,
        GridFilterWithParams<GridQuery> model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var query = unit.Blogs.OrderByDescending(i => i.CreatedAt);
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
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<GridResult<BlogViewModel>>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<PostViewModel>>> AdminBlogPosts(Guid userId, Guid id,
        GridFilterWithParams<GridQuery> model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var query = unit.BlogPosts
                    .Where(i => i.BlogId == id)
                    .OrderByDescending(i => i.Priority)
                    .ThenByDescending(i => i.CreatedAt);
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
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<GridResult<PostViewModel>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> AdminEditBlog(Guid userId, Guid id, BlogEditViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var blog = await unit.Blogs.SingleOrDefaultAsync(b => b.Id == id);
                if (blog == null) return OperationResult<bool>.NotFound();

                var exists = await unit.Blogs.AnyAsync(b =>
                    b.Culture == model.Culture && b.Type == model.Type && b.Id != id);
                if (exists) return OperationResult<bool>.Duplicate();

                blog.Culture = model.Culture;
                blog.Description = model.Description;
                blog.Keywords = model.Keywords;
                blog.Title = model.Title;
                blog.Type = model.Type;
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

    public async Task<OperationResult<bool>> AdminCreateBlog(Guid userId, BlogEditViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var exists = await unit.Blogs.AnyAsync(b =>
                    b.Culture == model.Culture && b.Type == model.Type);
                if (exists) return OperationResult<bool>.Duplicate();

                await unit.Blogs.AddAsync(new Blog
                {
                    Culture = model.Culture,
                    Description = model.Description,
                    Keywords = model.Keywords,
                    Title = model.Title,
                    Type = model.Type
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

    public async Task<OperationResult<bool>> AdminCreatePost(
        Guid userId, Guid id, BlogPostEditViewModel model, StorageItemDto[] files)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var storageService = _serviceProvider.GetService<IStorageService>();
                var blog = await unit.Blogs.SingleOrDefaultAsync(b => b.Id == id);
                string largeImage = null, mediumImage = null, thumbImage = null;
                var postId = Guid.NewGuid();

                foreach (var file in files)
                {
                    file.UserId = userId;
                    file.RecordId = postId;
                    file.Section = UploadSection.Blog;
                    file.PlanId = Guid.Empty;
                }

                var thumbFile = files.FirstOrDefault(f => f.FileField == "thumbImage");
                if (thumbFile != null)
                {
                    var op = await storageService.Upload(thumbFile);
                    if (op.Status == OperationResultStatus.Success)
                        thumbImage = op.Data.Url;
                }

                var mediumFile = files.FirstOrDefault(f => f.FileField == "mediumImage");
                if (mediumFile != null)
                {
                    var op = await storageService.Upload(mediumFile);
                    if (op.Status == OperationResultStatus.Success)
                        mediumImage = op.Data.Url;
                }

                var largeFile = files.FirstOrDefault(f => f.FileField == "largeImage");
                if (largeFile != null)
                {
                    var op = await storageService.Upload(largeFile);
                    if (op.Status == OperationResultStatus.Success)
                        largeImage = op.Data.Url;
                }


                await unit.BlogPosts.AddAsync(new BlogPost
                {
                    Id = postId,
                    Key = DateTime.UtcNow.GetTime(),
                    Culture = blog.Culture,
                    Description = model.Description,
                    Keywords = model.Keywords,
                    Summary = model.Summary,
                    EmbedCode = model.EmbedCode,
                    Text = model.Text,
                    Title = model.Title,
                    Priority = model.Priority,
                    BlogId = blog.Id,
                    LargeImage = largeImage,
                    MediumImage = mediumImage,
                    ThumbImage = thumbImage,
                    NormalizedTitle = NormalizeTitle(model.Title)
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

    public async Task<OperationResult<bool>> AdminEditPost(
        Guid userId, Guid id, BlogPostEditViewModel model, StorageItemDto[] files)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                foreach (var file in files)
                {
                    file.UserId = userId;
                    file.RecordId = id;
                    file.Section = UploadSection.Blog;
                    file.PlanId = Guid.Empty;
                }

                var post = await unit.BlogPosts.SingleOrDefaultAsync(i => i.Id == id);
                var largeImage = post.LargeImage;
                var mediumImage = post.MediumImage;
                var thumbImage = post.ThumbImage;
                var storageService = _serviceProvider.GetService<IStorageService>();

                var thumbFile = files.FirstOrDefault(f => f.FileField == "thumbImage");
                if (thumbFile != null)
                {
                    var op = await storageService.Upload(thumbFile);
                    if (op.Status == OperationResultStatus.Success)
                        thumbImage = op.Data.Url;
                }

                var mediumFile = files.FirstOrDefault(f => f.FileField == "mediumImage");
                if (mediumFile != null)
                {
                    var op = await storageService.Upload(mediumFile);
                    if (op.Status == OperationResultStatus.Success)
                        mediumImage = op.Data.Url;
                }

                var largeFile = files.FirstOrDefault(f => f.FileField == "largeImage");
                if (largeFile != null)
                {
                    var op = await storageService.Upload(largeFile);
                    if (op.Status == OperationResultStatus.Success)
                        largeImage = op.Data.Url;
                }

                if (!string.IsNullOrEmpty(thumbImage) && thumbImage != post.ThumbImage)
                    await storageService.Delete(post.ThumbImage);
                if (!string.IsNullOrEmpty(mediumImage) && mediumImage != post.MediumImage)
                    await storageService.Delete(post.MediumImage);
                if (!string.IsNullOrEmpty(largeImage) && largeImage != post.LargeImage)
                    await storageService.Delete(post.LargeImage);

                post.Priority = model.Priority;
                post.Description = model.Description;
                post.Keywords = model.Keywords;
                post.Summary = model.Summary;
                post.EmbedCode = model.EmbedCode;
                post.Text = model.Text;
                post.Title = model.Title;
                post.NormalizedTitle = NormalizeTitle(model.Title);
                post.LargeImage = largeImage;
                post.MediumImage = mediumImage;
                post.ThumbImage = thumbImage;
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

    public async Task<OperationResult<bool>> AdminDeletePost(Guid userId, Guid id)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var post = await unit.BlogPosts.SingleOrDefaultAsync(i => i.Id == id);
                if (post == null) return OperationResult<bool>.NotFound();

                var storageService = _serviceProvider.GetService<IStorageService>();
                if (!string.IsNullOrEmpty(post.ThumbImage))
                    await storageService.Delete(post.ThumbImage);
                if (!string.IsNullOrEmpty(post.MediumImage))
                    await storageService.Delete(post.ThumbImage);
                if (!string.IsNullOrEmpty(post.LargeImage))
                    await storageService.Delete(post.ThumbImage);
                unit.BlogPosts.Remove(post);
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

    private string NormalizeTitle(string input)
    {
        return input.Trim().ToKebabCase();
    }
}