using Asoode.Application.Business.Extensions;
using Asoode.Application.Business.ProjectManagement;
using Asoode.Application.Core.Contracts.Collaboration;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.Collaboration;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Application.Core.ViewModels.Reports;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Base;
using Asoode.Application.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.Collaboration
{
    internal class GroupBiz : IGroupBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public GroupBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Create / Remove

        public async Task<OperationResult<bool>> Create(Guid userId, GroupCreateViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var validation = _serviceProvider.GetService<IValidateBiz>();

                    var user = await unit.FindUser(userId);
                    if (user == null) return OperationResult<bool>.Rejected();

                    var parent = !model.ParentId.HasValue
                        ? null
                        : await unit.Groups
                            .AsNoTracking()
                            .SingleOrDefaultAsync(i => i.Id == model.ParentId);

                    var parsed = await unit.ParseInvite(userId, validation, model.Members);

                    var groupId = Guid.NewGuid();
                    var group = new Group
                    {
                        Id = groupId,
                        RootId = parent?.RootId ?? groupId,
                        UserId = userId,
                        Title = model.Title.Trim(),
                        Description = model.Description.Trim(),
                        Type = model.Complex ? model.Type : GroupType.Team,
                        Complex = model.Complex,
                        Level = 1,
                        ParentId = model.ParentId
                    };
                    // var postman = _serviceProvider.GetService<IPostmanBiz>();
                    var groupPermissions = new List<GroupMember>
                    {
                        new GroupMember
                        {
                            Access = AccessType.Owner,
                            GroupId = groupId,
                            UserId = userId,
                            RootId = groupId,
                            Level = 1
                        }
                    };
                    foreach (var invite in parsed.InviteById)
                    {
                        groupPermissions.Add(new GroupMember
                        {
                            Access = invite.Access,
                            GroupId = group.Id,
                            UserId = Guid.Parse(invite.Id),
                            RootId = groupId,
                            Level = 1
                        });
                    }

                    var pendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access,
                        Identifier = e.Id,
                        RecordId = group.Id,
                        Type = PendingInvitationType.Group
                    }).ToArray();

                    var channel = new Channel
                    {
                        Title = model.Title,
                        Type = ChannelType.Group,
                        UserId = userId,
                        Id = group.Id,
                        RootId = groupId,
                    };

                    // TODO: send welcome messages in chat


                    await unit.Groups.AddAsync(group);
                    await unit.GroupMembers.AddRangeAsync(groupPermissions);
                    await unit.PendingInvitations.AddRangeAsync(pendingInvitations);
                    await unit.Channels.AddAsync(channel);
                    await unit.SaveChangesAsync();

                    Dictionary<string, string> mapped = new Dictionary<string, string>();
                    foreach (var usr in parsed.InviteById)
                    {
                        var found = parsed.AllInvited.Single(u => u.Id == Guid.Parse(usr.Id));
                        mapped.Add(usr.Id, found.Email);
                    }

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupAdd,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Group = group.ToViewModel(),
                        GroupMembers = groupPermissions.Select(p => p.ToViewModel()).ToArray(),
                        Pendings = pendingInvitations.Select(p => p.ToViewModel()).ToArray(),
                        UserIds = groupPermissions.Select(p => p.UserId).ToArray()
                    });

                    // TODO: send to queue for background service
