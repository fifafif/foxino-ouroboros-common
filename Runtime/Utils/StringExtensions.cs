namespace Ouroboros.Common.Utils
{
    public static class StringExtensions
    {
        public static string IfNullOrEmptyReturnOther(this string s, string other)
        {
            if (string.IsNullOrEmpty(s))
            {
                return other;
            }

            return s;
        }

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
