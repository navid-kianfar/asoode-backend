using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Storage;

namespace Asoode.Core.Contracts.Admin;

public interface IBlogBiz
{
    Task<OperationResult<GridResult<BlogViewModel>>> AdminBlogsList(Guid userId, GridFilterWithParams<GridQuery> model);

    Task<OperationResult<GridResult<PostViewModel>>> AdminBlogPosts(Guid userId, Guid id,
        GridFilterWithParams<GridQuery> model);

    Task<OperationResult<bool>> AdminEditBlog(Guid userId, Guid id, BlogEditViewModel model);
    Task<OperationResult<bool>> AdminCreateBlog(Guid userId, BlogEditViewModel model);

    Task<OperationResult<bool>> AdminCreatePost(Guid userId, Guid id, BlogPostEditViewModel model,
        StorageItemDto[] files);

    Task<OperationResult<bool>> AdminEditPost(Guid userId, Guid id, BlogPostEditViewModel model,
        StorageItemDto[] files);

    Task<OperationResult<bool>> AdminDeletePost(Guid userId, Guid id);
}