using InventoryAPIRest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Queries
{
    public sealed class SqliteDbFixture : IDisposable
    {
        private readonly SqliteConnection _connection;

        public AppDbContext Context { get; }

        public SqliteDbFixture()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
        }
    }
}
