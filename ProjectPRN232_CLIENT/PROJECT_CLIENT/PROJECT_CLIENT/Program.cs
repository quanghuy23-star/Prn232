using Microsoft.AspNetCore.Authentication.Cookies;
using PROJECT_CLIENT.Service;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BaseService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";         // Khi chưa đăng nhập
        options.AccessDeniedPath = "/Login";  // Khi không đủ quyền truy cập
        options.LogoutPath = "/Logout";       // (Không bắt buộc nhưng tốt nếu có)
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

var app = builder.Build();

// ✅ Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Thứ tự quan trọng:
app.UseAuthentication(); // Trước Authorization
app.UseSession();        // Trước Authorization
app.UseAuthorization();

// ✅ Default route (nếu muốn mặc định mở trang đăng nhập có thể đổi thành Account/Login)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
