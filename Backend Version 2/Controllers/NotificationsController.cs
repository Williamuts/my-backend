using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E1.Backend.Api.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using FirebaseAdmin.Messaging; // 1. 导入 (用于发送消息)
using System.Collections.Generic; // 2. 导入 (用于 List<>)

namespace E1.Backend.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FirebaseMessaging _firebaseMessaging; // 3. 注入 (E6 发送服务)

        // 4. 更新构造函数以注入 FirebaseMessaging
        public NotificationsController(ApplicationDbContext context, FirebaseMessaging firebaseMessaging)
        {
            _context = context;
            _firebaseMessaging = firebaseMessaging; // 赋值
        }

        // --- E6 核心接口: 注册设备令牌 (保持不变) ---
        // POST /api/notifications/register-device
        [HttpPost("register-device")]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            bool deviceExists = await _context.UserDevices
                .AnyAsync(d => d.UserId == userId && d.DeviceToken == model.DeviceToken);

            if (!deviceExists)
            {
                var newUserDevice = new UserDevice
                {
                    UserId = userId,
                    DeviceToken = model.DeviceToken,
                    RegisteredAt = DateTime.UtcNow
                };

                await _context.UserDevices.AddAsync(newUserDevice);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Device registered successfully" });
            }

            return Ok(new { Message = "Device was already registered" });
        }

        // -----------------------------------------------------------------
        // *** 5. 关键 (E6): 新增的“发送测试通知”接口 ***
        // -----------------------------------------------------------------
        // POST /api/notifications/send-test
        [HttpPost("send-test")]
        public async Task<IActionResult> SendTestNotification([FromBody] SendTestModel model)
        {
            // A. 获取当前登录用户的 ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // B. 从数据库中找到该用户的所有“设备令牌” (Device Tokens)
            var userDeviceTokens = await _context.UserDevices
                .Where(d => d.UserId == userId)
                .Select(d => d.DeviceToken)
                .ToListAsync();

            if (userDeviceTokens.Count == 0)
            {
                return NotFound(new { Message = "未找到该用户的注册设备 (No devices found for this user)" });
            }

            // C. 创建通知 (Notification)
            var notification = new Notification
            {
                Title = model.Title,
                Body = model.Body
            };

            // D. 创建消息 (Message)
            // (我们可以一次性将同一条消息发送给该用户的所有设备)
            var message = new MulticastMessage()
            {
                Tokens = userDeviceTokens, // (包含所有设备令牌的列表)
                Notification = notification,
                // (您还可以在这里发送 "Data" (数据) 负载)
                // Data = new Dictionary<string, string>()
                // {
                //     { "reportId", "12345" },
                //     { "pageToOpen", "IncidentPage" }
                // }
            };

            // E. 发送消息!
            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

            return Ok(new
            {
                Message = "通知已发送 (Notification sent)",
                SuccessCount = response.SuccessCount, // (有多少设备成功接收)
                FailureCount = response.FailureCount  // (有多少设备失败)
            });
        }
    }

    // --- (E6 接收 DTO - 保持不变) ---
    public class DeviceRegisterModel
    {
        [Required]
        public string DeviceToken { get; set; }
    }

    // --- (E6 发送 DTO - 新增) ---
    public class SendTestModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}