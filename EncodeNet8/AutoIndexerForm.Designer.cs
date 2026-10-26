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
        normalButton = Button("Animacion normal", 32, 8, 105, 23, (_, _) => ShowMode(normalFrame));
        advancedButton = Button("Animacion Avanzada", 152, 8, 185, 23, (_, _) => ShowMode(advancedFrame));
        simpleButton = Button("simple", 352, 8, 65, 23, (_, _) => ShowMode(simpleFrame));
        normalFrame = new GroupBox { Text = "Animacion normal", Location = new Point(0, 32), Size = new Size(425, 365) };
        advancedFrame = new GroupBox { Text = "Animacion Especial", Location = new Point(0, 32), Size = new Size(425, 365), Visible = false };
        simpleFrame = new GroupBox { Text = "Indexacion simple", Location = new Point(0, 32), Size = new Size(425, 365), Visible = false };

        normalFrames = TextBoxControl("1", 128, 104, 45); normalColumns = TextBoxControl("1", 128, 104, 45); normalRows = TextBoxControl("1", 128, 104, 45); normalTotal = TextBoxControl("1", 256, 104, 45);
        normalFirstIndex = TextBoxControl("", 128, 136, 75); normalHeight = TextBoxControl("", 128, 168, 75); normalWidth = TextBoxControl("", 128, 200, 75); normalBmp = TextBoxControl("", 128, 232, 75);
        sameBitmapOption = new RadioButton { Text = "Mismo BMP", Location = new Point(48, 45), AutoSize = true, Checked = true };
        consecutiveBitmapOption = new RadioButton { Text = "BMP consecutivos", Location = new Point(220, 45), AutoSize = true };
        normalSearchButton = Button("Buscar", 216, 136, 55, 23, BrowseBitmap_Click); normalCalculateButton = Button("AutoCalcular", 224, 200, 80, 23, (_, _) => CalculateNormal());
        var normalCreate = Button("Crear", 112, 264, 75, 23, Create_Click);
        Add(normalFrame, sameBitmapOption, consecutiveBitmapOption, Label("nºframes:", 48, 104), normalFrames, Label("A lo ancho", 112, 80), normalColumns, Label("A lo alto", 200, 80), normalRows, Label("Totales", 256, 80), normalTotal, Label("Primer indice:", 48, 136), normalFirstIndex, normalSearchButton, Label("Alto:", 48, 168), normalHeight, Label("Ancho:", 48, 200), normalWidth, normalCalculateButton, Label("BMP:", 48, 232), normalBmp, normalCreate);

        advancedType = new ComboBox { Location = new Point(56, 32), Size = new Size(155, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        advancedType.Items.AddRange(new object[] { "Animacion normal", "Animacion por grilla" }); advancedType.SelectedIndex = 0;
        advancedFrames = TextBoxControl("6", 128, 104, 45); advancedColumns = TextBoxControl("6", 224, 104, 45); advancedRows = TextBoxControl("4", 288, 104, 45); advancedTotal = TextBoxControl("22", 352, 104, 45);
        advancedFirstIndex = TextBoxControl("", 128, 136, 75); advancedHeight = TextBoxControl("", 128, 168, 75); advancedWidth = TextBoxControl("", 128, 200, 75); advancedBmp = TextBoxControl("", 128, 232, 75);
        advancedSearchButton = Button("Buscar", 216, 136, 55, 23, BrowseBitmap_Click); advancedCalculateButton = Button("AutoCalcular", 224, 200, 80, 23, (_, _) => CalculateNormal());
        var advancedCreate = Button("Crear", 112, 296, 75, 23, Create_Click);
        Add(advancedFrame, advancedType, Label("nºframes:", 48, 104), advancedFrames, Label("A lo ancho", 112, 80), advancedColumns, Label("A lo alto", 200, 80), advancedRows, Label("Totales", 320, 80), advancedTotal, Label("Primer indice:", 48, 136), advancedFirstIndex, advancedSearchButton, Label("Alto:", 48, 168), advancedHeight, Label("Ancho:", 48, 200), advancedWidth, advancedCalculateButton, Label("BMP:", 48, 232), advancedBmp, advancedCreate, new RadioButton { Text = "(pj) alkon", Location = new Point(304, 185), AutoSize = true }, new RadioButton { Text = "(pj) clasico", Location = new Point(304, 208), AutoSize = true }, new RadioButton { Text = "Normal(NPC)", Location = new Point(304, 231), AutoSize = true }, new CheckBox { Text = "Autodindexar", Location = new Point(24, 256), AutoSize = true });

        simpleBmp = TextBoxControl("", 120, 40, 115); simpleGrh = TextBoxControl("", 120, 80, 115); simpleSearchButton = Button("buscar", 264, 40, 55, 23, BrowseBitmap_Click); simpleIndexButton = Button("indexar", 144, 128, 95, 23, Create_Click);
        Add(simpleFrame, Label("Bmp:", 56, 40), simpleBmp, Label("Numero Grh:", 40, 80), simpleGrh, simpleSearchButton, simpleIndexButton);

        bitmapText = normalBmp; startGrh = normalFirstIndex; columns = normalColumns; rows = normalRows; cellWidth = normalWidth; cellHeight = normalHeight; speed = normalFrames; createAnimation = new CheckBox { Text = "Crear animación", Visible = false };
        browseBitmapButton = normalSearchButton; createButton = normalCreate;
        Controls.AddRange(new Control[] { normalButton, advancedButton, simpleButton, normalFrame, advancedFrame, simpleFrame });
        ClientSize = new Size(425, 405); StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Text = "Indexador automatico";
    }

    private static Button Button(string text, int x, int y, int w, int h, EventHandler handler) { var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(w, h) }; b.Click += handler; return b; }
    private static TextBox TextBoxControl(string text, int x, int y, int w) => new() { Text = text, Location = new Point(x, y), Size = new Size(w, 23) };
    private static Label Label(string text, int x, int y) => new() { Text = text, Location = new Point(x, y + 3), AutoSize = true };
    private static void Add(Control parent, params Control[] controls) { parent.Controls.AddRange(controls); }
    private void ShowMode(Control active) { normalFrame.Visible = advancedFrame.Visible = simpleFrame.Visible = false; active.Visible = true; }
    private void CalculateNormal() { }
}
