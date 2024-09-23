using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace NET8.Demo.TemplateService;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly Dictionary<string, string> _localizedStrings;

    public JsonStringLocalizer(string culture)
    {
        var basePath = AppContext.BaseDirectory;
        var jsonFile = Path.Combine(basePath, "Localization", $"{culture}.json");
        if (File.Exists(jsonFile))
        {
            var json = File.ReadAllText(jsonFile);
            _localizedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        else
        {
            _localizedStrings = new Dictionary<string, string>();
        }
    }

    public LocalizedString this[string name]
    {
        get
        {
            return _localizedStrings.TryGetValue(name, out var value)
                ? new LocalizedString(name, value, true)
                : new LocalizedString(name, name, false);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = this[name].Value;
            return new LocalizedString(name, string.Format(value, arguments), true);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        foreach (var kvp in _localizedStrings)
        {
            yield return new LocalizedString(kvp.Key, kvp.Value, true);
        }
    }
}
