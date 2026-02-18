using Microsoft.JSInterop;
using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

/// <summary>
/// Service for managing viewer themes, accent colors, and applying styles to the DOM.
/// </summary>
public class ThemeService
{
    private ViewerTheme _currentTheme = ViewerTheme.Dark;
    private string _lightAccentColor = "#0969da";
    private string _darkAccentColor = "#4da3ff";
    private string? _selectionColor;
    private string? _hoverColor;
    private string _lightBackgroundColor = "#f0f0f0";
    private string _darkBackgroundColor = "#1a1a2e";
    private double[] _lightGridColor = new[] { 0.0, 0.0, 0.0, 1.0 };
    private double[] _darkGridColor = new[] { 1.0, 1.0, 1.0, 1.0 };
    
    /// <summary>
    /// Event fired when theme or colors change.
    /// </summary>
    public event Action? OnThemeChanged;
    
    /// <summary>
    /// Gets or sets the current theme.
    /// </summary>
    public ViewerTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnThemeChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Gets or sets the accent color for light theme.
    /// </summary>
    public string LightAccentColor
    {
        get => _lightAccentColor;
        set
        {
            if (_lightAccentColor != value)
            {
                _lightAccentColor = value;
                OnThemeChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Gets or sets the accent color for dark theme.
    /// </summary>
    public string DarkAccentColor
    {
        get => _darkAccentColor;
        set
        {
            if (_darkAccentColor != value)
            {
                _darkAccentColor = value;
                OnThemeChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Gets the accent color for the current theme.
    /// </summary>
    public string CurrentAccentColor => CurrentTheme == ViewerTheme.Light ? LightAccentColor : DarkAccentColor;
    
    /// <summary>
    /// Gets or sets the viewer background color for light theme.
    /// </summary>
    public string LightBackgroundColor
    {
        get => _lightBackgroundColor;
        set => _lightBackgroundColor = value;
    }
    
    /// <summary>
    /// Gets or sets the viewer background color for dark theme.
    /// </summary>
    public string DarkBackgroundColor
    {
        get => _darkBackgroundColor;
        set => _darkBackgroundColor = value;
    }
    
    /// <summary>
    /// Gets the background color for the current theme.
    /// </summary>
    public string CurrentBackgroundColor => CurrentTheme == ViewerTheme.Light ? LightBackgroundColor : DarkBackgroundColor;
    
    /// <summary>
    /// Gets or sets the grid color for light theme (RGBA, 0.0-1.0).
    /// </summary>
    public double[] LightGridColor
    {
        get => _lightGridColor;
        set => _lightGridColor = value;
    }
    
    /// <summary>
    /// Gets or sets the grid color for dark theme (RGBA, 0.0-1.0).
    /// </summary>
    public double[] DarkGridColor
    {
        get => _darkGridColor;
        set => _darkGridColor = value;
    }
    
    /// <summary>
    /// Gets the grid color for the current theme (RGBA, 0.0-1.0).
    /// </summary>
    public double[] CurrentGridColor => CurrentTheme == ViewerTheme.Light ? LightGridColor : DarkGridColor;
    
    /// <summary>
    /// Selection (highlighting) color. If not set, defaults to the current accent color.
    /// </summary>
    public string? SelectionColor
    {
        get => _selectionColor;
        set
        {
            if (_selectionColor != value)
            {
                _selectionColor = value;
                OnThemeChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Hover color. If not set, defaults to a lighter version of the current accent color.
    /// </summary>
    public string? HoverColor
    {
        get => _hoverColor;
        set
        {
            if (_hoverColor != value)
            {
                _hoverColor = value;
                OnThemeChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Gets the effective selection color (uses SelectionColor if set, otherwise CurrentAccentColor).
    /// </summary>
    public string EffectiveSelectionColor => SelectionColor ?? CurrentAccentColor;
    
    /// <summary>
    /// Gets the effective hover color (uses HoverColor if set, otherwise a lighter version of CurrentAccentColor).
    /// </summary>
    public string EffectiveHoverColor => HoverColor ?? Lighten(CurrentAccentColor, 0.2);
    
    /// <summary>
    /// Sets the current theme.
    /// </summary>
    public void SetTheme(ViewerTheme theme)
    {
        CurrentTheme = theme;
    }
    
    /// <summary>
    /// Toggles between light and dark theme.
    /// </summary>
    public void ToggleTheme()
    {
        CurrentTheme = CurrentTheme == ViewerTheme.Light ? ViewerTheme.Dark : ViewerTheme.Light;
    }
    
    /// <summary>
    /// Gets the CSS class name for the current theme.
    /// </summary>
    public string GetThemeClass()
    {
        return CurrentTheme == ViewerTheme.Dark ? "theme-dark" : "theme-light";
    }
    
    /// <summary>
    /// Sets the accent colors for both themes.
    /// </summary>
    public void SetAccentColors(string? lightColor = null, string? darkColor = null)
    {
        if (lightColor != null) _lightAccentColor = lightColor;
        if (darkColor != null) _darkAccentColor = darkColor;
        OnThemeChanged?.Invoke();
    }
    
    /// <summary>
    /// Sets the background colors for both themes.
    /// </summary>
    public void SetBackgroundColors(string? lightColor = null, string? darkColor = null)
    {
        if (lightColor != null) _lightBackgroundColor = lightColor;
        if (darkColor != null) _darkBackgroundColor = darkColor;
    }
    
    /// <summary>
    /// Sets the grid colors for both themes (RGBA, 0.0-1.0).
    /// </summary>
    public void SetGridColors(double[]? lightColor = null, double[]? darkColor = null)
    {
        if (lightColor != null) _lightGridColor = lightColor;
        if (darkColor != null) _darkGridColor = darkColor;
    }
    
    /// <summary>
    /// Sets the selection and hover colors.
    /// </summary>
    public void SetSelectionAndHoverColors(string? selectionColor = null, string? hoverColor = null)
    {
        if (selectionColor != null) _selectionColor = selectionColor;
        if (hoverColor != null) _hoverColor = hoverColor;
        OnThemeChanged?.Invoke();
    }
    
    /// <summary>
    /// Applies the current theme to the DOM (body class and CSS variables).
    /// </summary>
    public async Task ApplyAsync(IJSRuntime jsRuntime)
    {
        var themeClass = GetThemeClass();
        var accentColor = CurrentAccentColor;
        var hoverColor = Lighten(accentColor, 0.15);
        var bgColor = WithAlpha(accentColor, 0.2);
        
        var script = $@"
            document.body.className = '{themeClass}';
            document.body.style.setProperty('--Xbim-accent-primary', '{accentColor}', 'important');
            document.body.style.setProperty('--Xbim-accent-primary-hover', '{hoverColor}', 'important');
            document.body.style.setProperty('--Xbim-accent-primary-bg', '{bgColor}', 'important');
            document.body.style.setProperty('--Xbim-border-hover', '{accentColor}', 'important');
        ";
        
        await jsRuntime.InvokeVoidAsync("eval", script);
    }
    
    /// <summary>
    /// Applies the current theme to an XbimViewer.
    /// </summary>
    public async Task ApplyToViewerAsync(Components.XbimViewer viewer, GridPlugin? gridPlugin = null)
    {
        await viewer.SetBackgroundColorAsync(CurrentBackgroundColor);
        await viewer.SetHighlightingColorAsync(EffectiveSelectionColor);
        await viewer.SetHoverPickColorAsync(EffectiveHoverColor);
        
        if (gridPlugin != null)
        {
            gridPlugin.Color = CurrentGridColor;
        }
    }
    
    /// <summary>
    /// Lightens a hex color by the specified amount (0.0 to 1.0).
    /// </summary>
    public static string Lighten(string hex, double amount)
    {
        hex = hex.TrimStart('#');
        if (hex.Length != 6) return $"#{hex}";
        
        var r = Convert.ToInt32(hex.Substring(0, 2), 16);
        var g = Convert.ToInt32(hex.Substring(2, 2), 16);
        var b = Convert.ToInt32(hex.Substring(4, 2), 16);
        
        r = Math.Min(255, (int)(r + (255 - r) * amount));
        g = Math.Min(255, (int)(g + (255 - g) * amount));
        b = Math.Min(255, (int)(b + (255 - b) * amount));
        
        return $"#{r:X2}{g:X2}{b:X2}";
    }
    
    /// <summary>
    /// Darkens a hex color by the specified amount (0.0 to 1.0).
    /// </summary>
    public static string Darken(string hex, double amount)
    {
        hex = hex.TrimStart('#');
        if (hex.Length != 6) return $"#{hex}";
        
        var r = Convert.ToInt32(hex.Substring(0, 2), 16);
        var g = Convert.ToInt32(hex.Substring(2, 2), 16);
        var b = Convert.ToInt32(hex.Substring(4, 2), 16);
        
        r = Math.Max(0, (int)(r * (1 - amount)));
        g = Math.Max(0, (int)(g * (1 - amount)));
        b = Math.Max(0, (int)(b * (1 - amount)));
        
        return $"#{r:X2}{g:X2}{b:X2}";
    }
    
    /// <summary>
    /// Converts a hex color to rgba with the specified alpha (0.0 to 1.0).
    /// </summary>
    public static string WithAlpha(string hex, double alpha)
    {
        hex = hex.TrimStart('#');
        if (hex.Length != 6) return $"rgba(0, 0, 0, {alpha})";
        
        var r = Convert.ToInt32(hex.Substring(0, 2), 16);
        var g = Convert.ToInt32(hex.Substring(2, 2), 16);
        var b = Convert.ToInt32(hex.Substring(4, 2), 16);
        
        return $"rgba({r}, {g}, {b}, {alpha})";
    }
    
    /// <summary>
    /// Converts a hex color to RGB tuple.
    /// </summary>
    public static (int R, int G, int B) HexToRgb(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length != 6) return (0, 0, 0);
        
        return (
            Convert.ToInt32(hex.Substring(0, 2), 16),
            Convert.ToInt32(hex.Substring(2, 2), 16),
            Convert.ToInt32(hex.Substring(4, 2), 16)
        );
    }
    
    /// <summary>
    /// Converts RGB values to hex string.
    /// </summary>
    public static string RgbToHex(int r, int g, int b)
    {
        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}
