using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.ProjectManagement;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Import.Taskulu;
using Asoode.Core.ViewModels.Logging;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Z.EntityFramework.Plus;
using Task = System.Threading.Tasks.Task;

namespace Asoode.Business.ProjectManagement
{
    internal class WorkPackageBiz : IWorkPackageBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkPackageBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        #region Create

        public async Task<OperationResult<bool>> CreateWorkPackage(Guid userId, Guid projectId,
            CreateWorkPackageViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsProjectAdmin(unit, userId, projectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var validation = _serviceProvider.GetService<IValidateBiz>();
                    var translate = _serviceProvider.GetService<ITranslateBiz>();

                    var user = await unit.Users
                        .AsNoTracking()
                        .SingleAsync(i => i.Id == userId);

                    var project = await unit.Projects.AsNoTracking().SingleAsync(i => i.Id == projectId);

                    var plan = await unit.UserPlanInfo.FirstAsync(g => g.Id == project.PlanInfoId);

                    if (plan.UsedWorkPackage >= plan.WorkPackage) return OperationResult<bool>.OverCapacity();
                    if (model.Members.Length > plan.Users) return OperationResult<bool>.OverCapacity();

                    var season = model.ParentId.HasValue && await unit.SubProjects
                        .Where(s => s.ProjectId == projectId && s.Id == model.ParentId.Value)
                        .AnyAsync();

                    var parsed = await unit.ParseInvite(userId, plan, validation, model.Members);
                    if ((plan.UsedUser + parsed.NewMembers.Length) > plan.Users) return OperationResult<bool>.OverCapacity();

                    foreach (var newMember in parsed.NewMembers)
                    {
                        await unit.PlanMembers.AddAsync(new PlanMember
                        {
                            Identifier = newMember,
                            PlanId = plan.Id,
                        });
                    }

                    plan.UsedUser += parsed.NewMembers.Length;
                    plan.UsedWorkPackage++;

                    var projectMembers = await unit.ProjectMembers
                        .Where(i => i.ProjectId == projectId)
                        .Select(i => i.RecordId)
                        .ToArrayAsync();

                    var freePlan = await unit.Plans
                        .AsNoTracking()
                        .SingleAsync(i => i.Type == PlanType.Free);

                    var alreadyCreated = await unit.Projects
                        .AsNoTracking()
                        .Where(i => i.PlanInfoId == plan.Id && !i.Complex)
                        .CountAsync();
                    
                    var groupNames = new List<string>();
                    var workPackageLabels = new WorkPackageLabel[0];
                    var workPackageLists = new List<WorkPackageList>();
                    var workPackage = new WorkPackage
                    {
                        Description = model.Description.Trim(),
                        Title = model.Title.Trim(),
                        UserId = userId,
                        CommentPermission = WorkPackageCommentPermission.Members,
                        ProjectId = project.Id,
                        AllowAttachment = true,
                        AllowComments = true,
                        AllowLabels = true,
                        AllowMembers = true,
                        AllowPoll = true,
                        AllowSegments = true,
                        AllowState = true,
                        AllowCustomField = true,
                        AllowEndAt = true,
                        AllowEstimatedTime = true,
                        AllowGeoLocation = true,
                        AllowTimeSpent = true,
                        AllowBlockingBoardTasks = true,
                        Color = AsoodeColors.Default.Value,
                        DarkColor = AsoodeColors.Default.Dark,
                        SubProjectId = season ? model.ParentId : null,
                        Premium = alreadyCreated >= freePlan.WorkPackage
                    };


                    var workPackageMembers = new List<WorkPackageMember>
                    {
                        new WorkPackageMember
                        {
                            PackageId = workPackage.Id,
                            Access = AccessType.Owner,
                            IsGroup = false,
                            RecordId = userId,
                            ProjectId = project.Id
                        }
                    };
                    var channel = new Channel
                    {
                        Title = model.Title,
                        Type = ChannelType.WorkPackage,
                        UserId = userId,
                        Id = workPackage.Id
                    };
                    if (model.Groups.Any())
                    {
                        var groupAccess = await (
                            from member in unit.GroupMembers
                            join grp in unit.Groups on member.GroupId equals grp.Id
                            where member.UserId == userId && !grp.ArchivedAt.HasValue && !member.DeletedAt.HasValue
                            select grp
                        ).ToArrayAsync();

                        foreach (var grp in model.Groups)
                        {
                            var groupId = Guid.Parse(grp.Id);
                            var group = groupAccess.SingleOrDefault(i => i.Id == groupId);
                            if (group == null) continue;
                            workPackageMembers.Add(new WorkPackageMember()
                            {
                                Access = grp.Access,
                                PackageId = workPackage.Id,
                                IsGroup = true,
                                RecordId = groupId,
                                ProjectId = workPackage.ProjectId
                            });
                            groupNames.Add(group.Title);
                        }
                    }

                    foreach (var invite in parsed.InviteById)
                    {
                        workPackageMembers.Add(new WorkPackageMember()
                        {
                            Access = invite.Access,
                            PackageId = workPackage.Id,
                            RecordId = Guid.Parse(invite.Id),
                            IsGroup = false,
                            ProjectId = workPackage.ProjectId
                        });
                    }

                    var newProjectMembers = workPackageMembers
                        .Where(pm => !projectMembers.Contains(pm.RecordId))
                        .Select(m => new ProjectMember
                        {
                            Access = m.Access == AccessType.Visitor ? AccessType.Visitor : AccessType.Editor,
                            IsGroup = m.IsGroup,
                            ProjectId = workPackage.ProjectId,
                            RecordId = m.RecordId
                        }).ToArray();

                    var pendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access,
                        Identifier = e.Id,
                        RecordId = workPackage.Id,
                        Type = PendingInvitationType.WorkPackage
                    }).ToArray();

                    workPackageLabels = AsoodeColors.Plate
                        .Select(c => new WorkPackageLabel
                        {
                            Color = c.Value,
                            DarkColor = c.Dark,
                            PackageId = workPackage.Id,
                        }).ToArray();

