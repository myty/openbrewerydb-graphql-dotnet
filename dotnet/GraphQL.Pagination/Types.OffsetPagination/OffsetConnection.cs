using System.Collections.Generic;
using GraphQL.Pagination.Types.OffsetPagination;

namespace GrahpQL.Pagination.Types.OffsetPagination
{
    public class OffsetConnection<TNode> : Connection<TNode, OffsetEdge<TNode>>
    {
        /// <summary>
        /// Additional pagination information for this result data set.
        /// </summary>
        public new OffsetPageInfo PageInfo { get; set; }

        /// <summary>
        /// The result data set, stored as a list of edges containing a node (the data) and a cursor (a unique identifier for the data).
        /// </summary>
        public new List<OffsetEdge<TNode>> Edges { get; set; }
    }
}
