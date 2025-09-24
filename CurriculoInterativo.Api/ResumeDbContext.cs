using CurriculoInterativo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api
{
    public class ResumeDbContext : DbContext
    {
        public ResumeDbContext(DbContextOptions<ResumeDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Responsibility> Responsibilities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Configurações da entidade User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            #endregion

            #region Configuração  RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired();
                entity.Property(e => e.ExpiryDate).IsRequired();
                entity.Property(e => e.IsRevoked).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Token).IsUnique();
            });
            #endregion

            #region Configurações da entidade Experience
            modelBuilder.Entity<Experience>()
                .Property(e => e.Company)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Experience>()
                .Property(e => e.Location)
                .HasMaxLength(150);

            modelBuilder.Entity<Experience>()
                .Property(e => e.Description)
                .HasMaxLength(1000);

            #endregion

            #region Configurações da entidade Skill
            modelBuilder.Entity<Skill>()
                .Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Skill>()
                .Property(s => s.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Skill>()
                .Property(s => s.Category)
                .IsRequired();

            modelBuilder.Entity<Skill>()
                .Property(s => s.ProficiencyLevel)
                .IsRequired();

            #endregion

            #region Configurações da entidade Project
            modelBuilder.Entity<Project>()
                .Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Project>()
                .Property(p => p.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Project>()
                .Property(p => p.Position)
                .HasMaxLength(150);

            #endregion

            #region Configurações da entidade Certification
            modelBuilder.Entity<Certification>()
                .Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Certification>()
                .Property(c => c.Institution)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Certification>()
                .Property(c => c.Category)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Certification>()
                .Property(c => c.CertificateUrl)
                .HasMaxLength(500);

            modelBuilder.Entity<Certification>()
                .Property(c => c.ImageUrl)
                .HasMaxLength(500);

            #endregion

            #region Configurações da entidade Contact
            modelBuilder.Entity<Contact>()
                .Property(c => c.Name)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Contact>()
                .Property(c => c.Email)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Contact>()
                .Property(c => c.LinkedIn)
                .HasMaxLength(300);

            modelBuilder.Entity<Contact>()
                .Property(c => c.Location)
                .HasMaxLength(150);

            modelBuilder.Entity<Contact>()
                .Property(c => c.Phone)
                .HasMaxLength(20);

            modelBuilder.Entity<Contact>()
                .Property(c => c.GitHub)
                .HasMaxLength(300);

            #endregion

            #region Configurações da entidade Responsibility
            modelBuilder.Entity<Responsibility>()
                .Property(r => r.Description)
                .HasMaxLength(500)
                .IsRequired();

            #endregion

            #region Relacionamentos
            // Experience -> Projects (One-to-Many)
            modelBuilder.Entity<Project>()
                .HasOne<Experience>()
                .WithMany(e => e.Projects)
                .HasForeignKey("ExperienceId")
                .OnDelete(DeleteBehavior.Cascade);

            // Project -> Responsibilities (One-to-Many)
            modelBuilder.Entity<Responsibility>()
                .HasOne<Project>()
                .WithMany(p => p.Responsibilities)
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);

            // Project -> Skills (Many-to-Many)
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Skills)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectSkill",
                    j => j.HasOne<Skill>().WithMany().HasForeignKey("SkillId"),
                    j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId"));

            #endregion

            #region Índices para melhor performance
            modelBuilder.Entity<Experience>()
                .HasIndex(e => e.Company);

            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.Name);

            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.Category);

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<Certification>()
                .HasIndex(c => c.Category);

            modelBuilder.Entity<Contact>()
                .HasIndex(c => c.Email)
                .IsUnique();
            #endregion
        }
    }
}
