using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prometheus;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SimOpsService.Auth0;
using SimOpsService.Interfaces;
using SimOpsService.Models;
using SimOpsService.Repository;
using SimOpsService.Services;


void SetupApplicationDependencyInjection(IServiceCollection services)
{
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IAuthManager, AuthManager>();
}

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("SimOps Service is starting...");

LogLevelSwitch.MinimumLevel = LogEventLevel.Information;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => { lc.WriteTo.Console(); });
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    var authOptions = builder.Configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>();
    if ((authOptions.Method.ToLower() != "internal") && (authOptions.Method.ToLower() != "auth0"))
    {
        throw new Exception("Invalid authentication method specified! Cannot proceed...");
    }

    builder.Services.AddSingleton(authOptions);
    builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    if (authOptions.Method.ToLower().Equals("auth0"))
    {
        //we are using Auth0 for authenticaion....
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.Authority = authOptions.Auth0.Authority;
            x.Audience = authOptions.Auth0.ApiIdentifier;
        });

        builder.Services.AddAuthorization(x =>
        {
            x.AddPolicy("role:admin",
                policy => policy.Requirements.Add(new HasScopeRequirement("role:admin", authOptions.Auth0.Authority)));
            x.AddPolicy("role:pilot",
                policy => policy.Requirements.Add(new HasScopeRequirement("role:pilot", authOptions.Auth0.Authority)));
        });
    }
    else
    {
        //we are using internal authentication
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authOptions.InternalAuth.JwtConfig.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authOptions.InternalAuth.JwtConfig.Secret)),
                ValidAudience = authOptions.InternalAuth.JwtConfig.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
        builder.Services.AddAuthorization(x =>
        {
            x.AddPolicy("role:admin",
                policy => policy.Requirements.Add(
                    new HasScopeRequirement("role:admin", authOptions.InternalAuth.JwtConfig.Issuer)));
            x.AddPolicy("role:pilot",
                policy => policy.Requirements.Add(
                    new HasScopeRequirement("role:pilot", authOptions.InternalAuth.JwtConfig.Issuer)));
        });
    }

    builder.Services.AddCors();
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOptions();
    builder.Services.AddHealthChecks();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SimOps Services v1.0",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Email = "info@pyxisint.com",
                Name = "SimOps Service",
                Url = new Uri("https://github.com/PyxisInt/SimOpsService")
            }
        });
        //add security bits here....
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter Bearer followed by a JWT token here...",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });
    builder.Services.AddSwaggerGenNewtonsoftSupport();

    // Configure Database here...
    var connString = builder.Configuration.GetConnectionString("Default");
    builder.Services.AddDbContext<SimOpsContext>(options =>
    {
        options.UseMySql(connString, new MySqlServerVersion(new Version(8, 0, 21)));
    });
    
    builder.Services.AddHttpContextAccessor();

    SetupApplicationDependencyInjection(builder.Services);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSerilogRequestLogging();

    app.UseHsts();
    
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    //******* customize CORS policy as needed
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    app.UseHttpMetrics(); //uses Prometheus for metrics

    //******* SWAGGER: Put in if (app.Environment.IsDevelopment) section if not wanting to expose documentation *******
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimOps Web Service 1.0"); 
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
    //********* END OF SWAGGER ********

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapMetrics();
        endpoints.MapHealthChecks("/health");
    });

    //******* Create or Update database (code first database design) *********
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<SimOpsContext>();
        db.Database.Migrate();
    }
    
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Unhandled Exception!");
}
finally
{
    Log.Information("SimOps Service is shutting down...");
    Log.CloseAndFlush();
}



public partial class Program
{
    public static LoggingLevelSwitch LogLevelSwitch = new LoggingLevelSwitch();
}