                    if (model.BoardTemplate.HasValue && model.BoardTemplate.Value != BoardTemplate.Blank)
                    {
                        switch (model.BoardTemplate.Value)
                        {
                            case BoardTemplate.Departments:
                                workPackageLists.AddRange(new[]
                                {
                                    new WorkPackageList
                                    {
                                        Order = 1,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_1"),
                                        PackageId = workPackage.Id,
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 2,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_2"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 3,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_3"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 4,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_4"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 5,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_5"),
                                        PackageId = workPackage.Id
                                    },
                                });
                                break;
                            case BoardTemplate.Kanban:
                                workPackageLists.AddRange(new[]
                                {
                                    new WorkPackageList
                                    {
                                        Order = 1,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_1"),
                                        PackageId = workPackage.Id,
                                        Kanban = WorkPackageTaskState.ToDo
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 2,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_2"),
                                        PackageId = workPackage.Id,
                                        Kanban = WorkPackageTaskState.InProgress
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 3,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_3"),
                                        PackageId = workPackage.Id,
                                        Kanban = WorkPackageTaskState.Done
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 4,
                                        Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_4"),
                                        PackageId = workPackage.Id,
                                        Kanban = WorkPackageTaskState.Canceled
                                    },
                                });
                                break;
                            case BoardTemplate.WeekDay:
                                workPackageLists.AddRange(new[]
                                {
                                    new WorkPackageList
                                    {
                                        Order = 1,
                                        Title = translate.Get("ENUMS_WEEKDAY_SATURDAY"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 2,
                                        Title = translate.Get("ENUMS_WEEKDAY_SUNDAY"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 3,
                                        Title = translate.Get("ENUMS_WEEKDAY_MONDAY"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 4,
                                        Title = translate.Get("ENUMS_WEEKDAY_TUESDAY"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 5,
                                        Title = translate.Get("ENUMS_WEEKDAY_WEDNESDAY"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 6,
                                        Title = translate.Get("ENUMS_WEEKDAY_THURSDAY"),
                                        PackageId = workPackage.Id
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 7,
                                        Title = translate.Get("ENUMS_WEEKDAY_FRIDAY"),
                                        PackageId = workPackage.Id
                                    },
                                });
                                break;
                            case BoardTemplate.TeamMembers:
                                var counter = 0;
                                foreach (var fullName in parsed.AllInvited.Select(i => i.FullName))
                                {
                                    workPackageLists.Add(new WorkPackageList
                                    {
                                        Order = ++counter,
                                        Title = fullName,
                                        PackageId = workPackage.Id
                                    });
                                }

                                foreach (var groupName in groupNames)
                                {
                                    workPackageLists.Add(new WorkPackageList
                                    {
                                        Order = ++counter,
                                        Title = groupName,
                                        PackageId = workPackage.Id
                                    });
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (project.Template)
                        {
                            case ProjectTemplate.Animation:
                                if (model.BoardTemplate == BoardTemplate.Blank)
                                {
                                    workPackageLists.AddRange(new[]
                                    {
                                        new WorkPackageList
                                        {
                                            Order = 1,
                                            Title = translate.Get("TEMPLATE_ANIMATION_EP_STORY_REEL"),
                                            PackageId = workPackage.Id
                                        },
                                        new WorkPackageList
                                        {
                                            Order = 2,
                                            Title = translate.Get("TEMPLATE_ANIMATION_EP_ANIMATE"),
                                            PackageId = workPackage.Id
                                        },
                                        new WorkPackageList
                                        {
                                            Order = 3,
                                            Title = translate.Get("TEMPLATE_ANIMATION_EP_COMPOSITE"),
                                            PackageId = workPackage.Id
                                        },
                                        new WorkPackageList
                                        {
                                            Order = 4,
                                            Title = translate.Get("TEMPLATE_ANIMATION_EP_EDITING"),
                                            PackageId = workPackage.Id
                                        }
                                    });
                                }

                                break;
                            case ProjectTemplate.None:
                                break;
                        }
                    }

                    // TODO: send welcome messages in chat

                    await unit.PendingInvitations.AddRangeAsync(parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access == AccessType.Visitor ? AccessType.Visitor : AccessType.Editor,
                        Identifier = e.Id,
                        RecordId = workPackage.ProjectId,
                        Type = PendingInvitationType.Project
                    }));
                    await unit.PendingInvitations.AddRangeAsync(pendingInvitations);
                    await unit.WorkPackageLabels.AddRangeAsync(workPackageLabels);
                    await unit.WorkPackages.AddAsync(workPackage);
                    await unit.ProjectMembers.AddRangeAsync(newProjectMembers);
                    await unit.WorkPackageMembers.AddRangeAsync(workPackageMembers);
                    await unit.WorkPackageLists.AddRangeAsync(workPackageLists);
                    await unit.Channels.AddAsync(channel);
                    await unit.SaveChangesAsync();

                    Dictionary<string, string> mapped = new Dictionary<string, string>();
                    foreach (var usr in parsed.InviteById)
                    {
                        var found = parsed.AllInvited.Single(u => u.Id == Guid.Parse(usr.Id));
                        mapped.Add(usr.Id, found.Email);
                    }

                    var viewModel = workPackage.ToViewModel(
                        workPackageMembers.Select(m => m.ToViewModel()).ToArray(),
                        pendingInvitations.Select(p => p.ToViewModel()).ToArray()
                    );
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageAdd,
                        UserId = userId,
                        WorkPackage = viewModel,
                        User = user.ToViewModel()
                    });

                    var postman = _serviceProvider.GetService<IPostmanBiz>();
#pragma warning disable 4014
                    Task.Run(() => postman.InviteWorkPackage(user.FullName, parsed.EmailIdentities, mapped, viewModel));
#pragma warning restore 4014
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

        public async Task<OperationResult<bool>> Leave(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.WorkPackageMembers
                        .SingleOrDefaultAsync(i => i.RecordId == userId && i.PackageId == id);
                    if (access == null) return OperationResult<bool>.Rejected();

                    var project = await unit.Projects.SingleOrDefaultAsync(p => p.Id == access.ProjectId);
                    if (!project.Complex)
                    {
                        var projAccess = await unit.ProjectMembers.SingleOrDefaultAsync(i =>
                            i.ProjectId == project.Id && i.RecordId == userId);
                        if (projAccess != null) unit.ProjectMembers.Remove(projAccess);
                    }

                    unit.WorkPackageMembers.Remove(access);
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageMemberRemove,
                        UserId = userId,
                        WorkPackageMember = access.ToViewModel()
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

        public async Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid accessId,
            ChangeAccessViewModel permission)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.WorkPackageMembers
                        .SingleOrDefaultAsync(i => i.Id == accessId);

                    if (access == null) return OperationResult<bool>.NotFound();
                    var checkAccess = await IsWorkPackageAdmin(unit, userId, access.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;
                    access.Access = permission.Access;

                    var project = await unit.Projects.AsNoTracking()
                        .SingleAsync(i => i.Id == access.ProjectId);

                    if (!project.Complex)
                    {
                        var projectAccess = await unit.ProjectMembers
                            .SingleOrDefaultAsync(i =>
                                i.ProjectId == access.ProjectId && i.RecordId == access.RecordId);
                        if (projectAccess != null) projectAccess.Access = permission.Access;
                    }

                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageMemberPermission,
                        UserId = userId,
                        WorkPackageMember = access.ToViewModel()
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
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.PendingInvitations
                        .SingleOrDefaultAsync(i => i.Id == accessId);
                    if (access == null) return OperationResult<bool>.NotFound();
                    var accessCheck = await IsWorkPackageAdmin(unit, userId, access.RecordId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    access.Access = permission.Access;

                    var project = await unit.Projects.AsNoTracking()
                        .SingleAsync(i => i.Id == access.RecordId);

                    if (!project.Complex)
                    {
                        var projectAccess = await unit.PendingInvitations
                            .SingleOrDefaultAsync(i =>
                                i.RecordId == access.RecordId && i.Identifier == access.Identifier);
                        if (projectAccess != null) projectAccess.Access = permission.Access;
                    }

                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageMemberPermission,
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


        public async Task<OperationResult<bool>> RemoveAccess(Guid userId, Guid accessId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.WorkPackageMembers
                        .SingleOrDefaultAsync(i => i.Id == accessId);

                    if (access == null) return OperationResult<bool>.NotFound();
                    var checkAccess = await IsWorkPackageAdmin(unit, userId, access.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    unit.WorkPackageMembers.Remove(access);
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageMemberRemove,
                        UserId = userId,
                        WorkPackageMember = access.ToViewModel()
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

        public async Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.PendingInvitations
                        .SingleOrDefaultAsync(i => i.Id == accessId);
                    if (access == null) return OperationResult<bool>.NotFound();
                    var accessCheck = await IsWorkPackageAdmin(unit, userId, access.RecordId);
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

                    unit.PendingInvitations.Remove(access);
                    if (anyOtherPendingInvitations.Length == 1)
                    {
                        var planInfo = await unit.FindPlan(planOwnerId);
                        planInfo.UsedUser--;
                    }

                    unit.PendingInvitations.Remove(access);
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageMemberRemove,
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

        public async Task<OperationResult<bool>> AddAccess(Guid userId, Guid workPackageId, AccessViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var accessCheck = await IsWorkPackageAdmin(unit, userId, workPackageId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.Users
                        .AsNoTracking()
                        .SingleAsync(i => i.Id == userId);

                    var workPackage = await unit.WorkPackages
                        .AsNoTracking()
                        .SingleAsync(g => g.Id == workPackageId);
                    
                    var project = await unit.Projects
                        .AsNoTracking()
                        .SingleAsync(g => g.Id == workPackage.ProjectId);

                    var existingProjectMembers = await unit.ProjectMembers
                        .Where(g => g.ProjectId == workPackage.ProjectId)
                        .Select(i => i.RecordId)
                        .ToArrayAsync();

                    var existingPackageMembers = await unit.WorkPackageMembers
                        .Where(g => g.PackageId == workPackageId)
                        .Select(i => i.RecordId.ToString())
                        .ToListAsync();

                    var existingInvites = await unit.PendingInvitations
                        .Where(i => i.RecordId == workPackageId)
                        .Select(i => i.Identifier)
                        .ToArrayAsync();

                    existingPackageMembers.AddRange(existingInvites);

                    model.Groups = model.Groups.Where(g => !existingPackageMembers.Contains(g.Id)).ToArray();
                    model.Members = model.Members.Where(g => 
                        !existingPackageMembers.Contains(g.Id) &&
                        !existingInvites.Contains(g.Id)
                    ).ToArray();

                    var postman = _serviceProvider.GetService<IPostmanBiz>();
                    var validation = _serviceProvider.GetService<IValidateBiz>();
                    var plan = await unit.FindPlan(project.UserId);

                    var parsed = await unit.ParseInvite(userId, plan, validation, model.Members);
                    if ((plan.UsedUser + parsed.NewMembers.Length) > plan.Users) return OperationResult<bool>.OverCapacity();

                    foreach (var newMember in parsed.NewMembers)
                    {
                        await unit.PlanMembers.AddAsync(new PlanMember
                        {
                            Identifier = newMember,
                            PlanId = plan.Id,
                        });
                    }

                    plan.UsedUser += parsed.NewMembers.Length;
                    var workPackagePermissions = new List<WorkPackageMember>();
                    if (model.Groups.Any())
                    {
                        var groupAccess = await (
                            from member in unit.GroupMembers
                            join grp in unit.Groups on member.GroupId equals grp.Id
                            where member.UserId == userId && !grp.ArchivedAt.HasValue && !member.DeletedAt.HasValue
                            select grp
                        ).ToArrayAsync();

                        foreach (var grp in model.Groups)
                        {
                            var groupId = Guid.Parse(grp.Id);
                            var group = groupAccess.SingleOrDefault(i => i.Id == groupId);
                            if (group == null) continue;
                            workPackagePermissions.Add(new WorkPackageMember()
                            {
                                Access = grp.Access,
                                PackageId = workPackage.Id,
                                IsGroup = true,
                                RecordId = groupId,
                                ProjectId = workPackage.ProjectId
                            });
                        }
                    }

                    foreach (var invite in parsed.InviteById)
                    {
                        workPackagePermissions.Add(new WorkPackageMember
                        {
                            Access = invite.Access,
                            PackageId = workPackage.Id,
                            RecordId = Guid.Parse(invite.Id),
                            IsGroup = false,
                            ProjectId = workPackage.ProjectId
                        });
                    }

                    var projectPermissions = workPackagePermissions
                        .Where(pm => !existingProjectMembers.Contains(pm.RecordId))
                        .Select(invite => new ProjectMember
                        {
                            Access = invite.Access == AccessType.Visitor ? invite.Access : AccessType.Editor,
                            ProjectId = workPackage.ProjectId,
                            RecordId = invite.RecordId,
                            IsGroup = invite.IsGroup,
                        });

                    var pendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access == AccessType.Visitor ? e.Access : AccessType.Editor,
                        Identifier = e.Id,
                        RecordId = workPackage.ProjectId,
                        Type = PendingInvitationType.Project
                    }).ToArray();

                    var workPackagePendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access,
                        Identifier = e.Id,
                        RecordId = workPackage.Id,
                        Type = PendingInvitationType.WorkPackage
                    }).ToArray();

                    await unit.ProjectMembers.AddRangeAsync(projectPermissions);
                    await unit.PendingInvitations.AddRangeAsync(pendingInvitations);
                    await unit.WorkPackageMembers.AddRangeAsync(workPackagePermissions);
                    await unit.PendingInvitations.AddRangeAsync(workPackagePendingInvitations);
                    await unit.SaveChangesAsync();

                    Dictionary<string, string> mapped = new Dictionary<string, string>();
                    foreach (var usr in parsed.InviteById)
                    {
                        var found = parsed.AllInvited.Single(u => u.Id == Guid.Parse(usr.Id));
                        mapped.Add(usr.Id, found.Email);
                    }

                    var viewModel = workPackage.ToViewModel(
                        workPackagePermissions.Select(p => p.ToViewModel()).ToArray(),
                        workPackagePendingInvitations.Select(p => p.ToViewModel()).ToArray()
                    );
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageMemberAdd,
                        UserId = userId,
                        WorkPackage = viewModel,
                        User = user.ToViewModel()
                    });

#pragma warning disable 4014
                    Task.Run(() =>
                        postman.InviteWorkPackage(user.FullName, parsed.EmailIdentities, mapped, viewModel));
#pragma warning restore 4014
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

        public async Task<OperationResult<WorkPackageViewModel>> Fetch(Guid userId, Guid packageId,
            WorkPackageFilterViewModel filter)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var user = await unit.Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
                        return OperationResult<WorkPackageViewModel>.Rejected();

                    var workPackage = await unit.WorkPackages
                        .AsNoTracking()
                        .SingleOrDefaultAsync(w => w.Id == packageId);
                    if (workPackage == null) return OperationResult<WorkPackageViewModel>.NotFound();

                    var groupMembers = await unit.FindGroupPermissions(userId);
                    var groupIds = groupMembers.Select(i => i.GroupId).ToArray();

                    var workPackageMembers = await unit.WorkPackageMembers
                        .Where(i => i.PackageId == packageId && (!i.IsGroup || groupIds.Contains(i.RecordId)))
                        .OrderByDescending(m => m.CreatedAt)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var isMember = workPackageMembers.Any(m =>
                        (m.RecordId == userId && !m.IsGroup) || (m.IsGroup && groupIds.Contains(m.RecordId)));
                    if (!isMember && workPackage.UserId != userId)
                        return OperationResult<WorkPackageViewModel>.Rejected();

                    Guid[] hidden = new Guid[0];
                    var access = workPackage.UserId == userId
                        ? AccessType.Owner
                        : GetPackageHighestAccess(userId, groupMembers, workPackageMembers);
                    if (access != AccessType.Admin && access != AccessType.Owner && access != AccessType.HiddenEditor)
                    {
                        hidden = workPackageMembers
                            .Where(i => i.Access == AccessType.HiddenEditor)
                            .Select(i => i.RecordId).ToArray();

                        workPackageMembers = workPackageMembers.Where(m =>
                            m.Access != AccessType.HiddenEditor).ToArray();

                        if (workPackage.TaskVisibility == WorkPackageTaskVisibility.MembersOnly) filter.Mine = true;
                    }

                    var packageSetting = await unit.WorkPackageMemberSettings
                        .SingleOrDefaultAsync(m => m.UserId == userId && m.PackageId == packageId);
                    if (packageSetting == null)
                    {
                        packageSetting = new WorkPackageMemberSetting
                        {
                            PackageId = packageId,
                            ProjectId = workPackage.ProjectId,
                            UserId = userId,
                            ReceiveNotification = ReceiveNotificationType.ReceiveAll,
                        };
                        await unit.WorkPackageMemberSettings.AddAsync(packageSetting);
                        await unit.SaveChangesAsync();
                    }

                    var pendings = (await unit.PendingInvitations.Where(i => i.RecordId == packageId)
                        .AsNoTracking().ToArrayAsync()).Select(i => i.ToViewModel()).ToArray();

                    var workPackageObjectives = await unit.WorkPackageObjectives
                        .Where(m => m.PackageId == packageId)
                        .OrderBy(m => m.Order)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var workPackageLists = await unit.WorkPackageLists
                        .Where(l =>
                            l.PackageId == packageId &&
                            !l.DeletedAt.HasValue &&
                            (filter.Archived || (!filter.Archived && !l.ArchivedAt.HasValue))
                        )
                        .OrderBy(m => m.Order)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var workPackageCustomFields = await unit.WorkPackageCustomFields
                        .Where(l => l.PackageId == packageId && !l.DeletedAt.HasValue)
                        .OrderByDescending(i => i.CreatedAt)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var workPackageCustomFieldItems = await unit.WorkPackageCustomFieldItems
                        .Where(l => l.PackageId == packageId && !l.DeletedAt.HasValue)
                        .OrderByDescending(i => i.Order)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var workPackageLabels = await unit.WorkPackageLabels
                        .Where(l => l.PackageId == packageId && !l.DeletedAt.HasValue)
                        .OrderByDescending(i => i.CreatedAt)
                        .AsNoTracking()
                        .ToArrayAsync();

                    if (workPackageLabels.Length == 0)
                    {
                        workPackageLabels = AsoodeColors.Plate
                            .Select(c => new WorkPackageLabel
                            {
                                Color = c.Value,
                                DarkColor = c.Dark,
                                PackageId = packageId,
                            }).ToArray();
                        await unit.WorkPackageLabels.AddRangeAsync(workPackageLabels);
                        await unit.SaveChangesAsync();
                    }

                    var userInTasks = new Guid[0];
                    if (filter.Mine)
                    {
                        userInTasks = await unit.WorkPackageTaskMember.Where(i =>
                                i.PackageId == packageId &&
                                (i.RecordId == userId || groupIds.Contains(i.RecordId)))
                            .Select(i => i.TaskId)
                            .ToArrayAsync();
                    }

                    var taskWithLabels = new Guid[0];
                    var requireLabelFilter = filter.Labels.Values.Any(i => i);
                    if (requireLabelFilter)
                    {
                        var labelIds = filter.Labels.Where(l => l.Value)
                            .Select(i => i.Key)
                            .ToArray();

                        taskWithLabels = await unit.WorkPackageTaskLabels
                            .Where(i => i.PackageId == packageId && labelIds.Contains(i.LabelId))
                            .Select(i => i.TaskId).ToArrayAsync();
                    }

                    var workPackageTasks = await unit.WorkPackageTasks
                        .Where(l =>
                            !l.ParentId.HasValue &&
                            l.PackageId == packageId &&
                            !l.DeletedAt.HasValue &&
                            (!filter.Active || (filter.Active && l.State == WorkPackageTaskState.InProgress)) &&
                            ((filter.Archived && l.ArchivedAt.HasValue) ||
                             (!filter.Archived && !l.ArchivedAt.HasValue)) &&
                            (!filter.Mine || (filter.Mine && userInTasks.Contains(l.Id))) &&
                            (!requireLabelFilter || (requireLabelFilter && taskWithLabels.Contains(l.Id)))
                        )
                        .OrderByDescending(i => i.CreatedAt)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var workPackageTaskMembers = await unit.WorkPackageTaskMember
                        .Where(l => l.PackageId == packageId && !hidden.Contains(l.RecordId))
                        .ToArrayAsync();

                    var workPackageTaskLabels = await unit.WorkPackageTaskLabels
                        .Where(l => l.PackageId == packageId && !l.DeletedAt.HasValue)
                        .OrderByDescending(i => i.CreatedAt)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var totalTasks = await unit.WorkPackageTasks
                        .Where(l => l.PackageId == packageId)
                        .CountAsync();

                    var totalDone = await unit.WorkPackageTasks
                        .Where(l => l.PackageId == packageId && l.State == WorkPackageTaskState.Done)
                        .CountAsync();

                    var totalCancelOrDuplicate = await unit.WorkPackageTasks
                        .Where(l => l.PackageId == packageId &&
                                    (l.State == WorkPackageTaskState.Canceled ||
                                     l.State == WorkPackageTaskState.Duplicate))
                        .CountAsync();

                    var attachmentsCount = await unit.WorkPackageTaskAttachments
                        .Where(i => i.PackageId == packageId)
                        .GroupBy(t => t.TaskId)
                        .Select(r => new {TaskId = r.Key, Count = r.Count()})
                        .ToListAsync();

                    var interactionsCount = await unit.WorkPackageTaskInteractions
                        .Where(i => i.PackageId == packageId && i.UserId == userId)
                        .ToListAsync();

                    var subTasksCount = await unit.WorkPackageTasks
                        .Where(i => i.PackageId == packageId && i.ParentId.HasValue &&
                                    ((!filter.Archived && !i.ArchivedAt.HasValue) || filter.Archived))
                        .Select(r => new {r.ParentId, Done = r.DoneAt.HasValue})
                        .ToListAsync();

                    var voteCount = await unit.WorkPackageTaskVotes
                        .Where(i => i.PackageId == packageId)
                        .ToListAsync();

                    var commentCount = await unit.WorkPackageTaskComments
                        .Where(i => i.PackageId == packageId)
                        .ToListAsync();

                    var progress = new WorkPackageProgressViewModel
                    {
                        Done = totalDone,
                        Total = totalTasks,
                        CancelOrDuplicate = totalCancelOrDuplicate
                    };
                    var members = workPackageMembers.Select(m => m.ToViewModel()).ToArray();
                    var objectives = workPackageObjectives.Select(o => o.ToViewModel()).ToArray();
                    var lists = workPackageLists
                        // .Where(l => 
                        //     !filter.Mine ||
                        //     workPackageTasks.Any(t => t.ListId == l.Id)
                        // )
                        .Select(l => l.ToViewModel())
                        .ToArray();
                    var tasks = workPackageTasks.Select(t =>
                    {
                        var members = workPackageTaskMembers.Where(i => i.TaskId == t.Id)
                            .Select(i => i.ToViewModel()).ToArray();
                        var labels = workPackageTaskLabels.Where(i => i.TaskId == t.Id)
                            .Select(i => i.ToViewModel()).ToArray();
                        var viewModel = t.ToViewModel(members, labels);

                        viewModel.HasDescription = !string.IsNullOrEmpty(viewModel.Description);
                        viewModel.Description = "";

                        // TODO: add other details to task

                        viewModel.AttachmentCount = attachmentsCount
                            .SingleOrDefault(a => a.TaskId == viewModel.Id)?.Count ?? 0;

                        viewModel.SubTasksCount = subTasksCount.Count(a => a.ParentId == viewModel.Id);
                        viewModel.SubTasksDone = subTasksCount.Count(a => a.ParentId == viewModel.Id && a.Done);

                        viewModel.CommentCount = commentCount.Count(a => a.TaskId == viewModel.Id);
                        viewModel.DownVotes = voteCount.Count(a => a.TaskId == viewModel.Id && !a.Vote);
                        viewModel.UpVotes = voteCount.Count(a => a.TaskId == viewModel.Id && a.Vote);

                        viewModel.Watching = interactionsCount.Any(i => i.TaskId == viewModel.Id && i.Watching == true);
                        return viewModel;
                    }).ToArray();
                    var customFields = workPackageCustomFields.Select(t => t.ToViewModel()).ToArray();
                    var customFieldItems = workPackageCustomFieldItems.Select(t => t.ToViewModel()).ToArray();
                    var labels = workPackageLabels.Select(t => t.ToViewModel()).ToArray();
                    var result = workPackage.ToViewModel(
                        members,
                        pendings,
                        objectives,
                        lists,
                        tasks,
                        customFields,
                        customFieldItems,
                        labels,
                        progress
                    );
                    result.UserSetting = packageSetting.ToViewModel();
                    return OperationResult<WorkPackageViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex, new ErrorLogPayload
                {
                    UserId = userId,
                    RecordId = packageId,
                    Data = filter
                });
                return OperationResult<WorkPackageViewModel>.Failed();
            }
        }

        public Task<OperationResult<WorkPackageViewModel>> FetchArchived(Guid userId, Guid packageId,
            WorkPackageFilterViewModel filter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region WBS

        public async Task<OperationResult<bool>> Merge(Guid userId, Guid workPackageId, Guid destinationId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    if (workPackageId == destinationId) return OperationResult<bool>.Rejected();

                    var accessCheck = await IsWorkPackageAdmin(unit, userId, workPackageId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    accessCheck = await IsWorkPackageAdmin(unit, userId, destinationId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    var query = await (
                        from pkg in unit.WorkPackages
                        join project in unit.Projects on pkg.ProjectId equals project.Id
                        where pkg.Id == workPackageId || pkg.Id == destinationId
                        select new {Package = pkg, Project = project}
                    ).ToArrayAsync();

                    var sourcePackage = query.Single(i => i.Package.Id == workPackageId).Package;
                    var sourceProject = query.Single(i => i.Package.Id == workPackageId).Project;
                    var destinationPackage = query.Single(i => i.Package.Id == destinationId).Package;
                    var destinationProject = query.Single(i => i.Package.Id == destinationId).Project;

                    if (sourceProject.PlanInfoId != destinationProject.PlanInfoId)
                        return OperationResult<bool>.Rejected();

                    var packageMembers = await unit.WorkPackageMembers.Where(i =>
                        i.PackageId == workPackageId || i.PackageId == destinationId).ToArrayAsync();

                    var sourcePackageMembers = packageMembers
                        .Where(i => i.PackageId == workPackageId).ToArray();

                    var destinationPackageMembers = packageMembers.Except(sourcePackageMembers).ToArray();

                    var projectMembers = await unit.ProjectMembers.Where(i =>
                        i.ProjectId == sourceProject.Id || i.ProjectId == destinationProject.Id).ToArrayAsync();

                    var sourceProjectMembers = projectMembers
                        .Where(i => i.ProjectId == sourceProject.Id).ToArray();

                    var destinationProjectMembers = projectMembers.Except(sourceProjectMembers).ToArray();

                    foreach (var sourceMember in sourcePackageMembers)
                    {
                        if (destinationPackageMembers.Any(m => m.RecordId == sourceMember.RecordId))
                            continue;
                        var access = sourceMember.Access == AccessType.Visitor ? AccessType.Visitor : AccessType.Editor;
                        await unit.WorkPackageMembers.AddAsync(new WorkPackageMember
                        {
                            Access = access,
                            IsGroup = sourceMember.IsGroup,
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id,
                            RecordId = sourceMember.RecordId
                        });
                        if (destinationProjectMembers.Any(m => m.RecordId == sourceMember.RecordId))
                            continue;
                        await unit.ProjectMembers.AddAsync(new ProjectMember
                        {
                            Access = access,
                            IsGroup = sourceMember.IsGroup,
                            RecordId = sourceMember.RecordId,
                            ProjectId = destinationProject.Id
                        });
                    }

                    sourcePackage.ArchivedAt = sourceProject.ArchivedAt = DateTime.Now;
                    await unit.SaveChangesAsync();


                    await unit.Conversations
                        .Where(i => i.ChannelId == workPackageId)
                        .UpdateAsync(i => new Conversation()
                        {
                            ChannelId = destinationId
                        });
                    await unit.WorkPackageTaskInteractions
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskInteraction
                        {
                            PackageId = destinationId
                        });
                    await unit.WorkPackageTaskLabels
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskLabel
                        {
                            PackageId = destinationId
                        });
                    await unit.WorkPackageTaskMember
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskMember
                        {
                            PackageId = destinationId
                        });
                    await unit.Channels
                        .Where(i => i.Id == workPackageId)
                        .UpdateAsync(i => new Channel
                        {
                            ArchivedAt = sourcePackage.ArchivedAt
                        });
                    await unit.WorkPackageLabels
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageLabel
                        {
                            PackageId = destinationId
                        });
                    await unit.WorkPackageLists
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageList
                        {
                            PackageId = destinationId
                        });
                    await unit.WorkPackageObjectives
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageObjective
                        {
                            PackageId = destinationId
                        });
                    await unit.WorkPackageTasks
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTask
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageTaskAttachments
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskAttachment()
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageTaskBlockers
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskBlocker()
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageTaskComments
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskComment()
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageRelatedTasks
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageRelatedTask
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageTaskCustomFields
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskCustomField
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageCustomFields
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageCustomField
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageCustomFieldItems
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageCustomFieldItem
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageTaskObjectives
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskObjective
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageTaskTimes
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskTime()
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });
                    await unit.WorkPackageTaskVotes
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskVote()
                        {
                            PackageId = destinationId,
                            ProjectId = destinationProject.Id
                        });


                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageMerge,
                        UserId = userId,
                        WorkPackage = sourcePackage.ToViewModel(),
                        WorkPackage2 = destinationPackage.ToViewModel(),
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

        public async Task<OperationResult<bool>> Connect(Guid userId, Guid workPackageId, Guid projectId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var accessCheck = await IsWorkPackageAdmin(unit, userId, workPackageId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    accessCheck = await IsProjectAdmin(unit, userId, projectId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var package = await unit.WorkPackages.SingleAsync(i => i.Id == workPackageId);

                    if (package.ProjectId == projectId) return OperationResult<bool>.Rejected();

                    var projects = await unit.Projects.Where(i => i.Id == projectId || i.Id == package.ProjectId)
                        .ToArrayAsync();
                    var source = projects.Single(i => i.Id == package.ProjectId);
                    var destination = projects.Single(i => i.Id == projectId);

                    if (!destination.Complex) return OperationResult<bool>.Rejected();

                    if (source.PlanInfoId != destination.PlanInfoId) return OperationResult<bool>.Rejected();

                    var user = await unit.Users.SingleAsync(i => i.Id == userId);

                    var packageMembers = await unit.WorkPackageMembers.Where(i =>
                        i.PackageId == workPackageId).ToArrayAsync();

                    var projectMembers = await unit.ProjectMembers.Where(i =>
                        i.ProjectId == destination.Id).ToArrayAsync();

                    foreach (var sourceMember in packageMembers)
                    {
                        if (projectMembers.Any(m => m.RecordId == sourceMember.RecordId))
                            continue;
                        var access = sourceMember.Access == AccessType.Visitor ? AccessType.Visitor : AccessType.Editor;
                        await unit.ProjectMembers.AddAsync(new ProjectMember
                        {
                            Access = access,
                            IsGroup = sourceMember.IsGroup,
                            RecordId = sourceMember.RecordId,
                            ProjectId = destination.Id
                        });
                    }

                    source.ArchivedAt = DateTime.UtcNow;
                    package.ProjectId = destination.Id;

                    await unit.SaveChangesAsync();

                    await unit.WorkPackageTasks
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTask
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageTaskAttachments
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskAttachment()
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageTaskBlockers
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskBlocker()
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageTaskComments
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskComment()
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageRelatedTasks
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageRelatedTask
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageTaskCustomFields
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskCustomField
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageCustomFields
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageCustomField
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageCustomFieldItems
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageCustomFieldItem
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageTaskObjectives
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskObjective
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageTaskTimes
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskTime()
                        {
                            ProjectId = projectId
                        });
                    await unit.WorkPackageTaskVotes
                        .Where(i => i.PackageId == workPackageId)
                        .UpdateAsync(i => new WorkPackageTaskVote()
                        {
                            ProjectId = projectId
                        });

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageConnect,
                        UserId = userId,
                        WorkPackage = package.ToViewModel(),
                        Project = destination.ToViewModel(),
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

        public async Task<OperationResult<bool>> Upgrade(Guid userId, Guid workPackageId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var accessCheck = await IsWorkPackageAdmin(unit, userId, workPackageId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);

                    var package = await unit.WorkPackages
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.Id == workPackageId);

                    var project = await unit.Projects.SingleAsync(i => i.Id == package.ProjectId);
                    if (project.Complex) return OperationResult<bool>.Rejected();

                    var planInfo = await unit.UserPlanInfo.SingleOrDefaultAsync(i => i.Id == project.PlanInfoId);
                    if (planInfo == null) return OperationResult<bool>.Rejected();

                    if (planInfo.Project <= planInfo.UsedProject) return OperationResult<bool>.OverCapacity();
                    planInfo.UsedProject++;

                    project.Complex = true;

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageUpgrade,
                        UserId = userId,
                        WorkPackage = package.ToViewModel(),
                        Project = project.ToViewModel(),
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

        #region Edit

        public async Task<OperationResult<bool>> Archive(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await IsWorkPackageAdmin(unit, userId, id);
                    if (access.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    var result = await (
                        from wp in unit.WorkPackages
                        join proj in unit.Projects on wp.ProjectId equals proj.Id
                        where wp.Id == id
                        select new {WorkPackage = wp, Project = proj}
                    ).SingleOrDefaultAsync();

                    var now = result.WorkPackage.ArchivedAt.HasValue ? (DateTime?) null : DateTime.UtcNow;

                    await unit.WorkPackageLists.Where(i => i.PackageId == id)
                        .UpdateAsync(i => new WorkPackageList {ArchivedAt = now});
                    await unit.WorkPackageTasks.Where(i => i.PackageId == id)
                        .UpdateAsync(i => new WorkPackageTask {ArchivedAt = now});

                    result.WorkPackage.ArchivedAt = now;
                    if (!result.Project.Complex)
                    {
                        result.Project.ArchivedAt = now;
                    }

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = now == null ? ActivityType.WorkPackageRestore : ActivityType.WorkPackageArchive,
                        UserId = userId,
                        WorkPackage = result.WorkPackage.ToViewModel()
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

        public async Task<OperationResult<bool>> Remove(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var found = await (
                        from wp in unit.WorkPackages
                        join proj in unit.Projects on wp.ProjectId equals proj.Id
                        where wp.Id == id
                        select new {Package = wp, Project = proj}
                    ).AsNoTracking().SingleOrDefaultAsync();

                    if (found == null) return OperationResult<bool>.Rejected();

                    var packageMembers = await unit.WorkPackageMembers
                        .Where(i => i.PackageId == id)
                        .ToArrayAsync();

                    var access = packageMembers.Any(i =>
                        i.RecordId == userId &&
                        i.PackageId == id &&
                        i.Access == AccessType.Owner
                    );

                    if (!access)
                    {
                        access = await unit.ProjectMembers
                            .AnyAsync(i =>
                                i.RecordId == userId && i.ProjectId == found.Project.Id &&
                                i.Access == AccessType.Owner);

                        if (!access) return OperationResult<bool>.Rejected();
                    }

                    var memberIds = packageMembers.Where(i => !i.IsGroup)
                        .Select(i => i.RecordId)
                        .ToArray();

                    var groupIds = packageMembers
                        .Where(i => i.IsGroup)
                        .Select(i => i.RecordId)
                        .ToArray();

                    var groupMembers = await unit.GroupMembers
                        .Where(g => groupIds.Contains(g.Id))
                        .Select(i => i.UserId)
                        .ToArrayAsync();

                    var members = groupMembers.Concat(memberIds).Distinct().ToArray();

                    var plan = await unit.FindPlan(userId);
                    plan.UsedWorkPackage--;

                    var attachments = await (
                        from attach in unit.WorkPackageTaskAttachments
                        join upload in unit.Uploads on attach.UploadId equals upload.Id
                        where attach.PackageId == id
                        select new {Attachment = attach, Upload = upload}
                    ).ToArrayAsync();

                    if (attachments.Any())
                    {
                        var uploadBiz = _serviceProvider.GetService<IUploadProvider>();
                        var total = attachments.Select(a => a.Upload.Size).Sum();
                        plan.UsedSpace -= total;

                        var attachs = attachments.Select(i => i.Attachment).ToArray();
                        var uploads = attachments.Select(i => i.Upload).ToArray();

                        foreach (var attachment in attachments)
                        {
                            await uploadBiz.Delete(
                                attachment.Attachment.Path,
                                UploadSection.WorkPackage,
                                attachment.Attachment.UserId
                            );
                        }

                        unit.WorkPackageTaskAttachments.RemoveRange(attachs);
                        unit.Uploads.RemoveRange(uploads);
                    }

                    unit.WorkPackageMembers.RemoveRange(packageMembers);
                    await unit.WorkPackages.Where(i => i.Id == id).DeleteAsync();
                    await unit.WorkPackageTaskMember.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageLists.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTasks.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskTimes.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskBlockers.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskComments.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskInteractions.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskObjectives.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskVotes.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskCustomFields.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageCustomFieldItems.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageCustomFields.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageTaskLabels.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.WorkPackageRelatedTasks.Where(i => i.PackageId == id).DeleteAsync();
                    await unit.PendingInvitations.Where(i => i.RecordId == id).DeleteAsync();

                    await unit.Channels.Where(i => i.Id == id).DeleteAsync();
                    await unit.Conversations.Where(i => i.ChannelId == id).DeleteAsync();
                    await unit.WorkPackageObjectives.Where(i => i.PackageId == id).DeleteAsync();

                    if (!found.Project.Complex)
                    {
                        await unit.Conversations.Where(i => i.ChannelId == found.Project.Id).DeleteAsync();
                        await unit.Channels.Where(i => i.Id == found.Project.Id).DeleteAsync();
                        await unit.Projects.Where(i => i.Id == found.Project.Id).DeleteAsync();
                        await unit.ProjectMembers.Where(i => i.ProjectId == found.Project.Id).DeleteAsync();
                        await unit.WorkPackageTaskTimes.Where(i => i.ProjectId == found.Project.Id).DeleteAsync();
                        await unit.PendingInvitations.Where(i => i.RecordId == found.Project.Id).DeleteAsync();
                    }

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageRemove,
                        UserId = userId,
                        WorkPackage = found.Package.ToViewModel(),
                        UserIds = members
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

        public async Task<OperationResult<bool>> Edit(Guid userId, Guid id, SimpleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var package = await unit.WorkPackages
                        .Where(i => i.Id == id)
                        .SingleOrDefaultAsync();

                    var checkAccess = await IsProjectAdmin(unit, userId, package.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    package.Title = model.Title;
                    package.Description = model.Description;

                    var project = await unit.Projects.SingleAsync(i => i.Id == package.ProjectId);
                    if (!project.Complex)
                    {
                        project.Title = model.Title;
                        project.Description = model.Description;
                    }

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageEdit,
                        UserId = userId,
                        WorkPackage = package.ToViewModel(),
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

        #region Setting

        public async Task<OperationResult<bool>> SortOrder(Guid userId, Guid packageId, SortOrderViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var user = await unit.FindUser(userId);
                    if (user == null) return OperationResult<bool>.Rejected();

                    var permission = await IsWorkPackageAdmin(unit, userId, packageId);
                    if (permission.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    var package = await unit.WorkPackages.SingleOrDefaultAsync(i => i.Id == packageId);
                    if (package == null) return OperationResult<bool>.Rejected();

                    package.AttachmentsSort = model.AttachmentsSort;
                    package.ListsSort = model.ListsSort;
                    package.TasksSort = model.TasksSort;
                    package.SubTasksSort = model.SubTasksSort;

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageEdit,
                        UserId = userId,
                        WorkPackage = package.ToViewModel(),
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

        public async Task<OperationResult<bool>> Setting(Guid userId, Guid id, WorkPackageSettingViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await IsWorkPackageAdmin(unit, userId, id);
                    if (access.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();
                    var workPackage = await unit.WorkPackages.SingleAsync(w => w.Id == id);

                    if (model.Visibility.HasValue) workPackage.TaskVisibility = model.Visibility.Value;

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageSetting,
                        UserId = userId,
                        WorkPackage = workPackage.ToViewModel()
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

        public async Task<OperationResult<bool>> UserSetting(Guid userId, Guid id,
            WorkPackageUserSettingViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var setting = await unit.WorkPackageMemberSettings
                        .SingleOrDefaultAsync(i => i.UserId == userId && i.PackageId == id);

                    if (setting == null) return OperationResult<bool>.Rejected();

                    if (model.NotificationType.HasValue) setting.ReceiveNotification = model.NotificationType.Value;
                    if (model.ShowTotal.HasValue) setting.ShowTotalCards = model.ShowTotal.Value;

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageUserSetting,
                        UserId = userId,
                        PackageUserSetting = setting.ToViewModel()
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

        public async Task<OperationResult<bool>> Permissions(Guid userId, Guid packageId,
            WorkPackagePermissionViewModel permission)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsWorkPackageAdmin(unit, userId, packageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var user = await unit.Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.Id == userId);

                    var package = await unit.WorkPackages.SingleOrDefaultAsync(i => i.Id == packageId);
                    if (package == null || package.ArchivedAt.HasValue || package.DeletedAt.HasValue)
                        return OperationResult<bool>.Rejected();

                    package.PermissionComment = permission.PermissionComment;
                    package.PermissionEditAttachment = permission.PermissionEditAttachment;
                    package.PermissionCreateAttachment = permission.PermissionCreateAttachment;
                    package.PermissionAssignMembers = permission.PermissionAssignMembers;
                    package.PermissionAssignLabels = permission.PermissionAssignLabels;
                    package.PermissionChangeTaskState = permission.PermissionChangeTaskState;
                    package.PermissionEditTask = permission.PermissionEditTask;
                    package.PermissionArchiveTask = permission.PermissionArchiveTask;
                    package.PermissionCreateTask = permission.PermissionCreateTask;
                    package.PermissionArchiveList = permission.PermissionArchiveList;
                    package.PermissionEditList = permission.PermissionEditList;
                    package.PermissionCreateList = permission.PermissionCreateList;

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageEdit,
                        UserId = userId,
                        WorkPackage = package.ToViewModel(),
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

        #region Labels

        public async Task<OperationResult<bool>> CreateLabel(Guid userId, Guid packageId, LabelViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsWorkPackageAdmin(unit, userId, packageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var label = new WorkPackageLabel
                    {
                        Title = model.Title?.Trim(),
                        Color = model.Color?.Trim(),
                        PackageId = packageId
                    };
                    await unit.WorkPackageLabels.AddAsync(label);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageLabelAdd,
                        UserId = userId,
                        PackageLabel = label.ToViewModel()
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

        public async Task<OperationResult<bool>> RenameLabel(Guid userId, Guid labelId, LabelViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var label = await unit.WorkPackageLabels.SingleOrDefaultAsync(i => i.Id == labelId);
                    if (label == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsWorkPackageAdmin(unit, userId, label.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    label.Title = model.Title?.Trim();
                    label.Color = model.Color?.Trim();
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageLabelRename,
                        UserId = userId,
                        PackageLabel = label.ToViewModel()
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

        public async Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid labelId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var label = await unit.WorkPackageLabels.SingleOrDefaultAsync(i => i.Id == labelId);
                    if (label == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsWorkPackageAdmin(unit, userId, label.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    unit.WorkPackageLabels.Remove(label);
                    await unit.WorkPackageTaskLabels.Where(i => i.LabelId == labelId).DeleteAsync();
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageLabelRemove,
                        UserId = userId,
                        PackageLabel = label.ToViewModel()
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

        #region Lists

        public async Task<OperationResult<bool>> Order(Guid userId, Guid id, ChangeOrderViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var package = await unit.WorkPackages
                        .Where(i => i.Id == id)
                        .SingleOrDefaultAsync();

                    var checkAccess = await IsProjectAdmin(unit, userId, package.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var allPackages = await unit.WorkPackages
                        .Where(i => i.ProjectId == package.ProjectId && i.SubProjectId == package.SubProjectId)
                        .OrderBy(i => i.CreatedAt)
                        .ToListAsync();

                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    var noneOrdered = allPackages.Any(p => p.Order == 0);
                    int order = 1;
                    if (noneOrdered)
                    {
                        allPackages = allPackages.OrderBy(i => i.CreatedAt).ToList();
                        allPackages.ForEach(p => p.Order = order++);
                    }

                    var element = allPackages.Single(i => i.Id == id);
                    allPackages.Remove(element);
                    allPackages.Insert(model.Order - 1, element);

                    order = 1;
                    allPackages.ForEach((e) => { e.Order = order++; });

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageEdit,
                        UserId = userId,
                        WorkPackage = element.ToViewModel(),
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

        public async Task<OperationResult<bool>> CloneList(Guid userId, Guid listId, TitleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var list = await unit.WorkPackageLists
                        .Where(i => i.Id == listId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    var checkAccess = await IsWorkPackageAdmin(unit, userId, list.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var now = DateTime.UtcNow;
                    list.Id = Guid.NewGuid();
                    list.Title = model.Title.Trim();
                    list.CreatedAt = now;
                    list.UpdatedAt = now;

                    var tasks = await unit.WorkPackageTasks.Where(i =>
                        i.ListId == listId &&
                        !i.DeletedAt.HasValue &&
                        !i.ArchivedAt.HasValue &&
                        !i.ParentId.HasValue
                    ).AsNoTracking().ToArrayAsync();

                    var mappedIds = tasks.ToDictionary(i
                        => i.Id, j => Guid.NewGuid());

                    foreach (var task in tasks)
                    {
                        task.Id = mappedIds[task.Id];
                        task.ListId = list.Id;
                        task.CreatedAt = now;
                        task.UpdatedAt = now;
                    }

                    await unit.WorkPackageLists.AddAsync(list);
                    await unit.WorkPackageTasks.AddRangeAsync(tasks);
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageListClone,
                        UserId = userId,
                        PackageList = list.ToViewModel(),
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

        public async Task<OperationResult<bool>> ArchiveList(Guid userId, Guid listId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var list = await unit.WorkPackageLists
                        .Where(i => i.Id == listId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    var checkAccess = await IsWorkPackageAdmin(unit, userId, list.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var now = DateTime.UtcNow;

                    await unit.WorkPackageLists
                        .Where(i => i.Id == listId)
                        .UpdateAsync(i => new WorkPackageList {ArchivedAt = now});
                    await unit.WorkPackageTasks
                        .Where(i => i.ListId == listId && !i.ArchivedAt.HasValue && !i.DeletedAt.HasValue)
                        .UpdateAsync(i => new WorkPackageTask {ArchivedAt = now});

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageListArchive,
                        UserId = userId,
                        PackageList = list.ToViewModel(),
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

        public async Task<OperationResult<bool>> ArchiveListTasks(Guid userId, Guid listId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var list = await unit.WorkPackageLists
                        .Where(i => i.Id == listId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    var checkAccess = await IsWorkPackageAdmin(unit, userId, list.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var now = DateTime.UtcNow;

                    await unit.WorkPackageTasks
                        .Where(i => i.ListId == listId && !i.ArchivedAt.HasValue && !i.DeletedAt.HasValue)
                        .UpdateAsync(i => new WorkPackageTask {ArchivedAt = now});

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageListTasksArchive,
                        UserId = userId,
                        PackageList = list.ToViewModel(),
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
        
        
        public async Task<OperationResult<bool>> DeleteListTasks(Guid userId, Guid listId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var list = await unit.WorkPackageLists
                        .Where(i => i.Id == listId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    var checkAccess = await IsWorkPackageAdmin(unit, userId, list.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    await unit.WorkPackageTasks
                        .Where(i => i.ListId == listId && !i.ArchivedAt.HasValue && !i.DeletedAt.HasValue)
                        .DeleteAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageListTasksDelete,
                        UserId = userId,
                        PackageList = list.ToViewModel(),
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

        public async Task<OperationResult<bool>> CreateList(Guid userId, Guid workPackageId, TitleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsWorkPackageEditor(unit, userId, workPackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var count = await unit.WorkPackageLists
                        .Where(p => p.PackageId == workPackageId).CountAsync();

                    var list = new WorkPackageList
                    {
                        Order = count + 1,
                        Title = model.Title.Trim(),
                        PackageId = workPackageId,
                    };
                    await unit.WorkPackageLists.AddAsync(list);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageListAdd,
                        UserId = userId,
                        PackageList = list.ToViewModel(),
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

        public async Task<OperationResult<bool>> RenameList(Guid userId, Guid listId, TitleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var list = await unit.WorkPackageLists
                        .Where(i => i.Id == listId)
                        .SingleOrDefaultAsync();

                    var checkAccess = await IsWorkPackageEditor(unit, userId, list.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    list.Title = model.Title.Trim();
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageListEdit,
                        UserId = userId,
                        PackageList = list.ToViewModel(),
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

        public async Task<OperationResult<bool>> RepositionList(Guid userId, Guid listId, RepositionViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var list = await unit.WorkPackageLists
                        .Where(i => i.Id == listId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    if (model.Order == list.Order) return OperationResult<bool>.Success(true);

                    var checkAccess = await IsWorkPackageEditor(unit, userId, list.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var allLists = await unit.WorkPackageLists.Where(p =>
                            p.PackageId == list.PackageId)
                        .OrderBy(i => i.Order)
                        .ToListAsync();

                    var element = allLists.Single(e => e.Id == listId);
                    allLists.Remove(element);
                    allLists.Insert(model.Order - 1, element);

                    int orders = 1;
                    allLists.ForEach((e) => { e.Order = orders++; });

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageListOrder,
                        UserId = userId,
                        PackageList = allLists.Single(i => i.Id == listId).ToViewModel(),
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

        #region Objectives

        public async Task<OperationResult<bool>> CreateObjective(Guid userId, Guid workPackageId,
            CreateObjectiveViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsWorkPackageEditor(unit, userId, workPackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var existing = await unit.WorkPackageObjectives
                        .Where(p => p.PackageId == workPackageId && p.ParentId == model.ParentId
                        ).AsNoTracking().ToArrayAsync();

                    var objective = new WorkPackageObjective
                    {
                        Description = model.Description.Trim(),
                        Level = existing.FirstOrDefault()?.Level ?? 1,
                        Order = existing.Length + 1,
                        Type = model.Type,
                        Title = model.Title.Trim(),
                        PackageId = workPackageId,
                        ParentId = model.ParentId
                    };
                    await unit.WorkPackageObjectives.AddAsync(objective);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageObjectiveAdd,
                        UserId = userId,
                        Objective = objective.ToViewModel(),
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

        public async Task<OperationResult<bool>> EditObjective(Guid userId, Guid objectiveId,
            CreateObjectiveViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var objective = await unit.WorkPackageObjectives
                        .SingleOrDefaultAsync(i => i.Id == objectiveId);

                    if (objective == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsWorkPackageEditor(unit, userId, objective.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    objective.Description = model.Description.Trim();
                    objective.Type = model.Type;
                    objective.Title = model.Title.Trim();

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageObjectiveEdit,
                        UserId = userId,
                        Objective = objective.ToViewModel(),
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

        public async Task<OperationResult<bool>> DeleteObjective(Guid userId, Guid objectiveId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var objective = await unit.WorkPackageObjectives
                        .SingleOrDefaultAsync(i => i.Id == objectiveId);

                    if (objective == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsWorkPackageEditor(unit, userId, objective.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    await unit.WorkPackageTasks.Where(i => i.ObjectiveId == objectiveId)
                        .UpdateAsync(i => new WorkPackageTask {ObjectiveId = null});
                    unit.WorkPackageObjectives.Remove(objective);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageObjectiveRemove,
                        UserId = userId,
                        Objective = objective.ToViewModel(),
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

        #region Private

        private string NormalizeEmail(string email)
        {
            return email.Trim().ToLower();
        }

        private AccessType GetPackageHighestAccess(Guid userId, GroupMember[] groups, WorkPackageMember[] members)
        {
            return members.Where(i =>
                i.RecordId == userId ||
                groups.Any(g => g.GroupId == i.RecordId)
            ).Select(i => i.Access).OrderBy(i => i).First();
        }

        private async Task<OperationResult<bool>> IsProjectAdmin(ProjectManagementDbContext unit, Guid userId,
            Guid projectId)
        {
            var user = await unit.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == userId);
            if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
                return OperationResult<bool>.Rejected();

            var groupIds = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                where member.UserId == userId &&
                      !grp.ArchivedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select grp.Id
            ).ToArrayAsync();

            var access = await (
                    from member in unit.ProjectMembers
                    join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                    from proj in tmp.DefaultIfEmpty()
                    where member.ProjectId == projectId &&
                          (member.Access == AccessType.Admin || member.Access == AccessType.Owner) &&
                          (
                              member.RecordId == userId ||
                              groupIds.Contains(member.RecordId) &&
                              !member.DeletedAt.HasValue &&
                              !proj.DeletedAt.HasValue &&
                              !proj.ArchivedAt.HasValue
                          )
                    select proj
                )
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .AnyAsync();

            if (!access) return OperationResult<bool>.Rejected();
            return OperationResult<bool>.Success(true);
        }

        private async Task<OperationResult<bool>> IsWorkPackageAdmin(ProjectManagementDbContext unit, Guid userId,
            Guid packageId)
        {
            var user = await unit.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == userId);
            if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
                return OperationResult<bool>.Rejected();

            var groupIds = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                where member.UserId == userId &&
                      !grp.ArchivedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select grp.Id
            ).ToArrayAsync();

            var access = await (
                    from member in unit.WorkPackageMembers
                    join proj in unit.WorkPackages on member.PackageId equals proj.Id into tmp
                    from proj in tmp.DefaultIfEmpty()
                    where ((!member.IsGroup && member.RecordId == userId) ||
                           (member.IsGroup && groupIds.Contains(member.RecordId)) &&
                           !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                           !proj.ArchivedAt.HasValue) && member.PackageId == packageId &&
                          (member.Access == AccessType.Admin || member.Access == AccessType.Owner)
                    select proj
                )
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .AnyAsync();

            if (!access) return OperationResult<bool>.Rejected();
            return OperationResult<bool>.Success(true);
        }

        private async Task<OperationResult<bool>> IsWorkPackageEditor(ProjectManagementDbContext unit, Guid userId,
            Guid packageId)
        {
            var user = await unit.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == userId);
            if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
                return OperationResult<bool>.Rejected();

            var groupIds = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                where member.UserId == userId &&
                      !grp.ArchivedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select grp.Id
            ).ToArrayAsync();

            var access = await (
                    from member in unit.WorkPackageMembers
                    join proj in unit.WorkPackages on member.PackageId equals proj.Id into tmp
                    from proj in tmp.DefaultIfEmpty()
                    where ((!member.IsGroup && member.RecordId == userId) ||
                           (member.IsGroup && groupIds.Contains(member.RecordId)) &&
                           !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                           !proj.ArchivedAt.HasValue) && member.PackageId == packageId &&
                          (member.Access != AccessType.Visitor)
                    select proj
                )
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .AnyAsync();

            if (!access) return OperationResult<bool>.Rejected();
            return OperationResult<bool>.Success(true);
        }

        #endregion
    }
}