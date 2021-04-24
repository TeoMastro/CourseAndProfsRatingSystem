using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAndProfsPersistence.Helpers;

namespace CourseAndProfsPersistence.Models
{
  public class Professor : CaPEntity<long>
  {
    public string ProfFullName { get; set; }
    public string ProfMail { get; set; }
    public string ProfPhone { get; set; }
    public string ProfOffice { get; set; }
    public string ProfEOffice { get; set; }
    public Department ProfDepartMent { get; set; }

  }
}
