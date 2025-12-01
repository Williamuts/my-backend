// ---------------------------------
// 导入所有必需的库
// ---------------------------------
using E1.Backend.Api;
using E1.Backend.Api.Models;
using E1.Backend.Api.Services;
using E1.Backend.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

// --- 1. 关键 (E6): 导入 Firebase Admin (后端) ---
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;
// ------------------------------------------

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// --- 2. 关键 (E6): 初始化 Firebase Admin SDK ---
try
{
    // 检查文件是否存在，防止报错
    if (File.Exists("firebase-admin-key.json"))
    {
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile("firebase-admin-key.json"),
        });
        builder.Services.AddSingleton<FirebaseMessaging>(FirebaseMessaging.DefaultInstance);
    }
    else
    {
        Console.WriteLine("Warning: firebase-admin-key.json not found. Skipping Firebase init.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("!!!!!!!!!! E6 error: Firebase Admin SDK Initialization failed !!!!!!!!!!");
    Console.WriteLine(ex.Message);
}
// ------------------------------------------

// ---------------------------------
// (1) 注册服务 (Services)
// ---------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- Swagger 配置 ---
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' [Space] Then enter your Token. \r\n\r\n For example: 'Bearer 12345abcdef'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// --- (E1 & E3 & E4 & E6 数据库配置) ---
var connectionString = "Data Source=app.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. 添加 .NET Identity 服务 (E1)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// --- (E1: 配置JWT认证) ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

// ============================================================
// 🔽 [新增] 注册邮件服务
// ============================================================
builder.Services.AddScoped<IEmailService, GmailEmailService>();
// ============================================================


var app = builder.Build();

// ---------------------------------
// (2) 配置HTTP请求管道 (Middleware)
// ---------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// 🔽 [新增] 注册单设备登录中间件
// ============================================================
app.UseMiddleware<SingleSessionMiddleware>();
// ============================================================

app.MapControllers();

// ============================================================
// 🔽 [关键部分] 数据初始化 (Seeding)
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // 1. 获取数据库上下文并自动建表
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // 👈 确保这行还在，防止删了数据库后报错

        // --------------------------------------------------------
        //  Part A: 角色与管理员账号初始化
        // --------------------------------------------------------
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "wttm0416@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser != null)
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"[Seeding] User {adminEmail} has been promoted to Admin.");
            }
        }

        // --------------------------------------------------------
        //  Part B: 回收指南初始化 (这里填入 Firebase 链接)
        // --------------------------------------------------------
        if (!context.RecyclingGuides.Any())
        {
            Console.WriteLine("[Seeding] Generating recycling guides...");

            context.RecyclingGuides.AddRange(
                new RecyclingGuide
                {
                    Title = "How to recycle Paper",
                    ImageUrl = "icon_guide_paper.png",

                    // 👇👇👇 [修改这里] 填入你的 Firebase Storage 链接 👇👇👇
                    // 格式类似：https://firebasestorage.googleapis.com/.../paper.mp4?alt=media&token=...
                    VideoUrl = "在此处填入_Firebase_Paper_视频链接",

                    ArticleUrl = "https://methodrecycling.com/world/journal/paper-recycling-recycling-101",
                    Description = "Learn how to properly recycle paper products to save trees.",
                    CreatedAt = DateTime.UtcNow
                },
                new RecyclingGuide
                {
                    Title = "How to recycle Plastic",
                    ImageUrl = "icon_guide_plastic.png",

                    // 👇👇👇 [修改这里] 👇👇👇
                    VideoUrl = "在此处填入_Firebase_Plastic_视频链接",

                    ArticleUrl = "https://www.recyclenow.com/how-to-recycle/plastic-recycling",
                    Description = "Understanding different plastic types and cleaning requirements.",
                    CreatedAt = DateTime.UtcNow
                },
                new RecyclingGuide
                {
                    Title = "How to recycle Glass",
                    ImageUrl = "icon_guide_glass.png",

                    // 👇👇👇 [修改这里] 👇👇👇
                    VideoUrl = "在此处填入_Firebase_Glass_视频链接",

                    ArticleUrl = "https://www.recyclenow.com/how-to-recycle/glass-recycling",
                    Description = "Glass is 100% recyclable and can be recycled endlessly.",
                    CreatedAt = DateTime.UtcNow
                },
                new RecyclingGuide
                {
                    Title = "How to recycle Metal",
                    ImageUrl = "icon_guide_metal.png",

                    // 👇👇👇 [修改这里] 👇👇👇
                    VideoUrl = "在此处填入_Firebase_Metal_视频链接",

                    ArticleUrl = "https://www.recyclenow.com/how-to-recycle/metal-recycling",
                    Description = "Recycling metal cans saves a significant amount of energy.",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // 保存到数据库
            await context.SaveChangesAsync();
            Console.WriteLine("[Seeding] Recycling guides added successfully!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
// ============================================================

app.Run();