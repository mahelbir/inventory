namespace Inventory.Utils
{
    public class Helper
    {
        public static bool IsValidID(int? id)
        {
            return id != null && id > 0;
        }

        public static bool IsValidCode(string? Code)
        {
            return !String.IsNullOrEmpty(Code);
        }

    }
}
