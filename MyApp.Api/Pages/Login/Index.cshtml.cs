using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Api.Data;

namespace MyApp.Api.Pages.Login;

public class IndexModel(SupabaseService db) : PageModel
{
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string username, string password)
    {
        var account = await db.LoginAsync(username, password);
        if (account == null)
        {
            ErrorMessage = "帳號或密碼錯誤";
            return Page();
        }
        HttpContext.Session.SetString("UserId", account.Id.ToString());
        HttpContext.Session.SetString("UserName", account.Name);
        HttpContext.Session.SetString("UserRole", account.Role);
        return RedirectToPage("/Appointments/Index");
    }
}
