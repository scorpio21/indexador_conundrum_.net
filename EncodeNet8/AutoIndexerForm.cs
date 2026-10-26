namespace EncodeNet8;

public sealed class AutoIndexerOptions
{
    public required string BitmapPath { get; init; }
    public required int StartGrh { get; init; }
    public required int Columns { get; init; }
    public required int Rows { get; init; }
    public required int CellWidth { get; init; }
    public required int CellHeight { get; init; }
    public bool CreateAnimation { get; init; }
    public int AnimationSpeed { get; init; } = 1;
}

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
            BitmapPath = bitmapText.Text, StartGrh = (int)startGrh.Value, Columns = (int)columns.Value,
            Rows = (int)rows.Value, CellWidth = (int)cellWidth.Value, CellHeight = (int)cellHeight.Value,
            CreateAnimation = createAnimation.Checked, AnimationSpeed = (int)speed.Value
        };
    }
}
