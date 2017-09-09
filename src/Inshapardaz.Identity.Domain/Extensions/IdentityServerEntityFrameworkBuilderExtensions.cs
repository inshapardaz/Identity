using System;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Inshapardaz.Identity.Domain.DbContexts;
using Inshapardaz.Identity.Domain.Interfaces;
using Inshapardaz.Identity.Domain.Options;
using Inshapardaz.Identity.Domain.Services;
using Inshapardaz.Identity.Domain.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Identity.Domain.Extensions
{
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder, 
            Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            var options = new ConfigurationStoreOptions();
            builder.Services.AddSingleton(options);
            storeOptionsAction?.Invoke(options);

            builder.Services.AddDbContext<ConfigurationDbContext>(dbCtxBuilder =>
            {
                options.ConfigureDbContext?.Invoke(dbCtxBuilder);
            });
            builder.Services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            // these need to be registered as concrete classes in DI for
            // the caching decorators to work
            builder.Services.AddTransient<ClientStore>();
            builder.Services.AddTransient<ResourceStore>();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            builder.Services.AddSingleton<TokenCleanup>();
            builder.Services.AddSingleton<IStartupFilter, TokenCleanupConfig>();

            var storeOptions = new OperationalStoreOptions();
            builder.Services.AddSingleton(storeOptions);
            storeOptionsAction?.Invoke(storeOptions);

            builder.Services.AddDbContext<PersistedGrantDbContext>(dbCtxBuilder =>
            {
                storeOptions.ConfigureDbContext?.Invoke(dbCtxBuilder);
            });

            builder.Services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }

        class TokenCleanupConfig : IStartupFilter
        {
            private readonly IApplicationLifetime _applicationLifetime;
            private readonly TokenCleanup _tokenCleanup;
            private readonly OperationalStoreOptions _options;

            public TokenCleanupConfig(IApplicationLifetime applicationLifetime, TokenCleanup tokenCleanup, OperationalStoreOptions options)
            {
                _applicationLifetime = applicationLifetime;
                _tokenCleanup = tokenCleanup;
                _options = options;
            }

            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                if (_options.EnableTokenCleanup)
                {
                    _applicationLifetime.ApplicationStarted.Register(_tokenCleanup.Start);
                    _applicationLifetime.ApplicationStopping.Register(_tokenCleanup.Stop);
                }

                return next;
            }
        }
    }
}