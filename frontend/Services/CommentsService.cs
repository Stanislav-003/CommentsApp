using Azure;
using frontend.Requests;
using frontend.Responses;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System.Net.Http;

namespace frontend.Services;

public class CommentsService
{
    private readonly ApiClient _apiClient;
    public Func<Task>? RefreshComments { get; set; }

    public CommentsService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CommentsResponse>> GetAllComments(
        string? username = null,
        string? email = null,
        bool sortAscending = false,
        int page = 1,
        int pageSize = 200)
    {
        var url = $"/api/comments/get-all-comments?username={username}&email={email}&sortAscending={sortAscending}&page={page}&pageSize={pageSize}";
        return await _apiClient.GetFromJsonAsync<List<CommentsResponse>>(url);
    }

    public async Task<GetCaptchaResponse> GetCaptcha()
    {
        return await _apiClient.GetFromJsonAsync<GetCaptchaResponse>("/api/captcha/generate");
    }

    public async Task<GetCaptchaResponse> RefreshCaptcha(string captchaId)
    {
        return await _apiClient.GetFromJsonAsync<GetCaptchaResponse>($"/api/captcha/refresh?CaptchaId={captchaId}");
    }

    public async Task<Guid> CreateComment(CreateCommentRequest request)
    {
        return await _apiClient.PostAsync<Guid, CreateCommentRequest>("/api/comments/create", request);
    }

    public async Task<Guid> DeleteComment(DeleteCommentRequest request)
    {
        return await _apiClient.PostAsync<Guid, DeleteCommentRequest>("/api/comments/delete", request);
    }

    public async Task<Guid> UploadFile(Guid commentId, IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(commentId.ToString()), "CommentId");

        var fileStream = file.OpenReadStream(file.Size);
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

        if (file.ContentType.StartsWith("image/"))
        {
            content.Add(streamContent, "Picture", file.Name);
        }
        else if (file.ContentType == "text/plain")
        {
            content.Add(streamContent, "TextFile", file.Name);
        }
        else
        {
            throw new ApplicationException("Only images or text files are allowed.");
        }

        var url = $"/api/comments/file-upload?CommentId={commentId}";

        var result = await _apiClient.PostMultipartAsync(url, content);
        result = result.Trim('"');

        return Guid.Parse(result);
    }

    public async Task<FileResponse> DownloadFile(Guid commentId)
    {
        var json = await _apiClient.GetStringAsync($"/api/comments/file-download?commentId={commentId}");
        return JsonConvert.DeserializeObject<FileResponse>(json)!;
    }
}
