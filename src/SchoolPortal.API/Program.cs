using Microsoft.OpenApi.Models;
using SchoolPortal.API.Infrastructure;
using SchoolPortal.Application;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Domain.Common;
using SchoolPortal.Infrastructure;
using SchoolPortal.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SchoolPortal API",
        Version = "v1",
        Description =
            "Multi-tenant School Portal API. Auth is not wired yet: in Development, send the " +
            "`X-School-Id` header (a SCHOOLS.ID GUID) on every request to select the tenant. " +
            "This header is ignored outside Development and will be replaced by the `school_id` JWT claim.",
    });

    options.AddSecurityDefinition("X-School-Id", new OpenApiSecurityScheme
    {
        Name = "X-School-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Development-only tenant selector (SCHOOLS.ID GUID).",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-School-Id" },
        }] = Array.Empty<string>(),
    });
});

var app = builder.Build();

// Idempotent reference-data seeding (the ROLES table ships empty).
using (var scope = app.Services.CreateScope())
{
    var roles = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
    await roles.EnsureSeededAsync(RoleNames.All);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Exposed so SchoolPortal.API.IntegrationTests can use WebApplicationFactory<Program>.
public partial class Program;
