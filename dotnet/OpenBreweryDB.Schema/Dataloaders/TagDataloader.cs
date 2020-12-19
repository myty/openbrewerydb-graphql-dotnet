using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Core.Conductors.Tags.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Schema.Dataloaders
{
    public class TagDataloader
    {
        private readonly ITagConductor _tagConductor;

        public TagDataloader(ITagConductor tagConductor)
        {
            _tagConductor = tagConductor;
        }

        public async Task<IDictionary<long, Tag>> GetTagById(IEnumerable<long> ids, CancellationToken cancellationToken) => await _tagConductor
            .FindAll(t => ids.Contains(t.Id))
            .ThrowIfAnyErrorsOrResultIsNull()
            .ResultObject
            .ToDictionaryAsync(c => c.Id);
    }
}
