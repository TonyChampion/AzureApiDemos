using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontDoorAndCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CachingController : ControllerBase
    {
        [HttpGet("[action]")]
        public string GetCachedHeaderTime()
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            DateTime currentDate = DateTime.Now;

            return $"{currentDate.ToString()} ({localZone.StandardName})";
        }
    }
}
