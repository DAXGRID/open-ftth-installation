using OpenFTTH.Results;

namespace OpenFTTH.Installation;

public enum InstallationErrorCode
{
    ALREADY_CREATED,
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
