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
        private void generarWeMenu_Click(object? sender, EventArgs e) => ShowPending("Generar índices para WE");
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
}

#pragma warning restore CS8618

