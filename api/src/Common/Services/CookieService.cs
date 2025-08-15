using Microsoft.Extensions.Options;

using CookieOptions_ = Api.Common.Options.CookieOptions;

namespace Api.Common.Services;

public interface ICookieService
{
    void AppendCookie(HttpResponse httpResponse, string key, string value, string path, DateTimeOffset expires);
    void DeleteCookie(HttpResponse httpResponse, string key);
}

public class CookieService(IOptions<CookieOptions_> cookieOptions) : ICookieService
{
    public void AppendCookie(HttpResponse httpResponse, string key, string value, string path, DateTimeOffset expires)
    {
        httpResponse.Cookies.Append(key, value, new CookieOptions()
        {
            Expires = expires,
            Path = path,
            Secure = cookieOptions.Value.Secure,
            HttpOnly = cookieOptions.Value.HttpOnly,
            SameSite = Enum.Parse<SameSiteMode>(cookieOptions.Value.SameSite)
        });
    }

    public void DeleteCookie(HttpResponse httpResponse, string key)
    {
        httpResponse.Cookies.Delete(key);
    }
}
