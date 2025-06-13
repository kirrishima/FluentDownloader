using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp.Options;

/// <summary>
/// Provides extension methods for the <see cref="OptionSet"/> class.
/// </summary>
public static class OptionSetExtensions
{
    /// <summary>
    /// Converts an <see cref="OptionSet"/> object to a string of command-line parameters.
    /// </summary>
    /// <param name="options">The <see cref="OptionSet"/> object to be converted.</param>
    /// <returns>A string representing the command-line parameters based on the properties of the <see cref="OptionSet"/>.</returns>
    public static string ToCliParameters(this OptionSet options)
    {
        try
        {
            var parameters = new StringBuilder();

            var properties = options.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(options);

                if (value == null || (value is bool boolValue && !boolValue))
                    continue;

                string paramName = $"--{property.Name.ToLower()}";

                parameters.Append(paramName);

                if (value is not bool)
                {
                    parameters.Append(" ");
                    parameters.Append(value.ToString());
                }

                parameters.Append(" ");
            }

            return parameters.ToString().Trim();
        }
        catch { return string.Empty; }
    }
}
