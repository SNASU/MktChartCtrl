﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarketDataSeries;
using MarketDataSeries.PFPatterns;
using PFChartCtrl.IndicatorRenderItem;
using System.Text.RegularExpressions;

namespace ForexPAF
{
    public class CsvStandard : ICsvLoader
    {
        public static bool CanLoading(string filename)
        {
            using (var sr = new System.IO.StreamReader(filename))
            {
                // skip
                var line = sr.ReadLine();

                // 先頭行がデータ行でないことをチェック
                if ((Regex.Match(line, @"^[\.,0-9]+$")).Success)
                {
                    return false;
                }

                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                // 6列の構成かをチェック
                var values = line.Split(',');
                if (values.Length != 6)
                    return false;

            }
            return true;
        }

        public Tuple<bool, int, int> NFormat(string filename, int iMaxLength = 7)
        {
            int imx = 200;
            int i = 0;

            bool dotDate = false;
            int leftStr = 0;
            int rightStr = 0;

            using (var sr = new System.IO.StreamReader(filename))
            {
                // skip
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    if (i > imx)
                        break;

                    var line = sr.ReadLine();

                    var values = line.Split(',');
                    if (values.Length < 6)
                        continue;

                    if (values[0].IndexOf(".") < 8)
                    {
                        dotDate = true;
                    }

                    Action<string> f = (v) =>
                    {
                        int x = v.IndexOf(".");
                        if (leftStr < x)
                            leftStr = x;
                    };

                    Action<string> g = (v) =>
                    {
                        int x = v.Length - leftStr;
                        if (rightStr < x)
                        {
                            int y = iMaxLength - (leftStr + 1);
                            if (y > x)
                                rightStr = x;
                            else
                                rightStr = y;
                        }
                    };

                    f(values[1]);
                    f(values[2]);
                    f(values[3]);
                    f(values[4]);

                    g(values[1]);
                    g(values[2]);
                    g(values[3]);
                    g(values[4]);

                    ++i;
                }
                sr.Close();
            }

            // ドル円の2.3対応
            if (leftStr == 2)
            {
                leftStr = 3;
                rightStr = 3;
            }

            // 上記でサポートしていない 99999.99 のパターンの対応コード
            if (leftStr > 3)
                if (rightStr < 2)
                    rightStr = 2;

            return new Tuple<bool, int, int>(dotDate, leftStr, rightStr);

        }

        public MarketDataSeries.MarketData Load(string filename)
        {

            var time = new MarketDataSeries.TimeSeries();
            var open = new MarketDataSeries.DataSeries();
            var high = new MarketDataSeries.DataSeries();
            var low = new MarketDataSeries.DataSeries();
            var close = new MarketDataSeries.DataSeries();
            var vol = new MarketDataSeries.DataSeries();

            // csvファイルを開く
            using (var sr = new System.IO.StreamReader(filename))
            {
                // skip top line
                sr.ReadLine();

                // ストリームの末尾まで繰り返す
                while (!sr.EndOfStream)
                {
                    // ファイルから一行読み込む
                    var line = sr.ReadLine();

                    // 読み込んだ一行をカンマ毎に分けて配列に格納する
                    var values = line.Split(',');
                    if (values.Length < 6)
                        return null;

                    time.Add(DateTime.Parse(values[0]));

                    open.Add(double.Parse(values[1]));
                    high.Add(double.Parse(values[2]));
                    low.Add(double.Parse(values[3]));
                    close.Add(double.Parse(values[4]));
                    if (values.Count() >= 6)
                        vol.Add(double.Parse(values[5]));
                    else
                        vol.Add(0d);
                }
                sr.Close();
            }
            return new MarketDataSeries.MarketData("", time, open, high, low, close, vol);
        }

    }
}
