using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Contracts.ProjectManagement;
using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.Collaboration;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Base;
using Asoode.Application.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.ProjectManagement
{
    internal class ProjectBiz : IProjectBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public ProjectBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Create

        public async Task<OperationResult<bool>> Create(Guid userId, ProjectCreateViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var op = await PrepareCreate(unit, userId, model);
                    if (op.Status != OperationResultStatus.Success)
                    {
                        var ret = OperationResult<bool>.Rejected();
                        ret.Status = op.Status;
                        ret.Data = false;
                        ret.Exception = op.Exception;
                        return ret;
                    }

                    await unit.SaveChangesAsync();

                    Dictionary<string, string> mapped = new Dictionary<string, string>();
                    foreach (var usr in op.Data.InviteById)
                    {
                        var found = op.Data.AllInvited.Single(u => u.Id == Guid.Parse(usr.Id));
                        mapped.Add(usr.Id, found.Email);
                    }

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectAdd,
                        User = op.Data.User,
                        UserId = userId,
                        Project = op.Data.ViewModel,
                        Pendings = op.Data.PendingInvitations
                    });

                    // TODO: send to background
// #pragma warning disable 4014
//                     Task.Run(() =>
//                         postman.InviteProject(op.Data.User.FullName, op.Data.EmailIdentities, mapped,
//                             op.Data.ViewModel));
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

        public async Task<OperationResult<ProjectPrepareViewModel>> PrepareCreate(ProjectManagementDbContext unit,
            Guid userId,
            ProjectCreateViewModel model)
        {
            try
            {
                var validation = _serviceProvider.GetService<IValidateBiz>();
                
                var user = await unit.FindUser(userId);
                if (user == null) return OperationResult<ProjectPrepareViewModel>.Rejected();

                var parsed = await unit.ParseInvite(userId, validation, model.Members);
                var groupNames = new List<string>();
                var workPackageLabels = Array.Empty<WorkPackageLabel>();
                var seasons = new List<ProjectSeason>();
                var channels = new List<Channel>();
                var workPackages = new List<WorkPackage>();
                var workPackageLists = new List<WorkPackageList>();
                var workPackageMembers = new List<WorkPackageMember>();
                var subProjects = new List<SubProject>();
                var project = new Project
                {
                    UserId = userId,
                    Title = model.Title,
                    Description = model.Description,
                    Complex = model.Complex,
                    Template = model.Complex ? model.Template : ProjectTemplate.None
                };
                var channel = new Channel
                {
                    Title = model.Title,
                    Type = ChannelType.Project,
                    UserId = userId,
                    Id = project.Id
                };
                channels.Add(channel);
                var projectPermissions = new List<ProjectMember>
                {
                    new ProjectMember
                    {
                        Access = AccessType.Owner,
                        ProjectId = project.Id,
                        RecordId = userId,
                        IsGroup = false
                    }
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
                        projectPermissions.Add(new ProjectMember
                        {
                            Access = grp.Access,
                            ProjectId = project.Id,
                            IsGroup = true,
                            RecordId = groupId
                        });
                        groupNames.Add(group.Title);
                    }
                }

                foreach (var invite in parsed.InviteById)
                {
                    projectPermissions.Add(new ProjectMember
                    {
                        Access = invite.Access,
                        ProjectId = project.Id,
                        RecordId = Guid.Parse(invite.Id),
                        IsGroup = false,
                    });
                }

                var packagePendingInvitations = Array.Empty<PendingInvitation>();
                var pendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                {
                    Access = e.Access,
                    Identifier = e.Id,
                    RecordId = project.Id,
                    Type = PendingInvitationType.Project
                }).ToArray();

                if (model.Complex)
                {
                    #region Seasons

                    seasons.Add(new ProjectSeason
                    {
                        // Title = translate.Get("COMPLEX_PROJECT_SEASON_1"),
                        // Description = translate.Get("COMPLEX_PROJECT_SEASON_1_DESCRIPTION"),
                        ProjectId = project.Id,
                        UserId = userId,
                        Order = 1
                    });
                    seasons.Add(new ProjectSeason
                    {
                        // Title = translate.Get("COMPLEX_PROJECT_SEASON_2"),
                        // Description = translate.Get("COMPLEX_PROJECT_SEASON_2_DESCRIPTION"),
                        ProjectId = project.Id,
                        UserId = userId,
                        Order = 2
                    });
                    seasons.Add(new ProjectSeason
                    {
                        // Title = translate.Get("COMPLEX_PROJECT_SEASON_3"),
                        // Description = translate.Get("COMPLEX_PROJECT_SEASON_3_DESCRIPTION"),
                        ProjectId = project.Id,
                        UserId = userId,
                        Order = 3
                    });
                    seasons.Add(new ProjectSeason
                    {
                        // Title = translate.Get("COMPLEX_PROJECT_SEASON_4"),
                        // Description = translate.Get("COMPLEX_PROJECT_SEASON_4_DESCRIPTION"),
                        ProjectId = project.Id,
                        UserId = userId,
                        Order = 4
                    });

                    #endregion
                }
                else
                {
                    var packageId = Guid.NewGuid();
                    channel.Type = ChannelType.WorkPackage;
                    channel.Id = packageId;
                    workPackages.Add(new WorkPackage
                    {
                        Id = packageId,
                        Description = model.Description,
                        Title = model.Title,
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
                    });
                    workPackageLabels = AsoodeColors.Plate
                        .Select(c => new WorkPackageLabel
                        {
                            Color = c.Value,
                            DarkColor = c.Dark,
                            PackageId = packageId,
                        }).ToArray();

                    packagePendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access,
                        Identifier = e.Id,
                        RecordId = project.Id,
                        Type = PendingInvitationType.WorkPackage
                    }).ToArray();

                    foreach (var projectMember in projectPermissions)
                    {
                        workPackageMembers.Add(new WorkPackageMember
                        {
                            PackageId = packageId,
                            Access = projectMember.Access,
                            IsGroup = projectMember.IsGroup,
                            RecordId = projectMember.RecordId,
                            ProjectId = project.Id
                        });
                    }

                    if (model.BoardTemplate.HasValue)
                    {
                        switch (model.BoardTemplate.Value)
                        {
                            case BoardTemplate.Departments:
                                workPackageLists.AddRange(new[]
                                {
                                    new WorkPackageList
                                    {
                                        Order = 1,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_1"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 2,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_2"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 3,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_3"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 4,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_4"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 5,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_DEPARTMENT_5"),
                                        PackageId = packageId
                                    },
                                });
                                break;
                            case BoardTemplate.Kanban:
                                workPackageLists.AddRange(new[]
                                {
                                    new WorkPackageList
                                    {
                                        Order = 1,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_1"),
                                        PackageId = packageId,
                                        Kanban = WorkPackageTaskState.ToDo
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 2,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_2"),
                                        PackageId = packageId,
                                        Kanban = WorkPackageTaskState.InProgress
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 3,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_3"),
                                        PackageId = packageId,
                                        Kanban = WorkPackageTaskState.Done
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 4,
                                        // Title = translate.Get("BOARD_TEMPLATES_SAMPLES_KANBAN_4"),
                                        PackageId = packageId,
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
                                        // Title = translate.Get("ENUMS_WEEKDAY_SATURDAY"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 2,
                                        // Title = translate.Get("ENUMS_WEEKDAY_SUNDAY"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 3,
                                        // Title = translate.Get("ENUMS_WEEKDAY_MONDAY"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 4,
                                        // Title = translate.Get("ENUMS_WEEKDAY_TUESDAY"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 5,
                                        // Title = translate.Get("ENUMS_WEEKDAY_WEDNESDAY"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 6,
                                        // Title = translate.Get("ENUMS_WEEKDAY_THURSDAY"),
                                        PackageId = packageId
                                    },
                                    new WorkPackageList
                                    {
                                        Order = 7,
                                        // Title = translate.Get("ENUMS_WEEKDAY_FRIDAY"),
                                        PackageId = packageId
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
                                        PackageId = packageId
                                    });
                                }

                                foreach (var groupName in groupNames)
                                {
                                    workPackageLists.Add(new WorkPackageList
                                    {
                                        Order = ++counter,
                                        Title = groupName,
                                        PackageId = packageId
                                    });
                                }

                                break;
                        }
                    }
                }

                // TODO: send welcome messages in chat

                await unit.Projects.AddAsync(project);
                await unit.ProjectMembers.AddRangeAsync(projectPermissions);
                await unit.ProjectSeasons.AddRangeAsync(seasons);
                await unit.SubProjects.AddRangeAsync(subProjects);
                await unit.PendingInvitations.AddRangeAsync(pendingInvitations);
                await unit.Channels.AddRangeAsync(channels);
                
                await unit.WorkPackageLabels.AddRangeAsync(workPackageLabels);
                await unit.WorkPackages.AddRangeAsync(workPackages);
                await unit.WorkPackageMembers.AddRangeAsync(workPackageMembers);
                await unit.WorkPackageLists.AddRangeAsync(workPackageLists);
                await unit.PendingInvitations.AddRangeAsync(packagePendingInvitations);

                Dictionary<string, string> mapped = new Dictionary<string, string>();
                foreach (var usr in parsed.InviteById)
                {
                    var found = parsed.AllInvited.Single(u => u.Id == Guid.Parse(usr.Id));
                    if (!mapped.ContainsKey(usr.Id)) mapped.Add(usr.Id, found.Email);
                }

                var viewModel = project.ToViewModel(
                    seasons.Select(s => s.ToViewModel()).ToArray(),
                    projectPermissions.Select(p =>
                    {
                        var u = parsed.AllInvited.SingleOrDefault(i => i.Id == p.RecordId);
                        var usr = p.ToViewModel();
                        usr.Member = u;
                        return usr;
                    }).ToArray(),
                    workPackages.Select(w =>
                        w.ToViewModel(
                            workPackageMembers.Select(pm => pm.ToViewModel()).ToArray()
                        )).ToArray(),
                    subProjects.Select(s => s.ToViewModel()).ToArray(),
                    pendingInvitations.Select(p => p.ToViewModel()).ToArray()
                );

                return OperationResult<ProjectPrepareViewModel>.Success(new ProjectPrepareViewModel
                {
                    ViewModel = viewModel,
                    PendingInvitations = pendingInvitations.Select(p => p.ToViewModel()).ToArray(),
                    AllInvited = parsed.AllInvited,
                    InviteById = parsed.InviteById,
                    User = user.ToViewModel(),
                    EmailIdentities = parsed.EmailIdentities
                });
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectPrepareViewModel>.Failed();
            }
        }

        #endregion

        #region Import / Export

        public async Task<OperationResult<ProjectPrepareViewModel>> Import(Guid userId, ImportViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    int packageCounter = 0;
                    Func<WorkPackageTaskViewModel[], WorkPackageViewModel, Task> LoopTasks = async (tasks, package) =>
                    {
                        int innerTaskCounter = 0;
                        foreach (var task in tasks)
                        {
                            innerTaskCounter++;
                            await unit.AddAsync(new WorkPackageTask
                            {
                                Description = task.Description,
                                Order = innerTaskCounter,
                                Restricted = task.Restricted,
                                State = task.State,
                                Title = task.Title,
                                ArchivedAt = task.ArchivedAt,
                                BeginAt = task.BeginAt,
                                BeginReminder = task.BeginReminder,
                                CoverId = task.CoverId,
                                DoneAt = task.DoneAt,
                                DueAt = task.DueAt,
                                EndAt = task.EndAt,
                                EndReminder = task.EndReminder,
                                GeoLocation = task.GeoLocation,
                                ListId = task.ListId,
                                ObjectiveValue = task.ObjectiveValue,
                                ParentId = task.ParentId,
                                PackageId = package.Id,
                                ProjectId = package.ProjectId,
                                SeasonId = task.SeasonId,
                                UserId = task.UserId,
                                VoteNecessity = WorkPackageTaskVoteNecessity.None,
                                VotePaused = task.VotePaused,
                                VotePrivate = task.VotePrivate,
                                DoneUserId = task.DoneUserId,
                                SubProjectId = task.SubProjectId,
                                Id = task.Id,
                                CreatedAt = task.CreatedAt,
                                UpdatedAt = task.UpdatedAt,
                            });

                            foreach (var member in task.Members)
                            {
                                await unit.WorkPackageTaskMember.AddAsync(new WorkPackageTaskMember
                                {
                                    PackageId = package.Id,
                                    RecordId = member.RecordId,
                                    TaskId = task.Id,
                                });
                            }

                            foreach (var label in task.Labels)
                            {
                                await unit.WorkPackageTaskLabels.AddAsync(new WorkPackageTaskLabel
                                {
                                    LabelId = label.LabelId,
                                    PackageId = package.Id,
                                    TaskId = task.Id
                                });
                            }

                            foreach (var comment in task.Comments)
                            {
                                await unit.WorkPackageTaskComments.AddAsync(new WorkPackageTaskComment
                                {
                                    PackageId = package.Id,
                                    ProjectId = package.ProjectId,
                                    TaskId = task.Id,
                                    Message = comment.Message,
                                    UserId = comment.UserId,
                                    CreatedAt = comment.CreatedAt,
                                });
                            }

                            foreach (var attachment in task.Attachments)
                            {
                                await unit.WorkPackageTaskAttachments.AddAsync(new WorkPackageTaskAttachment
                                {
                                    Description = attachment.Description,
                                    Path = attachment.Path,
                                    Title = attachment.Title,
                                    Type = attachment.Type,
                                    CreatedAt = attachment.CreatedAt,
                                    PackageId = package.Id,
                                    ProjectId = package.ProjectId,
                                    TaskId = task.Id,
                                    UserId = attachment.UserId
                                });
                            }
                        }
                    };
                    var op = await PrepareCreate(unit, userId, new ProjectCreateViewModel
                    {
                        Complex = true,
                        Description = model.Description,
                        Title = model.Title,
                        Groups = Array.Empty<InviteViewModel>(),
                        Members = model.Members,
                        Import = true
                    });
                    if (op.Status != OperationResultStatus.Success)
                    {
                        var ret = OperationResult<ProjectPrepareViewModel>.Rejected();
                        ret.Status = op.Status;
                        ret.Exception = op.Exception;
                        return ret;
                    }

                    var foundMe = false;
                    if (model.Teams == null) model.Teams = Array.Empty<GroupViewModel>();
                    
                    foreach (var group in model.Teams)
                    {
                        await unit.Groups.AddAsync(new Group
                        {
                            Id = group.Id,
                            RootId = group.RootId,
                            Type = group.Type,
                            Title = group.Title,
                            Description = group.Description,
                            CreatedAt = group.CreatedAt
                        });
                        await unit.GroupMembers.AddRangeAsync(group.Members.Select(gm => new GroupMember
                        {
                            Access = gm.Access,
                            Id = gm.Id,
                            Level = 1,
                            CreatedAt = gm.CreatedAt,
                            GroupId = gm.GroupId,
                            RootId = gm.GroupId,
                            UserId = gm.UserId
                        }).ToArray());
                        await unit.PendingInvitations.AddRangeAsync(group.Pending.Select(gm => new PendingInvitation
                        {
                            Access = gm.Access,
                            Id = gm.Id,
                            CreatedAt = gm.CreatedAt,
                            RecordId = gm.RecordId,
                            Identifier = gm.Identifier,
                            Type = PendingInvitationType.Group,
                        }).ToArray());
                        if (group.Members.Any(i => i.UserId == userId)) foundMe = true;
                        if (!foundMe)
                        {
                            await unit.GroupMembers.AddAsync(new GroupMember
                            {
                                Access = AccessType.Admin,
                                Id = Guid.NewGuid(),
                                Level = 1,
                                CreatedAt = DateTime.UtcNow,
                                GroupId = group.Id,
                                RootId = group.RootId,
                                UserId = userId
                            });
                        }
                    }

                    foreach (var package in model.Packages)
                    {
                        packageCounter++;
                        package.ProjectId = op.Data.ViewModel.Id;
                        await unit.AddAsync(new WorkPackage
                        {
                            Id = package.Id,
                            Description = package.Description,
                            Title = package.Title,
                            UserId = userId,
                            CommentPermission = WorkPackageCommentPermission.Members,
                            ProjectId = package.ProjectId,
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
                            Premium = op.Data.ViewModel.Premium,
                            Order = packageCounter,
                            TaskVisibility = WorkPackageTaskVisibility.Normal,
                        });

                        foreach (var label in package.Labels)
                        {
                            await unit.WorkPackageLabels.AddAsync(new WorkPackageLabel
                            {
                                Color = label.Color,
                                Id = label.Id,
                                Title = label.Title,
                                PackageId = package.Id,
                            });
                        }

                        foreach (var list in package.Lists)
                        {
                            await unit.WorkPackageLists.AddAsync(new WorkPackageList
                            {
                                Color = list.Color,
                                Id = list.Id,
                                Title = list.Title,
                                PackageId = package.Id,
                                Kanban = list.Kanban,
                                ArchivedAt = list.ArchivedAt
                            });
                        }

                        foreach (var member in package.Members)
                        {
                            if (member.RecordId == userId) foundMe = true;
                            await unit.WorkPackageMembers.AddAsync(new WorkPackageMember
                            {
                                PackageId = package.Id,
                                Access = member.Access,
                                ProjectId = package.ProjectId,
                                RecordId = member.RecordId
                            });
                        }

                        if (!foundMe)
                        {
                            await unit.WorkPackageMembers.AddAsync(new WorkPackageMember
                            {
                                PackageId = package.Id,
                                Access = AccessType.Admin,
                                ProjectId = package.ProjectId,
                                RecordId = userId
                            });
                        }

                        await LoopTasks(package.Tasks, package);
                        foreach (var task in package.Tasks)
                            await LoopTasks(task.SubTasks, package);
                    }

                    op.Data.Plan.UsedWorkPackage += model.Packages.Length;
                    if (op.Data.Plan.UsedWorkPackage > op.Data.Plan.WorkPackage)
                        op.Data.Plan.WorkPackage = op.Data.Plan.UsedWorkPackage;

                    op.Data.Plan.UsedSimpleGroup += model.Teams.Length;
                    if (op.Data.Plan.UsedSimpleGroup > op.Data.Plan.SimpleGroup)
                        op.Data.Plan.SimpleGroup = op.Data.Plan.UsedSimpleGroup;

                    op.Data.Plan.UsedSpace += model.TotalAttachmentSize;
                    if (op.Data.Plan.UsedSpace > op.Data.Plan.Space)
                        op.Data.Plan.Space = op.Data.Plan.UsedSpace;

                    await unit.SaveChangesAsync();

                    Dictionary<string, string> mapped = new Dictionary<string, string>();
                    foreach (var usr in op.Data.InviteById)
                    {
                        var found = op.Data.AllInvited.Single(u => u.Id == Guid.Parse(usr.Id));
                        if (!mapped.ContainsKey(usr.Id)) mapped.Add(usr.Id, found.Email);
                    }

                    op.Data.ViewModel.WorkPackages = model.Packages.Select(p =>
                    {
                        p.Tasks = Array.Empty<WorkPackageTaskViewModel>();
                        return p;
                    }).ToArray();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectAdd,
                        User = op.Data.User,
                        UserId = userId,
                        Project = op.Data.ViewModel,
                        Pendings = op.Data.PendingInvitations
                    });
                    // TODO: send to background
