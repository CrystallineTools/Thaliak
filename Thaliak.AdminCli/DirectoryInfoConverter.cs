using System.ComponentModel;
using System.Globalization;

namespace Thaliak.AdminCli;

public class DirectoryInfoConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var casted = value as string;
        if (casted == null) {
            return base.ConvertFrom(context, culture, value);
        }

        return new DirectoryInfo(Path.GetFullPath(casted));
    }
}
