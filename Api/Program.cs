using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Data;
using Api.Services;
using Api.Middleware;
using Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Use camelCase for JSON property names to match JavaScript conventions
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"), sqliteOptions =>
    {
        sqliteOptions.CommandTimeout(30);
    })
           .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Authentication & Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        
        // Add event handlers to log authentication failures
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "JWT Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT Challenge: {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                logger.LogInformation("JWT Token validated for user ID: {UserId}", userId);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Register common permission policies
    // These policies will be checked by PermissionAuthorizationHandler
    var resources = new[] { "Products", "Categories", "Users", "Roles", "Inventory", "Orders", "Sales", "Warehouses", "Reports", "PromoCodes" };
    var actions = new[] { "Create", "Update", "Delete", "Read" };

    foreach (var resource in resources)
    {
        foreach (var action in actions)
        {
            var policyName = $"Permission:{resource}:{action}";
            options.AddPolicy(policyName, policy =>
                policy.Requirements.Add(new Api.Handlers.PermissionRequirement(resource, action)));
        }
    }
});

// Register permission authorization handler
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Api.Handlers.PermissionAuthorizationHandler>();

// Custom services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IRevenueTrackingService, RevenueTrackingService>();
builder.Services.AddScoped<IOnlineOrderManager, OnlineOrderManager>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
        ?? new[] { "http://localhost:3000", "http://localhost:3001" };
    
    options.AddPolicy("AllowAll", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development, allow any origin for easier testing
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // In production, use specific origins
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Important for cookies/auth headers
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Custom middleware
app.UseMiddleware<AuditMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is migrated and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    context.Database.Migrate();
    await SeedRoles.SeedAsync(context);
    await SeedPermissions.SeedAsync(context);
    await SeedUsers.SeedAsync(context);
    await SeedProducts.SeedAsync(context);
}

app.Run();
