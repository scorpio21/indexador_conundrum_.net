#nullable enable

namespace EncodeNet8;

partial class AutoIndexerForm
{
    private System.ComponentModel.IContainer? components = null;
    private Button normalButton = null!, advancedButton = null!, simpleButton = null!;
    private GroupBox normalFrame = null!, advancedFrame = null!, simpleFrame = null!;
    private TextBox bitmapText = null!, startGrh = null!, columns = null!, rows = null!, cellWidth = null!, cellHeight = null!, speed = null!;
    private TextBox normalFrames = null!, normalFirstIndex = null!, normalHeight = null!, normalWidth = null!, normalBmp = null!, normalColumns = null!, normalRows = null!, normalTotal = null!;
    private TextBox advancedFrames = null!, advancedFirstIndex = null!, advancedHeight = null!, advancedWidth = null!, advancedBmp = null!, advancedColumns = null!, advancedRows = null!, advancedTotal = null!;
    private TextBox simpleBmp = null!, simpleGrh = null!;
    private CheckBox createAnimation = null!;
    private RadioButton sameBitmapOption = null!, consecutiveBitmapOption = null!;
    private Button browseBitmapButton = null!, createButton = null!, normalSearchButton = null!, normalCalculateButton = null!, advancedSearchButton = null!, advancedCalculateButton = null!, simpleSearchButton = null!, simpleIndexButton = null!;
    private ComboBox advancedType = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        normalButton = new Button { Text = "Animacion normal", Location = new Point(32, 8), Size = new Size(105, 23) };
        advancedButton = new Button { Text = "Animacion Avanzada", Location = new Point(152, 8), Size = new Size(185, 23) };
        simpleButton = new Button { Text = "simple", Location = new Point(352, 8), Size = new Size(65, 23) };
        normalButton.Click += normalButton_Click; advancedButton.Click += advancedButton_Click; simpleButton.Click += simpleButton_Click;
        normalFrame = new GroupBox { Text = "Animacion normal", Location = new Point(0, 32), Size = new Size(425, 365) };
        advancedFrame = new GroupBox { Text = "Animacion Especial", Location = new Point(0, 32), Size = new Size(425, 365), Visible = false };
        simpleFrame = new GroupBox { Text = "Indexacion simple", Location = new Point(0, 32), Size = new Size(425, 365), Visible = false };

        normalFrames = new TextBox { Text = "1", Location = new Point(128, 104), Size = new Size(45, 23) };
        normalColumns = new TextBox { Text = "1", Location = new Point(224, 104), Size = new Size(45, 23) };
        normalRows = new TextBox { Text = "1", Location = new Point(288, 104), Size = new Size(45, 23) };
        normalTotal = new TextBox { Text = "1", Location = new Point(352, 104), Size = new Size(45, 23) };
        normalFirstIndex = new TextBox { Location = new Point(128, 136), Size = new Size(75, 23) };
        normalHeight = new TextBox { Location = new Point(128, 168), Size = new Size(75, 23) };
        normalWidth = new TextBox { Location = new Point(128, 200), Size = new Size(75, 23) };
        normalBmp = new TextBox { Location = new Point(128, 232), Size = new Size(75, 23) };
        sameBitmapOption = new RadioButton { Text = "Mismo BMP", Location = new Point(48, 45), AutoSize = true, Checked = true };
        consecutiveBitmapOption = new RadioButton { Text = "BMP consecutivos", Location = new Point(220, 45), AutoSize = true };
        normalSearchButton = new Button { Text = "Buscar", Location = new Point(216, 136), Size = new Size(55, 23) };
        normalCalculateButton = new Button { Text = "AutoCalcular", Location = new Point(224, 200), Size = new Size(80, 23) };
        createButton = new Button { Text = "Crear", Location = new Point(112, 264), Size = new Size(75, 23) };
        normalSearchButton.Click += BrowseBitmap_Click; normalCalculateButton.Click += normalCalculateButton_Click; createButton.Click += Create_Click;
        normalFrame.Controls.Add(sameBitmapOption); normalFrame.Controls.Add(consecutiveBitmapOption); normalFrame.Controls.Add(new Label { Text = "nºframes:", Location = new Point(48, 107), AutoSize = true }); normalFrame.Controls.Add(normalFrames); normalFrame.Controls.Add(new Label { Text = "A lo ancho", Location = new Point(112, 83), AutoSize = true }); normalFrame.Controls.Add(normalColumns); normalFrame.Controls.Add(new Label { Text = "A lo alto", Location = new Point(200, 83), AutoSize = true }); normalFrame.Controls.Add(normalRows); normalFrame.Controls.Add(new Label { Text = "Totales", Location = new Point(256, 83), AutoSize = true }); normalFrame.Controls.Add(normalTotal); normalFrame.Controls.Add(new Label { Text = "Primer indice:", Location = new Point(48, 139), AutoSize = true }); normalFrame.Controls.Add(normalFirstIndex); normalFrame.Controls.Add(normalSearchButton); normalFrame.Controls.Add(new Label { Text = "Alto:", Location = new Point(48, 171), AutoSize = true }); normalFrame.Controls.Add(normalHeight); normalFrame.Controls.Add(new Label { Text = "Ancho:", Location = new Point(48, 203), AutoSize = true }); normalFrame.Controls.Add(normalWidth); normalFrame.Controls.Add(normalCalculateButton); normalFrame.Controls.Add(new Label { Text = "BMP:", Location = new Point(48, 235), AutoSize = true }); normalFrame.Controls.Add(normalBmp); normalFrame.Controls.Add(createButton);

