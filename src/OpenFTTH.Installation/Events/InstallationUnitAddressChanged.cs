namespace OpenFTTH.Installation.Events;

public sealed record InstallationUnitAddressChanged
{
    public Guid Id { get; init; }
    public Guid? UnitAddressId { get; init; }

    public InstallationUnitAddressChanged(
        Guid id,
        Guid? unitAddressId)
    {
        Id = id;
        UnitAddressId = unitAddressId;
    }
}
