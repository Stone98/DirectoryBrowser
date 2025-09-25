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
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            // Clear the background
            g.Clear(Color.Transparent);
            
            // Calculate dimensions
            float centerX = size / 2f;
            float centerY = size / 2f;
            float arrowSize = size * 0.7f; // Use 70% of the icon size
            float margin = (size - arrowSize) / 2f;
            
            // Create gradient brush for modern look
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                new PointF(0, margin),
                new PointF(0, margin + arrowSize),
                Color.FromArgb(60, 120, 180), // Blue gradient start
                Color.FromArgb(40, 80, 140)))  // Blue gradient end
            {
                // Define arrow points for a sleek chevron-style arrow
                PointF[] arrowPoints = new PointF[]
                {
                    // Top point (tip of arrow)
                    new PointF(centerX, margin),
                    
                    // Right side points
                    new PointF(centerX + arrowSize * 0.35f, margin + arrowSize * 0.4f),
                    new PointF(centerX + arrowSize * 0.15f, margin + arrowSize * 0.4f),
                    new PointF(centerX + arrowSize * 0.15f, margin + arrowSize * 0.85f),
                    
                    // Bottom right
                    new PointF(centerX - arrowSize * 0.15f, margin + arrowSize * 0.85f),
                    
                    // Left side points (mirror of right)
                    new PointF(centerX - arrowSize * 0.15f, margin + arrowSize * 0.4f),
                    new PointF(centerX - arrowSize * 0.35f, margin + arrowSize * 0.4f)
                };
                
                // Fill the arrow with gradient
                g.FillPolygon(gradientBrush, arrowPoints);
                
                // Add subtle highlight on top edge
                using (Pen highlightPen = new Pen(Color.FromArgb(120, Color.White), 1.5f))
                {
                    highlightPen.LineJoin = LineJoin.Round;
                    // Draw highlight on the top edges
                    g.DrawLine(highlightPen, arrowPoints[6], arrowPoints[0]); // Left top edge
                    g.DrawLine(highlightPen, arrowPoints[0], arrowPoints[1]); // Right top edge
                }
                
                // Add subtle shadow/outline
                using (Pen outlinePen = new Pen(Color.FromArgb(80, Color.Black), 1f))
                {
                    outlinePen.LineJoin = LineJoin.Round;
                    g.DrawPolygon(outlinePen, arrowPoints);
                }
            }
            
            // Convert bitmap to icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            return (Icon)icon.Clone();
        }
    }
}