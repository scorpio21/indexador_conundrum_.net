using System.Text.Json;

namespace EncodeNet8;

public sealed class IndexerSettings
{
    public string InitPath { get; set; } = "init";
    public string GraphicsPath { get; set; } = "graficos";
    // Equivalente a CarpetaDedatos/PathDat del cliente VB6.
    // Puede ser una carpeta ("Datos") o la ruta completa de Indices.ini.
    public string IndicesPath { get; set; } = "Datos";
    // 1 = BMP, 2 = ResF, 3 = BMP o ResF.
    // ResF queda documentado pero desactivado hasta definir el compresor propio de Aospain.
    public int ResourceMode { get; set; } = 1;
    public string JsonPath => Path.Combine(AppContext.BaseDirectory, "EncodeNet8.settings.json");

    public string ResolveIndicesFilePath(string baseDirectory)
    {
        var value = string.IsNullOrWhiteSpace(IndicesPath) ? "Datos" : IndicesPath.Trim();
        if (Path.GetExtension(value).Equals(".ini", StringComparison.OrdinalIgnoreCase))
            return Path.IsPathRooted(value) ? value : Path.Combine(baseDirectory, value);

        var folder = Path.IsPathRooted(value) ? value : Path.Combine(baseDirectory, value);
        return Path.Combine(folder, "Indices.ini");
    }

    public static IndexerSettings Load()
    {
        var settings = new IndexerSettings();
        if (!File.Exists(settings.JsonPath))
        {
            LoadLegacyConf(settings);
            return settings;
        }
        try
        {
            var loaded = JsonSerializer.Deserialize<IndexerSettings>(File.ReadAllText(settings.JsonPath));
            return loaded ?? settings;
        }
        catch { return settings; }
    }

    private static void LoadLegacyConf(IndexerSettings settings)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Datos", "Conf.ini");
        if (!File.Exists(path)) return;

        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Trim();
            var separator = line.IndexOf('=');
            if (separator <= 0) continue;
            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            if (key.Equals("CarpetaDeInis", StringComparison.OrdinalIgnoreCase)) settings.InitPath = value;
            else if (key.Equals("CarpetaGraficos", StringComparison.OrdinalIgnoreCase)) settings.GraphicsPath = value;
            else if (key.Equals("CarpetaDedatos", StringComparison.OrdinalIgnoreCase)) settings.IndicesPath = value;
            else if (key.Equals("recurso", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out var mode)) settings.ResourceMode = mode;
        }
        settings.ResourceMode = settings.ResourceMode is >= 1 and <= 3 ? settings.ResourceMode : 1;
    }

    public void Save()
    {
        ResourceMode = ResourceMode is >= 1 and <= 3 ? ResourceMode : 1;
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(JsonPath, json);
    }
}
