using System.Collections.Generic;
using GraphQL.Pagination.Types.CursorPagination;

namespace GrahpQL.Pagination.Types.CursorPagination
{
    public class CursorConnection<TNode> : Connection<TNode, CursorEdge<TNode>>
    {
        /// <summary>
        /// Additional pagination information for this result data set.
        /// </summary>
        public new CursorPageInfo PageInfo { get; set; }

        /// <summary>
        /// The result data set, stored as a list of edges containing a node (the data) and a cursor (a unique identifier for the data).
        /// </summary>
        public new List<CursorEdge<TNode>> Edges { get; set; }
    }
}
