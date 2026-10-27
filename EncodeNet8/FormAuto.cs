using System.Drawing.Drawing2D;

namespace EncodeNet8;

public partial class FormAuto : Form
{
    private readonly string _graphicsFolder;
    private readonly int _maxGrh;
    private readonly HashSet<int> _usedGrh;

    public IReadOnlyList<GrhRecord>? GeneratedRecords { get; private set; }

    public FormAuto() : this(string.Empty, int.MaxValue, Array.Empty<int>()) { }

    public FormAuto(string graphicsFolder, int maxGrh, IEnumerable<int>? usedGrh = null)
    {
        _graphicsFolder = graphicsFolder;
        _maxGrh = maxGrh > 0 ? maxGrh : int.MaxValue;
        _usedGrh = usedGrh?.ToHashSet() ?? new HashSet<int>();

        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (DesignMode) return;

        WireEvents();
        InitialiseOriginalState();
    }

    private void WireEvents()
    {
        Command3.Click += (_, _) => ShowMode(groupBox2);
        Command4.Click += (_, _) => ShowMode(GroupBox);
        ISimple.Click += (_, _) => ShowMode(groupBox1);

        radioButton6.CheckedChanged += (_, _) => UpdateNormalInputs();
        button5.Click += (_, _) => FindNormalIndex();
        button4.Click += (_, _) => CalculateNormalCellSize();
        button6.Click += (_, _) => CreateNormalIndex();

        comboBox2.SelectedIndexChanged += (_, _) => UpdateAdvancedMode();
        radioButton1.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        radioButton2.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        radioButton3.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        CommandBuscar.Click += (_, _) => FindAdvancedIndex();
        CommandCalu.Click += (_, _) => CalculateAdvancedCellSize();
        Command1.Click += (_, _) => CreateAdvancedIndex();

        Btn_Buscar3.Click += (_, _) => FindSimpleIndex();
        BtnIndexar.Click += (_, _) => CreateSimpleIndex();
    }

    private void InitialiseOriginalState()
    {
        Text = "Indexador automatico";
        GroupBox.Text = "Animacion Especial";
        groupBox2.Text = "Animacion normal";
        groupBox1.Text = "Indexacion Simple";

        // Animación normal: TextDatos(0..6) del VB6 = textBox21..textBox15.
        textBox21.Text = "1";   // A lo ancho
        textBox20.Text = "1";   // A lo alto
        textBox19.Text = "1";   // Totales
        textBox18.Text = "0";   // Primer índice
        textBox17.Text = "16";  // Alto
        textBox16.Text = "16";  // Ancho
        textBox15.Text = "1";   // BMP
        radioButton6.Checked = true;

        // Animación especial: ComboTipoAnim, Combo2, Text1 y Text2 del VB6.
        comboBox2.Items.Clear();
        comboBox2.Items.AddRange(new object[] { "Bodys/armas/Escudo", "Animacion con offset" });
        comboBox2.SelectedIndex = 0;
        comboBox1.Items.Clear();
        comboBox1.Items.AddRange(new object[] { "Body", "Arma", "Escudo" });
        comboBox1.SelectedIndex = 0;
        textBox8.Text = "280";
        textBox9.Text = "-38";
        radioButton1.Checked = true;

        ShowMode(groupBox2);
        UpdateNormalInputs();
        UpdateAdvancedMode();
        UpdateAdvancedDimensionMode();
    }

    private void ShowMode(Control active)
    {
        // Los tres GroupBox guardados por el diseñador son hermanos directos
        // del formulario.
        groupBox1.Visible = ReferenceEquals(active, groupBox1);
        GroupBox.Visible = ReferenceEquals(active, GroupBox);
        groupBox2.Visible = ReferenceEquals(active, groupBox2);

        active.BringToFront();
        active.Invalidate();
        active.Update();
    }

    private void UpdateNormalInputs()
    {
        var sameBitmap = radioButton6.Checked;
        textBox21.Enabled = sameBitmap;
        textBox20.Enabled = sameBitmap;
        if (!sameBitmap)
        {
            textBox21.Text = "1";
            textBox20.Text = "1";
        }
    }

