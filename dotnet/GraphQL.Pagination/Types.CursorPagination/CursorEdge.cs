namespace GrahpQL.Pagination.Types.CursorPagination
{
    /// <summary>
    /// Represents an edge of a connection containing a node (a row of data) and cursor (a unique identifier for the row of data).
    /// </summary>
    /// <typeparam name="TNode">The data type.</typeparam>
    public class CursorEdge<TNode> : Edge<TNode>
    {
        /// <summary>
        /// The cursor of this edge's node. A cursor is a string representation of a unique identifier of this node.
        /// </summary>
        public string Cursor { get; set; }
    }
}
