namespace CalcNova.Scientific;

public static class ScientificFunctionCatalog
{
    public static IReadOnlyList<string> Functions { get; } =
    [
        "sqrt", "cbrt", "abs", "sqr", "cube", "pow", "root", "reciprocal",
        "ln", "log", "log10", "log2", "exp",
        "sin", "cos", "tan", "asin", "acos", "atan",
        "sinh", "cosh", "tanh", "asinh", "acosh", "atanh",
        "floor", "ceil", "round", "trunc", "sign", "min", "max",
        "factorial", "gcd", "lcm", "comb", "perm", "mod", "percent"
    ];
}
