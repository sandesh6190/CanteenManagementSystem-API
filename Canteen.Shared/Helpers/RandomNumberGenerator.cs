using System.Text;

namespace Canteen.Shared.Helpers;

public static class StringHelper
{
    public static string RandomNumberGenerator(int length)
    {
        var digits = "0123456789";
        var random = new Random();
        string resultString;
        var result = new StringBuilder(length);
        result.Append(digits[random.Next(1, digits.Length)]);
        for (int i = 1; i < length; i++)
        {
            result.Append(digits[random.Next(digits.Length)]);
        }
        resultString = result.ToString();
        return resultString;
    }
}