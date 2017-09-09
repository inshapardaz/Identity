using System;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Identity.Domain.Options
{
    public class OperationalStoreOptions
    {
        public string DefaultSchema { get; set; } = null;
        public Action<DbContextOptionsBuilder> ConfigureDbContext { get; set; }

        public TableConfiguration PersistedGrants { get; set; } = new TableConfiguration("PersistedGrants");

        public bool EnableTokenCleanup { get; set; } = false;
        public int TokenCleanupInterval { get; set; } = 3600;
    }
}