﻿using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Minio;
using mvp.tickets.data;
using mvp.tickets.data.Stores;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Services;
using mvp.tickets.domain.Stores;
using mvp.tickets.web.Kafka;
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
                options.AddPolicy(AuthConstants.RootSpacePolicy, policy => policy.RequireClaim(AuthConstants.RootSpaceClaim));
            });

            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, EmailBackgroundSearvice>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, RequestTimeConsumer>();
            #endregion

            #region Data
            var connectionsStrings = new ConnectionStrings
            {
                DefaultConnection = config.GetConnectionString("DefaultConnection"),
                RedisConnection = config.GetConnectionString("RedisConnection"),
                KafkaConnection = config.GetConnectionString("KafkaConnection")
            };
            services.AddSingleton<IConnectionStrings>(connectionsStrings);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionsStrings.RedisConnection;
                options.InstanceName = "RedisInstance";
            });

            services.AddSingleton<KafkaClientHandle>();
            services.AddSingleton<KafkaDependentProducer<Null, string>>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionsStrings.DefaultConnection));
            services.AddTransient<IUserStore, UserStore>();
            services.AddTransient<ICategoryStore, CategoryStore>();
            services.AddTransient<ICompanyStore, CompanyStore>();
            #endregion

            #region Domain
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddSingleton<IS3Service, S3Service>();
            #endregion


            var settings = new Settings
            {
                FirebaseAdminConfig = File.ReadAllText(Path.Combine(env.ContentRootPath, "FirebaseAdmin.json")),
                Gmail = config.GetSection("Gmail").Get<GMailSettings>(),
                Host = config.GetValue<string>("Host"),
                ApiKey = config.GetValue<string>("ApiKey"),
                TelegramToken = config.GetValue<string>("TelegramToken"),
                S3 = config.GetSection("S3").Get<S3Settings>(),
            };
            services.AddSingleton<ISettings>(settings);

            services.AddMinio(configureClient => configureClient
                .WithEndpoint(settings.S3.Url)
                .WithSSL(true)
                .WithCredentials(settings.S3.AccessKey, settings.S3.SecretKey));
        }
    }
}
