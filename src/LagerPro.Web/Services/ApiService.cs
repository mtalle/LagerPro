using System.Net.Http.Json;
using LagerPro.Web.Models;
using LagerPro.Contracts.Requests.Kunder;
using LagerPro.Contracts.Requests.Leverandorer;
using LagerPro.Contracts.Requests.Resepter;

namespace LagerPro.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl = "http://localhost:5000";

    public ApiService(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri(_baseUrl);
    }

    // Articles
    public async Task<List<Article>> GetArticlesAsync()
    {
        var response = await _http.GetAsync("/api/articles");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Article>>() ?? new();
    }

    public async Task<Article?> GetArticleByIdAsync(int id)
    {
        var response = await _http.GetAsync($"/api/articles/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Article>();
    }

    public async Task<int> CreateArticleAsync(CreateArticleRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/articles", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<bool> UpdateArticleAsync(int id, UpdateArticleRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/articles/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/articles/{id}");
        return response.IsSuccessStatusCode;
    }

    // Inventory
    public async Task<List<LagerBeholdning>> GetLagerBeholdningAsync()
    {
        var response = await _http.GetAsync("/api/lager");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<LagerBeholdning>>() ?? new();
    }

    // Mottak
    public async Task<List<Mottak>> GetMottakListAsync()
    {
        var response = await _http.GetAsync("/api/mottak");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Mottak>>() ?? new();
    }

    public async Task<Mottak?> GetMottakByIdAsync(int id)
    {
        var response = await _http.GetAsync($"/api/mottak/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Mottak>();
    }

    public async Task<int> CreateMottakAsync(CreateMottakRequest request)
    {
        var payload = new
        {
            leverandorId = request.LeverandorId,
            mottaksDato = request.MottaksDato,
            referanse = request.Referanse,
            kommentar = request.Kommentar,
            mottattAv = request.MottattAv,
            linjer = request.Linjer.Select(l => new
            {
                artikkelId = l.ArtikkelId,
                lotNr = l.LotNr,
                mengde = l.Mengde,
                enhet = l.Enhet,
                bestForDato = l.BestForDato,
                temperatur = l.Temperatur,
                strekkode = l.Strekkode,
                avvik = l.Avvik,
                kommentar = l.Kommentar
            }).ToList()
        };
        var response = await _http.PostAsJsonAsync("/api/mottak", payload);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<bool> UpdateMottakStatusAsync(int id, string status)
    {
        var response = await _http.PutAsJsonAsync($"/api/mottak/{id}/status", new { status });
        return response.IsSuccessStatusCode;
    }

    // Produksjon
    public async Task<List<ProduksjonsOrdre>> GetProduksjonsOrdreAsync()
    {
        var response = await _http.GetAsync("/api/produksjon");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProduksjonsOrdre>>() ?? new();
    }

    public async Task<ProduksjonsOrdreDetaljer?> GetProduksjonsOrdreDetaljerAsync(int id)
    {
        var response = await _http.GetAsync($"/api/produksjon/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ProduksjonsOrdreDetaljer>();
    }

    public async Task<int> CreateProduksjonsOrdreAsync(CreateProduksjonsOrdreRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/produksjon", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<bool> UpdateProduksjonsOrdreStatusAsync(int id, string status)
    {
        var response = await _http.PutAsJsonAsync($"/api/produksjon/{id}/status", new { status });
        return response.IsSuccessStatusCode;
    }

    // Levering
    public async Task<List<Levering>> GetLeveringListAsync()
    {
        var response = await _http.GetAsync("/api/levering");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Levering>>() ?? new();
    }

    public async Task<Levering?> GetLeveringByIdAsync(int id)
    {
        var response = await _http.GetAsync($"/api/levering/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Levering>();
    }

    public async Task<int> CreateLeveringAsync(CreateLeveringRequest request)
    {
        var payload = new
        {
            kundeId = request.KundeId,
            leveringsDato = request.LeveringsDato,
            referanse = request.Referanse,
            fraktBrev = request.FraktBrev,
            kommentar = request.Kommentar,
            linjer = request.Linjer.Select(l => new
            {
                artikkelId = l.ArtikkelId,
                lotNr = l.LotNr,
                mengde = l.Mengde,
                enhet = l.Enhet
            }).ToList()
        };
        var response = await _http.PostAsJsonAsync("/api/levering", payload);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<bool> UpdateLeveringStatusAsync(int id, string status)
    {
        var response = await _http.PutAsJsonAsync($"/api/levering/{id}/status", new { status });
        return response.IsSuccessStatusCode;
    }

    // Kunder
    public async Task<List<Kunde>> GetKunderAsync()
    {
        var response = await _http.GetAsync("/api/kunder");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Kunde>>() ?? new();
    }

    // Kunder CRUD
    public async Task<int> CreateKundeAsync(CreateKundeRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/kunder", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<bool> UpdateKundeAsync(int id, UpdateKundeRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/kunder/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteKundeAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/kunder/{id}");
        return response.IsSuccessStatusCode;
    }

    // Leverandører
    public async Task<List<Leverandor>> GetLeverandørerAsync()
    {
        var response = await _http.GetAsync("/api/leverandorer");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Leverandor>>() ?? new();
    }

    // Leverandører CRUD
    public async Task<int> CreateLeverandorAsync(CreateLeverandorRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/leverandorer", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<bool> UpdateLeverandorAsync(int id, UpdateLeverandorRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/leverandorer/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteLeverandorAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/leverandorer/{id}");
        return response.IsSuccessStatusCode;
    }

    // Resepter
    public async Task<List<Resept>> GetResepterAsync()
    {
        var response = await _http.GetAsync("/api/resepter");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Resept>>() ?? new();
    }

    // Resepter CRUD
    public async Task<int> CreateReseptAsync(CreateReseptRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/resepter", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<bool> UpdateReseptAsync(int id, UpdateReseptRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/resepter/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteReseptAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/resepter/{id}");
        return response.IsSuccessStatusCode;
    }
}
