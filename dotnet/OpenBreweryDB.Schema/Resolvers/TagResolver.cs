using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Schema.Dataloaders;

namespace OpenBreweryDB.Schema.Resolvers
{
    public class TagResolver
    {
        private readonly IDataLoaderContextAccessor _accessor;
        private readonly TagDataloader _tagDataloader;

        public TagResolver(
            IDataLoaderContextAccessor accessor,
            TagDataloader tagDataloader)
        {
            _accessor = accessor;
            _tagDataloader = tagDataloader;
        }

        public async Task<IEnumerable<string>> ResolveTagListAsync(IResolveFieldContext<Brewery> context)
        {
            var tagIds = context.Source
                .BreweryTags
                .Select(bt => bt.TagId)
                .ToList() ?? Enumerable.Empty<long>();

            var loader = _accessor.Context.GetOrAddBatchLoader<long, Tag>("GetTagById", _tagDataloader.GetTagById);

            var tags = await Task.WhenAll(tagIds
                .Select(async id =>
                {
                    var tag = await loader.LoadAsync(id).GetResultAsync();

                    return tag?.Name;
                })
            );

            return tags.Where(t => (t?.Trim() ?? "") != "");
        }
    }
}
