using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortenerAPI.Models;

namespace UrlShortener.Server.Controllers
{
    [ApiController] 
    [Route("[controller]")]
    public class WakeupController : ControllerBase
    {
        private readonly UrlshortenerContext dbContext;

        public WakeupController(UrlshortenerContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet(Name = "Wakeup")]

        public async Task<IActionResult> WakeUp()
        {
            await dbContext.Urls.FirstOrDefaultAsync();
            return Ok("DB awake");
        }


    }
}
