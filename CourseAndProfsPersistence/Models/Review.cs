using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAndProfsPersistence.Helpers;
using CourseAndProfsPersistence.Identity;

namespace CourseAndProfsPersistence.Models
{
  public class Review : CaPEntity<long>
  {
    public Course RevCourse { get; set; }
    public Professor RevProfessor { get; set; }
    public CaPUser RevUser { get; set; }
    public double UsersSubjectScore { get; set; }
    public double RevRating { get; set; }
    public string RevComments { get; set; }

  }
}
