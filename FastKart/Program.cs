using FastKart.Auth;
using FastKart.Helpers;
using FastKart.Models.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSwaggerGen(c =>
{
    // Define the security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // Fixed: Changed from ApiKey to Http for seamless JWT UI inputs
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    // Fix: Updated to Swashbuckle v10 / OpenAPI v2 syntax using a delegate callback
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var constr = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(constr);
});

// Authentication
var JwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new Exception("JWT Options aren't set yet");

builder.Services.AddSingleton(JwtOptions);
builder.Services.AddScoped<JwtHelper>();

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            RequireAudience = true,
            ValidateAudience = true,
            ValidAudience = JwtOptions.Audience,

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey)),

            ValidIssuer = JwtOptions.Issuer,
        };
    });

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddScoped< // scoped to match the appdbcontext
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();

var app = builder.Build();

// Apply EF Core migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger(); // Serves the Swagger JSON
    app.UseSwaggerUI(); // Serves Swagger UI
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();