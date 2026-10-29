using System.Text;

namespace EncodeNet8;

/// <summary>
/// Lee mapas.dat y genera FK.ind con el formato binario que consume el cliente VB6.
/// Formato: cabecera tCabecera (255 bytes + 2 Long = 263 bytes),
/// contador Integer y un byte Lluvia por mapa.
/// </summary>
public static class MapIndexFile
{
    private const int HeaderSize = 263;

    public sealed record MapSetting(int Number, string Name, byte RainMode);

    public static int Encode(string mapasDatPath, string fkPath)
    {
        if (!File.Exists(mapasDatPath))
            throw new FileNotFoundException("No se encontró mapas.dat.", mapasDatPath);

        var maps = Load(mapasDatPath);
        return Encode(mapasDatPath, fkPath, maps.Select(map => map.RainMode).ToList());
    }

    public static int Encode(string mapasDatPath, string fkPath, IReadOnlyList<byte> rainModes)
    {
        if (!File.Exists(mapasDatPath))
            throw new FileNotFoundException("No se encontró mapas.dat.", mapasDatPath);

        var maps = Load(mapasDatPath);
        if (maps.Count != rainModes.Count)
            throw new InvalidDataException("La cantidad de valores no coincide con NumMaps.");
        if (maps.Count > short.MaxValue)
            throw new InvalidDataException($"NumMaps ({maps.Count}) supera el límite Integer del formato VB6.");

        var directory = Path.GetDirectoryName(Path.GetFullPath(fkPath));
        if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);

        var header = File.Exists(fkPath) ? ReadHeader(fkPath) : CreateHeader();
        using var stream = File.Create(fkPath);
        using var writer = new BinaryWriter(stream, Encoding.Latin1, leaveOpen: false);
        writer.Write(header);
        writer.Write((short)maps.Count);
        foreach (var value in rainModes) writer.Write(value);
        return maps.Count;
    }

    public static List<MapSetting> Load(string path)
    {
        var count = 0;
        var currentMap = 0;
        var currentName = string.Empty;
        var inInit = false;
        var values = new Dictionary<int, (string Name, byte RainMode)>();

        foreach (var rawLine in File.ReadLines(path))
        {
            var commentIndex = rawLine.IndexOf('\'');
            var line = (commentIndex >= 0 ? rawLine[..commentIndex] : rawLine).Trim();
            var comment = commentIndex >= 0 ? rawLine[(commentIndex + 1)..].Trim() : string.Empty;

            if (line.StartsWith("[", StringComparison.Ordinal) && line.EndsWith("]", StringComparison.Ordinal))
            {
                var section = line[1..^1].Trim();
                inInit = section.Equals("INIT", StringComparison.OrdinalIgnoreCase);
                currentMap = 0;
                currentName = comment;
                if (section.StartsWith("Map", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(section[3..], out var mapNumber) && mapNumber > 0)
                    currentMap = mapNumber;
                continue;
            }

            var separator = line.IndexOf('=');
            if (separator <= 0) continue;
            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();

            if (inInit && key.Equals("NumMaps", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(value, out count) || count < 0)
                    throw new InvalidDataException("El valor INIT/NumMaps no es válido.");
            }

            if (currentMap > 0 && key.Equals("Lluvia", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(value, out var rain))
                values[currentMap] = (currentName, (byte)Math.Clamp(rain, byte.MinValue, byte.MaxValue));
        }

        if (count == 0) return new List<MapSetting>();
        var maps = new List<MapSetting>(count);
        for (var i = 1; i <= count; i++)
        {
            var value = values.TryGetValue(i, out var map) ? map : (string.Empty, (byte)0);
            maps.Add(new MapSetting(i, value.Item1, value.Item2));
        }
        return maps;
    }

    private static byte[] ReadHeader(string path)
    {
        using var stream = File.OpenRead(path);
        var header = new byte[HeaderSize];
        var read = stream.Read(header, 0, header.Length);
        return read == HeaderSize ? header : CreateHeader();
    }

    private static byte[] CreateHeader()
    {
        var header = new byte[HeaderSize];
        var description = Encoding.Latin1.GetBytes("Aomania by Helios. Copyright Noland-Studios 2001, pablomarquez@noland-studios.com.ar");
        Array.Copy(description, header, Math.Min(description.Length, 255));
        return header;
    }
}
