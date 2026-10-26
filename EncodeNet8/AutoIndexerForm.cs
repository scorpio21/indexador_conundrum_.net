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

public sealed class AutoIndexerForm : Form
{
    private readonly TextBox bitmapText = new();
    private readonly NumericUpDown startGrh = Numeric(1, 1, int.MaxValue);
    private readonly NumericUpDown columns = Numeric(1, 1, 25);
    private readonly NumericUpDown rows = Numeric(1, 1, 25);
    private readonly NumericUpDown cellWidth = Numeric(32, 1, short.MaxValue);
    private readonly NumericUpDown cellHeight = Numeric(32, 1, short.MaxValue);
    private readonly NumericUpDown speed = Numeric(1, 1, short.MaxValue);
    private readonly CheckBox createAnimation = new() { Text = "Crear también una animación", AutoSize = true };

    public AutoIndexerOptions? Options { get; private set; }

    public AutoIndexerForm(string graphicsFolder)
    {
        Text = "Autoindexador";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(550, 285);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 3, RowCount = 9 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));
        AddRow(layout, 0, "BMP:", bitmapText, "Examinar", (_, _) => BrowseBitmap(graphicsFolder));
        AddRow(layout, 1, "Primer GRH:", startGrh);
        AddRow(layout, 2, "Columnas:", columns);
        AddRow(layout, 3, "Filas:", rows);
        AddRow(layout, 4, "Ancho de frame:", cellWidth);
        AddRow(layout, 5, "Alto de frame:", cellHeight);
        AddRow(layout, 6, "Velocidad:", speed);
        layout.Controls.Add(createAnimation, 1, 7);

        var create = new Button { Text = "Crear", DialogResult = DialogResult.OK, AutoSize = true };
        create.Click += Create_Click;
        var cancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.Add(cancel); buttons.Controls.Add(create);
        layout.Controls.Add(buttons, 0, 8); layout.SetColumnSpan(buttons, 3);
        Controls.Add(layout);
        AcceptButton = create;
        CancelButton = cancel;
    }

    private static NumericUpDown Numeric(decimal value, decimal min, decimal max) => new() { Value = value, Minimum = min, Maximum = max, Dock = DockStyle.Fill };

    private static void AddRow(TableLayoutPanel layout, int row, string label, Control control, string? buttonText = null, EventHandler? handler = null)
    {
        layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        layout.Controls.Add(control, 1, row);
        if (buttonText is not null)
        {
            var button = new Button { Text = buttonText, Dock = DockStyle.Fill };
            button.Click += handler;
            layout.Controls.Add(button, 2, row);
        }
    }

    private void BrowseBitmap(string graphicsFolder)
    {
        using var dialog = new OpenFileDialog { Filter = "Bitmap (*.bmp)|*.bmp|Todos los archivos (*.*)|*.*", InitialDirectory = graphicsFolder };
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
            BitmapPath = bitmapText.Text,
            StartGrh = (int)startGrh.Value,
            Columns = (int)columns.Value,
            Rows = (int)rows.Value,
            CellWidth = (int)cellWidth.Value,
            CellHeight = (int)cellHeight.Value,
            CreateAnimation = createAnimation.Checked,
            AnimationSpeed = (int)speed.Value
        };
    }
}
