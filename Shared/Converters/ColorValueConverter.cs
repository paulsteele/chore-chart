using System.Drawing;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace hub.Shared.Converters;

public class ColorValueConverter() : ValueConverter<Color, int>(color => color.ToArgb(), i => Color.FromArgb(i));