// #pragma warning disable 4014
//                     Task.Run(() => postman.InviteGroup(user.FullName, parsed.EmailIdentities, mapped, group.Id, group.Title));
// #pragma warning restore 4014
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> Remove(Guid userId, Guid groupId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    // TODO: refactor
                    // var accessCheck = await unit.GroupMembers
                    //     .Where(i => i.GroupId == groupId && i.UserId == userId && i.Access == AccessType.Owner)
                    //     .AnyAsync();
                    // if (!accessCheck) return OperationResult<bool>.Rejected();
                    //
                    // var user = await unit.Users.AsNoTracking().SingleAsync(i => i.Id == userId);
                    // var group = await unit.Groups.SingleAsync(i => i.Id == groupId);
                    //
                    // unit.Groups.Remove(group);
                    // await unit.Groups.Where(i => i.ParentId == groupId)
                    //     .UpdateAsync(g => new Group {ParentId = group.ParentId});
                    // await unit.ProjectMembers.Where(i => i.RecordId == groupId).DeleteAsync();
                    // await unit.WorkPackageMembers.Where(i => i.RecordId == groupId).DeleteAsync();
                    // await unit.WorkPackageTaskMember.Where(i => i.RecordId == groupId).DeleteAsync();
                    // await unit.WorkingTimes.Where(i => i.GroupId == groupId).DeleteAsync();
                    // await unit.Shifts.Where(i => i.GroupId == groupId).DeleteAsync();
                    // await unit.TimeOffs.Where(i => i.GroupId == groupId).DeleteAsync();
                    // await unit.Channels.Where(i => i.Id == groupId).DeleteAsync();
                    // await unit.Conversations.Where(i => i.ChannelId == groupId).DeleteAsync();
                    // await unit.PendingInvitations.Where(i => i.RecordId == groupId).DeleteAsync();
                    //
                    // // TODO: remove chat attachments and add to plan
                    // await unit.SaveChangesAsync();
                    //
                    // await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    // {
                    //     Type = ActivityType.GroupRemove,
                    //     UserId = userId,
                    //     Group = group.ToViewModel(),
                    //     User = user.ToViewModel()
                    // });
                    //
                    // await unit.GroupMembers.Where(i => i.GroupId == groupId).DeleteAsync();
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        #endregion

        #region Access

        public async Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId,
            ChangeAccessViewModel permission)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var access = await unit.GroupMembers
                        .SingleOrDefaultAsync(i => i.Id == accessId);
                    if (access == null) return OperationResult<bool>.NotFound();
                    var accessCheck = await IsAdmin(unit, userId, access.GroupId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    access.Access = permission.Access;

                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupMemberPermission,
                        UserId = userId,
                        GroupMember = access.ToViewModel()
                    });
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ChangePendingAccess(Guid userId, Guid accessId,
            ChangeAccessViewModel permission)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var access = await unit.PendingInvitations
                        .SingleOrDefaultAsync(i => i.Id == accessId);
                    if (access == null) return OperationResult<bool>.NotFound();
                    var accessCheck = await IsAdmin(unit, userId, access.RecordId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    access.Access = permission.Access;
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupMemberPermission,
                        UserId = userId,
                        Pending = access.ToViewModel()
                    });
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> AddAccess(Guid userId, Guid groupId, AccessViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    if (!model.Members.Any()) return OperationResult<bool>.Success(false);

                    var accessCheck = await IsAdmin(unit, userId, groupId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.FindUser(userId);
                    if (user == null) return OperationResult<bool>.Rejected();

                    var existingGroupMembers = await unit.GroupMembers
                        .AsNoTracking()
                        .Where(g => g.GroupId == groupId && g.DeletedAt == null)
                        .Select(i => i.UserId.ToString())
                        .ToListAsync();

                    var existingInvites = await unit.PendingInvitations
                        .AsNoTracking()
                        .Where(i => i.RecordId == groupId && i.DeletedAt == null)
                        .Select(i => i.Identifier)
                        .ToArrayAsync();

                    existingGroupMembers.AddRange(existingInvites);

                    model.Groups = model.Groups.Where(g => !existingGroupMembers.Contains(g.Id)).ToArray();
                    model.Members = model.Members.Where(g =>
                        !existingGroupMembers.Contains(g.Id) &&
                        !existingInvites.Contains(g.Id.Trim().ToLower())
                    ).ToArray();

                    var group = await unit.Groups.AsNoTracking().SingleAsync(g => g.Id == groupId);

                    var validation = _serviceProvider.GetService<IValidateBiz>();

                    var parsed = await unit.ParseInvite(userId, validation, model.Members);
                    
                    var groupPermissions = new List<GroupMember>();
                    foreach (var invite in parsed.InviteById)
                    {
                        groupPermissions.Add(new GroupMember
                        {
                            Access = invite.Access,
                            GroupId = group.Id,
                            UserId = Guid.Parse(invite.Id),
                            RootId = groupId,
                            Level = 1
                        });
                    }

                    var pendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access,
                        Identifier = e.Id,
                        RecordId = group.Id,
                        Type = PendingInvitationType.Group
                    }).ToArray();

                    await unit.GroupMembers.AddRangeAsync(groupPermissions);
                    await unit.PendingInvitations.AddRangeAsync(pendingInvitations);
                    await unit.SaveChangesAsync();

                    Dictionary<string, string> mapped = new Dictionary<string, string>();
                    foreach (var usr in parsed.InviteById)
                    {
                        var found = parsed.AllInvited.Single(u => u.Id == Guid.Parse(usr.Id));
                        mapped.Add(usr.Id, found.Email);
                    }

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupMemberAdd,
                        UserId = userId,
                        Group = group.ToViewModel(),
                        User = user.ToViewModel(),
                        Pendings = pendingInvitations.Select(p => p.ToViewModel()).ToArray(),
                        GroupMembers = groupPermissions.Select(p =>
                        {
                            var tmp = p.ToViewModel();
                            tmp.Member = parsed.AllInvited.SingleOrDefault(u => u.Id == p.UserId);
                            return tmp;
                        }).ToArray(),
                        UserIds = groupPermissions.Select(p => p.UserId).ToArray(),
                    });

                    // TODO: send to background
