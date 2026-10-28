using Hangfire;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Text;
//using WebApplication2.BackgroundServices;

//using Restuarant.Infrastucture.Repository; 
using Newtonsoft.Json;
using Resturant.Infrastructure.Context;

using Resturant.Domain.Entity;
using Resturant.Infrastructure.Identity;
using System.Reflection;
using System.Text.Json.Serialization;

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Resturant.Application.Extension;
using Resturant.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);



builder.Services.AddRepositoryService();



builder.Services.AddAuthorization();



builder.Services.AddIdentityApiEndpoints<WebUser>()
    .AddEntityFrameworkStores<RestaurantContext>()
    .AddApiEndpoints();



// 3. Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });


//builder.Services.Configure<IdentityOptions>(options =>
//{
    // Password settings.
    // options.Password.RequireDigit = true;
    // options.Password.RequireLowercase = true;
    // options.Password.RequireNonAlphanumeric = true;
    // options.Password.RequireUppercase = true;
    // options.Password.RequiredLength = 6;
    // options.Password.RequiredUniqueChars = 1;
    //
    // // Lockout settings.
    // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    // options.Lockout.MaxFailedAccessAttempts = 5;
    // options.Lockout.AllowedForNewUsers = true;

    // User settings.
   // options.User.AllowedUserNameCharacters =
     //   "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
  
     //options.User.RequireUniqueEmail = true;
//});

builder.Services.AddDbContext<RestaurantContext>(options
      => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
          optionsBuilder => optionsBuilder.MigrationsAssembly("Resturant.Infrastructure"))
      
 //  => options.UseInMemoryDatabase("TestDB").ConfigureWarnings(warning => warning.Ignore(InMemoryEventId.TransactionIgnoredWarning))
);


//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
//builder.AddServices();
builder.Services.AddAnotherService(); 
//builder.Services.AddHangfire(configuration =>
//{
//    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
//           .UseColouredConsoleLogProvider()
//           .UseSimpleAssemblyNameTypeSerializer()
//           .UseRecommendedSerializerSettings()
//           .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));



//}
//);
//builder.Services.AddHangfireServer(); 

//builder.Services.AddDbContext<ToDoContext>(opt =>
//   opt.UseInMemoryDatabase("TodoList"));




builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyResturantAPI", Version = "v1" });
});

// In Program.cs
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://100.27.216.67", "https://yummy.tonymdesigns.com/", "http://localhost:4200") // The URL of your front-end
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // IMPORTANT for cookies
        });
});

// Add the CORS middleware
//app.UseCors(MyAllowSpecificOrigins);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Add your Nginx IP or subnet to KnownProxies if Nginx is not directly exposed
    // options.KnownProxies.Add(IPAddress.Parse("192.168.1.100")); 
});


builder.Services.AddHttpContextAccessor();



//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//this is needed to convert http to https 
app.UseHttpsRedirection();

app.UseForwardedHeaders();

app.MapControllers();
//app.UseCors(MyAllowSpecificOrigins);
app.UseCors(MyAllowSpecificOrigins);
//app.UseCors("AllowAllHeaders");

//app.UseMiddleware<ExceptionHandlingMIddleware>();
//app.UseHangfireDashboard(); 
app.UseAuthentication();
app.UseAuthorization();
//app.MapIdentityApi<WebUser>();

app.Run();



//add this for integration testing 
public partial class Program { }
