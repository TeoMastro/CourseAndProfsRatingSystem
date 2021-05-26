using CourseAndProfsPersistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAndProfsClientModels.Dto
{
  public class ReviewDto
  {
    public long ReviewId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; }
    public long ProfessorId { get; set; }
    public string ProfessorName { get; set; }
    public long AppsId { get; set; }
    public string Token { get; set; }
    public double UsersSubjectScore { get; set; }
    public double Rating { get; set; }
    public string Comments { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
  }
}
