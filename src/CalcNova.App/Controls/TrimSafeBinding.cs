using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

namespace CalcNova.App.Controls;

/// <summary>
/// Builds the same compiled bindings the XAML compiler emits, but from code-behind.
/// </summary>
/// <remarks>
/// <para>
/// <c>new Binding("SomeProperty")</c> looks its property up reflectively against whatever
/// the data context turns out to be. The trimmer cannot follow that lookup, so every such
/// binding is an IL2026 site and the WebAssembly head — the only head published with
/// trimming enabled — refused to publish at all.
/// </para>
/// <para>
/// A compiled binding carries an explicit getter and setter instead of a property name to
/// resolve, so nothing has to be discovered at run time and nothing the binding needs can
/// be trimmed away. The property name is still supplied because the INotifyPropertyChanged
/// accessor matches change notifications against it.
/// </para>
/// </remarks>
internal static class TrimSafeBinding
{
    /// <summary>Binds a target property to a source property that the target never writes back.</summary>
    public static CompiledBinding OneWay<TSource, TValue>(
        string propertyName,
        Func<TSource, TValue> getter,
        string? stringFormat = null)
        where TSource : class =>
        Create(propertyName, getter, setter: null, BindingMode.OneWay, stringFormat);

    /// <summary>Binds a target property to a source property that the target also writes back.</summary>
    public static CompiledBinding TwoWay<TSource, TValue>(
        string propertyName,
        Func<TSource, TValue> getter,
        Action<TSource, TValue> setter)
        where TSource : class =>
        Create(propertyName, getter, setter, BindingMode.TwoWay, stringFormat: null);

    private static CompiledBinding Create<TSource, TValue>(
        string propertyName,
        Func<TSource, TValue> getter,
        Action<TSource, TValue>? setter,
        BindingMode mode,
        string? stringFormat)
        where TSource : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(getter);

        // Reference types and Nullable<T> can legitimately be assigned null from the target
        // side; a non-nullable value type cannot, and Avalonia hands Set a null when a target
        // value fails to convert (an emptied NumericUpDown, say). Dropping that write leaves
        // the source property at its last good value instead of resetting it to default.
        var acceptsNull = default(TValue) is null;

        var propertyInfo = new ClrPropertyInfo(
            propertyName,
            target => getter((TSource)target),
            setter is null
                ? null
                : (target, value) =>
                {
                    if (value is TValue typedValue)
                    {
                        setter((TSource)target, typedValue);
                    }
                    else if (value is null && acceptsNull)
                    {
                        setter((TSource)target, default!);
                    }
                },
            typeof(TValue));

        var path = new CompiledBindingPathBuilder()
            .Property(propertyInfo, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Build();

        return new CompiledBinding(path)
        {
            Mode = mode,
            StringFormat = stringFormat
        };
    }
}
