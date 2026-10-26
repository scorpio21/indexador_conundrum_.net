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
    private int _savedIndexSlot;

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
        edicionMenu.DropDownItems.Add(MenuItem("Mover", (_, _) => MoveOrCopySelectedIndex(deleteOriginal: true)));
        edicionMenu.DropDownItems.Add(MenuItem("Copiar", (_, _) => MoveOrCopySelectedIndex(deleteOriginal: false)));
        edicionMenu.DropDownItems.Add(MenuItem("Suprimir", deleteButton_Click, Keys.Delete));
        edicionMenu.DropDownItems.Add(MenuItem("Clonar...", (_, _) => CloneSelectedIndex()));
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

    private void SelectSavedIndex(int slot)
    {
        _savedIndexSlot = Math.Clamp(slot, 0, 9);
        statusLabel.Text = $"Índice de guardado seleccionado: {_savedIndexSlot}.";
        SetUtilError(_savedIndexSlot == 0
            ? "El guardado sobrescribirá el índice cargado."
            : $"El guardado creará una copia numerada ({_savedIndexSlot}) junto al índice cargado.");
    }

    private string GetSavedGrhPath()
    {
        var basePath = _loadedPath;
        if (string.IsNullOrWhiteSpace(basePath))
            basePath = Path.Combine(ResolvePath(_settings.InitPath), "Graficos.ind");
        if (_savedIndexSlot == 0) return basePath;

        var directory = Path.GetDirectoryName(basePath) ?? ResolvePath(_settings.InitPath);
        var name = Path.GetFileNameWithoutExtension(basePath);
        return Path.Combine(directory, $"{name}{_savedIndexSlot}.ind");
    }
    private void ShowPending(string action) => MessageBox.Show(this, $"La acción «{action}» está preparada en el menú y se conectará al módulo correspondiente.", "AOSpain");

    private void SetUtilError(string? message)
    {
        LUlitError.Text = message ?? string.Empty;
        LUlitError.ForeColor = string.IsNullOrWhiteSpace(message) ? Color.Black : Color.DarkRed;
    }

    private void ClearUtilError() => SetUtilError(string.Empty);

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
            ["Alas"] = "Alas.ico"
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
        if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            OpenAuxiliaryIndex();
            return;
        }

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

    private void OpenAuxiliaryIndex()
    {
        if (_indexerState.Equals("Resource", StringComparison.OrdinalIgnoreCase))
        {
            SetUtilError("Resource no tiene un índice editable para cargar.");
            return;
        }

        var isIni = _indexerState is "Armas" or "Escudos";
        var filter = isIni
            ? "Archivo de datos (*.dat)|*.dat|Todos los archivos (*.*)|*.*"
            : "Índices auxiliares (*.ind;*.dat)|*.ind;*.dat|Todos los archivos (*.*)|*.*";
        using var dialog = new OpenFileDialog
        {
            Filter = filter,
            FileName = isIni ? $"{StateCaption(_indexerState)}.dat" : $"{StateCaption(_indexerState)}.ind",
            InitialDirectory = ResolvePath(_settings.InitPath)
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            _directionalRecords.Clear();
            _fxRecords.Clear();
            _bodyRecords.Clear();
            _directionalPath = null;
            _iniDirectionalPath = null;
            _fxPath = null;

            if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
            {
                _fxRecords.AddRange(LoadFxAutoWithFormat(dialog.FileName, out _fxFormat));
                _fxPath = dialog.FileName;
                RefreshFxList();
            }
            else if (isIni || Path.GetExtension(dialog.FileName).Equals(".dat", StringComparison.OrdinalIgnoreCase))
            {
                _directionalRecords.AddRange(IniDirectionalIndexFile.Load(dialog.FileName, _indexerState));
                _iniDirectionalPath = dialog.FileName;
                if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase)) RefreshBodyList();
                else RefreshDirectionalList();
            }
            else
            {
                var includesOffsets = _indexerState is "Body" or "Alas" or "Botas";
                _directionalRecords.AddRange(LoadDirectionalAutoWithFormat(dialog.FileName, includesOffsets, out _directionalFormat));
                _directionalPath = dialog.FileName;
                if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var item in _directionalRecords)
                        _bodyRecords.Add(new BodyIndexRecord { Id = item.Id, Walk1 = item.Directions[0], Walk2 = item.Directions[1], Walk3 = item.Directions[2], Walk4 = item.Directions[3], HeadOffsetX = item.OffsetX, HeadOffsetY = item.OffsetY });
                    RefreshBodyList();
                }
                else RefreshDirectionalList();
            }

            _lastIndexByState[_indexerState] = -1;
            RebuildReferenceCounts();
            ClearUtilError();
            statusLabel.Text = $"Cargado {StateCaption(_indexerState)}: {dialog.FileName}";
        }
        catch (Exception ex)
        {
            SetUtilError($"No se pudo cargar {StateCaption(_indexerState)}: {ex.Message}");
            statusLabel.Text = "Error al cargar el índice auxiliar.";
        }
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
        foreach (var record in _file.Records.OrderBy(record => record.Id))
            indexList.Items.Add(FormatGrhLabel(record));
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

    private void ResetGrhValidationColors()
    {
        foreach (var textBox in new[] { bmpText, frameText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText })
            textBox.BackColor = Color.White;
    }

    private static void MarkValidation(TextBox textBox, Color color) => textBox.BackColor = color;

    private void ValidateSelectedGrh(GrhRecord record)
    {
        ResetGrhValidationColors();
        var errors = new List<string>();
        var graphicsFolder = ResolvePath(_settings.GraphicsPath);

        if (record.Frames.Count <= 0)
        {
            MarkValidation(framesText, Color.Red);
            errors.Add("Nº de frames incorrecto.");
            SetUtilError(string.Join(" ", errors));
            return;
        }

        if (record.IsAnimation)
        {
            var firstFrame = _file?.Records.FirstOrDefault(item => item.Id == record.Frames[0]);
            var firstSize = firstFrame is null ? (Width: 0, Height: 0) : (firstFrame.Width, firstFrame.Height);
            var invalidFrame = false;
            var differentSize = false;

            for (var i = 0; i < record.Frames.Count; i++)
            {
                var frameId = record.Frames[i];
                var frame = _file?.Records.FirstOrDefault(item => item.Id == frameId);
                var frameIsValid = frame is not null && !frame.IsAnimation && frame.Width > 0 && frame.Height > 0;
                if (!frameIsValid)
                {
                    invalidFrame = true;
                    errors.Add($"El frame nº {i + 1} es incorrecto.");
                    continue;
                }

                if (i > 0 && (frame!.Width != firstSize.Width || frame.Height != firstSize.Height))
                {
                    differentSize = true;
                    errors.Add($"El frame nº {i + 1} tiene distintas dimensiones.");
                }

                var framePath = Path.Combine(graphicsFolder, $"{frame!.FileNumber}.bmp");
                if (!File.Exists(framePath))
                {
                    invalidFrame = true;
                    errors.Add($"Falta el BMP del frame nº {i + 1} ({frame.FileNumber}.bmp).");
                }
            }

            if (invalidFrame) MarkValidation(frameText, Color.Red);
            else if (differentSize) MarkValidation(frameText, Color.Yellow);

            SetUtilError(string.Join(Environment.NewLine, errors));
            return;
        }

        var bitmapPath = Path.Combine(graphicsFolder, $"{record.FileNumber}.bmp");
        if (!File.Exists(bitmapPath))
        {
            MarkValidation(bmpText, Color.Red);
            errors.Add($"El archivo {record.FileNumber}.bmp no existe.");
        }
        else
        {
            try
            {
                var bitmap = GetCachedBitmap(bitmapPath);
                if (record.X < 0 || record.X > bitmap.Width)
                {
                    MarkValidation(xText, Color.Red);
                    errors.Add("Posición X fuera del BMP.");
                }
                if (record.Y < 0 || record.Y > bitmap.Height)
                {
                    MarkValidation(yText, Color.Red);
                    errors.Add("Posición Y fuera del BMP.");
                }
                if (record.Y >= 0 && record.Y + record.Height > bitmap.Height)
                {
                    MarkValidation(heightText, Color.Yellow);
                    errors.Add("Alto fuera del BMP.");
                }
                if (record.X >= 0 && record.X + record.Width > bitmap.Width)
                {
                    MarkValidation(widthText, Color.Yellow);
                    errors.Add("Ancho fuera del BMP.");
                }
            }
            catch (Exception ex)
            {
                MarkValidation(bmpText, Color.Red);
                errors.Add($"No se pudo comprobar el BMP: {ex.Message}");
            }
        }

        SetUtilError(string.Join(" ", errors));
    }

    private void UpdateUi()
    {
        var canSaveState = _indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase)
            || _directionalPath is not null || _iniDirectionalPath is not null || _fxPath is not null;
        saveButton.Enabled = _file is not null && canSaveState;
        UpdateSaveCaption();
        statusLabel.Text = _file is null ? "Abre un Graficos.ind para comenzar." : $"GRH: {_file.Records.Count}";
    }

    private void UpdateSaveCaption()
    {
        var baseName = _indexerState switch
        {
            "Grh" => "Graficos",
            "Body" => "Personajes",
            "Cabezas" => "Cabezas",
            "Cascos" => "Cascos",
            "Escudos" => "Escudos",
            "Armas" => "Armas",
            "Alas" => "Alas",
            "Capas" => "Capas",
            "Fx" => "Fxs",
            "Botas" => "Botas",
            _ => "Índice"
        };
        var extension = _indexerState is "Armas" or "Escudos" ? ".dat" : ".ind";
        saveIndexButton.Text = $"Guardar {baseName}{extension}";
    }

    private void indexList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (indexList.SelectedIndex < 0) return;
        ClickEnLista();
    }

    private void ClickEnLista()
    {
        ClearUtilError();
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
        var frameData = _selectedRecord.IsAnimation
            ? _file.Records.FirstOrDefault(x => x.Id == _selectedRecord.Frames[0]) ?? _selectedRecord
            : _selectedRecord;
        grhText.Text = _selectedRecord.Id.ToString();
        bmpText.Text = _selectedRecord.FileNumber.ToString();
        xText.Text = _selectedRecord.X.ToString();
        yText.Text = _selectedRecord.Y.ToString();
        widthText.Text = frameData.Width.ToString();
        heightText.Text = frameData.Height.ToString();
        speedText.Text = _selectedRecord.Speed.ToString();
        framesText.Text = _selectedRecord.Frames.Count.ToString();
        frameText.Text = _selectedRecord.IsAnimation ? string.Join("-", _selectedRecord.Frames) : "";
        tileHeightText.Text = frameData.Height > 0 ? (frameData.Height / 32).ToString() : "";
        tileWidthText.Text = frameData.Width > 0 ? (frameData.Width / 32).ToString() : "";
        referencesText.Text = CountReferences(_selectedRecord.Id).ToString();
        ValidateSelectedGrh(_selectedRecord);
        _drawTimer.Enabled = _selectedRecord.IsAnimation;
        ConfigureAnimationTimer(_selectedRecord);
        previewPanel.Invalidate();
        statusLabel.Text = $"GRH {_selectedRecord.Id} seleccionado ({_indexerState}).";
    }

    private void DeleteSelected()
    {
        if (indexList.SelectedIndex < 0) return;

        if (_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            if (_file is null) return;
            var record = SelectedGrhRecord();
            if (record is null) return;
            var answer = MessageBox.Show(this, $"¿Borrar el GRH {record.Id}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return;
            _file.Records.Remove(record);
            RefreshGrid();
            ClearUtilError();
            statusLabel.Text = $"GRH {record.Id} eliminado de la memoria. Guarda los cambios para escribir el índice.";
            return;
        }

        if (_indexerState.Equals("Resource", StringComparison.OrdinalIgnoreCase))
        {
            SetUtilError("Los recursos no se pueden borrar desde el índice gráfico.");
            return;
        }

        var selectedId = indexList.SelectedItem?.ToString() ?? string.Empty;
        var answerAux = MessageBox.Show(this, $"¿Borrar {StateCaption(_indexerState)} {selectedId}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answerAux != DialogResult.Yes) return;

        if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
        {
            if (indexList.SelectedIndex >= _fxRecords.Count) return;
            _fxRecords.RemoveAt(indexList.SelectedIndex);
            RefreshFxList();
        }
        else
        {
            if (indexList.SelectedIndex >= _directionalRecords.Count) return;
            var id = _directionalRecords[indexList.SelectedIndex].Id;
            _directionalRecords.RemoveAt(indexList.SelectedIndex);
            if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
                _bodyRecords.RemoveAll(record => record.Id == id);
            if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase)) RefreshBodyList();
            else RefreshDirectionalList();
        }

        RebuildReferenceCounts();
        _selectedRecord = null;
        _drawTimer.Stop();
        previewPanel.Invalidate();
        ClearUtilError();
        statusLabel.Text = $"{StateCaption(_indexerState)} {selectedId} eliminado de la memoria. Guarda los cambios para escribir el índice.";
    }

    private void MoveOrCopySelectedGrh(bool deleteOriginal)
    {
        if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase) || _file is null)
        {
            SetUtilError($"{(deleteOriginal ? "Mover" : "Copiar")} solo está disponible actualmente para Gráficos.");
            return;
        }

        var source = SelectedGrhRecord();
        if (source is null)
        {
            SetUtilError("Selecciona primero un GRH.");
            return;
        }

        var targetText = Microsoft.VisualBasic.Interaction.InputBox(
            $"Introduzca número de GRH al que quieres {(deleteOriginal ? "mover" : "copiar")} el gráfico {source.Id}.",
            deleteOriginal ? "Mover gráfico" : "Copiar gráfico", source.Id.ToString());
        if (string.IsNullOrWhiteSpace(targetText)) return;
        if (!int.TryParse(targetText, out var targetId) || targetId <= 0 || targetId > ParseInt(maxGrhText.Text))
        {
            SetUtilError("Introduzca un número de GRH válido.");
            return;
        }
        if (targetId == source.Id)
        {
            SetUtilError("El GRH de destino debe ser diferente del origen.");
            return;
        }

        var existing = _file.Records.FirstOrDefault(record => record.Id == targetId);
        if (existing is not null)
        {
            var answer = MessageBox.Show(this, $"El gráfico {targetId} ya existe. ¿Deseas sobrescribirlo?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return;
            _file.Records.Remove(existing);
        }

        var copy = CloneGrh(source, targetId);
        _file.Records.Add(copy);
        if (deleteOriginal) _file.Records.Remove(source);

        RefreshGrid();
        SelectGrhById(targetId);
        ClearUtilError();
        statusLabel.Text = $"GRH {source.Id} {(deleteOriginal ? "movido" : "copiado")} a {targetId}. Guarda los cambios para escribir el índice.";
    }

    private void MoveOrCopySelectedIndex(bool deleteOriginal)
    {
        if (_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            MoveOrCopySelectedGrh(deleteOriginal);
            return;
        }
        if (_indexerState.Equals("Resource", StringComparison.OrdinalIgnoreCase))
        {
            SetUtilError($"{(deleteOriginal ? "Mover" : "Copiar")} no está disponible para recursos.");
            return;
        }
        if (indexList.SelectedIndex < 0)
        {
            SetUtilError("Selecciona primero un índice.");
            return;
        }

        var sourceId = int.TryParse(indexList.SelectedItem?.ToString(), out var parsedId) ? parsedId : 0;
        if (sourceId <= 0)
        {
            SetUtilError("El índice seleccionado no es válido.");
            return;
        }
        var targetText = Microsoft.VisualBasic.Interaction.InputBox(
            $"Introduzca número de {StateCaption(_indexerState)} al que quieres {(deleteOriginal ? "mover" : "copiar")} el índice {sourceId}.",
            deleteOriginal ? $"Mover {StateCaption(_indexerState)}" : $"Copiar {StateCaption(_indexerState)}", sourceId.ToString());
        if (string.IsNullOrWhiteSpace(targetText)) return;
        if (!int.TryParse(targetText, out var targetId) || targetId <= 0 || targetId >= 31000)
        {
            SetUtilError("Introduzca un número de índice válido.");
            return;
        }
        if (targetId == sourceId)
        {
            SetUtilError("El índice de destino debe ser diferente del origen.");
            return;
        }

        if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
        {
            var source = _fxRecords.FirstOrDefault(record => record.Id == sourceId);
            if (source is null) { SetUtilError("No se encontró el FX seleccionado."); return; }
            var existing = _fxRecords.FirstOrDefault(record => record.Id == targetId);
            if (existing is not null && !ConfirmOverwrite(targetId)) return;
            if (existing is not null) _fxRecords.Remove(existing);
            _fxRecords.Add(new FxIndexRecord { Id = targetId, Animation = source.Animation, OffsetX = source.OffsetX, OffsetY = source.OffsetY });
            if (deleteOriginal) _fxRecords.Remove(source);
            RefreshFxList();
        }
        else
        {
            var source = _directionalRecords.FirstOrDefault(record => record.Id == sourceId);
            if (source is null) { SetUtilError("No se encontró el índice seleccionado."); return; }
            var existing = _directionalRecords.FirstOrDefault(record => record.Id == targetId);
            if (existing is not null && !ConfirmOverwrite(targetId)) return;
            if (existing is not null)
            {
                _directionalRecords.Remove(existing);
                if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
                    _bodyRecords.RemoveAll(record => record.Id == targetId);
            }
            _directionalRecords.Add(new DirectionalIndexRecord
            {
                Id = targetId,
                Directions = source.Directions.ToArray(),
                OffsetX = source.OffsetX,
                OffsetY = source.OffsetY
            });
            if (deleteOriginal)
            {
                _directionalRecords.Remove(source);
                if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
                    _bodyRecords.RemoveAll(record => record.Id == sourceId);
            }
            if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase)) RefreshBodyList();
            else RefreshDirectionalList();
        }

        RebuildReferenceCounts();
        SelectGrhById(targetId);
        ClearUtilError();
        statusLabel.Text = $"{StateCaption(_indexerState)} {sourceId} {(deleteOriginal ? "movido" : "copiado")} a {targetId}. Guarda los cambios para escribir el índice.";
    }

    private bool ConfirmOverwrite(int targetId) => MessageBox.Show(this,
        $"El índice {targetId} ya existe. ¿Deseas sobrescribirlo?", "Aviso",
        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

    private void CloneSelectedGrh()
    {
        if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase) || _file is null)
        {
            SetUtilError("Clonar solo está disponible actualmente para Gráficos.");
            return;
        }

        var source = SelectedGrhRecord();
        if (source is null)
        {
            SetUtilError("Selecciona primero un GRH.");
            return;
        }

        var firstText = Microsoft.VisualBasic.Interaction.InputBox(
            $"Introduzca el primer número de GRH para clonar {source.Id}.", "Clonar gráfico", (source.Id + 1).ToString());
        if (string.IsNullOrWhiteSpace(firstText)) return;
        var countText = Microsoft.VisualBasic.Interaction.InputBox(
            $"Introduzca la cantidad de copias de {source.Id}.", "Clonar gráfico", "1");
        if (string.IsNullOrWhiteSpace(countText)) return;
        if (!int.TryParse(firstText, out var firstId) || !int.TryParse(countText, out var count) || firstId <= 0 || count <= 0)
        {
            SetUtilError("Introduzca un número inicial y una cantidad válidos.");
            return;
        }
        if (firstId > ParseInt(maxGrhText.Text) || count > ParseInt(maxGrhText.Text) - firstId + 1)
        {
            SetUtilError("El rango de clonación está fuera de los límites del índice.");
            return;
        }

        var targets = Enumerable.Range(firstId, count).ToList();
        var occupied = targets.Where(target => _file.Records.Any(record => record.Id == target)).ToList();
        if (occupied.Count > 0)
        {
            var answer = MessageBox.Show(this, $"Ya existen los GRH: {string.Join(", ", occupied)}. ¿Deseas sobrescribirlos?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return;
            _file.Records.RemoveAll(record => occupied.Contains(record.Id));
        }

        foreach (var targetId in targets)
            _file.Records.Add(CloneGrh(source, targetId));

        RefreshGrid();
        SelectGrhById(firstId);
        ClearUtilError();
        statusLabel.Text = $"Se clonó el GRH {source.Id} {count} vez/veces desde {firstId}. Guarda los cambios para escribir el índice.";
    }

    private void CloneSelectedIndex()
    {
        if (_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
        {
            CloneSelectedGrh();
            return;
        }
        if (_indexerState.Equals("Resource", StringComparison.OrdinalIgnoreCase))
        {
            SetUtilError("Clonar no está disponible para recursos.");
            return;
        }
        if (indexList.SelectedIndex < 0)
        {
            SetUtilError("Selecciona primero un índice.");
            return;
        }

        var sourceId = int.TryParse(indexList.SelectedItem?.ToString(), out var parsedId) ? parsedId : 0;
        if (sourceId <= 0) { SetUtilError("El índice seleccionado no es válido."); return; }
        var firstText = Microsoft.VisualBasic.Interaction.InputBox(
            $"Introduzca el primer número de {StateCaption(_indexerState)} para clonar {sourceId}.", "Clonar índice", (sourceId + 1).ToString());
        if (string.IsNullOrWhiteSpace(firstText)) return;
        var countText = Microsoft.VisualBasic.Interaction.InputBox("Introduzca la cantidad de copias.", "Clonar índice", "1");
        if (string.IsNullOrWhiteSpace(countText)) return;
        if (!int.TryParse(firstText, out var firstId) || !int.TryParse(countText, out var count) || firstId <= 0 || count <= 0 || firstId >= 31000 || count > 31000 - firstId)
        {
            SetUtilError("El rango de clonación no es válido.");
            return;
        }

        var targets = Enumerable.Range(firstId, count).ToList();
        var occupied = targets.Where(target => indexList.Items.Cast<object>().Any(item => string.Equals(item.ToString(), target.ToString(), StringComparison.OrdinalIgnoreCase))).ToList();
        if (occupied.Count > 0 && MessageBox.Show(this, $"Ya existen los índices: {string.Join(", ", occupied)}. ¿Deseas sobrescribirlos?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
        {
            var source = _fxRecords.FirstOrDefault(record => record.Id == sourceId);
            if (source is null) { SetUtilError("No se encontró el FX seleccionado."); return; }
            _fxRecords.RemoveAll(record => targets.Contains(record.Id));
            foreach (var target in targets)
                _fxRecords.Add(new FxIndexRecord { Id = target, Animation = source.Animation, OffsetX = source.OffsetX, OffsetY = source.OffsetY });
            RefreshFxList();
        }
        else
        {
            var source = _directionalRecords.FirstOrDefault(record => record.Id == sourceId);
            if (source is null) { SetUtilError("No se encontró el índice seleccionado."); return; }
            _directionalRecords.RemoveAll(record => targets.Contains(record.Id));
            foreach (var target in targets)
                _directionalRecords.Add(new DirectionalIndexRecord { Id = target, Directions = source.Directions.ToArray(), OffsetX = source.OffsetX, OffsetY = source.OffsetY });
            if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
            {
                _bodyRecords.RemoveAll(record => targets.Contains(record.Id));
                foreach (var target in targets)
                    _bodyRecords.Add(new BodyIndexRecord { Id = target, Walk1 = source.Directions[0], Walk2 = source.Directions[1], Walk3 = source.Directions[2], Walk4 = source.Directions[3], HeadOffsetX = source.OffsetX, HeadOffsetY = source.OffsetY });
                RefreshBodyList();
            }
            else RefreshDirectionalList();
        }

        RebuildReferenceCounts();
        SelectGrhById(firstId);
        ClearUtilError();
        statusLabel.Text = $"Se clonó el índice {sourceId} {count} vez/veces desde {firstId}. Guarda los cambios para escribir el índice.";
    }

    private static GrhRecord CloneGrh(GrhRecord source, int id) => new()
    {
        Id = id,
        FileNumber = source.FileNumber,
        X = source.X,
        Y = source.Y,
        Width = source.Width,
        Height = source.Height,
        Speed = source.Speed,
        Frames = new List<int>(source.Frames)
    };

    private void SelectGrhById(int id)
    {
        for (var i = 0; i < indexList.Items.Count; i++)
        {
            var item = indexList.Items[i]?.ToString() ?? string.Empty;
            var separator = item.IndexOf(' ');
            var idText = separator >= 0 ? item[..separator] : item;
            if (!int.TryParse(idText, out var itemId) || itemId != id) continue;
            indexList.SelectedIndex = i;
            return;
        }
    }

    private GrhRecord? SelectedGrhRecord()
    {
        if (_file is null || indexList.SelectedIndex < 0) return null;
        var text = indexList.SelectedItem?.ToString() ?? string.Empty;
        var separator = text.IndexOf(' ');
        if (separator >= 0) text = text[..separator];
        return int.TryParse(text, out var id) ? _file.Records.FirstOrDefault(x => x.Id == id) : null;
    }

    private static string FormatGrhLabel(GrhRecord record) =>
        record.IsAnimation ? $"{record.Id} [animación]" : record.Id.ToString();

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
        ClearUtilError();
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
        var targetPath = GetSavedGrhPath();
        var isCopySlot = _savedIndexSlot != 0;
        if (string.IsNullOrWhiteSpace(_loadedPath) && !isCopySlot)
        {
            SaveIndexAs();
            return;
        }
        var answer = MessageBox.Show(this,
            isCopySlot ? $"¿Guardar una copia en el slot {_savedIndexSlot}?\n{targetPath}" : $"¿Sobrescribir el índice actual?\n{targetPath}",
            "Guardar índice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answer != DialogResult.Yes) return;
        try
        {
            ApplyGridToModel();
            _file.Save(targetPath, SelectedFormat);
            statusLabel.Text = isCopySlot ? $"Copia guardada en: {targetPath}" : $"Guardado: {targetPath}";
            SetUtilError(string.Empty);
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
            if (_indexerState.Equals("Resource", StringComparison.OrdinalIgnoreCase))
            {
                SetUtilError("Resource no tiene un índice editable para guardar.");
                return;
            }

            var isIni = _indexerState is "Armas" or "Escudos";
            var extension = isIni ? ".dat" : ".ind";
            var filter = isIni
                ? "Archivo de datos (*.dat)|*.dat|Todos los archivos (*.*)|*.*"
                : "Índice binario (*.ind)|*.ind|Todos los archivos (*.*)|*.*";
            var currentPath = _directionalPath ?? _iniDirectionalPath ?? _fxPath;
            using var auxiliaryDialog = new SaveFileDialog
            {
                Filter = filter,
                DefaultExt = extension.TrimStart('.'),
                AddExtension = true,
                FileName = Path.GetFileName(currentPath ?? $"{StateCaption(_indexerState)}{extension}")
            };
            if (auxiliaryDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                ApplyNonGrhToModel();
                SaveNonGrhToPath(auxiliaryDialog.FileName);
                RebuildReferenceCounts();
                statusLabel.Text = $"Guardado: {auxiliaryDialog.FileName}";
                SetUtilError(string.Empty);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo guardar el índice", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
        var sourcePath = _directionalPath ?? _iniDirectionalPath ?? _fxPath;
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            statusLabel.Text = $"No hay archivo de {StateCaption(_indexerState)} cargado para guardar.";
            return;
        }
        var path = GetSavedAuxiliaryPath(sourcePath);
        var isCopySlot = _savedIndexSlot != 0;
        var answer = MessageBox.Show(this,
            isCopySlot ? $"¿Guardar una copia en el slot {_savedIndexSlot}?\n{path}" : $"¿Sobrescribir el índice actual?\n{path}",
            "Guardar índice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answer != DialogResult.Yes) return;
        try
        {
            ApplyNonGrhToModel();
            SaveNonGrhToPath(path);
            RebuildReferenceCounts();
            statusLabel.Text = isCopySlot ? $"Copia guardada en: {path}" : $"Guardado: {path}";
            SetUtilError(string.Empty);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo guardar el índice", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private string GetSavedAuxiliaryPath(string sourcePath)
    {
        if (_savedIndexSlot == 0) return sourcePath;
        var directory = Path.GetDirectoryName(sourcePath) ?? ResolvePath(_settings.InitPath);
        var name = Path.GetFileNameWithoutExtension(sourcePath);
        var extension = Path.GetExtension(sourcePath);
        return Path.Combine(directory, $"{name}{_savedIndexSlot}{extension}");
    }

    private void SaveNonGrhToPath(string path)
    {
        if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
        {
            FxIndexFile.Save(path, _fxRecords, _fxFormat);
            _fxPath = path;
            _directionalPath = null;
            _iniDirectionalPath = null;
            return;
        }
        if (_indexerState is "Armas" or "Escudos" || _iniDirectionalPath is not null)
        {
            IniDirectionalIndexFile.Save(path, _directionalRecords, _indexerState);
            _iniDirectionalPath = path;
            _directionalPath = null;
            _fxPath = null;
            return;
        }

        DirectionalIndexFile.Save(path, _directionalRecords, _directionalFormat, _indexerState is "Body" or "Alas" or "Botas");
        _directionalPath = path;
        _iniDirectionalPath = null;
        _fxPath = null;
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
        if (_searchResults.Count == 0) { SetUtilError("No hay una búsqueda activa."); statusLabel.Text = "Buscar siguiente: sin búsqueda activa."; return; }
        _searchCursor = (_searchCursor + 1) % _searchResults.Count;
        indexList.SelectedIndex = _searchCursor;
        if (_searchCursor == 0 && _searchResults.Count > 1)
            SetUtilError("Se terminó la búsqueda; se ha vuelto al primer resultado.");
        else
            SetUtilError("F3 para continuar la búsqueda.");
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
        foreach (var id in ids)
        {
            var record = _file?.Records.FirstOrDefault(x => x.Id == id);
            indexList.Items.Add(record is null ? id.ToString() : FormatGrhLabel(record));
        }
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
        ClearUtilError();
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
        _animationIntervalMs = Math.Clamp((int)Math.Round(record.Speed * (1000D / 60D)), 16, 1000);
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
        if (_selectedRecord is null)
        {
            ClearUtilError();
            return;
        }

        var record = GetPreviewRecord(out var frameError);
        if (record is null)
        {
            SetUtilError(frameError ?? "No hay un recurso seleccionado.");
            using var errorBrush = new SolidBrush(Color.DarkRed);
            e.Graphics.DrawString(frameError ?? "No hay un recurso seleccionado.", Font, errorBrush, new PointF(12, 12));
            return;
        }

        var graphicsFolder = ResolvePath(_settings.GraphicsPath);
        var bitmapPath = Path.Combine(graphicsFolder, $"{record.FileNumber}.bmp");
        if (!File.Exists(bitmapPath))
        {
            SetUtilError($"BMP {record.FileNumber} no encontrado: {bitmapPath}");
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
                SetUtilError($"GRH {record.Id}: dimensiones inválidas ({source.Width}x{source.Height}).");
                using var invalidBrush = new SolidBrush(Color.DarkRed);
                e.Graphics.DrawString($"GRH {record.Id}: dimensiones inválidas ({source.Width}x{source.Height}).", Font, invalidBrush, new PointF(12, 12));
                return;
            }
            if (source.X < 0 || source.Y < 0 || source.Right > bitmap.Width || source.Bottom > bitmap.Height)
            {
                SetUtilError($"GRH {record.Id}: recorte fuera de BMP {record.FileNumber} ({bitmap.Width}x{bitmap.Height}).");
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
            ClearUtilError();
        }
        catch (Exception ex)
        {
            SetUtilError($"No se pudo dibujar el BMP: {ex.Message}");
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
        NewOrGoToIndex();
    }

    private void NewOrGoToIndex()
    {
        if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase) || _file is null)
        {
            SetUtilError("Nuevo/Ir A está implementado actualmente para Gráficos.");
            return;
        }

        var value = Microsoft.VisualBasic.Interaction.InputBox(
            "Introduzca el número de GRH (0 para encontrar el primer hueco libre).",
            "Nuevo gráfico / Ir a", "0");
        if (string.IsNullOrWhiteSpace(value)) return;
        if (!int.TryParse(value, out var requestedId) || requestedId < 0 || requestedId > ParseInt(maxGrhText.Text))
        {
            SetUtilError("Introduzca un número de GRH válido.");
            return;
        }

        var targetId = requestedId == 0 ? FindFirstFreeGrhId() : requestedId;
        if (targetId <= 0)
        {
            SetUtilError("No se encontró hueco libre.");
            return;
        }

        if (_file.Records.Any(record => record.Id == targetId))
        {
            SelectGrhById(targetId);
            ClearUtilError();
            statusLabel.Text = $"GRH {targetId} seleccionado.";
            return;
        }

        var answer = MessageBox.Show(this, $"El GRH {targetId} no existe. ¿Quieres crearlo?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes) return;

        _file.Records.Add(new GrhRecord
        {
            Id = targetId,
            FileNumber = 1,
            Width = 32,
            Height = 32,
            Frames = new List<int> { targetId }
        });
        RebuildReferenceCounts();
        RefreshGrid();
        SelectGrhById(targetId);
        ClearUtilError();
        statusLabel.Text = $"GRH {targetId} creado en memoria. Completa sus datos y guarda el índice.";
    }

    private int FindFirstFreeGrhId()
    {
        var used = _file?.Records.Select(record => record.Id).ToHashSet() ?? new HashSet<int>();
        var max = ParseInt(maxGrhText.Text);
        for (var id = 1; id <= max; id++)
            if (!used.Contains(id)) return id;
        return 0;
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

    private void RefreshFxList()
    {
        indexList.Items.Clear();
        foreach (var item in _fxRecords) indexList.Items.Add(item.Id);
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
