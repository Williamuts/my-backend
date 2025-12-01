using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E1.Backend.Api.Models;
using System.Security.Claims;

namespace E1.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // 只有 Admin 才能进来
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // 1. 获取所有用户列表 (包含是否是 Admin 的信息)
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<object>();

            foreach (var user in users)
            {
                // 检查该用户是否是 Admin
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                userDtos.Add(new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    IsAdmin = isAdmin // 前端可以根据这个显示 "任命" 还是 "已是管理员"
                });
            }

            return Ok(userDtos);
        }

        // ============================================================
        //  接口: 任命新管理员 (Promote)
        //  POST api/admin/promote/{id}
        // ============================================================
        [HttpPost("promote/{id}")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            // 检查是否已经是 Admin
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return BadRequest("User is already an Admin.");
            }

            // 添加 Admin 角色
            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (result.Succeeded)
            {
                return Ok(new { Message = $"User {user.Email} has been promoted to Admin!" });
            }

            return BadRequest("Failed to promote user.");
        }

        // ============================================================
        //  接口: 删除用户
        //  DELETE api/admin/users/{id}
        // ============================================================
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete == null) return NotFound("User does not exist.");

            // 防止删除自己 (当前登录的管理员)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userToDelete.Id == currentUserId)
            {
                return BadRequest("You cannot delete your own admin account.");
            }

            var result = await _userManager.DeleteAsync(userToDelete);
            if (result.Succeeded)
            {
                return Ok(new { Message = $"User {userToDelete.Email} Deleted" });
            }

            return BadRequest(result.Errors);
        }
    }
}