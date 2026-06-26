using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Api.Data;
using MyApp.Api.Models;

namespace MyApp.Api.Pages.AccountManagement;

public class IndexModel(SupabaseService db) : PageModel
{
    public List<UserAccount> Accounts { get; set; } = [];
    public string? SearchName { get; set; }

    public async Task<IActionResult> OnGetAsync(string? searchName)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            return RedirectToPage("/Login/Index");
        if (HttpContext.Session.GetString("UserRole") != "admin")
            return RedirectToPage("/Appointments/Index");

        SearchName = searchName;
        var all = await db.GetUserAccountsAsync();
        Accounts = string.IsNullOrWhiteSpace(searchName)
            ? all
            : all.Where(a => a.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)
                          || a.Username.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(string Name, string Username, string Password, string Role)
    {
        if (HttpContext.Session.GetString("UserRole") != "admin") return Forbid();
        await db.CreateUserAccountAsync(new UserAccount { Name = Name, Username = Username, Password = Password, Role = Role });
        TempData["Success"] = "帳號新增成功！";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(long Id, string Name, string Username, string Password, string Role)
    {
        if (HttpContext.Session.GetString("UserRole") != "admin") return Forbid();
        await db.UpdateUserAccountAsync(new UserAccount { Id = Id, Name = Name, Username = Username, Password = Password, Role = Role });
        TempData["Success"] = "帳號更新成功！";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        if (HttpContext.Session.GetString("UserRole") != "admin") return Forbid();
        await db.DeleteUserAccountAsync(id);
        TempData["Success"] = "帳號刪除成功！";
        return RedirectToPage();
    }
}
