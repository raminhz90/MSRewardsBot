using Microsoft.EntityFrameworkCore;
using System;

namespace MSRewardsBOT.Core.Models
{
    class ApplicationDbContext : DbContext
    {

        public DbSet<Account> Accounts { get; set; }


        public string DbPath { get; private set; }


        public ApplicationDbContext() 
        {

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = $"Database.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
            //options.UseLazyLoadingProxies();
        }
    }
}
