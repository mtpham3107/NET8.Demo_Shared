using Microsoft.Extensions.Localization;

namespace NET8.Demo.GlobalAdmin;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    public IStringLocalizer Create(string baseName, string location)
    {
        return new JsonStringLocalizer(System.Globalization.CultureInfo.CurrentUICulture.Name);
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new JsonStringLocalizer(System.Globalization.CultureInfo.CurrentUICulture.Name);
    }
}