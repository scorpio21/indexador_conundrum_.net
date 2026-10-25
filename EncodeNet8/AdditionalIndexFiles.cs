namespace EncodeNet8;

public static class FxIndexFile
{
    private const int HeaderSize = 263;

    public static List<FxIndexRecord> LoadAuto(string path)
    {
        try { return Load(path, GrhStorageFormat.LegacyInteger); }
        catch (InvalidDataException) { return Load(path, GrhStorageFormat.LongIndexes); }
    }

    public static List<FxIndexRecord> Load(string path, GrhStorageFormat format)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        if (stream.Length < HeaderSize + sizeof(short)) throw new InvalidDataException("El índice FX no tiene cabecera.");
        stream.Position = HeaderSize;
        var count = reader.ReadInt16();
        if (count <= 0) return new List<FxIndexRecord>();
        var recordSize = format == GrhStorageFormat.LongIndexes ? 8 : 6;
        if (stream.Length - stream.Position != count * (long)recordSize)
            throw new InvalidDataException("El tamaño de los registros FX no coincide con el contador.");
        var result = new List<FxIndexRecord>(count);
        for (var i = 1; i <= count; i++)
        {
            var animation = format == GrhStorageFormat.LongIndexes ? reader.ReadInt32() : reader.ReadInt16();
            var offsetX = reader.ReadInt16();
            var offsetY = reader.ReadInt16();
            if (animation > 0) result.Add(new FxIndexRecord { Id = i, Animation = animation, OffsetX = offsetX, OffsetY = offsetY });
        }
        return result;
    }

    public static void Save(string path, IReadOnlyCollection<FxIndexRecord> records, GrhStorageFormat format)
    {
        var count = records.Count == 0 ? 0 : records.Max(record => record.Id);
        var byId = records.ToDictionary(record => record.Id);
        var prefix = File.Exists(path) ? ReadPrefix(path) : new byte[HeaderSize];
        using var stream = File.Create(path);
        using var writer = new BinaryWriter(stream);
        writer.Write(prefix);
        writer.Write((short)Math.Min(count, short.MaxValue));
        for (var i = 1; i <= count; i++)
        {
            byId.TryGetValue(i, out var record);
            var animation = record?.Animation ?? 0;
            if (format == GrhStorageFormat.LongIndexes) writer.Write(animation);
            else writer.Write(animation is >= short.MinValue and <= short.MaxValue ? (short)animation : (short)0);
            writer.Write(record?.OffsetX ?? (short)0);
            writer.Write(record?.OffsetY ?? (short)0);
        }
    }

    private static byte[] ReadPrefix(string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        var prefix = reader.ReadBytes(HeaderSize);
        return prefix.Length == HeaderSize ? prefix : new byte[HeaderSize];
    }
}

public static class IniDirectionalIndexFile
{
    public static List<DirectionalIndexRecord> Load(string path, string state)
    {
        var countKey = state switch
        {
            "Armas" => "NumArmas",
            "Escudos" => "NumEscudos",
            "Alas" => "NumAlas",
            "Capas" => "NumCapas",
            "Botas" => "NumBotas",
            _ => throw new InvalidDataException($"Estado INI no soportado: {state}")
        };
        var sectionPrefix = state switch
        {
            "Armas" => "ARMA",
            "Escudos" => "ESC",
            "Alas" => "ALAS",
            "Capas" => "CAPA",
            "Botas" => "BOTA",
            _ => string.Empty
        };
        var keyPrefix = state switch
        {
            "Alas" => "ALAS",
            "Capas" => "Capa",
            "Botas" => "Bota",
            _ => "Dir"
        };
        var count = 0;
        var sections = new Dictionary<int, int[]>( );
        var current = 0;
        var inInit = false;
        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Split('\'', 2)[0].Trim();
            if (line.StartsWith("["))
            {
                inInit = line.Equals("[INIT]", StringComparison.OrdinalIgnoreCase);
                current = 0;
                var section = line.Trim('[', ']');
                if (section.StartsWith(sectionPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(section[sectionPrefix.Length..], out current);
                    if (current > 0) sections[current] = new int[4];
                }
                continue;
            }
            var separator = line.IndexOf('=');
            if (separator <= 0) continue;
            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            if (inInit && key.Equals(countKey, StringComparison.OrdinalIgnoreCase))
                int.TryParse(value, out count);
            if (current <= 0 || !key.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase)) continue;
            if (!int.TryParse(key[keyPrefix.Length..], out var direction) || direction is < 1 or > 4) continue;
            if (int.TryParse(value, out var grh)) sections[current][direction - 1] = grh;
        }

        return sections.Where(pair => pair.Key <= count && pair.Value[0] > 0)
            .OrderBy(pair => pair.Key)
            .Select(pair => new DirectionalIndexRecord { Id = pair.Key, Directions = pair.Value })
            .ToList();
    }

    public static void Save(string path, IReadOnlyCollection<DirectionalIndexRecord> records, string state)
    {
        var countKey = state switch
        {
            "Armas" => "NumArmas",
            "Escudos" => "NumEscudos",
            "Alas" => "NumAlas",
            "Capas" => "NumCapas",
            "Botas" => "NumBotas",
            _ => throw new InvalidDataException($"Estado INI no soportado: {state}")
        };
        var sectionPrefix = state switch
        {
            "Armas" => "ARMA",
            "Escudos" => "ESC",
            "Alas" => "ALAS",
            "Capas" => "CAPA",
            "Botas" => "BOTA",
            _ => string.Empty
        };
        var keyPrefix = state switch
        {
            "Alas" => "ALAS",
            "Capas" => "Capa",
            "Botas" => "Bota",
            _ => "Dir"
        };
        var count = records.Count == 0 ? 0 : records.Max(record => record.Id);
        var byId = records.ToDictionary(record => record.Id);
        using var writer = new StreamWriter(path, false);
        writer.WriteLine("[INIT]");
        writer.WriteLine($"{countKey}={count}");
        for (var i = 1; i <= count; i++)
        {
            if (!byId.TryGetValue(i, out var record)) continue;
            writer.WriteLine();
            writer.WriteLine($"[{sectionPrefix}{i}]");
            for (var direction = 0; direction < 4; direction++)
                writer.WriteLine($"{keyPrefix}{direction + 1}={record.Directions[direction]}");
        }
    }
}
