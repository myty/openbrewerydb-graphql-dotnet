using System;
using System.Security.Cryptography;
using System.Text;
using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Core.Conductors.Users.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models.Users;

public class UserConductor : IUserConductor
{
    readonly BreweryDbContext _data;

    public UserConductor(BreweryDbContext data)
    {
        _data = data;
    }

    public IResult<User> Create(User user, string password) => Do<User>.Try((r) =>
    {
        var salt = Guid.NewGuid().ToString("N");

        using (var sha = SHA512.Create())
        {
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt));

            user.PasswordHash = Convert.ToBase64String(hash);
            user.Salt = salt;

            _data.Users.Add(user);
            _data.SaveChanges();

            return user;
        }
    }).Result;
}
