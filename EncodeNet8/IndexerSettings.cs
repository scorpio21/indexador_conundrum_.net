using System.Text.Json;

namespace EncodeNet8;

public sealed class IndexerSettings
{
    public string InitPath { get; set; } = "init";
    public string GraphicsPath { get; set; } = "graficos";
    public string JsonPath => Path.Combine(AppContext.BaseDirectory, "EncodeNet8.settings.json");

    public static IndexerSettings Load()
    {
        var settings = new IndexerSettings();
        if (!File.Exists(settings.JsonPath)) return settings;
        try
        {
            var loaded = JsonSerializer.Deserialize<IndexerSettings>(File.ReadAllText(settings.JsonPath));
            return loaded ?? settings;
        }
        catch { return settings; }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(JsonPath, json);
    }
}
