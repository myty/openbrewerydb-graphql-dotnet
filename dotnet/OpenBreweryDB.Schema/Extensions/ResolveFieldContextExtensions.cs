using System;
using System.Linq;
using AndcultureCode.CSharp.Core.Interfaces;
using GraphQL.Builders;
using GraphQL.Types.Relay.DataObjects;
using Panic.StringUtils;

using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace GraphQL
{
    public static class ResolveFieldContextExtensions
    {
        private const int DEFAULT_PAGE_SIZE = 25;

        public static bool ContainsField(this IResolveFieldContext context, string name) =>
            context.SubFields.Keys.Contains(name);

        public static void ResolveQueryableResult<TSourceType>(
            this ConnectionBuilder<TSourceType> connection,
            Func<IResolveConnectionContext<TSourceType>, IResult<IQueryable<TSourceType>>> resolver)
            where TSourceType : Entities.Entity => connection
            .Resolve(context => resolver(context)
                .ToConnection(context));

        private static int CursorToOffset(
            this IResolveConnectionContext context,
            string cursor) => int.Parse(
            StringUtils.Base64Decode(cursor).Substring(context.Prefix().Length + 1)
        );

        private static string OffsetToCursor(
            this IResolveConnectionContext context,
            int offset) =>
            StringUtils.Base64Encode($"{context.Prefix()}:{offset}");

        private static string Prefix(this IResolveConnectionContext context) =>
            $"{context.FieldName}";

        public static Connection<TSourceType> ToConnection<TSourceType>(
            this IResult<IQueryable<TSourceType>> result,
            IResolveConnectionContext context)
            where TSourceType : Entities.Entity
        {
            if (result.HasErrors)
            {
                context.Errors.AddRange(
                    result.Errors.Select(err =>
                        new ExecutionError(err.Message)
                        {
                            Code = err.Key
                        }
                    )
                );

                return null;
            }

            var pagedResult = Enumerable.Empty<TSourceType>();

            var skip = (context.After != null)
                ? (context.CursorToOffset(context.After) + 1)
                : 0;

            if (context.IsUnidirectional || context.After != null || context.Before == null)
            {
                pagedResult = result.ResultObject
                    .Skip(skip)
                    .Take(context.PageSize ?? DEFAULT_PAGE_SIZE)
                    .AsEnumerable();
            }
            else
            {
                // TODO: Handle Before cursor
            }

            var edges = pagedResult
                .Select((r, idx) =>
                {
                    return new Edge<TSourceType>
                    {
                        Cursor = context.OffsetToCursor(skip + idx),
                        Node = r
                    };
                })
                .ToList();

            var connection = new Connection<TSourceType>
            {
                Edges = edges
            };

            var loadTotalCount = context.ContainsField("totalCount");
            var loadPageInfo = context.ContainsField("pageInfo");

            if (loadTotalCount || loadPageInfo)
            {
                connection.TotalCount = result.ResultObject.Count();
            }

            if (loadPageInfo)
            {
                connection.PageInfo = new PageInfo
                {
                    StartCursor = edges.Select(e => e.Cursor).FirstOrDefault(),
                    EndCursor = edges.Select(e => e.Cursor).LastOrDefault(),
                    HasPreviousPage = connection.TotalCount > 0 && skip > 0,
                    HasNextPage = (skip + edges.Count) < connection.TotalCount
                };
            }

            return connection;
        }
    }
}
