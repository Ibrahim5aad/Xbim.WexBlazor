using Microsoft.AspNetCore.Components;
using Xbim.WexBlazor.Models;
using static Xbim.WexBlazor.Models.ViewerConstants;

namespace Xbim.WexBlazor.Components.BuiltInButtons;

/// <summary>
/// Factory for creating built-in toolbar buttons
/// </summary>
public static class ViewerBuiltInButtons
{
    /// <summary>
    /// Creates a perspective toggle button (Orthogonal/Perspective camera)
    /// </summary>
    public static ToolbarToggleButton CreatePerspectiveToggle(
        XbimViewerComponent viewer,
        bool startWithPerspective = true)
    {
        return new ToolbarToggleButton
        {
            Icon = "bi bi-box",
            ToggledIcon = "bi bi-grid-3x3",
            Tooltip = "Orthogonal View",
            ToggledTooltip = "Perspective View",
            IsToggled = startWithPerspective,
            OnToggle = EventCallback.Factory.Create<bool>(viewer, async (isPerspective) =>
            {
                var cameraType = isPerspective ? CameraType.Perspective : CameraType.Orthogonal;
                await viewer.CallViewerMethodAsync<object>("set", new { camera = cameraType });
            })
        };
    }

    /// <summary>
    /// Creates a zoom to fit button
    /// </summary>
    public static ToolbarButton CreateZoomFitButton(XbimViewerComponent viewer)
    {
        return new ToolbarButton
        {
            Icon = "bi bi-arrows-angle-expand",
            Tooltip = "Zoom to Fit",
            OnClick = EventCallback.Factory.Create(viewer, async () =>
            {
                await viewer.ZoomFitAsync();
            })
        };
    }

    /// <summary>
    /// Creates a reset view button
    /// </summary>
    public static ToolbarButton CreateResetViewButton(XbimViewerComponent viewer)
    {
        return new ToolbarButton
        {
            Icon = "bi bi-arrow-clockwise",
            Tooltip = "Reset View",
            OnClick = EventCallback.Factory.Create(viewer, async () =>
            {
                await viewer.ResetAsync();
            })
        };
    }

    /// <summary>
    /// Creates a clear selection button
    /// </summary>
    public static ToolbarButton CreateClearSelectionButton(XbimViewerComponent viewer)
    {
        return new ToolbarButton
        {
            Icon = "bi bi-x-circle",
            Tooltip = "Clear Selection",
            OnClick = EventCallback.Factory.Create(viewer, async () =>
            {
                await viewer.ClearSelectionAsync();
            })
        };
    }

