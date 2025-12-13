namespace frontend.Errors;

public static class ErrorMapper
{
    private static readonly Dictionary<string, string> _map = new()
    {
        { "CaptchaCodeMustBeProvided", "Captcha is required" },
        { "CaptchaIdMustBeProvided", "Captcha ID is required" },
        { "TextIsRequired", "Comment text is required" },
        { "InvalidCAPTCHAPleaseTryAgain", "Captcha code is invalid" },
        { "CaptchaHasExpiredOrIsInvalidPleaseRefreshAndTryAgain", "Captcha code is expired, please try again" },
        { "UserIsNotAuthorized", "Please login" }
    };

    public static string Map(string backendDetail)
    {
        if (_map.TryGetValue(backendDetail, out var message))
            return message;

        return backendDetail;
    }
}
