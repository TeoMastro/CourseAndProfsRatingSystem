using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kritikos.Configuration.Persistence.Abstractions;

using Microsoft.AspNetCore.Identity;

namespace CourseAndProfsPersistence.Identity
{
  public class CaPRoles : IdentityRole<Guid>, IEntity<Guid>, ITimestamped
  {
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
  }
}
