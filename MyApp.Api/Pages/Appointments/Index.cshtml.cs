using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Api.Data;
using MyApp.Api.Models;

namespace MyApp.Api.Pages.Appointments;

public class IndexModel(SupabaseService db) : PageModel
{
    public List<Appointment> Appointments { get; set; } = [];
    public List<User> Users { get; set; } = [];
    public string? SearchName { get; set; }
    public string? SearchDateFrom { get; set; }
    public string? SearchDateTo { get; set; }

    public async Task OnGetAsync(string? searchName, string? searchDateFrom, string? searchDateTo)
    {
        SearchName = searchName;
        SearchDateFrom = searchDateFrom;
        SearchDateTo = searchDateTo;
        Users = await db.GetUsersAsync();
        Appointments = await db.GetAppointmentsAsync(searchName, searchDateFrom, searchDateTo);
    }

    public async Task<IActionResult> OnPostCreateAsync(long UserId, string AppointeeName, DateTimeOffset AppointmentTime, string Location)
    {
        await db.CreateAppointmentAsync(new Appointment
        {
            UserId = UserId,
            AppointeeName = AppointeeName,
            AppointmentTime = AppointmentTime,
            Location = Location,
        });
        TempData["Success"] = "預約新增成功！";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(long Id, long UserId, string AppointeeName, DateTimeOffset AppointmentTime, string Location)
    {
        await db.UpdateAppointmentAsync(new Appointment
        {
            Id = Id,
            UserId = UserId,
            AppointeeName = AppointeeName,
            AppointmentTime = AppointmentTime,
            Location = Location,
        });
        TempData["Success"] = "預約更新成功！";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        await db.DeleteAppointmentAsync(id);
        TempData["Success"] = "預約刪除成功！";
        return RedirectToPage();
    }
}
