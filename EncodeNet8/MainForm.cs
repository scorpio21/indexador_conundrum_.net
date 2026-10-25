namespace EncodeNet8;

public partial class MainForm : Form
{
    private GraficosIndFile? _file;
    private string? _loadedPath;
    private readonly IndexerSettings _settings;
    private readonly System.Windows.Forms.Timer _drawTimer = new() { Interval = 150 };
    private GrhRecord? _selectedRecord;
    private int _animationFrame;
    private string _indexerState = "Grh";
    private readonly Dictionary<string, int> _lastIndexByState = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<BodyIndexRecord> _bodyRecords = new();

    private static readonly string[] AllStates = { "Grh", "Body", "Cabezas", "Cascos", "Escudos", "Armas", "Alas", "Capas", "Fx", "Resource", "Botas" };

    public MainForm()
    {
        InitializeComponent();
        _settings = IndexerSettings.Load();
        _drawTimer.Tick += drawTimer_Tick;
        ConfigureRuntimeUi();
        ConfigureIcons();
        indsText.Text = _settings.InitPath;
        graphicsText.Text = _settings.GraphicsPath;
        formatCombo.SelectedIndex = 0;
        LoadConfiguredGraphics();
        UpdateUi();
    }

    private void ConfigureRuntimeUi()
    {
        archivoMenu.DropDownItems.Add(MenuItem("Guardar", saveButton_Click, Keys.Control | Keys.S));
        archivoMenu.DropDownItems.Add(MenuItem("Guardar...", saveIndexButton_Click));
        archivoMenu.DropDownItems.Add(MenuItem("Cargar", openButton_Click, Keys.Control | Keys.O));
        archivoMenu.DropDownItems.Add(MenuItem("Cargar...", openButton_Click));
        var savedIndexMenu = new ToolStripMenuItem("Índice de guardado");
        savedIndexMenu.DropDownItems.Add(MenuItem("Sobreescribir", (_, _) => SelectSavedIndex(0)));
        for (var i = 1; i <= 9; i++)
        {
            var slot = i;
            savedIndexMenu.DropDownItems.Add(MenuItem(slot.ToString(), (_, _) => SelectSavedIndex(slot)));
        }
        archivoMenu.DropDownItems.Add(new ToolStripSeparator());
        archivoMenu.DropDownItems.Add(savedIndexMenu);
        archivoMenu.DropDownItems.Add(new ToolStripSeparator());
        archivoMenu.DropDownItems.Add(MenuItem("Salir", (_, _) => Close()));

        edicionMenu.DropDownItems.Add(MenuItem("Nuevo/Ir A", newSearchButton_Click, Keys.Control | Keys.F));
        edicionMenu.DropDownItems.Add(MenuItem("Mover", (_, _) => ShowPending("Mover")));
        edicionMenu.DropDownItems.Add(MenuItem("Copiar", (_, _) => ShowPending("Copiar")));
        edicionMenu.DropDownItems.Add(MenuItem("Suprimir", deleteButton_Click, Keys.Delete));
        edicionMenu.DropDownItems.Add(MenuItem("Clonar...", (_, _) => ShowPending("Clonar")));
        edicionMenu.DropDownItems.Add(MenuItem("Color de fondo...", (_, _) => ShowPending("Color de fondo")));

        herramientasMenu.DropDownItems.Add(MenuItem("Buscar Grh con bmp...", (_, _) => ShowPending("Buscar GRH con BMP")));
        herramientasMenu.DropDownItems.Add(MenuItem("Buscar Bmps sin indexar (Cry)", (_, _) => ShowPending("Buscar BMPs sin indexar")));
        herramientasMenu.DropDownItems.Add(MenuItem("Buscar siguiente", (_, _) => ShowPending("Buscar siguiente"), Keys.F3));
        herramientasMenu.DropDownItems.Add(MenuItem("Autoindexador", (_, _) => ShowPending("Autoindexador")));
        herramientasMenu.DropDownItems.Add(MenuItem("Buscar Grh Repetidos", (_, _) => ShowPending("Buscar GRH repetidos")));

        indexarMenu.DropDownItems.Add(MenuItem("Generar Índices Para WE", (_, _) => ShowPending("Generar índices para WE")));
        ayudaMenu.DropDownItems.Add(MenuItem("Acerca de...", (_, _) => MessageBox.Show(this, "AOSpain - Indexador de gráficos .NET 8\nBasado en Encode.frm de Conundrum.", "Ayuda")));

    }

