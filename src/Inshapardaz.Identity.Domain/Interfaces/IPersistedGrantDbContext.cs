using System;
using System.Threading.Tasks;
using Inshapardaz.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Identity.Domain.Interfaces
{
    public interface IPersistedGrantDbContext : IDisposable
    {
        DbSet<PersistedGrant> PersistedGrants { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}