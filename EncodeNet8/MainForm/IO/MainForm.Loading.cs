#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
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
}

#pragma warning restore CS8618

