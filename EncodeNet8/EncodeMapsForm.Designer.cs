#nullable enable

namespace EncodeNet8;

partial class EncodeMapsForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label mapasLabel = null!, fkLabel = null!, statusLabel = null!;
    private TextBox mapasDatText = null!, fkText = null!;
    private Button browseMapasButton = null!, browseFkButton = null!, generateButton = null!, closeButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        mapasLabel = new Label();
        fkLabel = new Label();
        statusLabel = new Label();
        mapasDatText = new TextBox();
        fkText = new TextBox();
        browseMapasButton = new Button();
        browseFkButton = new Button();
        generateButton = new Button();
        closeButton = new Button();
        SuspendLayout();
        // mapasLabel
        mapasLabel.AutoSize = true;
        mapasLabel.Location = new Point(18, 22);
        mapasLabel.Text = "mapas.dat:";
        // mapasDatText
        mapasDatText.Location = new Point(100, 18);
        mapasDatText.Size = new Size(440, 23);
        // browseMapasButton
        browseMapasButton.Location = new Point(550, 17);
        browseMapasButton.Size = new Size(90, 25);
        browseMapasButton.Text = "Examinar...";
        browseMapasButton.Click += browseMapasButton_Click;
        // fkLabel
        fkLabel.AutoSize = true;
        fkLabel.Location = new Point(18, 61);
        fkLabel.Text = "FK.ind:";
        // fkText
        fkText.Location = new Point(100, 57);
        fkText.Size = new Size(440, 23);
        // browseFkButton
        browseFkButton.Location = new Point(550, 56);
        browseFkButton.Size = new Size(90, 25);
        browseFkButton.Text = "Examinar...";
        browseFkButton.Click += browseFkButton_Click;
        // generateButton
        generateButton.Location = new Point(205, 104);
        generateButton.Size = new Size(145, 32);
        generateButton.Text = "Generar FK.ind";
        generateButton.Click += generateButton_Click;
        // closeButton
        closeButton.Location = new Point(370, 104);
        closeButton.Size = new Size(100, 32);
        closeButton.Text = "Cerrar";
        closeButton.Click += closeButton_Click;
        // statusLabel
        statusLabel.BorderStyle = BorderStyle.FixedSingle;
        statusLabel.Location = new Point(18, 157);
        statusLabel.Size = new Size(622, 50);
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // form
        ClientSize = new Size(660, 225);
        Controls.Add(mapasLabel);
        Controls.Add(mapasDatText);
        Controls.Add(browseMapasButton);
        Controls.Add(fkLabel);
        Controls.Add(fkText);
        Controls.Add(browseFkButton);
        Controls.Add(generateButton);
        Controls.Add(closeButton);
        Controls.Add(statusLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Encode Mapas - Generar FK.ind";
        ResumeLayout(false);
        PerformLayout();
    }
}
