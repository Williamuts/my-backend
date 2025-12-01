using E1.Backend.Api.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace E1.Backend.Api.Middlewares
{
    public class SingleSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SingleSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // 如果用户没登录，直接放行（交给后面的 Auth 模块处理）
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            // 获取当前 Token 里的 SessionToken
            var tokenSessionId = context.User.FindFirst("SessionToken")?.Value;

            // 如果 Token 里没有 SessionToken (可能是旧代码生成的)，也放行或报错
            if (string.IsNullOrEmpty(tokenSessionId))
            {
                await _next(context);
                return;
            }

            // 从 Token 获取 UserID
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                // 创建一个新的 Scope 来获取 UserManager (因为 Middleware 是单例，UserManager 是 Scoped)
                using (var scope = serviceProvider.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    // 查数据库
                    var user = await userManager.FindByIdAsync(userId);

                    // 🔥 [核心逻辑] 比对 🔥
                    // 如果数据库里的标记 (user.CurrentSessionToken) 
                    // 不等于 
                    // 用户手里的标记 (tokenSessionId)
                    // 说明该用户已经在别的手机登录了！
                    if (user != null && user.CurrentSessionToken != tokenSessionId)
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        await context.Response.WriteAsJsonAsync(new { Message = "Your account has been logged in on another device. Please login again." });
                        return; // ⛔ 终止请求，不往下走了
                    }
                }
            }

            // 验证通过，继续下一步
            await _next(context);
        }
    }
}