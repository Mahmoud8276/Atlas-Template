using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Atlas.Template.Services.ApplicatoinServicesConfig;
using Atlas.Template.Api.StartupExtensions;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using System;
using Atlas.Template.Api.ExceptionHandlers;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Atlas Template", Version = "1.0" });
    options.SwaggerDoc("v2", new OpenApiInfo() { Title = "Atlas Template", Version = "2.0" });
});

// TODO: Check its presestence
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        ctx.ProblemDetails.Extensions["instance"] = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddIdentityConfigurations();
builder.Services.AddDataSeeders();
builder.Services.AddApplicationServices();
builder.Services.AddMappingProfiles(builder.Configuration);
builder.Services.AddOptionsConfigurations(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc();

builder.Services.AddApiVersioning()
             .AddApiExplorer(options =>
             {
                 options.GroupNameFormat = "'v'VVV"; 
                 options.SubstituteApiVersionInUrl = true;
             });


var app = builder.Build();
await app.ExecuteStartupExtensions();

app.UseExceptionHandler();

app.UseHsts();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Atlas Template v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Atlas Template v2");
    });
}
app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
