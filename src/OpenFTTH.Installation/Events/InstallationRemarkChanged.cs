namespace OpenFTTH.Installation.Events;

public sealed record InstallationRemarkChanged
{
    public Guid Id { get; init; }
    public string? Remark { get; init; }

    public InstallationRemarkChanged(
        Guid id,
        string? remark)
    {
        Id = id;
        Remark = remark;
    }
}
