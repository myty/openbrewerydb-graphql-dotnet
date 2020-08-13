using AndcultureCode.CSharp.Core.Interfaces;
using AndcultureCode.CSharp.Core.Interfaces.Conductors;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.Core.Conductors.Users.Interfaces
{
    public interface IUserConductor
    {
        IResult<User> Create(User user, string password);
    }
}
