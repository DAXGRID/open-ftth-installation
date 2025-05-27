namespace OpenFTTH.Installation.Events;

public sealed record InstallationStatusChanged
{
    public Guid Id { get; init; }
    public string? Status { get; init; }

    public InstallationStatusChanged(
        Guid id,
        string? status)
    {
        Id = id;
        Status = status;
    }
}
