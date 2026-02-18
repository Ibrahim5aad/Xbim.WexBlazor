namespace Xbim.WexBlazor.Models;

public enum SidebarPosition
{
    Left,
    Right
}

public class SidebarPanelInfo
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Icon { get; set; } = "bi-square";
    public string Title { get; set; } = "Panel";
    public int Width { get; set; } = 320;
    public bool IsOpen { get; set; } = false;
    public Action? OnToggle { get; set; }
}
