using System;

namespace GraphQL.Pagination
{
    /// <summary>
    /// Absteact class that contains pagination information relating to the result data set.
    /// </summary>
    public abstract class PageInfo
    {
        /// <summary>
        /// Indicates if there are additional pages of data that can be returned.
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Indicates if there are prior pages of data that can be returned.
        /// </summary>
        public bool HasPreviousPage { get; set; }
    }
}
