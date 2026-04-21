using Ferreteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace Ferreteria.Tests.Services
{
    public static class TestDbContextFactory
    {
        public static FerreteriaDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<FerreteriaDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new FerreteriaDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
}
