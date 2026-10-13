using DocFlowSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocFlowSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }   

        public DbSet<Document> Documents { get; set; }
        public DbSet<User> Users { get; set; } // جدول المستخدمين الجديد

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إعدادات جدول المستندات
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            });

            // إعدادات جدول المستخدمين
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique(); // البريد الإلكتروني فريد ولا يتكرر
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            });
        }
    }
}