    private static ToolStripMenuItem MenuItem(string text, EventHandler handler, Keys shortcut = Keys.None)
    {
        var item = new ToolStripMenuItem(text) { ShortcutKeys = shortcut };
        item.Click += handler;
        return item;
    }

    private void SelectSavedIndex(int slot) => statusLabel.Text = $"Índice de guardado seleccionado: {slot}.";
    private void ShowPending(string action) => MessageBox.Show(this, $"La acción «{action}» está preparada en el menú y se conectará al módulo correspondiente.", "AOSpain");

    private void configuracionMenu_Click(object? sender, EventArgs e)
    {
        using var dialog = new SettingsForm(_settings);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            indsText.Text = _settings.InitPath;
            graphicsText.Text = _settings.GraphicsPath;
            statusLabel.Text = "Configuración guardada en JSON.";
        }
    }

    private void ConfigureIcons()
    {
        var iconFolder = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons");
        if (!Directory.Exists(iconFolder)) return;

        Icon? appIcon = TryLoadIcon(Path.Combine(iconFolder, "logo.ico"));
        if (appIcon is not null) Icon = appIcon;

        var buttonIcons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Bodies"] = "Bodys.ico", ["Cascos"] = "Cascos.ico", ["Escudos"] = "escudos.ico", ["Armas"] = "Armas.ico",
            ["Fx"] = "Alas.ico"
        };

        foreach (var button in Controls.OfType<Button>())
        {
            if (!buttonIcons.TryGetValue(button.Text, out var file)) continue;
            var icon = TryLoadIcon(Path.Combine(iconFolder, file));
            if (icon is null) continue;
            using var source = icon.ToBitmap();
            button.Image = new Bitmap(source, new Size(20, 20));
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Padding = new Padding(2, 0, 2, 0);
        }
    }

    private static Icon? TryLoadIcon(string path)
    {
        try { return File.Exists(path) ? new Icon(path) : null; }
        catch { return null; }
    }

    private GrhStorageFormat SelectedFormat => formatCombo.SelectedIndex == 1 ? GrhStorageFormat.LongIndexes : GrhStorageFormat.LegacyInteger;

    private void openButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "Índices gráficos (*.ind)|*.ind|Todos los archivos (*.*)|*.*", FileName = "Graficos.ind", InitialDirectory = ResolvePath(_settings.InitPath) };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _file = LoadAuto(dialog.FileName);

            _loadedPath = dialog.FileName;
            RefreshGrid();
            statusLabel.Text = $"Cargados {_file.Records.Count} GRH: {dialog.FileName}";
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo cargar el índice", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void LoadConfiguredGraphics()
    {
        var indexPath = Path.Combine(ResolvePath(_settings.InitPath), "Graficos.ind");
        if (!File.Exists(indexPath))
        {
            statusLabel.Text = $"No se encontró el índice: {indexPath}";
            return;
        }

        try
        {
            _file = LoadAuto(indexPath);
            _loadedPath = indexPath;
            RefreshGrid();
            statusLabel.Text = $"Cargados {_file.Records.Count} GRH desde Init.";
        }
        catch (Exception ex)
        {
            statusLabel.Text = $"No se pudo cargar Graficos.ind: {ex.Message}";
        }
    }

    private GraficosIndFile LoadAuto(string path)
    {
        try
        {
            _file = GraficosIndFile.Load(path, GrhStorageFormat.LegacyInteger);
            formatCombo.SelectedIndex = 0;
            return _file;
        }
        catch
        {
            _file = GraficosIndFile.Load(path, GrhStorageFormat.LongIndexes);
            formatCombo.SelectedIndex = 1;
            return _file;
        }
    }

    private static string ResolvePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return AppContext.BaseDirectory;
        if (Path.IsPathRooted(path)) return path;

        var direct = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
        if (Directory.Exists(direct)) return direct;

        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, path);
            if (Directory.Exists(candidate)) return candidate;
            current = current.Parent;
        }

        return direct;
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        if (_file is null) return;
        using var dialog = new SaveFileDialog { Filter = "Índice gráfico (*.ind)|*.ind|Todos los archivos (*.*)|*.*", FileName = Path.GetFileName(_loadedPath ?? "Graficos.ind") };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try { ApplyGridToModel(); _file.Save(dialog.FileName, SelectedFormat); _loadedPath = dialog.FileName; statusLabel.Text = $"Guardado: {dialog.FileName}"; }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo guardar el índice", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void formatCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_file is null || _loadedPath is null) return;
        try { _file = GraficosIndFile.Load(_loadedPath, SelectedFormat); RefreshGrid(); } catch { }
    }

    private void RefreshGrid()
    {
        indexList.Items.Clear();
        if (_file is null) return;
        foreach (var record in _file.Records)
            indexList.Items.Add(record.Id);
        ComprobarIndexLista();
        UpdateUi();
    }

    private void ApplyGridToModel()
    {
        if (_file is null || indexList.SelectedIndex < 0) return;
        var record = _file.Records[indexList.SelectedIndex];
        record.FileNumber = ParseShort(bmpText.Text); record.X = ParseShort(xText.Text); record.Y = ParseShort(yText.Text);
        record.Width = ParseShort(widthText.Text); record.Height = ParseShort(heightText.Text); record.Speed = ParseShort(speedText.Text);
        var frameText = framesText.Text.Trim();
        record.Frames = string.IsNullOrWhiteSpace(frameText) ? new List<int> { record.Id } : frameText.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    }

    private static short ParseShort(object? value) => short.TryParse(Convert.ToString(value), out var result) ? result : (short)0;

    private void UpdateUi()
    {
        saveButton.Enabled = _file is not null;
        statusLabel.Text = _file is null ? "Abre un Graficos.ind para comenzar." : $"GRH: {_file.Records.Count}";
    }

    private void indexList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (indexList.SelectedIndex < 0) return;
        ClickEnLista();
    }

    private void ClickEnLista()
    {
        if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
        {
            ClickBodyEnLista();
            return;
        }
        if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            _lastIndexByState[_indexerState] = indexList.SelectedIndex;
            grhText.Text = indexList.SelectedItem?.ToString() ?? "";
            foreach (var box in new[] { bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText, referencesText }) box.Clear();
            _selectedRecord = null;
            _drawTimer.Stop();
            previewPanel.Invalidate();
            statusLabel.Text = $"{StateCaption(_indexerState)} {grhText.Text} seleccionado. Detalles pendientes del lector de este índice.";
            return;
        }
        if (_file is null || indexList.SelectedIndex < 0) return;
        _selectedRecord = _file.Records[indexList.SelectedIndex];
        _lastIndexByState[_indexerState] = indexList.SelectedIndex;
        _animationFrame = 0;
        grhText.Text = _selectedRecord.Id.ToString(); bmpText.Text = _selectedRecord.FileNumber.ToString(); xText.Text = _selectedRecord.X.ToString(); yText.Text = _selectedRecord.Y.ToString();
        widthText.Text = _selectedRecord.Width.ToString(); heightText.Text = _selectedRecord.Height.ToString(); speedText.Text = _selectedRecord.Speed.ToString();
        framesText.Text = _selectedRecord.IsAnimation ? string.Join("-", _selectedRecord.Frames) : ""; frameText.Text = _selectedRecord.IsAnimation ? "1" : ""; referencesText.Text = "0";
        _drawTimer.Enabled = _selectedRecord.IsAnimation;
        previewPanel.Invalidate();
        statusLabel.Text = $"GRH {_selectedRecord.Id} seleccionado ({_indexerState}).";
    }

    private void DeleteSelected()
    {
        if (_file is null || indexList.SelectedIndex < 0) return;
        _file.Records.RemoveAt(indexList.SelectedIndex); RefreshGrid();
    }

    private void ClearMemory()
    {
        _file = null; _loadedPath = null; _selectedRecord = null; _drawTimer.Stop(); indexList.Items.Clear(); previewPanel.Invalidate();
        foreach (var box in new[] { grhText, bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText, referencesText }) box.Clear();
        UpdateUi();
    }

    private void SaveIndex()
    {
        saveButton.PerformClick();
    }

    private void viewButton_Click(object? sender, EventArgs e)
    {
        if (_file is null) return;
        previewPanel.Invalidate(); statusLabel.Text = $"Mostrando {_file.Records.Count} GRH en memoria.";
    }

    private void graphicsButton_Click(object? sender, EventArgs e)
    {
        CambiarEstado("Grh");
        ComprobarIndexLista();
        if (indexList.SelectedIndex >= 0) ClickEnLista();
    }

    private void CambiarEstado(string state)
    {
        _indexerState = NormalizeState(state);
        _drawTimer.Stop();
        _selectedRecord = null;
        previewPanel.Invalidate();
        ConfigureStateFields(_indexerState);

        if (_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            if (_file is null) LoadConfiguredGraphics();
            RefreshGrid();
            statusLabel.Text = $"Estado Gráficos seleccionado. GRH cargados: {_file?.Records.Count ?? 0}.";
        }
        else if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
        {
            if (_file is null) LoadConfiguredGraphics();
            LoadBodies();
            RefreshBodyList();
            statusLabel.Text = $"Estado Bodies seleccionado. Bodies cargados: {_bodyRecords.Count}.";
        }
        else
        {
            LoadStateSlots(_indexerState);
            statusLabel.Text = $"Estado {StateCaption(_indexerState)} seleccionado. {indexList.Items.Count} índices disponibles.";
        }

        ComprobarIndexLista();
    }

    private static string NormalizeState(string state)
    {
        if (state.Equals("Bodies", StringComparison.OrdinalIgnoreCase)) return "Body";
        if (state.Equals("Gráficos", StringComparison.OrdinalIgnoreCase)) return "Grh";
        return AllStates.FirstOrDefault(x => x.Equals(state, StringComparison.OrdinalIgnoreCase)) ?? "Grh";
    }

    private static string StateCaption(string state) => state switch
    {
        "Grh" => "Gráficos",
        "Body" => "Bodies",
        "Fx" => "FX",
        "Resource" => "Recursos",
        _ => state
    };

    private void ConfigureStateFields(string state)
    {
        var textBoxes = new[] { bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText, referencesText };
        var labels = new[] { bmpLabel, framesLabel, heightLabel, widthLabel, speedLabel, xLabel, yLabel, tileHeightLabel, tileWidthLabel, referencesLabel };
        foreach (var control in textBoxes) control.Visible = true;
        foreach (var control in labels) control.Visible = true;

        if (state == "Grh")
        {
            grhLabel.Text = "Grh:";
            bmpLabel.Text = "Numero BMP:";
            framesLabel.Text = "Numero Frames:";
            heightLabel.Text = "Alto:";
            widthLabel.Text = "Ancho:";
            speedLabel.Text = "Velocidad:";
            xLabel.Text = "Posicion X:";
            yLabel.Text = "Posicion Y:";
            tileHeightLabel.Text = "Alto Titles:";
            tileWidthLabel.Text = "Ancho Titles:";
            referencesLabel.Text = "Referencias:";
            return;
        }

        if (state == "Fx")
        {
            grhLabel.Text = "Fx:";
            bmpLabel.Text = "NumGrh:";
            framesLabel.Text = "Offset:";
            HideStateFields(3);
            return;
        }

        grhLabel.Text = $"{StateCaption(state)}:";
        bmpLabel.Text = "Subir:";
        framesLabel.Text = "Derecha:";
        heightLabel.Text = "Abajo:";
        widthLabel.Text = "Izquierda:";
        speedLabel.Text = state == "Body" || state == "Alas" ? "Offset:" : "";
        HideStateFields(state == "Body" || state == "Alas" ? 6 : 5);
    }

    private void HideStateFields(int visibleCount)
    {
        var textBoxes = new[] { bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText, referencesText };
        var labels = new[] { bmpLabel, framesLabel, heightLabel, widthLabel, speedLabel, xLabel, yLabel, tileHeightLabel, tileWidthLabel, referencesLabel };
        for (var i = visibleCount; i < textBoxes.Length; i++)
        {
            textBoxes[i].Visible = false;
            labels[i].Visible = false;
        }
    }

    private void ComprobarIndexLista()
    {
        if (indexList.Items.Count == 0)
        {
            indexList.SelectedIndex = -1;
            return;
        }

        var lastIndex = _lastIndexByState.TryGetValue(_indexerState, out var savedIndex) ? savedIndex : 0;
        if (lastIndex < 0) indexList.SelectedIndex = -1;
        else if (lastIndex >= indexList.Items.Count) indexList.SelectedIndex = indexList.Items.Count - 1;
        else indexList.SelectedIndex = lastIndex;
    }

    private void LoadStateSlots(string state)
    {
        indexList.Items.Clear();
        var initPath = ResolvePath(_settings.InitPath);
        var candidates = state switch
        {
            "Cabezas" => new[] { "Cabezas.ind" },
            "Cascos" => new[] { "Cascos.ind" },
            "Fx" => new[] { "Fxs.ind", "Fxsz.ind" },
            "Armas" => new[] { "Armas.dat" },
            "Escudos" => new[] { "Escudos.dat" },
            "Alas" => new[] { "Alas.ind", "Alas.dat" },
            "Capas" => new[] { "Capas.ind", "Capas.dat" },
            "Botas" => new[] { "Botas.ind", "Botas.dat" },
            _ => Array.Empty<string>()
        };

        foreach (var candidate in candidates)
        {
            var path = Path.Combine(initPath, candidate);
            if (!File.Exists(path)) continue;
            var occupied = Path.GetExtension(path).Equals(".ind", StringComparison.OrdinalIgnoreCase)
                ? ReadOccupiedBinarySlots(path, new[] { 6, 8, 12, 16, 20 })
                : ReadOccupiedIniSlots(path, state);
            foreach (var index in occupied) indexList.Items.Add(index);
            if (occupied.Count > 0) return;
        }

        if (state.Equals("Resource", StringComparison.OrdinalIgnoreCase))
        {
            var graphicsPath = ResolvePath(_settings.GraphicsPath);
            if (Directory.Exists(graphicsPath))
            {
                foreach (var file in Directory.EnumerateFiles(graphicsPath, "*.bmp"))
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (int.TryParse(name, out var index) && index > 0) indexList.Items.Add(index);
                }
            }
            var sorted = indexList.Items.Cast<int>().OrderBy(x => x).ToArray();
            indexList.Items.Clear();
            foreach (var index in sorted) indexList.Items.Add(index);
        }
    }

    private static List<int> ReadOccupiedBinarySlots(string path, int[] possibleRecordSizes)
    {
        var result = new List<int>();
        using var stream = File.OpenRead(path);
        if (stream.Length < 265) return result;
        stream.Position = 263;
        using var reader = new BinaryReader(stream);
        var count = reader.ReadInt16();
        if (count <= 0) return result;
        var remaining = stream.Length - stream.Position;
        var recordSize = possibleRecordSizes.FirstOrDefault(size => remaining == count * (long)size);
        if (recordSize == 0) return result;
        var indexSize = recordSize >= 12 ? 4 : 2;
        for (var i = 1; i <= count; i++)
        {
            var first = indexSize == 4 ? reader.ReadInt32() : reader.ReadInt16();
            if (first > 0) result.Add(i);
            stream.Position += recordSize - indexSize;
        }
        return result;
    }

    private static List<int> ReadOccupiedIniSlots(string path, string state)
    {
        var result = new List<int>();
        var key = state switch
        {
            "Armas" => "NumArmas",
            "Escudos" => "NumEscudos",
            "Alas" => "NumAlas",
            "Capas" => "NumCapas",
            "Botas" => "NumBotas",
            _ => "NumFxs"
        };
        var sectionPrefix = state switch
        {
            "Armas" => "ARMA",
            "Escudos" => "ESC",
            "Alas" => "ALAS",
            "Capas" => "CAPA",
            "Botas" => "BOTA",
            _ => "FX"
        };
        var firstField = state switch
        {
            "Armas" or "Escudos" => "Dir1",
            "Alas" => "ALAS1",
            "Capas" => "Capa1",
            "Botas" => "Bota1",
            _ => "Animacion"
        };
        var inInit = false;
        var currentIndex = 0;
        var currentOccupied = false;
        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Split('\'', 2)[0].Trim();
            if (line.StartsWith("["))
            {
                inInit = line.Equals("[INIT]", StringComparison.OrdinalIgnoreCase);
                if (currentIndex > 0 && currentOccupied) result.Add(currentIndex);
                currentIndex = 0;
                currentOccupied = false;
                var section = line.Trim('[', ']');
                if (section.StartsWith(sectionPrefix, StringComparison.OrdinalIgnoreCase))
                    int.TryParse(section[sectionPrefix.Length..], out currentIndex);
                continue;
            }
            var separator = line.IndexOf('=');
            if (separator <= 0) continue;
            var name = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            if (inInit && name.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(value, out var count) || count <= 0) return result;
                inInit = false;
            }
            if (name.Equals(firstField, StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out var first) && first > 0)
                currentOccupied = true;
        }
        if (currentIndex > 0 && currentOccupied) result.Add(currentIndex);
        return result.OrderBy(x => x).ToList();
    }

    private void drawTimer_Tick(object? sender, EventArgs e)
    {
        if (_selectedRecord is null || !_selectedRecord.IsAnimation) return;
        _animationFrame = (_animationFrame + 1) % _selectedRecord.Frames.Count;
        frameText.Text = (_animationFrame + 1).ToString();
        previewPanel.Invalidate();
    }

    private void previewPanel_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.Clear(previewPanel.BackColor);
        if (_selectedRecord is null) return;

        var record = _selectedRecord;
        if (record.IsAnimation && _file is not null)
        {
            var frameId = record.Frames[Math.Min(_animationFrame, record.Frames.Count - 1)];
            var frameRecord = _file.Records.FirstOrDefault(x => x.Id == frameId);
            if (frameRecord is not null) record = frameRecord;
        }

        var graphicsFolder = ResolvePath(_settings.GraphicsPath);
        var bitmapPath = Path.Combine(graphicsFolder, $"{record.FileNumber}.bmp");
        if (!File.Exists(bitmapPath))
        {
            using var brush = new SolidBrush(Color.DimGray);
            using var font = new Font(Font.FontFamily, 10F);
            e.Graphics.DrawString($"BMP {record.FileNumber} no encontrado\n{bitmapPath}", font, brush, new PointF(12, 12));
            return;
        }

        try
        {
            using var bitmap = Image.FromFile(bitmapPath);
            var source = new Rectangle(record.X, record.Y, record.Width, record.Height);
            if (source.Width <= 0 || source.Height <= 0 || source.Right > bitmap.Width || source.Bottom > bitmap.Height)
                source = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var scale = Math.Min((double)(previewPanel.ClientSize.Width - 20) / source.Width, (double)(previewPanel.ClientSize.Height - 20) / source.Height);
            scale = Math.Min(scale, 4D);
            var dest = new Rectangle((previewPanel.ClientSize.Width - (int)(source.Width * scale)) / 2, (previewPanel.ClientSize.Height - (int)(source.Height * scale)) / 2, (int)(source.Width * scale), (int)(source.Height * scale));
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.DrawImage(bitmap, dest, source, GraphicsUnit.Pixel);
        }
        catch (Exception ex)
        {
            using var brush = new SolidBrush(Color.DarkRed);
            e.Graphics.DrawString($"No se pudo dibujar el BMP: {ex.Message}", Font, brush, new PointF(12, 12));
        }
    }

    private void deleteButton_Click(object? sender, EventArgs e) => DeleteSelected();
    private void saveIndexButton_Click(object? sender, EventArgs e) => SaveIndex();
    private void clearMemoryButton_Click(object? sender, EventArgs e) => ClearMemory();
    private void encodeMapsButton_Click(object? sender, EventArgs e) => MessageBox.Show(this, "El módulo de mapas se añadirá en la siguiente fase.", "Encode Mapas");
    private void newSearchButton_Click(object? sender, EventArgs e)
    {
        indexList.Items.Clear(); statusLabel.Text = "Nuevo índice / búsqueda preparada.";
    }
    private void greenButton_Click(object? sender, EventArgs e)
    {
        statusLabel.Text = "Filtro verde activado.";
    }
    private void kindButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        var state = button.Text.Equals("Bodies", StringComparison.OrdinalIgnoreCase) ? "Body" : button.Text;
        CambiarEstado(state);
        ComprobarIndexLista();
    }

    private void LoadBodies()
    {
        _bodyRecords.Clear();
        var path = Path.Combine(ResolvePath(_settings.InitPath), "Personajes.ind");
        if (!File.Exists(path)) { statusLabel.Text = $"No se encontró: {path}"; return; }

        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        if (stream.Length < 265) return;
        stream.Position = 263;
        var count = reader.ReadInt16();
        if (count < 0) return;
        var legacyLength = 263L + 2L + count * 12L;
        var longFormat = stream.Length >= 263L + 2L + count * 20L && stream.Length != legacyLength;
        for (var i = 1; i <= count; i++)
        {
            var walk1 = longFormat ? reader.ReadInt32() : reader.ReadInt16();
            var walk2 = longFormat ? reader.ReadInt32() : reader.ReadInt16();
            var walk3 = longFormat ? reader.ReadInt32() : reader.ReadInt16();
            var walk4 = longFormat ? reader.ReadInt32() : reader.ReadInt16();
            var offsetX = reader.ReadInt16();
            var offsetY = reader.ReadInt16();
            if (walk1 > 0) _bodyRecords.Add(new BodyIndexRecord { Id = i, Walk1 = walk1, Walk2 = walk2, Walk3 = walk3, Walk4 = walk4, HeadOffsetX = offsetX, HeadOffsetY = offsetY });
        }
    }

    private void RefreshBodyList()
    {
        indexList.Items.Clear();
        foreach (var body in _bodyRecords) indexList.Items.Add(body.Id);
        ComprobarIndexLista();
    }

    private void ClickBodyEnLista()
    {
        if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _bodyRecords.Count) return;
        var body = _bodyRecords[indexList.SelectedIndex];
        _lastIndexByState[_indexerState] = indexList.SelectedIndex;
        grhText.Text = body.Id.ToString();
        bmpText.Text = body.Walk1.ToString();
        xText.Text = body.HeadOffsetX.ToString();
        yText.Text = body.HeadOffsetY.ToString();
        widthText.Text = body.Walk2.ToString();
        heightText.Text = body.Walk3.ToString();
        framesText.Text = $"{body.Walk1}-{body.Walk2}-{body.Walk3}-{body.Walk4}";
        referencesText.Text = "4 direcciones";
        _selectedRecord = _file?.Records.FirstOrDefault(x => x.Id == body.Walk1);
        frameText.Text = "1";
        previewPanel.Invalidate();
        statusLabel.Text = $"Body {body.Id} seleccionado.";
    }
}
