using EncodeNet8;
using System.Drawing;
using System.Drawing.Imaging;
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
    TestWeIndexFile(root);
    TestIndexerSettings(root);
    TestMainFormStates();
    TestAllIndexerStates();
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
        var autodetected = DirectionalIndexFile.LoadAuto(path, includesOffsets: true);
        Assert(autodetected.Count == 2 && autodetected[1].Directions[3] == 17, $"autodetección directional {format}");
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
    Assert(FxIndexFile.LoadAuto(fxPath).Count == 1, "autodetección FX");
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

        Get<Button>(normal, "Command3").PerformClick();
        Get<RadioButton>(normal, "radioButton5").PerformClick();
        Assert(!Get<TextBox>(normal, "textBox21").Enabled && !Get<TextBox>(normal, "textBox20").Enabled,
            "FormAuto normal: BMP consecutivos deshabilita la cuadrícula");
        Assert(Get<TextBox>(normal, "textBox21").Text == "1" && Get<TextBox>(normal, "textBox20").Text == "1",
            "FormAuto normal: BMP consecutivos restablece 1x1");
        Get<RadioButton>(normal, "radioButton6").PerformClick();
        Get<TextBox>(normal, "textBox21").Text = "6";
        Get<TextBox>(normal, "textBox20").Text = "4";
        Get<TextBox>(normal, "textBox15").Text = "700";
        Get<Button>(normal, "button4").PerformClick();
        Assert(Get<TextBox>(normal, "textBox17").Text == "46" && Get<TextBox>(normal, "textBox16").Text == "26",
            "FormAuto normal: AutoCalcular divide el BMP");
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

        Get<RadioButton>(body, "Optiondimension_1").Checked = true;
        Application.DoEvents();
        Assert(Get<TextBox>(body, "textDatos2_3").Text == "24" && Get<ComboBox>(body, "combo2").SelectedIndex == 0,
            $"FormAuto avanzado: plantilla NPC fija 4 direcciones (total={Get<TextBox>(body, "textDatos2_3").Text}, combo={Get<ComboBox>(body, "combo2").SelectedIndex}, npc={Get<RadioButton>(body, "Optiondimension_1").Checked}, clasico={Get<RadioButton>(body, "Optiondimension_0").Checked})");
        Assert(Get<TextBox>(body, "Text2").Text == "0º0", "FormAuto avanzado: NPC usa offset cero");
        Get<RadioButton>(body, "Optiondimension_2").Checked = true;
        Application.DoEvents();
        Assert(Get<TextBox>(body, "textBox5").Text == "45" && Get<TextBox>(body, "textBox6").Text == "25",
            "FormAuto avanzado: plantilla Alkon conserva dimensiones");
        Assert(Get<TextBox>(body, "Text2").Text == "-38º0", "FormAuto avanzado: Alkon conserva offset clásico");
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
        Get<TextBox>(offset, "Text2").Text = "999999º-999999";
        Assert(Get<TextBox>(offset, "Text2").Text == "1000º-1000", "FormAuto offset: normaliza límites");
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

