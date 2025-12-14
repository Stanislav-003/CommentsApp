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
        { "UserIsNotAuthorized", "You are not authorized. Please, login or register." },
        { "YouMustAttachEitherAPictureOrATextFile", "You must attach either a picture or a text file" },
        { "YouMustAttachOnlyPictureOrTextFile", "You must attach only a picture or a text file" },
        { "AllowedImageFormatsAre_JPG_PNG_GIF", "Allowed image formats are JPG, PNG and GIF" },
        { "UnsupportedImageFormat", "Unsupported image format"},
        { "Only_TXT_FilesAreAllowed", "Only txt files are allowed except images files"},
        { "TextFileSizeMustNotExceed_100KB", "Text file size must not exceed 100KB"},
        { "UnsupportedFileType", "Unsupported file type"},
        { "YouCanDeleteOnlyYourOwnComments", "You can delete only your own comments"}
    };

    public static string Map(string backendDetail)
    {
        if (_map.TryGetValue(backendDetail, out var message))
            return message;

        return backendDetail;
    }
}
