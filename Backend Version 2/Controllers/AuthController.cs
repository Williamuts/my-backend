using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using E1.Backend.Api.Services;
using E1.Backend.Api.Models;

namespace E1.Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        // ============================================================
        //  接口 1: 注册
        // ============================================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            // 注册成功 (此时用户还没有角色，或者是 Program.cs 自动赋予的默认角色)
            return Ok(new { Message = "User registered successfully" });
        }

        // ============================================================
        //  接口 2: 登录 (正规版：从数据库读取 Role)
        // ============================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // 1. 基础验证
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new { Message = "Invalid email or password" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded) return Unauthorized(new { Message = "Invalid email or password" });

            // 2. 单设备登录逻辑 (更新 SessionToken)
            string newSessionToken = Guid.NewGuid().ToString();
            user.CurrentSessionToken = newSessionToken;
            await _userManager.UpdateAsync(user);

            // 3. 生成 JWT Token
            string token = await GenerateJwtToken(user, newSessionToken);

            // 👇👇👇 [关键步骤] 从数据库获取真实角色 👇👇👇
            // 这里会读取 Program.cs 里面 Seeding 进去的 "Admin" 或 "User"
            var roles = await _userManager.GetRolesAsync(user);
            string userRole = roles.FirstOrDefault() ?? "User"; // 如果没查到，默认是 User

            // 4. 回传给前端
            return Ok(new
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName ?? "",
                Address = user.Address ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                AvatarUrl = user.AvatarUrl ?? "",

                // 👇 把角色传给前端，前端 LoginViewModel 依据这个跳转
                Role = userRole
            });
        }

        // ============================================================
        //  接口 3 & 4: 忘记密码 / 重置密码
        // ============================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Ok(new { Message = "如果邮箱存在，验证码已发送。" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendEmailAsync(model.Email, "重置密码", $"您的验证码是: {token}");

            return Ok(new { Message = "验证码已发送", DebugToken = token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound("用户不存在");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded) return Ok("密码修改成功，请使用新密码登录。");

            return BadRequest(result.Errors);
        }

        // ============================================================
        //  Helper: 生成 Token
        // ============================================================
        private async Task<string> GenerateJwtToken(ApplicationUser user, string sessionToken)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("SessionToken", sessionToken)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddDays(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // ============================================================
    //  DTO Models
    // ============================================================
    public class RegisterModel
    {
        [Required] public string Username { get; set; }
        public string? FullName { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class LoginModel
    {
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}