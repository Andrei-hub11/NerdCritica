using System.Net;
using NerdCritica.Api.AutoMapperProfile;
using NerdCritica.Api.Extensions;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Infrastructure.DependencyInjection;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddUserContext(builder.Configuration);
builder.Services.AddGeneralServices();
builder.Services.AddCustomIdentity();
builder.Services.AddCustomAuthorization();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddFluentEmail(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.ConfigureIdentityOptions();

builder.Services.AddSerilogSettings(builder.Configuration);
builder.Services.AddSingleton<LoggerHelper>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddErrorsConfig();

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