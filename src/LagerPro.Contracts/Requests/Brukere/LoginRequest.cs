namespace LagerPro.Contracts.Requests.Brukere;

public class LoginRequest
{
    public string Brukernavn { get; set; } = string.Empty;
    public string Passord { get; set; } = string.Empty;
}
