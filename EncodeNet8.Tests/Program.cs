using EncodeNet8;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

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

    if (args.Length > 0 && args[0].Equals("--additional", StringComparison.OrdinalIgnoreCase))
    {
        var fx = FxIndexFile.LoadAuto(args[1]);
        var weapons = IniDirectionalIndexFile.Load(args[2], "Armas");
        var shields = IniDirectionalIndexFile.Load(args[3], "Escudos");
        Console.WriteLine($"{args[1]}: {fx.Count} FX ocupados");
        Console.WriteLine($"{args[2]}: {weapons.Count} armas ocupadas");
        Console.WriteLine($"{args[3]}: {shields.Count} escudos ocupados");
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
    TestAdditionalReaders(root);
    TestAutoIndexer(root);
    TestFormAutoModes(root);
    Console.WriteLine("Graficos.ind, lectores direccionales y adicionales: Integer/Long OK; truncado rechazado.");
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
        var roundtrip = Path.Combine(root, $"Personajes-roundtrip-{format}.ind");
        DirectionalIndexFile.Save(roundtrip, records, format, includesOffsets: true);
        var reopened = DirectionalIndexFile.Load(roundtrip, format, includesOffsets: true);
        Assert(reopened.Count == 2 && reopened[0].Directions[0] == 10, $"guardado directional {format}");
    }
}

static void WriteIndex(BinaryWriter writer, int value, GrhStorageFormat format)
{
    if (format == GrhStorageFormat.LongIndexes) writer.Write(value);
    else writer.Write((short)value);
}

static void TestAdditionalReaders(string root)
{
    var fxPath = Path.Combine(root, "Fxs.ind");
    using (var stream = File.Create(fxPath))
    using (var writer = new BinaryWriter(stream))
    {
        writer.Write(new byte[263]);
        writer.Write((short)2);
        writer.Write((short)20);
        writer.Write((short)1);
        writer.Write((short)-2);
        writer.Write((short)0);
        writer.Write((short)0);
        writer.Write((short)0);
    }
    var fx = FxIndexFile.Load(fxPath, GrhStorageFormat.LegacyInteger);
    Assert(fx.Count == 1 && fx[0].Animation == 20 && fx[0].OffsetX == 1, "lector FX");
    var fxRoundtrip = Path.Combine(root, "Fxs-roundtrip.ind");
    FxIndexFile.Save(fxRoundtrip, fx, GrhStorageFormat.LegacyInteger);
    Assert(FxIndexFile.Load(fxRoundtrip, GrhStorageFormat.LegacyInteger).Count == 1, "guardado FX");

    var weaponPath = Path.Combine(root, "Armas.dat");
    File.WriteAllText(weaponPath, "[INIT]\nNumArmas=3\n\n[ARMA1]\nDir1=10\nDir2=11\nDir3=12\nDir4=13\n\n[ARMA3]\nDir1=20\nDir2=21\nDir3=22\nDir4=23\n");
    var weapons = IniDirectionalIndexFile.Load(weaponPath, "Armas");
    Assert(weapons.Count == 2 && weapons[1].Directions[3] == 23, "lector de armas INI");

    var bootsPath = Path.Combine(root, "Botas.dat");
    File.WriteAllText(bootsPath, "[INIT]\nNumBotas=2\n\n[Bota1]\nBota1=30\nBota2=31\nBota3=32\nBota4=33\n");
    var boots = IniDirectionalIndexFile.Load(bootsPath, "Botas");
    Assert(boots.Count == 1 && boots[0].Directions[2] == 32, "lector de botas INI");
    var bootsRoundtrip = Path.Combine(root, "Botas-roundtrip.dat");
    IniDirectionalIndexFile.Save(bootsRoundtrip, boots, "Botas");
    Assert(IniDirectionalIndexFile.Load(bootsRoundtrip, "Botas").Count == 1, "guardado INI direccional");
}

static void TestAutoIndexer(string root)
{
    var bmpPath = Path.Combine(root, "700.bmp");
    using (var bitmap = new Bitmap(64, 32)) bitmap.Save(bmpPath);
    var options = new AutoIndexerOptions
    {
        BitmapPath = bmpPath,
        StartGrh = 10,
        Columns = 2,
        Rows = 1,
        CellWidth = 32,
        CellHeight = 32,
        CreateAnimation = true,
        AnimationSpeed = 2
    };
    var records = AutoIndexer.CreateGrid(options, 100);
    Assert(records.Count == 3, "autoindexador: cantidad de GRH");
    Assert(records[0].FileNumber == 700 && records[1].X == 32, "autoindexador: recortes");
    Assert(records[2].IsAnimation && records[2].Frames.SequenceEqual(new[] { 10, 11 }), "autoindexador: animación");
}

