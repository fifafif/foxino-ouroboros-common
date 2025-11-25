using System;
using System.Text;

namespace Ouroboros.Common.Exceptions
{
    public static class ExceptionsExtensions
    {
        public static string PrintException(this Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine(exception.Message);
            sb.AppendLine($"{exception.TargetSite.Name} - {exception.TargetSite.DeclaringType}");
            sb.AppendLine(exception.StackTrace);

            return sb.ToString();
        }
    }
}