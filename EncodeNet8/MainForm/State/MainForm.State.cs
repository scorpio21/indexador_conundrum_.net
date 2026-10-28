#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
        private void graphicsButton_Click(object? sender, EventArgs e)
        {
            CambiarEstado("Grh");
            ComprobarIndexLista();
            if (indexList.SelectedIndex >= 0) ClickEnLista();
        }
    
            private void CambiarEstado(string state)
        {
            _indexerState = NormalizeState(state);
            _resourceSearchMode = false;
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

            MergePendingDirectionalState(_indexerState);
            if (_indexerState == "Body" || IsDirectionalState(_indexerState) || _indexerState is "Armas" or "Escudos")
                RefreshDirectionalList();
    
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
            var textBoxes = new[] { bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText };
            var labels = new[] { bmpLabel, framesLabel, heightLabel, widthLabel, speedLabel, xLabel, yLabel, tileHeightLabel, tileWidthLabel };
            foreach (var control in textBoxes) { control.Visible = true; control.Enabled = true; control.BackColor = Color.White; }
            foreach (var control in labels) control.Visible = true;
            frameText.Visible = true;
            frameLabel.Visible = true;
            directionCombo.Visible = false;
            DescripcionAyuda.Visible = false;
            saveIndexButton.Visible = true;
            deleteButton.Visible = true;
            herramientasMenu.Visible = state.Equals("Grh", StringComparison.OrdinalIgnoreCase);
            clonarMenu.Visible = state.Equals("Grh", StringComparison.OrdinalIgnoreCase);
            buscarSiguienteMenu.Visible = false;
    
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
                return;
            }

            if (state.Equals("Resource", StringComparison.OrdinalIgnoreCase))
            {
                ShowStateFields(textBoxes, labels, 0, 2, 3, 4, 5);
                frameText.Visible = false;
                frameLabel.Visible = false;
                directionCombo.Visible = false;
                grhLabel.Text = "Crypt:";
                bmpLabel.Text = "Tamaño:";
                framesLabel.Text = string.Empty;
                heightLabel.Text = "Alto:";
                widthLabel.Text = "Ancho:";
                speedLabel.Text = "Bits:";
                xLabel.Text = "Situacion:";
                yLabel.Text = string.Empty;
                tileHeightLabel.Text = string.Empty;
                tileWidthLabel.Text = string.Empty;
                saveIndexButton.Visible = false;
                deleteButton.Visible = false;
                DescripcionAyuda.Visible = true;
                DescripcionAyuda.Text = "N: Si están disponible el BMP y el archivo de recursos, se usa el BMP.";
                return;
            }
    
            if (state == "Fx")
            {
                grhLabel.Text = "Fx:";
                bmpLabel.Text = "NumGrh:";
                framesLabel.Text = string.Empty;
                heightLabel.Text = "Offset:";
                frameText.Visible = false;
                frameLabel.Visible = false;
                ShowStateFields(textBoxes, labels, 0, 2);
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
            ShowStateFields(textBoxes, labels, state == "Body" || state == "Alas" ? new[] { 0, 2, 3, 4, 5 } : new[] { 0, 2, 3, 4 });
        }
    
            private static void ShowStateFields(TextBox[] textBoxes, Label[] labels, params int[] visibleIndexes)
        {
            var visible = visibleIndexes.ToHashSet();
            for (var i = 0; i < textBoxes.Length; i++)
            {
                var show = visible.Contains(i);
                textBoxes[i].Visible = show;
                textBoxes[i].Enabled = show;
                labels[i].Visible = show;
                if (!show) textBoxes[i].BackColor = Color.White;
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
}

#pragma warning restore CS8618

