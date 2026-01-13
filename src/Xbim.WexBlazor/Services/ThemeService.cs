using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

public class ThemeService
{
    private ViewerTheme _currentTheme = ViewerTheme.Dark;
    private string _lightAccentColor = "#0969da";
    private string _darkAccentColor = "#4da3ff";
    
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
}
