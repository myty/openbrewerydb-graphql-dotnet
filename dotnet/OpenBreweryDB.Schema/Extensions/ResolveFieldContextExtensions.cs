using System;
using System.Linq;
using AndcultureCode.CSharp.Core.Interfaces;
using GrahpQL.Pagination.Types.OffsetPagination;
using GraphQL.Builders;
using Panic.StringUtils;

using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace GraphQL
{
    public static class ResolveFieldContextExtensions
    {
        private const int DEFAULT_PAGE_SIZE = 25;
        private const int DEFAULT_OFFSET__SIZE = 0;

        public static bool ContainsField(this IResolveFieldContext context, string name) =>
            context.SubFields.Keys.Contains(name);

        public static void ResolveQueryableOffset<TSourceType>(
            this ConnectionBuilder<TSourceType> connection,
            Func<IResolveConnectionContext<TSourceType>, IResult<IQueryable<TSourceType>>> resolver)
            where TSourceType : Entities.Entity => connection
            .Resolve(context => resolver(context)
                .ToOffsetConnection(context));

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
            $"{context.FieldDefinition.Name}";

        public static OffsetConnection<TSourceType> ToOffsetConnection<TSourceType>(
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

            return result.ResultObject
                .ToOffsetConnection(
                    limit: context.GetArgument<int>("limit", DEFAULT_PAGE_SIZE),
                    offset: context.GetArgument<int>("offset", DEFAULT_OFFSET__SIZE),
                    loadTotalCount: context.ContainsField("totalCount"),
                    loadPageInfo: context.ContainsField("pageInfo"));
        }
    }
}