// #pragma warning disable 4014
//                     Task.Run(() =>
//                         postman.InviteProject(op.Data.User.FullName, op.Data.EmailIdentities, mapped,
//                             op.Data.ViewModel));
// #pragma warning restore 4014
                    return OperationResult<ProjectPrepareViewModel>.Success(new ProjectPrepareViewModel
                    {
                        ViewModel = op.Data.ViewModel
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectPrepareViewModel>.Failed();
            }
        }

        public async Task<OperationResult<bool>> Export(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
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

        #endregion

        #region Access

        public async Task<OperationResult<bool>> ChangeAccess(Guid userId, Guid id, ChangeAccessViewModel permission)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.ProjectMembers
                        .SingleOrDefaultAsync(i => i.Id == id);

                    if (access == null) return OperationResult<bool>.NotFound();
                    var checkAccess = await IsAdmin(unit, userId, access.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;
                    access.Access = permission.Access;

                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectMemberPermission,
                        UserId = userId,
                        ProjectMember = access.ToViewModel()
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
                    var accessCheck = await IsAdmin(unit, userId, access.RecordId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    access.Access = permission.Access;
                    await unit.SaveChangesAsync();

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectMemberPermission,
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

        public async Task<OperationResult<bool>> RemovePendingAccess(Guid userId, Guid accessId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.PendingInvitations
                        .SingleOrDefaultAsync(i => i.Id == accessId);
                    if (access == null) return OperationResult<bool>.NotFound();
                    var accessCheck = await IsAdmin(unit, userId, access.RecordId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var planOwnerId = await unit.Projects.Where(i => i.Id == access.RecordId)
                        .Select(i => i.UserId)
                        .SingleOrDefaultAsync();

                    var ownerGroupIds = await unit.GroupMembers
                        .Where(i => i.UserId == planOwnerId)
                        .Select(i => i.GroupId)
                        .ToArrayAsync();

                    var ownerWorkPackageIds = await unit.WorkPackageMembers
                        .Where(i => i.RecordId == planOwnerId || (i.IsGroup && ownerGroupIds.Contains(i.Id)))
                        .Select(i => i.PackageId)
                        .ToArrayAsync();

                    var ownerProjectIds = await unit.ProjectMembers
                        .Where(i => i.RecordId == planOwnerId || (i.IsGroup && ownerGroupIds.Contains(i.Id)))
                        .Select(i => i.ProjectId)
                        .ToArrayAsync();

                    var anyOtherPendingInvitations = await unit.PendingInvitations
                        .Where(i => i.Identifier == access.Identifier &&
                                    (
                                        ownerProjectIds.Contains(i.RecordId) ||
                                        ownerWorkPackageIds.Contains(i.RecordId) ||
                                        ownerGroupIds.Contains(i.RecordId)
                                    )
                        ).ToArrayAsync();


                    var packageIds = await unit.WorkPackages
                        .Where(i => i.ProjectId == access.RecordId)
                        .Select(i => i.Id)
                        .ToArrayAsync();

                    var packagePending = await unit.PendingInvitations
                        .Where(i => packageIds.Contains(i.RecordId) && i.Identifier == access.Identifier)
                        .ToArrayAsync();

                    unit.PendingInvitations.Remove(access);
                    unit.PendingInvitations.RemoveRange(packagePending);
                    
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectMemberRemove,
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
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await unit.ProjectMembers
                        .SingleOrDefaultAsync(i => i.Id == id);

                    if (access == null) return OperationResult<bool>.NotFound();
                    if (access.RecordId != userId)
                    {
                        var checkAccess = await IsAdmin(unit, userId, access.ProjectId);
                        if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;
                    }
                    
                    var workPackageMembers = await (
                        from package in unit.WorkPackages
                        join member in unit.WorkPackageMembers on package.Id equals member.PackageId
                        where package.ProjectId == access.ProjectId && member.RecordId == access.RecordId
                        select member
                    ).ToArrayAsync();

                    var now = DateTime.UtcNow;
                    access.DeletedAt = now;
                    foreach (var member in workPackageMembers) member.DeletedAt = now;
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectMemberRemove,
                        UserId = userId,
                        ProjectMember = access.ToViewModel()
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

        public async Task<OperationResult<bool>> AddAccess(Guid userId, Guid projectId, AccessViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var accessCheck = await IsAdmin(unit, userId, projectId);
                    if (accessCheck.Status != OperationResultStatus.Success) return accessCheck;

                    var user = await unit.Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.Id == userId);

                    var existingProjectMembers = await unit.ProjectMembers
                        .AsNoTracking()
                        .Where(g => g.ProjectId == projectId)
                        .Select(i => i.RecordId.ToString())
                        .ToListAsync();

                    var existingInvites = await unit.PendingInvitations
                        .AsNoTracking()
                        .Where(i => i.RecordId == projectId)
                        .Select(i => i.Identifier)
                        .ToArrayAsync();

                    existingProjectMembers.AddRange(existingInvites);

                    model.Groups = model.Groups.Where(g => !existingProjectMembers.Contains(g.Id)).ToArray();
                    model.Members = model.Members.Where(g => 
                        !existingProjectMembers.Contains(g.Id) &&
                        !existingInvites.Contains(g.Id.Trim().ToLower())
                    ).ToArray();

                    var validation = _serviceProvider.GetService<IValidateBiz>();
                    
                    var parsed = await unit.ParseInvite(userId, validation, model.Members);
                    var project = await unit.Projects.AsNoTracking().SingleAsync(g => g.Id == projectId);
                    var projectPermissions = new List<ProjectMember>();
                    foreach (var invite in parsed.InviteById)
                    {
                        projectPermissions.Add(new ProjectMember
                        {
                            Access = invite.Access,
                            ProjectId = project.Id,
                            RecordId = Guid.Parse(invite.Id),
                            IsGroup = false
                        });
                    }

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
                            projectPermissions.Add(new ProjectMember()
                            {
                                Access = grp.Access,
                                ProjectId = projectId,
                                IsGroup = true,
                                RecordId = groupId,
                            });
                        }
                    }

                    var pendingInvitations = parsed.InviteByEmail.Select(e => new PendingInvitation
                    {
                        Access = e.Access,
                        Identifier = e.Id,
                        RecordId = project.Id,
                        Type = PendingInvitationType.Project
                    }).ToArray();

                    await unit.ProjectMembers.AddRangeAsync(projectPermissions);
                    await unit.PendingInvitations.AddRangeAsync(pendingInvitations);
                    await unit.SaveChangesAsync();

                    Dictionary<string, string> mapped = new Dictionary<string, string>();
                    foreach (var usr in parsed.InviteById)
                    {
                        var found = parsed.AllInvited
                            .Single(u => u.Id == Guid.Parse(usr.Id));
                        mapped.Add(usr.Id, found.Email);
                    }

                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectMemberAdd,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Project = project.ToViewModel(),
                        Pendings = pendingInvitations.Select(p => p.ToViewModel()).ToArray(),
                        ProjectMembers = projectPermissions.Select(p =>
                        {
                            var tmp = p.ToViewModel();
                            tmp.Member = parsed.AllInvited.SingleOrDefault(u => u.Id == p.RecordId);
                            return tmp;
                        }).ToArray()
                    });
                    // TODO: send to background
// #pragma warning disable 4014
//                     Task.Run(() =>
//                         postman.InviteProject(user.FullName, parsed.EmailIdentities, mapped, project.ToViewModel()));
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

        #endregion

        #region Edit

        public async Task<OperationResult<bool>> Archive(Guid userId, Guid projectId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var access = await IsAdmin(unit, userId, projectId);
                    if (access.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    var packageIds = await unit.WorkPackages
                        .Where(i => i.ProjectId == projectId)
                        .Select(i => i.Id)
                        .ToArrayAsync();

                    var now = DateTime.UtcNow;

                    var project = await unit.Projects
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.Id == projectId);

                    // TODO: use linqtodb
                    // await unit.Projects
                    //     .Where(i => i.Id == projectId)
                    //     .UpdateAsync(i => new Project {ArchivedAt = now});
                    //
                    // await unit.WorkPackages
                    //     .Where(i => i.ProjectId == projectId)
                    //     .UpdateAsync(i => new WorkPackage {ArchivedAt = now});
                    //
                    // await unit.WorkPackageLists.Where(i => packageIds.Contains(i.PackageId))
                    //     .UpdateAsync(i => new WorkPackageList {ArchivedAt = now});
                    //
                    // await unit.WorkPackageTasks.Where(i => i.ProjectId == projectId)
                    //     .UpdateAsync(i => new WorkPackageTask {ArchivedAt = now});

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectArchive,
                        UserId = userId,
                        Project = project.ToViewModel()
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

        public async Task<OperationResult<bool>> Remove(Guid userId, Guid projectId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var project = await unit.Projects.SingleOrDefaultAsync(i => i.Id == projectId);

                    if (project == null) return OperationResult<bool>.Rejected();

                    var projectMembers = await unit.ProjectMembers
                        .Where(i => i.ProjectId == projectId)
                        .ToArrayAsync();

                    var access = projectMembers.Any(i =>
                        i.RecordId == userId &&
                        i.ProjectId == projectId &&
                        i.Access == AccessType.Owner
                    );

                    if (!access) return OperationResult<bool>.Rejected();

                    var memberIds = projectMembers.Where(i => !i.IsGroup)
                        .Select(i => i.RecordId)
                        .ToArray();

                    var groupIds = projectMembers
                        .Where(i => i.IsGroup)
                        .Select(i => i.RecordId)
                        .ToArray();

                    var groupMembers = await unit.GroupMembers
                        .Where(g => groupIds.Contains(g.Id))
                        .Select(i => i.UserId)
                        .ToArrayAsync();

                    var members = groupMembers.Concat(memberIds).Distinct().ToArray();

                    var packageIds = await unit.WorkPackages
                        .Where(i => i.ProjectId == projectId)
                        .Select(i => i.Id)
                        .ToArrayAsync();

                    var attachments = await (
                        from attach in unit.WorkPackageTaskAttachments
                        join upload in unit.Uploads on attach.UploadId equals upload.Id
                        where attach.ProjectId == projectId
                        select new {Attachment = attach, Upload = upload}
                    ).ToArrayAsync();

                    if (attachments.Any())
                    {
                        var uploadBiz = _serviceProvider.GetService<IUploadProvider>();

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

                    unit.ProjectMembers.RemoveRange(projectMembers);
                    unit.Projects.Remove(project);
                    
                    // TODO: use linqtodb
                    // await unit.SubProjects.Where(i => i.ProjectId == project.Id).DeleteAsync();
                    // await unit.ProjectSeasons.Where(i => i.ProjectId == project.Id).DeleteAsync();
                    // await unit.WorkPackageObjectives.Where(i => packageIds.Contains(i.PackageId)).DeleteAsync();
                    // await unit.WorkPackageMembers.Where(i => i.ProjectId == project.Id).DeleteAsync();
                    // await unit.WorkPackageTaskTimes.Where(i => i.ProjectId == project.Id).DeleteAsync();
                    // await unit.WorkPackages.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskMember.Where(i => packageIds.Contains(i.PackageId)).DeleteAsync();
                    // await unit.WorkPackageLists.Where(i => packageIds.Contains(i.PackageId)).DeleteAsync();
                    // await unit.WorkPackageTasks.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskTimes.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskBlockers.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskComments.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskInteractions.Where(i => packageIds.Contains(i.PackageId)).DeleteAsync();
                    // await unit.WorkPackageTaskObjectives.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskVotes.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskCustomFields.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageCustomFieldItems.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageCustomFields.Where(i => i.ProjectId == projectId).DeleteAsync();
                    // await unit.WorkPackageTaskLabels.Where(i => packageIds.Contains(i.PackageId)).DeleteAsync();
                    // await unit.WorkPackageRelatedTasks.Where(i => i.ProjectId == projectId).DeleteAsync();
                    //
                    // await unit.Channels.Where(i => i.Id == projectId || packageIds.Contains(i.Id)).DeleteAsync();
                    // await unit.Conversations.Where(i => i.ChannelId == projectId || packageIds.Contains(i.ChannelId))
                    //     .DeleteAsync();
                    // await unit.PendingInvitations
                    //     .Where(i => i.RecordId == project.Id || packageIds.Contains(i.RecordId)).DeleteAsync();

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectRemove,
                        UserId = userId,
                        Project = project.ToViewModel(),
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

        public async Task<OperationResult<bool>> EditProject(Guid userId, Guid id, ProjectEditViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsAdmin(unit, userId, id);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var project = await unit.Projects.SingleAsync(p => p.Id == id);
                    project.Title = model.Title.Trim();
                    project.Description = model.Description.Trim();
                    project.Template = model.Template;
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectEdit,
                        UserId = userId,
                        Project = project.ToViewModel()
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

        #region Season

        public async Task<OperationResult<bool>> CreateSeason(Guid userId, Guid projectId, SimpleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsAdmin(unit, userId, projectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var order = await unit.ProjectSeasons.Where(i => i.ProjectId == projectId)
                        .CountAsync();
                    var season = new ProjectSeason
                    {
                        Description = model.Description.Trim(),
                        Order = order + 1,
                        ProjectId = projectId,
                        UserId = userId,
                        Title = model.Title.Trim()
                    };
                    await unit.ProjectSeasons.AddAsync(season);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectSeasonAdd,
                        UserId = userId,
                        Season = season.ToViewModel()
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

        public async Task<OperationResult<bool>> EditSeason(Guid userId, Guid seasonId, SimpleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var season = await unit.ProjectSeasons.SingleOrDefaultAsync(i => i.Id == seasonId);
                    if (season == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsAdmin(unit, userId, season.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    season.Title = model.Title.Trim();
                    season.Description = model.Description.Trim();

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectSeasonEdit,
                        UserId = userId,
                        Season = season.ToViewModel()
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

        public async Task<OperationResult<bool>> RemoveSeason(Guid userId, Guid seasonId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var season = await unit.ProjectSeasons.SingleOrDefaultAsync(i => i.Id == seasonId);
                    if (season == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsAdmin(unit, userId, season.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    // TODO: use linqtodb
                    // await unit.WorkPackageTasks.Where(i => i.SeasonId == seasonId)
                    //     .UpdateAsync(i => new WorkPackageTask {SeasonId = null});
                    unit.ProjectSeasons.Remove(season);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectSeasonRemove,
                        UserId = userId,
                        Season = season.ToViewModel()
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

        #region Sub Project

        public async Task<OperationResult<bool>> CreateSubProject(Guid userId, Guid projectId,
            CreateSubProjectViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var checkAccess = await IsAdmin(unit, userId, projectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var order = await unit.SubProjects.Where(s =>
                        s.ProjectId == projectId &&
                        ((!model.ParentId.HasValue && s.ParentId == null) ||
                         (model.ParentId.HasValue && s.ParentId == model.ParentId.Value))
                    ).CountAsync();

                    SubProject parent = null;
                    if (model.ParentId.HasValue)
                    {
                        parent = await unit.SubProjects.SingleOrDefaultAsync(s =>
                            s.Id == model.ParentId && s.ProjectId == projectId);

                        if (parent != null && parent.Level >= 3) return OperationResult<bool>.Rejected();
                    }

                    var sub = new SubProject
                    {
                        Description = model.Description,
                        Title = model.Title,
                        Level = parent?.Level + 1 ?? 1,
                        Order = order + 1,
                        ParentId = model.ParentId,
                        ProjectId = projectId,
                        UserId = userId
                    };
                    await unit.SubProjects.AddAsync(sub);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectSubAdd,
                        UserId = userId,
                        SubProject = sub.ToViewModel()
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

        public async Task<OperationResult<bool>> EditSubProject(Guid userId, Guid subId, SimpleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var sub = await unit.SubProjects.SingleOrDefaultAsync(i => i.Id == subId);
                    if (sub == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsAdmin(unit, userId, sub.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    sub.Title = model.Title.Trim();
                    sub.Description = model.Description.Trim();
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectSubEdit,
                        UserId = userId,
                        SubProject = sub.ToViewModel()
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

        public async Task<OperationResult<bool>> RemoveSubProject(Guid userId, Guid subId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var sub = await unit.SubProjects.SingleOrDefaultAsync(i => i.Id == subId);
                    if (sub == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsAdmin(unit, userId, sub.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    // TODO: use linqtodb
                    // await unit.WorkPackages.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackage {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageCustomFields.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageCustomField {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageCustomFieldItems.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageCustomFieldItem {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageTasks.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageTask {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageTaskAttachments.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageTaskAttachment {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageTaskBlockers.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageTaskBlocker {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageRelatedTasks.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageRelatedTask {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageTaskCustomFields.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageTaskCustomField {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageTaskObjectives.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageTaskObjective {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageTaskTimes.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageTaskTime {SubProjectId = sub.ParentId});
                    //
                    // await unit.WorkPackageTaskVotes.Where(i => i.SubProjectId == subId)
                    //     .UpdateAsync(i => new WorkPackageTaskVote {SubProjectId = sub.ParentId});

                    var allSubs = await unit.SubProjects.Where(i => i.ProjectId == sub.ProjectId)
                        .OrderBy(i => i.Order).ToListAsync();
                    var pop = allSubs.Single(i => i.Id == subId);
                    allSubs.Remove(pop);
                    int counter = 1;
                    allSubs.ForEach(s => s.Order = counter++);
                    unit.SubProjects.Remove(sub);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectSubRemove,
                        UserId = userId,
                        SubProject = pop.ToViewModel()
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

        public async Task<OperationResult<bool>> OrderSubProject(Guid userId, Guid subId, ChangeOrderViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var sub = await unit.SubProjects.AsNoTracking().SingleOrDefaultAsync(i => i.Id == subId);
                    if (sub == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsAdmin(unit, userId, sub.ProjectId);
                    if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                    var allSubs = await unit.SubProjects
                        .Where(i => i.ProjectId == sub.ProjectId && i.ParentId == sub.ParentId)
                        .OrderBy(i => i.Order)
                        .ToListAsync();

                    var element = allSubs.Single(s => s.Id == subId);
                    allSubs.Remove(element);
                    allSubs.Insert(model.Order - 1, element);

                    int orders = 1;
                    allSubs.ForEach((e) => { e.Order = orders++; });

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ProjectSubEdit,
                        UserId = userId,
                        SubProject = element.ToViewModel()
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

        #region WBS

        public async Task<OperationResult<TreeViewModel>> Tree(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var groupIds = await unit.FindGroupIds(userId);
                    var access = await unit.ProjectMembers
                        .Where(i => i.RecordId == userId || groupIds.Contains(i.RecordId))
                        .AnyAsync();

                    if (!access) return OperationResult<TreeViewModel>.Rejected();

                    var packages = await unit.FindProjectWorkPackages(userId, id, groupIds);
                    var packageIds = packages.Select(i => i.Id).ToArray();

                    var tasks = await unit.WorkPackageTasks
                        .Where(i => packageIds.Contains(i.PackageId) && !i.DeletedAt.HasValue)
                        .Select(t => new
                        {
                            // t.DueAt,
                            t.BeginAt,
                            t.EndAt,
                            t.Id,
                            t.PackageId,
                            t.SubProjectId,
                            t.State,
                            t.CreatedAt
                        }).AsNoTracking().ToArrayAsync();

                    var taskIds = tasks.Select(i => i.Id).ToArray();
                    var timeSpents = await unit.WorkPackageTaskTimes
                        .Where(i => taskIds.Contains(i.TaskId))
                        .Select(i => new
                        {
                            i.PackageId,
                            i.TaskId,
                            i.Begin,
                            i.End
                        })
                        .AsNoTracking()
                        .ToArrayAsync();

                    var tree = new Dictionary<Guid, TreeReportViewModel>();
                    var tasksGrouped = tasks.GroupBy(i => i.PackageId).ToArray();
                    foreach (var grouping in tasksGrouped)
                    {
                        var timeSpentFiltered = timeSpents
                            .Where(i => i.PackageId == grouping.Key)
                            .ToArray();

                        var timeSpent = timeSpentFiltered
                            .Select(t => ((t.End ?? DateTime.UtcNow) - t.Begin).TotalMilliseconds)
                            .Sum();

                        tree.Add(grouping.Key, new TreeReportViewModel
                        {
                            Done = grouping.Count(t =>
                                t.State == WorkPackageTaskState.Done ||
                                t.State == WorkPackageTaskState.Canceled ||
                                t.State == WorkPackageTaskState.Duplicate
                            ),
                            From = grouping.Min(t => t.BeginAt),
                            To = grouping.Max(t => t.EndAt),
                            Total = grouping.Count(),
                            TimeSpent = timeSpent
                        });
                    }

                    var result = new TreeViewModel { Tree = tree };
                    return OperationResult<TreeViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<TreeViewModel>.Failed();
            }
        }
        
        public async Task<OperationResult<ProjectProgressViewModel[]>> ProjectProgress(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    // TODO: fix reports
                    // var groupIds = await unit.FindGroupIds(userId);
                    // var access = await unit.ProjectMembers
                    //     .Where(i => i.RecordId == userId || groupIds.Contains(i.RecordId))
                    //     .AnyAsync();
                    //
                    // if (!access) return OperationResult<ProjectProgressViewModel[]>.Rejected();
                    //
                    // var packages = await unit.FindProjectWorkPackages(userId, id, groupIds);
                    // var packageIds = packages.Select(i => i.Id).ToArray();

                    // var reportResultSets = await unit.GetMultipleResultSets(
                    //     "exec spGetMountlyTaskReport @ProjectId", 
                    //     new SqlParameter("@ProjectId", SqlDbType.UniqueIdentifier) {Value = id}
                    // );
                    //
                    // var mapped = new Dictionary<string, ProjectProgressViewModel>();
                    // foreach (var set in reportResultSets["Table-1"])
                    // {
                    //     var date = set["Date"].ToString();
                    //     if (!mapped.ContainsKey(date)) mapped.Add(date, new ProjectProgressViewModel{ Date = DateTime.Parse(date)});
                    //     mapped[date].Blocked = Convert.ToInt32(set["Count"]);
                    // }
                    // foreach (var set in reportResultSets["Table-2"])
                    // {
                    //     var date = set["Date"].ToString();
                    //     if (!mapped.ContainsKey(date)) mapped.Add(date, new ProjectProgressViewModel{ Date = DateTime.Parse(date)});
                    //     mapped[date].Done = Convert.ToInt32(set["Count"]);
                    // }
                    // foreach (var set in reportResultSets["Table-3"])
                    // {
                    //     var date = set["Date"].ToString();
                    //     if (!mapped.ContainsKey(date)) mapped.Add(date, new ProjectProgressViewModel{ Date = DateTime.Parse(date)});
                    //     mapped[date].Created = Convert.ToInt32(set["Count"]);
                    // }
                    //
                    // var result = mapped.Select(m => m.Value).ToArray();
                    return OperationResult<ProjectProgressViewModel[]>.Success(Array.Empty<ProjectProgressViewModel>());
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectProgressViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<RoadMapViewModel>> RoadMap(Guid userId, Guid id)
        {
            try
            {
                var result = new RoadMapViewModel();
                return OperationResult<RoadMapViewModel>.Success(result);
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<RoadMapViewModel>.Failed();
            }
        }

        public async Task<OperationResult<WorkPackageObjectiveViewModel[]>> Objectives(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var groupIds = await unit.FindGroupIds(userId);
                    var access = await unit.ProjectMembers
                        .Where(i => i.RecordId == userId || groupIds.Contains(i.RecordId))
                        .AnyAsync();

                    if (!access) return OperationResult<WorkPackageObjectiveViewModel[]>.Rejected();

                    var packages = await unit.FindProjectWorkPackages(userId, id, groupIds);
                    var packageIds = packages.Select(i => i.Id).ToArray();
                    var objectives = await unit.WorkPackageObjectives
                        .Where(i => packageIds.Contains(i.PackageId))
                        .AsNoTracking()
                        .ToArrayAsync();

                    return OperationResult<WorkPackageObjectiveViewModel[]>
                        .Success(objectives.Select(i => i.ToViewModel()).ToArray());
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<WorkPackageObjectiveViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<ProjectObjectiveEstimatedPriceViewModel[]>> ObjectiveDetails(Guid userId,
            Guid id)
        {
            try
            {
                return OperationResult<ProjectObjectiveEstimatedPriceViewModel[]>.Success(
                    Array.Empty<ProjectObjectiveEstimatedPriceViewModel>());
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectObjectiveEstimatedPriceViewModel[]>.Failed();
            }
        }

        #endregion

        #region Fetch

        public async Task<OperationResult<ProjectViewModel[]>> List(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var user = await unit.FindUser(userId);
                    if (user == null) return OperationResult<ProjectViewModel[]>.Rejected();

                    var groupPermissions = await unit.FindGroupPermissions(userId);
                    var groupIds = groupPermissions.Select(i => i.GroupId).ToArray();

                    var projects = await unit.FindProjects(userId, groupIds);
                    var projectWorkPackages = await unit.FindWorkPackages(userId, groupIds);
                    var result = await MapList(unit, userId, groupIds, projects, projectWorkPackages);
                    return result;
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<ProjectViewModel[]>> Archived(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var user = await unit.FindUser(userId);
                    if (user == null) return OperationResult<ProjectViewModel[]>.Rejected();

                    var groupPermissions = await unit.FindGroupPermissions(userId);
                    var groupIds = groupPermissions.Select(i => i.GroupId).ToArray();

                    var projects = (await (
                                from member in unit.ProjectMembers
                                join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                                from proj in tmp.DefaultIfEmpty()
                                where (member.RecordId == userId || groupIds.Contains(member.RecordId)) &&
                                      !member.DeletedAt.HasValue &&
                                      !proj.DeletedAt.HasValue &&
                                      proj.ArchivedAt.HasValue &&
                                      (member.Access == AccessType.Admin || member.Access == AccessType.Owner)
                                select proj
                            )
                            .Distinct()
                            .OrderByDescending(p => p.CreatedAt)
                            .AsNoTracking()
                            .ToArrayAsync())
                        .GroupBy(p => p.Id)
                        .Select(y => y.First())
                        .ToArray();
                    if (!projects.Any()) return OperationResult<ProjectViewModel[]>.Success(Array.Empty<ProjectViewModel>());

                    var projectIds = projects.Select(i => i.Id).ToArray();

                    var projectWorkPackages = await unit.WorkPackages.Where(i => projectIds.Contains(i.ProjectId))
                        .Distinct()
                        .OrderBy(i => i.Order)
                        .AsNoTracking()
                        .ToArrayAsync();
                    return await MapList(unit, userId, groupIds, projects, projectWorkPackages);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<ProjectViewModel>> Fetch(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var groupPermissions = await unit.FindGroupPermissions(userId);
                    var groupIds = groupPermissions.Select(i => i.GroupId).ToArray();

                    var projectMembers = await unit.ProjectMembers
                        .Where(i => i.ProjectId == id)
                        .ToArrayAsync();
                    var projectPermissions = projectMembers.Any(m =>
                        (m.RecordId == userId || groupIds.Contains(m.RecordId)));
                    if (!projectPermissions) return OperationResult<ProjectViewModel>.Rejected();

                    var project = await unit.Projects.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
                    if (project == null) return OperationResult<ProjectViewModel>.NotFound();

                    if (project.ArchivedAt.HasValue && !projectMembers.Any(a =>
                        (a.Access == AccessType.Admin || a.Access == AccessType.Owner)))
                        return OperationResult<ProjectViewModel>.Rejected();

                    var workPackages = await unit.WorkPackages
                        .Where(i => i.ProjectId == id)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var mapped = await MapList(unit, userId, groupIds,
                        new[] {project}, workPackages);

                    var result = mapped.Data.First();
                    result.Members = projectMembers.Select(m => m.ToViewModel()).ToArray();
                    return OperationResult<ProjectViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectViewModel>.Failed();
            }
        }

        #endregion

        #region Private

        private string NormalizeEmail(string email)
        {
            return email.Trim().ToLower();
        }

        private async Task<OperationResult<ProjectViewModel[]>> MapList(
            ProjectManagementDbContext unit, Guid userId, Guid[] groupIds, Project[] projects,
            WorkPackage[] projectWorkPackages)
        {
            var projectIds = projects.Select(i => i.Id).ToArray();
            var projectMembers = await unit.FindProjectsMembers(projectIds);

            var allProjectUserIds = projectMembers
                .Where(i => !i.IsGroup)
                .Select(i => i.RecordId)
                .ToArray();

            var allProjectUsers = await unit.Users
                .Where(i => allProjectUserIds.Contains(i.Id))
                .AsNoTracking()
                .ToArrayAsync();

            var projectSeasons = await (
                    from season in unit.ProjectSeasons
                    join proj in unit.Projects on season.ProjectId equals proj.Id
                    where projectIds.Contains(proj.Id) && !season.DeletedAt.HasValue
                    select season
                )
                .AsNoTracking()
                .ToArrayAsync();

            var projectSubs = await (
                    from sub in unit.SubProjects
                    join proj in unit.Projects on sub.ProjectId equals proj.Id
                    where projectIds.Contains(proj.Id) && !sub.DeletedAt.HasValue
                    select sub
                )
                .AsNoTracking()
                .ToArrayAsync();

            var packageIds = projectWorkPackages.Select(p => p.Id).ToArray();

            var workPackageMembers = await unit.WorkPackageMembers
                .Where(m => packageIds.Contains(m.PackageId))
                .AsNoTracking()
                .ToArrayAsync();

            var pendingMembers = await (
                    from member in unit.PendingInvitations
                    where projectIds.Contains(member.RecordId) || packageIds.Contains(member.RecordId)
                    select member
                )
                .AsNoTracking()
                .ToArrayAsync();

            var result = projects
                .Where(p => p.Complex || projectWorkPackages.Any(w => w.ProjectId == p.Id))
                .Select(g =>
                {
                    var allProjectAccess = projectMembers.Where(p => p.ProjectId == g.Id).ToArray();

                    var currentAccess = allProjectAccess
                        .Where(p => (p.RecordId == userId) || groupIds.Contains(p.RecordId))
                        .Select(i => i.Access)
                        .OrderBy(i => i)
                        .FirstOrDefault();

                    if (currentAccess == 0) currentAccess = AccessType.Visitor;

                    var pendings = pendingMembers
                        .Where(p => p.RecordId == g.Id &&
                                    (
                                        (p.Access != AccessType.HiddenEditor) ||
                                        (
                                            p.Access == AccessType.HiddenEditor &&
                                            (currentAccess == AccessType.Admin ||
                                             currentAccess == AccessType.Owner)
                                        )
                                    ))
                        .Select(p => p.ToViewModel())
                        .ToArray();

                    var members = projectMembers
                        .Where(m => m.ProjectId == g.Id &&
                                    (
                                        (m.Access != AccessType.HiddenEditor) ||
                                        (
                                            m.Access == AccessType.HiddenEditor &&
                                            (currentAccess == AccessType.Admin ||
                                             currentAccess == AccessType.Owner)
                                        )
                                    ) && (!m.IsGroup || groupIds.Contains(m.RecordId)))
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(x => x.ToViewModel(allProjectUsers.SingleOrDefault(u => u.Id == x.RecordId)))
                        .ToArray();

                    var seasons = projectSeasons
                        .Where(s => s.ProjectId == g.Id)
                        .OrderBy(s => s.Order)
                        .Select(s => s.ToViewModel())
                        .ToArray();
                    var workPackages = projectWorkPackages
                        .Where(s => s.ProjectId == g.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(s =>
                        {
                            var allPackageAccess = workPackageMembers
                                .Where(p => p.PackageId == s.Id).ToArray();

                            var currentPackageAccess = allPackageAccess.Where(p =>
                                    (p.RecordId == userId) || groupIds.Contains(p.RecordId))
                                .Select(i => i.Access).OrderBy(i => i).FirstOrDefault();
                            if (currentPackageAccess == 0) currentPackageAccess = AccessType.Visitor;
                            var packagePending = pendings
                                .Where(i => i.RecordId == s.Id).ToArray();
                            var packageMembers = workPackageMembers
                                .Where(m => m.PackageId == s.Id &&
                                            (
                                                (m.Access != AccessType.HiddenEditor) ||
                                                (
                                                    m.Access == AccessType.HiddenEditor &&
                                                    (currentPackageAccess == AccessType.Admin ||
                                                     currentPackageAccess == AccessType.Owner)
                                                )
                                            ) && (!m.IsGroup || groupIds.Contains(m.RecordId)))
                                .OrderByDescending(m => m.CreatedAt)
                                .Select(w => w.ToViewModel())
                                .ToArray();
                            return s.ToViewModel(packageMembers, packagePending);
                        }).ToArray();
                    var subs = projectSubs
                        .Where(s => s.ProjectId == g.Id)
                        .OrderBy(s => s.Order)
                        .Select(s => s.ToViewModel())
                        .ToArray();
                    return g.ToViewModel(seasons, members, workPackages, subs, pendings);
                }).ToArray();
            return OperationResult<ProjectViewModel[]>.Success(result);
        }

        private async Task<OperationResult<bool>> IsAdmin(ProjectManagementDbContext unit, Guid userId, Guid projectId)
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

        #endregion
    }
}