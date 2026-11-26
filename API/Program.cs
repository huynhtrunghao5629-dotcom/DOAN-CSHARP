using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyKhachSanAPI.Models;
using QuanLyKhachSanAPI.Security;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Dán trực tiếp chuỗi JWT Token lấy từ API đăng nhập (không cần gõ chữ Bearer)."
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
            Array.Empty<string>()
        }
    });
}); builder.Services.AddCors(options => options.AddPolicy("WebLocal", policy => policy
    .WithOrigins("https://localhost:7146", "http://localhost:5146")
    .AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Jwt__Key qua biến môi trường hoặc dotnet user-secrets trên máy triển khai.
var secret = builder.Configuration["Jwt:Key"];
byte[] jwtKey;
if (string.IsNullOrWhiteSpace(secret))
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException("Thiếu Jwt:Key. Cần cấu hình khóa JWT ngoài source code.");
    // Chỉ Development: khóa tạm an toàn, token cũ mất hiệu lực sau khi restart API.
    jwtKey = RandomNumberGenerator.GetBytes(32);
    Console.WriteLine("[DEV] JWT dùng khóa tạm; hãy cấu hình Jwt__Key khi cần giữ phiên giữa các lần restart.");
}
else
{
    jwtKey = System.Text.Encoding.UTF8.GetBytes(secret);
    if (jwtKey.Length < 32) throw new InvalidOperationException("Jwt:Key phải dài tối thiểu 32 byte UTF-8.");
}
var signing = new JwtSigningSettings { Key = jwtKey };
builder.Services.AddSingleton(signing);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signing.Key),
            ValidateIssuer = true, ValidIssuer = signing.Issuer,
            ValidateAudience = true, ValidAudience = signing.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Tự động đồng bộ schema bảng KhachHang cho tất cả máy trong nhóm khi API khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw(@"
        IF OBJECT_ID('dbo.KhachHang', 'U') IS NOT NULL
        BEGIN
            IF COL_LENGTH('dbo.KhachHang', 'MatKhau') IS NULL
            BEGIN
                ALTER TABLE dbo.KhachHang ADD MatKhau NVARCHAR(255) NULL;
            END

            DECLARE @ConstraintName NVARCHAR(200);
            SELECT @ConstraintName = kc.name
            FROM sys.key_constraints kc
            JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
            JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            WHERE kc.parent_object_id = OBJECT_ID('dbo.KhachHang') AND kc.type = 'UQ' AND c.name = 'CCCD';

            IF @ConstraintName IS NOT NULL
                EXEC('ALTER TABLE dbo.KhachHang DROP CONSTRAINT [' + @ConstraintName + ']');

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_KhachHang_CCCD_NotNull' AND object_id = OBJECT_ID('dbo.KhachHang'))
                CREATE UNIQUE INDEX UX_KhachHang_CCCD_NotNull ON dbo.KhachHang(CCCD) WHERE CCCD IS NOT NULL;
        END
    ");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("WebLocal");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();