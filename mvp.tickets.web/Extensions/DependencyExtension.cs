using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data;
using mvp.tickets.data.Stores;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Services;
using mvp.tickets.domain.Stores;
using mvp.tickets.web.Services;

namespace mvp.tickets.web.Extensions
{
    public static class DependencyExtension
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            #region Web
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardLimit = 1;
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/login/");
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthConstants.AdminPolicy, policy => policy.RequireClaim(AuthConstants.AdminClaim));
                options.AddPolicy(AuthConstants.EmployeePolicy, policy => policy.RequireClaim(AuthConstants.EmployeeClaim));
                options.AddPolicy(AuthConstants.UserPolicy, policy => policy.RequireClaim(AuthConstants.UserClaim));
            });

            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, EmailBackgroundSearvice>();
            #endregion

            #region Data
            var connectionsStrings = new ConnectionStrings
            {
                DefaultConnection = config.GetConnectionString("DefaultConnection")
            };
            services.AddSingleton<IConnectionStrings>(connectionsStrings);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionsStrings.DefaultConnection));
            services.AddTransient<IUserStore, UserStore>();
            services.AddTransient<ICategoryStore, CategoryStore>();
            #endregion

            #region Domain
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICategoryService, CategoryService>();
            #endregion


            var settings = new Settings
            {
                FirebaseAdminConfig = File.ReadAllText(Path.Combine(env.ContentRootPath, "FirebaseAdmin.json")),
                Gmail = config.GetSection("Gmail").Get<GMailSettings>(),
                Host = config.GetValue<string>("Host"),
                ApiKey = config.GetValue<string>("ApiKey"),
                TelegramToken = config.GetValue<string>("TelegramToken"),
            };
            services.AddSingleton<ISettings>(settings);
        }
    }
}
