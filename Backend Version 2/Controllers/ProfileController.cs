using E1.Backend.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E1.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 鎖頭：只有登入並帶 Token 的人才能進來
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
        }

        // ============================================================
        //  1. 获取当前用户资料 (Get Profile)
        //  GET: api/profile
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            // 安全機制：從 Token 裡自動抓取 UserID，而不是讓前端傳 Email
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("User not found");

            // 處理頭像 URL
            string fullAvatarUrl = null;
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                fullAvatarUrl = $"{baseUrl}{user.AvatarUrl}";
            }

            return Ok(new UserProfileDto
            {
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName, // ✅ 補上 FullName
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = fullAvatarUrl
            });
        }

        // ============================================================
        //  2. 修改基本资料 (Update Profile)
        //  PUT: api/profile
        // ============================================================
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            // 更新字段
            user.FullName = model.FullName; // ✅ 更新 FullName
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            // user.UserName = model.Username; // 通常不讓用戶隨便改帳號，看你需求

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Profile updated successfully" });
            }

            return BadRequest(result.Errors);
        }

        // ============================================================
        //  DTO Models (請確保這些類別存在)
        // ============================================================
        public class UserProfileDto
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; } // ✅
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public string AvatarUrl { get; set; }
        }

        public class UpdateProfileRequest
        {
            public string FullName { get; set; } // ✅
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
        }
    }
}