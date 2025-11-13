using UnityEngine;
using System.Text; // Required for StringBuilder

public static class UsernameGenerator
{
    // You can easily expand these lists with more words.
    private static readonly string[] adjectives = new string[]
    {
        "Silent", "Golden", "Red", "Blue", "Shadow", "Iron", "Brave", "Swift", "Dark", "Light",
        "Ancient", "Mystic", "Crimson", "Lazy", "Happy", "Angry", "Clever", "Frost", "Solar", "Lunar",
        "Tiny", "Giant", "Phantom", "Electric", "Aqua", "Terra", "Cosmic", "Grand", "Royal", "Noble"
    };

    private static readonly string[] nouns = new string[]
    {
        "Wolf", "Dragon", "Panda", "Fox", "Knight", "Wizard", "Eagle", "Lion", "Tiger", "Shark",
        "Phoenix", "Golem", "Specter", "Hunter", "Warden", "Blade", "Storm", "Heart", "Soul", "Byte",
        "Agent", "King", "Queen", "Rogue", "Saint", "Reaper", "Guardian", "Viper", "Cobra", "Shield"
    };

    /// <summary>
    /// Generates a plausible, human-like username.
    /// </summary>
    /// <param name="addNumber">Should a number be appended to the end?</param>
    /// <param name="capitalize">Should the first letter of each word be capitalized (PascalCase)?</param>
    /// <param name="separator">A character to place between the words (e.g., "_" or "."). Leave empty for none.</param>
    /// <param name="numberDigits">How many digits should the appended number have (e.g., 2 for 00-99).</param>
    /// <returns>A randomly generated username string.</returns>
    public static string Generate(bool addNumber = true, bool capitalize = true, string separator = "", int numberDigits = 2)
    {
        // 1. Pick a random adjective and noun
        string adjective = adjectives[Random.Range(0, adjectives.Length)];
        string noun = nouns[Random.Range(0, nouns.Length)];

        // 2. Handle capitalization
        if (capitalize)
        {
            adjective = CapitalizeFirstLetter(adjective);
            noun = CapitalizeFirstLetter(noun);
        }
        else
        {
            adjective = adjective.ToLower();
            noun = noun.ToLower();
        }

        // 3. Build the base username using StringBuilder for efficiency
        StringBuilder usernameBuilder = new StringBuilder();
        usernameBuilder.Append(adjective);
        if (!string.IsNullOrEmpty(separator))
        {
            usernameBuilder.Append(separator);
        }
        usernameBuilder.Append(noun);

        // 4. Optionally append a number
        if (addNumber)
        {
            int maxNumber = (int)Mathf.Pow(10, numberDigits); // e.g., 10^2 = 100 for 0-99
            string numberString = Random.Range(0, maxNumber).ToString();

            // Pad with leading zeros if necessary (e.g., 7 becomes "07")
            usernameBuilder.Append(numberString.PadLeft(numberDigits, '0'));
        }

        return usernameBuilder.ToString();
    }

    /// <summary>
    /// Helper function to capitalize the first letter of a string.
    /// </summary>
    private static string CapitalizeFirstLetter(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }
}