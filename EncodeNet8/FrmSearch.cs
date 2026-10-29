namespace EncodeNet8;

public enum SearchIndexMode
{
    Body,
    Animation,
    IndependentGraphic
}

public sealed class SearchResultItem
{
    public required string DisplayText { get; init; }
    public required int GrhId { get; init; }
    public bool IsResource { get; init; }
    public int? SecondaryGrhId { get; init; }

    public override string ToString() => DisplayText;
}

public partial class FrmSearch : Form
{
    private readonly Action<int, bool> _selectResult;
    private readonly Action<SearchIndexMode, int> _indexResult;
    private bool _cancelRequested;

    public bool CancelRequested => _cancelRequested;

    public FrmSearch(
        IEnumerable<SearchResultItem> results,
        string progressText,
        Action<int, bool> selectResult,
        Action<SearchIndexMode, int> indexResult)
    {
        InitializeComponent();
        _selectResult = selectResult;
        _indexResult = indexResult;
        SetResults(results, progressText);
    }

    public void SetResults(IEnumerable<SearchResultItem> results, string progressText)
    {
        resultsList.BeginUpdate();
        try
        {
            resultsList.Items.Clear();
            foreach (var result in results)
                resultsList.Items.Add(result);
        }
        finally
        {
            resultsList.EndUpdate();
        }

        progressLabel.Text = progressText;
        if (resultsList.Items.Count > 0)
            resultsList.SelectedIndex = 0;
    }

    private SearchResultItem? SelectedResult => resultsList.SelectedItem as SearchResultItem;

    private void resultsList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (SelectedResult is { } result)
            _selectResult(result.GrhId, result.IsResource);
    }

    private void resultsList_DoubleClick(object? sender, EventArgs e) =>
        resultsList_SelectedIndexChanged(sender, e);

    private void optionsMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = SelectedResult is null || SelectedResult.IsResource;
    }

    private void indexAsBodyMenu_Click(object? sender, EventArgs e) => IndexSelected(SearchIndexMode.Body);
    private void indexAsAnimationMenu_Click(object? sender, EventArgs e) => IndexSelected(SearchIndexMode.Animation);
    private void indexAsGraphicMenu_Click(object? sender, EventArgs e) => IndexSelected(SearchIndexMode.IndependentGraphic);

    private void IndexSelected(SearchIndexMode mode)
    {
        if (SelectedResult is not { } result || result.IsResource) return;
        _indexResult(mode, result.GrhId);
    }

    private void FrmSearch_FormClosing(object? sender, FormClosingEventArgs e) => _cancelRequested = true;

    public override string ToString() => SelectedResult?.DisplayText ?? string.Empty;
}
