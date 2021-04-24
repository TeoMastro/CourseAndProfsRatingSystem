using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kritikos.Configuration.Persistence.Abstractions;

namespace CourseAndProfsPersistence.Helpers
{
  public class CaPEntity<TKey> : CaPJoinEntity, IEntity<TKey>
    where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
  {
    public TKey Id { get; set; }
  }
}
