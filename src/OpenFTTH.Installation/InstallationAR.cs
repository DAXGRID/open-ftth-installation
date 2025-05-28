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
        Register<InstallationStatusChanged>(Apply);
        Register<InstallationRemarkChanged>(Apply);
        Register<InstallationLocationRemarkChanged>(Apply);
        Register<InstallationUnitAddressChanged>(Apply);
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

    public Result ChangeStatus(string? status)
    {
        if (!IsInitialized(Id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.NOT_INITIALIZED,
                    $"Cannot update something that has not been initialized."));
        }

        if (Status == status)
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.NO_CHANGES,
                    $"Cannot update {nameof(Status)}, no changes was detected between: '{Status}' and '{status}'."));
        }

        RaiseEvent(
           new InstallationStatusChanged(
               id: Id,
               status: status));

        return Result.Ok();
    }

    public Result ChangeRemark(string? remark)
    {
        if (!IsInitialized(Id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.NOT_INITIALIZED,
                    $"Cannot update something that has not been initialized."));
        }

        if (Remark == remark)
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.NO_CHANGES,
                    $"Cannot update {nameof(Remark)}, no changes was detected between: '{Remark}' and '{remark}'."));
        }

        RaiseEvent(
           new InstallationRemarkChanged(
               id: Id,
               remark: remark));

        return Result.Ok();
    }

    public Result ChangeLocationRemark(string? locationRemark)
    {
        if (!IsInitialized(Id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.NOT_INITIALIZED,
                    $"Cannot update something that has not been initialized."));
        }

        if (LocationRemark == locationRemark)
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.NO_CHANGES,
                    $"Cannot update {nameof(LocationRemark)}, no changes was detected between: '{LocationRemark}' and '{locationRemark}'."));
        }

        RaiseEvent(
           new InstallationLocationRemarkChanged(
               id: Id,
               locationRemark: locationRemark));

        return Result.Ok();
    }

    public Result ChangeUnitAddressId(Guid? unitAddressId)
    {
        if (!IsInitialized(Id))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.NOT_INITIALIZED,
                    $"Cannot update something that has not been initialized."));
        }

        if (!UnitAddressIdValid(unitAddressId))
        {
            return Result.Fail(
                new InstallationError(
                    InstallationErrorCode.UNIT_ADDRESS_ID_INVALID,
                    $"Unit address ID cannot be an empty guid: '{unitAddressId}'."));
        }

        RaiseEvent(
           new InstallationUnitAddressChanged(
               id: Id,
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

    private void Apply(InstallationStatusChanged installationStatusChanged)
    {
        Status = installationStatusChanged.Status;
    }

    private void Apply(InstallationRemarkChanged installationRemarkChanged)
    {
        Remark = installationRemarkChanged.Remark;
    }

    private void Apply(InstallationLocationRemarkChanged installationLocationRemarkChanged)
    {
        LocationRemark = installationLocationRemarkChanged.LocationRemark;
    }

    private void Apply(InstallationUnitAddressChanged installationUnitAddressChanged)
    {
        UnitAddressId = installationUnitAddressChanged.UnitAddressId;
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
