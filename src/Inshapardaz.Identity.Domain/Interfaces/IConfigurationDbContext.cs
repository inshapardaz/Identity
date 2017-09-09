using System;
using System.Threading.Tasks;
using Inshapardaz.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Identity.Domain.Interfaces
{
    public interface IConfigurationDbContext : IDisposable
    {
        DbSet<Client> Clients { get; set; }
        DbSet<IdentityResource> IdentityResources { get; set; }
        DbSet<ApiResource> ApiResources { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}