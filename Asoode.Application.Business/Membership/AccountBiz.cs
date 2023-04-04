using System.Security.Claims;
using System.Text;
using Asoode.Application.Business.Extensions;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Contracts.Membership;
using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.Extensions;
using Asoode.Application.Core.Helpers;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Application.Core.ViewModels.Membership.Authentication;
using Asoode.Application.Core.ViewModels.Membership.Profile;
using Asoode.Application.Core.ViewModels.Storage;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Base;
using Asoode.Application.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.Membership
{
    internal class AccountBiz : IAccountBiz
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IValidateBiz _validateBiz;
        public AccountBiz(IServiceProvider serviceProvider, IValidateBiz validateBiz)
        {
            _serviceProvider = serviceProvider;
            _validateBiz = validateBiz;
        }

        #region Panel

        public async Task<OperationResult<SelectableItem<string>[]>> InviteQuery(Guid userId, AutoCompleteFilter filter)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    if (string.IsNullOrEmpty(filter.Search) || string.IsNullOrWhiteSpace(filter.Search))
                        return OperationResult<SelectableItem<string>[]>.Success(Array.Empty<SelectableItem<string>>());
                    
                    var validate = _serviceProvider.GetService<IValidateBiz>();
                    filter.Search = filter.Search.Trim().ToLower().ConvertDigitsToLatin();
                    var email = validate.IsEmail(filter.Search);
                    var mobile = validate.IsMobile(filter.Search);
                    if (mobile) filter.Search = validate.PrefixMobileNumber(filter.Search);
                    var hasAt = filter.Search.Contains("@");
                    if (email || mobile || hasAt || filter.Search.Length > 7)
                    {
                        var users = await unit.Users.AsNoTracking().Where(u =>
                            (email && u.Email == filter.Search) ||
                            (mobile && u.Phone == filter.Search) ||
                            u.Email.Contains(filter.Search)
                        ).ToListAsync();
                        var result = users.Select(user => new SelectableItem<string>
                        {
                            Text = user.FullName,
                            Value = user.Email,
                            Payload = user.ToViewModel()
                        }).ToArray();
                        return OperationResult<SelectableItem<string>[]>.Success(result);
                    }

                    // find among related people 
                    var members = new List<User>();
                    if (filter.Search.StartsWith("00")) filter.Search = filter.Search.Substring(2);
                    if (filter.Search.StartsWith("0")) filter.Search = filter.Search.Substring(1);

                    var groups = await unit.GroupMembers
                        .Where(g => g.UserId == userId)
                        .Select(i => i.GroupId)
                        .ToArrayAsync();
                    var projects = await unit.ProjectMembers
                        .Where(g => g.RecordId == userId && !g.IsGroup)
                        .Select(i => i.ProjectId)
                        .ToArrayAsync();
                    var workPackages = await unit.WorkPackageMembers
                        .Where(g => g.RecordId == userId && !g.IsGroup)
                        .Select(i => i.PackageId)
                        .ToArrayAsync();
                    var split = filter.Search.Split(" ");
                    members.AddRange(await (
                        from member in unit.GroupMembers
                        join grp in unit.Groups on member.GroupId equals grp.Id
                        join usr in unit.Users on member.Id equals usr.Id
                        where groups.Contains(grp.Id) && (
                            split.Contains(usr.FirstName) ||
                            split.Contains(usr.LastName) ||
                            usr.Email.Contains(filter.Search) ||
                            usr.Phone.Contains(filter.Search)
                        )
                        select usr
                    ).AsNoTracking().ToArrayAsync());
                    members.AddRange(await (
                        from member in unit.ProjectMembers
                        join proj in unit.Projects on member.ProjectId equals proj.Id
                        join usr in unit.Users on member.Id equals usr.Id
                        where projects.Contains(proj.Id) && (
                            split.Contains(usr.FirstName) ||
                            split.Contains(usr.LastName) ||
                            usr.Email.Contains(filter.Search) ||
                            usr.Phone.Contains(filter.Search)
                        )
                        select usr
                    ).AsNoTracking().ToArrayAsync());
                    members.AddRange(await (
                        from member in unit.WorkPackageMembers
                        join wp in unit.WorkPackages on member.PackageId equals wp.Id
                        join usr in unit.Users on member.Id equals usr.Id
                        where workPackages.Contains(wp.Id) && (
                            split.Contains(usr.FirstName) ||
                            split.Contains(usr.LastName) ||
                            usr.Email.Contains(filter.Search) ||
                            usr.Phone.Contains(filter.Search)
                        )
                        select usr
                    ).AsNoTracking().ToArrayAsync());

                    var distinct = members.GroupBy(m => m.Id)
                        .Select(y => y.First())
                        .ToArray()
                        .Select(user => new SelectableItem<string>
                        {
                            Text = user.FullName,
                            Value = user.Email,
                            Payload = user.ToViewModel()
                        }).ToArray();

                    return OperationResult<SelectableItem<string>[]>.Success(distinct);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<SelectableItem<string>[]>.Failed();
            }
        }

        public async Task<OperationResult<Guid?>> ChangeEmail(Guid userId, ChangeEmailViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var email = NormalizeEmail(model.Email);
                    if (!_validateBiz.IsEmail(model.Email)) return OperationResult<Guid?>.Rejected();

                    var existing = await unit.Users.AnyAsync(i => i.Id != userId && i.Email == email);
                    if (existing) return OperationResult<Guid?>.Duplicate();

                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null) return OperationResult<Guid?>.NotFound();

                    var id = Guid.NewGuid();
                    var sameEmail = string.Equals(user.Email, email, StringComparison.InvariantCultureIgnoreCase);
                    if (!sameEmail || !user.LastEmailConfirmed.HasValue)
                    {
                        user.Email = email;
                        user.LastEmailConfirmed = null;
                        var token = CryptoHelper.GeneratePhoneConfirmationCode();
                        var sent = await _serviceProvider.GetService<IPostmanBiz>()
                            .EmailChange(user.Id.ToString(), user.Email, token);
                        if (sent.Status != OperationResultStatus.Success)
                            return OperationResult<Guid?>.Failed();
                        await unit.UserVerifications.AddAsync(new UserVerification
                        {
                            Id = id,
                            Code = token,
                            ExpireAt = DateTime.UtcNow.AddMinutes(10),
                            LastSend = DateTime.UtcNow,
                            UserId = user.Id,
                            Email = email
                        });

                        await unit.SaveChangesAsync();
                    }

                    return OperationResult<Guid?>.Success(id);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<Guid?>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ChangePassword(Guid userId, ChangePasswordViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null) return OperationResult<bool>.NotFound();
                    if (!CheckPassword(user, model.OldPassword)) return OperationResult<bool>.Rejected();
                    HashPassword(user, model.NewPassword);
                    user.UpdatedAt = DateTime.UtcNow;
                    await unit.SaveChangesAsync();
                    return OperationResult<bool>.Success();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<Guid?>> ChangePhone(Guid userId, ChangePhoneViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var phone = NormalizePhone(model.Phone);
                    if (!_validateBiz.IsMobile(model.Phone)) return OperationResult<Guid?>.Rejected();

                    var existing = await unit.Users.AnyAsync(i => i.Id != userId && i.Phone == phone);
                    if (existing) return OperationResult<Guid?>.Duplicate();

                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null) return OperationResult<Guid?>.NotFound();

                    var id = Guid.NewGuid();
                    var sameNumber = string.Equals(user.Phone, phone, StringComparison.InvariantCultureIgnoreCase);
                    if (!sameNumber || !user.LastPhoneConfirmed.HasValue)
                    {
                        user.Phone = phone;
                        user.LastPhoneConfirmed = null;
                        var token = CryptoHelper.GeneratePhoneConfirmationCode();
                        var sent = await _serviceProvider.GetService<IPostmanBiz>()
                            .PhoneChange(user.Phone, token);
                        if (sent.Status != OperationResultStatus.Success)
                            return OperationResult<Guid?>.Failed();
                        await unit.UserVerifications.AddAsync(new UserVerification
                        {
                            Id = id,
                            Code = token,
                            ExpireAt = DateTime.UtcNow.AddMinutes(10),
                            LastSend = DateTime.UtcNow,
                            UserId = user.Id,
                            PhoneNumber = phone
                        });

                        await unit.SaveChangesAsync();
                    }

                    return OperationResult<Guid?>.Success(id);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<Guid?>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ChangePicture(Guid userId, IFormFile file)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null) return OperationResult<bool>.NotFound();
                    var uploadService = _serviceProvider.GetService<IUploadProvider>();
                    var upload = await uploadService.Upload(new StoreViewModel
                    {
                        FormFile = file,
                        Section = UploadSection.UserAvatar,
                        RecordId = user.Id,
                        UserId = user.Id
                    });

                    if (upload.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();
                    if (!string.IsNullOrEmpty(user.Avatar))
                        await uploadService.Delete(user.Avatar, UploadSection.UserAvatar);
                    user.Avatar = upload.Data.Path;
                    user.UpdatedAt = DateTime.UtcNow;
                    await unit.SaveChangesAsync();
                    await EnqueueProfileChange(user);
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ChangeUsername(Guid userId, UsernameViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var username = NormalizeUsername(model.Username);
                    var existing = await unit.Users.AnyAsync(i => i.Id != userId && i.Username == username);
                    if (existing) return OperationResult<bool>.Duplicate();

                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null) return OperationResult<bool>.NotFound();

                    user.Username = username;
                    user.UpdatedAt = DateTime.UtcNow;
                    await unit.SaveChangesAsync();
                    await EnqueueProfileChange(user);
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ConfirmChangeUserVerification(Guid userId,
            ConfirmVerificationViewModel model)
        {
            var op = await ConfirmUserVerification(model, userId);
            if (op.Status == OperationResultStatus.Success)
                return OperationResult<bool>.Success(true);
            return OperationResult<bool>.Failed();
        }

        private async Task<OperationResult<User>> ConfirmUserVerification(ConfirmVerificationViewModel model,
            Guid? userId = null)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    model.Code = model.Code.ConvertDigitsToLatin();
                    var confirmation = await unit.UserVerifications
                        .SingleOrDefaultAsync(i =>
                            i.Id == model.Id &&
                            i.ExpireAt > DateTime.UtcNow &&
                            i.Code == model.Code &&
                            (userId == null || i.UserId == userId)
                        );
                    if (confirmation == null) return OperationResult<User>.Rejected();
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == confirmation.UserId);
                    if (user == null) return OperationResult<User>.Rejected();
                    unit.UserVerifications.Remove(confirmation);
                    if (string.IsNullOrEmpty(confirmation.Email))
                    {
                        if (string.IsNullOrEmpty(user.Email))
                        {
                            user.Email = $"{confirmation.PhoneNumber}@asoode.user";
                            user.LastEmailConfirmed = DateTime.UtcNow;
                        }

                        user.Phone = confirmation.PhoneNumber;
                        user.UpdatedAt = user.LastPhoneConfirmed = DateTime.UtcNow;
                    }
                    else
                    {
                        user.Email = confirmation.Email;
                        user.UpdatedAt = user.LastEmailConfirmed = DateTime.UtcNow;
                    }

                    await unit.SaveChangesAsync();
                    await EnqueueProfileChange(user);
                    return OperationResult<User>.Success(user);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<User>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ResendVerification(Guid userId, Guid verificationId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var confirmation = await unit.UserVerifications
                        .SingleOrDefaultAsync(i =>
                            i.Id == verificationId
                        );
                    if (confirmation == null) return OperationResult<bool>.Rejected();
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == confirmation.UserId);
                    if (user == null) return OperationResult<bool>.Rejected();

                    if (DateTime.UtcNow - confirmation.LastSend < TimeSpan.FromMinutes(2))
                        return OperationResult<bool>.Success();
                    confirmation.Code = CryptoHelper.GeneratePhoneConfirmationCode();
                    confirmation.LastSend = DateTime.UtcNow;

                    OperationResult<bool> op;
                    if (!string.IsNullOrEmpty(confirmation.PhoneNumber))
                    {
                        op = await _serviceProvider.GetService<IPostmanBiz>()
                            .PhoneConfirmAccount(user.Phone, confirmation.Code);
                    }
                    else
                    {
                        op = await _serviceProvider.GetService<IPostmanBiz>()
                            .EmailConfirmAccount(user.Id.ToString(), user.Email, confirmation.Code);
                    }

                    await unit.SaveChangesAsync();
                    return op;
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }


        public async Task<OperationResult<LoginResultViewModel>> ConfirmAccount(ConfirmVerificationViewModel model)
        {
            var op = await ConfirmUserVerification(model);
            if (op.Status == OperationResultStatus.Success)
            {
                return OperationResult<LoginResultViewModel>.Success(new LoginResultViewModel
                {
                    UserId = op.Data.Id,
                    Username = op.Data.Username,
                    Token = GenerateToken(op.Data)
                });
            }

            return OperationResult<LoginResultViewModel>.Failed();
        }

        public async Task<OperationResult<ForgetPasswordResultViewModel>> ForgetPassword(ForgetPasswordViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await FineUser(unit, model.Username, true);
                    if (user == null)
                        return OperationResult<ForgetPasswordResultViewModel>
                            .Success(new ForgetPasswordResultViewModel {NotFound = true});

                    if (user.IsLocked)
                        return OperationResult<ForgetPasswordResultViewModel>
                            .Success(new ForgetPasswordResultViewModel {LockedOut = true});

                    var isPhone = _validateBiz.IsMobile(model.Username);
                    var phone = isPhone ? NormalizePhone(model.Username) : null;
                    if (isPhone && !user.LastPhoneConfirmed.HasValue)
                        return OperationResult<ForgetPasswordResultViewModel>
                            .Success(new ForgetPasswordResultViewModel {PhoneNotConfirmed = true});

                    if (!isPhone && !user.LastEmailConfirmed.HasValue)
                        return OperationResult<ForgetPasswordResultViewModel>
                            .Success(new ForgetPasswordResultViewModel {EmailNotConfirmed = true});

                    var token = await unit.UserVerifications
                        .Where(i => i.ExpireAt > DateTime.UtcNow)
                        .SingleOrDefaultAsync(i => i.UserId == user.Id);
                    if (token != null)
                    {
                        var sendMessage = token.LastSend > DateTime.UtcNow.AddMinutes(-1);
                        if (!sendMessage)
                            return OperationResult<ForgetPasswordResultViewModel>
                                .Success(new ForgetPasswordResultViewModel {Wait = true, Id = token.Id});
                    }
                    else
                    {
                        token = new UserVerification
                        {
                            Code = CryptoHelper.GeneratePhoneConfirmationCode(),
                            UserId = user.Id,
                            ExpireAt = DateTime.UtcNow.AddMinutes(5),
                            PhoneNumber = phone,
                            Email = user.Email
                        };
                        await unit.UserVerifications.AddAsync(token);
                    }

                    token.LastSend = DateTime.UtcNow;
                    await unit.SaveChangesAsync();

                    if (isPhone)
                    {
                        var sent = await _serviceProvider.GetService<IPostmanBiz>()
                            .PhoneForgetPassword(user.Phone, token.Code);
                        if (sent.Status != OperationResultStatus.Success)
                        {
                            await unit.UserVerifications.Where(i => i.Id == token.Id).DeleteAsync();
                            return OperationResult<ForgetPasswordResultViewModel>
                                .Success(new ForgetPasswordResultViewModel {SmsFailed = true});
                        }

                        return OperationResult<ForgetPasswordResultViewModel>
                            .Success(new ForgetPasswordResultViewModel {Id = token.Id});
                    }

                    var send = await _serviceProvider.GetService<IPostmanBiz>()
                        .EmailForgetPassword(user.Id.ToString(), user.Email, token.Code);
                    if (send.Status != OperationResultStatus.Success)
                    {
                        await unit.UserVerifications.Where(i => i.Id == token.Id).DeleteAsync();
                        return OperationResult<ForgetPasswordResultViewModel>
                            .Success(new ForgetPasswordResultViewModel {EmailFailed = true});
                    }

                    return OperationResult<ForgetPasswordResultViewModel>
                        .Success(new ForgetPasswordResultViewModel {Id = token.Id});
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ForgetPasswordResultViewModel>.Failed();
            }
        }

        public async Task<OperationResult<UserProfileViewModel>> GetProfile(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var profile = await (
                        from usr in unit.Users
                        join pln in unit.UserPlanInfo on usr.Id equals pln.UserId
                        where usr.Id == userId
                        orderby pln.CreatedAt descending 
                        select new {User = usr, Plan = pln}
                    ).FirstOrDefaultAsync();

                    if (profile?.User == null ||
                        profile.Plan == null ||
                        profile.User.IsLocked ||
                        profile.User.Blocked ||
                        profile.User.DeletedAt.HasValue)
                        return OperationResult<UserProfileViewModel>.NotFound();

                    var workingTime = await unit.WorkingTimes
                        .OrderByDescending(i => i.CreatedAt)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(i => i.UserId == userId && !i.EndAt.HasValue);

                    var workingTask = await unit.WorkPackageTaskTimes
                        .OrderByDescending(i => i.CreatedAt)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(i => i.UserId == userId && !i.End.HasValue);


                    var result = profile.User.ToProfileViewModel(profile.Plan);
                    result.WorkingGroupFrom = workingTime?.BeginAt;
                    result.WorkingGroupId = workingTime?.GroupId;
                    result.WorkingProjectId = workingTask?.ProjectId;
                    result.WorkingPackageId = workingTask?.PackageId;
                    result.WorkingTaskId = workingTask?.TaskId;
                    result.WorkingTaskFrom = workingTask?.Begin;
                    return OperationResult<UserProfileViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<UserProfileViewModel>.Failed();
            }
        }

        public async Task<OperationResult<LoginResultViewModel>> Login(LoginRequestViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    model.Username = model.Username.ConvertDigitsToLatin();
                    model.Password = model.Password.ConvertDigitsToLatin();

                    var user = await FineUser(unit, model.Username, true);
                    if (user == null)
                        return OperationResult<LoginResultViewModel>
                            .Success(new LoginResultViewModel {NotFound = true});
                    if (user.IsLocked)
                        return OperationResult<LoginResultViewModel>
                            .Success(new LoginResultViewModel
                            {
                                LockedOut = true,
                                LockedUntil = user.LockedUntil
                            });


                    var isPhone = _validateBiz.IsMobile(model.Username);
                    var emailNeedConfirm = !isPhone && !user.LastEmailConfirmed.HasValue;
                    var phoneNeedConfirm = isPhone && !user.LastPhoneConfirmed.HasValue;
                    if (emailNeedConfirm || phoneNeedConfirm)
                    {
                        var skipSend = await unit.UserVerifications
                            .AsNoTracking()
                            .SingleOrDefaultAsync(i =>
                                i.PhoneNumber == user.Phone && i.ExpireAt > DateTime.UtcNow);

                        if (skipSend != null)
                        {
                            return OperationResult<LoginResultViewModel>.Success(new LoginResultViewModel
                            {
                                Id = skipSend.Id,
                                EmailNotConfirmed = !user.LastEmailConfirmed.HasValue,
                                PhoneNotConfirmed = !user.LastPhoneConfirmed.HasValue
                            });
                        }

                        var code = CryptoHelper.GeneratePhoneConfirmationCode();
                        var verification = new UserVerification
                        {
                            Code = code,
                            ExpireAt = DateTime.UtcNow.AddMinutes(10),
                            LastSend = DateTime.UtcNow,
                            UserId = user.Id,
                            Email = emailNeedConfirm ? user.Email : null,
                            PhoneNumber = phoneNeedConfirm ? user.Phone : null
                        };
                        await unit.UserVerifications.AddAsync(verification);
                        await unit.SaveChangesAsync();
                        return OperationResult<LoginResultViewModel>.Success(new LoginResultViewModel
                        {
                            Id = verification.Id,
                            EmailNotConfirmed = !user.LastEmailConfirmed.HasValue,
                            PhoneNotConfirmed = !user.LastPhoneConfirmed.HasValue
                        });
                    }

                    if (!CheckPassword(user, model.Password))
                    {
                        user.LastAttempt = DateTime.UtcNow;
                        user.Attempts++;
                        if (user.Attempts == 5)
                        {
                            user.Attempts = 0;
                            user.LockedUntil = DateTime.UtcNow.AddHours(1);
                        }

                        await unit.SaveChangesAsync();
                        return OperationResult<LoginResultViewModel>
                            .Success(new LoginResultViewModel {InvalidPassword = true, LockedUntil = user.LockedUntil});
                    }

                    user.Attempts = 0;
                    user.LockedUntil = null;
                    await unit.SaveChangesAsync();

                    return OperationResult<LoginResultViewModel>
                        .Success(new LoginResultViewModel
                        {
                            Token = GenerateToken(user),
                            UserId = user.Id,
                            Username = user.Username
                        });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<LoginResultViewModel>.Failed();
            }
        }

        public async Task<OperationResult<LoginResultViewModel>> RecoverPassword(RecoverPasswordViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    model.Code = model.Code.ConvertDigitsToLatin();
                    model.Password = model.Password.ConvertDigitsToLatin();

                    var token = await unit.UserVerifications.SingleOrDefaultAsync(i =>
                        i.ExpireAt > DateTime.UtcNow && i.Code == model.Code &&
                        i.Id == model.Id);
                    if (token == null) return OperationResult<LoginResultViewModel>.Rejected();
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == token.UserId);
                    if (user == null) return OperationResult<LoginResultViewModel>.NotFound();
                    HashPassword(user, model.Password);
                    unit.UserVerifications.Remove(token);
                    user.UpdatedAt = DateTime.UtcNow;
                    await unit.SaveChangesAsync();
                    return OperationResult<LoginResultViewModel>.Success(new LoginResultViewModel
                    {
                        UserId = user.Id,
                        Token = GenerateToken(user),
                        Username = user.Username
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<LoginResultViewModel>.Failed();
            }
        }

        public async Task<OperationResult<RegisterResultViewModel>> Register(RegisterRequestViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var username = model.Username;
                    model.Username = model.Username.ConvertDigitsToLatin().Trim().ToLower();
                    model.Password = model.Password.ConvertDigitsToLatin().Trim();

                    Guid? marketerId = null;
                    if (!string.IsNullOrEmpty(model.Marketer))
                    {
                        var marketer = await unit.Marketers
                            .AsNoTracking()
                            .SingleOrDefaultAsync(m => m.Code == model.Marketer && m.Enabled);
                        if (marketer != null) marketerId = marketer.Id;
                    }

                    var isPhone = _validateBiz.IsMobile(model.Username);
                    var isEmail = !isPhone && _validateBiz.IsEmail(model.Username);
                    if (!isPhone && !isEmail) { throw new Exception($"('{username}' => '{model.Username}') is not a valid username");}
                    var phone = isPhone ? NormalizePhone(model.Username) : null;
                    var email = isEmail ? NormalizeEmail(model.Username) : null;
                    var user = await unit.Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(u =>
                            isEmail && u.Email == email || isPhone && u.Phone == phone);

                    if (user != null)
                    {
                        if (isPhone && !user.LastPhoneConfirmed.HasValue)
                            return OperationResult<RegisterResultViewModel>
                                .Success(new RegisterResultViewModel {PhoneNotConfirmed = true});
                        if (!isPhone && !user.LastEmailConfirmed.HasValue)
                            return OperationResult<RegisterResultViewModel>
                                .Success(new RegisterResultViewModel {EmailNotConfirmed = true});
                        return OperationResult<RegisterResultViewModel>
                            .Success(new RegisterResultViewModel {Duplicate = true});
                    }

                    var salt = CryptoHelper.CreateSalt(50);

                    var newUser = new User
                    {
                        Blocked = false,
                        Email = email,
                        Phone = phone,
                        Type = UserType.User,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        MarketerId = marketerId,
                        Salt = salt,
                        Username = GenerateUsername(),
                        PasswordHash = HashPassword(model.Password, salt)
                    };
                    var channel = new Channel
                    {
                        Title = "ASOODE_BOT_CHANNEL",
                        Type = ChannelType.Bot,
                        UserId = newUser.Id,
                        Id = newUser.Id,
                        RootId = newUser.Id,
                    };
                    var messages = new List<Conversation>
                    {
                        new Conversation
                        {
                            Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_1",
                            ChannelId = channel.Id,
                            Type = ConversationType.Text
                        },
                        new Conversation
                        {
                            Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_2",
                            ChannelId = channel.Id,
                            Type = ConversationType.Text,
                        },
                        new Conversation
                        {
                            Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_3",
                            ChannelId = channel.Id,
                            Type = ConversationType.Link,
                            Path = "COMMAND_NEW_PROJECT"
                        },
                        new Conversation
                        {
                            Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_4",
                            ChannelId = channel.Id,
                            Type = ConversationType.Link,
                            Path = "COMMAND_NEW_GROUP"
                        }
                    };

                    var token = CryptoHelper.GeneratePhoneConfirmationCode();
                    var verification = new UserVerification
                    {
                        Code = token,
                        ExpireAt = DateTime.UtcNow.AddMinutes(10),
                        LastSend = DateTime.UtcNow,
                        UserId = newUser.Id,
                    };
                    if (isPhone)
                    {
                        verification.PhoneNumber = phone;
                        var sent = await _serviceProvider.GetService<IPostmanBiz>()
                            .PhoneConfirmAccount(newUser.Phone, token);
                        if (sent.Status != OperationResultStatus.Success)
                        {
                            await _serviceProvider.GetService<IErrorBiz>()
                                .LogException(new Exception($"Phone Registration failed : {phone}"));
                            return OperationResult<RegisterResultViewModel>
                                .Success(new RegisterResultViewModel {SmsFailed = true});
                        }
                    }
                    else
                    {
                        verification.Email = newUser.Email;
                        var sent = await _serviceProvider.GetService<IPostmanBiz>()
                            .EmailConfirmAccount(newUser.Id.ToString(), newUser.Email, token);
                        if (sent.Status != OperationResultStatus.Success)
                        {
                            await _serviceProvider.GetService<IErrorBiz>()
                                .LogException(new Exception($"Email Registration failed : {newUser.Email}"));
                            return OperationResult<RegisterResultViewModel>
                                .Success(new RegisterResultViewModel {EmailFailed = true});
                        }
                    }

                    var plan = await unit.Plans.AsNoTracking().SingleAsync(i => i.Type == PlanType.Free);
                    var planId = Guid.NewGuid();
                    await unit.UserPlanInfo.AddAsync(plan.ToNewUserPlanInfo(newUser.Id, planId));
                    await unit.PlanMembers.AddAsync(new PlanMember
                    {
                        Identifier = newUser.Id.ToString(),
                        PlanId = planId,
                    });
                    await unit.Channels.AddAsync(channel);
                    await unit.Conversations.AddRangeAsync(messages);
                    await unit.Users.AddAsync(newUser);
                    await unit.UserVerifications.AddAsync(verification);
                    await unit.SaveChangesAsync();
                    if (isEmail) await JoinPendingInvitations(newUser.Email, newUser.Id);

                    return OperationResult<RegisterResultViewModel>.Success(new RegisterResultViewModel
                    {
                        Id = verification.Id
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<RegisterResultViewModel>.Failed();
            }
        }

        public async Task<OperationResult<bool>> RemovePicture(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null) return OperationResult<bool>.NotFound();
                    if (!string.IsNullOrEmpty(user.Avatar))
                    {
                        await _serviceProvider.GetService<IUploadProvider>()
                            .Delete(user.Avatar, UploadSection.UserAvatar);
                    }

                    user.Avatar = null;
                    user.UpdatedAt = DateTime.UtcNow;
                    await unit.SaveChangesAsync();
                    await EnqueueProfileChange(user);
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> UpdateProfile(
            Guid userId, UserProfileUpdateViewModel model, IFormFile avatar)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null) return OperationResult<bool>.NotFound();

                    if (avatar != null)
                    {
                        var uploadBiz = _serviceProvider.GetService<IUploadProvider>();
                        if (!string.IsNullOrEmpty(user.Avatar))
                            await uploadBiz.Delete(user.Avatar, UploadSection.UserAvatar, userId);
                        var op = await uploadBiz.Upload(new StoreViewModel
                        {
                            FormFile = avatar,
                            Section = UploadSection.UserAvatar,
                            UserId = userId
                        });
                        if (op.Status == OperationResultStatus.Success)
                        {
                            user.Avatar = op.Data.ThumbnailPath;
                            await uploadBiz.Delete(op.Data.Path, UploadSection.UserAvatar, userId);
                        }
                    }
                    
                    user.Bio = model.Bio;
                    user.PersonalInitials = model.Initials;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UpdatedAt = DateTime.UtcNow;
                    user.TimeZone = model.TimeZone;
                    user.Calendar = model.Calendar;
                    user.DarkMode = model.DarkMode;
                    await unit.SaveChangesAsync();
                    await EnqueueProfileChange(user);
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> UsernameTaken(UsernameViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var username = NormalizeUsername(model.Username);
                    var existing = await unit.Users.AnyAsync(i => i.Username == username);
                    return OperationResult<bool>.Success(existing);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> AddDevice(Guid userId, PushNotificationViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<bool>.Rejected();

                    var found = await unit.WebPushes.SingleOrDefaultAsync(i =>
                        i.UserId == userId &&
                        i.Endpoint == model.Endpoint &&
                        i.Auth == model.Auth &&
                        i.P256dh == model.P256dh
                    );
                    if (found != null)
                    {
                        found.Enabled = true;
                    }
                    else
                    {
                        found = new WebPush
                        {
                            Auth = model.Auth,
                            P256dh = model.P256dh,
                            Endpoint = model.Endpoint,
                            UserId = userId,
                            Android = model.Android,
                            Browser = model.Browser,
                            Desktop = model.Desktop,
                            Device = model.Device,
                            Ios = model.Ios,
                            Mobile = model.Mobile,
                            Platform = model.Platform,
                            Safari = model.Safari,
                            Tablet = model.Tablet,
                            Enabled = true
                        };
                        await unit.WebPushes.AddAsync(found);
                    }

                    await unit.SaveChangesAsync();
                    var device = found.ToDeviceViewModel();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.AccountDeviceAdd,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Device = device
                    });

                    return OperationResult<bool>.Success();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> EditDevice(Guid userId, Guid deviceId, TitleViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<bool>.Rejected();

                    var found = await unit.WebPushes.SingleOrDefaultAsync(i =>
                        i.UserId == userId &&
                        i.Id == deviceId
                    );
                    if (found == null) return OperationResult<bool>.NotFound();

                    found.Title = model.Title;
                    await unit.SaveChangesAsync();
                    var device = found.ToDeviceViewModel();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.AccountDeviceEdit,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Device = device
                    });

                    return OperationResult<bool>.Success();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> ToggleDevice(Guid userId, Guid deviceId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<bool>.Rejected();

                    var found = await unit.WebPushes.SingleOrDefaultAsync(i =>
                        i.UserId == userId &&
                        i.Id == deviceId
                    );
                    if (found == null) return OperationResult<bool>.NotFound();

                    found.Enabled = !found.Enabled;
                    await unit.SaveChangesAsync();
                    var device = found.ToDeviceViewModel();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.AccountDeviceEdit,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Device = device
                    });

                    return OperationResult<bool>.Success();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<bool>> RemoveDevice(Guid userId, Guid deviceId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<bool>.Rejected();

                    var found = await unit.WebPushes.SingleOrDefaultAsync(i =>
                        i.UserId == userId &&
                        i.Id == deviceId
                    );
                    if (found == null) return OperationResult<bool>.NotFound();

                    unit.WebPushes.Remove(found);
                    await unit.SaveChangesAsync();
                    var device = found.ToDeviceViewModel();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.AccountDeviceRemove,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Device = device
                    });

                    return OperationResult<bool>.Success();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<DeviceViewModel[]>> ListDevices(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<DeviceViewModel[]>.Rejected();

                    var devices = (await unit.WebPushes
                        .Where(i => i.UserId == userId)
                        .AsNoTracking()
                        .ToArrayAsync()).Select(i => i.ToDeviceViewModel()).ToArray();

                    return OperationResult<DeviceViewModel[]>.Success(devices);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<DeviceViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<LoginResultViewModel>> OAuthAuthentication(
            string email, string firstName, string lastName)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(u => u.Email == email);
                    if (user == null)
                    {
                        Guid? marketerId = null;
                        if (!string.IsNullOrEmpty(marketer))
                        {
                            marketerId = await unit.Marketers
                                .AsNoTracking()
                                .Where(i => i.Code == marketer)
                                .Select(i => i.Id)
                                .SingleOrDefaultAsync();
                        }

                        var salt = CryptoHelper.CreateSalt(50);
                        user = new User
                        {
                            Blocked = false,
                            Email = email,
                            Type = UserType.User,
                            FirstName = firstName,
                            LastName = lastName,
                            MarketerId = marketerId,
                            Salt = salt,
                            Username = GenerateUsername(),
                            PasswordHash = HashPassword(Guid.NewGuid().ToString(), salt),
                            LastEmailConfirmed = DateTime.UtcNow
                        };
                        var plan = await unit.Plans.AsNoTracking().SingleAsync(i => i.Type == PlanType.Free);
                        var planId = Guid.NewGuid();
                        var channel = new Channel
                        {
                            Title = "ASOODE_BOT_CHANNEL",
                            Type = ChannelType.Bot,
                            UserId = user.Id,
                            Id = user.Id,
                            RootId = user.Id,
                        };
                        var messages = new List<Conversation>
                        {
                            new Conversation
                            {
                                Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_1",
                                ChannelId = channel.Id,
                                Type = ConversationType.Text
                            },
                            new Conversation
                            {
                                Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_2",
                                ChannelId = channel.Id,
                                Type = ConversationType.Text,
                            },
                            new Conversation
                            {
                                Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_3",
                                ChannelId = channel.Id,
                                Type = ConversationType.Link,
                                Path = "COMMAND_NEW_PROJECT"
                            },
                            new Conversation
                            {
                                Message = "ASOODE_BOT_CHANNEL_WELCOME_MESSAGE_4",
                                ChannelId = channel.Id,
                                Type = ConversationType.Link,
                                Path = "COMMAND_NEW_GROUP"
                            }
                        };
                        await unit.Channels.AddAsync(channel);
                        await unit.Conversations.AddRangeAsync(messages);
                        await unit.UserPlanInfo.AddAsync(plan.ToNewUserPlanInfo(user.Id, planId));
                        await unit.PlanMembers.AddAsync(new PlanMember
                        {
                            Identifier = user.Id.ToString(),
                            PlanId = planId,
                        });
                        await unit.Users.AddAsync(user);

                        var sent = await _serviceProvider.GetService<IPostmanBiz>()
                            .EmailWelcome(user.Id.ToString(), user.Email);
                        if (sent.Status != OperationResultStatus.Success)
                            return OperationResult<LoginResultViewModel>
                                .Success(new LoginResultViewModel {EmailFailed = true});
                        await unit.SaveChangesAsync();
                        await JoinPendingInvitations(user.Email, user.Id);
                    }

                    return OperationResult<LoginResultViewModel>.Success(new LoginResultViewModel
                    {
                        Token = GenerateToken(user),
                        Username = user.Username,
                        UserId = user.Id
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<LoginResultViewModel>.Failed();
            }
        }

        public async Task<OperationResult<GridResult<UserOrderViewModel>>> Transactions(Guid userId,
            GridFilter model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var query = from order in unit.Orders
                        join plan in unit.Plans on order.PlanId equals plan.Id
                        where order.UserId == userId
                        orderby order.CreatedAt descending
                        select new {Order = order, PlanTitle = plan.Title};
                    
                    return await DbHelper.GetPaginatedData(query, tuple =>
                    {
                        var (items, startIndex) = tuple;
                        return items.Select((i, index) => new UserOrderViewModel
                        {
                            Amount = i.Order.PaymentAmount,
                            Id = i.Order.Id,
                            Title = i.PlanTitle,
                            CreatedAt = i.Order.CreatedAt,
                            DueAt = i.Order.ExpireAt,
                            PreviousDebt = 0,
                            Index = startIndex + index + 1,
                            Status = i.Order.Status
                        }).ToArray();
                    }, model.Page, model.PageSize);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<GridResult<UserOrderViewModel>>.Failed();
            }
        }

        
        public async Task<OperationResult<MemberInfoViewModel>> OtherUserProfile(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
                    if (user == null) return OperationResult<MemberInfoViewModel>.NotFound();
                    if (!user.LastEmailConfirmed.HasValue) user.Email = "";
                    return OperationResult<MemberInfoViewModel>.Success(user.ToViewModel());
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<MemberInfoViewModel>.Failed();
            }
        }
        
        #endregion
        
        #region Admin

        public async Task<OperationResult<bool>> AdminConfirmUser(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == id);
                    if (user == null) return OperationResult<bool>.NotFound();
                    user.Blocked = false;
                    if (!string.IsNullOrEmpty(user.Phone)) user.LastPhoneConfirmed = DateTime.UtcNow;
                    if (!string.IsNullOrEmpty(user.Email)) user.LastEmailConfirmed = DateTime.UtcNow;
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

        public async Task<OperationResult<bool>> AdminBlockUser(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == id);
                    if (user == null) return OperationResult<bool>.NotFound();
                    user.Blocked = true;
                    user.LastPhoneConfirmed = null;
                    user.LastEmailConfirmed = null;
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

        public async Task<OperationResult<GridResult<UserViewModel>>> AdminUsersList(Guid userId,
            GridFilterWithParams<GridQuery> model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var query = unit.Users.Where(i =>
                        string.IsNullOrEmpty(model.Params.Query) ||
                        (
                            i.FirstName.Contains(model.Params.Query) ||
                            i.LastName.Contains(model.Params.Query) ||
                            i.Phone.Contains(model.Params.Query) ||
                            i.Email.Contains(model.Params.Query) ||
                            i.Bio.Contains(model.Params.Query)
                        )
                    ).OrderByDescending(i => i.CreatedAt);
                    var result = await DbHelper.GetPaginatedData(query, tuple =>
                    {
                        var (items, startIndex) = tuple;
                        return items.Select((i, index) => new UserViewModel
                        {
                            Index = startIndex + index + 1,
                            Avatar = i.Avatar,
                            Bio = i.Bio,
                            Calendar = i.Calendar,
                            Email = i.Email,
                            Id = i.Id,
                            Phone = i.Phone,
                            Type = i.Type,
                            Username = i.Username,
                            CreatedAt = i.CreatedAt,
                            FirstName = i.FirstName,
                            LastName = i.LastName,
                            TimeZone = i.TimeZone,
                            UpdatedAt = i.UpdatedAt,
                            EmailConfirmed = i.LastEmailConfirmed.HasValue && !i.Email.EndsWith("@asoode.user"),
                            PhoneConfirmed = i.LastPhoneConfirmed.HasValue,
                        }).ToArray();
                    }, model.Page, model.PageSize);

                    var userIds = result.Data.Items.Select(i => i.Id).ToArray();
                    var plans = await (
                        from usr in unit.Users
                        join info in unit.UserPlanInfo on usr.Id equals info.UserId
                        where userIds.Contains(usr.Id)
                        orderby info.CreatedAt descending
                        select info
                    ).AsNoTracking().ToArrayAsync();

                    foreach (var item in result.Data.Items)
                    {
                        item.Plan = plans.First(p => p.UserId == item.Id).ToViewModel();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<GridResult<UserViewModel>>.Failed();
            }
        }

        public async Task<OperationResult<LoginResultViewModel>> OAuthAdminAuthentication(string email)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Email == email);
                    if (user == null || user.Type != UserType.Admin)
                        return OperationResult<LoginResultViewModel>.Rejected();
                    var token = GenerateToken(user);
                    return OperationResult<LoginResultViewModel>.Success(new LoginResultViewModel
                    {
                        Token = token,
                        Username = user.Username,
                        UserId = user.Id
                    });
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<LoginResultViewModel>.Failed();
            }
        }

        public async Task<OperationResult<bool>> AdminResetUserPassword(Guid userId, Guid id,
            UserResetPasswordViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == id);
                    if (user == null) return OperationResult<bool>.NotFound();
                    HashPassword(user, model.Password);
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

        public async Task<OperationResult<bool>> AdminEditUser(Guid userId, Guid id, UserEditViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == id);
                    if (user == null) return OperationResult<bool>.NotFound();

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Type = model.Type;
                    
                    if (!string.IsNullOrEmpty(model.Phone))
                    {
                        if (model.Phone != user.Phone && _validateBiz.IsMobile(model.Phone))
                        {
                            user.Phone = _validateBiz.PrefixMobileNumber(model.Phone);
                            user.LastPhoneConfirmed = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        user.Phone = null;
                        user.LastPhoneConfirmed = null;
                    }
                    
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        if (model.Email != user.Email && _validateBiz.IsEmail(model.Email))
                        {
                            user.Email = model.Email;
                            user.LastEmailConfirmed = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        user.Email = null;
                        user.LastEmailConfirmed = null;
                    }

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
        
        #region Private Methods

        public ClaimsPrincipal ExtractToken(string token, UserType type = UserType.User)
        {
            const string bearer = "Bearer ";
            const string bearerPercent = "Bearer%20";
            if (token.StartsWith(bearerPercent)) token = token.Substring(bearerPercent.Length);
            if (token.StartsWith(bearer)) token = token.Substring(bearer.Length);
            if (string.IsNullOrEmpty(token)) return null;
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var issuer = configuration["Setting:Issuer"];
            var secret = configuration["Setting:Secret"];
            var key = Encoding.UTF8.GetBytes(secret);
            var validator = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };


            try
            {
                if (!validator.CanReadToken(token)) return null;
                
                IdentityModelEventSource.ShowPII = true;
                var principal = validator.ValidateToken(token, validationParameters, out _);

                var hasClaim = type == UserType.Anonymous ||
                               principal.Claims.Any(c =>
                                   (
                                       c.Value == UserType.Admin.ToString().ToLower() ||
                                       c.Value == type.ToString().ToLower()
                                   )
                               );

                if (!hasClaim) return null;
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool CheckPassword(User user, string password)
        {
            return VerifyHash(user.PasswordHash, password, user.Salt);
        }

        private Task EnqueueProfileChange(User user)
        {
            return _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
            {
                Type = ActivityType.AccountEdit,
                UserId = user.Id,
                User = user.ToViewModel(true)
            });
        }

        private async Task<User> FineUser(AccountDbContext context, string findBy, bool track = false)
        {
            var isPhone = _validateBiz.IsMobile(findBy);
            var isEmail = !isPhone && _validateBiz.IsEmail(findBy);
            var phone = isPhone ? NormalizePhone(findBy) : null;
            var email = isEmail ? NormalizeEmail(findBy) : null;
            var isUsername = !isPhone && !isEmail;
            var username = isUsername ? NormalizeUsername(findBy) : null;

            var query = context.Users.Where(u =>
                (isEmail && u.Email == email) ||
                (isPhone && u.Phone == phone) ||
                (isUsername && u.Username == username)
            );

            if (!track) query = query.AsNoTracking();
            return await query.SingleOrDefaultAsync();
        }

        private async Task JoinPendingInvitations(string email, Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var pendings = await unit.PendingInvitations
                        .Where(i => i.Identifier == email)
                        .ToArrayAsync();
                    if (!pendings.Any()) return;
                    foreach (var pending in pendings)
                    {
                        switch (pending.Type)
                        {
                            case PendingInvitationType.Project:
                                await unit.ProjectMembers.AddAsync(new ProjectMember
                                {
                                    Access = pending.Access,
                                    IsGroup = false,
                                    ProjectId = pending.RecordId,
                                    RecordId = userId,
                                });
                                break;
                            case PendingInvitationType.Group:
                                var group = await unit.Groups.AsNoTracking()
                                    .SingleOrDefaultAsync(i => i.Id == pending.RecordId);
                                if (group == null || group.DeletedAt.HasValue) continue;
                                await unit.GroupMembers.AddAsync(new GroupMember
                                {
                                    Level = group.Level,
                                    RootId = group.RootId,
                                    Access = pending.Access,
                                    GroupId = pending.RecordId,
                                    UserId = userId
                                });
                                break;
                            case PendingInvitationType.WorkPackage:
                                var workPackage = await unit.WorkPackages.AsNoTracking().Where(p =>
                                    p.Id == pending.RecordId).SingleOrDefaultAsync();
                                if (workPackage == null || workPackage.DeletedAt.HasValue) continue;
                                await unit.WorkPackageMembers.AddAsync(new WorkPackageMember
                                {
                                    Access = pending.Access,
                                    IsGroup = false,
                                    PackageId = pending.RecordId,
                                    RecordId = userId,
                                });
                                await unit.ProjectMembers.AddAsync(new ProjectMember
                                {
                                    Access = AccessType.Editor,
                                    IsGroup = false,
                                    ProjectId = workPackage.ProjectId,
                                    RecordId = userId,
                                });
                                break;
                        }
                    }

                    var planMembers = await unit.PlanMembers
                        .Where(m => m.Identifier == email).ToArrayAsync();
                    foreach (var planMember in planMembers) planMember.Identifier = userId.ToString();
                    unit.PendingInvitations.RemoveRange(pendings);
                    await unit.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            }
        }

        private string GenerateToken(User user)
        {
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var issuer = configuration["Setting:Issuer"];
            var secret = configuration["Setting:Secret"];
            var key = Encoding.UTF8.GetBytes(secret);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(AsoodeClaims.UserId, user.Id.ToString()),
                new Claim(AsoodeClaims.Username, user.Username),
                new Claim(AsoodeClaims.UserType, user.Type.ToString().ToLower()),
                new Claim(AsoodeClaims.TokenId, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.UtcNow.AddDays(120),
                signingCredentials: credentials);
            return $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";
        }

        private string GenerateUsername()
        {
            return $"A{DateTime.Now.GetTime()}";
        }

        private string HashPassword(string password, string salt)
        {
            return AuthCryptoHelper.Hash(password, salt);
        }

        private void HashPassword(User user, string password)
        {
            user.PasswordHash = HashPassword(password, user.Salt);
        }

        private string NormalizeEmail(string email)
        {
            return email.Trim().ToLower().ConvertDigitsToLatin();
            ;
        }

        private string NormalizePhone(string phone)
        {
            return _validateBiz.PrefixMobileNumber(phone).ConvertDigitsToLatin();
            ;
        }

        private string NormalizeUsername(string username)
        {
            return username.Trim().ToLower().ConvertDigitsToLatin();
            ;
        }

        private bool VerifyHash(string hash, string password, string salt)
        {
            return AuthCryptoHelper.Verify(password, hash, salt);
        }

        #endregion Private Methods
    }
}