using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

public class ThemeService
{
    private ViewerTheme _currentTheme = ViewerTheme.Dark;
    private string _lightAccentColor = "#0969da";
    private string _darkAccentColor = "#4da3ff";
    private string? _selectionColor;
    private string? _hoverColor;
    
    public event Action? OnThemeChanged;
    
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
    
    public string CurrentAccentColor => CurrentTheme == ViewerTheme.Light ? LightAccentColor : DarkAccentColor;
    
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
    /// Gets the effective selection color (uses SelectionColor if set, otherwise CurrentAccentColor)
    /// </summary>
    public string EffectiveSelectionColor => SelectionColor ?? CurrentAccentColor;
    
    /// <summary>
    /// Gets the effective hover color (uses HoverColor if set, otherwise a lighter version of CurrentAccentColor)
    /// </summary>
    public string EffectiveHoverColor => HoverColor ?? LightenColor(CurrentAccentColor, 0.2);
    
    private string LightenColor(string hex, double amount)
    {
        hex = hex.TrimStart('#');
        var r = Convert.ToInt32(hex.Substring(0, 2), 16);
        var g = Convert.ToInt32(hex.Substring(2, 2), 16);
        var b = Convert.ToInt32(hex.Substring(4, 2), 16);
        
        r = Math.Min(255, (int)(r + (255 - r) * amount));
        g = Math.Min(255, (int)(g + (255 - g) * amount));
        b = Math.Min(255, (int)(b + (255 - b) * amount));
        
        return $"#{r:X2}{g:X2}{b:X2}";
    }
    
    public void SetTheme(ViewerTheme theme)
    {
        CurrentTheme = theme;
    }
    
    public void ToggleTheme()
    {
        CurrentTheme = CurrentTheme == ViewerTheme.Light ? ViewerTheme.Dark : ViewerTheme.Light;
    }
    
    public string GetThemeClass()
    {
        return CurrentTheme == ViewerTheme.Dark ? "theme-dark" : "theme-light";
    }
    
    public void SetAccentColors(string? lightColor = null, string? darkColor = null)
    {
        if (lightColor != null) _lightAccentColor = lightColor;
        if (darkColor != null) _darkAccentColor = darkColor;
        OnThemeChanged?.Invoke();
    }
    
    /// <summary>
    /// Sets the selection and hover colors
    /// </summary>
    /// <param name="selectionColor">Color for selected/highlighted elements. If null, uses accent color.</param>
    /// <param name="hoverColor">Color for hovered elements. If null, uses a lighter version of accent color.</param>
    public void SetSelectionAndHoverColors(string? selectionColor = null, string? hoverColor = null)
    {
        if (selectionColor != null) _selectionColor = selectionColor;
        if (hoverColor != null) _hoverColor = hoverColor;
        OnThemeChanged?.Invoke();
    }
}