// #pragma warning disable 4014
//                     Task.Run(() =>
//                         postman.InviteGroup(user.FullName, parsed.EmailIdentities, mapped, group.Id, group.Title));
// #pragma warning restore 4014
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var access = await unit.PendingInvitations
                        .SingleOrDefaultAsync(i => i.Id == accessId);
                    if (access == null) return OperationResult<bool>.NotFound();
                    var accessCheck = await IsAdmin(unit, userId, access.RecordId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var planOwnerId = await unit.Groups.Where(i => i.Id == access.RecordId)
                        .Select(i => i.UserId)
                        .SingleOrDefaultAsync();

                    var ownerGroupIds = await unit.GroupMembers
                        .Where(i => i.UserId == planOwnerId)
                        .Select(i => i.GroupId)
                        .ToArrayAsync();

                    var ownerProjectIds = await unit.ProjectMembers
                        .Where(i => i.RecordId == planOwnerId || (i.IsGroup && ownerGroupIds.Contains(i.Id)))
                        .Select(i => i.ProjectId)
                        .ToArrayAsync();

                    var ownerWorkPackageIds = await unit.WorkPackageMembers
                        .Where(i => i.RecordId == planOwnerId || (i.IsGroup && ownerGroupIds.Contains(i.Id)))
                        .Select(i => i.PackageId)
                        .ToArrayAsync();

                    var anyOtherPendingInvitations = await unit.PendingInvitations
                        .Where(i => i.Identifier == access.Identifier &&
                                    (
                                        ownerGroupIds.Contains(i.RecordId) ||
                                        ownerWorkPackageIds.Contains(i.RecordId) ||
                                        ownerProjectIds.Contains(i.RecordId)
                                    )
                        ).ToArrayAsync();

                    access.DeletedAt = null;

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupMemberRemove,
                        UserId = userId,
                        Pending = access.ToViewModel()
                    });
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var access = await unit.GroupMembers
                        .SingleOrDefaultAsync(i => i.Id == id);
                    if (access == null) return OperationResult<bool>.NotFound();
                    var accessCheck = await IsAdmin(unit, userId, access.GroupId);
                    if (access.UserId != userId && accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var group = await unit.Groups.AsNoTracking()
                        .SingleOrDefaultAsync(g => g.Id == access.GroupId);

                    if (group == null) return OperationResult<bool>.NotFound();

                    // TODO: remove access to all sub groups
                    // if (!group.ParentId.HasValue)
                    // {
                    //     var subGroupAccess = await unit.GroupMembers
                    //         .Where(i => i.UserId == access.UserId && i.RootId == group.Id)
                    //         .ToArrayAsync();
                    //     unit.GroupMembers.RemoveRange(subGroupAccess);
                    // }

                    // TODO: use linqtodb
                    // await unit.UserShifts
                    //     .Where(i => i.UserId == access.UserId && i.GroupId == access.GroupId)
                    //     .UpdateAsync(u => new UserShift{ DeletedAt = null });
                    
                    access.DeletedAt = DateTime.UtcNow;
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupMemberRemove,
                        UserId = userId,
                        GroupMember = access.ToViewModel()
                    });

                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        #endregion

        #region Edit

        public async Task<OperationResult<bool>> Upgrade(Guid userId, Guid groupId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var accessCheck = await IsAdmin(unit, userId, groupId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.FindUser(userId);
                    var group = await unit.FindGroup(groupId);
                    if (group == null) return OperationResult<bool>.NotFound();

                    if (group.Complex) return OperationResult<bool>.Duplicate();
                    
                    group.Complex = true;
                    group.UpdatedAt = DateTime.UtcNow;

                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupEdit,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Group = group.ToViewModel()
                    });

                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> Connect(Guid userId, Guid parentId, Guid groupId)
        {
            try
            {
                if (parentId == groupId) return OperationResult<bool>.Rejected();
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var permissions = await unit.GroupMembers
                        .Where(i =>
                            i.UserId == userId &&
                            i.DeletedAt == null &&
                            (i.Access == AccessType.Admin || i.Access == AccessType.Owner) &&
                            (i.GroupId == groupId || i.GroupId == parentId)
                        )
                        .AsNoTracking()
                        .ToArrayAsync();

                    if (permissions.Length != 2) return OperationResult<bool>.Rejected();

                    var groups = await unit.Groups
                        .Where(g => g.Id == parentId || g.Id == groupId)
                        .ToArrayAsync();

                    var parent = groups.Single(g => g.Id == parentId);
                    if (!parent.Complex && !parent.ParentId.HasValue) return OperationResult<bool>.Rejected();

                    var group = groups.Single(g => g.Id == groupId);
                    if (group.ParentId.HasValue) return OperationResult<bool>.Duplicate();

                    var user = await unit.FindUser(userId);

                    group.ParentId = parentId;
                    group.RootId = parent.RootId;
                    group.UpdatedAt = DateTime.UtcNow;

                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupEdit,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Group = group.ToViewModel()
                    });

                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> Edit(Guid userId, Guid groupId, GroupViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var accessCheck = await IsAdmin(unit, userId, groupId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.FindUser(userId);
                    var group = await unit.FindGroup(groupId);
                    if (group == null) return OperationResult<bool>.NotFound();

                    group.Address = model.Address?.Trim();
                    group.Description = model.Description?.Trim();
                    group.Email = string.IsNullOrEmpty(model.Email) ? null : NormalizeEmail(model.Email);
                    group.Employees = model.Employees;
                    group.Fax = model.Fax?.Trim();
                    group.Offices = model.Offices;
                    group.Tel = model.Tel?.Trim();
                    group.Title = model.Title.Trim();
                    group.Website = model.Website?.Trim();
                    group.BrandTitle = model.BrandTitle?.Trim();
                    group.GeoLocation = model.GeoLocation?.Trim();
                    group.NationalId = model.NationalId?.Trim();
                    group.PostalCode = model.PostalCode?.Trim();
                    group.RegisteredAt = model.RegisteredAt;
                    group.RegistrationId = model.RegistrationId;
                    group.ResponsibleName = model.ResponsibleName?.Trim();
                    group.ResponsibleNumber = model.ResponsibleNumber?.Trim();
                    group.SubTitle = model.SubTitle?.Trim();
                    group.SupervisorName = model.SupervisorNumber?.Trim();
                    group.SupervisorNumber = model.SupervisorNumber?.Trim();
                    group.UpdatedAt = DateTime.UtcNow;

                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupEdit,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Group = group.ToViewModel()
                    });

                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> Archive(Guid userId, Guid groupId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var accessCheck = await IsAdmin(unit, userId, groupId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.Users.AsNoTracking().SingleAsync(i => i.Id == userId);
                    var group = await unit.Groups.SingleAsync(i => i.Id == groupId);

                    if (group == null || group.ArchivedAt.HasValue) return OperationResult<bool>.Duplicate();

                    group.ArchivedAt = DateTime.UtcNow;
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupArchive,
                        UserId = userId,
                        Group = group.ToViewModel(),
                        User = user.ToViewModel()
                    });

                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> Restore(Guid userId, Guid groupId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var accessCheck = await IsAdmin(unit, userId, groupId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.Users.AsNoTracking().SingleAsync(i => i.Id == userId);
                    var group = await unit.Groups.SingleAsync(i => i.Id == groupId);

                    if (group == null || !group.ArchivedAt.HasValue) return OperationResult<bool>.Duplicate();

                    group.ArchivedAt = null;
                    await unit.SaveChangesAsync();

                    var groupMembers = await (
                        from grp in unit.Groups
                        join member in unit.GroupMembers on grp.Id equals member.GroupId
                        join usr in unit.Users on member.UserId equals usr.Id
                        where grp.Id == groupId
                        select new {Member = member, User = usr}
                    ).AsNoTracking().ToArrayAsync();

                    var result = group.ToViewModel(
                        groupMembers.Select(g => g.Member.ToViewModel(g.User)).ToArray()
                    );

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupRestore,
                        UserId = userId,
                        Group = result,
                        User = user.ToViewModel()
                    });

                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        #endregion

        #region Fetch

        public async Task<OperationResult<SelectableItem<Guid>[]>> NonAttached(Guid userId, Guid groupId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var accessCheck = await IsAdmin(unit, userId, groupId);
                    if (accessCheck.Status != OperationResultStatus.Success)
                        return OperationResult<SelectableItem<Guid>[]>.Rejected();

                    var group = await unit.FindGroup(groupId, true);
                    if (group == null) return OperationResult<SelectableItem<Guid>[]>.NotFound();

                    var groups = await unit.Groups
                        .Where(i =>
                            !i.ParentId.HasValue &&
                            !i.ArchivedAt.HasValue &&
                            !i.DeletedAt.HasValue &&
                            !i.Complex
                        )
                        .Select(g => new SelectableItem<Guid>
                        {
                            Text = g.Title,
                            Value = g.Id,
                            Payload = g.Description
                        })
                        .ToArrayAsync();

                    return OperationResult<SelectableItem<Guid>[]>.Success(groups);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<SelectableItem<Guid>[]>.Failed();
            }
        }

        public async Task<OperationResult<GroupViewModel[]>> List(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var user = await unit.FindUser(userId);
                    if (user == null) return OperationResult<GroupViewModel[]>.Rejected();

                    var groups = await unit.FindGroups(userId);
                    if (!groups.Any()) return OperationResult<GroupViewModel[]>.Success(Array.Empty<GroupViewModel>());

                    var groupIds = groups.Select(i => i.Id).ToArray();
                    var groupMembers = await (
                            from member in unit.GroupMembers
                            join usr in unit.Users on member.UserId equals usr.Id
                            where
                                !member.DeletedAt.HasValue &&
                                !user.DeletedAt.HasValue &&
                                groupIds.Contains(member.GroupId)
                            select new {User = usr, Member = member}
                        )
                        .AsNoTracking()
                        .ToArrayAsync();

                    var pendingMembers = await (
                            from member in unit.PendingInvitations
                            where groupIds.Contains(member.RecordId)
                            select member
                        )
                        .AsNoTracking()
                        .ToArrayAsync();

                    var result = groups.Select(g =>
                    {
                        var currentAccess = groupMembers
                            .Single(m => m.Member.GroupId == g.Id && m.User.Id == userId)
                            .Member.Access;

                        var pendings = pendingMembers
                            .Where(p =>  p.RecordId == g.Id &&
                            (
                                (p.Access != AccessType.HiddenEditor) ||
                                (currentAccess <= AccessType.HiddenEditor)
                            ))
                            .Select(p => p.ToViewModel())
                            .ToArray();
                        var members = groupMembers
                            .Where(m => m.Member.GroupId == g.Id &&
                            (
                                (m.Member.Access != AccessType.HiddenEditor) ||
                                (currentAccess <= AccessType.HiddenEditor)
                            ))
                            .OrderByDescending(m => m.Member.CreatedAt)
                            .Select(x => x.Member.ToViewModel(x.User))
                            .ToArray();
                        return g.ToViewModel(members, pendings);
                    }).ToArray();
                    return OperationResult<GroupViewModel[]>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<GroupViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<GroupViewModel[]>> Archived(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var user = await unit.FindUser(userId);
                    if (user == null) return OperationResult<GroupViewModel[]>.Rejected();

                    var groups = await (
                        from member in unit.GroupMembers
                        join grp in unit.Groups on member.GroupId equals grp.Id
                        where member.UserId == userId &&
                              grp.ArchivedAt.HasValue &&
                              !grp.DeletedAt.HasValue &&
                              !member.DeletedAt.HasValue &&
                              (member.Access == AccessType.Owner || member.Access == AccessType.Admin)
                        select grp
                    ).Distinct().AsNoTracking().ToArrayAsync();

                    if (!groups.Any()) return OperationResult<GroupViewModel[]>.Success(Array.Empty<GroupViewModel>());

                    var groupIds = groups.Select(i => i.Id).ToArray();
                    var groupMembers = await (
                            from member in unit.GroupMembers
                            join usr in unit.Users on member.UserId equals usr.Id
                            where
                                !member.DeletedAt.HasValue &&
                                !user.DeletedAt.HasValue &&
                                groupIds.Contains(member.GroupId)
                            select new {User = usr, Member = member}
                        )
                        .AsNoTracking()
                        .ToArrayAsync();

                    var pendingMembers = await (
                            from member in unit.PendingInvitations
                            where groupIds.Contains(member.RecordId)
                            select member
                        )
                        .AsNoTracking()
                        .ToArrayAsync();

                    var result = groups.Select(g =>
                    {
                        var currentAccess = groupMembers
                            .Single(m => m.Member.GroupId == g.Id && m.User.Id == userId)
                            .Member.Access;

                        var pendings = pendingMembers
                            .Where(p =>
                                p.RecordId == g.Id &&
                                (
                                    (p.Access != AccessType.HiddenEditor) ||
                                    (currentAccess <= AccessType.HiddenEditor)
                                )
                            )
                            .Select(p => p.ToViewModel())
                            .ToArray();
                        var members = groupMembers
                            .Where(m => m.Member.GroupId == g.Id &&
                                        (
                                            (m.Member.Access != AccessType.HiddenEditor) ||
                                            (currentAccess <= AccessType.HiddenEditor)
                                        ))
                            .OrderByDescending(m => m.Member.CreatedAt)
                            .Select(x => x.Member.ToViewModel(x.User))
                            .ToArray();
                        return g.ToViewModel(members, pendings);
                    }).ToArray();
                    return OperationResult<GroupViewModel[]>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<GroupViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<GroupViewModel>> Fetch(Guid userId, Guid groupId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleAsync(i => i.Id == userId);
                    if (user == null || user.Blocked || user.IsLocked)
                        return OperationResult<GroupViewModel>.Rejected();
                    
                    var access = await unit.GroupMembers
                        .Where(i => i.GroupId == groupId && i.UserId == userId && i.DeletedAt == null)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    if (access == null) return OperationResult<GroupViewModel>.Rejected();
                    
                    var group = await unit.Groups.AsNoTracking().SingleAsync(i => i.Id == groupId);
                    if (group.ArchivedAt.HasValue &&
                        !(access.Access == AccessType.Admin || access.Access == AccessType.Owner))
                        return OperationResult<GroupViewModel>.Rejected();

                    var groupMembers = await (
                        from member in unit.GroupMembers
                        join usr in unit.Users on member.UserId equals usr.Id
                        where member.GroupId == groupId
                        select new {Member = member, User = usr}
                    ).AsNoTracking().ToArrayAsync();

                    var result = group.ToViewModel(
                        groupMembers.Select(g => g.Member.ToViewModel(g.User)).ToArray()
                    );
                    return OperationResult<GroupViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<GroupViewModel>.Failed();
            }
        }

        public async Task<OperationResult<DashBoardProgressViewModel[]>> Report(Guid userId, Guid groupId,
            DurationViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<DashBoardProgressViewModel[]>.Rejected();

                    var packages = await unit.WorkPackageMembers
                        .Where(i => i.RecordId == groupId)
                        .Select(i => i.PackageId)
                        .ToArrayAsync();

                    var report = await (
                        from task in unit.WorkPackageTasks
                        join activity in unit.Activities on task.Id equals activity.RecordId
                        where packages.Contains(task.PackageId) &&
                              (activity.CreatedAt >= model.Begin && activity.CreatedAt <= model.End)
                        select activity
                    ).AsNoTracking().ToArrayAsync();

                    var result = report
                        .GroupBy(i => i.CreatedAt.Date)
                        .Select(i =>
                        {
                            var blocked = i.Count(y => y.Type == ActivityType.WorkPackageTaskBlocked);
                            var unBlocked = i.Count(y => y.Type == ActivityType.WorkPackageTaskUnBlock);
                            var done = i.Count(y => y.Type == ActivityType.WorkPackageTaskDone);
                            var created = i.Count(y => y.Type == ActivityType.WorkPackageTaskAdd);
                            return new DashBoardProgressViewModel
                            {
                                Date = i.Key,
                                Blocked = blocked,
                                Done = done + unBlocked,
                                Total = created
                            };
                        }).ToArray();
                    return OperationResult<DashBoardProgressViewModel[]>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<DashBoardProgressViewModel[]>.Failed();
            }
        }

        #endregion

        #region TimeOff

        public async Task<OperationResult<GridResult<TimeOffViewModel>>> TimeOffs(
            Guid userId, Guid groupId, GridFilter model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var permission = await unit.GroupMembers
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.GroupId == groupId && i.UserId == userId && i.DeletedAt == null);

                    if (permission == null) return OperationResult<GridResult<TimeOffViewModel>>.Rejected();
                    var isAdmin = permission.Access == AccessType.Admin || permission.Access == AccessType.Owner;
                    var query = unit.TimeOffs
                        .Where(i => i.GroupId == groupId && (isAdmin || i.UserId == userId) && i.DeletedAt == null)
                        .OrderByDescending(i => i.CreatedAt);
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
                return OperationResult<GridResult<TimeOffViewModel>>.Failed();
            }
        }
        public async Task<OperationResult<GridResult<TimeOffViewModel>>> TimeOffHistory(
            Guid userId, Guid groupId, GridFilterWithParams<IdViewModel> filter)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var permission = await unit.GroupMembers
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.GroupId == groupId && i.UserId == userId && i.DeletedAt == null);
                    
                    if (permission == null) return OperationResult<GridResult<TimeOffViewModel>>.Rejected();
                    var isAdmin = permission.Access == AccessType.Admin || permission.Access == AccessType.Owner;
                    if (!isAdmin) return OperationResult<GridResult<TimeOffViewModel>>.Rejected();
                    
                    var query = unit.TimeOffs
                        .Where(i => i.GroupId == groupId && i.UserId == filter.Params.Id && i.DeletedAt == null)
                        .OrderByDescending(i => i.CreatedAt);
                    return await DbHelper.GetPaginatedData(query, tuple =>
                    {
                        var (items, startIndex) = tuple;
                        return items.Select((i, index) =>
                        {
                            var vm = i.ToViewModel();
                            vm.Index = startIndex + index + 1;
                            return vm;
                        }).ToArray();
                    }, filter.Page, filter.PageSize);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<GridResult<TimeOffViewModel>>.Failed();
            }
        }

        public async Task<OperationResult<bool>> RequestTimeOff(Guid userId, Guid id, RequestTimeOffViewModel model)
        {
            try
            {
                if (model.EndAt <= model.BeginAt) return OperationResult<bool>.Validation();
                
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var access = await unit.GroupMembers
                        .AnyAsync(m => m.UserId == userId && m.GroupId == id);
                    if (!access) return OperationResult<bool>.Rejected();
                    var already = await unit.TimeOffs.Where(i =>
                        i.UserId == userId &&
                        i.Status == RequestStatus.Pending &&
                        i.BeginAt == model.BeginAt &&
                        i.EndAt == model.EndAt
                    ).AnyAsync();
                    if (already) return OperationResult<bool>.Duplicate();
                    var timeOff = new TimeOff
                    {
                        Description = model.Description,
                        Status = RequestStatus.Pending,
                        BeginAt = model.BeginAt,
                        EndAt = model.EndAt,
                        GroupId = id,
                        IsHourly = model.IsHourly,
                        UserId = userId
                    };
                    await unit.TimeOffs.AddAsync(timeOff);
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

        public async Task<OperationResult<bool>> TimeOffResponse(Guid userId, Guid id, bool approve)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var timeOff = await unit.TimeOffs
                        .SingleOrDefaultAsync(i => i.Id == id && i.Status == RequestStatus.Pending);
                    if (timeOff == null) return OperationResult<bool>.Rejected();
                    var access = await unit.GroupMembers.AnyAsync(i =>
                        i.UserId == userId &&
                        i.GroupId == timeOff.GroupId &&
                        (i.Access == AccessType.Admin || i.Access == AccessType.Owner)
                    );
                    if (!access) return OperationResult<bool>.Rejected();
                    timeOff.Status = approve ? RequestStatus.Approved : RequestStatus.Canceled;
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

        public async Task<OperationResult<TimeOffDetailViewModel>> TimeOffDetail(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var timeOff = await unit.TimeOffs
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.Id == id && i.Status == RequestStatus.Pending);
                    if (timeOff == null) return OperationResult<TimeOffDetailViewModel>.Rejected();
                    var access = await unit.GroupMembers.AnyAsync(i =>
                        i.UserId == userId &&
                        i.GroupId == timeOff.GroupId &&
                        (i.Access == AccessType.Admin || i.Access == AccessType.Owner)
                    );
                    if (!access) return OperationResult<TimeOffDetailViewModel>.Rejected();

                    var packageIds = await unit.WorkPackageMembers
                        .Where(i => i.RecordId == timeOff.GroupId)
                        .Select(i => i.PackageId)
                        .ToArrayAsync();

                    var tasks = await (from task in unit.WorkPackageTasks
                        join member in unit.WorkPackageTaskMember on task.Id equals member.TaskId
                        where packageIds.Contains(task.PackageId) &&
                              (member.RecordId == timeOff.UserId || member.RecordId == timeOff.GroupId) &&
                              (
                                  task.State != WorkPackageTaskState.Done &&
                                  task.State != WorkPackageTaskState.Canceled &&
                                  task.State != WorkPackageTaskState.Duplicate
                              ) && (
                                (task.DueAt.HasValue && (task.DueAt.Value >= timeOff.BeginAt && task.DueAt.Value <= timeOff.EndAt)) ||
                                (task.BeginAt.HasValue && (task.BeginAt.Value >= timeOff.BeginAt && task.BeginAt.Value <= timeOff.EndAt)) ||
                                (task.EndAt.HasValue && (task.EndAt.Value >= timeOff.BeginAt && task.EndAt.Value <= timeOff.EndAt))
                              )
                        select task).ToArrayAsync();

                    return OperationResult<TimeOffDetailViewModel>.Success(new TimeOffDetailViewModel
                    {
                        Tasks = tasks.Select(t => t.ToViewModel()).ToArray()
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<TimeOffDetailViewModel>.Failed();
            }
        }


        public async Task<OperationResult<bool>> RemoveTimeOff(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var timeOff = await unit.TimeOffs
                        .SingleOrDefaultAsync(i => i.Id == id && i.Status == RequestStatus.Pending);
                    if (timeOff == null) return OperationResult<bool>.Rejected();
                    
                    var permission = await unit.GroupMembers
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.UserId == userId && i.GroupId == timeOff.GroupId  && i.DeletedAt == null);
                    if (permission == null) return OperationResult<bool>.Rejected();
                    
                    var isAdmin = permission.Access == AccessType.Admin || permission.Access == AccessType.Owner;
                    if (timeOff.UserId != userId && !isAdmin) return OperationResult<bool>.Rejected();

                    timeOff.DeletedAt = DateTime.UtcNow;
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
        #endregion

        #region Entry

        public async Task<OperationResult<GridResult<EntryLogViewModel>>> EntryLogs(Guid userId, Guid id,
            GridFilter model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var permission = await unit.GroupMembers
                        .Where(i => i.GroupId == id && i.UserId == userId && i.DeletedAt == null)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();
                    if (permission == null) return OperationResult<GridResult<EntryLogViewModel>>.Rejected();
                    var isAdmin = permission.Access == AccessType.Admin || permission.Access == AccessType.Owner;

                    var query = from wt in unit.WorkingTimes
                        join user in unit.Users on wt.UserId equals user.Id
                        orderby wt.BeginAt descending
                        where wt.GroupId == id && (isAdmin || wt.UserId == userId) && wt.DeletedAt == null
                        select new {User = user, Entry = wt};

                    return await DbHelper.GetPaginatedData(query, tuple =>
                    {
                        var (items, startIndex) = tuple;
                        return items.Select((i, index) =>
                        {
                            var vm = i.Entry.ToViewModel(i.User.FullName);
                            vm.Index = startIndex + index + 1;
                            return vm;
                        }).ToArray();
                    }, model.Page, model.PageSize);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<GridResult<EntryLogViewModel>>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ToggleEntry(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user.IsLocked || user.Blocked) return OperationResult<bool>.Rejected();

                    var found = await (
                        from grp in unit.Groups
                        join member in unit.GroupMembers on grp.Id equals member.GroupId
                        where 
                            member.DeletedAt == null &&
                            !grp.ArchivedAt.HasValue && 
                            !grp.DeletedAt.HasValue && 
                            grp.Id == id && 
                            member.UserId == userId
                        select new {Group = grp, Member = member}
                    ).AsNoTracking().SingleOrDefaultAsync();

                    if (found == null) return OperationResult<bool>.Rejected();

                    // TODO: use linqtodb
                    // await unit.WorkingTimes.Where(i =>
                    //     i.GroupId != id &&
                    //     i.UserId == userId &&
                    //     i.EndAt == null
                    // ).UpdateAsync(u => new WorkingTime {EndAt = DateTime.UtcNow});
                    
                    var entry = await unit.WorkingTimes
                        .Where(i => i.GroupId == id && i.UserId == userId)
                        .OrderByDescending(i => i.BeginAt)
                        .FirstOrDefaultAsync();

                    WorkingTime result = entry;
                    if (entry == null || entry.EndAt.HasValue)
                    {
                        result = new WorkingTime
                        {
                            BeginAt = DateTime.UtcNow,
                            GroupId = id,
                            UserId = userId
                        };
                        await unit.WorkingTimes.AddAsync(result);
                    }
                    else
                    {
                        entry.EndAt = DateTime.UtcNow;
                    }

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.GroupWorkEntry,
                        UserId = userId,
                        Group = found.Group.ToViewModel(),
                        WorkEntry = result.ToViewModel(user.FullName),
                        User = user.ToViewModel()
                    });

                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> RemoveEntry(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var entry = await unit.WorkingTimes
                        .SingleOrDefaultAsync(i => i.Id == id);
                    if (entry == null) return OperationResult<bool>.Rejected();
                    
                    var permission = await unit.GroupMembers
                        .Where(i => 
                            i.GroupId == entry.GroupId && 
                            i.UserId == userId && 
                            i.DeletedAt == null
                        )
                        .AsNoTracking()
                        .SingleOrDefaultAsync();
                    if (permission == null) return OperationResult<bool>.Rejected();
                    
                    var isAdmin = permission.Access == AccessType.Admin || permission.Access == AccessType.Owner;
                    if (!isAdmin && entry.UserId != userId) return OperationResult<bool>.Rejected();
                    
                    entry.DeletedAt = DateTime.UtcNow;
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

        public async Task<OperationResult<bool>> EditEntry(Guid userId, Guid id, OptionalDurationViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var entry = await unit.WorkingTimes
                        .SingleOrDefaultAsync(i => i.Id == id);
                    if (entry == null) return OperationResult<bool>.Rejected();
                    
                    var permission = await unit.GroupMembers
                        .Where(i => 
                            i.GroupId == entry.GroupId && 
                            i.UserId == userId && 
                            i.DeletedAt == null
                        )
                        .AsNoTracking()
                        .SingleOrDefaultAsync();
                    if (permission == null) return OperationResult<bool>.Rejected();
                    
                    var isAdmin = permission.Access == AccessType.Admin || permission.Access == AccessType.Owner;

                    if (!isAdmin)
                    {
                        // None admins can edit up to 24 hours
                        if (entry.UserId != userId) return OperationResult<bool>.Rejected();
                        var maxAllowed = DateTime.UtcNow.AddDays(-1);
                        if (model.Begin < maxAllowed || model.End < maxAllowed) 
                            return OperationResult<bool>.Rejected();
                    }
                    
                    entry.BeginAt = model.Begin;
                    entry.EndAt = model.End;
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

        public async Task<OperationResult<bool>> ManualEntry(Guid userId, Guid groupId, ManualEntryViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var access = await IsAdmin(unit, userId, groupId);
                    if (access.Status != OperationResultStatus.Success) return OperationResult<bool>.Rejected();
                    var member = await unit.GroupMembers.AnyAsync(i => 
                        i.UserId == model.UserId && 
                        i.GroupId == groupId && 
                        i.DeletedAt == null
                    );
                    if (!member) return OperationResult<bool>.Rejected();
                    await unit.WorkingTimes.AddAsync(new WorkingTime
                    {
                        BeginAt = model.Begin,
                        EndAt = model.End,
                        GroupId = groupId,
                        UserId = model.UserId
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

        #endregion

        #region Shift

        public async Task<OperationResult<bool>> CreateShift(Guid userId, Guid groupId, EditShiftViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var access = await IsAdmin(unit, userId, groupId);
                    if (access.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    switch (model.Type)
                    {
                        case ShiftType.Open:
                            model.End = null;
                            model.Start = null;
                            model.Float = null;
                            break;
                        case ShiftType.Fixed:
                            model.Float = null;
                            break;
                    }

                    var shift = new Shift
                    {
                        Description = model.Description,
                        End = model.End,
                        Float = model.Float,
                        Start = model.Start,
                        Title = model.Title,
                        Type = model.Type,
                        GroupId = groupId,
                        PenaltyRate = model.PenaltyRate,
                        RestHours = model.RestHours,
                        RewardRate = model.RewardRate,
                        UserId = userId,
                        WorkingHours = model.WorkingHours,
                        Saturday = model.Saturday,
                        Sunday = model.Sunday,
                        Monday = model.Monday,
                        Tuesday = model.Tuesday,
                        Wednesday = model.Wednesday,
                        Thursday = model.Thursday,
                        Friday = model.Friday
                    };
                    await unit.Shifts.AddAsync(shift);
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

        public async Task<OperationResult<bool>> EditShift(Guid userId, Guid shiftId, EditShiftViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var shift = await unit.Shifts.SingleOrDefaultAsync(i => i.Id == shiftId);
                    if (shift == null) return OperationResult<bool>.NotFound();
                    var access = await IsAdmin(unit, userId, shift.GroupId);
                    if (access.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    switch (model.Type)
                    {
                        case ShiftType.Open:
                            model.End = null;
                            model.Start = null;
                            model.Float = null;
                            break;
                        case ShiftType.Fixed:
                            model.Float = null;
                            break;
                    }

                    shift.Description = model.Description;
                    shift.End = model.End;
                    shift.Float = model.Float;
                    shift.Start = model.Start;
                    shift.Title = model.Title;
                    shift.Type = model.Type;
                    shift.PenaltyRate = model.PenaltyRate;
                    shift.RestHours = model.RestHours;
                    shift.RewardRate = model.RewardRate;
                    shift.WorkingHours = model.WorkingHours;
                    shift.Saturday = model.Saturday;
                    shift.Sunday = model.Sunday;
                    shift.Monday = model.Monday;
                    shift.Tuesday = model.Tuesday;
                    shift.Wednesday = model.Wednesday;
                    shift.Thursday = model.Thursday;
                    shift.Friday = model.Friday;

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

        public async Task<OperationResult<bool>> DeleteShift(Guid userId, Guid shiftId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var shift = await unit.Shifts.AsNoTracking().SingleOrDefaultAsync(i => i.Id == shiftId);
                    if (shift == null) return OperationResult<bool>.NotFound();
                    var access = await IsAdmin(unit, userId, shift.GroupId);
                    if (access.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();
                    
                    var now = DateTime.UtcNow;
                    // TODO: use linqtodb
                    // await unit.Shifts.Where(i => i.Id == shiftId)
                    //     .UpdateAsync(u => new Shift{ DeletedAt = now});
                    // await unit.UserShifts.Where(i => i.ShiftId == shiftId)
                    //     .UpdateAsync(u => new UserShift{ DeletedAt = now});
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<GridResult<ShiftViewModel>>> Shifts(Guid userId, Guid groupId,
            GridFilter model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var query = unit.Shifts
                        .Where(i => i.GroupId == groupId && i.DeletedAt == null)
                        .OrderByDescending(i => i.CreatedAt);
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
                return OperationResult<GridResult<ShiftViewModel>>.Failed();
            }
        }

        #endregion

        #region Setting

        public async Task<OperationResult<bool>> Export(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var accessCheck = await IsAdmin(unit, userId, id);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    // TODO: implement group export
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        #endregion

        #region Private

        private string NormalizeEmail(string email)
        {
            return email.Trim().ToLower();
        }

        private async Task<OperationResult<bool>> IsAdmin(CollaborationDbContext unit, Guid userId, Guid groupId)
        {
            var access = await (
                from grp in unit.Groups
                join mbr in unit.GroupMembers on grp.Id equals mbr.GroupId
                where mbr.UserId == userId && grp.Id == groupId && mbr.DeletedAt == null &&
                      (mbr.Access == AccessType.Admin || mbr.Access == AccessType.Owner)
                select mbr
            ).AnyAsync();
            
            
            // TODO: check permission on sub groups
            
            // var group = await unit.Groups
            //     .Where(i => i.Id == groupId)
            //     .SingleOrDefaultAsync();
            // if (group == null) return OperationResult<bool>.Rejected();
            // var access = await unit.GroupMembers.AnyAsync(m =>
            //     m.RootId == group.RootId &&
            //     m.UserId == userId &&
            //     (m.Access == AccessType.Admin || m.Access == AccessType.Owner)
            // );
            return access ? OperationResult<bool>.Success() : OperationResult<bool>.Rejected();
        }

        #endregion
    }
}