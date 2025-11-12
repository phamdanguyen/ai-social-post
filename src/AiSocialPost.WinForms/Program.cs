using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AiSocialPost.WinForms.Forms;
using AiSocialPost.Data;
using AiSocialPost.Core.Services;
using AiSocialPost.AI;
using Microsoft.EntityFrameworkCore;

namespace AiSocialPost.WinForms
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Load configuration
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Initialize database
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            // Run main form
            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddSingleton(Configuration);

            // Database
            services.AddDbContext<AppDbContext>(options =>
            {
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aisocialpost.db");
                options.UseSqlite($"Data Source={dbPath}");
            });

            // AI Services
            services.AddSingleton<GeminiRateLimiter>();
            services.AddScoped<GeminiService>(sp =>
            {
                var rateLimiter = sp.GetRequiredService<GeminiRateLimiter>();
                var apiKey = Configuration["Google:ApiKey"] ?? "";
                return new GeminiService(rateLimiter, apiKey);
            });

            // Core Services
            services.AddScoped<ITrendService, TrendService>();
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<IPostingService, PostingService>();
            services.AddScoped<ICommentService, CommentService>();

            // Forms
            services.AddTransient<MainForm>();
            services.AddTransient<SettingsForm>();
        }
    }
}
