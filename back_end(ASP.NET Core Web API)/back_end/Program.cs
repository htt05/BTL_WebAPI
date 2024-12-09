using back_end.Models.BusinessModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve; });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<back_endContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("back_endConnect")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options => { options.AddPolicy("AllowSpecificOrigin", builder => builder.WithOrigins("http://localhost:3001").AllowAnyHeader().AllowAnyMethod().AllowCredentials()); });

//lấy key trong tệp cấu hình
var key = builder.Configuration["Jwt:Key"];
//mã hóa key
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //có kiểm tra Issuer (default true)
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //có kiểm tra Audience (default true)
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            //Đảm bảo phải có thời gian hết hạn trong token

            RequireExpirationTime = true,
            ValidateLifetime = true,
            //Chỉ ra key sử dụng trong token
            IssuerSigningKey = signingKey,
            RequireSignedTokens = true

        };
    });


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
