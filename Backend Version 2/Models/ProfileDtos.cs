using System.ComponentModel.DataAnnotations;

namespace E1.Backend.Api.Models
{
    // 用于返回给前端的用户资料 (显示用)
    public class UserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // 支持前端页面显示地址和头像
        public string Address { get; set; }
        public string AvatarUrl { get; set; }
    }

    // 用于接收前端修改资料的请求 (点击 Save Changes 时用)
    public class UpdateProfileRequest
    {
        [Required]
        public string Username { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        // 允许用户修改地址
        public string Address { get; set; }
    }

    // 修改密码 (Account Setting 流程用：登录状态下输入旧密码 + 新密码)
    // 虽然涉及密码，但因为是在 Profile 页面操作，所以放在这里
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}