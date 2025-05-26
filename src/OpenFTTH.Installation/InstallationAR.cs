using OpenFTTH.EventSourcing;
using OpenFTTH.Installation.Events;
using OpenFTTH.Results;

namespace OpenFTTH.Installation;

public class InstallationAR : AggregateBase
{
    public string InstallationId { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Remark { get; private set; } = string.Empty;
    public string? LocationRemark { get; private set; }
    public Guid UnitAddressId { get; private set; }

    public Result Create(
        Guid id,
        string installationId,
        string status,
        string remark,
        string locationRemark,
        Guid unitAddressId)
    {
        if (IsInitialized(Id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.ALREADY_CREATED,
                    $"Cannot create, it has already been created: {nameof(Id)}: '{Id}'"));
        }

        if (IsIdValid(Id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.ID_INVALID,
                    $"Id is invalid: '{Id}'"));
        }

        RaiseEvent(
            new InstallationCreated(
                id: id,
                installationId: installationId,
                status: status,
                remark: remark,
                locationRemark: locationRemark,
                unitAddressId: unitAddressId));

        return Result.Ok();
    }

    private static bool IsInitialized(Guid id)
    {
        return id != Guid.Empty;
    }

    private static bool IsIdValid(Guid id)
    {
        return id != Guid.Empty;
    }
}
