using Identity.Infrastructure.Interfaces.Services;
using Identity.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyLifeApp.Application.Interfaces.Services;
using MyLifeApp.Application.Services;
using MyLifeApp.Infrastructure.Data.Context;
using MyLifeApp.Infrastructure.Data.Repositories;
using Identity.Infrastructure.Services;
using System.Text;
using MyLifeApp.Application.Interfaces.Repositories;
using System.Diagnostics;
using MyLifeApp.Infrastructure.Data.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Ignore null fields from Response
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.DefaultIgnoreCondition =
//               System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
//    });

// Dependency Injection
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IPostCommentRepository, PostCommentRepository>();

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IAuthenticatedProfileService, AuthenticatedProfileService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging();
});

// Configure Redis Caching
builder.Services.AddScoped<ICacheService, CacheService>();

// Identity Settings
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"]!)),
        ValidateIssuerSigningKey = true
    };
});

// Waiting for database
WaitForDatabase.Wait();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Applying Migrations
void MigrationInitialisation(IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices.CreateScope();
    serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
}

// Trying to apply migrations
try
{
    MigrationInitialisation(app);
}
catch
{
    Console.WriteLine("Migrations already applied!");
}


app.UseAuthorization();

app.MapControllers();

app.Run();

// run with custom port
// dotnet run --urls=http://localhost:5001/
