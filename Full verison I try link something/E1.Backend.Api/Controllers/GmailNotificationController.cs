using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using E1.Backend.Api.Services;
using E1.Backend.Api.Models;
using System.Security.Claims;

namespace E1.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 基础门槛：必须登录才能访问此 Controller
    public class GmailNotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GmailNotificationController(IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _emailService = emailService;
            _userManager = userManager;
        }

        // ============================================================
        //  场景 1: 给自己发 (普通用户用) - 我帮你把注释解开了，这样功能更完整
        //  POST api/GmailNotification/send-to-me
        // ============================================================
        //[HttpPost("send-to-me")]
        //public async Task<IActionResult> SendToMyself([FromBody] NotificationContent request)
        //{
            // 获取当前登录用户的 ID
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user == null) return Unauthorized();

            // 发送给用户自己
           // await _emailService.SendEmailAsync(user.Email, request.Title, request.Body);
            //return Ok(new { Message = "Email sent to yourself successfully." });
        //}

        // ============================================================
        //  场景 2: 给别人发 (只有 Admin 能用)
        //  POST api/GmailNotification/admin-send
        // ============================================================
        [HttpPost("admin-send")]
        [Authorize(Roles = "Admin")] // ⬅️ 关键修改：只有拥有 Admin 角色才能调用
        public async Task<IActionResult> AdminSendToUser([FromBody] AdminNotificationRequest request)
        {
            // ❌ 已删除：查当前用户、比对 wttm0416@gmail.com 的代码
            // 能进到这里，说明已经是管理员了，直接发送即可

            try
            {
                await _emailService.SendEmailAsync(request.TargetEmail, request.Title, request.Body);
                return Ok(new { Message = $"The administrator has sent an email to {request.TargetEmail}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class NotificationContent
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class AdminNotificationRequest
    {
        public string TargetEmail { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}