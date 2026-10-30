using System.Globalization;
using System.Text;

namespace NbaHub.Domain.Players;

/// <summary>Builds URL slugs such as "luka-doncic" from player names.</summary>
public static class PlayerSlug
{
    public static string From(string firstName, string lastName)
    {
        var text = $"{firstName} {lastName}".Normalize(NormalizationForm.FormD);
        var slug = new StringBuilder(text.Length);
        var pendingHyphen = false;

        foreach (var c in text)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue; // drops accents: "č" becomes "c"
            }

            if (char.IsAsciiLetterOrDigit(c))
            {
                if (pendingHyphen && slug.Length > 0)
                {
                    slug.Append('-');
                }

                slug.Append(char.ToLowerInvariant(c));
                pendingHyphen = false;
            }
            else if (c is not '\'' and not '’')
            {
                pendingHyphen = true; // spaces, hyphens, dots and other separators
            }
        }

        if (slug.Length == 0)
        {
            throw new ArgumentException("Name must contain at least one letter or digit.", nameof(firstName));
        }

        return slug.ToString();
    }
}
