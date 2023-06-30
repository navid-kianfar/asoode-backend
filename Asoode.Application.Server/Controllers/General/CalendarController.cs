using Asoode.Application.Abstraction.Contracts;
using Asoode.Application.Abstraction.Fixtures;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.General;


[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Calendar")]
public class CalendarController : BaseController
{
    private readonly ICalendarService _calendarBiz;

    public CalendarController(ICalendarService calendarBiz)
    {
        _calendarBiz = calendarBiz;
    }
}