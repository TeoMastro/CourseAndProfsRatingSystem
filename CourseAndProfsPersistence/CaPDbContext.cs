using System;
using CourseAndProfsPersistence.Identity;
using CourseAndProfsPersistence.Joins;
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

    public DbSet<Professor> Professors { get; set; }

    public DbSet<Review> Reviews{ get; set; }

    public DbSet<ProfessorCourse> ProfessorCourses { get; set; }

    public DbSet<Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<ProfessorCourse>(e =>
      {
        e.HasOne(pr => pr.Professor)
         .WithMany(c => c.ProfessorCourses)
         .HasForeignKey("ProfessorId")
         .OnDelete(DeleteBehavior.Restrict);
        e.HasOne(c => c.Course)
         .WithMany()
         .HasForeignKey("CourseId")
         .OnDelete(DeleteBehavior.Restrict);
        e.HasKey("ProfessorId", "CourseId");
      });
    }
  }
}
