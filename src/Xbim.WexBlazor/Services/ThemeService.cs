using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

public class ThemeService
{
    private ViewerTheme _currentTheme = ViewerTheme.Dark;
    
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
}
