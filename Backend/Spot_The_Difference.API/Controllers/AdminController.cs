using Microsoft.AspNetCore.Mvc;
using Spot_The_Difference.Contracts.Requests;
using Spot_The_Difference.Domain.Services;

namespace Spot_The_Difference.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly AdminService _adminService;

        public AdminController(IWebHostEnvironment env, AdminService adminService)
        {
            _env = env;
            _adminService = adminService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Geen bestand");
            }

            var webRoot = _env.WebRootPath ?? 
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadPath = Path.Combine(webRoot, "images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); 
            var fileName = file.FileName;                                                                   // Artem: original filename behouden, kan later voor problemen zorgen als twee dezelfde namen geuploaded worden
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            return Ok(
                new { 
                    path = $"{baseUrl}/images/{fileName}" 
                }
            );
        }

        [HttpPost("create-round")]
        public async Task<IActionResult> CreateRound([FromBody] CreateRoundRequest request)
        {
            if (request == null) return BadRequest("Lege data");

            try
            {
                var result = await _adminService.CreateRoundAsync(request);
                return Ok(new                                                                   // где это выводится ?
                {
                    message = "Round created successfully",
                    roundId = result.RoundId,
                    title = result.Title
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}