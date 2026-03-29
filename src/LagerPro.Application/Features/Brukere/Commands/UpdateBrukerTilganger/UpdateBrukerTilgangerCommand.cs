namespace LagerPro.Application.Features.Brukere.Commands.UpdateBrukerTilganger;

public record UpdateBrukerTilgangerCommand(int BrukerId, List<int> RessursIder);