static void TestMainFormStates()
{
    RunSta(() =>
    {
        using var main = new MainForm();
        main.Show();
        Application.DoEvents();
        var deleteMenu = GetMainValue<ToolStripMenuItem>(main, "borrarMenu");
        Assert(deleteMenu.ShortcutKeys == Keys.None && deleteMenu.ShortcutKeyDisplayString == "Supr",
            "MainForm Edición: Supr no secuestra la edición de TextBox");
        var changeState = typeof(MainForm).GetMethod("CambiarEstado", BindingFlags.Instance | BindingFlags.NonPublic)!;

        changeState.Invoke(main, new object[] { "Resource" });
        Assert(!GetMain<ComboBox>(main, "directionCombo").Visible && !GetMain<TextBox>(main, "framesText").Visible,
            "MainForm Recursos: oculta frames y selector direccional");
        Assert(GetMain<Label>(main, "grhLabel").Text == "Crypt:" && GetMain<Label>(main, "bmpLabel").Text == "Tamaño:",
            "MainForm Recursos: etiquetas VB6");
        Assert(GetMain<Label>(main, "DescripcionAyuda").Visible && !GetMain<Button>(main, "deleteButton").Visible,
            "MainForm Recursos: ayuda visible y borrado oculto");

        changeState.Invoke(main, new object[] { "Body" });
        Assert(GetMain<ComboBox>(main, "directionCombo").Visible && !GetMain<TextBox>(main, "framesText").Visible,
            "MainForm Body: selector visible y frames oculto");
        Assert(GetMain<TextBox>(main, "speedText").Visible && !GetMain<TextBox>(main, "yText").Visible,
            "MainForm Body: offset visible y posición Y oculta");
        var walkCombo = GetMain<ComboBox>(main, "directionCombo");
        Assert(walkCombo.Items.Cast<object>().Select(item => item.ToString()).SequenceEqual(new[] { "Todos", "Norte", "Este", "Sur", "Oeste" }),
            "MainForm Body: selector CDibujarWalk conserva Todos y las cuatro direcciones");
        walkCombo.SelectedIndex = 1;
        Assert(GetMainValue<int>(main, "_drawWalk") == 1, "MainForm Body: Norte selecciona una sola dirección");
        walkCombo.SelectedIndex = 0;
        Assert(GetMainValue<int>(main, "_drawWalk") == 0, "MainForm Body: Todos selecciona las cuatro direcciones");
        var checkCabeza = GetMain<CheckBox>(main, "Checkcabeza");
        Assert(checkCabeza.Visible && GetMain<Label>(main, "xLabel").Text == "Offset:",
            "MainForm Body: Checkcabeza visible y Posicion X sustituida por Offset");
        checkCabeza.Checked = true;
        Assert(GetMainValue<int>(main, "cabezaActual") == 3008,
            "MainForm Body: Checkcabeza asigna la cabeza 3008");
        checkCabeza.Checked = false;
        Assert(GetMainValue<int>(main, "cabezaActual") == 0,
            "MainForm Body: desmarcar Checkcabeza limpia cabezaActual");
        GetMain<TextBox>(main, "xText").Text = "-3.0";
        Assert(GetMainValue<short>(main, "_previewHeadOffsetY") == -3 && GetMainValue<short>(main, "_previewHeadOffsetX") == 0,
            "MainForm Body: TextDatos(5) actualiza el offset Y/X del preview");
        GetMain<TextBox>(main, "xText").Text = "-38º0";

        var directional = GetMainList<DirectionalIndexRecord>(main, "_directionalRecords");
        directional.Clear();
        directional.Add(new DirectionalIndexRecord { Id = 7, Directions = new[] { 10, 11, 12, 13 }, OffsetX = 3, OffsetY = 4 });
        InvokeMain(main, "RefreshDirectionalList");
        GetMain<ListBox>(main, "indexList").SelectedIndex = 0;
        Assert(GetMain<TextBox>(main, "bmpText").Text == "10" && GetMain<TextBox>(main, "heightText").Text == "11"
            && GetMain<TextBox>(main, "widthText").Text == "12" && GetMain<TextBox>(main, "speedText").Text == "13"
            && GetMain<TextBox>(main, "xText").Text == "4º3",
            "MainForm Body: ClickEnLista conserva el mapeo VB6 de direcciones y offset");

        changeState.Invoke(main, new object[] { "Cabezas" });
        Assert(GetMain<Label>(main, "grhLabel").Text == "Cabezas:" && !GetMain<TextBox>(main, "xText").Visible,
            "MainForm Cabezas: etiquetas y campos exactos");

        changeState.Invoke(main, new object[] { "Fx" });
        Assert(GetMain<Label>(main, "heightLabel").Text == "Offset:" && !GetMain<TextBox>(main, "framesText").Visible,
            "MainForm FX: offset en TextDatos(2)");

        var graphicsPath = Path.Combine(Path.GetTempPath(), "EncodeNet8.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(graphicsPath);
        var bitmapPath = Path.Combine(graphicsPath, "700.bmp");
        using (var bitmap = new Bitmap(3, 4)) bitmap.Save(bitmapPath, ImageFormat.Bmp);
        var settings = GetMainValue<IndexerSettings>(main, "_settings");
        settings.GraphicsPath = graphicsPath;
        changeState.Invoke(main, new object[] { "Resource" });
        var searchResults = GetMainList<int>(main, "_searchResults");
        searchResults.Clear(); searchResults.Add(700);
        var resourceMode = typeof(MainForm).GetField("_resourceSearchMode", BindingFlags.Instance | BindingFlags.NonPublic)!;
        resourceMode.SetValue(main, true);
        var resourceList = GetMain<ListBox>(main, "indexList");
        resourceList.Items.Clear(); resourceList.Items.Add(700); resourceList.SelectedIndex = 0;
        InvokeMain(main, "ClickResourceSearchItem");
        Assert(GetMain<TextBox>(main, "heightText").Text == "4" && GetMain<TextBox>(main, "widthText").Text == "3"
            && GetMain<TextBox>(main, "speedText").Text == "32" && GetMain<TextBox>(main, "xText").Text == "BMP",
            "MainForm Recursos: BMP muestra tamaño, dimensiones, bits y situación");

        var graphicsFile = GetMainValue<GraficosIndFile>(main, "_file");
        graphicsFile.Records.Clear();
        graphicsFile.Records.Add(new GrhRecord { Id = 1, FileNumber = 700, Width = 3, Height = 4, Frames = new List<int> { 1 } });
        changeState.Invoke(main, new object[] { "Grh" });
        var grhList = GetMain<ListBox>(main, "indexList");
        grhList.SelectedIndex = 0;
        GetMain<TextBox>(main, "bmpText").Text = "999";
        Assert(GetMain<TextBox>(main, "bmpText").BackColor == Color.Red && GetMain<Label>(main, "LUlitError").Text.Length > 0,
            "MainForm Validación: BMP inexistente en rojo y mensaje LUlitError");
        GetMain<TextBox>(main, "bmpText").Text = "700";
        Assert(GetMain<TextBox>(main, "bmpText").BackColor == Color.White,
            "MainForm Validación: campo restaurado a blanco al corregirse");
        GetMain<TextBox>(main, "framesText").Text = "2";
        Assert(GetMain<TextBox>(main, "framesText").BackColor == Color.Red,
            "MainForm Validación: cantidad de frames incoherente en rojo");
        GetMain<TextBox>(main, "framesText").Text = "1";

        GetMain<TextBox>(main, "frameText").Text = "1-2-3";
        GetMain<TextBox>(main, "framesText").Text = "2";
        Assert(GetMain<TextBox>(main, "framesText").BackColor == Color.Red,
            "MainForm Validación: frames sobrantes no provocan excepción");
        GetMain<TextBox>(main, "frameText").Text = "1";
        GetMain<TextBox>(main, "framesText").Text = "1";

        var selectedField = typeof(MainForm).GetField("_selectedRecord", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var advance = typeof(MainForm).GetMethod("AdvanceAnimationFrame", BindingFlags.Instance | BindingFlags.NonPublic)!;
        foreach (var frameCount in new[] { 4, 15, 25 })
        {
            var animation = new GrhRecord { Id = 1000 + frameCount, Speed = 1, Frames = Enumerable.Range(1, frameCount).ToList() };
            selectedField.SetValue(main, animation);
            typeof(MainForm).GetMethod("ConfigureAnimationTimer", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(main, new object[] { animation });
            advance.Invoke(main, new object[] { 50L });
            var currentFrame = GetMainValue<int>(main, "_animationFrame");
            Assert(currentFrame >= 0 && currentFrame < frameCount, $"Animación: frame dentro de rango para {frameCount} frames");
        }
        selectedField.SetValue(main, new GrhRecord { Id = 999, Speed = 1, Frames = new List<int> { 999, 1000 } });
        var preview = GetMain<BufferedPreviewPanel>(main, "previewPanel");
        using (var canvas = new Bitmap(120, 120))
        using (var graphics = Graphics.FromImage(canvas))
        {
            var paint = typeof(MainForm).GetMethod("previewPanel_Paint", BindingFlags.Instance | BindingFlags.NonPublic)!;
            paint.Invoke(main, new object[] { preview, new PaintEventArgs(graphics, new Rectangle(0, 0, 120, 120)) });
        }
        Assert(GetMain<Label>(main, "LUlitError").Text.Length > 0,
            "Visor: frame inexistente se informa sin excepción ni índice fuera de rango");

        var backupMethod = typeof(MainForm).GetMethod("BackupExistingFile", BindingFlags.Static | BindingFlags.NonPublic)!;
        var backupSource = Path.Combine(graphicsPath, "graficos.ind");
        File.WriteAllText(backupSource, "contenido anterior");
        var backupPath = (string?)backupMethod.Invoke(null, new object[] { backupSource });
        Assert(backupPath == backupSource + ".bak" && File.ReadAllText(backupPath!) == "contenido anterior",
            "Edición: crea una copia .bak recuperable antes de sobrescribir");

        var toggleWeMode = typeof(MainForm).GetMethod("ToggleWeMode", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var surfaceGroup = GetMain<GroupBox>(main, "surfaceGroup");
        var generateSurfaceButton = GetMain<Button>(main, "generateSurfaceButton");
        var capasButton = GetMain<Button>(main, "capasButton");
        Assert(!surfaceGroup.Visible && !generateSurfaceButton.Visible,
            "WE: panel y botón de generación empiezan ocultos");
        toggleWeMode.Invoke(main, null);
        Assert(surfaceGroup.Visible && generateSurfaceButton.Visible
            && !GetMain<GroupBox>(main, "foldersGroup").Visible
            && !GetMain<Button>(main, "encodeMapsButton").Visible
            && !GetMain<Button>(main, "viewButton").Visible
            && !GetMain<Button>(main, "clearMemoryButton").Visible
            && !GetMain<Button>(main, "saveIndexButton").Visible
            && !GetMain<Button>(main, "greenButton").Visible
            && !GetMain<Button>(main, "deleteButton").Visible
            && GetMain<TextBox>(main, "surfaceGrhText").Text == "999",
            "WE: activar el modo muestra el panel y sincroniza el GRH seleccionado");
        toggleWeMode.Invoke(main, null);
        Assert(!surfaceGroup.Visible && !generateSurfaceButton.Visible && GetMain<GroupBox>(main, "foldersGroup").Visible
            && GetMain<Button>(main, "encodeMapsButton").Visible && GetMain<Button>(main, "viewButton").Visible
            && GetMain<Button>(main, "clearMemoryButton").Visible && GetMain<Button>(main, "saveIndexButton").Visible
            && GetMain<Button>(main, "greenButton").Visible && GetMain<Button>(main, "deleteButton").Visible,
            "WE: desactivar el modo oculta el panel y el botón");
        capasButton.PerformClick();
        Assert(surfaceGroup.Visible, "WE: Capas activa el modo de superficies desde el botón del diseñador");
        capasButton.PerformClick();
        Assert(!surfaceGroup.Visible, "WE: Capas vuelve a ocultar el modo de superficies");

        Directory.Delete(graphicsPath, true);
        main.Close();
    });
}

static void TestWeIndexFile(string root)
{
    var path = Path.Combine(root, "Indices.ini");
    var first = WeIndexFile.Append(path, "Terreno", 115, 32, 32);
    var second = WeIndexFile.Append(path, "Agua", 116, 64, 32);
    var content = File.ReadAllText(path);
    Assert(first == 1 && second == 2 && content.Contains("Referencias=2")
        && content.Contains("[REFERENCIA1]") && content.Contains("GrhIndice=115")
        && content.Contains("[REFERENCIA2]") && content.Contains("GrhIndice=116"),
        "WE: genera y añade referencias compatibles en Indices.ini");
}

static void TestIndexerSettings(string root)
{
    var settings = new IndexerSettings { IndicesPath = "Datos", ResourceMode = 1 };
    var expected = Path.Combine(AppContext.BaseDirectory, "Datos", "Indices.ini");
    Assert(settings.ResolveIndicesFilePath(AppContext.BaseDirectory) == expected, "Indices.ini: carpeta de datos");

    settings.IndicesPath = Path.Combine(root, "personalizado", "Indices.ini");
    Assert(settings.ResolveIndicesFilePath(AppContext.BaseDirectory) == settings.IndicesPath, "Indices.ini: ruta completa");

    settings.ResourceMode = 7;
    settings.Save();
    Assert(settings.ResourceMode == 1, "ResourceMode: valor fuera de rango normalizado");
}

static void TestAllIndexerStates()
{
    RunSta(() =>
    {
        using var main = new MainForm();
        main.Show();
        Application.DoEvents();
        var changeState = typeof(MainForm).GetMethod("CambiarEstado", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var states = new[] { "Grh", "Body", "Cabezas", "Cascos", "Escudos", "Armas", "Alas", "Capas", "Botas", "Fx", "Resource" };
        foreach (var state in states)
        {
            changeState.Invoke(main, new object[] { state });
            var expected = state.Equals("Bodies", StringComparison.OrdinalIgnoreCase) ? "Body" : state;
            Assert(GetMainValue<string>(main, "_indexerState").Equals(expected, StringComparison.OrdinalIgnoreCase),
                $"Regresión: estado {state} se puede cargar sin excepción");
        }
        main.Close();
    });
}

static T Get<T>(Form form, string name) where T : Control
{
    var field = typeof(FormAuto).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Fallo: no existe el control {name}.");
    return (T)(field.GetValue(form) ?? throw new InvalidOperationException($"Fallo: control {name} no inicializado."));
}

static T GetMain<T>(MainForm form, string name) where T : Control
{
    var field = typeof(MainForm).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Fallo: no existe el control MainForm.{name}.");
    return (T)(field.GetValue(form) ?? throw new InvalidOperationException($"Fallo: control MainForm.{name} no inicializado."));
}

static List<T> GetMainList<T>(MainForm form, string name)
{
    var field = typeof(MainForm).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Fallo: no existe la lista MainForm.{name}.");
    return (List<T>)(field.GetValue(form) ?? throw new InvalidOperationException($"Fallo: lista MainForm.{name} no inicializada."));
}

static T GetMainValue<T>(MainForm form, string name)
{
    var field = typeof(MainForm).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Fallo: no existe el valor MainForm.{name}.");
    return (T)(field.GetValue(form) ?? throw new InvalidOperationException($"Fallo: valor MainForm.{name} no inicializado."));
}

static void InvokeMain(MainForm form, string name)
{
    var method = typeof(MainForm).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Fallo: no existe MainForm.{name}.");
    method.Invoke(form, null);
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
