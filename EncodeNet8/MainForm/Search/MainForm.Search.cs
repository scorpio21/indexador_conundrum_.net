#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
        private void viewButton_Click(object? sender, EventArgs e)
        {
            if (_file is null) return;
            previewPanel.Invalidate(); statusLabel.Text = $"Mostrando {_file.Records.Count} GRH en memoria.";
        }
    
            private void SearchGrhWithBmp()
        {
            if (_file is null) { SetUtilError("No hay Graficos.ind cargado."); return; }
            _resourceSearchMode = false;
            var value = Microsoft.VisualBasic.Interaction.InputBox("Introduzca número de BMP a buscar.", "Buscar GRH con BMP");
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!int.TryParse(value, out var bmpNumber) || bmpNumber <= 0)
            {
                SetUtilError("Introduzca un número de BMP válido.");
                return;
            }
    
            _searchResults = _file.Records
                .Where(record => record.FileNumber == bmpNumber)
                .Select(record => record.Id)
                .ToList();
            _searchCursor = 0;
            if (_searchResults.Count == 0)
            {
                ReplaceGrhList(Array.Empty<int>());
                SetUtilError($"BMP {bmpNumber} no encontrado en ningún GRH.");
                statusLabel.Text = "Búsqueda sin resultados.";
                return;
            }
            ReplaceGrhList(_searchResults);
            SetUtilError("F3 para continuar la búsqueda.");
            statusLabel.Text = $"BMP {bmpNumber}: {_searchResults.Count} GRH encontrados.";
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
    
            private void ClickResourceSearchItem()
        {
            if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _searchResults.Count) return;
            var fileNumber = _searchResults[indexList.SelectedIndex];
            var bitmapPath = Path.Combine(ResolvePath(_settings.GraphicsPath), $"{fileNumber}.bmp");
            if (!File.Exists(bitmapPath))
            {
                SetUtilError($"BMP {fileNumber} no encontrado: {bitmapPath}");
                return;
            }
    
            try
            {
                if (fileNumber > short.MaxValue)
                {
                    SetUtilError($"BMP {fileNumber} no se puede mostrar como recurso en formato Integer.");
                    return;
                }
                var bitmap = GetCachedBitmap(bitmapPath);
                _selectedRecord = new GrhRecord
                {
                    Id = fileNumber,
                    FileNumber = checked((short)fileNumber),
                    Width = checked((short)Math.Min(bitmap.Width, short.MaxValue)),
                    Height = checked((short)Math.Min(bitmap.Height, short.MaxValue)),
                    Frames = new List<int> { fileNumber }
                };
                grhText.Text = fileNumber.ToString();
                bmpText.Text = fileNumber.ToString();
                frameText.Text = string.Empty;
                framesText.Text = "1";
                heightText.Text = bitmap.Height.ToString();
                widthText.Text = bitmap.Width.ToString();
                speedText.Text = string.Empty;
                xText.Text = "0";
                yText.Text = "0";
                SetUsageCount(0);
                previewPanel.Invalidate();
                statusLabel.Text = $"BMP {fileNumber} seleccionado (recurso sin indexar).";
            }
            catch (Exception ex)
            {
                SetUtilError($"No se pudo mostrar el BMP {fileNumber}: {ex.Message}");
            }
        }
    
            private void SearchUnindexedBmps()
        {
            if (_file is null) { SetUtilError("No hay Graficos.ind cargado."); return; }
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
            _resourceSearchMode = true;
            _searchResults = unindexed;
            _searchCursor = 0;
            indexList.Items.Clear();
            foreach (var fileNumber in unindexed) indexList.Items.Add(fileNumber);
            if (unindexed.Count > 0) indexList.SelectedIndex = 0;
            SetUtilError(unindexed.Count == 0 ? "No hay BMPs sin indexar." : "F3 para continuar la búsqueda.");
            statusLabel.Text = $"BMP sin GRH indexado: {unindexed.Count}.";
        }
    
            private void SearchRepeatedGrh()
        {
            if (_file is null) { ShowPending("Buscar GRH repetidos: no hay Graficos.ind cargado"); return; }
            _resourceSearchMode = false;
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
    
            private void OpenAutoIndexer()
        {
            if (_file is null)
            {
                SetUtilError("Carga primero Graficos.ind para autoindexar.");
                return;
            }
    
            using var dialog = new FormAuto(
                ResolvePath(_settings.GraphicsPath),
                ParseInt(maxGrhText.Text),
                _file.Records.Select(record => record.Id),
                _directionalRecords.Select(record => record.Id).Concat(_bodyRecords.Select(record => record.Id)));
            dialog.PreviewChanged += AutoIndexerPreviewChanged;
            dialog.ResourcePreviewChanged += AutoIndexerResourcePreviewChanged;
            try
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.GeneratedRecords is null) return;
            }
            finally
            {
                dialog.PreviewChanged -= AutoIndexerPreviewChanged;
                dialog.ResourcePreviewChanged -= AutoIndexerResourcePreviewChanged;
                _autoPreviewRecords = null;
                previewPanel.Invalidate();
            }
            try
            {
                var records = dialog.GeneratedRecords.ToList();
                var ids = records.Select(record => record.Id).ToHashSet();
                if (_file.Records.Any(record => ids.Contains(record.Id)) && !dialog.OverwriteExisting)
                    throw new InvalidDataException("Uno o más GRH de destino ya existen.");
                if (dialog.OverwriteExisting)
                    _file.Records.RemoveAll(record => ids.Contains(record.Id));
                _file.Records.AddRange(records);
                if (dialog.GeneratedDirectionalIndex is not null)
                    ApplyGeneratedDirectionalIndex(dialog.GeneratedDirectionalIndex, dialog.GeneratedDirectionalKind);
                RebuildReferenceCounts();
                CambiarEstado("Grh");
                SelectGrhById(records[0].Id);
                statusLabel.Text = $"Autoindexados {records.Count} GRH en memoria. Guarda Graficos.ind para conservarlos.";
                SetUtilError(null);
            }
            catch (Exception ex)
            {
                SetUtilError($"Autoindexador: {ex.Message}");
            }
        }

        private void AutoIndexerPreviewChanged(object? sender, EventArgs e)
        {
            _autoPreviewRecords = (sender as FormAuto)?.PreviewRecords;
            // El Autoindexador es modal. Refresh fuerza el repintado del visor
            // propietario mientras el diálogo sigue abierto, igual que el
            // Timer Dibujado del formulario VB6.
            previewPanel.Refresh();
        }

        private void AutoIndexerResourcePreviewChanged(int fileNumber)
        {
            CambiarEstado("Resource");
            _resourceSearchMode = true;
            _searchResults = new List<int> { fileNumber };
            indexList.Items.Clear();
            indexList.Items.Add(fileNumber);
            indexList.SelectedIndex = 0;
            ClickResourceSearchItem();
        }

        private void ApplyGeneratedDirectionalIndex(DirectionalIndexRecord generated, int generatedKind)
        {
            var state = generatedKind switch
            {
                0 => "Body",
                1 => "Armas",
                2 => "Escudos",
                3 => "Alas",
                4 => "Cascos",
                5 => "Botas",
                _ => "Body"
            };
            var copy = new DirectionalIndexRecord
            {
                Id = generated.Id,
                Directions = generated.Directions.ToArray(),
                OffsetX = generated.OffsetX,
                OffsetY = generated.OffsetY
            };
            if (!_pendingDirectionalRecords.TryGetValue(state, out var pending))
                _pendingDirectionalRecords[state] = pending = new List<DirectionalIndexRecord>();
            pending.RemoveAll(item => item.Id == copy.Id);
            pending.Add(copy);

            if (_indexerState.Equals(state, StringComparison.OrdinalIgnoreCase))
            {
                _directionalRecords.RemoveAll(item => item.Id == copy.Id);
                _directionalRecords.Add(copy);
            }

            if (!state.Equals("Body", StringComparison.OrdinalIgnoreCase)) return;
            _bodyRecords.RemoveAll(item => item.Id == generated.Id);
            _bodyRecords.Add(new BodyIndexRecord
            {
                Id = generated.Id,
                Walk1 = generated.Directions[0],
                Walk2 = generated.Directions[1],
                Walk3 = generated.Directions[2],
                Walk4 = generated.Directions[3],
                HeadOffsetX = generated.OffsetX,
                HeadOffsetY = generated.OffsetY
            });
        }

        private void MergePendingDirectionalState(string state)
        {
            if (!_pendingDirectionalRecords.TryGetValue(state, out var pending)) return;
            foreach (var item in pending)
            {
                _directionalRecords.RemoveAll(existing => existing.Id == item.Id);
                _directionalRecords.Add(new DirectionalIndexRecord
                {
                    Id = item.Id,
                    Directions = item.Directions.ToArray(),
                    OffsetX = item.OffsetX,
                    OffsetY = item.OffsetY
                });
            }
            if (!state.Equals("Body", StringComparison.OrdinalIgnoreCase)) return;
            foreach (var item in pending)
            {
                _bodyRecords.RemoveAll(existing => existing.Id == item.Id);
                _bodyRecords.Add(new BodyIndexRecord
                {
                    Id = item.Id,
                    Walk1 = item.Directions[0],
                    Walk2 = item.Directions[1],
                    Walk3 = item.Directions[2],
                    Walk4 = item.Directions[3],
                    HeadOffsetX = item.OffsetX,
                    HeadOffsetY = item.OffsetY
                });
            }
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
}

#pragma warning restore CS8618

