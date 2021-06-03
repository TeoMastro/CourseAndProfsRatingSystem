using CourseAndProfsPersistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAndProfsClientModels.Dto
{
  public class ProfessorDto
  {
    public long Id { get; set; }
    public string FullName { get; set; }
    public string Mail { get; set; }
    public string Phone { get; set; }
    public string Office { get; set; }
    public string EOffice { get; set; }
    public string Department { get; set; }
    public double AverageRating { get; set; }
    public double TotalReviews { get; set; }
  }
}
