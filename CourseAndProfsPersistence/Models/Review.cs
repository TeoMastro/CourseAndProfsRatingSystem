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
    public Course Course { get; set; }
    public Professor Professor { get; set; }
    public CaPUser User { get; set; }
    public double UsersSubjectScore { get; set; }
    public double Rating { get; set; }
    public string Comments { get; set; }

  }
}
