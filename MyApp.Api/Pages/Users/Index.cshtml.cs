using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Api.Data;
using MyApp.Api.Models;

namespace MyApp.Api.Pages.Users;

public class IndexModel(SupabaseService db) : PageModel
{
    public List<User> Users { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            return RedirectToPage("/Login/Index");
        Users = await db.GetUsersAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(
        string Name, string? Phone, string? Address, string? LineQrcode)
    {
        await db.CreateUserAsync(new User
        {
            Name = Name,
            Phone = Phone,
            Address = Address,
            LineQrcode = LineQrcode,
        });
        TempData["Success"] = "使用者新增成功！";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(
        long Id, string Name, string? Phone, string? Address, string? LineQrcode)
    {
        await db.UpdateUserAsync(new User
        {
            Id = Id,
            Name = Name,
            Phone = Phone,
            Address = Address,
            LineQrcode = LineQrcode,
        });
        TempData["Success"] = "使用者資料更新成功！";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        await db.DeleteUserAsync(id);
        TempData["Success"] = "使用者刪除成功！";
        return RedirectToPage();
    }
}
