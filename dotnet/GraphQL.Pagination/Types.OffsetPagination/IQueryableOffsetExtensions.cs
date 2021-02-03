using System.Linq;
using GraphQL.Pagination.Types.OffsetPagination;

namespace GrahpQL.Pagination.Types.OffsetPagination
{
    public static class IQueryableOffsetExtensions
    {
        private const int DEFAULT_LIMIT = 25;
        private const int DEFAULT_OFFSET = 0;

        public static OffsetConnection<TNode> ToOffsetConnection<TNode>(
            this IQueryable<TNode> queryable,
            int limit = DEFAULT_LIMIT,
            int offset = DEFAULT_OFFSET,
            bool loadTotalCount = false,
            bool loadPageInfo = false
        )
        {
            var edges = queryable
                .Skip(offset)
                .Take(limit)
                .AsEnumerable()
                .Select((r) =>
                {
                    return new OffsetEdge<TNode>
                    {
                        Node = r
                    };
                })
                .ToList();

            var connection = new OffsetConnection<TNode>
            {
                Edges = edges
            };

            if (loadTotalCount || loadPageInfo)
            {
                connection.TotalCount = queryable.Count();
            }

            if (loadPageInfo)
            {
                connection.PageInfo = new OffsetPageInfo
                {
                    HasPreviousPage = connection.TotalCount > 0 && offset > 0,
                    HasNextPage = (offset + edges.Count) < connection.TotalCount
                };
            }

            return connection;
        }
    }
}
