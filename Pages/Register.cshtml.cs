using AzerothConnect.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Text.RegularExpressions;

namespace AzerothConnect.Pages;

public class RegisterModel : PageModel
{
    public string? Success { get; set; }
    public string? Error { get; set; }

    private readonly AccountService account;

    public RegisterModel(AccountService account)
    {
        this.account = account;
    }

    public async Task<IActionResult> OnPostAsync(string? username, string? email, string? password, string? passwordVerify)
    {
        if(string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(passwordVerify))
        {
            this.Error = "You must populate all fields.";
            return Page();
        }

        if (username.Length < 1 ||
            username.Length > 20)
        {
            this.Error = "Username must be between 1 and 20 characters.";
            return Page();
        }

        if (!Regex.IsMatch(username, "^[0-9a-zA-Z]{1,20}$"))
        {
            this.Error = "Username should contain only alphanumeric characters.";
            return Page();
        }

        if (!Regex.IsMatch(email, "^.+?@.+?\\..+$"))
        {
            this.Error = "Email format is invalid.";
            return Page();
        }

        if (!Regex.IsMatch(password, "^[\\S]{1,16}$"))
        {
            this.Error = "Password cannot contain spaces.";
            return Page();
        }

        if (password.Length < 1 ||
            password.Length > 16)
        {
            this.Error = "Password must be between 1 and 16 characters.";
            return Page();
        }

        if (password != passwordVerify)
        {
            this.Error = "Passwords do not match.";
            return Page();
        }

        if (account.IsUsernameTaken(username))
        {
            this.Error = "Username already taken.";
            return Page();
        }

        bool result = await account.CreateAccountAsync(username, password, email);
        if(!result)
        {
            this.Error = "An internal error occured.";
            return Page();
        }

        this.Success = "Account created.";
        return Page();
    }
}
