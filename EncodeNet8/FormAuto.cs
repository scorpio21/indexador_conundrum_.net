using System.Drawing.Drawing2D;

namespace EncodeNet8;

public partial class FormAuto : Form
{
    private readonly string _graphicsFolder;
    private readonly int _maxGrh;
    private readonly HashSet<int> _usedGrh;
    private readonly HashSet<int> _usedDirectional;
    private bool _updatingInputs;

    public IReadOnlyList<GrhRecord>? GeneratedRecords { get; private set; }
    public IReadOnlyList<GrhRecord> PreviewRecords { get; private set; } = Array.Empty<GrhRecord>();
    public DirectionalIndexRecord? GeneratedDirectionalIndex { get; private set; }
    public int GeneratedDirectionalKind { get; private set; } = -1;
    public bool OverwriteExisting { get; private set; }
    public event EventHandler? PreviewChanged;
    public event Action<int>? ResourcePreviewChanged;
    private int _lastResourcePreviewBmp;

    public FormAuto() : this(string.Empty, int.MaxValue, Array.Empty<int>(), Array.Empty<int>()) { }

    public FormAuto(string graphicsFolder, int maxGrh, IEnumerable<int>? usedGrh = null, IEnumerable<int>? usedDirectional = null)
    {
        _graphicsFolder = graphicsFolder;
        _maxGrh = maxGrh > 0 ? maxGrh : int.MaxValue;
        _usedGrh = usedGrh?.ToHashSet() ?? new HashSet<int>();
        _usedDirectional = usedDirectional?.ToHashSet() ?? new HashSet<int>();

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
        foreach (var field in new[] { textBox21, textBox20, textBox19, textBox18, textBox17, textBox16, textBox15 })
            field.TextChanged += (_, _) => ValidateNormalInputs();
        button5.Click += (_, _) => FindNormalIndex();
        button4.Click += (_, _) => CalculateNormalCellSize();
        button6.Click += (_, _) => CreateNormalIndex();

        ComboTipoAnim.SelectedIndexChanged += (_, _) => UpdateAdvancedMode();
        foreach (var field in new[] { textDatos2_0, textDatos2_1, textBox5, textBox6, textBox7, textBox4, textDatos2_3, TextDatos2_7, TextDatos2_8 })
            field.TextChanged += (_, _) => ValidateAdvancedInputs();
        text1.TextChanged += (_, _) => ValidateAdvancedBodyIndex();
        Text2.TextChanged += (_, _) => NormalizeOffsetText();
        checkAuto.CheckedChanged += (_, _) => UpdateAutoIndexInputs();
        Optiondimension_2.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        Optiondimension_0.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        Optiondimension_1.CheckedChanged += (_, _) => UpdateAdvancedDimensionMode();
        CommandBuscar.Click += (_, _) => FindAdvancedIndex();
        CommandCalu.Click += (_, _) => CalculateAdvancedCellSize();
        Command1.Click += (_, _) => CreateAdvancedIndex();

        Btn_Buscar3.Click += (_, _) => FindSimpleIndex();
        BtnIndexar.Click += (_, _) => CreateSimpleIndex();
        textBoxBmp.TextChanged += (_, _) => ValidateSimpleInputs();
        textBoxGrh.TextChanged += (_, _) => ValidateSimpleInputs();
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
        combo2.Items.AddRange(new object[] { "Body", "Arma", "Escudo", "Alas", "Espalda", "Botas" });
        combo2.SelectedIndex = 0;
        text1.Text = "280";
        Text2.Text = "-38º0";
        checkAuto.Checked = false;
        Optiondimension_0.Checked = true;

        ShowMode(groupBox2);
        UpdateNormalInputs();
        UpdateAdvancedMode();
        UpdateAutoIndexInputs();
        UpdateAdvancedDimensionMode();
        ValidateNormalInputs();
        ValidateAdvancedInputs();
        ValidateSimpleInputs();
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
        ValidateNormalInputs();
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
            Text2.Text = "-38º0";
            checkAuto.Checked = false;
            if (combo2.Items.Count > 0)
                combo2.SelectedIndex = 0;
            Optiondimension_0.Checked = true;
        }