        advancedType = new ComboBox { Location = new Point(56, 32), Size = new Size(155, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        advancedType.Items.AddRange(new object[] { "Animacion normal", "Animacion por grilla" }); advancedType.SelectedIndex = 0;
        advancedFrames = new TextBox { Text = "6", Location = new Point(128, 104), Size = new Size(45, 23) }; advancedColumns = new TextBox { Text = "6", Location = new Point(224, 104), Size = new Size(45, 23) }; advancedRows = new TextBox { Text = "4", Location = new Point(288, 104), Size = new Size(45, 23) }; advancedTotal = new TextBox { Text = "22", Location = new Point(352, 104), Size = new Size(45, 23) };
        advancedFirstIndex = new TextBox { Location = new Point(128, 136), Size = new Size(75, 23) }; advancedHeight = new TextBox { Location = new Point(128, 168), Size = new Size(75, 23) }; advancedWidth = new TextBox { Location = new Point(128, 200), Size = new Size(75, 23) }; advancedBmp = new TextBox { Location = new Point(128, 232), Size = new Size(75, 23) };
        advancedSearchButton = new Button { Text = "Buscar", Location = new Point(216, 136), Size = new Size(55, 23) }; advancedCalculateButton = new Button { Text = "AutoCalcular", Location = new Point(224, 200), Size = new Size(80, 23) }; advancedSearchButton.Click += BrowseBitmap_Click; advancedCalculateButton.Click += normalCalculateButton_Click;
        var advancedCreate = new Button { Text = "Crear", Location = new Point(112, 296), Size = new Size(75, 23) }; advancedCreate.Click += Create_Click;
        advancedFrame.Controls.Add(advancedType); advancedFrame.Controls.Add(new Label { Text = "nºframes:", Location = new Point(48, 107), AutoSize = true }); advancedFrame.Controls.Add(advancedFrames); advancedFrame.Controls.Add(new Label { Text = "A lo ancho", Location = new Point(112, 83), AutoSize = true }); advancedFrame.Controls.Add(advancedColumns); advancedFrame.Controls.Add(new Label { Text = "A lo alto", Location = new Point(200, 83), AutoSize = true }); advancedFrame.Controls.Add(advancedRows); advancedFrame.Controls.Add(new Label { Text = "Totales", Location = new Point(320, 83), AutoSize = true }); advancedFrame.Controls.Add(advancedTotal); advancedFrame.Controls.Add(new Label { Text = "Primer indice:", Location = new Point(48, 139), AutoSize = true }); advancedFrame.Controls.Add(advancedFirstIndex); advancedFrame.Controls.Add(advancedSearchButton); advancedFrame.Controls.Add(new Label { Text = "Alto:", Location = new Point(48, 171), AutoSize = true }); advancedFrame.Controls.Add(advancedHeight); advancedFrame.Controls.Add(new Label { Text = "Ancho:", Location = new Point(48, 203), AutoSize = true }); advancedFrame.Controls.Add(advancedWidth); advancedFrame.Controls.Add(advancedCalculateButton); advancedFrame.Controls.Add(new Label { Text = "BMP:", Location = new Point(48, 235), AutoSize = true }); advancedFrame.Controls.Add(advancedBmp); advancedFrame.Controls.Add(advancedCreate); advancedFrame.Controls.Add(new RadioButton { Text = "(pj) alkon", Location = new Point(304, 185), AutoSize = true }); advancedFrame.Controls.Add(new RadioButton { Text = "(pj) clasico", Location = new Point(304, 208), AutoSize = true }); advancedFrame.Controls.Add(new RadioButton { Text = "Normal(NPC)", Location = new Point(304, 231), AutoSize = true }); advancedFrame.Controls.Add(new CheckBox { Text = "Autodindexar", Location = new Point(24, 256), AutoSize = true });

        simpleBmp = new TextBox { Location = new Point(120, 40), Size = new Size(115, 23) }; simpleGrh = new TextBox { Location = new Point(120, 80), Size = new Size(115, 23) }; simpleSearchButton = new Button { Text = "buscar", Location = new Point(264, 40), Size = new Size(55, 23) }; simpleIndexButton = new Button { Text = "indexar", Location = new Point(144, 128), Size = new Size(95, 23) }; simpleSearchButton.Click += BrowseBitmap_Click; simpleIndexButton.Click += Create_Click;
        simpleFrame.Controls.Add(new Label { Text = "Bmp:", Location = new Point(56, 43), AutoSize = true }); simpleFrame.Controls.Add(simpleBmp); simpleFrame.Controls.Add(new Label { Text = "Numero Grh:", Location = new Point(40, 83), AutoSize = true }); simpleFrame.Controls.Add(simpleGrh); simpleFrame.Controls.Add(simpleSearchButton); simpleFrame.Controls.Add(simpleIndexButton);

        bitmapText = normalBmp; startGrh = normalFirstIndex; columns = normalColumns; rows = normalRows; cellWidth = normalWidth; cellHeight = normalHeight; speed = normalFrames; createAnimation = new CheckBox { Text = "Crear animación", Visible = false }; browseBitmapButton = normalSearchButton;
        Controls.Add(normalButton); Controls.Add(advancedButton); Controls.Add(simpleButton); Controls.Add(normalFrame); Controls.Add(advancedFrame); Controls.Add(simpleFrame);
        ClientSize = new Size(425, 405); StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Text = "Indexador automatico";
    }
}
