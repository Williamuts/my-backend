using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E1.Backend.Api.Models; // 1. 导入 (用于 IncidentReport)
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Hosting; // 2. 导入 (用于查找 wwwroot)
using System.IO; // 3. 导入 (用于 Path.Combine)

namespace E1.Backend.Api.Controllers
{
    [Authorize] // 1. 保护此控制器，必须登录 (E1)
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // (用于获取 wwwroot 路径)

        // 2. 注入数据库 和 WebHostEnvironment
        public AiController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // -----------------------------------------------------------------
        // *** E7 核心: 触发 AI 分析的接口 ***
        // -----------------------------------------------------------------
        // POST /api/ai/analyze/{id}
        [HttpPost("analyze/{id:int}")]
        public async Task<IActionResult> AnalyzeIncidentImage(int id)
        {
            // A. (E4) 从数据库中找到“事故报告” (Incident Report)
            var incidentReport = await _context.IncidentReports.FindAsync(id);
            if (incidentReport == null)
            {
                return NotFound(new { Message = "Incident Report not found" });
            }

            if (string.IsNullOrEmpty(incidentReport.PhotoUrl))
            {
                return BadRequest(new { Message = "Report has no PhotoUrl" });
            }

            // B. (E4) 获取照片的“物理路径” (Physical Path)
            // (例如: "/uploads/image.jpg")
            var relativePath = incidentReport.PhotoUrl;
            // (例如: "C:\dev\E1_Solution\E1.Backend.Api\wwwroot")
            var webRootPath = _webHostEnvironment.WebRootPath;
            // (例如: "C:\dev\E1_Solution\E1.Backend.Api\wwwroot\uploads\image.jpg")
            var physicalPath = Path.Combine(webRootPath, relativePath.TrimStart('/'));

            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound(new { Message = $"(File not found at: {physicalPath})" });
            }

            // -------------------------------------------------
            // C. *** AI 模型分析 (占位符) ***
            // 
            // 当您“之后会有”那个模型时 (例如 my_model.onnx)，
            // 您将在这里加载它，并用 'physicalPath' 的图片去运行它。
            //
            // (示例: var result = await _myModelService.AnalyzeAsync(physicalPath);)
            //
            // 现在，我们只使用一个“占位符” (Placeholder)
            // -------------------------------------------------
            var analysisResult = "[Placeholder] has been analyzed by AI."; // (模拟的 AI 结果)
            await Task.Delay(500); // (模拟 0.5 秒的 AI 处理时间)


            // D. (E4) 将 AI 结果更新回数据库
            // (我们更新 E4 的 Status 字段)
            incidentReport.Status = analysisResult;
            _context.IncidentReports.Update(incidentReport);
            await _context.SaveChangesAsync();

            // E. 返回成功
            return Ok(new
            {
                Message = "AI Analysis successful",
                Report = incidentReport // (返回更新后的报告)
            });
        }
    }
}