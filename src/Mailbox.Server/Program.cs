using System.Net.Http.Headers;
using Destructurama;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mailbox.Persistence;
using Mailbox.Persistence.Triggers;
using Mailbox.Server.Identity;
using Mailbox.Server.Processors;
using Mailbox.Services;
using Mailbox.Services.Identity;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger(); // Initial log setup, will be overwritten by Serilog, but we need a logger before Dependency Injection is activated.

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services
        .AddSerilog((_, lc) => lc.ReadFrom.Configuration(builder.Configuration) // Configuration in AppSettings.json
            .Destructure.UsingAttributes()) // Sensitive data logging
        .AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        })
        .AddIdentity<IdentityUser, IdentityRole>() 
        .AddEntityFrameworkStores<ApplicationDbContext>()
        
        .Services.AddHttpClient("SecureApi", c =>
        {
            var imgurClientId = builder.Configuration.GetSection("imgur")["Client-Id"];
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", imgurClientId);
        })
        .Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SecureApi"))
        
        .AddDbContext<ApplicationDbContext>(o =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.");

            o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            o.EnableDetailedErrors();
            if (builder.Environment.IsDevelopment())
                o.EnableSensitiveDataLogging();

            o.AddApplicationTriggers();
        })
        .ConfigureApplicationCookie(o =>
        {
            o.Cookie.SameSite = SameSiteMode.None;
            
            o.Events.OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            o.Events.OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        })
        .AddHttpContextAccessor()
        .AddScoped<ISessionContextProvider, HttpContextSessionProvider>() // Provides the current user from the HttpContext to the session provider.
        .AddApplicationServices() // You'll need to add your own services in this function call.
        .AddAuthorization()
        .AddFastEndpoints(o =>
        {
            o.IncludeAbstractValidators = true; // Include validators from abstract classes (see https://docs.fluentvalidation.net/en/latest/).
            //o.Assemblies = [typeof(ProductRequest).Assembly]; // Adds the validators from other assemblies
        })
        .SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.Title = "MIMMISBRUNNR API";
            };
        });

    var app = builder.Build();
    // apply Database migraticons on startup, not so wise in production (Use Generated SQL Scripts) 
    // See: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli
    if (app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            dbContext.Database.EnsureDeleted(); // Delete the database if it exists to clean it up if needed.

            await dbContext.Database.EnsureCreatedAsync(); // Creates the database if it doesn't exist and applies all migrations. See Readme.md for more info.
            await dbSeeder.SeedAsync(); // Seeds the database with some test data.
        }
    }
    // Theses middlewares are strict in order of calling!
    app.UseHttpsRedirection()
        //.UseBlazorFrameworkFiles() // Blazor is also served from the API. 
        //.UseStaticFiles()
        .UseDefaultExceptionHandler()
        .UseCors("Frontend")
        .UseAuthentication()
        .UseAuthorization()
        .UseFastEndpoints(o =>
        {
            o.Endpoints.Configurator = ep =>
            {
                ep.DontAutoSendResponse();
                ep.PreProcessor<GlobalRequestLogger>(Order.Before);
                ep.PostProcessor<GlobalResponseSender>(Order.Before);
                ep.PostProcessor<GlobalResponseLogger>(Order.Before);
            };
        })
        .UseSwaggerGen();
    //app.MapFallbackToFile("index.html"); // Serves the Blazor app from the API, when no routes match.
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}