        UpdateAutoIndexInputs();
        UpdateAdvancedDimensionMode();
        ValidateAdvancedInputs();
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
            Text2.Text = "-38º0";
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
            Text2.Text = "0º0";
        }
        else if (Optiondimension_2.Checked)
        {
            // Optiondimension(2): (pj) Alkon.
            CommandCalu.Enabled = false;
            textDatos2_0.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            ComboTipoAnim.Enabled = true;
            Text2.Text = "-38º0";
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

    private void ValidateNormalInputs()
    {
        if (_updatingInputs || IsDisposed) return;
        _updatingInputs = true;
        try
        {
            var fields = new[] { textBox21, textBox20, textBox17, textBox16, textBox15, textBox18, textBox19 };
            for (var i = 1; i < fields.Length; i++)
            {
                var value = Parse(fields[i], 0);
                if (value > _maxGrh || value <= 0) fields[i].Text = "0";
            }

            var total = Parse(textBox19, 0);
            if (total > 25) textBox19.Text = "25";
            if (total <= 0) textBox19.Text = "0";
            total = Parse(textBox19, 0);

            button5.Enabled = total > 0;
            var start = Parse(textBox18, 0);
            textBox18.BackColor = HasFreeIndexes(start, total + 1) ? SystemColors.Window : Color.Red;

            var bmp = Parse(textBox15, 0);
            if (!TryGetBitmap(bmp, out var bitmap))
            {
                textBox15.BackColor = Color.Red;
                button4.Enabled = false;
                UpdatePreview();
                return;
            }

            using (bitmap)
            {
                textBox15.BackColor = SystemColors.Window;
                button4.Enabled = true;
                NotifyResourcePreview(bmp);
                var rows = Parse(textBox20, 0);
                var columns = Parse(textBox21, 0);
                textBox17.BackColor = rows > 0 && rows * Parse(textBox17, 0) > bitmap.Height ? Color.Yellow : SystemColors.Window;
                textBox16.BackColor = columns > 0 && columns * Parse(textBox16, 0) > bitmap.Width ? Color.Yellow : SystemColors.Window;
            }
            UpdatePreview();
        }
        finally { _updatingInputs = false; }
    }

    private void ValidateAdvancedInputs()
    {
        if (_updatingInputs || IsDisposed) return;
        _updatingInputs = true;
        try
        {
            var fields = new[] { textDatos2_0, textDatos2_1, textBox5, textBox6, textBox7, textBox4, textDatos2_3 };
            for (var i = 1; i < fields.Length; i++)
            {
                var value = Parse(fields[i], 0);
                if (value > _maxGrh || value <= 0) fields[i].Text = "0";
            }

            var columns = Parse(textDatos2_0, 0);
            var rows = Parse(textDatos2_1, 0);
            var total = Parse(textDatos2_3, 0);
            if (ComboTipoAnim.SelectedIndex == 0 && Optiondimension_1.Checked)
            {
                total = Math.Max(0, columns * 4);
                textDatos2_3.Text = total.ToString();
            }
            if (ComboTipoAnim.SelectedIndex > 1 && total > 25) textDatos2_3.Text = "25";
            if (total <= 0) textDatos2_3.Text = "0";
            total = Parse(textDatos2_3, 0);

            CommandBuscar.Enabled = total > 0;
            var start = Parse(textBox4, 0);
            textBox4.BackColor = HasFreeIndexes(start, total + 4) ? SystemColors.Window : Color.Red;

            var bmp = Parse(textBox7, 0);
            if (!TryGetBitmap(bmp, out var bitmap))
            {
                textBox7.BackColor = Color.Red;
                CommandCalu.Enabled = false;
                UpdatePreview();
                return;
            }

            using (bitmap)
            {
                textBox7.BackColor = SystemColors.Window;
                CommandCalu.Enabled = Optiondimension_1.Checked || ComboTipoAnim.SelectedIndex == 1;
                NotifyResourcePreview(bmp);
                textBox5.BackColor = rows > 0 && rows * Parse(textBox5, 0) > bitmap.Height ? Color.Yellow : SystemColors.Window;
                textBox6.BackColor = columns > 0 && columns * Parse(textBox6, 0) > bitmap.Width ? Color.Yellow : SystemColors.Window;
            }
            UpdatePreview();
        }
        finally { _updatingInputs = false; }
    }

    private void ValidateAdvancedBodyIndex()
    {
        if (int.TryParse(text1.Text, out var value) && value > 0 && value <= _maxGrh && !_usedDirectional.Contains(value))
            text1.BackColor = SystemColors.Window;
        else
            text1.BackColor = Color.Red;
    }

    private void NormalizeOffsetText()
    {
        if (_updatingInputs || string.IsNullOrWhiteSpace(Text2.Text)) return;
        var parts = Text2.Text.Split('º', StringSplitOptions.TrimEntries);
        if (parts.Length == 0) return;
        var x = ParseValue(parts[0], 0);
        var y = parts.Length > 1 ? ParseValue(parts[1], 0) : 0;
        x = Math.Clamp(x, -_maxGrh, _maxGrh);
        y = Math.Clamp(y, -_maxGrh, _maxGrh);
        var normalized = $"{x}º{y}";
        if (!string.Equals(Text2.Text, normalized, StringComparison.Ordinal)) Text2.Text = normalized;
    }

    private void ValidateSimpleInputs()
    {
        Bitmap? bitmap = null;
        var validBitmap = int.TryParse(textBoxBmp.Text, out var bmp) && bmp > 0 && TryGetBitmap(bmp, out bitmap);
        if (validBitmap && bitmap is not null)
        {
            bitmap.Dispose();
            textBoxBmp.BackColor = SystemColors.Window;
            if (groupBox1.Visible) NotifyResourcePreview(bmp);
        }
        else
        {
            textBoxBmp.BackColor = Color.Red;
            _lastResourcePreviewBmp = 0;
        }

        if (int.TryParse(textBoxGrh.Text, out var id) && id > 0 && id <= _maxGrh)
            textBoxGrh.BackColor = HasFreeIndexes(id, 1) ? SystemColors.Window : Color.Yellow;
        else
            textBoxGrh.BackColor = Color.Red;
        UpdatePreview();
    }

    private void NotifyResourcePreview(int bmp)
    {
        if (bmp <= 0 || bmp == _lastResourcePreviewBmp) return;
        _lastResourcePreviewBmp = bmp;
        ResourcePreviewChanged?.Invoke(bmp);
    }

    private void UpdatePreview()
    {
        if (IsDisposed) return;
        try
        {
            if (groupBox2.Visible)
            {
                var columns = Parse(textBox21, 0);
                var rows = Parse(textBox20, 0);
                var total = Parse(textBox19, 0);
                PreviewRecords = TryBuildPreview(
                    Parse(textBox18, 0), total, columns, rows, Parse(textBox15, 0),
                    Math.Max(1, Parse(textBox16, 16)), Math.Max(1, Parse(textBox17, 16)),
                    0, 0, radioButton6.Checked, false, null, false);
            }
            else if (GroupBox.Visible)
            {
                var columns = Math.Max(1, Parse(textDatos2_0, 6));
                var rows = Math.Max(1, Parse(textDatos2_1, 4));
                var total = Math.Max(1, Parse(textDatos2_3, columns * rows));
                var directional = ComboTipoAnim.SelectedIndex == 0;
                var positions = Optiondimension_2.Checked ? CreateAlkonPositions() : CreateClassicPositions();
                PreviewRecords = TryBuildPreview(
                    Parse(textBox4, 0), total, columns, rows, Parse(textBox7, 0),
                    Math.Max(1, Parse(textBox6, 26)), Math.Max(1, Parse(textBox5, 46)),
                    ComboTipoAnim.SelectedIndex == 1 ? Parse(TextDatos2_7, 0) : 0,
                    ComboTipoAnim.SelectedIndex == 1 ? Parse(TextDatos2_8, 0) : 0,
                    true, false, directional && !Optiondimension_1.Checked ? positions : null,
                    directional && !Optiondimension_1.Checked);
            }
            else
            {
                var bmp = Parse(textBoxBmp, 0);
                PreviewRecords = TryBuildPreview(Parse(textBoxGrh, 0), 1, 1, 1, bmp, 1, 1, 0, 0, true, false, null, false);
            }
        }
        catch
        {
            PreviewRecords = Array.Empty<GrhRecord>();
        }
        PreviewChanged?.Invoke(this, EventArgs.Empty);
    }

    private List<GrhRecord> TryBuildPreview(
        int start, int total, int columns, int rows, int bmpNumber, int width, int height,
        int offsetX, int offsetY, bool sameBitmap, bool directionalAnimations,
        IReadOnlyList<Point>? legacyPositions, bool legacyFrameReferences)
    {
        if (start <= 0 || total <= 0 || columns <= 0 || rows <= 0 || bmpNumber <= 0) return new List<GrhRecord>();
        return BuildGrid(start, total, columns, rows, bmpNumber, width, height, offsetX, offsetY,
            sameBitmap, createAnimation: false, directionalAnimations: directionalAnimations,
            legacyDirectionalPositions: legacyPositions, legacyFrameReferences: legacyFrameReferences,
            ignoreUsedIndexes: true);
    }

    private static int ParseValue(string value, int fallback) => int.TryParse(value, out var result) ? result : fallback;

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
            var start = Parse(textBox18, 0);
            if (!HasFreeIndexes(start, total + 1))
                throw new InvalidDataException("No hay sitio para la animación o el primer índice no es válido.");
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
            var start = Parse(textBox4, 0);
            if (!HasFreeIndexes(start, total + 4))
                throw new InvalidDataException("No hay sitio para la animación avanzada o el primer índice no es válido.");
            var offset = ComboTipoAnim.SelectedIndex == 1 ? Parse(TextDatos2_7, 0) : 0;
            var offsetY = ComboTipoAnim.SelectedIndex == 1 ? Parse(TextDatos2_8, 0) : 0;
            var directional = ComboTipoAnim.SelectedIndex == 0;
            var useLegacyDirectionalLayout = directional && !Optiondimension_1.Checked;
            var directionalPositions = Optiondimension_2.Checked
                ? CreateAlkonPositions()
                : CreateClassicPositions();
            GeneratedRecords = BuildGrid(
                start,
                total,
                columns,
                rows,
                Parse(textBox7, 0),
                Math.Max(1, Parse(textBox6, 26)),
                Math.Max(1, Parse(textBox5, 46)),
                offset,
                offsetY,
                true,
                createAnimation: directional || ComboTipoAnim.SelectedIndex == 1,
                directionalAnimations: directional,
                legacyDirectionalPositions: useLegacyDirectionalLayout ? directionalPositions : null,
                legacyFrameReferences: useLegacyDirectionalLayout);
            if (directional && checkAuto.Checked)
            {
                var bodyId = Parse(text1, 0);
                if (bodyId <= 0 || bodyId > _maxGrh)
                    throw new InvalidDataException("El índice de autoindexación no es válido.");
                if (_usedDirectional.Contains(bodyId))
                {
                    var response = MessageBox.Show(this, $"El índice {bodyId} ya existe, ¿deseas sobrescribirlo?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (response != DialogResult.Yes) return;
                }
                var animationIds = GeneratedRecords.Skip(total).Take(4).Select(record => record.Id).ToArray();
                var offsetParts = ParseOffset(Text2.Text);
                GeneratedDirectionalIndex = new DirectionalIndexRecord
                {
                    Id = bodyId,
                    Directions = new[] { animationIds[1], animationIds[3], animationIds[0], animationIds[2] },
                    OffsetX = (short)Math.Clamp(offsetParts.X, short.MinValue, short.MaxValue),
                    OffsetY = (short)Math.Clamp(offsetParts.Y, short.MinValue, short.MaxValue)
                };
                GeneratedDirectionalKind = combo2.SelectedIndex;
            }
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
        bool createAnimation = false,
        bool directionalAnimations = false,
        IReadOnlyList<Point>? legacyDirectionalPositions = null,
        bool legacyFrameReferences = false,
        bool ignoreUsedIndexes = false)
    {
        if (bmpNumber <= 0) throw new InvalidDataException("Indica un número de BMP válido.");
        if (start <= 0 || start > _maxGrh) throw new InvalidDataException("El primer índice no es válido.");
        if (total <= 0 || total > 25) throw new InvalidDataException("El total debe estar entre 1 y 25.");
        if (columns <= 0 || rows <= 0) throw new InvalidDataException("La cuadrícula debe tener columnas y filas válidas.");
        if (legacyDirectionalPositions is not null && legacyDirectionalPositions.Count < total)
            throw new InvalidDataException("La animación direccional necesita una posición para cada frame.");
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
            if (id > _maxGrh || (!ignoreUsedIndexes && _usedGrh.Contains(id))) throw new InvalidDataException($"El GRH {id} ya existe o supera el máximo.");
            var column = i % columns;
            var row = i / columns;
            var sourceX = legacyDirectionalPositions is null ? column * width + offsetX : legacyDirectionalPositions[i].X;
            var sourceY = legacyDirectionalPositions is null ? row * height + offsetY : legacyDirectionalPositions[i].Y;
            result.Add(new GrhRecord
            {
                Id = id,
                FileNumber = checked((short)(sameBitmap ? bmpNumber : bmpNumber + i)),
                X = checked((short)(sameBitmap ? sourceX : 0)),
                Y = checked((short)(sameBitmap ? sourceY : 0)),
                Width = checked((short)width),
                Height = checked((short)height),
                Frames = new List<int> { legacyFrameReferences ? i + 1 : id }
            });
        }

        if (createAnimation && directionalAnimations)
        {
            var groupSizes = rows == 4 && columns == 6 && total >= 22
                ? new[] { 6, 6, 5, 5 }
                : Enumerable.Repeat(columns, 4).ToArray();
            if (groupSizes.Sum() > result.Count)
                throw new InvalidDataException("La cuadrícula no contiene suficientes frames para las cuatro direcciones.");

            var frameOffset = 0;
            foreach (var groupSize in groupSizes)
            {
                var firstFrame = result[frameOffset];
                result.Add(new GrhRecord
                {
                    Id = checked(start + result.Count),
                    FileNumber = firstFrame.FileNumber,
                    X = firstFrame.X,
                    Y = firstFrame.Y,
                    Width = firstFrame.Width,
                    Height = firstFrame.Height,
                    Speed = 1,
                    Frames = result.Skip(frameOffset).Take(groupSize).Select(frame => frame.Id).ToList()
                });
                frameOffset += groupSize;
            }
        }
        else if (createAnimation)
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
        if (required <= 0 || required > _maxGrh) return 0;
        for (var id = 1; id <= _maxGrh - required + 1; id++)
            if (Enumerable.Range(id, required).All(candidate => !_usedGrh.Contains(candidate))) return id;
        return 0;
    }

    private bool HasFreeIndexes(int first, int required)
    {
        if (first <= 0 || required <= 0 || first > _maxGrh - required + 1) return false;
        return Enumerable.Range(first, required).All(candidate => !_usedGrh.Contains(candidate));
    }

    private static int Parse(TextBox field, int fallback) => int.TryParse(field.Text, out var value) ? value : fallback;

    private static IReadOnlyList<Point> CreateClassicPositions()
    {
        var x = new[] { 0, 25, 49, 73, 98, 123 };
        var y = new[] { 0, 45, 90, 135 };
        return Enumerable.Range(0, 22)
            .Select(index => new Point(x[index % 6], y[index / 6]))
            .ToArray();
    }

    private static IReadOnlyList<Point> CreateAlkonPositions()
    {
        var x = new[] { 0, 25, 50, 75, 100, 125 };
        var y = new[] { 0, 45, 90, 135 };
        return Enumerable.Range(0, 22)
            .Select(index => new Point(x[index % 6], y[index / 6]))
            .ToArray();
    }

    private static (int X, int Y) ParseOffset(string text)
    {
        var parts = text.Split('º', StringSplitOptions.TrimEntries);
        var y = parts.Length > 0 ? ParseValue(parts[0], 0) : 0;
        var x = parts.Length > 1 ? ParseValue(parts[1], 0) : 0;
        return (x, y);
    }
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
