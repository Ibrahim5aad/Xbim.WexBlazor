namespace Xbim.WexBlazor.Models;

/// <summary>
/// Constants for xBIM Viewer enumerations
/// </summary>
public static class ViewerConstants
{
    /// <summary>
    /// Camera type values
    /// </summary>
    public static class CameraType
    {
        public const int Perspective = 0;
        public const int Orthogonal = 1;
    }

    /// <summary>
    /// View type values for preset camera angles
    /// </summary>
    public static class ViewType
    {
        public const int Top = 0;
        public const int Bottom = 1;
        public const int Front = 2;
        public const int Back = 3;
        public const int Left = 4;
        public const int Right = 5;
        public const int Default = 6;
    }

    /// <summary>
    /// Rendering mode values
    /// </summary>
    public static class RenderingMode
    {
        public const int Normal = 0;
        public const int Grayscale = 1;
        public const int XRay = 2;
        public const int XRayUltra = 4;
    }

    /// <summary>
    /// State values for product visibility and highlighting
    /// </summary>
    public static class State
    {
        public const int Undefined = 255;
        public const int Hidden = 254;
        public const int Highlighted = 253;
        public const int XRayVisible = 252;
        public const int PickingOnly = 251;
        public const int HoverOver = 250;
        public const int Unstyled = 225;
    }

    /// <summary>
    /// Navigation mode values for viewer interaction
    /// </summary>
    public static class NavigationMode
    {
        public const string Orbit = "orbit";
        public const string FreeOrbit = "free-orbit";
        public const string Pan = "pan";
        public const string Zoom = "zoom";
        public const string LookAround = "look-around";
        public const string Walk = "walk";
        public const string None = "none";
    }
}

