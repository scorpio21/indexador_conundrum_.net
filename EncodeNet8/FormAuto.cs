using System.Drawing.Drawing2D;

namespace EncodeNet8;

public partial class FormAuto : Form
{
    private readonly string _graphicsFolder;
    private readonly int _maxGrh;
    private readonly HashSet<int> _usedGrh;

    public IReadOnlyList<GrhRecord>? GeneratedRecords { get; private set; }
    public bool OverwriteExisting { get; private set; }

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
        if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;

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

        ComboTipoAnim.SelectedIndexChanged += (_, _) => UpdateAdvancedMode();
        checkAuto.CheckedChanged += (_, _) => UpdateAutoIndexInputs();
        Optiondimension_2.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        Optiondimension_0.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        Optiondimension_1.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
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
        ComboTipoAnim.Items.Clear();
        ComboTipoAnim.Items.AddRange(new object[] { "Bodys/armas/Escudo", "Animacion con offset" });
        ComboTipoAnim.SelectedIndex = 0;
        combo2.Items.Clear();
        combo2.Items.AddRange(new object[] { "Body", "Arma", "Escudo" });
        combo2.SelectedIndex = 0;
        text1.Text = "280";
        Text2.Text = "-38";
        checkAuto.Checked = false;
        Optiondimension_0.Checked = true;

        ShowMode(groupBox2);
        UpdateNormalInputs();
        UpdateAdvancedMode();
        UpdateAutoIndexInputs();
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
        // Equivalente a ComboTipoAnim_Click del formulario VB6.
        var isOffsetAnimation = ComboTipoAnim.SelectedIndex == 1;

        combo2.Visible = !isOffsetAnimation;
        text1.Visible = !isOffsetAnimation;
        label11.Visible = !isOffsetAnimation;
        Text2.Visible = !isOffsetAnimation;
        checkAuto.Visible = !isOffsetAnimation;
        Optiondimension_0.Visible = !isOffsetAnimation;
        Optiondimension_1.Visible = !isOffsetAnimation;
        Optiondimension_2.Visible = !isOffsetAnimation;

        Loff.Visible = isOffsetAnimation;
        Loffx.Visible = isOffsetAnimation;
        Loffy.Visible = isOffsetAnimation;
        TextDatos2_7.Visible = isOffsetAnimation;
        TextDatos2_8.Visible = isOffsetAnimation;

        if (isOffsetAnimation)
        {
            textDatos2_0.Enabled = true;
            textDatos2_1.Enabled = true;
            textDatos2_3.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
            CommandCalu.Enabled = true;
        }
        else
        {
            textDatos2_0.Text = "6";
            textDatos2_1.Text = "4";
            textDatos2_3.Text = "22";
            textBox5.Text = "46";
            textBox6.Text = "26";
            text1.Text = "280";
            Text2.Text = "-38";
            checkAuto.Checked = false;
            if (combo2.Items.Count > 0)
                combo2.SelectedIndex = 0;
            Optiondimension_0.Checked = true;
        }

