using Microsoft.AspNetCore.Components;

namespace Xbim.WexBlazor.Models;

/// <summary>
/// Base class for toolbar items
/// </summary>
public abstract class ToolbarItemBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string? Icon { get; set; }
    public string? Tooltip { get; set; }
    public bool Disabled { get; set; }
    public string? CssClass { get; set; }
}

/// <summary>
/// Regular toolbar button
/// </summary>
public class ToolbarButton : ToolbarItemBase
{
    public EventCallback OnClick { get; set; }
}

/// <summary>
/// Toggle toolbar button (on/off state)
/// </summary>
public class ToolbarToggleButton : ToolbarItemBase
{
    public bool IsToggled { get; set; }
    public EventCallback<bool> OnToggle { get; set; }
    public string? ToggledIcon { get; set; }
    public string? ToggledTooltip { get; set; }
}

/// <summary>
/// Dropdown toolbar button
/// </summary>
public class ToolbarDropdown : ToolbarItemBase
{
    public string? Label { get; set; }
    public List<ToolbarButton> Items { get; set; } = new();
}

/// <summary>
/// Button group container for visually grouping related buttons together
/// </summary>
public class ToolbarButtonGroup : ToolbarItemBase
{
    public List<ToolbarItemBase> Items { get; set; } = new();
    public string? Label { get; set; }
}

/// <summary>
/// Radio button for use in ToolbarRadioButtonGroup (single selection)
/// </summary>
public class ToolbarRadioButton : ToolbarItemBase
{
    public bool IsSelected { get; set; }
    public EventCallback OnClick { get; set; }
    public object? Value { get; set; }
}

/// <summary>
/// Radio button group where only one button can be selected at a time
/// </summary>
public class ToolbarRadioButtonGroup : ToolbarItemBase
{
    public List<ToolbarRadioButton> Items { get; set; } = new();
    public string? Label { get; set; }
    public int SelectedIndex { get; set; } = 0;
    public EventCallback<int> OnSelectionChanged { get; set; }
}
