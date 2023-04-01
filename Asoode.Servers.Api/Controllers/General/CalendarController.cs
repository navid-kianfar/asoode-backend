using Asoode.Backend.Engine;
using Asoode.Backend.Filters;
using Asoode.Core.Contracts.General;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Controllers.General
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