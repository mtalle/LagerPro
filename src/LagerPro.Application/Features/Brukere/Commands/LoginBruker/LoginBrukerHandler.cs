using LagerPro.Contracts.Dtos.Brukere;
using LagerPro.Contracts.Requests.Brukere;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Brukere.Commands.LoginBruker;

public class LoginBrukerHandler
{
    private readonly IBrukerRepository _brukerRepository;

    public LoginBrukerHandler(IBrukerRepository brukerRepository)
    {
        _brukerRepository = brukerRepository;
    }

    public async Task<BrukerDto?> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var bruker = await _brukerRepository.GetByBrukernavnAsync(request.Brukernavn, cancellationToken);
        if (bruker == null || bruker.Passord != request.Passord)
        {
            return null;
        }

        return new BrukerDto(
            bruker.Id,
            bruker.Navn,
            bruker.Brukernavn,
            bruker.Epost,
            bruker.ErAdmin,
            bruker.Aktiv,
            bruker.Tilganger.Select(t => new BrukerRessursDto(t.RessursId, t.Ressurs.Navn)).ToList()
        );
    }
}