    private void UpdateAdvancedMode()
    {
        // En el original estos controles permanecen visibles para el modo
        // Bodys/armas/Escudo, incluido el campo Offset inferior.
        comboBox1.Visible = true;
        textBox8.Visible = true;
        label11.Visible = true;
        textBox9.Visible = true;

        UpdateAdvancedDimensionMode();
    }

    private void UpdateAdvancedDimensionMode()
    {
        var fixedLayout = radioButton1.Checked || radioButton3.Checked;
        textBox1.Enabled = !fixedLayout;
        textBox5.Enabled = !fixedLayout;
        textBox6.Enabled = !fixedLayout;

        if (radioButton1.Checked)
        {
            textBox1.Text = "6";
            textBox2.Text = "4";
            textBox3.Text = "22";
            textBox5.Text = "46";
            textBox6.Text = "26";
            textBox9.Text = "-38";
        }
        else if (radioButton3.Checked)
        {
            textBox1.Text = "6";
            textBox2.Text = "4";
            textBox3.Text = "22";
            textBox5.Text = "45";
            textBox6.Text = "25";
            textBox9.Text = "-38";
        }
    }

    private void FindNormalIndex() => textBox18.Text = FindFreeIndex(Math.Max(1, Parse(textBox19, 1)) + 1).ToString();
    private void FindAdvancedIndex() => textBox4.Text = FindFreeIndex(Math.Max(1, Parse(textBox3, 1)) + 4).ToString();
    private void FindSimpleIndex() => textBoxGrh.Text = FindFreeIndex(1).ToString();

    private void CalculateNormalCellSize() => CalculateCellSize(textBox21, textBox20, textBox15, textBox17, textBox16);
    private void CalculateAdvancedCellSize() => CalculateCellSize(textBox1, textBox2, textBox7, textBox5, textBox6);

    private void CalculateCellSize(TextBox columnsField, TextBox rowsField, TextBox bmpField, TextBox heightField, TextBox widthField)
    {
        if (!TryGetBitmap(Parse(bmpField, 0), out var bitmap)) return;
        using (bitmap)
        {
            var columns = Math.Max(1, Parse(columnsField, 1));
            var rows = Math.Max(1, Parse(rowsField, 1));
            heightField.Text = (bitmap.Height / rows).ToString();
            widthField.Text = (bitmap.Width / columns).ToString();
        }
    }

