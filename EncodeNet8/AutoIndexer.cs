namespace EncodeNet8;

public static class AutoIndexer
{
    public static List<GrhRecord> CreateGrid(AutoIndexerOptions options, int maxGrh)
    {
        using var bitmap = Image.FromFile(options.BitmapPath);
        var requiredWidth = checked(options.Columns * options.CellWidth);
        var requiredHeight = checked(options.Rows * options.CellHeight);
        if (requiredWidth > bitmap.Width || requiredHeight > bitmap.Height)
            throw new InvalidDataException($"La cuadrícula {requiredWidth}x{requiredHeight} no cabe en el BMP ({bitmap.Width}x{bitmap.Height}).");

        var total = checked(options.Columns * options.Rows);
        var animationId = checked(options.StartGrh + total);
        if (animationId > maxGrh) throw new InvalidDataException($"El último GRH calculado ({animationId}) supera el máximo configurado ({maxGrh}).");

        var result = new List<GrhRecord>(total + (options.CreateAnimation ? 1 : 0));
        for (var row = 0; row < options.Rows; row++)
        for (var column = 0; column < options.Columns; column++)
        {
            var id = checked(options.StartGrh + row * options.Columns + column);
            result.Add(new GrhRecord
            {
                Id = id,
                FileNumber = checked((short)ParseBmpNumber(options.BitmapPath)),
                X = checked((short)(column * options.CellWidth)),
                Y = checked((short)(row * options.CellHeight)),
                Width = checked((short)options.CellWidth),
                Height = checked((short)options.CellHeight),
                Frames = new List<int> { id }
            });
        }

        if (options.CreateAnimation)
            result.Add(new GrhRecord { Id = animationId, Speed = checked((short)options.AnimationSpeed), Frames = result.Select(record => record.Id).ToList(), FileNumber = checked((short)ParseBmpNumber(options.BitmapPath)), Width = checked((short)options.CellWidth), Height = checked((short)options.CellHeight) });
        return result;
    }

    private static int ParseBmpNumber(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        if (!int.TryParse(name, out var number) || number <= 0 || number > short.MaxValue)
            throw new InvalidDataException($"El nombre del BMP debe ser un número entre 1 y {short.MaxValue}.");
        return number;
    }
}
