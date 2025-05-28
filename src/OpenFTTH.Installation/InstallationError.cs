using OpenFTTH.Results;

namespace OpenFTTH.Installation;

public enum InstallationErrorCode
{
    ALREADY_CREATED,
    NOT_INITIALIZED,
    ID_INVALID,
    INSTALLATION_ID_INVALID,
    UNIT_ADDRESS_ID_INVALID,
    NO_CHANGES
}

public class InstallationError : Error
{
    public InstallationErrorCode Code { get; init; }

    public InstallationError(InstallationErrorCode errorCode, string errorMsg)
        : base(errorCode.ToString() + ": " + errorMsg)
    {
        Code = errorCode;
        Metadata.Add("ErrorCode", errorCode.ToString());
    }
}
