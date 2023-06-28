using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Plan.Plan;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[Route(EndpointConstants.Prefix)]
public class PlanController : BaseController
{
    private readonly IUserIdentityService _identity;
    private readonly IPlanService _planBiz;

    public PlanController(IPlanService planBiz, IUserIdentityService identity)
    {
        _planBiz = planBiz;
        _identity = identity;
    }


    [HttpPost("plan/list")]
    public async Task<IActionResult> List([FromBody] GridFilter model)
    {
        var op = await _planBiz.List(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("plan/all")]
    public async Task<IActionResult> All()
    {
        var op = await _planBiz.All(_identity.User!.UserId);
        return Json(op);
    }


    [HttpPost("plan/create")]
    public async Task<IActionResult> Create([FromBody] PlanDto model)
    {
        var op = await _planBiz.Create(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("plan/toggle/{id:guid}")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var op = await _planBiz.Toggle(_identity.User!.UserId, id);
        return Json(op);
    }


    [HttpPost("plan/edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] PlanDto model)
    {
        var op = await _planBiz.Edit(_identity.User!.UserId, id, model);
        return Json(op);
    }


    [HttpPost("plan/user/{id:guid}")]
    public async Task<IActionResult> EditUserPlan(Guid id, [FromBody] UserPlanInfoDto model)
    {
        var op = await _planBiz.EditUserPlan(_identity.User!.UserId, id, model);
        return Json(op);
    }
}