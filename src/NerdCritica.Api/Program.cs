using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using NerdCritica.Api.AutoMapperProfile;
using NerdCritica.Api.Extensions;
using NerdCritica.Api.Utils.ExceptionService;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Application.Services.User;
using NerdCritica.Infrastructure.Context;
using NerdCritica.Infrastructure.DependencyInjection;
using Newtonsoft.Json;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddUserContext(builder.Configuration);
builder.Services
 .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
builder.Services.AddTransient<MoviePostService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = string.IsNullOrEmpty(jwtSettings["Key"]) ? Array.Empty<byte>() : Encoding.ASCII.
    GetBytes(jwtSettings["Key"] ?? string.Empty);

builder.Services.AddJwtAuthentication(key);
builder.Services.ConfigureIdentityOptions();
Log.Logger = new LoggerConfiguration().ReadFrom.
    Configuration(builder.Configuration).CreateLogger();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Services.AddLogging();
builder.Services.AddSingleton<LoggerHelper>();
builder.Services.AddSingleton<ErrorHandlerOptions>();
builder.Services.AddSingleton<ExceptionHandler>();
builder.Services.AddSingleton<ExceptionDetailsHelper>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("ModeratorOrAdmin", policy => policy.RequireRole("Moderator", "Admin"));
});

var app = builder.Build();
app.Services.GetRequiredService<IConfiguration>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
        {
            StatusCode = 401,
            Message = "Você não tem acesso acesso a esse recurso, ou ainda não realizou login"
        }));
    }
});

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.Run();