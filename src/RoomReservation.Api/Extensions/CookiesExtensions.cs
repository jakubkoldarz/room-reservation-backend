namespace RoomReservation.Api.Extensions
{
    public static class CookiesExtensions
    {
        public static void AppendRefreshToken(this IResponseCookies cookies, string refreshToken)
        {
            cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        public static void DeleteRefreshToken(this IResponseCookies cookies)
        {
            cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }
    }
}
