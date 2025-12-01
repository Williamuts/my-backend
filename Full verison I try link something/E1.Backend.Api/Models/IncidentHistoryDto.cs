using System;

namespace E1.Backend.Api.DTOs
{
    // 用于前端 History 页面显示的 DTO
    public class IncidentHistoryDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // 例如: "Submitted", "Approved"
        public DateTime ReportedAt { get; set; }
        public string PhotoUrl { get; set; } // 显示缩略图用
        // 可以根据需要加 Latitude/Longitude
    }
}