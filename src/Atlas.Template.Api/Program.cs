using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Atlas.Template.Services.ApplicatoinServicesConfig;
using Atlas.Template.Api.StartupExtensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddIdentityConfigurations();
builder.Services.AddDataSeeders();


var app = builder.Build();
await app.ExecuteStartupExtensions();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
