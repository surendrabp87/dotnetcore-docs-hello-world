using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

public class Startup
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        Configuration = configuration;
        _hostingEnvironment = hostingEnvironment;

        // Get value from azure configuration tab using the key provided
        var environmentValue = Configuration["APPSETTING_environment_stage"];

        if (!string.IsNullOrEmpty(environmentValue))
        {
            // Create a new configuration object that uses the app.{environmentValue}.json file.
            _configuration = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile($"appsettings.{environmentValue}.json", optional: false, reloadOnChange: true)
                .AddConfiguration(Configuration) // Preserve the original configuration
                .AddEnvironmentVariables()
                .Build();
        }
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Use the newConfiguration if it's available, otherwise use the original Configuration
        var configurationToUse = _configuration ?? Configuration;
        services.AddSingleton(configurationToUse);

        // Add other service configurations as needed...
        // For example, you can use the Configuration object to configure other services:
        // services.Configure<MyOptions>(Configuration.GetSection("MyOptions"));

        // Register other services here...
        services.AddRazorPages();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });

        // Get the environment_stage value for logging
        var environmentStage = configuration["environment_stage"];
        logger.LogInformation("Current environment_stage: {EnvironmentStage}", environmentStage);

        // ...

        // Get the message from configuration
        var message = configuration["Message"];
        logger.LogInformation("Message: {Message}", message);
        app.Run(async context =>
        {
            await context.Response.WriteAsync(message);
        });
    }
}
