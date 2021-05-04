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
    public long ProfessorId { get; set; }
    public Guid UserId { get; set; }
    public double UsersSubjectScore { get; set; }
    public double Rating { get; set; }
    public string Comments { get; set; }
  }
}
