namespace GrahpQL.Pagination.Types
{
    /// <summary>
    /// Represents an edge of a connection containing a node (a row of data)
    /// </summary>
    /// <typeparam name="TNode">The data type.</typeparam>
    public abstract class Edge<TNode>
    {
        /// <summary>
        /// The node. A node is a single row of data within the result data set.
        /// </summary>
        public TNode Node { get; set; }
    }
}
