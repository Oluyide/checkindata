using APILogic.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILogic.DBSet
{
    public class LogsContext:DbContext
    {
        public LogsContext() : base("DefaultConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LogsContext, APILogic.Migrations.Configuration>());
        }
        
        public DbSet<Logs> PatientChecks { get; set; }
        public DbSet<FacilityIdentities> Facilities { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
