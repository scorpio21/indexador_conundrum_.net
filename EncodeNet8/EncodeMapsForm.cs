namespace EncodeNet8;

public partial class EncodeMapsForm : Form
{
    public EncodeMapsForm(string mapasDatPath, string fkPath)
    {
        InitializeComponent();
        mapasDatText.Text = mapasDatPath;
        fkText.Text = fkPath;
        LoadMaps();
    }

    private void browseMapasButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Mapas (*.dat)|*.dat|Todos los archivos (*.*)|*.*",
            FileName = Path.GetFileName(mapasDatText.Text),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(mapasDatText.Text))
                ? Path.GetDirectoryName(mapasDatText.Text)!
                : AppContext.BaseDirectory
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            mapasDatText.Text = dialog.FileName;
            LoadMaps();
        }
    }

    private void browseFkButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Índice FK (*.ind)|*.ind|Todos los archivos (*.*)|*.*",
            DefaultExt = "ind",
            AddExtension = true,
            FileName = Path.GetFileName(fkText.Text),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(fkText.Text))
                ? Path.GetDirectoryName(fkText.Text)!
                : AppContext.BaseDirectory
        };
        if (dialog.ShowDialog(this) == DialogResult.OK) fkText.Text = dialog.FileName;
    }

    private void generateButton_Click(object? sender, EventArgs e)
    {
        var input = mapasDatText.Text.Trim();
        var output = fkText.Text.Trim();
        if (string.IsNullOrWhiteSpace(input) || !File.Exists(input))
        {
            SetStatus("No se encontró el archivo mapas.dat.", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(output))
        {
            SetStatus("Indica dónde guardar FK.ind.", true);
            return;
        }

        try
        {
            if (mapsGrid.Rows.Count == 0)
            {
                SetStatus("Carga primero la lista de mapas.", true);
                return;
            }

            var values = mapsGrid.Rows.Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .Select(row => ParseRainMode(row.Cells[2].Value?.ToString()))
                .ToList();
            var count = MapIndexFile.Encode(input, output, values);
            SetStatus($"FK.ind generado correctamente: {count} mapas.\n{output}", false);
        }
        catch (Exception ex)
        {
            SetStatus($"No se pudo generar FK.ind: {ex.Message}", true);
        }
    }

    private void closeButton_Click(object? sender, EventArgs e) => Close();

    private void loadMapsButton_Click(object? sender, EventArgs e) => LoadMaps();

    private void LoadMaps()
    {
        mapsGrid.Rows.Clear();
        var input = mapasDatText.Text.Trim();
        if (string.IsNullOrWhiteSpace(input) || !File.Exists(input))
        {
            SetStatus("No se encontró mapas.dat.", true);
            return;
        }

        try
        {
            foreach (var map in MapIndexFile.Load(input))
            {
                var mode = map.RainMode is 1 or 2 ? map.RainMode : (byte)0;
                mapsGrid.Rows.Add(map.Number, map.Name, mode == 0 ? "0 - Sin configurar" : $"{mode} - {(mode == 1 ? "Se muestra" : "No se muestra")}");
            }
            SetStatus($"Cargados {mapsGrid.Rows.Count} mapas. Selecciona 1 o 2 y genera FK.ind.", false);
        }
        catch (Exception ex)
        {
            SetStatus($"No se pudo cargar mapas.dat: {ex.Message}", true);
        }
    }

    private static byte ParseRainMode(string? value)
    {
        if (value?.StartsWith("1", StringComparison.Ordinal) == true) return 1;
        if (value?.StartsWith("2", StringComparison.Ordinal) == true) return 2;
        return 0;
    }

    private void SetStatus(string message, bool error)
    {
        statusLabel.Text = message;
        statusLabel.ForeColor = error ? Color.DarkRed : Color.DarkGreen;
    }
}
