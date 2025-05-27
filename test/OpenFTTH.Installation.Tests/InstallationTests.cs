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

    [Fact, Order(1)]
    public void Can_create_installation()
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
