using System.Collections.Generic;

namespace Bloggy.Domain
{
    public class PaginatedList<T> : List<T>, IPaginatedList<T> where T : IEntity
    {
        public int Skipped { get; private set; }
        public int Taken { get; private set; }
        public int TotalCount { get; private set; }

        public PaginatedList(IEnumerable<T> source, int skipped, int taken, int totalCount)
        {
            AddRange(source);

            Taken = taken;
            Skipped = skipped;
            TotalCount = totalCount;
        }
    }
}