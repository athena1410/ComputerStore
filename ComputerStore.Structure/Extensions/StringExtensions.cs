using System.Linq;

namespace ComputerStore.Structure.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
           input switch
           {
               null => null,
               _ => input.First().ToString().ToUpper() + input.Substring(1)
           };
    }
}
