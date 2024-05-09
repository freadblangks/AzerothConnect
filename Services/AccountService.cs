using AzerothConnect.Crypto;
using AzerothConnect.Database.Auth;
using AzerothConnect.Database.Contexts;
using AzerothConnect.Models.Account;

namespace AzerothConnect.Services;

public class AccountService
{
    private readonly AuthDbContext authContext;

    public AccountService(AuthDbContext authContext)
    {
        this.authContext = authContext;
    }

    public bool IsUsernameTaken(string username)
    {
        return authContext.Account.FirstOrDefault(a => a.Username!.ToLower() == username.ToLower()) is not null;
    }

    public async Task<bool> CreateAccountAsync(string username, string password, string email)
    {
        var salt = SRP6.GenerateSalt();
        var verifier = SRP6.GenerateVerifier(salt, username, password);

        var account = new Account()
        {
            Username = username,
            Salt = salt,
            Verifier = verifier,
            Email = email,
            RegisterEmail = email,
            Expansion = Expansion.WrathOfTheLichKing,
            JoinDate = DateTimeOffset.Now
        };

        await authContext.Account.AddAsync(account);
        var rows = await authContext.SaveChangesAsync();

        if (rows < 1)
        {
            return false;
        }

        var accountAccess = new AccountAccess()
        {
            Id = account.Id,
            Role = GMLevel.Player,
            RealmID = -1
        };

        await authContext.AccountAccess.AddAsync(accountAccess);
        rows = await authContext.SaveChangesAsync();

        if (rows < 1)
        {
            return false;
        }

        return true;
    }
}
