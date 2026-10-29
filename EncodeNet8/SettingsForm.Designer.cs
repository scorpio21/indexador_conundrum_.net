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
        titleLabel = new Label();
        initPathLabel = new Label();
        graphicsPathLabel = new Label();
        initPathText = new TextBox();
        graphicsPathText = new TextBox();
        browseInitButton = new Button();
        browseGraphicsButton = new Button();
        saveButton = new Button();
        cancelButton = new Button();
        Datoslabel = new Label();
        Datosindicetext = new TextBox();
        browseIndiceButton = new Button();
        SuspendLayout();
        // 
        // titleLabel
        // 
        titleLabel.AutoSize = true;
        titleLabel.Location = new Point(16, 16);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(202, 15);
        titleLabel.TabIndex = 0;
        titleLabel.Text = "Configuración de rutas del indexador";
        // 
        // initPathLabel
        // 
        initPathLabel.AutoSize = true;
        initPathLabel.Location = new Point(16, 51);
        initPathLabel.Name = "initPathLabel";
        initPathLabel.Size = new Size(71, 15);
        initPathLabel.TabIndex = 1;
        initPathLabel.Text = "Carpeta Init:";
        // 
        // graphicsPathLabel
        // 
        graphicsPathLabel.AutoSize = true;
        graphicsPathLabel.Location = new Point(16, 80);
        graphicsPathLabel.Name = "graphicsPathLabel";
        graphicsPathLabel.Size = new Size(53, 15);
        graphicsPathLabel.TabIndex = 4;
        graphicsPathLabel.Text = "Gráficos:";
        // 
        // initPathText
        // 
        initPathText.Location = new Point(105, 47);
        initPathText.Name = "initPathText";
        initPathText.Size = new Size(300, 23);
        initPathText.TabIndex = 2;
        // 
        // graphicsPathText
        // 
        graphicsPathText.Location = new Point(105, 76);
        graphicsPathText.Name = "graphicsPathText";
        graphicsPathText.Size = new Size(300, 23);
        graphicsPathText.TabIndex = 5;
        // 
        // browseInitButton
        // 
        browseInitButton.Location = new Point(415, 46);
        browseInitButton.Name = "browseInitButton";
        browseInitButton.Size = new Size(32, 25);
        browseInitButton.TabIndex = 3;
        browseInitButton.Text = "...";
        browseInitButton.Click += browseInitButton_Click;
        // 
        // browseGraphicsButton
        // 
        browseGraphicsButton.Location = new Point(415, 75);
        browseGraphicsButton.Name = "browseGraphicsButton";
        browseGraphicsButton.Size = new Size(32, 25);
        browseGraphicsButton.TabIndex = 6;
        browseGraphicsButton.Text = "...";
        browseGraphicsButton.Click += browseGraphicsButton_Click;
        // 
        // saveButton
        // 
        saveButton.Location = new Point(286, 145);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(78, 30);
        saveButton.TabIndex = 7;
        saveButton.Text = "Guardar";
        saveButton.Click += saveButton_Click;
        // 
        // cancelButton
        // 
        cancelButton.Location = new Point(369, 145);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(78, 30);
        cancelButton.TabIndex = 8;
        cancelButton.Text = "Cancelar";
        cancelButton.Click += cancelButton_Click;
        // 
        // Datoslabel
        // 
        Datoslabel.AutoSize = true;
        Datoslabel.Location = new Point(16, 109);
        Datoslabel.Name = "Datoslabel";
        Datoslabel.Size = new Size(40, 15);
        Datoslabel.TabIndex = 9;
        Datoslabel.Text = "Datos:";
        // 
        // Datosindicetext
        // 
        Datosindicetext.Location = new Point(105, 105);
        Datosindicetext.Name = "Datosindicetext";
        Datosindicetext.Size = new Size(300, 23);
        Datosindicetext.TabIndex = 10;
        // 
        // browseIndiceButton
        // 
        browseIndiceButton.Location = new Point(415, 104);
        browseIndiceButton.Name = "browseIndiceButton";
        browseIndiceButton.Size = new Size(32, 25);
        browseIndiceButton.TabIndex = 11;
        browseIndiceButton.Text = "...";
        // 
        // SettingsForm
        // 
        ClientSize = new Size(465, 195);
        Controls.Add(Datoslabel);
        Controls.Add(Datosindicetext);
        Controls.Add(browseIndiceButton);
        Controls.Add(titleLabel);
        Controls.Add(initPathLabel);
        Controls.Add(initPathText);
        Controls.Add(browseInitButton);
        Controls.Add(graphicsPathLabel);
        Controls.Add(graphicsPathText);
        Controls.Add(browseGraphicsButton);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Configuración";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label Datoslabel = null!;
    private TextBox Datosindicetext = null!;
    private Button browseIndiceButton = null!;
}
