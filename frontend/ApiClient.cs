using frontend.Auth;
using frontend.Errors;
using frontend.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;

namespace frontend;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authStateProvider;

    public ApiClient(
        HttpClient httpClient,
        ProtectedLocalStorage localStorage,
        NavigationManager navigationManager,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;
    }

    public async Task SetAuthorizeHeader()
    {
        try
        {
            var sessionState = (await _localStorage.GetAsync<LoginResponseModel>("sessionState")).Value;
            if (sessionState != null && !string.IsNullOrEmpty(sessionState.Token))
            {
                if (sessionState.TokenExpired < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
                    _navigationManager.NavigateTo("/login");
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.Token);
                }

                var requestCulture = new RequestCulture(
                        CultureInfo.CurrentCulture,
                        CultureInfo.CurrentUICulture);

                var cultureCookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                _httpClient.DefaultRequestHeaders.Add("Cookie", $"{CookieRequestCultureProvider.DefaultCookieName}={cultureCookieValue}");
            }
        }
        catch (Exception)
        {
            _navigationManager.NavigateTo("/login");
        }
    }

    public async Task HandleErrors(HttpResponseMessage res)
    {
        var errorContent = await res.Content.ReadAsStringAsync();

        var errors = JsonConvert.DeserializeObject<List<Error>>(errorContent, new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        });

        if (errors != null && errors.Count > 0)
        {
            var messages = errors.Select(e => ErrorMapper.Map(e.Detail)).ToList();
            throw new ApplicationException(string.Join("\n", messages));
        }

        throw new ApplicationException("Unknown error occurred");
    }

    public async Task<T> GetFromJsonAsync<T>(string path)
    {
        await SetAuthorizeHeader();

        var result = await _httpClient.GetAsync(path);

        if (result.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync())!;

        await HandleErrors(result);
        return default!;
    }

    public async Task<string> GetStringAsync(string path)
    {
        await SetAuthorizeHeader();
        var res = await _httpClient.GetAsync(path);

        if (res.IsSuccessStatusCode)
            return await res.Content.ReadAsStringAsync();

        await HandleErrors(res);
        return string.Empty;
    }

    public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizeHeader();

        var result = await _httpClient.PostAsJsonAsync(path, postModel);

        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T1>(content)!;
        }

        await HandleErrors(result);
        return default!;
    }

    public async Task<T1> PutAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizeHeader();
        
        var result = await _httpClient.PutAsJsonAsync(path, postModel);

        if (result.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T1>(await result.Content.ReadAsStringAsync())!;

        await HandleErrors(result);
        return default!;
    }

    public async Task<T> DeleteAsync<T>(string path)
    {
        await SetAuthorizeHeader();
        
        var result = await _httpClient.DeleteAsync(path);

        if (result.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync())!;

        await HandleErrors(result);
        return default!;
    }

    public async Task<string> PostMultipartAsync(string path, MultipartFormDataContent content)
    {
        await SetAuthorizeHeader();

        var response = await _httpClient.PostAsync(path, content);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        await HandleErrors(response);

        return string.Empty;
    }
}