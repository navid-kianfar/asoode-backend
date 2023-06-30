using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.General;


[Route("v2/calendar")]
[ApiExplorerSettings(GroupName = "Calendar")]
public class CalendarController : BaseController
{
    private readonly ICalendarBiz _calendarBiz;

    public CalendarController(ICalendarBiz calendarBiz)
    {
        _calendarBiz = calendarBiz;
    }
}