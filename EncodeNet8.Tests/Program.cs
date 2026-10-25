using EncodeNet8;

var root = Path.Combine(Path.GetTempPath(), "EncodeNet8.Tests", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
try
{
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
    Console.WriteLine("Graficos.ind: Integer y Long OK; truncado rechazado.");
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
