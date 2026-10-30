using Microsoft.AspNetCore.Localization;

namespace MiniStore.Web.Localization;

public static class UiCultureCookie
{
    public static void Append(HttpResponse response, string culture, bool secure)
    {
        response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = secure
            });
    }
}
