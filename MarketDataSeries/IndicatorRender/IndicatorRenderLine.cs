﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using MarketChartCtrl;

namespace MarketSeries
{
    public enum LineStyle
    {
        Dots,  // Dot
        DotsRare,  // Custom Dot
        DotsVeryRare,  // Custom Dot
        Lines,  // Dash
        LinesDots,  // DashDot
        Solid
    }

    public class IndicatorRenderLine : IndicatorRender
    {
        public IndicatorRenderLine(string seriesName, IDataSeries dataSeries) : base(seriesName, dataSeries)
        {
            LineStyle = LineStyle.Solid;
        }

        public Pen pen = new Pen(System.Drawing.Color.Green, 1);

        private LineStyle _LineStyle;
        public LineStyle LineStyle { get { return _LineStyle; } set { _LineStyle = value; SetLineStyle(value); } }

        public float Size { get { return pen.Width; } set { pen.Width = value; } }
        public Color Color { get { return pen.Color; } set { pen.Color = value; } }

        private void SetLineStyle(LineStyle value)
        {
            switch (value)
            {
                case LineStyle.Solid: { pen.DashStyle = DashStyle.Solid; break; }
                case LineStyle.Dots: { pen.DashStyle = DashStyle.Dot; pen.DashOffset = 2.0F; break; }
                case LineStyle.Lines: { pen.DashStyle = DashStyle.Dash; pen.DashOffset = 2.0F; break; }
                case LineStyle.LinesDots: { pen.DashStyle = DashStyle.DashDot; pen.DashOffset = 2.0F; break; }
                case LineStyle.DotsRare: { pen.DashStyle = DashStyle.Dot; pen.DashOffset = 2.0F; break; }
                case LineStyle.DotsVeryRare: { pen.DashStyle = DashStyle.Dot; pen.DashOffset = 3.0F; break; }
            }
        }

        public override void Paint(Graphics graphics, int index, int graphIdx, int drawPositionX, ConvertValueToView func1, ChartColors Colors, ChartScaling scaling, Rectangle graphRect, ref List<int> TimeLines, bool main)
        {
            base.Paint(graphics, index, graphIdx, drawPositionX, func1, Colors, scaling, graphRect, ref TimeLines, main);

            // 開始点は線なし
            if (index <= 0)
                return;

            // データが終端を超えたら処理しない
            if (index >= data.Count)
                return;

            if (double.IsNaN(data[index]) || double.IsNaN(data[index - 1]))
                return;

            try
            {
                //System.Diagnostics.Debug.WriteLine("[" + series[index].ToString()+"]");

                // ひとつ前時点からの線引き
                graphics.DrawLine(pen, drawPositionX - (scaling.Width - scaling.Margin), func1(data[index - 1]), drawPositionX + scaling.Size + 1, func1(data[index]));
            }
            catch (Exception e)
            {
                //System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}