    /// <summary>
    /// Creates a view dropdown with preset camera angles
    /// </summary>
    public static ToolbarDropdown CreateViewsDropdown(XbimViewerComponent viewer)
    {
        return new ToolbarDropdown
        {
            Icon = "bi bi-eye",
            Tooltip = "Preset Views",
            Items = new List<ToolbarButton>
            {
                new ToolbarButton
                {
                    Icon = "bi bi-arrow-up",
                    Tooltip = "Top View",
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("show", ViewType.Top);
                    })
                },
                new ToolbarButton
                {
                    Icon = "bi bi-arrow-down",
                    Tooltip = "Bottom View",
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("show", ViewType.Bottom);
                    })
                },
                new ToolbarButton
                {
                    Icon = "bi bi-arrow-left",
                    Tooltip = "Left View",
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("show", ViewType.Left);
                    })
                },
                new ToolbarButton
                {
                    Icon = "bi bi-arrow-right",
                    Tooltip = "Right View",
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("show", ViewType.Right);
                    })
                },
                new ToolbarButton
                {
                    Icon = "bi bi-arrow-bar-up",
                    Tooltip = "Front View",
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("show", ViewType.Front);
                    })
                },
                new ToolbarButton
                {
                    Icon = "bi bi-arrow-bar-down",
                    Tooltip = "Back View",
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("show", ViewType.Back);
                    })
                }
            }
        };
    }

    /// <summary>
    /// Creates an X-Ray mode toggle button
    /// </summary>
    public static ToolbarToggleButton CreateXRayToggle(XbimViewerComponent viewer)
    {
        return new ToolbarToggleButton
        {
            Icon = "bi bi-transparency",
            Tooltip = "X-Ray Mode Off",
            ToggledTooltip = "X-Ray Mode On",
            IsToggled = false,
            OnToggle = EventCallback.Factory.Create<bool>(viewer, async (isXRay) =>
            {
                var renderingMode = isXRay ? RenderingMode.XRay : RenderingMode.Normal;
                await viewer.CallViewerMethodAsync<object>("set", new { renderingMode = renderingMode });
            })
        };
    }

    /// <summary>
    /// Creates a button to open the file loader
    /// </summary>
    public static ToolbarButton CreateLoadModelButton(EventCallback onClick)
    {
        return new ToolbarButton
        {
            Icon = "bi bi-folder2-open",
            Tooltip = "Load Model",
            OnClick = onClick
        };
    }

    /// <summary>
    /// Creates a visually grouped set of two toggle buttons for clipping plane control
    /// </summary>
    /// <param name="viewer">The viewer component</param>
    /// <param name="getPlugin">Function to get the ClippingPlanePlugin instance</param>
    /// <param name="label">Optional label for the button group</param>
    /// <param name="onStateChanged">Optional callback to trigger UI update when button state changes</param>
    /// <returns>A toolbar button group for clipping plane control</returns>
    public static ToolbarButtonGroup CreateClippingPlaneButtons(
        XbimViewerComponent viewer,
        Func<ClippingPlanePlugin?> getPlugin,
        string? label = "Clipping",
        Action? onStateChanged = null)
    {
        var visibilityToggle = new ToolbarToggleButton
        {
            Icon = "bi bi-eye",
            ToggledIcon = "bi bi-eye-slash",
            Tooltip = "Show Clipping Control",
            ToggledTooltip = "Hide Clipping Control",
            IsToggled = true,
            Disabled = true,
            OnToggle = EventCallback.Factory.Create<bool>(viewer, async (isVisible) =>
            {
                var plugin = getPlugin();
                if (plugin != null)
                {
                    await viewer.SetPluginStoppedAsync(plugin.Id, !isVisible);
                }
            })
        };

        var enableToggle = new ToolbarToggleButton
        {
            Icon = "bi bi-scissors",
            ToggledIcon = "bi bi-x-octagon",
            Tooltip = "Enable Clipping Plane",
            ToggledTooltip = "Disable Clipping Plane",
            IsToggled = false,
            OnToggle = EventCallback.Factory.Create<bool>(viewer, async (isEnabled) =>
            {
                var plugin = getPlugin();
                if (plugin == null) return;

                visibilityToggle.Disabled = !isEnabled;
                onStateChanged?.Invoke();

                if (isEnabled)
                {
                    await viewer.SetPluginStoppedAsync(plugin.Id, false);
                }
                else
                {
                    await viewer.UnclipAsync();
                    await viewer.SetPluginStoppedAsync(plugin.Id, true);
                }
            })
        };

        return new ToolbarButtonGroup
        {
            Label = label,
            Tooltip = "Clipping Plane Controls",
            Items = new List<ToolbarItemBase>
            {
                enableToggle,
                visibilityToggle
            }
        };
    }

    /// <summary>
    /// Creates a visually grouped set of two toggle buttons for section box control
    /// </summary>
    /// <param name="viewer">The viewer component</param>
    /// <param name="getPlugin">Function to get the SectionBoxPlugin instance</param>
    /// <param name="label">Optional label for the button group</param>
    /// <param name="onStateChanged">Optional callback to trigger UI update when button state changes</param>
    /// <returns>A toolbar button group for section box control</returns>
    public static ToolbarButtonGroup CreateSectionBoxButtons(
        XbimViewerComponent viewer,
        Func<SectionBoxPlugin?> getPlugin,
        string? label = "Section Box",
        Action? onStateChanged = null)
    {
        var visibilityToggle = new ToolbarToggleButton
        {
            Icon = "bi bi-eye",
            ToggledIcon = "bi bi-eye-slash",
            Tooltip = "Show Section Box Control",
            ToggledTooltip = "Hide Section Box Control",
            IsToggled = true,
            Disabled = true,
            OnToggle = EventCallback.Factory.Create<bool>(viewer, async (isVisible) =>
            {
                var plugin = getPlugin();
                if (plugin != null)
                {
                    await viewer.SetPluginStoppedAsync(plugin.Id, !isVisible);
                }
            })
        };

        var enableToggle = new ToolbarToggleButton
        {
            Icon = "bi bi-box",
            ToggledIcon = "bi bi-box-arrow-up-right",
            Tooltip = "Enable Section Box",
            ToggledTooltip = "Disable Section Box",
            IsToggled = false,
            OnToggle = EventCallback.Factory.Create<bool>(viewer, async (isEnabled) =>
            {
                var plugin = getPlugin();
                if (plugin == null) return;

                visibilityToggle.Disabled = !isEnabled;
                onStateChanged?.Invoke();

                if (isEnabled)
                {
                    await viewer.CreateSectionBoxAsync(plugin.Id);
                }
                else
                {
                    await viewer.ClearSectionBoxAsync(plugin.Id);
                }
            })
        };

        return new ToolbarButtonGroup
        {
            Label = label,
            Tooltip = "Section Box Controls",
            Items = new List<ToolbarItemBase>
            {
                enableToggle,
                visibilityToggle
            }
        };
    }

    /// <summary>
    /// Creates a navigation controls radio button group where only one navigation mode can be active
    /// </summary>
    public static ToolbarRadioButtonGroup CreateNavigationButtons(
        XbimViewerComponent viewer,
        string? label = "Navigation",
        int defaultSelectedIndex = 0)
    {
        return new ToolbarRadioButtonGroup
        {
            Label = label,
            Tooltip = "Navigation Controls",
            SelectedIndex = defaultSelectedIndex,
            Items = new List<ToolbarRadioButton>
            {
                new ToolbarRadioButton
                {
                    Icon = "bi bi-globe",
                    Tooltip = "Orbit",
                    IsSelected = defaultSelectedIndex == 0,
                    Value = NavigationMode.Orbit,
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("set", new { navigationMode = NavigationMode.Orbit });
                    })
                },
                new ToolbarRadioButton
                {
                    Icon = "bi bi-globe-americas",
                    Tooltip = "Free Orbit",
                    IsSelected = defaultSelectedIndex == 1,
                    Value = NavigationMode.FreeOrbit,
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("set", new { navigationMode = NavigationMode.FreeOrbit });
                    })
                },
                new ToolbarRadioButton
                {
                    Icon = "bi bi-arrows-move",
                    Tooltip = "Pan",
                    IsSelected = defaultSelectedIndex == 2,
                    Value = NavigationMode.Pan,
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("set", new { navigationMode = NavigationMode.Pan });
                    })
                },
                new ToolbarRadioButton
                {
                    Icon = "bi bi-zoom-in",
                    Tooltip = "Zoom",
                    IsSelected = defaultSelectedIndex == 3,
                    Value = NavigationMode.Zoom,
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("set", new { navigationMode = NavigationMode.Zoom });
                    })
                },
                new ToolbarRadioButton
                {
                    Icon = "bi bi-eye",
                    Tooltip = "Look Around",
                    IsSelected = defaultSelectedIndex == 4,
                    Value = NavigationMode.LookAround,
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("set", new { navigationMode = NavigationMode.LookAround });
                    })
                },
                new ToolbarRadioButton
                {
                    Icon = "bi bi-person-walking",
                    Tooltip = "Walk",
                    IsSelected = defaultSelectedIndex == 5,
                    Value = NavigationMode.Walk,
                    OnClick = EventCallback.Factory.Create(viewer, async () =>
                    {
                        await viewer.CallViewerMethodAsync<object>("set", new { navigationMode = NavigationMode.Walk });
                    })
                }
            }
        };
    }
}

