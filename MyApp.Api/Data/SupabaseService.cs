using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyApp.Api.Models;

namespace MyApp.Api.Data;

public class SupabaseService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public SupabaseService(IConfiguration config)
    {
        var url = config["Supabase:Url"]!;
        var key = config["Supabase:ServiceRoleKey"]!;
        _http = new HttpClient { BaseAddress = new Uri(url) };
        _http.DefaultRequestHeaders.Add("apikey", key);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var res = await _http.GetStringAsync("/rest/v1/Users?order=name");
        return JsonSerializer.Deserialize<List<User>>(res, _json) ?? [];
    }

    public async Task<List<Appointment>> GetAppointmentsAsync(string? name, string? from, string? to)
    {
        var query = "/rest/v1/Appointments?order=appointment_time.desc";
        if (!string.IsNullOrWhiteSpace(name))
            query += $"&appointee_name=ilike.*{Uri.EscapeDataString(name)}*";
        if (!string.IsNullOrWhiteSpace(from))
            query += $"&appointment_time=gte.{Uri.EscapeDataString(from)}";
        if (!string.IsNullOrWhiteSpace(to))
        {
            var toDate = DateTime.Parse(to).AddDays(1).ToString("yyyy-MM-dd");
            query += $"&appointment_time=lt.{Uri.EscapeDataString(toDate)}";
        }
        var res = await _http.GetStringAsync(query);
        return JsonSerializer.Deserialize<List<Appointment>>(res, _json) ?? [];
    }

    public async Task CreateAppointmentAsync(Appointment a)
    {
        var body = JsonSerializer.Serialize(new
        {
            user_id = a.UserId,
            appointee_name = a.AppointeeName,
            appointment_time = a.AppointmentTime.UtcDateTime.ToString("o"),
            location = a.Location,
            created_at = DateTimeOffset.UtcNow.ToString("o"),
        });
        var req = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/Appointments")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Prefer", "return=minimal");
        await _http.SendAsync(req);
    }

    public async Task UpdateAppointmentAsync(Appointment a)
    {
        var body = JsonSerializer.Serialize(new
        {
            user_id = a.UserId,
            appointee_name = a.AppointeeName,
            appointment_time = a.AppointmentTime.UtcDateTime.ToString("o"),
            location = a.Location,
        });
        var req = new HttpRequestMessage(HttpMethod.Patch, $"/rest/v1/Appointments?id=eq.{a.Id}")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Prefer", "return=minimal");
        await _http.SendAsync(req);
    }

    public async Task DeleteAppointmentAsync(long id)
    {
        await _http.DeleteAsync($"/rest/v1/Appointments?id=eq.{id}");
    }

    public async Task CreateUserAsync(User u)
    {
        var body = JsonSerializer.Serialize(new
        {
            name = u.Name,
            phone = u.Phone,
            address = u.Address,
            line_qrcode = u.LineQrcode,
            created_at = DateTimeOffset.UtcNow.ToString("o"),
        });
        var req = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/Users")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Prefer", "return=minimal");
        await _http.SendAsync(req);
    }

    public async Task UpdateUserAsync(User u)
    {
        var body = JsonSerializer.Serialize(new
        {
            name = u.Name,
            phone = u.Phone,
            address = u.Address,
            line_qrcode = u.LineQrcode,
        });
        var req = new HttpRequestMessage(HttpMethod.Patch, $"/rest/v1/Users?id=eq.{u.Id}")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Prefer", "return=minimal");
        await _http.SendAsync(req);
    }

    public async Task DeleteUserAsync(long id)
    {
        await _http.DeleteAsync($"/rest/v1/Users?id=eq.{id}");
    }

    public async Task<UserAccount?> LoginAsync(string username, string password)
    {
        var res = await _http.GetStringAsync(
            $"/rest/v1/UserAccounts?username=eq.{Uri.EscapeDataString(username)}&password=eq.{Uri.EscapeDataString(password)}&limit=1");
        var list = JsonSerializer.Deserialize<List<UserAccount>>(res, _json);
        return list?.FirstOrDefault();
    }

    public async Task<List<UserAccount>> GetUserAccountsAsync()
    {
        var res = await _http.GetStringAsync("/rest/v1/UserAccounts?order=created_at.desc");
        return JsonSerializer.Deserialize<List<UserAccount>>(res, _json) ?? [];
    }

    public async Task CreateUserAccountAsync(UserAccount a)
    {
        var body = JsonSerializer.Serialize(new
        {
            name = a.Name,
            username = a.Username,
            password = a.Password,
            role = a.Role,
            created_at = DateTimeOffset.UtcNow.ToString("o"),
        });
        var req = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/UserAccounts")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Prefer", "return=minimal");
        await _http.SendAsync(req);
    }

    public async Task UpdateUserAccountAsync(UserAccount a)
    {
        var bodyObj = string.IsNullOrWhiteSpace(a.Password)
            ? (object)new { name = a.Name, username = a.Username, role = a.Role }
            : new { name = a.Name, username = a.Username, password = a.Password, role = a.Role };
        var body = JsonSerializer.Serialize(bodyObj);
        var req = new HttpRequestMessage(HttpMethod.Patch, $"/rest/v1/UserAccounts?id=eq.{a.Id}")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Prefer", "return=minimal");
        await _http.SendAsync(req);
    }

    public async Task DeleteUserAccountAsync(long id)
    {
        await _http.DeleteAsync($"/rest/v1/UserAccounts?id=eq.{id}");
    }
}
