using Microsoft.EntityFrameworkCore;

namespace WebApiBD.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Sotrudniki> Sotrudniki { get; set; }
        public DbSet<Polzovateli> Polzovateli { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sotrudniki>().ToTable("Sotrydniki").HasKey(s => s.IDSotrydnika);
            modelBuilder.Entity<Polzovateli>().ToTable("Polzovateli").HasKey(s=>s.IDPolzovateliaDlyaAvtorizacii);
            modelBuilder.Entity<Role>().ToTable("Roli").HasKey(r => r.IDRoli);
            modelBuilder.Entity<Polzovateli>()
            .HasOne(p => p.Roli)
            .WithMany()
            .HasForeignKey(p => p.IDRoli)
            .HasConstraintName("FK_Polzovateli_Roles_IDRoli");
        }
    }
}
