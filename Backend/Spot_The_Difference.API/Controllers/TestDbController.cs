using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spot_The_Difference.Persistence.Entities.MijnMap;

namespace Spot_The_Difference.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDbController : ControllerBase
    {
        private readonly SpotthedifferencedbContext _context;

        public TestDbController(SpotthedifferencedbContext context)
        {
            _context = context;
        }

        [HttpGet("difficulties")]
        public async Task<IActionResult> GetDifficulties()
        {
            var difficulties = await _context.Difficulties.ToListAsync();
            return Ok(difficulties);
        }
    }
}
