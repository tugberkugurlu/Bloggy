using System.Collections.Generic;

namespace Bloggy.Domain
{
    public interface IPaginatedList<TEntity> : IList<TEntity> where TEntity : IEntity
    {
        int Skipped { get; }
        int Taken { get; }
        int TotalCount { get; }
    }
}