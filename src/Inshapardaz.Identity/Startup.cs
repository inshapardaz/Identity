using System;
using System.Reflection;
using Inshapardaz.Identity.Data;
using Inshapardaz.Identity.Models;
using Inshapardaz.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Identity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                                                            options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(connectionString, sql =>
                                                     sql.MigrationsAssembly(migrationsAssembly));
                    })
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(connectionString,
                                                 sql => sql.MigrationsAssembly(migrationsAssembly));
                    })
                    .AddAspNetIdentity<ApplicationUser>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseIdentity();
            app.UseIdentityServer();

            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    SignInScheme = AuthenticationSchemeConstants.ExternalCookieAuthenticationScheme,
            //    AccessType = "offline",
            //    ClientId = "<..>",
            //    ClientSecret = "<..>",
            //    Scope =
            //    {
            //        "https://www.googleapis.com/auth/plus.me",
            //        "https://www.googleapis.com/auth/userinfo.email",
            //        "https://www.googleapis.com/auth/userinfo.profile"
            //    }
            //});

            //app.UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions
            //{
            //    //https://apps.dev.microsoft.com/?mkt=en-us#/appList
            //    AuthenticationScheme = "Microsoft",
            //    SignInScheme = AuthenticationSchemeConstants.ExternalCookieAuthenticationScheme,
            //    ClientId = "<..>",
            //    ClientSecret = "<..>",
            //});

            //app.UseTwitterAuthentication(new TwitterOptions
            //{
            //    AuthenticationScheme = "Twitter",
            //    SignInScheme = AuthenticationSchemeConstants.ExternalCookieAuthenticationScheme,
            //    ConsumerKey = "<..>",
            //    ConsumerSecret = "<..>",
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DataInitializer.MigrateDatabase(app);
            DataInitializer.Initialize(app).Wait();
            DataInitializer.InitializeIdentity(app);
        }
    }
}
