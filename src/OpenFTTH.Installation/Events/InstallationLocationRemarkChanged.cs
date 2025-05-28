namespace OpenFTTH.Installation.Events;

public sealed record InstallationLocationRemarkChanged
{
    public Guid Id { get; init; }
    public string? LocationRemark { get; init; }

    public InstallationLocationRemarkChanged(
        Guid id,
        string? locationRemark)
    {
        Id = id;
        LocationRemark = locationRemark;
    }
}
