#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
        private void salirMenu_Click(object? sender, EventArgs e) => Close();

        private void deleteButton_Click(object? sender, EventArgs e)
        {
            if (IsTextEditingActive()) return;
            DeleteSelected();
        }

        private bool IsTextEditingActive()
        {
            Control? focused = ActiveControl;
            while (focused is ContainerControl container && container.ActiveControl is not null)
                focused = container.ActiveControl;
            return focused is TextBoxBase;
        }
    
            private void saveIndexButton_Click(object? sender, EventArgs e) => SaveIndex();
    
            private void clearMemoryButton_Click(object? sender, EventArgs e) => ClearMemory();
    
            private void encodeMapsButton_Click(object? sender, EventArgs e) => MessageBox.Show(this, "El módulo de mapas se añadirá en la siguiente fase.", "Encode Mapas");
    
            private void newSearchButton_Click(object? sender, EventArgs e)
        {
            NewOrGoToIndex();
        }

        private void textData_DoubleClick(object? sender, EventArgs e)
        {
            if (_indexerState.Equals("Grh", StringComparison.OrdinalIgnoreCase)) return;
            if (sender is not TextBox textBox || !int.TryParse(textBox.Text.Trim(), out var index)) return;
            if (index <= 0) return;

            CambiarEstado("Grh");
            BuscarNuevoF(index);
        }
    
            private void greenButton_Click(object? sender, EventArgs e)
        {
            _greenBackground = !_greenBackground;
            previewPanel.BackColor = _greenBackground ? Color.Lime : Color.WhiteSmoke;
            previewPanel.Invalidate();
            statusLabel.Text = _greenBackground ? "Fondo verde activado." : "Fondo normal activado.";
        }
    
            private void SelectBackgroundColor()
        {
            using var dialog = new ColorDialog { Color = previewPanel.BackColor, FullOpen = true };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            _greenBackground = false;
            previewPanel.BackColor = dialog.Color;
            previewPanel.Invalidate();
            statusLabel.Text = "Color de fondo actualizado.";
        }
    
            private void kindButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button) return;
            var state = button.Text.Equals("Bodies", StringComparison.OrdinalIgnoreCase) ? "Body" : button.Text;
            if (state.Equals("Capas", StringComparison.OrdinalIgnoreCase))
                ToggleWeMode();
            CambiarEstado(state);
            ComprobarIndexLista();
        }

        private void Checkcabeza_CheckedChanged(object? sender, EventArgs e)
        {
            cabezaActual = Checkcabeza.Checked ? 3008 : 0;
            previewPanel.Invalidate();
        }

        private void UpdateBodyPreviewOffset()
        {
            if (!_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase)) return;
            var parts = xText.Text.Trim().Replace('°', 'º').Split(new[] { 'º', '.' }, StringSplitOptions.TrimEntries);
            if (parts.Length >= 2 && short.TryParse(parts[0], out var y) && short.TryParse(parts[1], out var x))
            {
                _previewHeadOffsetY = y;
                _previewHeadOffsetX = x;
            }
            else
            {
                _previewHeadOffsetX = 0;
                _previewHeadOffsetY = 0;
            }
            previewPanel.Invalidate();
        }
    
            private void ClickDirectionalEnLista()
        {
            if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _directionalRecords.Count) return;
            var item = _directionalRecords[indexList.SelectedIndex];
            _lastIndexByState[_indexerState] = indexList.SelectedIndex;
            directionCombo.SelectedIndex = Math.Clamp(directionCombo.SelectedIndex, 0, 4);
            var direction = Math.Clamp(directionCombo.SelectedIndex - 1, 0, 3);
            grhText.Text = item.Id.ToString();
            // GetInfoDataIndex del VB6 siempre muestra las cuatro direcciones
            // en TextDatos(0), (2), (3) y (4); el selector solo cambia el
            // frame que se previsualiza.
            bmpText.Text = item.Directions[0].ToString();
            framesText.Clear();
            heightText.Text = item.Directions[1].ToString();
            widthText.Text = item.Directions[2].ToString();
            speedText.Text = item.Directions[3].ToString();
            if (_indexerState is "Body" or "Alas")
                xText.Text = $"{item.OffsetY}º{item.OffsetX}";
            else
                xText.Clear();
            yText.Clear();
            tileHeightText.Clear();
            tileWidthText.Clear();
            if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
                UpdateBodyPreviewOffset();
            SetUsageCount(CountReferences(item.Directions[direction]));
            _selectedRecord = _file?.Records.FirstOrDefault(x => x.Id == item.Directions[direction]);
            frameText.Text = "1";
            if (_selectedRecord is not null) UpdateSurfaceSelection(_selectedRecord);
            if (_selectedRecord is not null) ConfigureAnimationTimer(_selectedRecord);
            ValidateDirectionalInput();
            previewPanel.Invalidate();
            statusLabel.Text = $"{StateCaption(_indexerState)} {item.Id} seleccionado.";
        }
    
            private void directionCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _drawWalk = Math.Clamp(directionCombo.SelectedIndex, 0, 4);
            if (_indexerState is "Body" or "Cabezas" or "Cascos" or "Alas" or "Capas" or "Botas")
                ClickDirectionalEnLista();
            previewPanel.Invalidate();
        }
    
            private void ClickFxEnLista()
        {
            if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _fxRecords.Count) return;
            var item = _fxRecords[indexList.SelectedIndex];
            _lastIndexByState[_indexerState] = indexList.SelectedIndex;
            grhText.Text = item.Id.ToString();
            bmpText.Text = item.Animation.ToString();
            framesText.Clear();
            heightText.Text = $"{item.OffsetY}º{item.OffsetX}";
            widthText.Text = item.Animation.ToString();
            speedText.Text = item.Animation.ToString();
            xText.Clear();
            yText.Clear();
            tileHeightText.Clear();
            tileWidthText.Clear();
            SetUsageCount(CountReferences(item.Animation));
            _selectedRecord = _file?.Records.FirstOrDefault(x => x.Id == item.Animation);
            if (_selectedRecord is not null) UpdateSurfaceSelection(_selectedRecord);
            if (_selectedRecord is not null) ConfigureAnimationTimer(_selectedRecord);
            ValidateDirectionalInput();
            previewPanel.Invalidate();
            statusLabel.Text = $"FX {item.Id} seleccionado.";
        }
}

#pragma warning restore CS8618

