using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Atlas.Template.Services.ApplicatoinServicesConfig;
using Atlas.Template.Api.StartupExtensions;
using Asp.Versioning;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Atlas Template", Version = "1.0" });
    options.SwaggerDoc("v2", new OpenApiInfo() { Title = "Atlas Template", Version = "2.0" });
});

builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddIdentityConfigurations();
builder.Services.AddDataSeeders();

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

app.UseHttpsRedirection();


app.Run();
