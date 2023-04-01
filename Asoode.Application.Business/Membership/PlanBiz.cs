using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Business.Extensions;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Membership.Plan;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.Membership
{
    internal class PlanBiz : IPlanBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public PlanBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<OperationResult<PlansFetchViewModel>> Fetch(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var query = await (
                        from user in unit.Users
                        join planInfo in unit.UserPlanInfo on user.Id equals planInfo.UserId
                        orderby planInfo.CreatedAt descending
                        where user.Id == userId
                        select new {User = user, PlanInfo = planInfo}
                    ).AsNoTracking().FirstOrDefaultAsync();

                    if (query == null) return OperationResult<PlansFetchViewModel>.NotFound();
                    if (query.User.IsLocked || query.User.Blocked)
                        return OperationResult<PlansFetchViewModel>.Rejected();

                    var plans = await unit.Plans
                        .Where(i => i.Enabled)
                        .OrderBy(i => i.Order).AsNoTracking()
                        .ToArrayAsync();

                    return OperationResult<PlansFetchViewModel>.Success(new PlansFetchViewModel
                    {
                        Mine = query.PlanInfo.ToViewModel(),
                        Plans = plans.Select(p => p.ToViewModel()).ToArray(),
                        ValueAdded = 9
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<PlansFetchViewModel>.Failed();
            }
        }

        public async Task<OperationResult<SelectableItem<Guid>[]>> AdminPlansAll(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    var query = await unit.Plans.OrderBy(i => i.Order)
                        .AsNoTracking()
                        .ToArrayAsync();
                    return OperationResult<SelectableItem<Guid>[]>.Success(
                        query.Select(i => new SelectableItem<Guid>
                        {
                            Text = i.Title,
                            Value = i.Id
                        }).ToArray()
                    );
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<SelectableItem<Guid>[]>.Failed();
            }
        }
        
        public async Task<OperationResult<GridResult<PlanViewModel>>> AdminPlansList(Guid userId, GridFilter model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    var query = unit.Plans.OrderBy(i => i.Order);
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
                return OperationResult<GridResult<PlanViewModel>>.Failed();
            }
        }

        public async Task<OperationResult<bool>> AdminCreate(Guid userId, PlanViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    var plan = new Plan
                    {
                        Days = model.Days,
                        Description = model.Description,
                        Enabled = model.Enabled,
                        Order = model.Order,
                        Picture = model.Picture,
                        Project = model.Project,
                        Title = model.Title,
                        Type = model.Type,
                        Unit = model.Unit,
                        Users = model.Users,
                        AttachmentSize = model.AttachmentSize,
                        CanExtend = model.CanExtend,
                        ComplexGroup = model.ComplexGroup,
                        DiskSpace = model.DiskSpace,
                        FeatureBlocking = model.FeatureBlocking,
                        FeatureCalendar = model.FeatureCalendar,
                        FeatureChat = model.FeatureChat,
                        FeatureFiles = model.FeatureFiles,
                        FeatureKartabl = model.FeatureKartabl,
                        FeatureObjectives = model.FeatureObjectives,
                        FeaturePayments = model.FeaturePayments,
                        FeatureRelated = model.FeatureRelated,
                        FeatureReports = model.FeatureReports,
                        FeatureSeasons = model.FeatureSeasons,
                        FeatureShift = model.FeatureShift,
                        FeatureTree = model.FeatureTree,
                        FeatureVote = model.FeatureVote,
                        FeatureWbs = model.FeatureWbs,
                        OneTime = model.OneTime,
                        PlanCost = model.PlanCost,
                        SimpleGroup = model.SimpleGroup,
                        WorkPackage = model.WorkPackage,
                        AdditionalProjectCost = model.AdditionalProjectCost,
                        AdditionalSpaceCost = model.AdditionalSpaceCost,
                        AdditionalUserCost = model.AdditionalUserCost,
                        FeatureComplexGroup = model.FeatureComplexGroup,
                        FeatureCustomField = model.FeatureCustomField,
                        FeatureRoadMap = model.FeatureRoadMap,
                        FeatureSubTask = model.FeatureSubTask,
                        FeatureTimeOff = model.FeatureTimeOff,
                        FeatureTimeSpent = model.FeatureTimeSpent,
                        FeatureTimeValue = model.FeatureTimeValue,
                        AdditionalComplexGroupCost = model.AdditionalComplexGroupCost,
                        AdditionalSimpleGroupCost = model.AdditionalSimpleGroupCost,
                        AdditionalWorkPackageCost = model.AdditionalWorkPackageCost,
                        FeatureGroupTimeSpent = model.FeatureGroupTimeSpent,
                    };

                    await unit.Plans.AddAsync(plan);
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

        public async Task<OperationResult<bool>> AdminToggle(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    var plan = await unit.Plans.SingleOrDefaultAsync(i => i.Id == id);
                    plan.Enabled = !plan.Enabled;
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

        public async Task<OperationResult<bool>> AdminEdit(Guid userId, Guid id, PlanViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    var plan = await unit.Plans.SingleOrDefaultAsync(i => i.Id == id);
                    plan.Days = model.Days;
                    plan.Description = model.Description;
                    plan.Enabled = model.Enabled;
                    plan.Order = model.Order;
                    plan.Picture = model.Picture;
                    plan.Project = model.Project;
                    plan.Title = model.Title;
                    plan.Type = model.Type;
                    plan.Unit = model.Unit;
                    plan.Users = model.Users;
                    plan.AttachmentSize = model.AttachmentSize;
                    plan.CanExtend = model.CanExtend;
                    plan.ComplexGroup = model.ComplexGroup;
                    plan.DiskSpace = model.DiskSpace;
                    plan.FeatureBlocking = model.FeatureBlocking;
                    plan.FeatureCalendar = model.FeatureCalendar;
                    plan.FeatureChat = model.FeatureChat;
                    plan.FeatureFiles = model.FeatureFiles;
                    plan.FeatureKartabl = model.FeatureKartabl;
                    plan.FeatureObjectives = model.FeatureObjectives;
                    plan.FeaturePayments = model.FeaturePayments;
                    plan.FeatureRelated = model.FeatureRelated;
                    plan.FeatureReports = model.FeatureReports;
                    plan.FeatureSeasons = model.FeatureSeasons;
                    plan.FeatureShift = model.FeatureShift;
                    plan.FeatureTree = model.FeatureTree;
                    plan.FeatureVote = model.FeatureVote;
                    plan.FeatureWbs = model.FeatureWbs;
                    plan.OneTime = model.OneTime;
                    plan.PlanCost = model.PlanCost;
                    plan.SimpleGroup = model.SimpleGroup;
                    plan.WorkPackage = model.WorkPackage;
                    plan.AdditionalProjectCost = model.AdditionalProjectCost;
                    plan.AdditionalSpaceCost = model.AdditionalSpaceCost;
                    plan.AdditionalUserCost = model.AdditionalUserCost;
                    plan.FeatureComplexGroup = model.FeatureComplexGroup;
                    plan.FeatureCustomField = model.FeatureCustomField;
                    plan.FeatureRoadMap = model.FeatureRoadMap;
                    plan.FeatureSubTask = model.FeatureSubTask;
                    plan.FeatureTimeOff = model.FeatureTimeOff;
                    plan.FeatureTimeSpent = model.FeatureTimeSpent;
                    plan.FeatureTimeValue = model.FeatureTimeValue;
                    plan.AdditionalComplexGroupCost = model.AdditionalComplexGroupCost;
                    plan.AdditionalSimpleGroupCost = model.AdditionalSimpleGroupCost;
                    plan.AdditionalWorkPackageCost = model.AdditionalWorkPackageCost;
                    plan.FeatureGroupTimeSpent = model.FeatureGroupTimeSpent;
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

        public async Task<OperationResult<bool>> AdminEditUser(Guid userId, Guid id, UserPlanInfoViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<GeneralDbContext>())
                {
                    var plan = await unit.UserPlanInfo
                        .Where(i => i.UserId == id)
                        .OrderByDescending(i => i.CreatedAt)
                        .FirstAsync();
                    
                    plan.Days = model.Days;
                    plan.Description = model.Description;
                    plan.Enabled = model.Enabled;
                    plan.Picture = model.Picture;
                    plan.Project = model.Project;
                    plan.Title = model.Title;
                    plan.Type = model.Type;
                    plan.Unit = model.Unit;
                    plan.Users = model.Users;
                    plan.AttachmentSize = model.AttachmentSize;
                    plan.CanExtend = model.CanExtend;
                    plan.ComplexGroup = model.ComplexGroup;
                    plan.Space = model.Space;
                    plan.FeatureBlocking = model.FeatureBlocking;
                    plan.FeatureCalendar = model.FeatureCalendar;
                    plan.FeatureChat = model.FeatureChat;
                    plan.FeatureFiles = model.FeatureFiles;
                    plan.FeatureKartabl = model.FeatureKartabl;
                    plan.FeatureObjectives = model.FeatureObjectives;
                    plan.FeaturePayments = model.FeaturePayments;
                    plan.FeatureRelated = model.FeatureRelated;
                    plan.FeatureReports = model.FeatureReports;
                    plan.FeatureSeasons = model.FeatureSeasons;
                    plan.FeatureShift = model.FeatureShift;
                    plan.FeatureTree = model.FeatureTree;
                    plan.FeatureVote = model.FeatureVote;
                    plan.FeatureWbs = model.FeatureWbs;
                    plan.OneTime = model.OneTime;
                    plan.PlanCost = model.PlanCost;
                    plan.SimpleGroup = model.SimpleGroup;
                    plan.WorkPackage = model.WorkPackage;
                    plan.AdditionalProjectCost = model.AdditionalProjectCost;
                    plan.AdditionalSpaceCost = model.AdditionalSpaceCost;
                    plan.AdditionalUserCost = model.AdditionalUserCost;
                    plan.FeatureComplexGroup = model.FeatureComplexGroup;
                    plan.FeatureCustomField = model.FeatureCustomField;
                    plan.FeatureRoadMap = model.FeatureRoadMap;
                    plan.FeatureSubTask = model.FeatureSubTask;
                    plan.FeatureTimeOff = model.FeatureTimeOff;
                    plan.FeatureTimeSpent = model.FeatureTimeSpent;
                    plan.FeatureTimeValue = model.FeatureTimeValue;
                    plan.AdditionalComplexGroupCost = model.AdditionalComplexGroupCost;
                    plan.AdditionalSimpleGroupCost = model.AdditionalSimpleGroupCost;
                    plan.AdditionalWorkPackageCost = model.AdditionalWorkPackageCost;
                    plan.FeatureGroupTimeSpent = model.FeatureGroupTimeSpent;

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

    }
}