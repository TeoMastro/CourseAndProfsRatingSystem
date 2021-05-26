using CourseAndProfsPersistence.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAndProfsPersistence.Identity
{
  public class UserAuth : CaPEntity<long>
  {
    public long Appsid { get; set; }
    public string Token { get; set; }
  }
}
