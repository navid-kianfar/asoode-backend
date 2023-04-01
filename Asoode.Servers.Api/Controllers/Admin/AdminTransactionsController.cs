using System.Threading.Tasks;
using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.Admin
{
    [JwtAuthorize(UserType.Admin)]
    [Route("v2/admin/transaction")]
    public class AdminTransactionsController : BaseController
    {
        private readonly ITransactionBiz _transactionBiz;

        public AdminTransactionsController(ITransactionBiz transactionBiz)
        {
            _transactionBiz = transactionBiz;
        }
        
        [ValidateModel]
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GridFilter model)
        {
            var op = await _transactionBiz.AdminTransactionsList(Identity.UserId, model);
            return Json(op);
        }
    }
}