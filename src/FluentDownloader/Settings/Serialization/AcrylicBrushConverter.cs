using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using CommunityToolkit.WinUI.Helpers; // Предполагается, что в этом пространстве есть метод расширения ToColor()

public class AcrylicBrushJsonConverter : JsonConverter<AcrylicBrush?>
{
    public override AcrylicBrush? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        double tintOpacity = 0;
        double opacity = 0;
        string? tintColorStr = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propertyName = reader.GetString()!;
            reader.Read();
            switch (propertyName)
            {
                case "TintOpacity":
                    tintOpacity = reader.GetDouble();
                    break;
                case "Opacity":
                    opacity = reader.GetDouble();
                    break;
                case "TintColor":
                    tintColorStr = reader.GetString();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (string.IsNullOrEmpty(tintColorStr))
        {
            throw new JsonException("TintColor cannot be null or empty.");
        }

        // Преобразование строки в Color. Предполагается, что метод ToColor() корректно реализован.
        Color color = tintColorStr.ToColor();

        return new AcrylicBrush
        {
            TintOpacity = tintOpacity,
            Opacity = opacity,
            TintColor = color
        };
    }

    public override void Write(Utf8JsonWriter writer, AcrylicBrush? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("TintOpacity", value.TintOpacity);
        writer.WriteNumber("Opacity", value.Opacity);
        writer.WriteString("TintColor", value.TintColor.ToString());
        writer.WriteEndObject();
    }
}
