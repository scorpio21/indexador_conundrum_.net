namespace EncodeNet8;

public sealed class GraficosIndFile
{
    // VB6: String * 255 + Long + Long. Después hay cinco Integer reservados.
    private const int HeaderSize = 263;
    private const int ReservedIntegerCount = 5;
    private const int PrefixSize = HeaderSize + ReservedIntegerCount * sizeof(short);
    private const int MaxFrames = 25;

    public List<GrhRecord> Records { get; } = new();
    private byte[] PrefixBytes { get; set; } = Array.Empty<byte>();

    public static GraficosIndFile Load(string path, GrhStorageFormat format)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        var result = new GraficosIndFile();

        if (stream.Length < PrefixSize)
            throw new InvalidDataException("El archivo no contiene una cabecera completa de Graficos.ind.");

        result.PrefixBytes = ReadExactly(reader, PrefixSize, "la cabecera");
        var idSize = format == GrhStorageFormat.LongIndexes ? sizeof(int) : sizeof(short);
        var frameSize = idSize;
        var ids = new HashSet<int>();
        var previousId = 0;
        var recordBoundaryFound = false;

        while (stream.Position < stream.Length)
        {
            EnsureRemaining(stream, idSize, "el identificador GRH");
            var id = format == GrhStorageFormat.LongIndexes ? reader.ReadInt32() : reader.ReadInt16();

            if (id == 0)
            {
                if (stream.Position != stream.Length)
                    throw new InvalidDataException("Graficos.ind contiene datos después del terminador.");
                recordBoundaryFound = true;
                break;
            }

            if (id < 0)
                throw new InvalidDataException($"GRH inválido ({id}).");
            if (!ids.Add(id))
                throw new InvalidDataException($"GRH duplicado ({id}).");
            if (id <= previousId)
                throw new InvalidDataException($"Los GRH no están ordenados: {previousId} seguido de {id}.");
            previousId = id;

            EnsureRemaining(stream, sizeof(short), $"el número de frames del GRH {id}");
            var count = reader.ReadInt16();
            if (count <= 0 || count > MaxFrames)
                throw new InvalidDataException($"GRH {id}: frames inválidos ({count}).");

            var record = new GrhRecord { Id = id };
            if (count > 1)
            {
                EnsureRemaining(stream, checked(count * frameSize + sizeof(short)), $"la animación del GRH {id}");
                for (var i = 0; i < count; i++)
                {
                    var frame = format == GrhStorageFormat.LongIndexes ? reader.ReadInt32() : reader.ReadInt16();
                    if (frame <= 0)
                        throw new InvalidDataException($"GRH {id}: frame inválido ({frame}).");
                    record.Frames.Add(frame);
                }

                record.Speed = reader.ReadInt16();
                if (record.Speed <= 0)
                    throw new InvalidDataException($"GRH {id}: velocidad inválida ({record.Speed}).");
            }
            else
            {
                EnsureRemaining(stream, 5 * sizeof(short), $"los datos del GRH {id}");
                record.FileNumber = reader.ReadInt16();
                record.X = reader.ReadInt16();
                record.Y = reader.ReadInt16();
                record.Width = reader.ReadInt16();
                record.Height = reader.ReadInt16();
                record.Frames.Add(id);
            }

            result.Records.Add(record);
            recordBoundaryFound = true;
        }

        if (!recordBoundaryFound)
            throw new InvalidDataException("Graficos.ind termina dentro de un registro.");

        return result;
    }

    public void Save(string path, GrhStorageFormat format)
    {
        using var stream = File.Create(path);
        using var writer = new BinaryWriter(stream);
        if (PrefixBytes.Length == PrefixSize)
            writer.Write(PrefixBytes);
        else
            writer.Write(new byte[PrefixSize]);

        var ids = new HashSet<int>();
        var previousId = 0;
        foreach (var record in Records.OrderBy(x => x.Id))
        {
            if (record.Id <= 0 || !ids.Add(record.Id))
                throw new InvalidDataException($"GRH inválido o duplicado ({record.Id}).");
            if (record.Id <= previousId)
                throw new InvalidDataException($"Los GRH no están ordenados: {previousId} seguido de {record.Id}.");
            previousId = record.Id;

            WriteIndex(writer, record.Id, format, $"GRH {record.Id}");
            var frames = record.IsAnimation ? record.Frames : new List<int> { record.Id };
            if (frames.Count <= 0 || frames.Count > MaxFrames)
                throw new InvalidDataException($"GRH {record.Id}: frames inválidos ({frames.Count}).");

            writer.Write((short)frames.Count);
            if (frames.Count > 1)
            {
                foreach (var frame in frames) WriteIndex(writer, frame, format, $"Frame del GRH {record.Id}");
                if (record.Speed <= 0 || record.Speed > short.MaxValue)
                    throw new InvalidDataException($"GRH {record.Id}: velocidad inválida ({record.Speed}).");
                writer.Write(record.Speed);
            }
            else
            {
                writer.Write(record.FileNumber);
                writer.Write(record.X);
                writer.Write(record.Y);
                writer.Write(record.Width);
                writer.Write(record.Height);
            }
        }

        // El escritor VB6 termina al acabar el último registro; no añade un GRH 0.
    }

    private static void WriteIndex(BinaryWriter writer, int value, GrhStorageFormat format, string label)
    {
        if (value <= 0) throw new InvalidDataException($"{label}: índice inválido ({value}).");
        if (format == GrhStorageFormat.LongIndexes) writer.Write(value);
        else
        {
            if (value > short.MaxValue) throw new InvalidDataException($"{label} {value} no cabe en Integer.");
            writer.Write((short)value);
        }
    }

    private static byte[] ReadExactly(BinaryReader reader, int count, string description)
    {
        var bytes = reader.ReadBytes(count);
        if (bytes.Length != count) throw new InvalidDataException($"Faltan bytes en {description}.");
        return bytes;
    }

    private static void EnsureRemaining(Stream stream, long required, string description)
    {
        if (stream.Length - stream.Position < required)
            throw new InvalidDataException($"Faltan bytes para {description}.");
    }
}
