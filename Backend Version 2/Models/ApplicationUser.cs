using Microsoft.AspNetCore.Identity;

namespace E1.Backend.Api.Models
{
    // 继承 IdentityUser，保留原有功能，同时增加我们需要的字段
    public class ApplicationUser : IdentityUser
    {
        // 👇 [新增] 用戶的全名 (例如: Ahmad Shah Peng)
        public string? FullName { get; set; }

        public string? Address { get; set; } // 新增：地址
        public string? AvatarUrl { get; set; } // 新增：头像图片的链接

        // 👇 [新增] 用来记录当前唯一有效的登录标识
        public string? CurrentSessionToken { get; set; }
    }
}