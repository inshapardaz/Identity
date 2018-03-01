using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Inshapardaz.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
            }
        }

        public static async Task Initialize(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                string[] roles = new string[] {"Owner", "Administrator", "Contributor", "Reader"};

                foreach (string role in roles)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);

                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        await roleStore.CreateAsync(new IdentityRole(role) {NormalizedName = role.ToUpper()});
                    }
                }


                var user = new ApplicationUser
                {
                    Email = "momarf@gmail.com",
                    NormalizedEmail = "MOMARF@GMAIL.COM",
                    UserName = "momarf@gmail.com",
                    NormalizedUserName = "MOMARF@GMAIL.COM",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };


                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(user, "changeit");
                    user.PasswordHash = hashed;

                    var userStore = new UserStore<ApplicationUser>(context);
                    await userStore.CreateAsync(user);
                    await userStore.AddToRoleAsync(user, "Owner");
                    await context.SaveChangesAsync();
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
                        AllowedCorsOrigins = new List<string> {"http://localhost:4200"},
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
                            "customAPI.write"
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
                            "customAPI.write"
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
                new IdentityResource {
                    Name = "role",
                    UserClaims = new List<string> {"role"}
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
                new ApiResource {
                    Name = "customAPI",
                    DisplayName = "Custom API",
                    Description = "Custom API Access",
                    UserClaims = new List<string> {"role"},
                    ApiSecrets = new List<Secret> {new Secret("scopeSecret".Sha256())},
                    Scopes = new List<Scope> {
                        new Scope("customAPI.read"),
                        new Scope("customAPI.write")
                    }
                }
            };
        }
    }

    internal class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "guest",
                    Password = "tiger",
                    Claims = new List<Claim> {
                        new Claim(JwtClaimTypes.Email, "guest@tiget.com"),
                        new Claim(JwtClaimTypes.Role, "reader")
                    }
                }
            };
        }
    }
}
