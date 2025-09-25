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
            
            // Clear the background
            g.Clear(Color.Transparent);
            
            // Calculate dimensions
            int margin = size / 8;
            int circleSize = size - (margin * 2);
            int penWidth = Math.Max(1, size / 12);
            int arrowSize = Math.Max(2, size / 6);
            
            // Create pen for drawing the circular arrows
            using (Pen pen = new Pen(Color.FromArgb(80, 80, 80), penWidth))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                // Draw the main circular arc (about 270 degrees)
                Rectangle circleRect = new Rectangle(margin, margin, circleSize, circleSize);
                g.DrawArc(pen, circleRect, -45, 270);
                
                // Calculate arrow positions
                float centerX = size / 2f;
                float centerY = size / 2f;
                float radius = circleSize / 2f;
                
                // Draw arrow head 1 (top right)
                float angle1 = (float)(-45 * Math.PI / 180); // -45 degrees in radians
                float arrowX1 = centerX + (float)(radius * Math.Cos(angle1));
                float arrowY1 = centerY + (float)(radius * Math.Sin(angle1));
                
                // Arrow head points
                float arrowAngle1 = angle1 + (float)(Math.PI / 6); // 30 degrees
                float arrowAngle2 = angle1 - (float)(Math.PI / 6); // -30 degrees
                
                PointF arrow1Point1 = new PointF(
                    arrowX1 - arrowSize * (float)Math.Cos(arrowAngle1),
                    arrowY1 - arrowSize * (float)Math.Sin(arrowAngle1)
                );
                
                PointF arrow1Point2 = new PointF(
                    arrowX1 - arrowSize * (float)Math.Cos(arrowAngle2),
                    arrowY1 - arrowSize * (float)Math.Sin(arrowAngle2)
                );
                
                // Draw first arrow head
                g.DrawLine(pen, arrowX1, arrowY1, arrow1Point1.X, arrow1Point1.Y);
                g.DrawLine(pen, arrowX1, arrowY1, arrow1Point2.X, arrow1Point2.Y);
                
                // Draw arrow head 2 (bottom left)
                float angle2 = (float)((225) * Math.PI / 180); // 225 degrees in radians
                float arrowX2 = centerX + (float)(radius * Math.Cos(angle2));
                float arrowY2 = centerY + (float)(radius * Math.Sin(angle2));
                
                // Arrow head points (pointing in opposite direction)
                float arrowAngle3 = angle2 - (float)(Math.PI / 6); // -30 degrees from arc direction
                float arrowAngle4 = angle2 + (float)(Math.PI / 6); // +30 degrees from arc direction
                
                PointF arrow2Point1 = new PointF(
                    arrowX2 - arrowSize * (float)Math.Cos(arrowAngle3),
                    arrowY2 - arrowSize * (float)Math.Sin(arrowAngle3)
                );
                
                PointF arrow2Point2 = new PointF(
                    arrowX2 - arrowSize * (float)Math.Cos(arrowAngle4),
                    arrowY2 - arrowSize * (float)Math.Sin(arrowAngle4)
                );
                
                // Draw second arrow head
                g.DrawLine(pen, arrowX2, arrowY2, arrow2Point1.X, arrow2Point1.Y);
                g.DrawLine(pen, arrowX2, arrowY2, arrow2Point2.X, arrow2Point2.Y);
            }
            
            // Convert bitmap to icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            return (Icon)icon.Clone();
        }
    }
}