using Blogger.Api.Infrastructure;
using Blogger.Api.Validators.Authors;
using Blogger.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBloggerPersistence(
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));

builder.Services.AddValidatorsFromAssemblyContaining<CreateAuthorRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
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
