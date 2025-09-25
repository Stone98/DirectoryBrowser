using System;
using System.Drawing;
using System.Drawing.Drawing2D;

/// <summary>
/// Helper class for creating a custom refresh icon with circular arrows.
/// </summary>
public static class RefreshIconHelper
{
    /// <summary>
    /// Creates a small (16x16) refresh icon with circular arrows.
    /// </summary>
    /// <returns>An Icon representing a refresh symbol</returns>
    public static Icon GetSmallRefreshIcon()
    {
        return CreateRefreshIcon(16);
    }

    /// <summary>
    /// Creates a large (32x32) refresh icon with circular arrows.
    /// </summary>
    /// <returns>An Icon representing a refresh symbol</returns>
    public static Icon GetLargeRefreshIcon()
    {
        return CreateRefreshIcon(32);
    }

    /// <summary>
    /// Creates a refresh icon of the specified size.
    /// </summary>
    /// <param name="size">The size of the icon (width and height)</param>
    /// <returns>An Icon representing a refresh symbol</returns>
    private static Icon CreateRefreshIcon(int size)
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
            float outerRadius = size * 0.4f;
            float innerRadius = size * 0.25f;
            float penWidth = Math.Max(1.5f, size / 10f);
            
            // Create gradient pen for modern look
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                new PointF(0, 0),
                new PointF(size, size),
                Color.FromArgb(50, 150, 50),  // Green gradient start
                Color.FromArgb(30, 100, 30))) // Green gradient end
            {
                using (Pen gradientPen = new Pen(gradientBrush, penWidth))
                {
                    gradientPen.StartCap = LineCap.Round;
                    gradientPen.EndCap = LineCap.Round;
                    
                    // Calculate arc rectangle
                    RectangleF arcRect = new RectangleF(
                        centerX - outerRadius,
                        centerY - outerRadius,
                        outerRadius * 2,
                        outerRadius * 2);
                    
                    // Draw main circular arc (300 degrees, leaving gaps for arrows)
                    g.DrawArc(gradientPen, arcRect, -30, 300);
                    
                    // Calculate arrow positions and draw arrow heads
                    DrawModernArrowHead(g, centerX, centerY, outerRadius, -30, gradientBrush, size); // Top-right arrow
                    DrawModernArrowHead(g, centerX, centerY, outerRadius, 270, gradientBrush, size); // Bottom-left arrow
                }
            }
            
            // Add inner highlight circle for depth
            using (Pen highlightPen = new Pen(Color.FromArgb(40, Color.White), Math.Max(1f, penWidth * 0.6f)))
            {
                RectangleF innerRect = new RectangleF(
                    centerX - innerRadius,
                    centerY - innerRadius,
                    innerRadius * 2,
                    innerRadius * 2);
                
                g.DrawArc(highlightPen, innerRect, 45, 180);
            }
            
            // Convert bitmap to icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            return (Icon)icon.Clone();
        }
    }
    
    /// <summary>
    /// Draws a modern triangular arrow head at the specified position.
    /// </summary>
    private static void DrawModernArrowHead(Graphics g, float centerX, float centerY, float radius, 
        float angleDegrees, LinearGradientBrush brush, int iconSize)
    {
        float angleRadians = (float)(angleDegrees * Math.PI / 180);
        float arrowSize = Math.Max(3, iconSize / 4f);
        
        // Calculate arrow tip position
        float tipX = centerX + radius * (float)Math.Cos(angleRadians);
        float tipY = centerY + radius * (float)Math.Sin(angleRadians);
        
        // Calculate arrow base points
        float baseAngle1 = angleRadians + (float)(Math.PI * 0.8); // 144 degrees offset
        float baseAngle2 = angleRadians - (float)(Math.PI * 0.8); // -144 degrees offset
        
        float base1X = tipX + arrowSize * (float)Math.Cos(baseAngle1);
        float base1Y = tipY + arrowSize * (float)Math.Sin(baseAngle1);
        
        float base2X = tipX + arrowSize * (float)Math.Cos(baseAngle2);
        float base2Y = tipY + arrowSize * (float)Math.Sin(baseAngle2);
        
        // Create arrow triangle
        PointF[] arrowPoints = new PointF[]
        {
            new PointF(tipX, tipY),
            new PointF(base1X, base1Y),
            new PointF(base2X, base2Y)
        };
        
        // Fill arrow with gradient
        g.FillPolygon(brush, arrowPoints);
        
        // Add subtle outline
        using (Pen outlinePen = new Pen(Color.FromArgb(60, Color.Black), 1f))
        {
            outlinePen.LineJoin = LineJoin.Round;
            g.DrawPolygon(outlinePen, arrowPoints);
        }
    }
}