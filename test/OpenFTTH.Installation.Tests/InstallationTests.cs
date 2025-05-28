using OpenFTTH.EventSourcing;
using Xunit.Extensions.Ordering;

namespace OpenFTTH.Installation.Tests;

[Order(0)]
public class InstallationTests
{
    private readonly IEventStore _eventStore;

    public InstallationTests(IEventStore eventStore)
    {
        _eventStore = eventStore;
        _eventStore.ScanForProjections();
    }

    [Fact, Order(1)]
    public void Create_empty_id_is_invalid()
    {
        var id = Guid.Empty;
        var installationId = "F12345";
        var status = "Opened";
        var remark = "Remark";
        var locationRemark = "Location remark";
        var unitAddressId = Guid.NewGuid();

        var installation = new InstallationAR();

        var createInstallationResult = installation.Create(
            id,
            installationId,
            status,
            remark,
            locationRemark,
            unitAddressId);

        Assert.False(createInstallationResult.IsSuccess);
        Assert.True(createInstallationResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)createInstallationResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.ID_INVALID);
    }

    [Theory, Order(1)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    public void Installation_id_invalid_data(string installationId)
    {
        var id = Guid.NewGuid();
        var status = "Opened";
        var remark = "Remark";
        var locationRemark = "Location remark";
        var unitAddressId = Guid.NewGuid();

        var installation = new InstallationAR();

        var createInstallationResult = installation.Create(
            id,
            installationId,
            status,
            remark,
            locationRemark,
            unitAddressId);

        _eventStore.Aggregates.Store(installation);

        Assert.True(createInstallationResult.IsFailed);
        Assert.True(createInstallationResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)createInstallationResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.INSTALLATION_ID_INVALID);
    }

    [Theory, Order(1)]
    [InlineData("1")]
    [InlineData("1234")]
    [InlineData("AB12345")]
    [InlineData("531318c3-098f-4e12-8d5c-9c06a625f8a6")]
    [InlineData("e0f57b7e-4b03-4449-bd23-faee1f7d8c1c")]
    public void Installation_id_valid_data(string installationId)
    {
        var id = Guid.NewGuid();
        var status = "Opened";
        var remark = "Remark";
        var locationRemark = "Location remark";
        var unitAddressId = Guid.NewGuid();

        var installation = new InstallationAR();

        var createInstallationResult = installation.Create(
            id,
            installationId,
            status,
            remark,
            locationRemark,
            unitAddressId);

        Assert.True(createInstallationResult.IsSuccess);
        Assert.True(createInstallationResult.Errors.Count() == 0);
    }

    [Theory, Order(1)]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void Unit_address_id_invalid_data(string unitAddressId)
    {
        var id = Guid.NewGuid();
        var status = "Opened";
        var remark = "Remark";
        var locationRemark = "Location remark";
        var installationId = "AB12345";

        var installation = new InstallationAR();

        var createInstallationResult = installation.Create(
            id,
            installationId,
            status,
            remark,
            locationRemark,
            Guid.Parse(unitAddressId));

        Assert.True(createInstallationResult.IsFailed);
        Assert.True(createInstallationResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)createInstallationResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.UNIT_ADDRESS_ID_INVALID);
    }

    [Fact, Order(2)]
    public void Can_create_installation_stored()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var installationId = "F12345";
        var status = "Opened";
        var remark = "Remark";
        var locationRemark = "Location remark";
        var unitAddressId = Guid.Parse("772ca9c1-6ac6-478a-8797-633a3cd012ea");

        var installation = new InstallationAR();

        var createInstallationResult = installation.Create(
            id,
            installationId,
            status,
            remark,
            locationRemark,
            unitAddressId);


        _eventStore.Aggregates.Store(installation);

        Assert.True(createInstallationResult.IsSuccess);
        Assert.True(createInstallationResult.Errors.Count() == 0);
        Assert.True(installation.Id == id);
        Assert.True(installation.InstallationId == installationId);
        Assert.True(installation.Status == status);
        Assert.True(installation.Remark == remark);
        Assert.True(installation.LocationRemark == locationRemark);
        Assert.True(installation.UnitAddressId == unitAddressId);
    }

    [Fact, Order(3)]
    public void Can_change_installation_status()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var installationId = "F12345";
        var status = "Changed";
        var remark = "Remark";
        var locationRemark = "Location remark";
        var unitAddressId = Guid.Parse("772ca9c1-6ac6-478a-8797-633a3cd012ea");

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        var changeStatusResult = installation.ChangeStatus(status);

        _eventStore.Aggregates.Store(installation);

        installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        Assert.True(changeStatusResult.IsSuccess);
        Assert.True(changeStatusResult.Errors.Count() == 0);
        Assert.True(installation.Id == id);
        Assert.True(installation.InstallationId == installationId);
        Assert.True(installation.Status == status);
        Assert.True(installation.Remark == remark);
        Assert.True(installation.LocationRemark == locationRemark);
        Assert.True(installation.UnitAddressId == unitAddressId);
    }

    [Fact, Order(3)]
    public void Cannot_change_installation_status_when_not_initialized()
    {
        var status = "Changed";

        var installation = new InstallationAR();

        var changeStatusResult = installation.ChangeStatus(status);

        Assert.True(changeStatusResult.IsFailed);
        Assert.True(changeStatusResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeStatusResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.NOT_INITIALIZED);
    }

    [Fact, Order(3)]
    public void Cannot_change_installation_remark_when_not_initialized()
    {
        var locationRemark = "Location remark";

        var installation = new InstallationAR();

        var changeRemarkResult = installation.ChangeRemark(locationRemark);

        Assert.True(changeRemarkResult.IsFailed);
        Assert.True(changeRemarkResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeRemarkResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.NOT_INITIALIZED);
    }

    [Fact, Order(3)]
    public void Can_change_installation_remark()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var installationId = "F12345";
        var status = "Changed";
        var remark = "Updated Remark";
        var locationRemark = "Location remark";
        var unitAddressId = Guid.Parse("772ca9c1-6ac6-478a-8797-633a3cd012ea");

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        var changeRemarkResult = installation.ChangeRemark(remark);

        _eventStore.Aggregates.Store(installation);

        installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        Assert.True(changeRemarkResult.IsSuccess);
        Assert.True(changeRemarkResult.Errors.Count() == 0);
        Assert.True(installation.Id == id);
        Assert.True(installation.InstallationId == installationId);
        Assert.True(installation.Status == status);
        Assert.True(installation.Remark == remark);
        Assert.True(installation.LocationRemark == locationRemark);
        Assert.True(installation.UnitAddressId == unitAddressId);
    }

    [Fact, Order(3)]
    public void Cannot_change_installation_location_remark_when_not_initialized()
    {
        var locationRemark = "Location remark";

        var installation = new InstallationAR();

        var changeRemarkResult = installation.ChangeLocationRemark(locationRemark);

        Assert.True(changeRemarkResult.IsFailed);
        Assert.True(changeRemarkResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeRemarkResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.NOT_INITIALIZED);
    }

    [Fact, Order(3)]
    public void Can_change_installation_location_remark()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var installationId = "F12345";
        var status = "Changed";
        var remark = "Updated Remark";
        var locationRemark = "Updated location remark";
        var unitAddressId = Guid.Parse("772ca9c1-6ac6-478a-8797-633a3cd012ea");

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        var changeLocationRemarkResult = installation.ChangeLocationRemark(locationRemark);

        _eventStore.Aggregates.Store(installation);

        installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        Assert.True(changeLocationRemarkResult.IsSuccess);
        Assert.True(changeLocationRemarkResult.Errors.Count() == 0);
        Assert.True(installation.Id == id);
        Assert.True(installation.InstallationId == installationId);
        Assert.True(installation.Status == status);
        Assert.True(installation.Remark == remark);
        Assert.True(installation.LocationRemark == locationRemark);
        Assert.True(installation.UnitAddressId == unitAddressId);
    }

    [Fact, Order(3)]
    public void Cannot_change_installation_unit_address_id_when_not_initialized()
    {
        var unitAddressId = Guid.Parse("c273460f-e3c1-4ab5-939a-91952ffafa0e");

        var installation = new InstallationAR();

        var changeUnitAddressId = installation.ChangeUnitAddressId(unitAddressId);

        Assert.True(changeUnitAddressId.IsFailed);
        Assert.True(changeUnitAddressId.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeUnitAddressId
             .Errors
             .First()
            ).Code == InstallationErrorCode.NOT_INITIALIZED);
    }

    [Fact, Order(3)]
    public void Cannot_change_installation_unit_address_id_to_empty_guid()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var unitAddressId = Guid.Empty;

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        var changeUnitAddressId = installation.ChangeUnitAddressId(unitAddressId);

        Assert.True(changeUnitAddressId.IsFailed);
        Assert.True(changeUnitAddressId.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeUnitAddressId
             .Errors
             .First()
            ).Code == InstallationErrorCode.UNIT_ADDRESS_ID_INVALID);
    }

    [Fact, Order(3)]
    public void Can_change_unit_address_id()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var installationId = "F12345";
        var status = "Changed";
        var remark = "Updated Remark";
        var locationRemark = "Updated location remark";
        var unitAddressId = Guid.Parse("772ca9c1-6ac6-478a-8797-633a3cd012ea");

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        var unitAddressChangedResult = installation.ChangeUnitAddressId(unitAddressId);

        _eventStore.Aggregates.Store(installation);

        installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        Assert.True(unitAddressChangedResult.IsSuccess);
        Assert.True(unitAddressChangedResult.Errors.Count() == 0);
        Assert.True(installation.Id == id);
        Assert.True(installation.InstallationId == installationId);
        Assert.True(installation.Status == status);
        Assert.True(installation.Remark == remark);
        Assert.True(installation.LocationRemark == locationRemark);
        Assert.True(installation.UnitAddressId == unitAddressId);
    }

    [Fact, Order(4)]
    public void Cannot_change_installation_status_to_the_same_value()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var status = "Changed";

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);
        var changeStatusResult = installation.ChangeStatus(status);

        installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        Assert.True(changeStatusResult.IsFailed);
        Assert.True(changeStatusResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeStatusResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.NO_CHANGES);
    }

    [Fact, Order(4)]
    public void Cannot_change_installation_remark_to_the_same_value()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var remark = "Updated Remark";

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);
        var changeRemarkResult = installation.ChangeRemark(remark);

        installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        Assert.True(changeRemarkResult.IsFailed);
        Assert.True(changeRemarkResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeRemarkResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.NO_CHANGES);
    }

    [Fact, Order(4)]
    public void Cannot_change_installation_location_remark_to_the_same_value()
    {
        var id = Guid.Parse("75b98e4b-9b82-4a1a-99a5-097b1c65d1ad");
        var locationRemark = "Updated location remark";

        var installation = _eventStore.Aggregates.Load<InstallationAR>(id);

        var changeLocationRemarkResult = installation.ChangeLocationRemark(locationRemark);

        Assert.True(changeLocationRemarkResult.IsFailed);
        Assert.True(changeLocationRemarkResult.Errors.Count() == 1);
        Assert.True(
            ((InstallationError)changeLocationRemarkResult
             .Errors
             .First()
            ).Code == InstallationErrorCode.NO_CHANGES);
    }
}
