using UnityEngine;

namespace DodgeBLTZ
{
    /// <summary>
    /// Centralized color palette management for Dodge BLTZ UI
    /// TODO: Update colors based on final design approval (Issue #001)
    /// </summary>
    public static class UIColorPalette
    {
        // Current colors (Orange/Blue/Purple theme)
        // TODO: Replace with approved "Retro Black/Neon Yellow/Arcade" theme
        
        // Primary Colors
        public static readonly Color Primary = HexToColor("#FF6B35");        // Vibrant Orange
        public static readonly Color Secondary = HexToColor("#004E89");      // Deep Blue
        public static readonly Color Background = HexToColor("#1A1A2E");     // Dark Purple
        public static readonly Color Surface = HexToColor("#16213E");        // Navy
        
        // Semantic Colors
        public static readonly Color Success = HexToColor("#2ECC71");        // Green
        public static readonly Color Error = HexToColor("#E74C3C");          // Red
        public static readonly Color Warning = HexToColor("#F39C12");        // Orange
        public static readonly Color Info = HexToColor("#3498DB");           // Blue
        
        // Text Colors
        public static readonly Color TextPrimary = HexToColor("#FFFFFF");    // White
        public static readonly Color TextSecondary = HexToColor("#BDC3C7");  // Light Gray
        public static readonly Color TextDisabled = HexToColor("#7F8C8D");   // Gray
        
        // Interactive Colors
        public static readonly Color ButtonPrimary = Primary;
        public static readonly Color ButtonSecondary = Secondary;
        public static readonly Color ButtonDisabled = HexToColor("#95A5A6"); // Light Gray
        
        // State Colors (for wallet connection, game states, etc.)
        public static readonly Color Connected = Success;
        public static readonly Color Disconnected = Error;
        public static readonly Color Processing = Warning;
        
        // Proposed "Retro/Arcade" theme (awaiting design approval)
        // TODO: Uncomment and adjust based on final color specifications
        /*
        public static readonly Color RetroBlack = HexToColor("#000000");      // TBD: Exact hex
        public static readonly Color NeonYellow = HexToColor("#FFFF00");      // TBD: Exact hex  
        public static readonly Color ArcadeBlue = HexToColor("#0080FF");      // TBD: Exact hex
        public static readonly Color CheeseOrange = HexToColor("#FFA500");    // TBD: Exact hex
        public static readonly Color FreshGreen = HexToColor("#00FF80");      // TBD: Exact hex
        */
        
        /// <summary>
        /// Converts hex string to Unity Color
        /// </summary>
        /// <param name="hex">Hex color string (e.g., "#FF6B35")</param>
        /// <returns>Unity Color object</returns>
        public static Color HexToColor(string hex)
        {
            // Remove # if present
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            
            // Ensure 6 characters
            if (hex.Length != 6)
            {
                Debug.LogError($"Invalid hex color: {hex}. Expected 6 characters.");
                return Color.magenta; // Error color
            }
            
            try
            {
                byte r = System.Convert.ToByte(hex.Substring(0, 2), 16);
                byte g = System.Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = System.Convert.ToByte(hex.Substring(4, 2), 16);
                
                return new Color(r / 255f, g / 255f, b / 255f, 1f);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to parse hex color {hex}: {e.Message}");
                return Color.magenta; // Error color
            }
        }
        
        /// <summary>
        /// Validates color contrast ratio for accessibility
        /// </summary>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        /// <returns>True if contrast ratio meets WCAG 2.1 AA standards (4.5:1)</returns>
        public static bool ValidateContrast(Color foreground, Color background)
        {
            float ratio = CalculateContrastRatio(foreground, background);
            return ratio >= 4.5f; // WCAG 2.1 AA standard
        }
        
        /// <summary>
        /// Calculates contrast ratio between two colors
        /// </summary>
        private static float CalculateContrastRatio(Color color1, Color color2)
        {
            float l1 = CalculateRelativeLuminance(color1);
            float l2 = CalculateRelativeLuminance(color2);
            
            float lighter = Mathf.Max(l1, l2);
            float darker = Mathf.Min(l1, l2);
            
            return (lighter + 0.05f) / (darker + 0.05f);
        }
        
        /// <summary>
        /// Calculates relative luminance for contrast ratio calculation
        /// </summary>
        private static float CalculateRelativeLuminance(Color color)
        {
            float RsRGB = color.r;
            float GsRGB = color.g;
            float BsRGB = color.b;
            
            float R = (RsRGB <= 0.03928f) ? RsRGB / 12.92f : Mathf.Pow((RsRGB + 0.055f) / 1.055f, 2.4f);
            float G = (GsRGB <= 0.03928f) ? GsRGB / 12.92f : Mathf.Pow((GsRGB + 0.055f) / 1.055f, 2.4f);
            float B = (BsRGB <= 0.03928f) ? BsRGB / 12.92f : Mathf.Pow((BsRGB + 0.055f) / 1.055f, 2.4f);
            
            return 0.2126f * R + 0.7152f * G + 0.0722f * B;
        }
        
        /// <summary>
        /// Gets color by semantic meaning for consistent theming
        /// </summary>
        public static Color GetSemanticColor(SemanticColor semanticColor)
        {
            switch (semanticColor)
            {
                case SemanticColor.Primary: return Primary;
                case SemanticColor.Secondary: return Secondary;
                case SemanticColor.Success: return Success;
                case SemanticColor.Error: return Error;
                case SemanticColor.Warning: return Warning;
                case SemanticColor.Info: return Info;
                case SemanticColor.Background: return Background;
                case SemanticColor.Surface: return Surface;
                case SemanticColor.TextPrimary: return TextPrimary;
                case SemanticColor.TextSecondary: return TextSecondary;
                case SemanticColor.Connected: return Connected;
                case SemanticColor.Disconnected: return Disconnected;
                case SemanticColor.Processing: return Processing;
                default:
                    Debug.LogWarning($"Unknown semantic color: {semanticColor}");
                    return Color.magenta;
            }
        }
    }
    
    /// <summary>
    /// Semantic color enumeration for consistent color usage
    /// </summary>
    public enum SemanticColor
    {
        Primary,
        Secondary,
        Success,
        Error,
        Warning,
        Info,
        Background,
        Surface,
        TextPrimary,
        TextSecondary,
        Connected,
        Disconnected,
        Processing
    }
}