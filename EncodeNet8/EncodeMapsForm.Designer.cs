#nullable enable

namespace EncodeNet8;

partial class EncodeMapsForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label sourceLabel = null!, outputLabel = null!, encodeStatusLabel = null!, legendLabel = null!, typeLabel = null!;
    private TextBox sourcePathText = null!, outputPathText = null!;
    private ComboBox encodeTypeCombo = null!;
    private Button browseSourceButton = null!, browseOutputButton = null!, loadDataButton = null!, saveDataButton = null!, closeButton = null!;
    private DataGridView encodeGrid = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        sourceLabel = new Label();
        outputLabel = new Label();
        encodeStatusLabel = new Label();
        legendLabel = new Label();
        typeLabel = new Label();
        sourcePathText = new TextBox();
        outputPathText = new TextBox();
        encodeTypeCombo = new ComboBox();
        browseSourceButton = new Button();
        browseOutputButton = new Button();
        loadDataButton = new Button();
        saveDataButton = new Button();
        closeButton = new Button();
        encodeGrid = new DataGridView();
        SuspendLayout();
        // typeLabel
        typeLabel.AutoSize = true;
        typeLabel.Location = new Point(18, 18);
        typeLabel.Text = "Codificar:";
        // encodeTypeCombo
        encodeTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        encodeTypeCombo.Location = new Point(100, 14);
        encodeTypeCombo.Size = new Size(220, 23);
        // sourceLabel
        sourceLabel.AutoSize = true;
        sourceLabel.Location = new Point(18, 54);
        sourceLabel.Text = "Archivo origen:";
        // sourcePathText
        sourcePathText.Location = new Point(100, 50);
        sourcePathText.Size = new Size(440, 23);
        // browseSourceButton
        browseSourceButton.Location = new Point(550, 49);
        browseSourceButton.Size = new Size(90, 25);
        browseSourceButton.Text = "Examinar...";
        browseSourceButton.Click += browseSourceButton_Click;
        // outputLabel
        outputLabel.AutoSize = true;
        outputLabel.Location = new Point(18, 87);
        outputLabel.Text = "Guardar en:";
        // outputPathText
        outputPathText.Location = new Point(100, 83);
        outputPathText.Size = new Size(440, 23);
        // browseOutputButton
        browseOutputButton.Location = new Point(550, 82);
        browseOutputButton.Size = new Size(90, 25);
        browseOutputButton.Text = "Examinar...";
        browseOutputButton.Click += browseOutputButton_Click;
        // loadDataButton
        loadDataButton.Location = new Point(18, 119);
        loadDataButton.Size = new Size(120, 28);
        loadDataButton.Text = "Cargar datos";
        loadDataButton.Click += loadDataButton_Click;
        // legendLabel
        legendLabel.AutoSize = true;
        legendLabel.Location = new Point(155, 126);
        legendLabel.Text = "Para Mapas: 1 = se muestra   2 = no se muestra";
        // encodeGrid
        encodeGrid.AllowUserToAddRows = false;
        encodeGrid.AllowUserToDeleteRows = false;
        encodeGrid.AutoGenerateColumns = true;
        encodeGrid.Location = new Point(18, 155);
        encodeGrid.ReadOnly = false;
        encodeGrid.RowHeadersVisible = false;
        encodeGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        encodeGrid.Size = new Size(622, 275);
        // saveDataButton
        saveDataButton.Location = new Point(205, 440);
        saveDataButton.Size = new Size(145, 32);
        saveDataButton.Text = "Generar índice";
        saveDataButton.Click += saveDataButton_Click;
        // closeButton
        closeButton.Location = new Point(370, 440);
        closeButton.Size = new Size(100, 32);
        closeButton.Text = "Cerrar";
        closeButton.Click += closeButton_Click;
        // encodeStatusLabel
        encodeStatusLabel.BorderStyle = BorderStyle.FixedSingle;
        encodeStatusLabel.Location = new Point(18, 480);
        encodeStatusLabel.Size = new Size(622, 50);
        encodeStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // form
        ClientSize = new Size(660, 550);
        Controls.Add(typeLabel);
        Controls.Add(encodeTypeCombo);
        Controls.Add(sourceLabel);
        Controls.Add(sourcePathText);
        Controls.Add(browseSourceButton);
        Controls.Add(outputLabel);
        Controls.Add(outputPathText);
        Controls.Add(browseOutputButton);
        Controls.Add(loadDataButton);
        Controls.Add(legendLabel);
        Controls.Add(encodeGrid);
        Controls.Add(saveDataButton);
        Controls.Add(closeButton);
        Controls.Add(encodeStatusLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Encode - Generar índices";
        ResumeLayout(false);
        PerformLayout();
    }
}
