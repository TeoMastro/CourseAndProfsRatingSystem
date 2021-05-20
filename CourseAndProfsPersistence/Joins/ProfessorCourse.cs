using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAndProfsPersistence.Helpers;
using CourseAndProfsPersistence.Models;

namespace CourseAndProfsPersistence.Joins
{
  public class ProfessorCourse : CaPJoinEntity
  {
#nullable disable
    public Professor Professor { get; set; }
    public Course Course { get; set; }
#nullable enable
  }
}
