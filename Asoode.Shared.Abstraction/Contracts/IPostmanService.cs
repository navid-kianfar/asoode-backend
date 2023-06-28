using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Abstraction.Contracts;

public interface IPostmanService
{
    Task<OperationResult<bool>> SendMail(SendMailDTO model);
    Task<OperationResult<bool>> SendSms(SendSmsDTO model);

    Task<OperationResult<bool>> SendTemplateMail(
        string template,
        string culture,
        SendMailDTO model,
        Dictionary<string, string>? replace = null);

    Task<OperationResult<bool>> SendTemplateSms(
        string template,
        string culture,
        string to,
        Dictionary<string, string>? replace = null);
}