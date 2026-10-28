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
    private IReadOnlyList<GrhRecord>? _autoPreviewRecords;

    private static readonly string[] AllStates = { "Grh", "Body", "Cabezas", "Cascos", "Escudos", "Armas", "Alas", "Capas", "Fx", "Resource", "Botas" };

    public MainForm()
    {
        InitializeComponent();
        _settings = IndexerSettings.Load();
        _drawTimer.Tick += drawTimer_Tick;
        FormClosed += (_, _) => DisposeBitmapCache();
        ConfigureIcons();
        foreach (var box in new[] { bmpText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText })
            box.DoubleClick += textData_DoubleClick;
        foreach (var box in new[] { bmpText, frameText, framesText, heightText, widthText, speedText, xText, yText, tileHeightText, tileWidthText })
            box.TextChanged += dataText_TextChanged;
        indsText.Text = _settings.InitPath;
        graphicsText.Text = _settings.GraphicsPath;
        formatCombo.SelectedIndex = 0;
        LoadConfiguredGraphics();
        UpdateUi();
    }
}

#pragma warning restore CS8618

