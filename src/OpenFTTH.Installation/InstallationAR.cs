using OpenFTTH.EventSourcing;
using OpenFTTH.Installation.Events;
using OpenFTTH.Results;

namespace OpenFTTH.Installation;

public class InstallationAR : AggregateBase
{
    public string InstallationId { get; private set; } = string.Empty;
    public string? Status { get; private set; }
    public string? Remark { get; private set; }
    public string? LocationRemark { get; private set; }
    public Guid? UnitAddressId { get; private set; }

    public InstallationAR()
    {
        Register<InstallationCreated>(Apply);
    }

    public Result Create(
        Guid id,
        string installationId,
        string? status,
        string? remark,
        string? locationRemark,
        Guid? unitAddressId)
    {
        if (IsInitialized(Id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.ALREADY_CREATED,
                    $"Cannot create, it has already been created: {nameof(Id)}: '{id}'."));
        }

        if (!IsIdValid(id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.ID_INVALID,
                    $"Id is invalid: '{id}'."));
        }

        if (!InstallationIdValid(installationId))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.INSTALLATION_ID_INVALID,
                    $"Installation ID cannot be NULL or just white space: '{installationId}'."));
        }

        if (!InstallationIdValid(installationId))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.INSTALLATION_ID_INVALID,
                    $"Installation ID cannot be NULL or just white space: '{installationId}'."));
        }

        if (!UnitAddressIdValid(unitAddressId))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.UNIT_ADDRESS_ID_INVALID,
                    $"Unit address ID cannot be an empty guid: '{unitAddressId}'."));
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

    private void Apply(InstallationCreated installationCreated)
    {
        Id = installationCreated.Id;
        InstallationId = installationCreated.InstallationId;
        Status = installationCreated.Status;
        Remark = installationCreated.Remark;
        LocationRemark = installationCreated.LocationRemark;
        UnitAddressId = installationCreated.UnitAddressId;
    }

    private static bool UnitAddressIdValid(Guid? unitAddressId)
    {
        return unitAddressId != Guid.Empty;
    }

    private static bool InstallationIdValid(string id)
    {
        return !string.IsNullOrWhiteSpace(id);
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
