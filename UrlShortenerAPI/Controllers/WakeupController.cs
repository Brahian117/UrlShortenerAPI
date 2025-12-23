using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortenerAPI.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            const int maxRetries = 5;
            const int delayMs = 1000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
                    return Ok("DB awake");
                }
                catch (Exception)
                {
                    if (attempt == maxRetries)
                        return StatusCode(503, "DB still waking up");

                    await Task.Delay(delayMs);
                }
            }

            return StatusCode(503);
        }



    }
}
