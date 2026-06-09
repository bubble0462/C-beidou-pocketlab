using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeepSeaAquacultureTerminal
{
    /// <summary>
    /// 全局主题配色方案 - 深海蓝科技风格
    /// </summary>
    public static class AppTheme
    {
        // 主色调 - 深海蓝
        public static readonly Color PrimaryDark = Color.FromArgb(10, 25, 47);
        public static readonly Color PrimaryMedium = Color.FromArgb(15, 40, 70);
        public static readonly Color PrimaryBlue = Color.FromArgb(30, 80, 140);
        public static readonly Color PrimaryLight = Color.FromArgb(45, 110, 180);

        // 强调色
        public static readonly Color AccentCyan = Color.FromArgb(0, 186, 228);
        public static readonly Color AccentGreen = Color.FromArgb(46, 204, 113);
        public static readonly Color AccentOrange = Color.FromArgb(243, 156, 18);
        public static readonly Color AccentRed = Color.FromArgb(231, 76, 60);
        public static readonly Color AccentYellow = Color.FromArgb(241, 196, 15);

        // 背景色
        public static readonly Color BackgroundDark = Color.FromArgb(18, 32, 55);
        public static readonly Color BackgroundPanel = Color.FromArgb(22, 38, 62);
        public static readonly Color BackgroundCard = Color.FromArgb(28, 48, 75);
        public static readonly Color BackgroundLight = Color.FromArgb(35, 58, 88);

        // 文字色
        public static readonly Color TextPrimary = Color.FromArgb(230, 240, 250);
        public static readonly Color TextSecondary = Color.FromArgb(160, 180, 200);
        public static readonly Color TextMuted = Color.FromArgb(100, 120, 145);

        // 边框色
        public static readonly Color BorderColor = Color.FromArgb(40, 65, 95);
        public static readonly Color BorderHighlight = Color.FromArgb(50, 90, 135);

        // 字体
        public static readonly Font TitleFont = new Font("微软雅黑", 16F, FontStyle.Bold);
        public static readonly Font SubTitleFont = new Font("微软雅黑", 12F, FontStyle.Bold);
        public static readonly Font HeaderFont = new Font("微软雅黑", 11F, FontStyle.Bold);
        public static readonly Font ContentFont = new Font("微软雅黑", 9F);
        public static readonly Font SmallFont = new Font("微软雅黑", 8F);
        public static readonly Font DataFont = new Font("Consolas", 10F);
        public static readonly Font DataFontBold = new Font("Consolas", 10F, FontStyle.Bold);

        /// <summary>
        /// 创建标准侧边栏按钮
        /// </summary>
        public static Button CreateNavButton(string text, int index)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Dock = DockStyle.Top;
            btn.Height = 42;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = BackgroundLight;
            btn.BackColor = BackgroundPanel;
            btn.ForeColor = TextSecondary;
            btn.Font = ContentFont;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(20, 0, 0, 0);
            btn.Tag = index;
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        /// <summary>
        /// 选中导航按钮样式
        /// </summary>
        public static void ActivateNavButton(Button btn)
        {
            btn.BackColor = PrimaryBlue;
            btn.ForeColor = TextPrimary;
            btn.Font = new Font("微软雅黑", 9F, FontStyle.Bold);
        }

        /// <summary>
        /// 取消选中导航按钮样式
        /// </summary>
        public static void DeactivateNavButton(Button btn)
        {
            btn.BackColor = BackgroundPanel;
            btn.ForeColor = TextSecondary;
            btn.Font = ContentFont;
        }

        /// <summary>
        /// 创建数据卡片面板
        /// </summary>
        public static Panel CreateCard(string title, int width, int height)
        {
            var card = new Panel();
            card.Size = new Size(width, height);
            card.BackColor = BackgroundCard;
            card.Padding = new Padding(1);
            card.BorderStyle = BorderStyle.None;

            // 标题标签
            var lbl = new Label();
            lbl.Text = title;
            lbl.ForeColor = AccentCyan;
            lbl.Font = HeaderFont;
            lbl.Location = new Point(12, 8);
            lbl.AutoSize = true;
            card.Controls.Add(lbl);

            // 底部分割线
            var line = new Panel();
            line.Height = 1;
            line.Width = width - 24;
            line.BackColor = BorderHighlight;
            line.Location = new Point(12, 32);
            card.Controls.Add(line);

            return card;
        }

        /// <summary>
        /// 创建状态标签
        /// </summary>
        public static Label CreateStatusLabel(string text, bool isGood)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.ForeColor = isGood ? AccentGreen : AccentRed;
            lbl.Font = new Font("微软雅黑", 9F, FontStyle.Bold);
            lbl.AutoSize = true;
            return lbl;
        }

        /// <summary>
        /// 创建数据标签 (字段名: 值)
        /// </summary>
        public static void AddDataRow(Panel parent, string label, string value, int x, int y, Color? valueColor = null)
        {
            var lbl = new Label();
            lbl.Text = label;
            lbl.ForeColor = TextSecondary;
            lbl.Font = SmallFont;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            parent.Controls.Add(lbl);

            var val = new Label();
            val.Text = value;
            val.ForeColor = valueColor ?? TextPrimary;
            val.Font = DataFont;
            val.Location = new Point(x + 100, y - 1);
            val.AutoSize = true;
            parent.Controls.Add(val);
        }

        /// <summary>
        /// 创建简易柱状图面板
        /// </summary>
        public static Panel CreateBarChart(string title, string[] labels, float[] values, int width, int height)
        {
            var chart = CreateCard(title, width, height);

            if (labels == null || values == null || labels.Length == 0) return chart;

            float maxVal = 0;
            foreach (var v in values) if (v > maxVal) maxVal = v;
            if (maxVal == 0) maxVal = 1;

            int chartLeft = 50;
            int chartTop = 50;
            int chartWidth = width - 70;
            int chartHeight = height - 90;
            int barWidth = chartWidth / labels.Length - 8;

            var rnd = new Random();

            for (int i = 0; i < labels.Length; i++)
            {
                int barH = (int)(chartHeight * values[i] / maxVal);
                if (barH < 2) barH = 2;

                int bx = chartLeft + i * (barWidth + 8);
                int by = chartTop + chartHeight - barH;

                var bar = new Panel();
                bar.Location = new Point(bx, by);
                bar.Size = new Size(barWidth, barH);

                // 根据数值给不同颜色
                if (values[i] / maxVal > 0.7f)
                    bar.BackColor = AccentGreen;
                else if (values[i] / maxVal > 0.4f)
                    bar.BackColor = AccentCyan;
                else
                    bar.BackColor = PrimaryLight;

                chart.Controls.Add(bar);

                // 标签
                var lbl = new Label();
                lbl.Text = labels[i];
                lbl.ForeColor = TextMuted;
                lbl.Font = new Font("微软雅黑", 7F);
                lbl.Location = new Point(bx - 4, chartTop + chartHeight + 4);
                lbl.AutoSize = true;
                chart.Controls.Add(lbl);

                // 数值
                var valLbl = new Label();
                valLbl.Text = values[i].ToString("F1");
                valLbl.ForeColor = TextPrimary;
                valLbl.Font = new Font("Consolas", 7F);
                valLbl.Location = new Point(bx, by - 14);
                valLbl.AutoSize = true;
                chart.Controls.Add(valLbl);
            }

            return chart;
        }

        /// <summary>
        /// 创建趋势折线图面板
        /// </summary>
        public static Panel CreateLineChart(string title, string[] labels, float[] values, int width, int height)
        {
            var chart = CreateCard(title, width, height);
            if (labels == null || values == null || labels.Length < 2) return chart;

            float minVal = float.MaxValue, maxVal = float.MinValue;
            foreach (var v in values)
            {
                if (v < minVal) minVal = v;
                if (v > maxVal) maxVal = v;
            }
            float range = maxVal - minVal;
            if (range == 0) range = 1;

            int chartLeft = 55;
            int chartTop = 50;
            int chartWidth = width - 75;
            int chartHeight = height - 90;

            // 计算点坐标
            Point[] points = new Point[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                int px = chartLeft + (int)(chartWidth * i / (values.Length - 1));
                int py = chartTop + chartHeight - (int)(chartHeight * (values[i] - minVal) / range);
                points[i] = new Point(px, py);
            }

            // 绘制折线
            var pictureBox = new PictureBox();
            pictureBox.Location = new Point(0, 35);
            pictureBox.Size = new Size(width, height - 40);
            pictureBox.BackColor = BackgroundCard;
            pictureBox.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 网格线
                using (var gridPen = new Pen(BorderColor))
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        int gy = chartTop - 15 + (int)((chartHeight + 30) * i / 4);
                        g.DrawLine(gridPen, chartLeft - 5, gy, chartLeft + chartWidth + 5, gy);
                    }
                }

                // 折线
                if (points.Length >= 2)
                {
                    using (var linePen = new Pen(AccentCyan, 2))
                    {
                        g.DrawLines(linePen, points);
                    }
                }

                // 数据点
                foreach (var p in points)
                {
                    g.FillEllipse(new SolidBrush(AccentCyan), p.X - 4, p.Y - 4, 8, 8);
                    g.FillEllipse(new SolidBrush(BackgroundCard), p.X - 2, p.Y - 2, 4, 4);
                }
            };
            chart.Controls.Add(pictureBox);

            // X轴标签
            int step = Math.Max(1, labels.Length / 8);
            for (int i = 0; i < labels.Length; i += step)
            {
                var lbl = new Label();
                lbl.Text = labels[i];
                lbl.ForeColor = TextMuted;
                lbl.Font = new Font("微软雅黑", 7F);
                int lx = chartLeft + (int)(chartWidth * i / (labels.Length - 1)) - 15;
                lbl.Location = new Point(lx, chartTop + chartHeight + 8);
                lbl.AutoSize = true;
                chart.Controls.Add(lbl);
            }

            return chart;
        }
    }
}
