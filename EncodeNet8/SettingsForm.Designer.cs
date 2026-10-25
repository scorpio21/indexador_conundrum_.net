#nullable enable

namespace EncodeNet8;

partial class SettingsForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label initPathLabel = null!, graphicsPathLabel = null!, titleLabel = null!;
    private TextBox initPathText = null!, graphicsPathText = null!;
    private Button browseInitButton = null!, browseGraphicsButton = null!, saveButton = null!, cancelButton = null!;

    protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container(); titleLabel = new Label(); initPathLabel = new Label(); graphicsPathLabel = new Label(); initPathText = new TextBox(); graphicsPathText = new TextBox(); browseInitButton = new Button(); browseGraphicsButton = new Button(); saveButton = new Button(); cancelButton = new Button(); SuspendLayout();
        titleLabel.AutoSize = true; titleLabel.Location = new Point(16, 16); titleLabel.Size = new Size(280, 15); titleLabel.Text = "Configuración de rutas del indexador";
        initPathLabel.AutoSize = true; initPathLabel.Location = new Point(16, 58); initPathLabel.Text = "Carpeta Init:";
        initPathText.Location = new Point(105, 54); initPathText.Size = new Size(300, 23); initPathText.Name = "initPathText";
        browseInitButton.Location = new Point(415, 53); browseInitButton.Size = new Size(32, 25); browseInitButton.Text = "..."; browseInitButton.Click += browseInitButton_Click;
        graphicsPathLabel.AutoSize = true; graphicsPathLabel.Location = new Point(16, 98); graphicsPathLabel.Text = "Gráficos:";
        graphicsPathText.Location = new Point(105, 94); graphicsPathText.Size = new Size(300, 23); graphicsPathText.Name = "graphicsPathText";
        browseGraphicsButton.Location = new Point(415, 93); browseGraphicsButton.Size = new Size(32, 25); browseGraphicsButton.Text = "..."; browseGraphicsButton.Click += browseGraphicsButton_Click;
        saveButton.Location = new Point(286, 145); saveButton.Size = new Size(78, 30); saveButton.Text = "Guardar"; saveButton.Click += saveButton_Click;
        cancelButton.Location = new Point(369, 145); cancelButton.Size = new Size(78, 30); cancelButton.Text = "Cancelar"; cancelButton.Click += cancelButton_Click;
        ClientSize = new Size(465, 195); Controls.AddRange(new Control[] { titleLabel, initPathLabel, initPathText, browseInitButton, graphicsPathLabel, graphicsPathText, browseGraphicsButton, saveButton, cancelButton }); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; StartPosition = FormStartPosition.CenterParent; Text = "Configuración"; ResumeLayout(false); PerformLayout();
    }
}
