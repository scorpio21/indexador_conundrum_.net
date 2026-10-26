#nullable enable

namespace EncodeNet8;

partial class AutoIndexerForm
{
    private System.ComponentModel.IContainer? components = null;
    private TextBox bitmapText = null!;
    private NumericUpDown startGrh = null!;
    private NumericUpDown columns = null!;
    private NumericUpDown rows = null!;
    private NumericUpDown cellWidth = null!;
    private NumericUpDown cellHeight = null!;
    private NumericUpDown speed = null!;
    private CheckBox createAnimation = null!;
    private Button browseBitmapButton = null!;
    private Button createButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        bitmapText = new TextBox(); startGrh = Numeric(1, 1, int.MaxValue); columns = Numeric(1, 1, 25);
        rows = Numeric(1, 1, 25); cellWidth = Numeric(32, 1, short.MaxValue); cellHeight = Numeric(32, 1, short.MaxValue); speed = Numeric(1, 1, short.MaxValue);
        createAnimation = new CheckBox(); browseBitmapButton = new Button(); createButton = new Button(); cancelButton = new Button();
        var layout = new TableLayoutPanel();
        SuspendLayout();
        layout.ColumnCount = 3; layout.RowCount = 9; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(10);
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
        AddRow(layout, 0, "BMP:", bitmapText, browseBitmapButton, "Examinar", BrowseBitmap_Click);
        AddRow(layout, 1, "Primer GRH:", startGrh); AddRow(layout, 2, "Columnas:", columns); AddRow(layout, 3, "Filas:", rows);
        AddRow(layout, 4, "Ancho de frame:", cellWidth); AddRow(layout, 5, "Alto de frame:", cellHeight); AddRow(layout, 6, "Velocidad:", speed);
        createAnimation.Text = "Crear también una animación"; createAnimation.AutoSize = true; layout.Controls.Add(createAnimation, 1, 7);
        createButton.Text = "Crear"; createButton.AutoSize = true; createButton.Click += Create_Click; createButton.DialogResult = DialogResult.OK;
        cancelButton.Text = "Cancelar"; cancelButton.AutoSize = true; cancelButton.DialogResult = DialogResult.Cancel;
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
        buttons.Controls.Add(cancelButton); buttons.Controls.Add(createButton); layout.Controls.Add(buttons, 0, 8); layout.SetColumnSpan(buttons, 3);
        Controls.Add(layout); AcceptButton = createButton; CancelButton = cancelButton;
        ClientSize = new Size(550, 285); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent; Text = "Autoindexador"; Name = "AutoIndexerForm";
        ResumeLayout(false);
    }

    private static NumericUpDown Numeric(decimal value, decimal min, decimal max) => new() { Value = value, Minimum = min, Maximum = max, Dock = DockStyle.Fill };

    private static void AddRow(TableLayoutPanel layout, int row, string label, Control control, Button? button = null, string? buttonText = null, EventHandler? handler = null)
    {
        layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        layout.Controls.Add(control, 1, row);
        if (button is not null)
        {
            button.Text = buttonText; button.Dock = DockStyle.Fill; button.Click += handler;
            layout.Controls.Add(button, 2, row);
        }
    }
}
