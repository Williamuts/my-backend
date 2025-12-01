using E1.Backend.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E1.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecyclingGuideController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecyclingGuideController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET: api/RecyclingGuide
        // 获取列表 (对应 Image 1: 带有图标的列表)
        // =========================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecyclingGuide>>> GetGuides()
        {
            return await _context.RecyclingGuides
                                 .OrderByDescending(g => g.CreatedAt)
                                 .ToListAsync();
        }

        // =========================================================
        // GET: api/RecyclingGuide/5
        // 获取详情 (对应 Image 2: 视频 + 文字 + 绿色跳转按钮)
        // =========================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<RecyclingGuide>> GetGuide(int id)
        {
            var guide = await _context.RecyclingGuides.FindAsync(id);

            if (guide == null)
            {
                return NotFound();
            }

            return guide;
        }

        // =========================================================
        // POST: api/RecyclingGuide
        // Admin 创建新指南 (必须包含 ImageUrl 和 ArticleUrl)
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<RecyclingGuide>> CreateGuide(CreateUpdateGuideDto dto)
        {
            var guide = new RecyclingGuide
            {
                Title = dto.Title,
                Description = dto.Description,
                VideoUrl = dto.VideoUrl,

                // 👇 [新增] 对应列表左侧的图标
                ImageUrl = dto.ImageUrl,

                // 👇 [新增] 对应详情页底部的 "Open Journal Article" 按钮
                ArticleUrl = dto.ArticleUrl,

                CreatedAt = DateTime.UtcNow
            };

            _context.RecyclingGuides.Add(guide);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuide), new { id = guide.Id }, guide);
        }

        // =========================================================
        // PUT: api/RecyclingGuide/5
        // Admin 更新指南
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuide(int id, CreateUpdateGuideDto dto)
        {
            var guide = await _context.RecyclingGuides.FindAsync(id);
            if (guide == null)
            {
                return NotFound();
            }

            // 更新所有字段
            guide.Title = dto.Title;
            guide.Description = dto.Description;
            guide.VideoUrl = dto.VideoUrl;

            // 👇 [新增] 更新图标和文章链接
            guide.ImageUrl = dto.ImageUrl;
            guide.ArticleUrl = dto.ArticleUrl;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // DELETE: api/RecyclingGuide/5
        // Admin 删除指南
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuide(int id)
        {
            var guide = await _context.RecyclingGuides.FindAsync(id);
            if (guide == null)
            {
                return NotFound();
            }

            _context.RecyclingGuides.Remove(guide);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}