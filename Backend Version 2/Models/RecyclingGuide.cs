using System;
using System.ComponentModel.DataAnnotations;

namespace E1.Backend.Api.Models
{
    public class RecyclingGuide
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } // 标题

        public string Description { get; set; } // 文字内容

        public string VideoUrl { get; set; } // Youtube URL

        // 👇 [新增 1] 列表页左侧的图标图片链接
        public string ImageUrl { get; set; }

        // 👇 [新增 2] 详情页底部 "Open Journal Article" 按钮点击后的跳转链接
        public string ArticleUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}