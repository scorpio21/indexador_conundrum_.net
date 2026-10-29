namespace EncodeNet8;

public partial class SettingsForm : Form
{
    public IndexerSettings Settings { get; }

    public SettingsForm(IndexerSettings settings)
    {
        Settings = settings;
        InitializeComponent();
        initPathText.Text = settings.InitPath;
        graphicsPathText.Text = settings.GraphicsPath;
        Datosindicetext.Text = settings.IndicesPath;
        browseIndiceButton.Click += browseIndiceButton_Click;
    }

    private void browseInitButton_Click(object? sender, EventArgs e) => SelectFolder(initPathText);
    private void browseGraphicsButton_Click(object? sender, EventArgs e) => SelectFolder(graphicsPathText);
    private void browseIndiceButton_Click(object? sender, EventArgs e) => SelectFolder(Datosindicetext);

    private static void SelectFolder(TextBox target)
    {
        using var dialog = new FolderBrowserDialog { SelectedPath = target.Text };
        if (dialog.ShowDialog() == DialogResult.OK) target.Text = dialog.SelectedPath;
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        Settings.InitPath = initPathText.Text.Trim();
        Settings.GraphicsPath = graphicsPathText.Text.Trim();
        Settings.IndicesPath = string.IsNullOrWhiteSpace(Datosindicetext.Text) ? "Datos" : Datosindicetext.Text.Trim();
        Settings.Save();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void cancelButton_Click(object? sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
}
