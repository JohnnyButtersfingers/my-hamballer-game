using UnityEngine;
using UnityEngine.UI;

namespace DodgeBLTZ
{
    /// <summary>
    /// Helper class for responsive UI management
    /// Ensures proper touch targets and scaling across devices
    /// </summary>
    public class ResponsiveUIHelper : MonoBehaviour
    {
        [Header("Touch Target Settings")]
        [SerializeField] private float minTouchTargetSize = 44f; // Minimum 44px for accessibility
        [SerializeField] private bool autoAdjustButtonSizes = true;
        
        [Header("Text Scaling")]
        [SerializeField] private bool autoScaleText = true;
        [SerializeField] private float baseScreenWidth = 1920f;
        [SerializeField] private float minTextScale = 0.8f;
        [SerializeField] private float maxTextScale = 1.2f;
        
        private void Start()
        {
            if (autoAdjustButtonSizes)
                AdjustButtonSizes();
                
            if (autoScaleText)
                AdjustTextScaling();
        }
        
        /// <summary>
        /// Ensures all buttons meet minimum touch target requirements
        /// </summary>
        private void AdjustButtonSizes()
        {
            Button[] buttons = FindObjectsOfType<Button>();
            
            foreach (Button button in buttons)
            {
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Ensure minimum touch target size
                    Vector2 size = rectTransform.sizeDelta;
                    
                    if (size.x < minTouchTargetSize)
                        size.x = minTouchTargetSize;
                        
                    if (size.y < minTouchTargetSize)
                        size.y = minTouchTargetSize;
                        
                    rectTransform.sizeDelta = size;
                    
                    // Apply color palette to button if not already set
                    ApplyButtonColors(button);
                }
            }
        }
        
        /// <summary>
        /// Applies color palette to buttons for consistency
        /// </summary>
        private void ApplyButtonColors(Button button)
        {
            // Check if button already has custom colors
            ColorBlock colors = button.colors;
            
            // Apply primary button style by default
            colors.normalColor = UIColorPalette.Primary;
            colors.highlightedColor = Color.Lerp(UIColorPalette.Primary, Color.white, 0.1f);
            colors.pressedColor = Color.Lerp(UIColorPalette.Primary, Color.black, 0.2f);
            colors.disabledColor = UIColorPalette.ButtonDisabled;
            colors.selectedColor = UIColorPalette.Primary;
            
            button.colors = colors;
        }
        
        /// <summary>
        /// Adjusts text scaling based on screen size
        /// </summary>
        private void AdjustTextScaling()
        {
            float screenWidth = Screen.width;
            float scaleFactor = Mathf.Clamp(screenWidth / baseScreenWidth, minTextScale, maxTextScale);
            
            Text[] texts = FindObjectsOfType<Text>();
            
            foreach (Text text in texts)
            {
                // Only scale if not already manually configured
                if (!text.gameObject.name.Contains("NoAutoScale"))
                {
                    int originalSize = text.fontSize;
                    text.fontSize = Mathf.RoundToInt(originalSize * scaleFactor);
                    
                    // Apply text colors from palette
                    ApplyTextColors(text);
                }
            }
        }
        
        /// <summary>
        /// Applies consistent text colors based on component name or tag
        /// </summary>
        private void ApplyTextColors(Text text)
        {
            string objectName = text.gameObject.name.ToLower();
            
            // Apply colors based on object naming convention
            if (objectName.Contains("title") || objectName.Contains("header"))
            {
                text.color = UIColorPalette.TextPrimary;
            }
            else if (objectName.Contains("error") || objectName.Contains("fail"))
            {
                text.color = UIColorPalette.Error;
            }
            else if (objectName.Contains("success") || objectName.Contains("win"))
            {
                text.color = UIColorPalette.Success;
            }
            else if (objectName.Contains("info") || objectName.Contains("help"))
            {
                text.color = UIColorPalette.TextSecondary;
            }
            else if (objectName.Contains("account") || objectName.Contains("connected"))
            {
                text.color = UIColorPalette.Connected;
            }
            // Default to primary text color
            else if (text.color == Color.white) // Only change if using default white
            {
                text.color = UIColorPalette.TextPrimary;
            }
        }
        
        /// <summary>
        /// Manual method to validate all UI elements meet accessibility standards
        /// Call this from inspector or during development
        /// </summary>
        [ContextMenu("Validate UI Accessibility")]
        public void ValidateAccessibility()
        {
            int issues = 0;
            
            // Check button sizes
            Button[] buttons = FindObjectsOfType<Button>();
            foreach (Button button in buttons)
            {
                RectTransform rt = button.GetComponent<RectTransform>();
                if (rt.sizeDelta.x < minTouchTargetSize || rt.sizeDelta.y < minTouchTargetSize)
                {
                    Debug.LogWarning($"Button '{button.name}' is too small for touch accessibility: {rt.sizeDelta}");
                    issues++;
                }
            }
            
            // Check text contrast (simplified check)
            Text[] texts = FindObjectsOfType<Text>();
            foreach (Text text in texts)
            {
                if (!UIColorPalette.ValidateContrast(text.color, UIColorPalette.Background))
                {
                    Debug.LogWarning($"Text '{text.name}' may have insufficient contrast");
                    issues++;
                }
            }
            
            if (issues == 0)
            {
                Debug.Log("✅ All UI elements pass accessibility validation!");
            }
            else
            {
                Debug.LogError($"❌ Found {issues} accessibility issues");
            }
        }
    }
}