namespace EncodeNet8;

public sealed class GraficosIndFile
{
    // VB6: Desc As String * 255 + CRC As Long + MagicWord As Long.
    // El registro ocupa 263 bytes; después vienen cinco Integer reservados.
    private const int HeaderSize = 263;
    private const int ReservedIntegerCount = 5;
    public List<GrhRecord> Records { get; } = new();
    private byte[] PrefixBytes { get; set; } = Array.Empty<byte>();

    public static GraficosIndFile Load(string path, GrhStorageFormat format)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        var result = new GraficosIndFile();
        var recordStart = HeaderSize + ReservedIntegerCount * 2;
        if (stream.Length < recordStart + 2)
            throw new InvalidDataException("El archivo no contiene una cabecera completa.");

        stream.Position = 0;
        result.PrefixBytes = reader.ReadBytes(recordStart);
        stream.Position = recordStart;
        while (stream.Position < stream.Length)
        {
            var idSize = format == GrhStorageFormat.LongIndexes ? 4 : 2;
            if (stream.Length - stream.Position < idSize) break;
            var id = format == GrhStorageFormat.LongIndexes ? reader.ReadInt32() : reader.ReadInt16();
            if (id <= 0) break;
            var record = new GrhRecord { Id = id };
            if (stream.Length - stream.Position < 2)
                throw new InvalidDataException($"GRH {id}: falta el número de frames.");
            var count = reader.ReadInt16();
            if (count <= 0 || count > 25) throw new InvalidDataException($"GRH {id}: frames inválidos ({count}).");
            if (count > 1)
            {
                var frameSize = format == GrhStorageFormat.LongIndexes ? 4 : 2;
                if (stream.Length - stream.Position < count * frameSize + 2)
                    throw new InvalidDataException($"GRH {id}: registro de animación incompleto.");
                for (var i = 0; i < count; i++) record.Frames.Add(format == GrhStorageFormat.LongIndexes ? reader.ReadInt32() : reader.ReadInt16());
                record.Speed = reader.ReadInt16();
            }
            else
            {
                if (stream.Length - stream.Position < 10)
                    throw new InvalidDataException($"GRH {id}: registro incompleto.");
                record.FileNumber = reader.ReadInt16(); record.X = reader.ReadInt16(); record.Y = reader.ReadInt16();
                record.Width = reader.ReadInt16(); record.Height = reader.ReadInt16(); record.Frames.Add(id);
            }
            result.Records.Add(record);
        }
        return result;
    }

    public void Save(string path, GrhStorageFormat format)
    {
        using var stream = File.Create(path);
        using var writer = new BinaryWriter(stream);
        if (PrefixBytes.Length == HeaderSize + ReservedIntegerCount * 2)
            writer.Write(PrefixBytes);
        else
        {
            writer.Write(new byte[HeaderSize]);
            for (var i = 0; i < ReservedIntegerCount; i++) writer.Write((short)0);
        }
        foreach (var record in Records.OrderBy(x => x.Id))
        {
            if (format == GrhStorageFormat.LongIndexes) writer.Write(record.Id);
            else { if (record.Id > short.MaxValue) throw new InvalidDataException($"GRH {record.Id} no cabe en Integer."); writer.Write((short)record.Id); }
            var frames = record.IsAnimation ? record.Frames : new List<int> { record.Id };
            writer.Write((short)frames.Count);
            if (frames.Count > 1)
            {
                foreach (var frame in frames)
                {
                    if (format == GrhStorageFormat.LongIndexes) writer.Write(frame);
                    else { if (frame > short.MaxValue) throw new InvalidDataException($"Frame {frame} no cabe en Integer."); writer.Write((short)frame); }
                }
                writer.Write(record.Speed <= 0 ? (short)1 : record.Speed);
            }
            else { writer.Write(record.FileNumber); writer.Write(record.X); writer.Write(record.Y); writer.Write(record.Width); writer.Write(record.Height); }
        }
        if (format == GrhStorageFormat.LongIndexes) writer.Write(0); else writer.Write((short)0);
    }
}
