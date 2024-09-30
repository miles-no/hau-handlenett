using HandlenettAPI.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Graph;

namespace HandlenettAPI.Services
{
    public class AzureSQLContext : DbContext
    {
        private readonly IConfiguration _config;
        public DbSet<Models.User> Users { get; set; }


        public AzureSQLContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Computed persisted col
            modelBuilder.Entity<Models.User>()
                .Property(p => p.Name)
                .HasComputedColumnSql("[FirstName] + ' ' + [LastName]", stored: true);

            modelBuilder.Entity<Models.User>(entity =>
            {
                //FK
                //entity.ToTable("nomination_revision");
                //entity.HasOne(d => d.Nomination).WithMany().HasForeignKey(d => d.NominationId);
                //entity.HasOne(d => d.Location).WithMany().HasForeignKey(d => d.LocationId);
                //entity.HasOne(d => d.Container).WithMany().HasForeignKey(d => d.ContainerId);
                //entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            //Default precision 3 for datetime2
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        if (property.GetPrecision() == null)
                        {
                            property.SetPrecision(3);
                        }
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));
            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }
            return base.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString;

            if (IsRunningInAzure())
            {
                connectionString = _config.GetConnectionString("AzureSQLDB");
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                connectionString = _config.GetConnectionString("AzureSQLDB_Local");

                var azureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ExcludeManagedIdentityCredential = true
                });

                var sqlConnection = new SqlConnection(connectionString);
                var token = azureCredential.GetToken(
                    new Azure.Core.TokenRequestContext(
                        new[] { "https://database.windows.net/.default" }
                    )).Token;

                sqlConnection.AccessToken = token;

                optionsBuilder.UseSqlServer(sqlConnection);
            }
        }

        private bool IsRunningInAzure()
        {
            return Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") != null;
        }
    }
}
