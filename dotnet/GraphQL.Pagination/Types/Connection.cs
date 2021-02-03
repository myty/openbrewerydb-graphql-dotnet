using System.Collections.Generic;
using System.Linq;
using GraphQL.Pagination;

namespace GrahpQL.Pagination.Types
{
    /// <summary>
    /// Represents a connection result containing nodes and pagination information.
    /// </summary>
    /// <typeparam name="TNode">The data type.</typeparam>
    /// <typeparam name="TEdge">The edge type, typically <see cref="Edge{TNode}"/>.</typeparam>
    public abstract class Connection<TNode, TEdge>
        where TEdge : Edge<TNode>
    {
        /// <summary>
        /// The total number of records available.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Additional pagination information for this result data set.
        /// </summary>
        public virtual PageInfo PageInfo { get; set; }

        /// <summary>
        /// The result data set, stored as a list of edges containing a node (the data) and a cursor (a unique identifier for the data).
        /// </summary>
        public virtual List<TEdge> Edges { get; set; }

        // TODO: This belongs on the GraphQL type
        /// <summary>
        /// The result data set.
        /// </summary>
        public List<TNode> Items => Edges?.Select(edge => edge.Node).ToList();
    }

}
