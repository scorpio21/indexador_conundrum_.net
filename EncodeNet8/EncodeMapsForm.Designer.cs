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
        ((System.ComponentModel.ISupportInitialize)encodeGrid).BeginInit();
        SuspendLayout();
        // 
        // sourceLabel
        // 
        sourceLabel.AutoSize = true;
        sourceLabel.Location = new Point(18, 54);
        sourceLabel.Name = "sourceLabel";
        sourceLabel.Size = new Size(88, 15);
        sourceLabel.TabIndex = 2;
        sourceLabel.Text = "Archivo origen:";
        // 
        // outputLabel
        // 
        outputLabel.AutoSize = true;
        outputLabel.Location = new Point(18, 87);
        outputLabel.Name = "outputLabel";
        outputLabel.Size = new Size(68, 15);
        outputLabel.TabIndex = 5;
        outputLabel.Text = "Guardar en:";
        // 
        // encodeStatusLabel
        // 
        encodeStatusLabel.BorderStyle = BorderStyle.FixedSingle;
        encodeStatusLabel.Location = new Point(18, 480);
        encodeStatusLabel.Name = "encodeStatusLabel";
        encodeStatusLabel.Size = new Size(622, 50);
        encodeStatusLabel.TabIndex = 13;
        encodeStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // legendLabel
        // 
        legendLabel.AutoSize = true;
        legendLabel.Location = new Point(155, 126);
        legendLabel.Name = "legendLabel";
        legendLabel.Size = new Size(254, 15);
        legendLabel.TabIndex = 9;
        legendLabel.Text = "Para Mapas: 1 = se muestra   2 = no se muestra";
        // 
        // typeLabel
        // 
        typeLabel.AutoSize = true;
        typeLabel.Location = new Point(18, 18);
        typeLabel.Name = "typeLabel";
        typeLabel.Size = new Size(58, 15);
        typeLabel.TabIndex = 0;
        typeLabel.Text = "Codificar:";
        // 
        // sourcePathText
        // 
        sourcePathText.Location = new Point(112, 51);
        sourcePathText.Name = "sourcePathText";
        sourcePathText.Size = new Size(440, 23);
        sourcePathText.TabIndex = 3;
        sourcePathText.Text = "Archivo .dat de origen";
        // 
        // outputPathText
        // 
        outputPathText.Location = new Point(112, 84);
        outputPathText.Name = "outputPathText";
        outputPathText.Size = new Size(440, 23);
        outputPathText.TabIndex = 6;
        outputPathText.Text = "Archivo .ind de salida";
        // 
        // encodeTypeCombo
        // 
        encodeTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        encodeTypeCombo.Items.AddRange(new object[] { "Alas.dat", "Body.dat", "Botas.dat", "Cascos.dat", "escudos.dat", "Fx.dat", "Head.dat", "Mapas.dat" });
        encodeTypeCombo.Location = new Point(100, 14);
        encodeTypeCombo.Name = "encodeTypeCombo";
        encodeTypeCombo.Size = new Size(220, 23);
        encodeTypeCombo.TabIndex = 1;
        encodeTypeCombo.SelectedIndexChanged += encodeTypeCombo_SelectedIndexChanged;
        // 
        // browseSourceButton
        // 
        browseSourceButton.Location = new Point(562, 50);
        browseSourceButton.Name = "browseSourceButton";
        browseSourceButton.Size = new Size(90, 25);
        browseSourceButton.TabIndex = 4;
        browseSourceButton.Text = "Examinar...";
        browseSourceButton.Click += browseSourceButton_Click;
        // 
        // browseOutputButton
        // 
        browseOutputButton.Location = new Point(562, 83);
        browseOutputButton.Name = "browseOutputButton";
        browseOutputButton.Size = new Size(90, 25);
        browseOutputButton.TabIndex = 7;
        browseOutputButton.Text = "Examinar...";
        browseOutputButton.Click += browseOutputButton_Click;
        // 
        // loadDataButton
        // 
        loadDataButton.Location = new Point(18, 119);
        loadDataButton.Name = "loadDataButton";
        loadDataButton.Size = new Size(120, 28);
        loadDataButton.TabIndex = 8;
        loadDataButton.Text = "Cargar datos";
        loadDataButton.Click += loadDataButton_Click;
        // 
        // saveDataButton
        // 
        saveDataButton.Location = new Point(205, 440);
        saveDataButton.Name = "saveDataButton";
        saveDataButton.Size = new Size(145, 32);
        saveDataButton.TabIndex = 11;
        saveDataButton.Text = "Generar índice";
        saveDataButton.Click += saveDataButton_Click;
        // 
        // closeButton
        // 
        closeButton.Location = new Point(370, 440);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(100, 32);
        closeButton.TabIndex = 12;
        closeButton.Text = "Cerrar";
        closeButton.Click += closeButton_Click;
        // 
        // encodeGrid
        // 
        encodeGrid.AllowUserToAddRows = false;
        encodeGrid.AllowUserToDeleteRows = false;
        encodeGrid.Location = new Point(18, 155);
        encodeGrid.Name = "encodeGrid";
        encodeGrid.RowHeadersVisible = false;
        encodeGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        encodeGrid.Size = new Size(622, 275);
        encodeGrid.TabIndex = 10;
        // 
        // EncodeMapsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(192, 255, 192);
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
        Name = "EncodeMapsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Encode - Generar índices";
        ((System.ComponentModel.ISupportInitialize)encodeGrid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
