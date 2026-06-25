using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Api.Data;
using MyApp.Api.Models;

namespace MyApp.Api.Pages.Users;

public class IndexModel(SupabaseService db) : PageModel
{
    public List<User> Users { get; set; } = [];

    public async Task OnGetAsync()
    {
        Users = await db.GetUsersAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync(string Name)
    {
        await db.CreateUserAsync(Name);
        TempData["Success"] = "使用者新增成功！";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        await db.DeleteUserAsync(id);
        TempData["Success"] = "使用者刪除成功！";
        return RedirectToPage();
    }
}
