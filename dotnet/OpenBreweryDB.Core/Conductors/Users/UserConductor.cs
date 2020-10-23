using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Enumerations;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Core.Conductors.Users.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models.Users;

public class UserConductor : IUserConductor
{
    private const string INVALID_CREDENTIALS = "The specified username or password are invalid.";

    readonly BreweryDbContext _data;

    public UserConductor(BreweryDbContext data)
    {
        _data = data;
    }

    public IResult<User> Create(User user, string password) => Do<User>.Try((r) =>
    {
        var hashedPasswordResult = GetHashedPassword(password);

        if (hashedPasswordResult.HasErrorsOrResultIsNull())
        {
            r.AddErrors(hashedPasswordResult);
            return null;
        }

        var (salt, hashedPassword) = hashedPasswordResult.ResultObject;

        user.PasswordHash = hashedPassword;
        user.Salt = salt;

        _data.Users.Add(user);
        _data.SaveChanges();

        return user;
    }).Result;

    public IResult<User> FindByEmail(string email) => Do<User>.Try((r) =>
        _data.Users.FirstOrDefault(u => u.Email == email)).Result;

    public IResult<User> FindByEmailAndPassword(string email, string password) => Do<User>.Try((r) =>
    {
        var userResult = FindByEmail(email);

        if (userResult.HasErrorsOrResultIsNull())
        {
            r.AddErrors(userResult);
            return null;
        }

        var user = userResult.ResultObject;

        var hashedPasswordResult = GetHashedPassword(password, user.Salt);

        if (hashedPasswordResult.HasErrorsOrResultIsNull())
        {
            r.AddErrors(hashedPasswordResult);
            return null;
        }

        var (_, hashedPassword) = hashedPasswordResult.ResultObject;

        if (hashedPassword != user.PasswordHash)
        {
            r.AddError(ErrorType.Error, nameof(INVALID_CREDENTIALS), INVALID_CREDENTIALS);
            return null;
        }

        return user;
    }).Result;

    private IResult<(string salt, string hashedPassword)> GetHashedPassword(string password, string salt = null) =>
        Do<(string, string)>.Try((r) =>
        {
            salt ??= Guid.NewGuid().ToString("N");

            using (var sha = SHA512.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt));

                return (salt, Convert.ToBase64String(hash));
            }
        }).Result;

}
