#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
    private GraficosIndFile? _file;
    private string? _loadedPath;
    private readonly IndexerSettings _settings;
    private readonly System.Windows.Forms.Timer _drawTimer = new() { Interval = 150 };
    private readonly Dictionary<string, Bitmap> _bitmapCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly System.Diagnostics.Stopwatch _animationClock = System.Diagnostics.Stopwatch.StartNew();
    private long _animationElapsed;
    private int _animationIntervalMs = 150;
    private GrhRecord? _selectedRecord;
    private int _animationFrame;
    private string _indexerState = "Grh";
    private readonly Dictionary<string, int> _lastIndexByState = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<BodyIndexRecord> _bodyRecords = new();
    private readonly List<DirectionalIndexRecord> _directionalRecords = new();
    private readonly Dictionary<string, List<DirectionalIndexRecord>> _pendingDirectionalRecords = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<FxIndexRecord> _fxRecords = new();
    private readonly Dictionary<int, int> _referenceCounts = new();
    private string? _directionalPath;
    private string? _iniDirectionalPath;
    private string? _fxPath;
    private GrhStorageFormat _directionalFormat;
    private GrhStorageFormat _fxFormat;
    private List<int> _searchResults = new();
    private int _searchCursor;
    private int _savedIndexSlot;
    private bool _greenBackground;
    private bool _resourceSearchMode;
    private bool _weMode;
    private int cabezaActual;
    private int _drawWalk;
    private short _previewHeadOffsetX;
    private short _previewHeadOffsetY;
    private IReadOnlyList<GrhRecord>? _autoPreviewRecords;
    private FrmSearch? _searchWindow;

    private static readonly string[] AllStates = { "Grh", "Body", "Cabezas", "Cascos", "Escudos", "Armas", "Alas", "Capas", "Fx", "Resource", "Botas" };

    // Conservamos también los Icon para mantener vivos sus handles nativos.
    // Crear el Cursor directamente desde Properties.Resources.X.Handle dejaba
    // el handle expuesto a la finalización del Icon y los cursores desaparecían
    // después de un tiempo.
    private readonly Icon _bastonIcon = (Icon)Properties.Resources.baston.Clone();
    private readonly Icon _diabloIcon = (Icon)Properties.Resources.diablo.Clone();
    private readonly Icon _manoIcon = (Icon)Properties.Resources.mano.Clone();
    private readonly Cursor _bastonCursor;
    private readonly Cursor _diabloCursor;
    private readonly Cursor _manoCursor;


    public MainForm()
    {
        InitializeComponent();
        _bastonCursor = new Cursor(_bastonIcon.Handle);
        _diabloCursor = new Cursor(_diabloIcon.Handle);
        _manoCursor = new Cursor(_manoIcon.Handle);
        _settings = IndexerSettings.Load();
        _drawTimer.Tick += drawTimer_Tick;
        FormClosed += (_, _) =>
        {
            DisposeBitmapCache();
            _bastonCursor.Dispose();
            _diabloCursor.Dispose();
            _manoCursor.Dispose();
            _bastonIcon.Dispose();
            _diabloIcon.Dispose();
            _manoIcon.Dispose();
        };
        Cursor = _manoCursor;
        ConfigureIcons();
        foreach (var box in new[] { bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText })
            box.DoubleClick += textData_DoubleClick;
        foreach (var box in new[] { bmpText, frameText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText })
            box.TextChanged += dataText_TextChanged;
        indsText.Text = _settings.InitPath;
        graphicsText.Text = _settings.GraphicsPath;
        Indicetext.Text = _settings.IndicesPath;
        formatCombo.SelectedIndex = 0;
        LoadConfiguredGraphics();
        UpdateUi();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Delete && !IsTextEditingActive() && indexList.Focused)
        {
            DeleteSelected();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void openButton_MouseEnter(object? sender, EventArgs e)
    {
        openButton.Cursor = _bastonCursor;
    }

    private void openButton_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            openButton.Cursor = _diabloCursor;
    }

    private void openButton_MouseUp(object? sender, MouseEventArgs e)
    {
        openButton.Cursor = IsPointerInside(openButton) ? _bastonCursor : _manoCursor;
    }

    private void openButton_MouseLeave(object? sender, EventArgs e)
    {
        openButton.Cursor = _manoCursor;
    }

    private static bool IsPointerInside(Control control)
    {
        var point = control.PointToClient(Cursor.Position);
        return control.ClientRectangle.Contains(point);
    }

}

#pragma warning restore CS8618

