using Microsoft.EntityFrameworkCore;
using StarkovGroupTest.Models;
using System.Configuration;

namespace StarkovGroupTest.DataBase;

internal class Context : DbContext
{
    public DbSet<DepartmentModel> Departments { get; set; }
    public DbSet<EmployeeModel> Employees { get; set; }
    public DbSet<JobTitleModel> JobTitle { get; set; }

    public Context()
    {
        if (ConfigurationManager.AppSettings["IsInitDb"]?.Equals("1") ?? false)
        {
            Database.EnsureCreated();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConfigurationManager.ConnectionStrings["Starkov"].ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DepartmentModel>().Property(t => t.ID).ValueGeneratedOnAdd();
        modelBuilder.Entity<DepartmentModel>().Ignore(t => t.Parent);
        modelBuilder.Entity<DepartmentModel>().Ignore(t => t.Manager);
        modelBuilder.Entity<DepartmentModel>().Ignore(t => t.Nested);
        modelBuilder.Entity<DepartmentModel>()
                    .HasMany(t => t.Employees)
                    .WithOne(t => t.DepartmentModel)
                    .HasForeignKey(t => t.Department)
                    .HasPrincipalKey(t => t.ID)
                    .IsRequired();
        modelBuilder.Entity<DepartmentModel>()
                    .HasMany(t => t.Departments)
                    .WithOne(t => t.Department)
                    .HasForeignKey(t => t.ParentID)
                    .HasPrincipalKey(t => t.ID)
                    .IsRequired(false);

        modelBuilder.Entity<EmployeeModel>().Property(t => t.ID).ValueGeneratedOnAdd();
        modelBuilder.Entity<EmployeeModel>().Ignore(t => t.DepartmentName);
        modelBuilder.Entity<EmployeeModel>().Ignore(t => t.JobTitleName);
        modelBuilder.Entity<EmployeeModel>().HasKey(t => t.FullName);
        modelBuilder.Entity<EmployeeModel>()
                    .HasMany(t => t.Departments)
                    .WithOne(t => t.Employee)
                    .HasForeignKey(t => t.ManagerID)
                    .HasPrincipalKey(t => t.ID)
                    .IsRequired(false);

        modelBuilder.Entity<JobTitleModel>().Property(t => t.ID).ValueGeneratedOnAdd();
        modelBuilder.Entity<JobTitleModel>().HasKey(t => t.Name);
        modelBuilder.Entity<JobTitleModel>()
                    .HasMany(t => t.Employees)
                    .WithOne(t => t.JobTitleModel)
                    .HasForeignKey(t => t.JobTitle)
                    .HasPrincipalKey(t => t.ID)
                    .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
