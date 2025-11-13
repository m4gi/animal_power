using System.Globalization;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    /// <summary>
    /// Normalizes a string into a "Title Case with spaces" format.
    /// Supports snake_case, camelCase, PascalCase, alphanumeric words, and removes special characters.
    /// </summary>
    public static string NormalizeString(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string formatted = input;

        // 1️⃣ Replace underscores or hyphens with spaces
        formatted = Regex.Replace(formatted, @"[_\-]+", " ");

        // 2️⃣ Insert space before capital letters (for camelCase / PascalCase)
        formatted = Regex.Replace(formatted, @"([a-z])([A-Z])", "$1 $2");

        // 3️⃣ Add space between letters and numbers (e.g. Car1 -> Car 1)
        formatted = Regex.Replace(formatted, @"([a-zA-Z])(\d)", "$1 $2");

        // 4️⃣ Add space between numbers and letters (e.g. 1Car -> 1 Car)
        formatted = Regex.Replace(formatted, @"(\d)([a-zA-Z])", "$1 $2");

        // 5️⃣ Remove extra spaces
        formatted = Regex.Replace(formatted, @"\s+", " ").Trim();

        // 6️⃣ Convert to Title Case (capitalize each word)
        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        formatted = textInfo.ToTitleCase(formatted.ToLower());

        // 7️⃣ Preserve uppercase acronyms (e.g. HP, UI, AI)
        formatted = Regex.Replace(formatted, @"\b([A-Z]{2,})\b", m => m.Value.ToUpper());

        return formatted;
    }
    
    /// <summary>
    /// Formats a large number into a short, readable string (K, M, B, T).
    /// Example: 100000 -> "100K", 1500000 -> "1.5M"
    /// </summary>
    /// <param name="number">The number to format.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatNumber(double number)
    {
        if (number < 1000)
        {
            // If the number is less than 1000, return it as an integer string.
            return ((int)number).ToString();
        }

        if (number < 1000000) // Less than 1 million -> format as K
        {
            // Divide by 1000 and append "K".
            // "0.#" will show one decimal place if needed (e.g., 1.5K)
            // and won't show it if it's a whole number (e.g., 100K instead of 100.0K).
            return (number / 1000f).ToString("0.#", CultureInfo.InvariantCulture) + "K";
        }

        if (number < 1000000000) // Less than 1 billion -> format as M
        {
            // Divide by 1 million and append "M".
            return (number / 1000000f).ToString("0.#", CultureInfo.InvariantCulture) + "M";
        }

        if (number < 1000000000000) // Less than 1 trillion -> format as B
        {
            // Divide by 1 billion and append "B".
            return (number / 1000000000f).ToString("0.#", CultureInfo.InvariantCulture) + "B";
        }
        
        // From 1 trillion upwards -> format as T
        return (number / 1000000000000f).ToString("0.#", CultureInfo.InvariantCulture) + "T";
    }
    
    /// <summary>
    /// Overload for formatting an integer.
    /// </summary>
    public static string FormatNumber(int number)
    {
        return FormatNumber((double)number);
    }

    /// <summary>
    /// Overload for formatting a long integer.
    /// </summary>
    public static string FormatNumber(long number)
    {
        return FormatNumber((double)number);
    }

    /// <summary>
    /// Overload for formatting a float.
    /// </summary>
    public static string FormatNumber(float number)
    {
        return FormatNumber((double)number);
    }
}