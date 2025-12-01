using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E1.Backend.Api.Models;

namespace E1.Backend.Api // 保持和你截图一致的根命名空间
{
    // 必须继承 IdentityDbContext<ApplicationUser> 以支持 Profile
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // 构造函数只能有一个！
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ============================================================
        //  数据库表 (根据你截图里的文件名为准)
        // ============================================================

        // E3: 回收站 (你截图里 Models 文件夹下叫 RecyclingSite.cs)
        public DbSet<RecyclingSite> RecyclingSites { get; set; }

        // E4: 事故报告
        public DbSet<IncidentReport> IncidentReports { get; set; }

        // E6: 用户设备 Token (用于推送)
        public DbSet<UserDevice> UserDevices { get; set; }

        // ============================================================
        //  未来功能 (暂时注释，等你建好 Model 再打开)
        // ============================================================
        public DbSet<RecyclingGuide> RecyclingGuides { get; set; }
        // public DbSet<RecycleRecord> RecycleRecords { get; set; }
    }
}