    private void CreateNormalIndex()
    {
        try
        {
            var columns = Math.Max(1, Parse(textBox21, 1));
            var rows = Math.Max(1, Parse(textBox20, 1));
            var total = Math.Max(1, Parse(textBox19, columns * rows));
            var start = Math.Max(1, Parse(textBox18, FindFreeIndex(total + 1)));
            GeneratedRecords = BuildGrid(start, total, columns, rows, Parse(textBox15, 0), Math.Max(1, Parse(textBox16, 16)), Math.Max(1, Parse(textBox17, 16)), 0, 0, radioButton6.Checked);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void CreateAdvancedIndex()
    {
        try
        {
            var columns = Math.Max(1, Parse(textBox1, 6));
            var rows = Math.Max(1, Parse(textBox2, 4));
            var total = Math.Max(1, Parse(textBox3, columns * rows));
            var start = Math.Max(1, Parse(textBox4, FindFreeIndex(total + 4)));
            var offset = Parse(textBox9, -38);
            GeneratedRecords = BuildGrid(start, total, columns, rows, Parse(textBox7, 0), Math.Max(1, Parse(textBox6, 26)), Math.Max(1, Parse(textBox5, 46)), offset, 0, true);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void CreateSimpleIndex()
    {
        try
        {
            var bmpNumber = Parse(textBoxBmp, 0);
            var id = Math.Max(1, Parse(textBoxGrh, FindFreeIndex(1)));
            if (!TryGetBitmap(bmpNumber, out var bitmap)) throw new InvalidDataException($"No se encontró el BMP {bmpNumber}.");
            using (bitmap)
            {
                GeneratedRecords = new[] { new GrhRecord { Id = id, FileNumber = checked((short)bmpNumber), Width = checked((short)bitmap.Width), Height = checked((short)bitmap.Height), Frames = new List<int> { id } } };
            }
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private List<GrhRecord> BuildGrid(int start, int total, int columns, int rows, int bmpNumber, int width, int height, int offsetX, int offsetY, bool sameBitmap)
    {
        if (bmpNumber <= 0) throw new InvalidDataException("Indica un número de BMP válido.");
        if (start <= 0 || start > _maxGrh) throw new InvalidDataException("El primer índice no es válido.");
        if (total <= 0 || total > 25) throw new InvalidDataException("El total debe estar entre 1 y 25.");
        if (columns <= 0 || rows <= 0) throw new InvalidDataException("La cuadrícula debe tener columnas y filas válidas.");
        if (!TryGetBitmap(bmpNumber, out var bitmap)) throw new InvalidDataException($"No se encontró el BMP {bmpNumber}.");
        using (bitmap)
        {
            if (sameBitmap && (columns * width > bitmap.Width || rows * height > bitmap.Height))
                throw new InvalidDataException($"La cuadrícula no cabe en el BMP {bmpNumber}.");
        }

        var result = new List<GrhRecord>(total);
        for (var i = 0; i < total; i++)
        {
            var id = checked(start + i);
            if (id > _maxGrh || _usedGrh.Contains(id)) throw new InvalidDataException($"El GRH {id} ya existe o supera el máximo.");
            var column = i % columns;
            var row = i / columns;
            result.Add(new GrhRecord { Id = id, FileNumber = checked((short)bmpNumber), X = checked((short)(column * width + offsetX)), Y = checked((short)(row * height + offsetY)), Width = checked((short)width), Height = checked((short)height), Frames = new List<int> { id } });
        }
        return result;
    }

    private bool TryGetBitmap(int number, out Bitmap bitmap)
    {
        bitmap = null!;
        if (number <= 0 || string.IsNullOrWhiteSpace(_graphicsFolder)) return false;
        var path = Path.Combine(_graphicsFolder, $"{number}.bmp");
        if (!File.Exists(path)) return false;
        bitmap = new Bitmap(path);
        return true;
    }

    private int FindFreeIndex(int required)
    {
        for (var id = 1; id <= _maxGrh - required + 1; id++)
            if (Enumerable.Range(id, required).All(candidate => !_usedGrh.Contains(candidate))) return id;
        return 1;
    }

    private static int Parse(TextBox field, int fallback) => int.TryParse(field.Text, out var value) ? value : fallback;
    private void ShowError(Exception ex) => MessageBox.Show(this, ex.Message, "Autoindexador", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void GroupBox_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(SystemColors.ControlDark, 1) { CustomEndCap = new AdjustableArrowCap(4, 4) };
        var destino = new Point(CommandCalu.Left, CommandCalu.Top + CommandCalu.Height / 2);
        var origenAlto = new Point(textBox5.Right, textBox5.Top + textBox5.Height / 2);
        var origenAncho = new Point(textBox6.Right, textBox6.Top + textBox6.Height / 2);
        e.Graphics.DrawLine(pen, origenAlto, destino);
        e.Graphics.DrawLine(pen, origenAncho, destino);
    }

    private void groupBox2_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(SystemColors.ControlDark, 1) { CustomEndCap = new AdjustableArrowCap(4, 4) };

        var destino = new Point(button4.Left, button4.Top + button4.Height / 2);
        var origenAlto = new Point(textBox16!.Right, textBox16.Top + textBox16.Height / 2);
        var origenAncho = new Point(textBox17!.Right, textBox17.Top + textBox17.Height / 2);

        e.Graphics.DrawLine(pen, origenAlto, destino);
        e.Graphics.DrawLine(pen, origenAncho, destino);
    }
}
