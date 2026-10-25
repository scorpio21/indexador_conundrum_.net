namespace EncodeNet8;

public static class DirectionalIndexFile
{
    private const int HeaderSize = 263;

    public static List<DirectionalIndexRecord> LoadAuto(string path, bool includesOffsets)
    {
        try { return Load(path, GrhStorageFormat.LegacyInteger, includesOffsets); }
        catch (InvalidDataException) { return Load(path, GrhStorageFormat.LongIndexes, includesOffsets); }
    }

    public static List<DirectionalIndexRecord> Load(string path, GrhStorageFormat format, bool includesOffsets)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        if (stream.Length < HeaderSize + sizeof(short))
            throw new InvalidDataException("El índice no contiene una cabecera completa.");

        stream.Position = HeaderSize;
        var count = reader.ReadInt16();
        if (count <= 0) return new List<DirectionalIndexRecord>();

        var recordSize = includesOffsets
            ? (format == GrhStorageFormat.LongIndexes ? 20 : 12)
            : (format == GrhStorageFormat.LongIndexes ? 16 : 8);
        var remaining = stream.Length - stream.Position;
        if (remaining != count * (long)recordSize)
            throw new InvalidDataException($"El tamaño de los registros no coincide: se esperaban {count * recordSize} bytes y hay {remaining}.");

        var result = new List<DirectionalIndexRecord>(count);
        for (var i = 1; i <= count; i++)
        {
            var directions = new int[4];
            for (var direction = 0; direction < directions.Length; direction++)
                directions[direction] = format == GrhStorageFormat.LongIndexes ? reader.ReadInt32() : reader.ReadInt16();

            var offsetX = includesOffsets ? reader.ReadInt16() : (short)0;
            var offsetY = includesOffsets ? reader.ReadInt16() : (short)0;
            if (directions[0] > 0)
                result.Add(new DirectionalIndexRecord { Id = i, Directions = directions, OffsetX = offsetX, OffsetY = offsetY });
        }

        return result;
    }
}
