#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
        private void salirMenu_Click(object? sender, EventArgs e) => Close();

        private void deleteButton_Click(object? sender, EventArgs e) => DeleteSelected();
    
            private void saveIndexButton_Click(object? sender, EventArgs e) => SaveIndex();
    
            private void clearMemoryButton_Click(object? sender, EventArgs e) => ClearMemory();
    
            private void encodeMapsButton_Click(object? sender, EventArgs e) => MessageBox.Show(this, "El módulo de mapas se añadirá en la siguiente fase.", "Encode Mapas");
    
            private void newSearchButton_Click(object? sender, EventArgs e)
        {
            NewOrGoToIndex();
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
            CambiarEstado(state);
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
            SetUsageCount(CountReferences(item.Directions[direction]));
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
            SetUsageCount(CountReferences(item.Animation));
            _selectedRecord = _file?.Records.FirstOrDefault(x => x.Id == item.Animation);
            if (_selectedRecord is not null) ConfigureAnimationTimer(_selectedRecord);
            previewPanel.Invalidate();
            statusLabel.Text = $"FX {item.Id} seleccionado.";
        }
}

#pragma warning restore CS8618

