namespace RoomReservation.Api.Extensions
{
    public static class CookiesExtensions
    {
        public static void AppendRefreshToken(this IResponseCookies cookies, string refreshToken, bool isDevelopment = false)
        {
            cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = isDevelopment ? false : true,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
                Path = "/auth/"
            });
        }

        public static void DeleteRefreshToken(this IResponseCookies cookies, bool isDevelopment = false)
        {
            cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = isDevelopment ? false : true,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
                Path = "/auth/"
            });
        }
    }
}
