namespace EncodeNet8;

[System.ComponentModel.DesignerCategory("Form")]
public partial class AutoIndexerForm : Form
{
    private readonly string _graphicsFolder;
    public AutoIndexerOptions? Options { get; private set; }

    public AutoIndexerForm() : this(string.Empty) { }

    public AutoIndexerForm(string graphicsFolder)
    {
        _graphicsFolder = graphicsFolder;
        InitializeComponent();
    }

    private void BrowseBitmap_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "Bitmap (*.bmp)|*.bmp|Todos los archivos (*.*)|*.*", InitialDirectory = _graphicsFolder };
        if (dialog.ShowDialog(this) == DialogResult.OK) bitmapText.Text = dialog.FileName;
    }

    private void Create_Click(object? sender, EventArgs e)
    {
        if (!File.Exists(bitmapText.Text))
        {
            MessageBox.Show(this, "Seleccione un BMP válido.", "Autoindexador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }
        Options = new AutoIndexerOptions
        {
            BitmapPath = bitmapText.Text, StartGrh = ParseField(startGrh, 1), Columns = ParseField(columns, 1),
            Rows = ParseField(rows, 1), CellWidth = ParseField(cellWidth, 32), CellHeight = ParseField(cellHeight, 32),
            CreateAnimation = createAnimation.Checked, AnimationSpeed = ParseField(speed, 1)
        };
    }

    private static int ParseField(TextBox field, int fallback) => int.TryParse(field.Text, out var value) && value > 0 ? value : fallback;
}
