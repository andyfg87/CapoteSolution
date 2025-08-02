using CapoteSolution.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapoteSolution.Models.EF
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Toner> Toners { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<MachineModel> MachineModels { get; set; }        
        public DbSet<Copier> Copiers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceReason> ServiceReasons { get; set; }
        public DbSet<Customer> Customers { get; set; }

        // Configuración de la conexión (LocalDB)
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=YourAppDb;Trusted_Connection=True;"
                );
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Role).HasConversion<string>();
            });

            // Configuración de TONER
            modelBuilder.Entity<Toner>(entity =>
            {
                entity.HasIndex(t => t.Model).IsUnique();
            });

            // Configuración de BRAND
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(b => b.Name).IsUnique();
            });

            // MachineModel
            modelBuilder.Entity<MachineModel>()
                .HasOne(mm => mm.Brand)
                .WithMany(b => b.MachineModels)
                .HasForeignKey(mm => mm.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MachineModel>()
                .HasOne(mm => mm.Toner)
                .WithMany(t => t.MachineModels)
                .HasForeignKey(mm => mm.TonerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Copier
            modelBuilder.Entity<Copier>()
                .HasIndex(c => c.SerialNumber).IsUnique();

            modelBuilder.Entity<Copier>()
                .HasOne(c => c.MachineModel)
                .WithMany(mm => mm.Copiers)
                .HasForeignKey(c => c.MachineModelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contract (1:1 con Copier)
            /*modelBuilder.Entity<Contract>()
                .HasOne(c => c.Copier)
                .WithOne(c => c.Contract)
                .HasForeignKey<Contract>(c => c.CopierId)
                .OnDelete(DeleteBehavior.ClientNoAction);*/

            // Contract (1:1 con Customer)
            /*modelBuilder.Entity<Copier>()
               .HasOne(c => c.Customer)
               .WithOne(c => c.Copier)
               .HasForeignKey<Copier>(c => c.CustomerId)
               .OnDelete(DeleteBehavior.ClientNoAction);*/

            // Customer ( 1: * Copier)
            modelBuilder.Entity<Copier>()
                 .HasOne(c => c.Customer)
                 .WithMany(mm => mm.Copiers)
                 .OnDelete(DeleteBehavior.ClientNoAction);


            // Service
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Copier)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CopierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.ServiceReason)
                .WithMany(sr => sr.Services)
                .HasForeignKey(s => s.ServiceReasonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Technician)
                .WithMany(u => u.Services)
                .HasForeignKey(s => s.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed de ServiceReason
            modelBuilder.Entity<ServiceReason>().HasData(
                new ServiceReason { Id = 1, Name = ServiceReason.Reasons.TonerChange },
                new ServiceReason { Id = 2, Name = ServiceReason.Reasons.MonthlyCounter },
                new ServiceReason { Id = 3, Name = ServiceReason.Reasons.Maintenance}
            );
        }

    }
}
