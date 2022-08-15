namespace Imato.CLR.Logger
{
    public static class StringExtensions
    {
        public static string GetSqlString(this string str, int length = 3999)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return str.Length <= length ? str : str.Substring(0, length);
        }
    }
}