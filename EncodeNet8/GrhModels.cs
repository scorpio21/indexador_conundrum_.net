namespace EncodeNet8;

public sealed class GrhRecord
{
    public int Id { get; set; }
    public short FileNumber { get; set; }
    public short X { get; set; }
    public short Y { get; set; }
    public short Width { get; set; }
    public short Height { get; set; }
    public short Speed { get; set; }
    public List<int> Frames { get; set; } = new();
    public bool IsAnimation => Frames.Count > 1;
}

public enum GrhStorageFormat
{
    LegacyInteger,
    LongIndexes
}

public sealed class BodyIndexRecord
{
    public int Id { get; init; }
    public int Walk1 { get; init; }
    public int Walk2 { get; init; }
    public int Walk3 { get; init; }
    public int Walk4 { get; init; }
    public short HeadOffsetX { get; init; }
    public short HeadOffsetY { get; init; }
}

public sealed class DirectionalIndexRecord
{
    public int Id { get; set; }
    public int[] Directions { get; set; } = new int[4];
    public short OffsetX { get; set; }
    public short OffsetY { get; set; }
}

public sealed class FxIndexRecord
{
    public int Id { get; set; }
    public int Animation { get; set; }
    public short OffsetX { get; set; }
    public short OffsetY { get; set; }
}
