using EncodeNet8;
using System.Reflection;

var root = Path.Combine(Path.GetTempPath(), "EncodeNet8.Tests", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
try
{
    if (args.Length > 0 && args[0].Equals("--directional", StringComparison.OrdinalIgnoreCase))
    {
        foreach (var path in args.Skip(1))
        {
            var name = Path.GetFileName(path);
            var includesOffsets = name.StartsWith("Personajes", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("Alas", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("Botas", StringComparison.OrdinalIgnoreCase);
            var records = DirectionalIndexFile.LoadAuto(path, includesOffsets);
            Console.WriteLine($"{path}: {records.Count} índices direccionales ocupados");
        }
        return;
    }

    if (args.Length > 0)
    {
        foreach (var path in args)
        {
            var detected = new List<string>();
            foreach (var format in new[] { GrhStorageFormat.LegacyInteger, GrhStorageFormat.LongIndexes })
            {
                try
                {
                    var file = GraficosIndFile.Load(path, format);
                    detected.Add($"{format} ({file.Records.Count} GRH)");
                }
                catch (InvalidDataException)
                {
                }
            }

            if (detected.Count == 0) throw new InvalidDataException($"No se pudo leer: {path}");
            Console.WriteLine($"{path}: {string.Join("; ", detected)}");
        }
        return;
    }

    foreach (var format in new[] { GrhStorageFormat.LegacyInteger, GrhStorageFormat.LongIndexes })
    {
        var animationId = format == GrhStorageFormat.LongIndexes ? 40000 : 30000;
        var source = new GraficosIndFile();
        source.Records.Add(new GrhRecord { Id = 1, FileNumber = 2, X = 3, Y = 4, Width = 32, Height = 32 });
        source.Records.Add(new GrhRecord { Id = animationId, Frames = new List<int> { 1, animationId, 3 }, Speed = 6 });
        var path = Path.Combine(root, $"Graficos-{format}.ind");

        source.Save(path, format);
        var loaded = GraficosIndFile.Load(path, format);
        Assert(loaded.Records.Count == 2, $"{format}: número de registros");
        Assert(loaded.Records[0].FileNumber == 2 && loaded.Records[0].Width == 32, $"{format}: datos normales");
        Assert(loaded.Records[1].Frames.SequenceEqual(new[] { 1, animationId, 3 }), $"{format}: frames");
        Assert(loaded.Records[1].Speed == 6, $"{format}: velocidad");
    }

    var invalid = Path.Combine(root, "truncado.ind");
    File.WriteAllBytes(invalid, new byte[273]);
    ExpectInvalid(() => GraficosIndFile.Load(invalid, GrhStorageFormat.LegacyInteger));
    TestStateSlotReaders(root);
    TestDirectionalReader(root);
    Console.WriteLine("Graficos.ind y lectores direccionales: Integer/Long OK; truncado rechazado.");
}
finally
{
    Directory.Delete(root, recursive: true);
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException($"Fallo: {message}");
}

static void ExpectInvalid(Action action)
{
    try
    {
        action();
        throw new InvalidOperationException("Fallo: se aceptó un archivo inválido.");
    }
    catch (InvalidDataException)
    {
    }
}

static void TestStateSlotReaders(string root)
{
    var binaryPath = Path.Combine(root, "Cabezas.ind");
    using (var stream = File.Create(binaryPath))
    using (var writer = new BinaryWriter(stream))
    {
        writer.Write(new byte[263]);
        writer.Write((short)2);
        writer.Write((short)10);
        writer.Write((short)0);
        writer.Write((short)0);
        writer.Write((short)0);
        writer.Write((short)0);
        writer.Write((short)0);
        writer.Write((short)0);
        writer.Write((short)0);
    }

    var iniPath = Path.Combine(root, "Armas.dat");
    File.WriteAllText(iniPath, "[INIT]\nNumArmas=3\n\n[Arma1]\nDir1=4719\n\n[ARMA2]\nDir1=0\n\n[ARMA3]\nDir1=4723 'norte\n");
    var method = typeof(MainForm).GetMethod("ReadOccupiedBinarySlots", BindingFlags.NonPublic | BindingFlags.Static)!;
    var binary = (List<int>)method.Invoke(null, new object[] { binaryPath, new[] { 8 } })!;
    Assert(binary.SequenceEqual(new[] { 1 }), "lista binaria de índices ocupados");
    var iniMethod = typeof(MainForm).GetMethod("ReadOccupiedIniSlots", BindingFlags.NonPublic | BindingFlags.Static)!;
    var ini = (List<int>)iniMethod.Invoke(null, new object[] { iniPath, "Armas" })!;
    Assert(ini.SequenceEqual(new[] { 1, 3 }), "lista INI de índices ocupados");
}

static void TestDirectionalReader(string root)
{
    foreach (var format in new[] { GrhStorageFormat.LegacyInteger, GrhStorageFormat.LongIndexes })
    {
        var path = Path.Combine(root, $"Personajes-{format}.ind");
        using (var stream = File.Create(path))
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(new byte[263]);
            writer.Write((short)2);
            WriteIndex(writer, 10, format);
            WriteIndex(writer, 11, format);
            WriteIndex(writer, 12, format);
            WriteIndex(writer, 13, format);
            writer.Write((short)-1);
            writer.Write((short)2);
            WriteIndex(writer, format == GrhStorageFormat.LongIndexes ? 40000 : 14, format);
            WriteIndex(writer, 15, format);
            WriteIndex(writer, 16, format);
            WriteIndex(writer, 17, format);
            writer.Write((short)3);
            writer.Write((short)4);
        }

        var records = DirectionalIndexFile.Load(path, format, includesOffsets: true);
        Assert(records.Count == 2 && records[1].Directions[3] == 17, $"lector directional {format}");
        Assert(records[1].OffsetX == 3 && records[1].OffsetY == 4, $"offsets directional {format}");
    }
}

static void WriteIndex(BinaryWriter writer, int value, GrhStorageFormat format)
{
    if (format == GrhStorageFormat.LongIndexes) writer.Write(value);
    else writer.Write((short)value);
}
