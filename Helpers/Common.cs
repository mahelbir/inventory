namespace Inventory.Helpers
{
    public static class Common
    {
        public static bool IsValidID(int? id)
        {
            return id != null && id > 0;
        }

        public static bool IsValidCode(string? Code)
        {
            return !String.IsNullOrEmpty(Code);
        }

        public static string BaseURL(HttpContext httpContext)
        {
            var request = httpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }

    }
}
