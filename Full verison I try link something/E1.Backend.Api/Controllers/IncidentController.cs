using E1.Backend.Api.Models;
using E1.Backend.Api.DTOs; // 引用你定义的 IncidentHistoryDto
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // 需要这个来进行 ToListAsync 查询
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic; // 需要这个来支持 IEnumerable
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq; // 需要这个来进行 Select 和 OrderBy
using System.Security.Claims;
using System.Threading.Tasks;

namespace E1.Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 确保只有登录的用户才能访问
    public class IncidentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IncidentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // ==========================================
        // 1. 提交新报告接口 (POST /api/incident/submit)
        // ==========================================
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitIncident([FromForm] IncidentReportDto reportDto)
        {
            if (reportDto.Photo == null || reportDto.Photo.Length == 0)
            {
                return BadRequest("没有提供照片 (Photo is required)");
            }

            // --- 1. 保存照片到服务器 ---
            string webRootPath = _webHostEnvironment.WebRootPath;

            // ******** 防崩溃代码 ********
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            // ***************************

            string uploadFolder = Path.Combine(webRootPath, "uploads");

            // 确保 "uploads" 文件夹存在
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // 创建一个唯一的文件名
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(reportDto.Photo.FileName);
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            // 将文件流复制到服务器
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await reportDto.Photo.CopyToAsync(fileStream);
            }

            // --- 2. 保存报告详情到数据库 ---
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            var incident = new IncidentReport
            {
                Description = reportDto.Description,
                Latitude = reportDto.Latitude,
                Longitude = reportDto.Longitude,
                Status = "Submitted", // 默认状态
                ReportedAt = DateTime.UtcNow,
                UserId = userId,
                // 只存储相对路径
                PhotoUrl = "/uploads/" + uniqueFileName
            };

            await _context.IncidentReports.AddAsync(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIncidentById), new { id = incident.Id }, incident);
        }

        // ==========================================
        // 2. 获取历史记录接口 (GET /api/incident/history)
        //    这是你之前缺少的关键部分
        // ==========================================
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<IncidentHistoryDto>>> GetHistory()
        {
            // 1. 获取当前用户ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // 2. 查询数据库
            // 逻辑：查找该用户的所有报告 -> 按时间倒序排列 -> 转换为 DTO
            var historyList = await _context.IncidentReports
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ReportedAt) // 最新的在最上面
                .Select(x => new IncidentHistoryDto
                {
                    Id = x.Id,
                    Description = x.Description,
                    Status = x.Status,
                    ReportedAt = x.ReportedAt,
                    PhotoUrl = x.PhotoUrl
                    // 如果你需要经纬度，也可以在这里赋值:
                    // Latitude = x.Latitude,
                    // Longitude = x.Longitude
                })
                .ToListAsync();

            return Ok(historyList);
        }

        // ==========================================
        // 3. 获取单条详情接口 (GET /api/incident/{id})
        // ==========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncidentById(int id)
        {
            var incident = await _context.IncidentReports.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }
            return Ok(incident);
        }
    }

    // --- 提交用的 DTO ---
    // (IncidentHistoryDto 位于单独的文件中，这里只保留提交用的 DTO)
    public class IncidentReportDto
    {
        [Required]
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
    }
}