        UpdateAutoIndexInputs();
        UpdateAdvancedDimensionMode();
    }

    private void UpdateAutoIndexInputs()
    {
        // Equivalente a CheckAuto_Click del formulario VB6.
        text1.Enabled = checkAuto.Checked;
        Text2.Enabled = checkAuto.Checked;
    }

    private void UpdateAdvancedDimensionMode()
    {
        if (ComboTipoAnim.SelectedIndex == 1)
        {
            // En Animacion con offset los tres Optiondimension no participan.
            CommandCalu.Enabled = true;
            textDatos2_0.Enabled = true;
            textDatos2_1.Enabled = true;
            textDatos2_3.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
            return;
        }

        if (Optiondimension_0.Checked)
        {
            // Optiondimension(0): (pj) clásico.
            CommandCalu.Enabled = false;
            textDatos2_0.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            ComboTipoAnim.Enabled = true;
            Text2.Text = "-38";
            textDatos2_0.Text = "6";
            textDatos2_1.Text = "4";
            textDatos2_3.Text = "22";
            textBox5.Text = "46";
            textBox6.Text = "26";
        }
        else if (Optiondimension_1.Checked)
        {
            // Optiondimension(1): Normal (NPC).
            CommandCalu.Enabled = true;
            textDatos2_0.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
            ComboTipoAnim.Enabled = false;
            ComboTipoAnim.SelectedIndex = 0;
            textDatos2_1.Text = "4";
            Text2.Text = "0";
        }
        else if (Optiondimension_2.Checked)
        {
            // Optiondimension(2): (pj) Alkon.
            CommandCalu.Enabled = false;
            textDatos2_0.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            ComboTipoAnim.Enabled = true;
            Text2.Text = "-38";
            textDatos2_0.Text = "6";
            textDatos2_1.Text = "4";
            textDatos2_3.Text = "22";
            textBox5.Text = "45";
            textBox6.Text = "25";
        }
    }

    private void FindNormalIndex()
    {
        // Equivalente a:
        // TextDatos(5).Text = BuscarGrHlibres(Val(TextDatos(6).Text) + 1)
        var totalFrames = Parse(textBox19, 0);
        var requiredIndexes = Math.Max(1, totalFrames + 1);
        textBox18.Text = FindFreeIndex(requiredIndexes).ToString();
    }
    private void FindAdvancedIndex()
    {
        // Equivalente a:
        // TextDatos2(5).Text = BuscarGrHlibres(Val(TextDatos2(6).Text) + 4)
        var totalFrames = Parse(textDatos2_3, 0);
        var requiredIndexes = Math.Max(1, totalFrames + 4);
        textBox4.Text = FindFreeIndex(requiredIndexes).ToString();
    }
    private void FindSimpleIndex() => textBoxGrh.Text = FindFreeIndex(1).ToString();

    private void CalculateNormalCellSize()
    {
        try
        {
            // Equivalente a CommandCalu_Click del formulario VB6.
            // TextDatos(0..6) = columnas, filas, alto, ancho, BMP,
            // primer índice y total de frames.
            var normalFields = new[] { textBox21, textBox20, textBox17, textBox16, textBox15, textBox18, textBox19 };
            for (var i = 1; i < normalFields.Length; i++)
            {
                var value = Parse(normalFields[i], 0);
                if (value > _maxGrh || value <= 0)
                    normalFields[i].Text = "0";
            }

            var totalFrames = Parse(textBox19, 0);
            if (totalFrames > 25)
                textBox19.Text = "25";
            else if (totalFrames <= 0)
                textBox19.Text = "0";

            var columns = Parse(textBox21, 0);
            var rows = Parse(textBox20, 0);
            if (columns < 1 || rows < 1) return;

            var bmpNumber = Parse(textBox15, 0);
            if (!TryGetBitmap(bmpNumber, out var bitmap)) return;

            using (bitmap)
            {
                textBox17.Text = (bitmap.Height / rows).ToString();
                textBox16.Text = (bitmap.Width / columns).ToString();
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }
    private void CalculateAdvancedCellSize()
    {
        try
        {
            // Equivalente a CommandCalu2_Click del formulario VB6.
            // TextDatos2(0..6) = columnas, filas, alto, ancho, BMP,
            // primer índice y total de frames.
            var advancedFields = new[] { textDatos2_0, textDatos2_1, textBox5, textBox6, textBox7, textBox4, textDatos2_3 };
            for (var i = 1; i < advancedFields.Length; i++)
            {
                var value = Parse(advancedFields[i], 0);
                if (value > _maxGrh || value <= 0)
                    advancedFields[i].Text = "0";
            }

            var totalFrames = Parse(textDatos2_3, 0);
            if (ComboTipoAnim.SelectedIndex > 1 && totalFrames > 25)
                textDatos2_3.Text = "25";
            else if (totalFrames <= 0)
                textDatos2_3.Text = "0";

            var columns = Parse(textDatos2_0, 0);
            var rows = Parse(textDatos2_1, 0);
            if (columns < 1 || rows < 1) return;

            var bmpNumber = Parse(textBox7, 0);
            if (!TryGetBitmap(bmpNumber, out var bitmap)) return;

            using (bitmap)
            {
                textBox5.Text = (bitmap.Height / rows).ToString();
                textBox6.Text = (bitmap.Width / columns).ToString();
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

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
            var columns = Parse(textBox21, 0);
            var rows = Parse(textBox20, 0);
            var total = Parse(textBox19, 0);
            var start = Math.Max(1, Parse(textBox18, FindFreeIndex(total + 1)));
            GeneratedRecords = BuildGrid(
                start,
                total,
                columns,
                rows,
                Parse(textBox15, 0),
                Parse(textBox16, 0),
                Parse(textBox17, 0),
                0,
                0,
                radioButton6.Checked,
                createAnimation: true);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void CreateAdvancedIndex()
    {
        try
        {
            var columns = Math.Max(1, Parse(textDatos2_0, 6));
            var rows = Math.Max(1, Parse(textDatos2_1, 4));
            var total = Math.Max(1, Parse(textDatos2_3, columns * rows));
            var start = Math.Max(1, Parse(textBox4, FindFreeIndex(total + 4)));
            var offset = Parse(Text2, -38);
            GeneratedRecords = BuildGrid(start, total, columns, rows, Parse(textBox7, 0), Math.Max(1, Parse(textBox6, 26)), Math.Max(1, Parse(textBox5, 46)), offset, 0, true);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void CreateSimpleIndex()
    {
        try
        {
            // Equivalente a Command6_Click del formulario VB6.
            var bmpNumber = Parse(textBoxBmp, 0);
            var id = Parse(textBoxGrh, 0);
            OverwriteExisting = false;

            if (bmpNumber <= 0 || bmpNumber > _maxGrh)
                textBoxBmp.Text = "0";
            if (id <= 0 || id > _maxGrh)
                textBoxGrh.Text = "0";

            bmpNumber = Parse(textBoxBmp, 0);
            id = Parse(textBoxGrh, 0);
            if (id <= 0 || id > _maxGrh)
            {
                MessageBox.Show(this, "El índice de autoindexación no es válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_usedGrh.Contains(id))
            {
                var response = MessageBox.Show(
                    this,
                    $"El gráfico {id} ya existe, ¿estás seguro de sobrescribirlo?",
                    "Aviso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (response != DialogResult.Yes) return;
                OverwriteExisting = true;
            }

            if (!TryGetBitmap(bmpNumber, out var bitmap))
            {
                MessageBox.Show(this, $"El BMP {bmpNumber} no existe.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (bitmap)
            {
                GeneratedRecords = new[] { new GrhRecord { Id = id, FileNumber = checked((short)bmpNumber), Width = checked((short)bitmap.Width), Height = checked((short)bitmap.Height), Frames = new List<int> { id } } };
            }
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private List<GrhRecord> BuildGrid(
        int start,
        int total,
        int columns,
        int rows,
        int bmpNumber,
        int width,
        int height,
        int offsetX,
        int offsetY,
        bool sameBitmap,
        bool createAnimation = false)
    {
        if (bmpNumber <= 0) throw new InvalidDataException("Indica un número de BMP válido.");
        if (start <= 0 || start > _maxGrh) throw new InvalidDataException("El primer índice no es válido.");
        if (total <= 0 || total > 25) throw new InvalidDataException("El total debe estar entre 1 y 25.");
        if (columns <= 0 || rows <= 0) throw new InvalidDataException("La cuadrícula debe tener columnas y filas válidas.");
        if (createAnimation && start > _maxGrh - total)
            throw new InvalidDataException("No hay sitio para la animación.");
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
            result.Add(new GrhRecord
            {
                Id = id,
                FileNumber = checked((short)(sameBitmap ? bmpNumber : bmpNumber + i)),
                X = checked((short)(sameBitmap ? column * width + offsetX : 0)),
                Y = checked((short)(sameBitmap ? row * height + offsetY : 0)),
                Width = checked((short)width),
                Height = checked((short)height),
                Frames = new List<int> { id }
            });
        }

        if (createAnimation)
        {
            var animationId = checked(start + total);
            if (_usedGrh.Contains(animationId))
                throw new InvalidDataException($"El GRH {animationId} ya existe o supera el máximo.");

            var firstFrame = result[0];
            result.Add(new GrhRecord
            {
                Id = animationId,
                FileNumber = firstFrame.FileNumber,
                X = firstFrame.X,
                Y = firstFrame.Y,
                Width = firstFrame.Width,
                Height = firstFrame.Height,
                Speed = 1,
                Frames = result.Select(frame => frame.Id).ToList()
            });
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
