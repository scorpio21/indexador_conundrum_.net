namespace EncodeNet8;

public partial class MainForm : Form
{
    private GraficosIndFile? _file;
    private string? _loadedPath;
    private readonly IndexerSettings _settings;
    private readonly System.Windows.Forms.Timer _drawTimer = new() { Interval = 150 };
    private readonly Dictionary<string, Bitmap> _bitmapCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly System.Diagnostics.Stopwatch _animationClock = System.Diagnostics.Stopwatch.StartNew();
    private long _animationElapsed;
    private int _animationIntervalMs = 150;
    private GrhRecord? _selectedRecord;
    private int _animationFrame;
    private string _indexerState = "Grh";
    private readonly Dictionary<string, int> _lastIndexByState = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<BodyIndexRecord> _bodyRecords = new();
    private readonly List<DirectionalIndexRecord> _directionalRecords = new();
    private readonly List<FxIndexRecord> _fxRecords = new();
    private readonly Dictionary<int, int> _referenceCounts = new();
    private string? _directionalPath;
    private string? _iniDirectionalPath;
    private string? _fxPath;
    private GrhStorageFormat _directionalFormat;
    private GrhStorageFormat _fxFormat;
    private List<int> _searchResults = new();
    private int _searchCursor;

    private static readonly string[] AllStates = { "Grh", "Body", "Cabezas", "Cascos", "Escudos", "Armas", "Alas", "Capas", "Fx", "Resource", "Botas" };

    public MainForm()
    {
        InitializeComponent();
        _settings = IndexerSettings.Load();
        _drawTimer.Tick += drawTimer_Tick;
        FormClosed += (_, _) => DisposeBitmapCache();
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

        herramientasMenu.DropDownItems.Add(MenuItem("Buscar Grh con bmp...", (_, _) => SearchGrhWithBmp()));
        herramientasMenu.DropDownItems.Add(MenuItem("Buscar Bmps sin indexar (Cry)", (_, _) => SearchUnindexedBmps()));
        herramientasMenu.DropDownItems.Add(MenuItem("Buscar siguiente", (_, _) => SearchNext(), Keys.F3));
        herramientasMenu.DropDownItems.Add(MenuItem("Autoindexador", (_, _) => ShowPending("Autoindexador")));
        herramientasMenu.DropDownItems.Add(MenuItem("Buscar Grh Repetidos", (_, _) => SearchRepeatedGrh()));

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
            DisposeBitmapCache();
            indsText.Text = _settings.InitPath;
            graphicsText.Text = _settings.GraphicsPath;
            previewPanel.Invalidate();
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
            RebuildReferenceCounts();
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
            RebuildReferenceCounts();
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
        SaveCurrentIndex();
    }

