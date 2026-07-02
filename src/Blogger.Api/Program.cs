using Blogger.Api.Infrastructure;
using Blogger.Api.Validators.Authors;
using Blogger.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("E2E"))
{
    builder.Services.AddBloggerPersistence(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
}

builder.Services.AddValidatorsFromAssemblyContaining<CreateAuthorRequestValidator>();
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<RequestValidationFilter>();
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<DbUpdateExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
