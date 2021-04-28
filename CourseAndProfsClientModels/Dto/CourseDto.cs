using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAndProfsPersistence.Models;

namespace CourseAndProfsClientModels.Dto
{
  public class CourseDto
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public CourseType Type { get; set; }
  }
}
