using System;
using System.Linq;
using AndcultureCode.CSharp.Core.Extensions;
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
                .ThrowIfAnyErrors()
                .GetPagedResults(context));

        private static int CursorToOffset<T>(string cursor) => int.Parse(
            StringUtils.Base64Decode(cursor).Substring(Prefix<T>().Length + 1)
        );

        private static string OffsetToCursor<T>(int offset) =>
            StringUtils.Base64Encode($"{Prefix<T>()}:{offset}");

        private static string Prefix<T>() => typeof(T).Name;

        public static Connection<TSourceType> GetPagedResults<TSourceType>(
            this IResult<IQueryable<TSourceType>> result,
            IResolveConnectionContext<object> context)
            where TSourceType : Entities.Entity
        {
            result.ThrowIfAnyErrors();

            var filteredResult = result.ResultObject;
            var pagedResult = Enumerable.Empty<TSourceType>();
            var totalCount = filteredResult.Count();

            var skip = (context.After != null)
                ? (CursorToOffset<TSourceType>(context.After) + 1)
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
                        Cursor = OffsetToCursor<TSourceType>(skip + idx),
                        Node = r
                    };
                })
                .ToList();

            return new Connection<TSourceType>
            {
                Edges = edges,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    StartCursor = edges.Select(e => e.Cursor).FirstOrDefault(),
                    EndCursor = edges.Select(e => e.Cursor).LastOrDefault(),
                    HasPreviousPage = totalCount > 0 && skip > 0,
                    HasNextPage = (skip + edges.Count) < totalCount
                }
            };
        }
    }
}
