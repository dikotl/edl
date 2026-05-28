namespace Edl.Core.Parsing;

public class ParseException(
    string message,
    string filename,
    uint pos,
    uint line,
    uint column
) : Exception($"{filename}:{line}:{column}: {message}")
{
    public string Filename { get; init; } = filename;
    public uint Pos { get; init; } = pos;
    public uint Line { get; init; } = line;
    public uint Column { get; init; } = column;
}
