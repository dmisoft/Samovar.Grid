namespace Samovar.Grid;

/// <summary>
/// Configuration options for the SamovarGrid component library.
/// </summary>
public sealed class SamovarGridOptions
{
    /// <summary>
    /// When <see langword="true"/>, the <see cref="SamovarGridStyles"/> component will render
    /// the library's CSS link tag into the document head.
    /// Defaults to <see langword="false"/> to preserve backward compatibility with
    /// apps that link the stylesheet manually in App.razor.
    /// </summary>
    public bool InjectCss { get; set; } = false;
}
