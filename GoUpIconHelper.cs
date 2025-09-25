using System;
using System.Drawing;
using System.Drawing.Drawing2D;

/// <summary>
/// Helper class for creating a custom up arrow icon for the "Go up" folder navigation.
/// </summary>
public static class GoUpIconHelper
{
    /// <summary>
    /// Creates a small (16x16) up arrow icon for the "Go up" folder navigation.
    /// </summary>
    /// <returns>An Icon representing an up arrow</returns>
    public static Icon GetSmallUpArrowIcon()
    {
        return CreateUpArrowIcon(16);
    }

    /// <summary>
    /// Creates a large (32x32) up arrow icon for the "Go up" folder navigation.
    /// </summary>
    /// <returns>An Icon representing an up arrow</returns>
    public static Icon GetLargeUpArrowIcon()
    {
        return CreateUpArrowIcon(32);
    }

    /// <summary>
    /// Creates an up arrow icon of the specified size.
    /// </summary>
    /// <param name="size">The size of the icon (width and height)</param>
    /// <returns>An Icon representing an up arrow</returns>
    private static Icon CreateUpArrowIcon(int size)
    {
        using (Bitmap bitmap = new Bitmap(size, size))
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            // Enable anti-aliasing for smoother drawing
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Clear the background
            g.Clear(Color.Transparent);
            
            // Calculate arrow dimensions based on icon size
            int margin = size / 8; // Small margin around the arrow
            int arrowWidth = size - (margin * 2);
            int arrowHeight = size - (margin * 2);
            
            // Define the arrow points
            Point[] arrowPoints = new Point[]
            {
                // Top point (tip of arrow)
                new Point(size / 2, margin),
                
                // Bottom left point
                new Point(margin, margin + arrowHeight),
                
                // Bottom left inner point (for arrow shaft)
                new Point(margin + arrowWidth / 3, margin + arrowHeight),
                
                // Bottom left shaft point
                new Point(margin + arrowWidth / 3, margin + (arrowHeight * 2 / 3)),
                
                // Bottom right shaft point
                new Point(size - margin - arrowWidth / 3, margin + (arrowHeight * 2 / 3)),
                
                // Bottom right inner point (for arrow shaft)
                new Point(size - margin - arrowWidth / 3, margin + arrowHeight),
                
                // Bottom right point
                new Point(size - margin, margin + arrowHeight)
            };
            
            // Create brushes and pens
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(80, 80, 80))) // Dark gray
            using (Pen pen = new Pen(Color.FromArgb(40, 40, 40), 1)) // Darker outline
            {
                // Fill the arrow
                g.FillPolygon(brush, arrowPoints);
                
                // Draw the arrow outline
                g.DrawPolygon(pen, arrowPoints);
            }
            
            // Convert bitmap to icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            return (Icon)icon.Clone();
        }
    }
}