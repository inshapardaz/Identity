using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Inshapardaz.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Identity.Data
{
    public static class DataInitializer
    {
        public static void MigrateDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var database = context.Database;
                database.Migrate();

                var persistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();

                var persistedGrantDb = persistedGrantDbContext.Database;
                persistedGrantDb.Migrate();

                var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();

                var configurationDatabase = configurationDbContext.Database;
                configurationDatabase.Migrate();
            }
        }

        public static async Task Initialize(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //initializing custom roles 
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string[] roleNames = { "Super", "Administrator", "Contributor", "Reader" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        //create the roles and seed them to the database: Question 1
                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                //Here you could create a super user who will maintain the web app
                var poweruser = new ApplicationUser
                {

                    UserName = configuration["AppSettings:SuperUserName"],
                    Email = configuration["AppSettings:SuperUserEmail"],
                };

                //Ensure you have these values in your appsettings.json file
                string userPWD = configuration["AppSettings:SuperUserPassword"];
                var _user = await userManager.FindByEmailAsync(configuration["AppSettings:SuperUserEmail"]);

                if (_user == null)
                {
                    var createPowerUser = await userManager.CreateAsync(poweruser, userPWD);
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the role
                        await userManager.AddToRoleAsync(poweruser, "Super");
                    }
                    else
                    {
                        throw new ApplicationException("Unable to create administrator user account." + Environment.NewLine +  string.Join('\n' , createPowerUser.Errors));
                    }
                }
            }
        }
        
        internal static void InitializeIdentity(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var environment = scope.ServiceProvider.GetService<IHostingEnvironment>();
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in Clients.Get(environment))
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Resources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Resources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //if (!userManager.Users.Any())
                //{
                //    foreach (var testUser in Users.Get())
                //    {
                //        var identityUser = new ApplicationUser
                //        {
                //            Id = testUser.SubjectId,
                //            UserName = testUser.Username
                //        };

                //        userManager.CreateAsync(identityUser, testUser.Password).Wait();
                //        userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();
                //    }
                //}
            }
        }
    }

    internal class Clients
    {
        public static IEnumerable<Client> Get(IHostingEnvironment environment)
        {
            if (environment.IsProduction())
            {
                return new List<Client>
                {
                    new Client
                    {
                        ClientId = "inshapardaz-web",
                        ClientName = "Inshapardaz Website",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowedCorsOrigins = new List<string> { "http://inshapardaz.azurewebsites.net"},
                        AllowAccessTokensViaBrowser = true,
                        ClientSecrets = new List<Secret>
                        {
                            new Secret("suoauthClientperSecretPassword".Sha256())
                        },
                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Email,
                            "role",
                            "inshapardazAPI.read",                            
                            "inshapardazAPI.write"
                        },
                        ClientUri = "http://inshapardaz.azurewebsites.net",
                        RedirectUris = new List<string>
                        {
                            "http://inshapardaz.azurewebsites.net",
                            "http://inshapardaz.azurewebsites.net/silent-renew.html"
                        },
                        PostLogoutRedirectUris = new List<string> { "http://inshapardaz.azurewebsites.net" }
                    }
                };
            }

            return new List<Client>
                {
                    new Client
                    {
                        ClientId = "inshapardaz-web",
                        ClientName = "Inshapardaz Website",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowedCorsOrigins = new List<string> {"http://localhost:4200"},
                        AllowAccessTokensViaBrowser = true,
                        ClientSecrets = new List<Secret>
                        {
                            new Secret("suoauthClientperSecretPassword".Sha256())
                        },
                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Email,
                            "role",
                            "inshapardazAPI.read",
                            "inshapardazAPI.write"
                        },
                        ClientUri = "http://localhost:4200",
                        RedirectUris = new List<string>
                        {
                            "http://localhost:4200",
                            "http://localhost:4200/silent-renew.html"
                        },
                        PostLogoutRedirectUris = new List<string> { "http://localhost:4200" }
                    }
                };
        }
    }

    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(
                    name: "custom.profile",
                    displayName: "Custom Profile",
                    claimTypes: new[] { "name", "email", "role" })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
                new ApiResource {
                    Name = "inshapardazAPI",
                    DisplayName = "Custom API",
                    Description = "Custom API Access",
                    UserClaims = new List<string> {"role"},
                    ApiSecrets = new List<Secret> {new Secret("scopeSecret".Sha256())},
                    Scopes = new List<Scope> {
                        new Scope("inshapardazAPI.read"),
                        new Scope("inshapardazAPI.write")
                    }
                }
            };
        }
    }
}
