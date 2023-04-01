using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Collaboration;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace Asoode.Business.ProjectManagement
{
    public static class CollaborationDbContextExtensions
    {
        
        public static async Task<ParsedInviteViewModel> ParseInvite(
            this CollaborationDbContext unit,
            Guid userId, 
            UserPlanInfo plan, 
            IValidateBiz validation,
            InviteViewModel[] members
        )
        {
            var planMembers = await unit.FindPlanMembers(plan.Id);

            var inviteByEmail = members.Where(m => validation.IsEmail(m.Id)).ToList();
            var inviteById = members.Except(inviteByEmail).ToList();
            var idIdentities = inviteById.Select(i => Guid.Parse(i.Id)).ToArray();
            var emailIdentities = inviteByEmail.Select(i => i.Id.Trim().ToLower()).ToArray();
            var allInvited = await unit.Users.Where(u =>
                emailIdentities.Contains(u.Email) ||
                idIdentities.Contains(u.Id) || u.Id == userId
            ).AsNoTracking().ToArrayAsync();
            var missing = allInvited.Where(i => emailIdentities.Contains(i.Email)).ToArray();
            foreach (var usr in missing)
            {
                var found = inviteByEmail.Single(u => u.Id == usr.Email);
                found.Id = usr.Id.ToString();
                inviteByEmail.Remove(found);
                inviteById.Add(found);
            }

            emailIdentities = inviteByEmail.Select(i => i.Id).ToArray();
            idIdentities = inviteById.Select(i => Guid.Parse(i.Id)).ToArray();

            var newMembers = emailIdentities.Concat(inviteById.Select(i => i.Id).ToArray())
                .Except(planMembers).ToArray();

            return new ParsedInviteViewModel
            {
                NewMembers = newMembers,
                EmailIdentities = emailIdentities,
                InviteById = inviteById,
                InviteByEmail = inviteByEmail,
                IdIdentities = idIdentities,
                AllInvited = allInvited.Select(u => u.ToViewModel()).ToArray()
            };
        }
        
        public static async Task<User> FindUser(this CollaborationDbContext unit, Guid userId, bool skipTracking = true)
        {
            var query = skipTracking ? unit.Users.AsNoTracking() : unit.Users;
            var user = await query.SingleOrDefaultAsync(i => i.Id == userId);
            if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
                return null;
            return user;
        }
        
        public static async Task<Guid[]> FindGroupIds(this CollaborationDbContext unit, Guid userId)
        {
            var groups = await FindGroups(unit, userId);
            return groups.Select(i => i.Id).ToArray();
        }
        public static async Task<Group[]> FindGroups(this CollaborationDbContext unit, Guid userId)
        {
            var groups = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                where member.UserId == userId &&
                      !grp.ArchivedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select grp
            ).Distinct().AsNoTracking().ToArrayAsync();
            return groups;
        }
        public static async Task<Tuple<Group, UserPlanInfo>[]> FindGroupsWithPlans(this CollaborationDbContext unit, Guid userId)
        {
            var groups = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                join plan in unit.UserPlanInfo on grp.PlanInfoId equals plan.Id
                where member.UserId == userId &&
                      !grp.ArchivedAt.HasValue &&
                      !grp.DeletedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select new { Group = grp, Plan = plan }
            ).Distinct().AsNoTracking().ToArrayAsync();
            return groups.Select(i => new Tuple<Group, UserPlanInfo>(i.Group, i.Plan)).ToArray();
        }
        public static async Task<GroupMember[]> FindGroupMembers(this CollaborationDbContext unit, Guid[] groupIds)
        {
            var result = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                where groupIds.Contains(grp.Id) &&
                      !grp.ArchivedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select member
            ).AsNoTracking().ToArrayAsync();
            
            return result;
        }

        public static async Task<UserPlanInfo> FindPlan(this CollaborationDbContext unit, Guid userId)
        {
            return await unit.UserPlanInfo
                .OrderByDescending(i => i.CreatedAt)
                .FirstAsync(g => g.UserId == userId);
        }
        public static async Task<Group> FindGroup(this CollaborationDbContext unit, Guid groupId, bool skipTracking = false)
        {
            var query = skipTracking ? unit.Groups.AsNoTracking() : unit.Groups;
            return await query.FirstAsync(g => g.Id == groupId);
        }
        public static async Task<string[]> FindPlanMembers(this CollaborationDbContext unit, Guid planId)
        {
            return await unit.PlanMembers.Where(p => p.PlanId == planId)
                .AsNoTracking()
                .Select(i => i.Identifier)
                .ToArrayAsync();
        }
    }
    
    public static class ProjectManagementDbContextExtensions
    {
        public static async Task<User> FindUser(this ProjectManagementDbContext unit, Guid userId)
        {
            var user = await unit.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == userId);
            if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
                return null;
            return user;
        }
        public static async Task<Guid[]> FindGroupIds(this ProjectManagementDbContext unit, Guid userId)
        {
            var groups = await FindGroups(unit, userId);
            return groups.Select(i => i.Id).ToArray();
        }
        public static async Task<Group[]> FindGroups(this ProjectManagementDbContext unit, Guid userId)
        {
            var groups = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                where member.UserId == userId &&
                      !grp.ArchivedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select grp
            ).Distinct().AsNoTracking().ToArrayAsync();
            return groups;
        }
        public static async Task<GroupMember[]> FindGroupPermissions(this ProjectManagementDbContext unit, Guid userId)
        {
            var result = await (
                from member in unit.GroupMembers
                join grp in unit.Groups on member.GroupId equals grp.Id
                where member.UserId == userId &&
                      !grp.ArchivedAt.HasValue &&
                      !member.DeletedAt.HasValue
                select member
            ).AsNoTracking().ToArrayAsync();
            
            return result;
        }
        public static async Task<Guid[]> FindProjectIds(this ProjectManagementDbContext unit, Guid userId, Guid[] groupIds)
        {
            var result = await FindProjects(unit, userId, groupIds);
            return result.Select(i => i.Id).ToArray();
        }
        public static async Task<Project[]> FindProjects(this ProjectManagementDbContext unit, Guid userId, Guid[] groupIds)
        {
            var result = (await (
                        from member in unit.ProjectMembers
                        join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                        from proj in tmp.DefaultIfEmpty()
                        where (member.RecordId == userId || groupIds.Contains(member.RecordId)) &&
                              !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue && !proj.ArchivedAt.HasValue
                        select proj
                    )
                    .Distinct()
                    .OrderByDescending(p => p.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync())
                .GroupBy(p => p.Id)
                .Select(y => y.First())
                .ToArray();
            
            return result;
        }
        public static async Task<Tuple<Project, UserPlanInfo>[]> FindProjectsWithPlans(this ProjectManagementDbContext unit, Guid userId, Guid[] groupIds)
        {
            var result = (await (
                        from proj in unit.Projects
                        join member in unit.ProjectMembers on proj.Id equals member.ProjectId
                        join plan in unit.UserPlanInfo on proj.PlanInfoId equals plan.Id
                        where (member.RecordId == userId || groupIds.Contains(member.RecordId)) &&
                              !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue && !proj.ArchivedAt.HasValue
                        select new { Project = proj, Plan = plan }
                    )
                    .Distinct()
                    .OrderByDescending(p => p.Project.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync())
                .GroupBy(p => p.Project.Id)
                .Select(y => y.First())
                .ToArray();
            
            return result.Select(i => new Tuple<Project, UserPlanInfo>(i.Project, i.Plan)).ToArray();
        }
        public static async Task<Guid[]> FindWorkPackageIds(this ProjectManagementDbContext unit, Guid userId, Guid[] groupIds)
        {
            var result = await FindWorkPackages(unit, userId, groupIds);
            return result.Select(i => i.Id).ToArray();
        }
        public static async Task<WorkPackage[]> FindWorkPackages(this ProjectManagementDbContext unit, Guid userId, Guid[] groupIds)
        {
            var result = await (
                    from pkg in unit.WorkPackages
                    join member in unit.WorkPackageMembers on pkg.Id equals member.PackageId
                    join project in unit.Projects on pkg.ProjectId equals project.Id
                    where !pkg.DeletedAt.HasValue && 
                          !pkg.ArchivedAt.HasValue && 
                          !member.DeletedAt.HasValue &&
                          !project.DeletedAt.HasValue && 
                          !project.ArchivedAt.HasValue &&
                          (member.RecordId == userId || groupIds.Contains(member.RecordId))
                    select pkg
                )
                .Distinct()
                .OrderBy(i => i.Order)
                .AsNoTracking()
                .ToArrayAsync();

            return result;
        }
        public static async Task<WorkPackage[]> FindProjectWorkPackages(this ProjectManagementDbContext unit, Guid userId, Guid projectId, Guid[] groupIds)
        {
            var result = await (
                    from pkg in unit.WorkPackages
                    join member in unit.WorkPackageMembers on pkg.Id equals member.PackageId
                    join project in unit.Projects on pkg.ProjectId equals project.Id
                    where !pkg.DeletedAt.HasValue && 
                          !pkg.ArchivedAt.HasValue && 
                          !member.DeletedAt.HasValue &&
                          !project.DeletedAt.HasValue && 
                          !project.ArchivedAt.HasValue &&
                          project.Id == projectId &&
                          (member.RecordId == userId || groupIds.Contains(member.RecordId))
                    select pkg
                ).Distinct()
                .AsNoTracking()
                .ToArrayAsync();

            return result;
        }
        public static async Task<ProjectMember[]> FindProjectPermissions(this ProjectManagementDbContext unit, Guid userId, Guid[] groupIds)
        {
            var result = await (
                    from member in unit.ProjectMembers
                    join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                    from proj in tmp.DefaultIfEmpty()
                    where (member.RecordId == userId || groupIds.Contains(member.RecordId)) &&
                          !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue && !proj.ArchivedAt.HasValue
                    select member
                )
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToArrayAsync();
            
            return result;
        }
        public static async Task<ProjectMember[]> FindProjectsMembers(this ProjectManagementDbContext unit, Guid[] projectIds)
        {
            var result = await (
                    from member in unit.ProjectMembers
                    join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                    from proj in tmp.DefaultIfEmpty()
                    where projectIds.Contains(proj.Id) && !member.DeletedAt.HasValue && 
                          !proj.DeletedAt.HasValue && !proj.ArchivedAt.HasValue
                    select member
                )
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToArrayAsync();
            
            return result;
        }
        public static async Task<UserPlanInfo> FindPlan(this ProjectManagementDbContext unit, Guid userId)
        {
            return await unit.UserPlanInfo
                .OrderByDescending(i => i.CreatedAt)
                .FirstAsync(g => g.UserId == userId);
        }
        public static async Task<string[]> FindPlanMembers(this ProjectManagementDbContext unit, Guid planId)
        {
            return await unit.PlanMembers.Where(p => p.PlanId == planId)
                .AsNoTracking()
                .Select(i => i.Identifier)
                .ToArrayAsync();
        }

        public static async Task<ParsedInviteViewModel> ParseInvite(
            this ProjectManagementDbContext unit,
            Guid userId, 
            UserPlanInfo plan, 
            IValidateBiz validation,
            InviteViewModel[] members
        )
        {
            var planMembers = await unit.FindPlanMembers(plan.Id);

            var inviteByEmail = members.Where(m => validation.IsEmail(m.Id)).ToList();
            var inviteById = members.Except(inviteByEmail).ToList();
            var idIdentities = inviteById.Select(i => Guid.Parse(i.Id)).ToArray();
            var emailIdentities = inviteByEmail.Select(i => i.Id.Trim().ToLower()).ToArray();
            var allInvited = await unit.Users.Where(u =>
                emailIdentities.Contains(u.Email) ||
                idIdentities.Contains(u.Id) || u.Id == userId
            ).AsNoTracking().Distinct().ToArrayAsync();
            var missing = allInvited.Where(i => emailIdentities.Contains(i.Email)).ToArray();
            foreach (var usr in missing)
            {
                var found = inviteByEmail.Single(u => u.Id == usr.Email);
                found.Id = usr.Id.ToString();
                inviteByEmail.Remove(found);
                inviteById.Add(found);
            }

            emailIdentities = inviteByEmail.Select(i => i.Id).ToArray();
            idIdentities = inviteById.Select(i => Guid.Parse(i.Id)).ToArray();

            var newMembers = emailIdentities.Concat(inviteById.Select(i => i.Id).ToArray())
                .Except(planMembers).ToArray();

            return new ParsedInviteViewModel
            {
                NewMembers = newMembers,
                EmailIdentities = emailIdentities,
                InviteById = inviteById,
                InviteByEmail = inviteByEmail,
                IdIdentities = idIdentities,
                AllInvited = allInvited.Select(u => u.ToViewModel()).ToArray()
            };
        }
    }
}