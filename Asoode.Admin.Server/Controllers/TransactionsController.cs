using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
public class TransactionsController : BaseController
{
    private readonly IUserIdentityService _identity;
    private readonly ITransactionBiz _transactionBiz;

    public TransactionsController(ITransactionBiz transactionBiz, IUserIdentityService identity)
    {
        _transactionBiz = transactionBiz;
        _identity = identity;
    }


    [HttpPost("transaction/list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _transactionBiz.List(_identity.User!.UserId, model);
        return Json(op);
    }
}