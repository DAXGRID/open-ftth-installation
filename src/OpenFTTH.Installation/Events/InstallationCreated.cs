namespace OpenFTTH.Installation.Events;

public sealed record InstallationCreated
{
    public Guid Id { get; init; }
    public string InstallationId { get; init; }
    public string Status { get; init; }
    public string? Remark { get; init; }
    public string? LocationRemark { get; init; }
    public Guid UnitAddressId { get; init; }

    public InstallationCreated(
        Guid id,
        string installationId,
        string status,
        string? remark,
        string? locationRemark,
        Guid unitAddressId)
    {
        Id = id;
        InstallationId = installationId;
        Status = status;
        Remark = remark;
        LocationRemark = locationRemark;
        UnitAddressId = unitAddressId;
    }
}
