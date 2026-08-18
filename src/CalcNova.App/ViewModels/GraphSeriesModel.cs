using CalcNova.Graphing;

namespace CalcNova.App.ViewModels;

public sealed record GraphSeriesModel(
    string Identifier,
    string Expression,
    IReadOnlyList<GraphSegment> Segments);
