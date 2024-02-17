using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using NerdCritica.Api.Extensions;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Infrastructure.Context;
using NerdCritica.Infrastructure.DependencyInjection;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddUserContext(builder.Configuration);
builder.Services
 .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
builder.Services.AddScoped<IMoviePostService, MoviePostService>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = string.IsNullOrEmpty(jwtSettings["Key"]) ? new byte[0]: Encoding.ASCII.
    GetBytes(jwtSettings["Key"] ?? string.Empty);

builder.Services.AddJwtAuthentication(key);
builder.Services.ConfigureIdentityOptions();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseCors("AllowSpecificOrigin");
//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.Run();