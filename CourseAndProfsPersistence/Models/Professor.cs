using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAndProfsPersistence.Helpers;
using CourseAndProfsPersistence.Joins;

namespace CourseAndProfsPersistence.Models
{
  public class Professor : CaPEntity<long>
  {
    public string FullName { get; set; }
    public string Mail { get; set; }
    public string Phone { get; set; }
    public string Office { get; set; }
    public string EOffice { get; set; }
    public string Department { get; set; }
    public double AverageRating { get; set; }
    public IReadOnlyCollection<Review> Reviews { get; set; } = new List<Review>(0);
    public IReadOnlyCollection<ProfessorCourse> ProfessorCourses { get; set; } = new List<ProfessorCourse>(0);

  }
}
