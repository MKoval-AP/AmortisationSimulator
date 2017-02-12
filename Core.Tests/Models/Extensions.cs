using System.Text;

namespace AmortisationSimulator.Core.Tests.Models
{
    public static class Extensions
    {
        public static void AppendIndented(this StringBuilder sb, string message) => sb.AppendLine($"\t{message}");

        public static void AppendHeader(this StringBuilder sb, string message) => sb.AppendLine($"=========={message.ToUpper()}==========");

        public static void AppendFooter(this StringBuilder sb, string message) => sb.AppendLine($"----------{message}----------");
    }
}