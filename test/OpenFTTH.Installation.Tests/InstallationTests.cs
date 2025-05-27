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

    [Fact, Order(2)]
    public void Can_create_installation_stored()
    {
        var id = Guid.NewGuid();
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
}
