using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.General
{
    [JwtAuthorize]
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
}