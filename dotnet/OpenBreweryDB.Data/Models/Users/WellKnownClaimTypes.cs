namespace OpenBreweryDB.Data.Models.Users
{
    public static class WellKnownClaimTypes
    {
        public static string UserId => $"{nameof(User)}.{nameof(User)}{nameof(User.Id)}";
    }
}