    private void formatCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_file is null || _loadedPath is null) return;
        try { _file = GraficosIndFile.Load(_loadedPath, SelectedFormat); RebuildReferenceCounts(); RefreshGrid(); } catch { }
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
        var record = SelectedGrhRecord();
        if (record is null) return;
        record.FileNumber = ParseShort(bmpText.Text); record.X = ParseShort(xText.Text); record.Y = ParseShort(yText.Text);
        record.Width = ParseShort(widthText.Text); record.Height = ParseShort(heightText.Text); record.Speed = ParseShort(speedText.Text);
        var frameText = framesText.Text.Trim();
        record.Frames = string.IsNullOrWhiteSpace(frameText) ? new List<int> { record.Id } : frameText.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    }

    private static short ParseShort(object? value) => short.TryParse(Convert.ToString(value), out var result) ? result : (short)0;

    private void UpdateUi()
    {
        var canSaveState = _indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase)
            || _directionalPath is not null || _iniDirectionalPath is not null || _fxPath is not null;
        saveButton.Enabled = _file is not null && canSaveState;
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
            ClickDirectionalEnLista();
            return;
        }
        if (IsDirectionalState(_indexerState))
        {
            ClickDirectionalEnLista();
            return;
        }
        if (_indexerState is "Armas" or "Escudos")
        {
            ClickDirectionalEnLista();
            return;
        }
        if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
        {
            ClickFxEnLista();
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
        _selectedRecord = SelectedGrhRecord();
        if (_selectedRecord is null) return;
        _lastIndexByState[_indexerState] = indexList.SelectedIndex;
        _animationFrame = 0;
        grhText.Text = _selectedRecord.Id.ToString(); bmpText.Text = _selectedRecord.FileNumber.ToString(); xText.Text = _selectedRecord.X.ToString(); yText.Text = _selectedRecord.Y.ToString();
        widthText.Text = _selectedRecord.Width.ToString(); heightText.Text = _selectedRecord.Height.ToString(); speedText.Text = _selectedRecord.Speed.ToString();
        framesText.Text = _selectedRecord.IsAnimation ? string.Join("-", _selectedRecord.Frames) : ""; frameText.Text = _selectedRecord.IsAnimation ? "1" : ""; referencesText.Text = CountReferences(_selectedRecord.Id).ToString();
        _drawTimer.Enabled = _selectedRecord.IsAnimation;
        ConfigureAnimationTimer(_selectedRecord);
        previewPanel.Invalidate();
        statusLabel.Text = $"GRH {_selectedRecord.Id} seleccionado ({_indexerState}).";
    }

    private void DeleteSelected()
    {
        if (_file is null || indexList.SelectedIndex < 0) return;
        var record = SelectedGrhRecord();
        if (record is null) return;
        _file.Records.Remove(record);
        RefreshGrid();
    }

    private GrhRecord? SelectedGrhRecord()
    {
        if (_file is null || indexList.SelectedIndex < 0) return null;
        var text = indexList.SelectedItem?.ToString() ?? string.Empty;
        var separator = text.IndexOf(' ');
        if (separator >= 0) text = text[..separator];
        return int.TryParse(text, out var id) ? _file.Records.FirstOrDefault(x => x.Id == id) : null;
    }

    private int CountReferences(int grhId)
    {
        if (grhId <= 0) return 0;
        return _referenceCounts.TryGetValue(grhId, out var count) ? count : 0;
    }

    private void RebuildReferenceCounts()
    {
        _referenceCounts.Clear();

        if (_file is not null)
        {
            foreach (var record in _file.Records)
            {
                foreach (var frame in record.Frames)
                    AddReference(frame);
            }
        }

        var initPath = ResolvePath(_settings.InitPath);
        foreach (var state in new[] { "Personajes", "Cabezas", "Cascos", "Alas", "Capas", "Botas" })
        {
            var binaryPath = Path.Combine(initPath, state + ".ind");
            var iniPath = Path.Combine(initPath, state + ".dat");
            try
            {
                var records = File.Exists(binaryPath)
                    ? LoadDirectionalAutoWithFormat(binaryPath, includesOffsets: state is "Personajes" or "Alas" or "Botas", out _)
                    : File.Exists(iniPath) ? IniDirectionalIndexFile.Load(iniPath, state) : new List<DirectionalIndexRecord>();
                foreach (var record in records)
                    foreach (var direction in record.Directions)
                        AddReference(direction);
            }
            catch (InvalidDataException)
            {
                // Un índice auxiliar inválido no impide consultar el resto del visor.
            }
        }

        foreach (var state in new[] { "Armas", "Escudos" })
        {
            var path = Path.Combine(initPath, state + ".dat");
            if (!File.Exists(path)) continue;
            try
            {
                foreach (var record in IniDirectionalIndexFile.Load(path, state))
                    foreach (var direction in record.Directions)
                        AddReference(direction);
            }
            catch (InvalidDataException)
            {
            }
        }

        foreach (var fileName in new[] { "Fxs.ind", "Fxsz.ind" })
        {
            var path = Path.Combine(initPath, fileName);
            if (!File.Exists(path)) continue;
            try
            {
                foreach (var record in LoadFxAutoWithFormat(path, out _))
                    AddReference(record.Animation);
                break;
            }
            catch (InvalidDataException)
            {
            }
        }
    }

    private void AddReference(int grhId)
    {
        if (grhId <= 0) return;
        _referenceCounts[grhId] = _referenceCounts.TryGetValue(grhId, out var count) ? count + 1 : 1;
    }

    private void ClearMemory()
    {
        _file = null; _loadedPath = null; _selectedRecord = null; _drawTimer.Stop(); indexList.Items.Clear(); previewPanel.Invalidate();
        foreach (var box in new[] { grhText, bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText, referencesText }) box.Clear();
        UpdateUi();
    }

    private Bitmap GetCachedBitmap(string path)
    {
        if (_bitmapCache.TryGetValue(path, out var cached)) return cached;

        var bitmap = new Bitmap(path);
        bitmap.MakeTransparent(Color.Black);
        _bitmapCache[path] = bitmap;
        return bitmap;
    }

    private void DisposeBitmapCache()
    {
        foreach (var bitmap in _bitmapCache.Values) bitmap.Dispose();
        _bitmapCache.Clear();
    }

    private void SaveIndex()
    {
        SaveIndexAs();
    }

    private void SaveCurrentIndex()
    {
        if (_file is null)
        {
            statusLabel.Text = "No hay ningún índice cargado para guardar.";
            return;
        }
        if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            SaveCurrentNonGrh();
            return;
        }
        if (string.IsNullOrWhiteSpace(_loadedPath))
        {
            SaveIndexAs();
            return;
        }
        var answer = MessageBox.Show(this, $"¿Sobrescribir el índice actual?\n{_loadedPath}", "Guardar índice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answer != DialogResult.Yes) return;
        try
        {
            ApplyGridToModel();
            _file.Save(_loadedPath, SelectedFormat);
            statusLabel.Text = $"Guardado: {_loadedPath}";
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo guardar el índice", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void SaveIndexAs()
    {
        if (_file is null)
        {
            statusLabel.Text = "No hay ningún índice cargado para guardar.";
            return;
        }
        if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            statusLabel.Text = $"Guardar como para {StateCaption(_indexerState)} se habilitará con su escritor específico.";
            return;
        }
        using var dialog = new SaveFileDialog { Filter = "Índice gráfico (*.ind)|*.ind|Todos los archivos (*.*)|*.*", FileName = Path.GetFileName(_loadedPath ?? "Graficos.ind") };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            ApplyGridToModel();
            _file.Save(dialog.FileName, SelectedFormat);
            _loadedPath = dialog.FileName;
            statusLabel.Text = $"Guardado: {dialog.FileName}";
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo guardar el índice", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void SaveCurrentNonGrh()
    {
        var path = _directionalPath ?? _iniDirectionalPath ?? _fxPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            statusLabel.Text = $"No hay archivo de {StateCaption(_indexerState)} cargado para guardar.";
            return;
        }
        var answer = MessageBox.Show(this, $"¿Sobrescribir el índice actual?\n{path}", "Guardar índice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answer != DialogResult.Yes) return;
        try
        {
            ApplyNonGrhToModel();
            if (_directionalPath is not null)
                DirectionalIndexFile.Save(_directionalPath, _directionalRecords, _directionalFormat, _indexerState is "Body" or "Alas" or "Botas");
            else if (_iniDirectionalPath is not null)
                IniDirectionalIndexFile.Save(_iniDirectionalPath, _directionalRecords, _indexerState);
            else if (_fxPath is not null)
                FxIndexFile.Save(_fxPath, _fxRecords, _fxFormat);
            RebuildReferenceCounts();
            statusLabel.Text = $"Guardado: {path}";
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo guardar el índice", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void ApplyNonGrhToModel()
    {
        if (indexList.SelectedIndex < 0) return;
        if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase) && indexList.SelectedIndex < _fxRecords.Count)
        {
            var item = _fxRecords[indexList.SelectedIndex];
            item.Animation = ParseInt(bmpText.Text);
            item.OffsetX = ParseShort(xText.Text);
            item.OffsetY = ParseShort(yText.Text);
            return;
        }
        if (indexList.SelectedIndex >= _directionalRecords.Count) return;
        var directional = _directionalRecords[indexList.SelectedIndex];
        var values = framesText.Text.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(value => int.TryParse(value, out var number) ? number : 0)
            .ToArray();
        if (values.Length == 4) directional.Directions = values;
        else directional.Directions[0] = ParseInt(bmpText.Text);
        directional.OffsetX = ParseShort(xText.Text);
        directional.OffsetY = ParseShort(yText.Text);
    }

    private static int ParseInt(object? value) => int.TryParse(Convert.ToString(value), out var result) ? result : 0;

    private void viewButton_Click(object? sender, EventArgs e)
    {
        if (_file is null) return;
        previewPanel.Invalidate(); statusLabel.Text = $"Mostrando {_file.Records.Count} GRH en memoria.";
    }

    private void SearchGrhWithBmp()
    {
        if (_file is null) { ShowPending("Buscar GRH con BMP: no hay Graficos.ind cargado"); return; }
        var graphicsPath = ResolvePath(_settings.GraphicsPath);
        _searchResults = _file.Records
            .Where(record => File.Exists(Path.Combine(graphicsPath, $"{record.FileNumber}.bmp")))
            .Select(record => record.Id)
            .ToList();
        _searchCursor = 0;
        ReplaceGrhList(_searchResults);
        statusLabel.Text = $"Búsqueda: {_searchResults.Count} GRH con BMP.";
    }

    private void SearchNext()
    {
        if (_searchResults.Count == 0) { statusLabel.Text = "No hay una búsqueda activa."; return; }
        _searchCursor = (_searchCursor + 1) % _searchResults.Count;
        indexList.SelectedIndex = _searchCursor;
        statusLabel.Text = $"Resultado {_searchCursor + 1}/{_searchResults.Count}.";
    }

    private void SearchUnindexedBmps()
    {
        if (_file is null) { ShowPending("Buscar BMPs sin indexar: no hay Graficos.ind cargado"); return; }
        var indexed = _file.Records.Select(record => (int)record.FileNumber).ToHashSet();
        var graphicsPath = ResolvePath(_settings.GraphicsPath);
        var unindexed = Directory.Exists(graphicsPath)
            ? Directory.EnumerateFiles(graphicsPath, "*.bmp")
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .Where(name => int.TryParse(name, out _))
                .Select(int.Parse)
                .Where(fileNumber => !indexed.Contains(fileNumber))
                .OrderBy(fileNumber => fileNumber)
                .ToList()
            : new List<int>();
        indexList.Items.Clear();
        foreach (var fileNumber in unindexed) indexList.Items.Add(fileNumber);
        statusLabel.Text = $"BMP sin GRH indexado: {unindexed.Count}.";
    }

    private void SearchRepeatedGrh()
    {
        if (_file is null) { ShowPending("Buscar GRH repetidos: no hay Graficos.ind cargado"); return; }
        var repeated = _file.Records
            .GroupBy(record => (record.FileNumber, record.X, record.Y, record.Width, record.Height))
            .Where(group => group.Count() > 1)
            .SelectMany(group => group.Select(record => record.Id))
            .OrderBy(id => id)
            .ToList();
        _searchResults = repeated;
        _searchCursor = 0;
        ReplaceGrhList(repeated);
        statusLabel.Text = $"GRH repetidos por recorte: {repeated.Count}.";
    }

    private void ReplaceGrhList(IEnumerable<int> ids)
    {
        indexList.Items.Clear();
        foreach (var id in ids) indexList.Items.Add(id);
        _lastIndexByState[_indexerState] = 0;
        ComprobarIndexLista();
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
            RefreshDirectionalList();
            statusLabel.Text = $"Estado Bodies seleccionado. Bodies cargados: {_bodyRecords.Count}.";
        }
        else if (IsDirectionalState(_indexerState))
        {
            LoadDirectionalState(_indexerState);
            RefreshDirectionalList();
            statusLabel.Text = $"Estado {StateCaption(_indexerState)} seleccionado. {_directionalRecords.Count} índices cargados.";
        }
        else if (_indexerState is "Armas" or "Escudos")
        {
            LoadIniDirectionalState(_indexerState);
            RefreshDirectionalList();
            statusLabel.Text = $"Estado {StateCaption(_indexerState)} seleccionado. {_directionalRecords.Count} índices cargados.";
        }
        else if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
        {
            LoadFxState();
            indexList.Items.Clear();
            foreach (var item in _fxRecords) indexList.Items.Add(item.Id);
            ComprobarIndexLista();
            statusLabel.Text = $"Estado FX seleccionado. {_fxRecords.Count} índices cargados.";
        }
        else
        {
            LoadStateSlots(_indexerState);
            statusLabel.Text = $"Estado {StateCaption(_indexerState)} seleccionado. {indexList.Items.Count} índices disponibles.";
        }

        UpdateUi();
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

    private static bool IsDirectionalState(string state) => state is "Cabezas" or "Cascos" or "Alas" or "Capas" or "Botas";

    private void ConfigureStateFields(string state)
    {
        directionCombo.Visible = false;
        frameText.Visible = true;
        frameLabel.Visible = true;
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
            frameText.Visible = false;
            frameLabel.Visible = false;
            HideStateFields(3);
            return;
        }

        grhLabel.Text = $"{StateCaption(state)}:";
        bmpLabel.Text = "Subir:";
        framesLabel.Text = "Derecha:";
        heightLabel.Text = "Abajo:";
        widthLabel.Text = "Izquierda:";
        speedLabel.Text = state == "Body" || state == "Alas" ? "Offset:" : "";
        directionCombo.Visible = true;
        frameText.Visible = false;
        frameLabel.Visible = false;
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
        if (_selectedRecord.Frames.Count == 0)
        {
            _drawTimer.Stop();
            return;
        }
        var elapsed = _animationClock.ElapsedMilliseconds;
        var elapsedSinceFrame = elapsed - _animationElapsed;
        if (elapsedSinceFrame < _animationIntervalMs) return;

        var framesToAdvance = (int)Math.Min(elapsedSinceFrame / _animationIntervalMs, _selectedRecord.Frames.Count);
        _animationFrame = (_animationFrame + framesToAdvance) % _selectedRecord.Frames.Count;
        _animationElapsed += (long)framesToAdvance * _animationIntervalMs;
        frameText.Text = (_animationFrame + 1).ToString();
        previewPanel.Invalidate();
    }

    private void ConfigureAnimationTimer(GrhRecord record)
    {
        if (!record.IsAnimation || record.Speed <= 0)
        {
            _drawTimer.Stop();
            return;
        }
        _animationFrame = 0;
        _animationIntervalMs = Math.Clamp(record.Speed * 25, 25, 1000);
        _drawTimer.Interval = Math.Min(15, _animationIntervalMs);
        WarmAnimationBitmaps(record);
        _animationClock.Restart();
        _animationElapsed = 0;
        _drawTimer.Start();
    }

    private void WarmAnimationBitmaps(GrhRecord animation)
    {
        var graphicsFolder = ResolvePath(_settings.GraphicsPath);
        foreach (var frameId in animation.Frames)
        {
            var frame = _file?.Records.FirstOrDefault(x => x.Id == frameId);
            if (frame is null) continue;
            var bitmapPath = Path.Combine(graphicsFolder, $"{frame.FileNumber}.bmp");
            if (File.Exists(bitmapPath)) _ = GetCachedBitmap(bitmapPath);
        }
    }

    private GrhRecord? GetPreviewRecord(out string? error)
    {
        error = null;
        if (_selectedRecord is null) return null;
        if (!_selectedRecord.IsAnimation) return _selectedRecord;
        if (_file is null)
        {
            error = "No hay un índice gráfico cargado para resolver la animación.";
            return null;
        }
        if (_animationFrame < 0 || _animationFrame >= _selectedRecord.Frames.Count)
        {
            error = $"Frame fuera de rango: {_animationFrame + 1}.";
            return null;
        }
        var frameId = _selectedRecord.Frames[_animationFrame];
        var frame = _file.Records.FirstOrDefault(x => x.Id == frameId);
        if (frame is null) error = $"No existe el GRH de frame {frameId}.";
        return frame;
    }

    private void previewPanel_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.Clear(previewPanel.BackColor);
        if (_selectedRecord is null) return;

        var record = GetPreviewRecord(out var frameError);
        if (record is null)
        {
            using var errorBrush = new SolidBrush(Color.DarkRed);
            e.Graphics.DrawString(frameError ?? "No hay un recurso seleccionado.", Font, errorBrush, new PointF(12, 12));
            return;
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
            var bitmap = GetCachedBitmap(bitmapPath);
            var source = new Rectangle(record.X, record.Y, record.Width, record.Height);
            if (source.Width <= 0 || source.Height <= 0)
            {
                using var invalidBrush = new SolidBrush(Color.DarkRed);
                e.Graphics.DrawString($"GRH {record.Id}: dimensiones inválidas ({source.Width}x{source.Height}).", Font, invalidBrush, new PointF(12, 12));
                return;
            }
            if (source.X < 0 || source.Y < 0 || source.Right > bitmap.Width || source.Bottom > bitmap.Height)
            {
                using var invalidBrush = new SolidBrush(Color.DarkRed);
                e.Graphics.DrawString($"GRH {record.Id}: recorte fuera de BMP {record.FileNumber} ({bitmap.Width}x{bitmap.Height}).", Font, invalidBrush, new PointF(12, 12));
                return;
            }
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
        _directionalRecords.Clear();
        _directionalPath = null;
        _iniDirectionalPath = null;
        var path = Path.Combine(ResolvePath(_settings.InitPath), "Personajes.ind");
        if (!File.Exists(path)) { statusLabel.Text = $"No se encontró: {path}"; return; }
        try { _directionalRecords.AddRange(LoadDirectionalAutoWithFormat(path, includesOffsets: true, out _directionalFormat)); _directionalPath = path; }
        catch (InvalidDataException ex) { statusLabel.Text = $"No se pudo cargar Bodies: {ex.Message}"; return; }
        foreach (var item in _directionalRecords)
            _bodyRecords.Add(new BodyIndexRecord { Id = item.Id, Walk1 = item.Directions[0], Walk2 = item.Directions[1], Walk3 = item.Directions[2], Walk4 = item.Directions[3], HeadOffsetX = item.OffsetX, HeadOffsetY = item.OffsetY });
    }

    private void LoadDirectionalState(string state)
    {
        _directionalRecords.Clear();
        var baseName = state switch
        {
            "Cabezas" => "Cabezas",
            "Cascos" => "Cascos",
            "Alas" => "Alas",
            "Capas" => "Capas",
            "Botas" => "Botas",
            _ => string.Empty
        };
        if (string.IsNullOrEmpty(baseName)) return;
        var initPath = ResolvePath(_settings.InitPath);
        var binaryPath = Path.Combine(initPath, baseName + ".ind");
        var iniPath = Path.Combine(initPath, baseName + ".dat");
        _directionalPath = null;
        _iniDirectionalPath = null;
        try
        {
            if (File.Exists(binaryPath))
            {
                _directionalRecords.AddRange(LoadDirectionalAutoWithFormat(binaryPath, includesOffsets: state is "Alas" or "Botas", out _directionalFormat));
                _directionalPath = binaryPath;
            }
            else if (File.Exists(iniPath))
            {
                _directionalRecords.AddRange(IniDirectionalIndexFile.Load(iniPath, state));
                _iniDirectionalPath = iniPath;
            }
            else
                statusLabel.Text = $"No se encontró {baseName}.ind ni {baseName}.dat en {initPath}.";
        }
        catch (InvalidDataException ex) { statusLabel.Text = $"No se pudo cargar {StateCaption(state)}: {ex.Message}"; }
    }

    private void LoadIniDirectionalState(string state)
    {
        _directionalRecords.Clear();
        var fileName = state == "Armas" ? "Armas.dat" : "Escudos.dat";
        var path = Path.Combine(ResolvePath(_settings.InitPath), fileName);
        if (!File.Exists(path)) { statusLabel.Text = $"No se encontró: {path}"; return; }
        _directionalPath = null;
        _iniDirectionalPath = path;
        try { _directionalRecords.AddRange(IniDirectionalIndexFile.Load(path, state)); }
        catch (InvalidDataException ex) { statusLabel.Text = $"No se pudo cargar {StateCaption(state)}: {ex.Message}"; }
    }

    private void LoadFxState()
    {
        _fxRecords.Clear();
        _fxPath = null;
        var initPath = ResolvePath(_settings.InitPath);
        foreach (var fileName in new[] { "Fxs.ind", "Fxsz.ind" })
        {
            var path = Path.Combine(initPath, fileName);
            if (!File.Exists(path)) continue;
            try
            {
                _fxRecords.AddRange(LoadFxAutoWithFormat(path, out _fxFormat));
                _fxPath = path;
                if (_fxRecords.Count > 0) return;
            }
            catch (InvalidDataException)
            {
            }
        }
    }

    private static List<DirectionalIndexRecord> LoadDirectionalAutoWithFormat(string path, bool includesOffsets, out GrhStorageFormat format)
    {
        try
        {
            format = GrhStorageFormat.LegacyInteger;
            return DirectionalIndexFile.Load(path, format, includesOffsets);
        }
        catch (InvalidDataException)
        {
            format = GrhStorageFormat.LongIndexes;
            return DirectionalIndexFile.Load(path, format, includesOffsets);
        }
    }

    private static List<FxIndexRecord> LoadFxAutoWithFormat(string path, out GrhStorageFormat format)
    {
        try
        {
            format = GrhStorageFormat.LegacyInteger;
            return FxIndexFile.Load(path, format);
        }
        catch (InvalidDataException)
        {
            format = GrhStorageFormat.LongIndexes;
            return FxIndexFile.Load(path, format);
        }
    }

    private void RefreshBodyList()
    {
        indexList.Items.Clear();
        foreach (var body in _bodyRecords) indexList.Items.Add(body.Id);
        ComprobarIndexLista();
    }

    private void RefreshDirectionalList()
    {
        indexList.Items.Clear();
        foreach (var item in _directionalRecords) indexList.Items.Add(item.Id);
        ComprobarIndexLista();
    }

    private void ClickDirectionalEnLista()
    {
        if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _directionalRecords.Count) return;
        var item = _directionalRecords[indexList.SelectedIndex];
        _lastIndexByState[_indexerState] = indexList.SelectedIndex;
        directionCombo.SelectedIndex = Math.Clamp(directionCombo.SelectedIndex, 0, 3);
        var direction = directionCombo.SelectedIndex;
        grhText.Text = item.Id.ToString();
        bmpText.Text = item.Directions[direction].ToString();
        xText.Text = item.OffsetX.ToString();
        yText.Text = item.OffsetY.ToString();
        widthText.Text = item.Directions[1].ToString();
        heightText.Text = item.Directions[2].ToString();
        framesText.Text = string.Join("-", item.Directions);
        referencesText.Text = CountReferences(item.Directions[direction]).ToString();
        _selectedRecord = _file?.Records.FirstOrDefault(x => x.Id == item.Directions[direction]);
        frameText.Text = "1";
        if (_selectedRecord is not null) ConfigureAnimationTimer(_selectedRecord);
        previewPanel.Invalidate();
        statusLabel.Text = $"{StateCaption(_indexerState)} {item.Id} seleccionado.";
    }

    private void directionCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_indexerState is "Body" or "Cabezas" or "Cascos" or "Alas" or "Capas" or "Botas")
            ClickDirectionalEnLista();
    }

    private void ClickFxEnLista()
    {
        if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _fxRecords.Count) return;
        var item = _fxRecords[indexList.SelectedIndex];
        _lastIndexByState[_indexerState] = indexList.SelectedIndex;
        grhText.Text = item.Id.ToString();
        bmpText.Text = item.Animation.ToString();
        xText.Text = item.OffsetX.ToString();
        yText.Text = item.OffsetY.ToString();
        framesText.Text = "";
        referencesText.Text = CountReferences(item.Animation).ToString();
        _selectedRecord = _file?.Records.FirstOrDefault(x => x.Id == item.Animation);
        if (_selectedRecord is not null) ConfigureAnimationTimer(_selectedRecord);
        previewPanel.Invalidate();
        statusLabel.Text = $"FX {item.Id} seleccionado.";
    }
}
