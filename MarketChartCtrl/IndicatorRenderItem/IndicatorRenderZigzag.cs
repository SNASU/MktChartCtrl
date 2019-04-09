﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MarketDataSeries;

namespace MarketChartCtrl.IndicatorRenderItem
{
    public class IndicatorRenderZigzag : IndicatorRender
    {
        public IndicatorRenderZigzag(string seriesName, ISeries<double> dataSeries) : base(seriesName, dataSeries) { }

        public override void Paint(FrameControler controler, Graphics graphics, int index, int graphIdx, int drawPositionX, ConvertValueToView func1, ChartColors Colors, ChartScaling scaling, Rectangle graphRect, ref List<int> TimeLines, bool main)
        {
            base.Paint(controler, graphics, index, graphIdx, drawPositionX, func1, Colors, scaling, graphRect, ref TimeLines, main);

        }
    }
}
