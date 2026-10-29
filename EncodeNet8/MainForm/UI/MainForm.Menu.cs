#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
    private void savedIndexMenu_Click(object? sender, EventArgs e) => SelectSavedIndex(Convert.ToInt32(((ToolStripMenuItem)sender!).Tag));
        private void moveMenu_Click(object? sender, EventArgs e) => MoveOrCopySelectedIndex(deleteOriginal: true);
        private void copyMenu_Click(object? sender, EventArgs e) => MoveOrCopySelectedIndex(deleteOriginal: false);
        private void cloneMenu_Click(object? sender, EventArgs e) => CloneSelectedIndex();
        private void colorMenu_Click(object? sender, EventArgs e) => SelectBackgroundColor();
        private void buscarGrhMenu_Click(object? sender, EventArgs e) => SearchGrhWithBmp();
        private void buscarBmpsMenu_Click(object? sender, EventArgs e) => SearchUnindexedBmps();
        private void buscarSiguienteMenu_Click(object? sender, EventArgs e) => SearchNext();
        private void autoindexadorMenu_Click(object? sender, EventArgs e) => OpenAutoIndexer();
        private void repetidosMenu_Click(object? sender, EventArgs e) => SearchRepeatedGrh();
        private void generarWeMenu_Click(object? sender, EventArgs e) => ToggleWeMode();
        private void acercaMenu_Click(object? sender, EventArgs e) => MessageBox.Show(this, "AOSpain - Indexador de gráficos .NET 8\nBasado en Encode.frm de Conundrum.", "Ayuda");
        private void SetUsageCount(int count) => usageStatusLabel.Text = $"Veces que el GRH es utilizado: {count}";
    
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

        private void ToggleWeMode()
        {
            _weMode = !_weMode;
            var normalControlsVisible = !_weMode;
            foldersGroup.Visible = normalControlsVisible;
            encodeMapsButton.Visible = normalControlsVisible;
            viewButton.Visible = normalControlsVisible;
            clearMemoryButton.Visible = normalControlsVisible;
            saveIndexButton.Visible = normalControlsVisible;
            greenButton.Visible = normalControlsVisible;
            deleteButton.Visible = normalControlsVisible;
            surfaceGroup.Visible = _weMode;
            generateSurfaceButton.Visible = _weMode;
            surfaceNameText.Visible = _weMode;
            surfaceHeightText.Visible = _weMode;
            surfaceWidthText.Visible = _weMode;
            surfaceGrhText.Visible = _weMode;
            surfaceReferencesText.Visible = _weMode;
            surfaceDescriptionLabel.Visible = _weMode;

            if (_weMode)
            {
                surfaceDescriptionLabel.Text = "Selecciona un GRH y completa el nombre.";
                surfaceReferencesText.Text = ReadWeReferenceCount().ToString();
                if (_selectedRecord is not null)
                    UpdateSurfaceSelection(_selectedRecord);
                statusLabel.Text = "Modo superficies WE activado. Selecciona un GRH y completa los datos.";
            }
            else
            {
                ConfigureStateFields(_indexerState);
                statusLabel.Text = "Modo superficies WE desactivado.";
            }
        }

        private int ReadWeReferenceCount()
        {
            var path = ResolveConfiguredIndicesFilePath();
            if (!File.Exists(path)) return 0;
            return File.ReadLines(path)
                .Select(line => line.Trim())
                .Where(line => line.StartsWith("Referencias", StringComparison.OrdinalIgnoreCase))
                .Select(line => line.IndexOf('=') >= 0 ? line[(line.IndexOf('=') + 1)..].Trim() : string.Empty)
                .Select(value => int.TryParse(value, out var count) ? count : 0)
                .FirstOrDefault();
        }

        private void UpdateSurfaceSelection(GrhRecord record)
        {
            if (!_weMode) return;
            surfaceGrhText.Text = record.Id.ToString();
            surfaceHeightText.Text = record.Height > 0 ? record.Height.ToString() : string.Empty;
            surfaceWidthText.Text = record.Width > 0 ? record.Width.ToString() : string.Empty;
        }

        private void generateSurfaceButton_Click(object? sender, EventArgs e) => GenerateWeIndex();

        private void GenerateWeIndex()
        {
            if (_file is null || _selectedRecord is null)
            {
                SetUtilError("Selecciona primero un GRH para generar su índice WE.");
                return;
            }

            var name = surfaceNameText.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                SetUtilError("Debes indicar un nombre para la superficie.");
                return;
            }

            if (!int.TryParse(surfaceHeightText.Text.Trim(), out var requestedHeight) || requestedHeight <= 0)
            {
                SetUtilError("Indica un alto numérico válido para la superficie.");
                return;
            }

            if (!int.TryParse(surfaceWidthText.Text.Trim(), out var requestedWidth) || requestedWidth <= 0)
            {
                SetUtilError("Indica un ancho numérico válido para la superficie.");
                return;
            }

            var path = ResolveConfiguredIndicesFilePath();

            try
            {
                var frame = _file.Records.FirstOrDefault(record => record.Id == _selectedRecord.Frames.FirstOrDefault()) ?? _selectedRecord;
                var height = requestedHeight;
                var width = requestedWidth;
                if (height <= 0) height = frame.Height > 0 ? frame.Height : _selectedRecord.Height;
                if (width <= 0) width = frame.Width > 0 ? frame.Width : _selectedRecord.Width;
                var grh = ParseInt(surfaceGrhText.Text);
                if (grh <= 0) grh = _selectedRecord.Id;
                var reference = WeIndexFile.Append(path, name, grh, height, width);
                surfaceReferencesText.Text = reference.ToString();
                statusLabel.Text = $"Índice WE {reference} generado en {path}.";
                SetUtilError($"Referencia {reference} añadida correctamente en Indices.ini.");
            }
            catch (Exception ex)
            {
                SetUtilError($"No se pudo generar el índice WE: {ex.Message}");
            }
        }

        // Indicetext es un control creado desde el diseñador del formulario principal.
        // Se busca por nombre para que esta parte siga compilando mientras el diseñador
        // aún no haya guardado el campo generado; si no existe, se usa JSON/Conf.ini.
        private string ResolveConfiguredIndicesFilePath()
        {
            var control = Controls.Find("Indicetext", true).OfType<TextBox>().FirstOrDefault();
            var configuredValue = control?.Text.Trim();
            if (string.IsNullOrWhiteSpace(configuredValue))
                return _settings.ResolveIndicesFilePath(AppContext.BaseDirectory);

            var settings = new IndexerSettings { IndicesPath = configuredValue };
            return settings.ResolveIndicesFilePath(AppContext.BaseDirectory);
        }
    
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
                Indicetext.Text = _settings.IndicesPath;
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
}

#pragma warning restore CS8618

