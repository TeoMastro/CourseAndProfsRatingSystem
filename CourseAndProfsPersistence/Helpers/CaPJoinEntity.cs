using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kritikos.Configuration.Persistence.Abstractions;


namespace CourseAndProfsPersistence.Helpers
{
  public abstract class CaPJoinEntity : IAuditable<Guid>, ITimestamped
  {
    #region Implementation of IAuditable<Guid>

    /// <inheritdoc />
    public Guid CreatedBy { get; set; }

    /// <inheritdoc />
    public Guid UpdatedBy { get; set; }

    #endregion
    #region Implementation of ITimestamped

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; set; }

    #endregion
  }
}
