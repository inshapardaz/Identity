using System;
using System.Threading.Tasks;
using Inshapardaz.Identity.Domain.Entities;
using Inshapardaz.Identity.Domain.Extensions;
using Inshapardaz.Identity.Domain.Interfaces;
using Inshapardaz.Identity.Domain.Options;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Identity.Domain.DbContexts
{
    public class PersistedGrantDbContext : DbContext, IPersistedGrantDbContext
    {
        private readonly OperationalStoreOptions _storeOptions;

        public PersistedGrantDbContext(DbContextOptions<PersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
            : base(options)
        {
            if (storeOptions == null) throw new ArgumentNullException(nameof(storeOptions));
            this._storeOptions = storeOptions;
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigurePersistedGrantContext(_storeOptions);

            base.OnModelCreating(modelBuilder);
        }
    }
}