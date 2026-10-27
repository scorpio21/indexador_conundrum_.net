#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
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
}

#pragma warning restore CS8618

