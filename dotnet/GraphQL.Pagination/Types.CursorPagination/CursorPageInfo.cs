namespace GraphQL.Pagination.Types.CursorPagination
{
    /// <summary>
    /// Contains pagination information relating to a cursor-based result data set.
    /// </summary>
    public class CursorPageInfo : PageInfo
    {
        /// <summary>
        /// The cursor of the first node in the result data set.
        /// </summary>
        public string StartCursor { get; set; }

        /// <summary>
        /// The cursor of the last node in the result data set.
        /// </summary>
        public string EndCursor { get; set; }
    }
}
