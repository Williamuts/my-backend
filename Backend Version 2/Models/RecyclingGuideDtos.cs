using System.ComponentModel.DataAnnotations;

namespace E1.Backend.Api.Models
{
    // 用于 Admin 创建或更新 Guide
    public class CreateUpdateGuideDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string VideoUrl { get; set; } // Admin 粘贴的 YouTube 链接

        // 👇 [新增] 让 Admin 能输入图片链接
        public string ImageUrl { get; set; }

        // 👇 [新增] 让 Admin 能输入文章链接
        public string ArticleUrl { get; set; }
    }
}