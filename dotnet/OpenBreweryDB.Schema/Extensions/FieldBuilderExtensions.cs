using System;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Interfaces;
using GraphQL;
using GraphQL.Builders;
using GraphQL.MicrosoftDI;
using Microsoft.Extensions.DependencyInjection;
using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace OpenBreweryDB.Schema
{
    public static class FieldBuilderExtensions
    {
        public static void ScopedResolver<TResolver, TSourceType, TEnity>(
            this FieldBuilder<TSourceType, TEnity> builder,
            Func<TResolver, Func<IResolveFieldContext, IResult<TEnity>>> resolverFunc,
            int defaultPageSize = 25
        ) where TEnity : Entities.Entity => builder.ResolveScoped(context =>
        {
            var resolverObject = context.RequestServices.GetRequiredService<TResolver>();
            var resolver = resolverFunc(resolverObject);

            return resolver(context).ResultObject;
        });

        public static void ScopedResolverAsync<TResolver, TSourceType, TResult>(
            this FieldBuilder<TSourceType, TResult> builder,
            Func<TResolver, Func<IResolveFieldContext<TSourceType>, Task<TResult>>> resolverFunc,
            int defaultPageSize = 25
        ) => builder.ResolveScopedAsync(context =>
        {
            var resolverObject = context.RequestServices.GetRequiredService<TResolver>();
            var resolver = resolverFunc(resolverObject);

            return resolver(context);
        });
    }
}
