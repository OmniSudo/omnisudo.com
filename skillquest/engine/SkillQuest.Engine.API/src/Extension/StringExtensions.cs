namespace SkillQuest.Engine.API.Extension;

public static class StringExtensions
{
    public static string? CapitalizeFirstLetter(this string input) =>
        input switch
        {
            null => null,
            "" => string.Empty,
            _ => input[0].ToString().ToUpper() + input.Substring(1)
        };
}