static void TestFormAutoModes(string root)
{
    var bitmapPath = Path.Combine(root, "700.bmp");
    using (var bitmap = new Bitmap(156, 184)) bitmap.Save(bitmapPath);

    RunSta(() =>
    {
        using var normal = new FormAuto(root, 1000);
        normal.Show();
        Application.DoEvents();
        Get<Button>(normal, "Command4").PerformClick();
        var normalResourcePreviewBmp = 0;
        normal.ResourcePreviewChanged += bmp => normalResourcePreviewBmp = bmp;
        Get<TextBox>(normal, "textBox7").Text = "700";
        Assert(normalResourcePreviewBmp == 700, "FormAuto avanzado: aviso de recurso para MainForm");
        Get<TextBox>(normal, "textBox4").Text = "1";
        Get<Button>(normal, "Command1").PerformClick();
        var classic = normal.GeneratedRecords ?? throw new InvalidOperationException("Fallo: FormAuto no generó la animación clásica.");
        Assert(classic.Count == 26, "FormAuto clásico: cantidad de GRH");
        Assert(classic[0].X == 0 && classic[0].Y == 0 && classic[0].Frames.SequenceEqual(new[] { 1 }), "FormAuto clásico: primer frame VB6");
        Assert(classic[1].X == 25 && classic[6].Y == 45, "FormAuto clásico: posiciones VB6");
        Assert(classic[^4].Frames.Count == 6 && classic[^2].Frames.Count == 5, "FormAuto clásico: grupos direccionales");
        var graphicsRoundtrip = Path.Combine(root, "FormAuto-Graficos.ind");
        var graphicsFile = new GraficosIndFile();
        graphicsFile.Records.AddRange(classic);
        graphicsFile.Save(graphicsRoundtrip, GrhStorageFormat.LongIndexes);
        var reopenedGraphics = GraficosIndFile.Load(graphicsRoundtrip, GrhStorageFormat.LongIndexes);
        Assert(reopenedGraphics.Records.Count == 26 && reopenedGraphics.Records[^1].Frames.Count == 5, "FormAuto clásico: guardado de GRH");
        normal.Close();

        using var body = new FormAuto(root, 1000);
        body.Show();
        Application.DoEvents();
        Get<Button>(body, "Command4").PerformClick();
        Get<TextBox>(body, "textBox7").Text = "700";
        Get<TextBox>(body, "textBox4").Text = "1";
        Get<CheckBox>(body, "checkAuto").Checked = true;
        Get<TextBox>(body, "text1").Text = "900";
        Get<Button>(body, "Command1").PerformClick();
        var bodyIndex = body.GeneratedDirectionalIndex ?? throw new InvalidOperationException("Fallo: FormAuto no generó el enlace direccional.");
        Assert(bodyIndex.Directions.SequenceEqual(new[] { 24, 26, 23, 25 }), "FormAuto Body: mapeo direccional VB6");
        var bodyPath = Path.Combine(root, "Personajes-formAuto.ind");
        DirectionalIndexFile.Save(bodyPath, new[] { bodyIndex }, GrhStorageFormat.LegacyInteger, includesOffsets: true);
        var reopenedBody = DirectionalIndexFile.Load(bodyPath, GrhStorageFormat.LegacyInteger, includesOffsets: true);
        Assert(reopenedBody.Count == 1 && reopenedBody[0].Directions.SequenceEqual(bodyIndex.Directions), "FormAuto Body: guardado direccional");
        body.Close();

        using var offset = new FormAuto(root, 1000);
        offset.Show();
        Application.DoEvents();
        Get<Button>(offset, "Command4").PerformClick();
        Get<ComboBox>(offset, "ComboTipoAnim").SelectedIndex = 1;
        Get<TextBox>(offset, "textBox7").Text = "700";
        Get<TextBox>(offset, "textBox4").Text = "1";
        Get<TextBox>(offset, "textDatos2_0").Text = "2";
        Get<TextBox>(offset, "textDatos2_1").Text = "2";
        Get<TextBox>(offset, "textDatos2_3").Text = "4";
        Get<TextBox>(offset, "textBox5").Text = "32";
        Get<TextBox>(offset, "textBox6").Text = "32";
        Get<TextBox>(offset, "TextDatos2_7").Text = "3";
        Get<TextBox>(offset, "TextDatos2_8").Text = "4";
        Get<Button>(offset, "Command1").PerformClick();
        var offsetRecords = offset.GeneratedRecords ?? throw new InvalidOperationException("Fallo: FormAuto no generó el offset.");
        Assert(offsetRecords.Count == 5 && offsetRecords[0].X == 3 && offsetRecords[0].Y == 4, "FormAuto offset: recortes");
        Assert(offsetRecords[^1].Frames.SequenceEqual(new[] { 1, 2, 3, 4 }), "FormAuto offset: animación");
        offset.Close();

        using var simple = new FormAuto(root, 1000);
        simple.Show();
        Application.DoEvents();
        Get<Button>(simple, "ISimple").PerformClick();
        var resourcePreviewBmp = 0;
        simple.ResourcePreviewChanged += bmp => resourcePreviewBmp = bmp;
        Get<TextBox>(simple, "textBoxBmp").Text = "700";
        Assert(resourcePreviewBmp == 700, "FormAuto simple: aviso de recurso para MainForm");
        Get<TextBox>(simple, "textBoxGrh").Text = "1";
        Get<Button>(simple, "BtnIndexar").PerformClick();
        var simpleRecords = simple.GeneratedRecords ?? throw new InvalidOperationException("Fallo: FormAuto no generó el índice simple.");
        Assert(simpleRecords.Count == 1 && simpleRecords[0].FileNumber == 700, "FormAuto simple: índice");
        simple.Close();
    });
}

static T Get<T>(Form form, string name) where T : Control
{
    var field = typeof(FormAuto).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Fallo: no existe el control {name}.");
    return (T)(field.GetValue(form) ?? throw new InvalidOperationException($"Fallo: control {name} no inicializado."));
}

static void RunSta(Action action)
{
    Exception? error = null;
    var thread = new Thread(() =>
    {
        try { action(); }
        catch (Exception ex) { error = ex; }
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();
    if (error is not null) throw error;
}
