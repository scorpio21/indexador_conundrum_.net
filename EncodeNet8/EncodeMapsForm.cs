using System.ComponentModel;

namespace EncodeNet8;

public partial class EncodeMapsForm : Form
{
    private readonly Dictionary<string, EncodeIndexType> _types = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Alas.dat"] = EncodeIndexType.Alas,
        ["Body.dat"] = EncodeIndexType.Body,
        ["Botas.dat"] = EncodeIndexType.Botas,
        ["Cascos.dat"] = EncodeIndexType.Cascos,
        ["escudos.dat"] = EncodeIndexType.Escudos,
        ["Fx.dat"] = EncodeIndexType.Fx,
        ["Head.dat"] = EncodeIndexType.Head,
        ["Mapas.dat"] = EncodeIndexType.Mapas
    };

    // Constructor utilizado por el diseñador de WinForms.
    public EncodeMapsForm()
    {
        InitializeComponent();
    }

    // Constructor utilizado por MainForm cuando se abre la ventana en ejecución.
    public EncodeMapsForm(string mapasDatPath, string fkPath) : this()
    {
        if (IsInDesignMode()) return;

        //BackColor = Color.FromArgb(192, 255, 192);
        sourcePathText.Text = mapasDatPath;
        outputPathText.Text = fkPath;
        encodeTypeCombo.SelectedItem = "Mapas.dat";
        UpdateTypeUi();
        LoadSelectedFile();
    }

    private static bool IsInDesignMode() =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    private EncodeIndexType SelectedType =>
        _types.TryGetValue(encodeTypeCombo.SelectedItem?.ToString() ?? string.Empty, out var type)
            ? type : EncodeIndexType.Mapas;

    private void encodeTypeCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var selected = encodeTypeCombo.SelectedItem?.ToString() ?? "Mapas.dat";
        var sourceDirectory = Path.GetDirectoryName(sourcePathText.Text);
        if (!string.IsNullOrWhiteSpace(sourceDirectory))
            sourcePathText.Text = Path.Combine(sourceDirectory, selected);
        outputPathText.Text = Path.Combine(Path.GetDirectoryName(outputPathText.Text) ?? string.Empty, OutputName(SelectedType));
        UpdateTypeUi();
        LoadSelectedFile();
    }

    private void UpdateTypeUi()
    {
        var isMapas = SelectedType == EncodeIndexType.Mapas;
        legendLabel.Visible = isMapas;
        saveDataButton.Text = $"Generar índice {DisplayName(SelectedType)}";
    }

    private void browseSourceButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Archivos DAT (*.dat)|*.dat|Todos los archivos (*.*)|*.*",
            FileName = Path.GetFileName(sourcePathText.Text),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(sourcePathText.Text))
                ? Path.GetDirectoryName(sourcePathText.Text)! : AppContext.BaseDirectory
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            sourcePathText.Text = dialog.FileName;
            LoadSelectedFile();
        }
    }

    private void browseOutputButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Índices (*.ind)|*.ind|Todos los archivos (*.*)|*.*",
            DefaultExt = "ind",
            AddExtension = true,
            FileName = Path.GetFileName(outputPathText.Text),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(outputPathText.Text))
                ? Path.GetDirectoryName(outputPathText.Text)! : AppContext.BaseDirectory
        };
        if (dialog.ShowDialog(this) == DialogResult.OK) outputPathText.Text = dialog.FileName;
    }

    private void loadDataButton_Click(object? sender, EventArgs e) => LoadSelectedFile();

    private void saveDataButton_Click(object? sender, EventArgs e)
    {
        if (encodeGrid.Rows.Count == 0)
        {
            SetStatus("Carga primero los datos del archivo seleccionado.", true);
            return;
        }
        try
        {
            DatIndexEncoder.Save(sourcePathText.Text.Trim(), outputPathText.Text.Trim(), SelectedType, ReadRowsFromGrid());
            SetStatus($"Índice generado correctamente: {outputPathText.Text}", false);
        }
        catch (Exception ex)
        {
            SetStatus($"No se pudo generar el índice: {ex.Message}", true);
        }
    }

    private void closeButton_Click(object? sender, EventArgs e) => Close();

    private void LoadSelectedFile()
    {
        encodeGrid.Rows.Clear();
        if (!File.Exists(sourcePathText.Text.Trim()))
        {
            SetStatus($"No se encontró: {sourcePathText.Text}", true);
            return;
        }
        try
        {
            ConfigureColumns();
            foreach (var row in DatIndexEncoder.Load(sourcePathText.Text.Trim(), SelectedType))
            {
                var values = row.Values;
                if (SelectedType == EncodeIndexType.Mapas)
                {
                    var rain = Get(values, "Lluvia");
                    encodeGrid.Rows.Add(row.Number, row.Name, rain switch
                    {
                        1 => "1 - Se muestra",
                        2 => "2 - No se muestra",
                        _ => "0 - Sin configurar"
                    });
                }
                else
                {
                    var fields = FieldsFor(SelectedType);
                    var cells = new List<object> { row.Number, row.Name };
                    cells.AddRange(fields.Select(field => (object)Get(values, field)));
                    encodeGrid.Rows.Add(cells.ToArray());
                }
            }
            SetStatus($"Cargados {encodeGrid.Rows.Count} registros de {Path.GetFileName(sourcePathText.Text)}.", false);
        }
        catch (Exception ex)
        {
            SetStatus($"No se pudo cargar el archivo: {ex.Message}", true);
        }
    }

    private void ConfigureColumns()
    {
        encodeGrid.Columns.Clear();
        encodeGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Índice", ReadOnly = true, Width = 65 });
        encodeGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sección", ReadOnly = true, Width = 145 });
        if (SelectedType == EncodeIndexType.Mapas)
        {
            var column = new DataGridViewComboBoxColumn { HeaderText = "Lluvia", Width = 190 };
            column.Items.AddRange("1 - Se muestra", "2 - No se muestra", "0 - Sin configurar");
            encodeGrid.Columns.Add(column);
            return;
        }
        foreach (var field in FieldsFor(SelectedType))
            encodeGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = field, Width = 95 });
    }

    private List<EncodeIndexRow> ReadRowsFromGrid()
    {
        var fields = FieldsFor(SelectedType);
        var rows = new List<EncodeIndexRow>();
        foreach (DataGridViewRow gridRow in encodeGrid.Rows)
        {
            if (gridRow.IsNewRow) continue;
            var values = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (SelectedType == EncodeIndexType.Mapas)
                values["Lluvia"] = ParseRain(gridRow.Cells[2].Value?.ToString());
            else
                for (var i = 0; i < fields.Count; i++)
                    if (!int.TryParse(gridRow.Cells[i + 2].Value?.ToString(), out var value))
                        throw new InvalidDataException($"Valor no válido en {fields[i]}, índice {gridRow.Cells[0].Value}.");
                    else values[fields[i]] = value;
            rows.Add(new EncodeIndexRow(Convert.ToInt32(gridRow.Cells[0].Value), gridRow.Cells[1].Value?.ToString() ?? string.Empty, values));
        }
        return rows;
    }

    private static List<string> FieldsFor(EncodeIndexType type) => type switch
    {
        EncodeIndexType.Alas => new() { "Alas1", "Alas2", "Alas3", "Alas4", "AlasOffsetX", "AlasOffsetY" },
        EncodeIndexType.Body => new() { "Walk1", "Walk2", "Walk3", "Walk4", "HeadOffsetX", "HeadOffsetY" },
        EncodeIndexType.Botas => new() { "Bota1", "Bota2", "Bota3", "Bota4", "BotaOffsetX", "BotaOffsetY" },
        EncodeIndexType.Cascos => new() { "Head1", "Head2", "Head3", "Head4" },
        EncodeIndexType.Escudos => new() { "Dir1", "Dir2", "Dir3", "Dir4" },
        EncodeIndexType.Fx => new() { "Animacion", "OffsetX", "OffsetY" },
        EncodeIndexType.Head => new() { "Head0", "Head1", "Head2", "Head3" },
        _ => new()
    };

    private static string OutputName(EncodeIndexType type) => type switch
    {
        EncodeIndexType.Alas => "Alas.ind",
        EncodeIndexType.Body => "Personajes.ind",
        EncodeIndexType.Botas => "Botas.ind",
        EncodeIndexType.Cascos => "Cascos.ind",
        EncodeIndexType.Escudos => "Escudos.ind",
        EncodeIndexType.Fx => "Fxs.ind",
        EncodeIndexType.Head => "Cabezas.ind",
        _ => "FK.ind"
    };

    private static string DisplayName(EncodeIndexType type) => type switch
    {
        EncodeIndexType.Alas => "alas",
        EncodeIndexType.Body => "body",
        EncodeIndexType.Botas => "botas",
        EncodeIndexType.Cascos => "cascos",
        EncodeIndexType.Escudos => "escudos",
        EncodeIndexType.Fx => "fx",
        EncodeIndexType.Head => "head",
        _ => "mapas"
    };

    private static int Get(IReadOnlyDictionary<string, int> values, string key) =>
        values.TryGetValue(key, out var value) ? value : 0;

    private static int ParseRain(string? value) =>
        value?.StartsWith("1", StringComparison.Ordinal) == true ? 1 :
        value?.StartsWith("2", StringComparison.Ordinal) == true ? 2 : 0;

    private void SetStatus(string message, bool error)
    {
        encodeStatusLabel.Text = message;
        encodeStatusLabel.ForeColor = error ? Color.DarkRed : Color.DarkGreen;
    }
}
