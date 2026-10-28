using System.Text.RegularExpressions;

namespace EncodeNet8;

/// <summary>Escritura compatible con el Indices.ini que genera Encode.frm.</summary>
public static class WeIndexFile
{
    private static readonly Regex ReferenceSection = new(@"^\s*\[REFERENCIA(?<id>\d+)\]\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static int Append(string path, string name, int grhIndex, int height, int width)
    {
        var lines = File.Exists(path)
            ? File.ReadAllLines(path).ToList()
            : new List<string>();

        var maxReference = lines
            .Select(line => ReferenceSection.Match(line))
            .Where(match => match.Success)
            .Select(match => int.TryParse(match.Groups["id"].Value, out var id) ? id : 0)
            .DefaultIfEmpty(0)
            .Max();
        var reference = maxReference + 1;
        var initIndex = lines.FindIndex(line => line.Trim().Equals("[INIT]", StringComparison.OrdinalIgnoreCase));
        var referencesLine = -1;
        if (initIndex >= 0)
        {
            for (var index = initIndex + 1; index < lines.Count && !lines[index].TrimStart().StartsWith("[", StringComparison.Ordinal); index++)
            {
                if (lines[index].TrimStart().StartsWith("Referencias=", StringComparison.OrdinalIgnoreCase)
                    || lines[index].TrimStart().StartsWith("Referencias ", StringComparison.OrdinalIgnoreCase))
                {
                    referencesLine = index;
                    break;
                }
            }
        }

        var total = Math.Max(reference, ReadReferenceCount(lines));
        if (initIndex < 0)
        {
            if (lines.Count > 0 && lines[^1].Length > 0) lines.Add(string.Empty);
            lines.Add("[INIT]");
            lines.Add($"Referencias={total}");
        }
        else if (referencesLine >= 0)
        {
            lines[referencesLine] = $"Referencias={total}";
        }
        else
        {
            lines.Insert(initIndex + 1, $"Referencias={total}");
        }

        if (lines.Count > 0 && lines[^1].Length > 0) lines.Add(string.Empty);
        lines.Add($"[REFERENCIA{reference}]");
        lines.Add($"Nombre={name}");
        lines.Add($"GrhIndice={grhIndex}");
        lines.Add($"Alto={height}");
        lines.Add($"Ancho={width}");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllLines(path, lines);
        return reference;
    }

    private static int ReadReferenceCount(IReadOnlyList<string> lines)
    {
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith("Referencias", StringComparison.OrdinalIgnoreCase)) continue;
            var separator = trimmed.IndexOf('=');
            if (separator >= 0 && int.TryParse(trimmed[(separator + 1)..].Trim(), out var count)) return count;
        }
        return 0;
    }
}
