namespace EncodeNet8;

public partial class EncodeMapsForm : Form
{
    public EncodeMapsForm(string mapasDatPath, string fkPath)
    {
        InitializeComponent();
        mapasDatText.Text = mapasDatPath;
        fkText.Text = fkPath;
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
        if (dialog.ShowDialog(this) == DialogResult.OK) mapasDatText.Text = dialog.FileName;
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
            var count = MapIndexFile.Encode(input, output);
            SetStatus($"FK.ind generado correctamente: {count} mapas.\n{output}", false);
        }
        catch (Exception ex)
        {
            SetStatus($"No se pudo generar FK.ind: {ex.Message}", true);
        }
    }

    private void closeButton_Click(object? sender, EventArgs e) => Close();

    private void SetStatus(string message, bool error)
    {
        statusLabel.Text = message;
        statusLabel.ForeColor = error ? Color.DarkRed : Color.DarkGreen;
    }
}
