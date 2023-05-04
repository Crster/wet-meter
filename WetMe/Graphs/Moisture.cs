using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using WetMe.Models;

namespace WetMe.Graphs
{
    public class Moisture : IDrawable
    {
        public int LowLevel { get; set; }

        private List<MoistureData> moistures = new List<MoistureData>();
        public void AddData(MoistureData data)
        {
            moistures.Add(data);
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            RectF rect = new RectF(dirtyRect.X + 5, dirtyRect.Y + 10, dirtyRect.Width - 10, dirtyRect.Height - 20);

            RectF graphRect = new RectF(rect.X + 35, rect.Y + 5, rect.Width - 30, rect.Height - 25);
            canvas.StrokeColor = Colors.Black;
            canvas.DrawRectangle(graphRect);

            // Draw Y Legend Value
            const int yLegendStep = 100;
            const int yLegendMax = 1024;

            float yLegendDeduction = graphRect.Height / (yLegendMax / yLegendStep);
            float yLegendNew = graphRect.Bottom;
            for (int yy = 0; yy <= yLegendMax; yy+= yLegendStep)
            {
                canvas.DrawString(yy.ToString(), graphRect.X - 25, yLegendNew, graphRect.Width, 12, HorizontalAlignment.Left, VerticalAlignment.Center);
                yLegendNew -= yLegendDeduction;
            }

            canvas.StrokeColor = Colors.Red;
            float yWaterLevel = graphRect.Bottom - (LowLevel * (graphRect.Height / yLegendMax)) + (LowLevel < 100 ? 0 : 5);
            canvas.DrawLine(new Microsoft.Maui.Graphics.PointF(graphRect.X, yWaterLevel), new Microsoft.Maui.Graphics.PointF(graphRect.Right, yWaterLevel));
            canvas.StrokeColor = Colors.Black;

            canvas.SaveState();
            canvas.Rotate(90);
            canvas.DrawString("Moisture Level", graphRect.Height / 2, -3, HorizontalAlignment.Left);
            canvas.RestoreState();

            // Draw X Legend Value
            int xLegendStep = 1;
            int xLegendMax = 20;
            float xLegendAddition = graphRect.Width / (xLegendMax / xLegendStep);
            float xLegendNew = graphRect.X - 25;
            for (int xx = 0; xx <= xLegendMax; xx += xLegendStep)
            {
                canvas.DrawString(xx.ToString(), xLegendNew, graphRect.Bottom, xLegendAddition, 12, HorizontalAlignment.Left, VerticalAlignment.Center);
                xLegendNew += xLegendAddition;
            }

            // Draw Graph
            PathF path = new PathF();
            path.MoveTo(graphRect.X, graphRect.Bottom);

            float xSpacing = xLegendAddition;
            int visibleData = Convert.ToInt32(graphRect.Width / xSpacing) + 1;

            float newPathX = graphRect.X;
            foreach (MoistureData data in (moistures.Count < visibleData ? moistures : moistures.GetRange(moistures.Count - visibleData, visibleData)))
            {
                float yMoisture = graphRect.Bottom - (data.Moisture * (graphRect.Height / yLegendMax)) + (data.Moisture < 100 ? 0 : 5);
                RectF dataRect = new RectF(newPathX == graphRect.X ? newPathX : newPathX - 25, yMoisture, graphRect.Width, graphRect.Height);

                path.LineTo(dataRect.X, dataRect.Y);
                // canvas.DrawString(data.Moisture.ToString(), dataRect.X, dataRect.Y, 20, 12, HorizontalAlignment.Left, VerticalAlignment.Center);
                newPathX += xSpacing;
            }

            canvas.StrokeColor = Colors.Blue;
            canvas.StrokeSize = 2;
            canvas.DrawPath(path);
        }
    }
}
