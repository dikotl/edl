namespace Edl.Core.Parsing;

public readonly record struct LineInfo(string Filename, uint Line, uint Column)
{
    public string Prefix => $"{this}: ";
    public override string ToString() => $"{Filename}:{Line}:{Column}";
}
