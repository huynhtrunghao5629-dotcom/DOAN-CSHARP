namespace QuanLyKhachSan.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                // Validate antiforgery token on all MVC POST/PUT/PATCH/DELETE actions.
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<QuanLyKhachSan.Web.Services.CustomerApiTokenHandler>();
            builder.Services.AddHttpClient("HotelApi", client =>
            {
                var apiUrl = builder.Configuration["HotelApi:BaseUrl"] ?? "http://localhost:5050/";
                client.BaseAddress = new Uri(apiUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(20);
            }).AddHttpMessageHandler<QuanLyKhachSan.Web.Services.CustomerApiTokenHandler>();
            // Kích hoạt Cookie Authentication
            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/TaiKhoan/DangNhap"; // Đường dẫn khi chưa đăng nhập bị bắt buộc
                    options.LogoutPath = "/TaiKhoan/DangXuat";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2); // Khớp thời hạn API token
                    options.SlidingExpiration = false;
                });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for prsoduction scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
