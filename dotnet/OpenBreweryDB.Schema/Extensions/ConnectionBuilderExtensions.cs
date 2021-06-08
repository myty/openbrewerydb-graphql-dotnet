using System;
using System.Linq;
using AndcultureCode.CSharp.Core.Interfaces;
using GraphQL.Builders;
using GraphQL.Relay.Types;
using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace OpenBreweryDB.Schema
{
    public static class ConnectionBuilderExtensions
    {
        public static void ResolveWith<TSourceType, TEnity>(
            this ConnectionBuilder<TSourceType> builder,
            Func<IResolveConnectionContext, IResult<IQueryable<TEnity>>> resolver,
            int defaultPageSize = 25
        ) where TEnity : Entities.Entity
        {
            builder.Resolve(context =>
            {
                var query = resolver(context).ResultObject;
                var totalCount = query.Count();

                var (startingIndex, pageSize) = GetStartingIndexAndPageSize(
                    defaultPageSize,
                    totalCount,
                    context);

                return ConnectionUtils.ToConnection(
                    query.Take(pageSize),
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
    }
}
