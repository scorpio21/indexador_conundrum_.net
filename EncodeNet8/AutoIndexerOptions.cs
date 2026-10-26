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
