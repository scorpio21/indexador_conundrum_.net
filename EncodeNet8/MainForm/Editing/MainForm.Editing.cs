#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
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
            if (_resourceSearchMode)
            {
                ClickResourceSearchItem();
                return;
            }
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
                foreach (var box in new[] { bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText }) box.Clear();
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
            UpdateSurfaceSelection(frameData);
            SetUsageCount(CountReferences(_selectedRecord.Id));
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
            _file = null; _loadedPath = null; _selectedRecord = null; _resourceSearchMode = false; _drawTimer.Stop(); indexList.Items.Clear(); previewPanel.Invalidate();
            foreach (var box in new[] { grhText, bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText }) box.Clear();
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
    
            private void NewOrGoToIndex()
        {
            var value = Microsoft.VisualBasic.Interaction.InputBox(
                _indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase)
                    ? "Introduzca el número de GRH (0 para encontrar el primer hueco libre)."
                    : $"Introduzca el número de {StateCaption(_indexerState)}.",
                _indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase) ? "Nuevo gráfico / Ir a" : $"Ir a {StateCaption(_indexerState)}", "0");
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!int.TryParse(value, out var requestedId) || requestedId < 0 || requestedId > ParseInt(maxGrhText.Text))
            {
                SetUtilError("Introduzca un número de GRH válido.");
                return;
            }
    
            if (!_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
            {
                if (requestedId <= 0)
                {
                    SetUtilError("Introduzca un número de índice válido.");
                    return;
                }

                BuscarNuevoF(requestedId);
                return;
            }

            if (_file is null)
            {
                SetUtilError("Abre primero un Graficos.ind.");
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

        private void BuscarNuevoF(int index)
        {
            if (index <= 0 || index > ParseInt(maxGrhText.Text))
            {
                SetUtilError("Valor no válido.");
                return;
            }

            if (_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase))
            {
                SelectGrhById(index);
                if (indexList.SelectedIndex >= 0)
                {
                    ClickEnLista();
                    return;
                }

                if (_file is null)
                {
                    SetUtilError("Abre primero un Graficos.ind.");
                    return;
                }

                if (MessageBox.Show(this, $"El GRH {index} no existe. ¿Quieres crearlo?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                _file.Records.Add(new GrhRecord { Id = index, FileNumber = 1, Width = 32, Height = 32, Frames = new List<int> { index } });
                RebuildReferenceCounts();
                RefreshGrid();
                SelectGrhById(index);
                ClickEnLista();
                statusLabel.Text = $"GRH {index} creado en memoria. Completa sus datos y guarda el índice.";
                return;
            }

            if (_indexerState.Equals("Resource", StringComparison.OrdinalIgnoreCase))
            {
                for (var i = 0; i < indexList.Items.Count; i++)
                {
                    if (!string.Equals(indexList.Items[i]?.ToString(), index.ToString(), StringComparison.OrdinalIgnoreCase)) continue;
                    indexList.SelectedIndex = i;
                    ClickEnLista();
                    return;
                }

                SetUtilError("BMP no existe.");
                return;
            }

            if (_indexerState.Equals("Fx", StringComparison.OrdinalIgnoreCase))
            {
                var fx = _fxRecords.FirstOrDefault(item => item.Id == index);
                if (fx is null)
                {
                    if (MessageBox.Show(this, $"El FX {index} no existe. ¿Quieres crearlo?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                    fx = new FxIndexRecord { Id = index, Animation = 0, OffsetX = 0, OffsetY = 0 };
                    _fxRecords.Add(fx);
                    RefreshFxList();
                }

                SelectListItem(index);
                ClickEnLista();
                statusLabel.Text = $"FX {index} seleccionado.";
                return;
            }

            var directional = _directionalRecords.FirstOrDefault(item => item.Id == index);
            if (directional is null)
            {
                var caption = StateCaption(_indexerState);
                if (MessageBox.Show(this, $"El {caption} {index} no existe. ¿Quieres crearlo?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                directional = new DirectionalIndexRecord { Id = index, Directions = new int[4], OffsetX = 0, OffsetY = 0 };
                AddDirectionalPending(_indexerState, directional);
                RebuildReferenceCounts();
                if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase)) RefreshBodyList();
                else RefreshDirectionalList();
            }

            SelectListItem(index);
            ClickEnLista();
            statusLabel.Text = $"{StateCaption(_indexerState)} {index} seleccionado.";
        }

        private void AddDirectionalPending(string state, DirectionalIndexRecord record)
        {
            if (!_pendingDirectionalRecords.TryGetValue(state, out var pending))
                _pendingDirectionalRecords[state] = pending = new List<DirectionalIndexRecord>();
            pending.RemoveAll(item => item.Id == record.Id);
            pending.Add(record);
            _directionalRecords.RemoveAll(item => item.Id == record.Id);
            _directionalRecords.Add(record);

            if (state.Equals("Body", StringComparison.OrdinalIgnoreCase))
            {
                _bodyRecords.RemoveAll(item => item.Id == record.Id);
                _bodyRecords.Add(new BodyIndexRecord
                {
                    Id = record.Id,
                    Walk1 = record.Directions[0],
                    Walk2 = record.Directions[1],
                    Walk3 = record.Directions[2],
                    Walk4 = record.Directions[3],
                    HeadOffsetX = record.OffsetX,
                    HeadOffsetY = record.OffsetY
                });
            }
        }

        private void SelectListItem(int id)
        {
            for (var i = 0; i < indexList.Items.Count; i++)
            {
                if (!string.Equals(indexList.Items[i]?.ToString(), id.ToString(), StringComparison.OrdinalIgnoreCase)) continue;
                indexList.SelectedIndex = i;
                return;
            }
        }
    
            private int FindFirstFreeGrhId()
        {
            var used = _file?.Records.Select(record => record.Id).ToHashSet() ?? new HashSet<int>();
            var max = ParseInt(maxGrhText.Text);
            for (var id = 1; id <= max; id++)
                if (!used.Contains(id)) return id;
            return 0;
        }
}

#pragma warning restore CS8618

