using System;
using System.Collections.Generic;
using System.Linq;
using AndcultureCode.CSharp.Core.Interfaces;
using GraphQL.Builders;
using GraphQL.MicrosoftDI;
using GraphQL.Relay.Types;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.Extensions.DependencyInjection;
using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace GraphQL.Core.Extensions
{
    public static class ConnectionBuilderExtensions
    {
        public static void ScopedResolver<TResolver, TSourceType, TEnity>(
            this ConnectionBuilder<TSourceType> builder,
            Func<TResolver, Func<IResolveConnectionContext, IResult<IQueryable<TEnity>>>> resolverFunc,
            int defaultPageSize = 25
        ) where TEnity : Entities.Entity
        {
            builder.ResolveScoped(context =>
            {
                var resolverObject = context.RequestServices.GetRequiredService<TResolver>();
                var resolver = resolverFunc(resolverObject);

                var query = resolver(context).ResultObject;
                var totalCount = query.Count();

                var (startingIndex, pageSize) = GetStartingIndexAndPageSize(
                    defaultPageSize,
                    totalCount,
                    context);

                return ToConnection(
                    query.Skip(startingIndex).Take(pageSize),
                    context,
                    startingIndex,
                    totalCount);
            });
        }

        private static (int startingIndex, int pageSize) GetStartingIndexAndPageSize<TSourceType>(int defaultPageSize, int totalCount, IResolveConnectionContext<TSourceType> context)
        {
            var (first, last) = GetFirstLast(context);
            var pageSize = last ?? first ?? defaultPageSize;

            if (!string.IsNullOrEmpty(context.After))
            {
                var after = ConnectionUtils.CursorToOffset(context.After);

                if (last.HasValue)
                {
                    var startingIndex = totalCount - last.Value;

                    return (
                        startingIndex: startingIndex < after ? after : startingIndex,
                        pageSize);
                }

                return (
                    startingIndex: after + 1,
                    pageSize
                );
            }

            if (!string.IsNullOrEmpty(context.Before))
            {
                var before = ConnectionUtils.CursorToOffset(context.Before);

                if (first.HasValue)
                {
                    var lastIndex = pageSize - 1;

                    return (
                        startingIndex: 0,
                        pageSize: lastIndex > before ? before - 1 : pageSize);
                }

                if (last.HasValue)
                {
                    var startingIndex = before - pageSize - 1;
                    var lastIndex = startingIndex + pageSize - 1;

                    return (
                        startingIndex: startingIndex < 0 ? 0 : startingIndex,
                        pageSize: lastIndex > before ? lastIndex - startingIndex : pageSize);
                }
            }

            return (
                startingIndex: 0,
                pageSize);
        }

        private static (int? first, int? last) GetFirstLast<TSourceType>(IResolveConnectionContext<TSourceType> context)
        {
            if (context.Last == null)
            {
                return (
                    first: context.First,
                    last: context.First.HasValue ? null : context.Last);
            }

            return (
                first: context.First.HasValue ? context.First : null,
                last: context.First.HasValue ? null : context.Last);
        }
        public static Connection<TSource> ToConnection<TSource, TParent>(
            IEnumerable<TSource> slice,
            IResolveConnectionContext<TParent> context,
            int sliceStartIndex,
            int totalCount
        )
        {
            var edges = slice
                .Select((item, i) => new Edge<TSource>
                {
                    Node = item,
                    Cursor = ConnectionUtils.OffsetToCursor(sliceStartIndex + i)
                })
                .ToList();

            var firstEdge = edges.FirstOrDefault();
            var lastEdge = edges.LastOrDefault();

            return new Connection<TSource>
            {
                Edges = edges,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    StartCursor = firstEdge?.Cursor,
                    EndCursor = lastEdge?.Cursor,
                    HasPreviousPage = sliceStartIndex > 0,
                    HasNextPage = (sliceStartIndex + edges.Count) < totalCount,
                }
            };
        }
    }
}
