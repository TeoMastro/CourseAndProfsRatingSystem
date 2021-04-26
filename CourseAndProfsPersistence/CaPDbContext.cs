using System;
using CourseAndProfsPersistence.Identity;
//using CourseAndProfsPersistence.Joins;
using CourseAndProfsPersistence.Models;

using Kritikos.Configuration.Peristence.IdentityServer;

using Microsoft.EntityFrameworkCore;

namespace CourseAndProfsPersistence
{
  public class CaPDbContext : ApiAuthorizationDbContext<CaPUser,CaPRoles,Guid>
  {
    private static readonly DateTimeOffset SeededAt = DateTime.Parse("24/04/2021");

    public CaPDbContext(DbContextOptions<CaPDbContext> options)
  : base(options)
    {
    }

    public DbSet<Course> Courses{ get; set; }

    public DbSet<CourseType> CourseTypes{ get; set; }

    public DbSet<Department> Departments{ get; set; }

    public DbSet<Professor> Professors { get; set; }

    public DbSet<Review> Reviews{ get; set; }

    public DbSet<Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<Course>()
        .HasIndex(e => e.Name)
        .IsTsVectorExpressionIndex("english");

      builder.Entity<CourseType>()
        .HasIndex(e => e.Name)
        .IsTsVectorExpressionIndex("english");

      builder.Entity<Department>()
        .HasIndex(e => e.Name)
        .IsTsVectorExpressionIndex("english");

      builder.Entity<Professor>()
        .HasIndex(e => e.FullName)
        .IsTsVectorExpressionIndex("english");


      builder.Entity<Review>(e =>
      {
        e.HasOne(rv => rv.Course)
          .WithMany()
          .HasForeignKey("CourseId")
          .OnDelete(DeleteBehavior.Restrict);
        e.HasOne(mv => mv.Professor)
          .WithMany()
          .HasForeignKey("ProfessorId")
          .OnDelete(DeleteBehavior.Restrict);
        e.HasKey("CourseId", "ProfessorId");
      });
    }
  }
}
