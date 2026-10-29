using System.Text;

namespace EncodeNet8;

public enum EncodeIndexType
{
    Alas,
    Body,
    Botas,
    Cascos,
    Escudos,
    Fx,
    Head,
    Mapas
}

public sealed record EncodeIndexRow(int Number, string Name, IReadOnlyDictionary<string, int> Values);

/// <summary>
/// Convierte los DAT INI de la carpeta encode a los índices binarios del cliente.
/// Los tipos y tamaños siguen los Type tIndice*Long del VB6 original.
/// </summary>
public static class DatIndexEncoder
{
    private const int HeaderSize = 263;

    public static List<EncodeIndexRow> Load(string path, EncodeIndexType type)
    {
        var sections = Parse(path);
        var countKey = type switch
        {
            EncodeIndexType.Alas => "NumAlas",
            EncodeIndexType.Body => "NumBodies",
            EncodeIndexType.Botas => "NumBotas",
            EncodeIndexType.Cascos => "NumCascos",
            EncodeIndexType.Escudos => "NumEscudos",
            EncodeIndexType.Fx => "NumFxs",
            EncodeIndexType.Head => "NumHeads",
            _ => "NumMaps"
        };

        var count = GetValue(sections, "INIT", countKey);
        var rows = new List<EncodeIndexRow>(count);
        for (var number = 1; number <= count; number++)
        {
            var section = SectionName(type, number);
            var values = sections.TryGetValue(section, out var data)
                ? new Dictionary<string, int>(data, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            rows.Add(new EncodeIndexRow(number, section, values));
        }
        return rows;
    }

    public static void Save(string sourcePath, string outputPath, EncodeIndexType type, IReadOnlyList<EncodeIndexRow> rows)
    {
        if (type == EncodeIndexType.Mapas)
        {
            var rain = rows.Select(row => GetValue(row.Values, "Lluvia")).Select(value => (byte)Math.Clamp(value, 0, 255)).ToList();
            MapIndexFile.Encode(sourcePath, outputPath, rain);
            return;
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
        var header = ReadHeader(outputPath);
        using var writer = new BinaryWriter(File.Create(outputPath), Encoding.Latin1);
        writer.Write(header);
        writer.Write((short)rows.Count);
        foreach (var row in rows) WriteRow(writer, type, row.Values);
    }

    private static void WriteRow(BinaryWriter writer, EncodeIndexType type, IReadOnlyDictionary<string, int> values)
    {
        string[] keys = type switch
        {
            EncodeIndexType.Body => new[] { "Walk1", "Walk2", "Walk3", "Walk4" },
            EncodeIndexType.Alas => new[] { "Alas1", "Alas2", "Alas3", "Alas4" },
            EncodeIndexType.Botas => new[] { "Bota1", "Bota2", "Bota3", "Bota4" },
            EncodeIndexType.Cascos => new[] { "Head1", "Head2", "Head3", "Head4" },
            EncodeIndexType.Head => new[] { "Head0", "Head1", "Head2", "Head3" },
            EncodeIndexType.Escudos => new[] { "Dir1", "Dir2", "Dir3", "Dir4" },
            EncodeIndexType.Fx => new[] { "Animacion" },
            _ => Array.Empty<string>()
        };

        foreach (var key in keys) writer.Write(GetValue(values, key));
        if (type is EncodeIndexType.Body or EncodeIndexType.Alas or EncodeIndexType.Botas)
        {
            writer.Write((short)GetValue(values, type == EncodeIndexType.Body ? "HeadOffsetX" : type + "OffsetX"));
            writer.Write((short)GetValue(values, type == EncodeIndexType.Body ? "HeadOffsetY" : type + "OffsetY"));
        }
        else if (type == EncodeIndexType.Fx)
        {
            writer.Write((short)GetValue(values, "OffsetX"));
            writer.Write((short)GetValue(values, "OffsetY"));
        }
    }

    private static string SectionName(EncodeIndexType type, int number) => type switch
    {
        EncodeIndexType.Alas => $"Alas{number}",
        EncodeIndexType.Body => $"Body{number}",
        EncodeIndexType.Botas => $"Bota{number}",
        EncodeIndexType.Cascos => $"Casco{number}",
        EncodeIndexType.Escudos => $"ESC{number}",
        EncodeIndexType.Fx => $"FX{number}",
        EncodeIndexType.Head => $"Head{number}",
        EncodeIndexType.Mapas => $"Map{number}",
        _ => $"Index{number}"
    };

    private static Dictionary<string, Dictionary<string, int>> Parse(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("No se encontró el archivo DAT.", path);
        var result = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int>? current = null;
        foreach (var raw in File.ReadLines(path))
        {
            var comment = raw.IndexOf('\'');
            var line = (comment >= 0 ? raw[..comment] : raw).Trim();
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                var name = line[1..^1].Trim();
                current = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                result[name] = current;
                continue;
            }
            var separator = line.IndexOf('=');
            if (current is null || separator <= 0) continue;
            if (int.TryParse(line[(separator + 1)..].Trim(), out var value))
                current[line[..separator].Trim()] = value;
        }
        return result;
    }

    private static int GetValue(IReadOnlyDictionary<string, int> values, string key) =>
        values.TryGetValue(key, out var value) ? value : 0;

    private static int GetValue(Dictionary<string, Dictionary<string, int>> sections, string section, string key) =>
        sections.TryGetValue(section, out var values) ? GetValue(values, key) : 0;

    private static byte[] ReadHeader(string path)
    {
        if (!File.Exists(path)) return new byte[HeaderSize];
        using var stream = File.OpenRead(path);
        var header = new byte[HeaderSize];
        return stream.Read(header, 0, HeaderSize) == HeaderSize ? header : new byte[HeaderSize];
    }
}
