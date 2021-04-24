using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAndProfsPersistence.Helpers;

namespace CourseAndProfsPersistence.Identity
{
  public class Device : CaPEntity<long>
  {
    public string Name { get; set; }

    public CaPUser User { get; set; }

    public Guid RefreshToken { get; set; }
  }
}
