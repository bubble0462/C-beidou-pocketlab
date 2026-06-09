using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DeepSeaAquacultureTerminal
{
    public class MainForm : Form
    {
        // 根布局
        private Panel _topBar;
        private TableLayoutPanel _rootTable;

        // 三栏面板
        private Panel _leftNav;
        private Panel _mainContent;
        private Panel _rightPanel;

        // 顶部栏控件
        private Label _lblTitle;
        private Label _lblBeiDouTime;
        private Label _lblStatusLine;

        // 右侧面板固定控件
        private Label _lblCurrentModule;
        private Panel _dividerLine;
        private Panel _rightDividerLine;

        // 导航
        private Button[] _navButtons;
        private Label _lblVersion;
        private int _activeModule = 0;

        // 轻量通知条
        private Panel _toastBar;
        private Label _toastLabel;
        private Timer _toastTimer;

        // 模块名称
        private readonly string[] _moduleNames = new string[]
        {
            "养殖池整体监控",
            "实时视频监控",
            "病虫防治",
            "利润分析",
            "标准值设定",
            "数据实时查询",
            "趋势图查询",
            "历史数据查询",
            "控制形式设置",
            "控制时间设置"
        };

        public MainForm()
        {
            InitializeForm();
            InitializeTopBar();
            InitializeRootTable();
            InitializeLeftNav();
            InitializeMainContent();
            InitializeRightPanel();
            InitializeTimer();
            InitializeToast();
            SwitchModule(0);
        }

        /// <summary>
        /// 窗体基本设置
        /// </summary>
        private void InitializeForm()
        {
            this.Text = "基于北斗时空数据的深远海养殖辅助分析终端软件 V1.0";
            this.Size = new Size(1366, 768);
            this.MinimumSize = new Size(1024, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppTheme.BackgroundDark;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 顶部状态栏 - 用 Dock Left/Right 子面板避免坐标计算
        /// </summary>
        private void InitializeTopBar()
        {
            _topBar = new Panel();
            _topBar.Dock = DockStyle.Top;
            _topBar.Height = 52;
            _topBar.BackColor = AppTheme.PrimaryDark;
            this.Controls.Add(_topBar);

            // ---- 右侧状态区 (先加，Dock.Right 先占位) ----
            var rightStatus = new Panel();
            rightStatus.Dock = DockStyle.Right;
            rightStatus.Width = 290;
            rightStatus.BackColor = AppTheme.PrimaryDark;
            _topBar.Controls.Add(rightStatus);

            _lblBeiDouTime = new Label();
            _lblBeiDouTime.Text = "北斗授时: " + DemoData.GetBeiDouTime();
            _lblBeiDouTime.ForeColor = AppTheme.AccentCyan;
            _lblBeiDouTime.Font = AppTheme.DataFont;
            _lblBeiDouTime.Location = new Point(8, 6);
            _lblBeiDouTime.AutoSize = true;
            rightStatus.Controls.Add(_lblBeiDouTime);

            _lblStatusLine = new Label();
            _lblStatusLine.Text = "● 系统在线 | 传感器: 24/24 | 北斗信号: 正常";
            _lblStatusLine.ForeColor = AppTheme.AccentGreen;
            _lblStatusLine.Font = AppTheme.SmallFont;
            _lblStatusLine.Location = new Point(8, 28);
            _lblStatusLine.AutoSize = true;
            rightStatus.Controls.Add(_lblStatusLine);

            // ---- 左侧标题区 (Fill 填满剩余) ----
            var leftTitle = new Panel();
            leftTitle.Dock = DockStyle.Fill;
            leftTitle.BackColor = AppTheme.PrimaryDark;
            _topBar.Controls.Add(leftTitle);

            _lblTitle = new Label();
            _lblTitle.Text = "基于北斗时空数据的深远海养殖辅助分析终端软件 V1.0";
            _lblTitle.ForeColor = AppTheme.TextPrimary;
            _lblTitle.Font = new Font("微软雅黑", 13F, FontStyle.Bold);
            _lblTitle.Location = new Point(16, 12);
            _lblTitle.AutoSize = true;
            leftTitle.Controls.Add(_lblTitle);

            // 底部分割线
            var line = new Panel();
            line.Dock = DockStyle.Bottom;
            line.Height = 2;
            line.BackColor = AppTheme.PrimaryBlue;
            _topBar.Controls.Add(line);
        }

        /// <summary>
        /// 根布局：TableLayoutPanel 三列 (180px | 100% | 250px)
        /// </summary>
        private void InitializeRootTable()
        {
            _rootTable = new TableLayoutPanel();
            _rootTable.Dock = DockStyle.Fill;
            _rootTable.ColumnCount = 3;
            _rootTable.RowCount = 1;
            _rootTable.BackColor = AppTheme.BackgroundDark;
            _rootTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            // 列宽：左导航固定180，右侧固定250，中间自适应
            _rootTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));   // 左导航
            _rootTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));     // 主内容
            _rootTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));    // 右侧
            _rootTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rootTable.Padding = new Padding(0);
            _rootTable.Margin = new Padding(0);

            this.Controls.Add(_rootTable);
        }

        /// <summary>
        /// 左侧导航栏
        /// </summary>
        private void InitializeLeftNav()
        {
            _leftNav = new Panel();
            _leftNav.Dock = DockStyle.Fill;
            _leftNav.BackColor = AppTheme.BackgroundPanel;
            _rootTable.Controls.Add(_leftNav, 0, 0);

            // 导航标题
            var lblNavTitle = new Label();
            lblNavTitle.Text = "功能模块";
            lblNavTitle.ForeColor = AppTheme.AccentCyan;
            lblNavTitle.Font = AppTheme.HeaderFont;
            lblNavTitle.Location = new Point(14, 8);
            lblNavTitle.AutoSize = true;
            _leftNav.Controls.Add(lblNavTitle);

            // 分割线
            var line = new Panel();
            line.Location = new Point(10, 32);
            line.Size = new Size(160, 1);
            line.BackColor = AppTheme.BorderColor;
            _leftNav.Controls.Add(line);

            // 导航按钮
            _navButtons = new Button[10];
            for (int i = 0; i < 10; i++)
            {
                var btn = new Button();
                btn.Text = "  " + _moduleNames[i];
                btn.Location = new Point(0, 38 + i * 42);
                btn.Size = new Size(180, 40);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = AppTheme.BackgroundLight;
                btn.BackColor = AppTheme.BackgroundPanel;
                btn.ForeColor = AppTheme.TextSecondary;
                btn.Font = AppTheme.ContentFont;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(16, 0, 0, 0);
                btn.Tag = i;
                btn.Cursor = Cursors.Hand;
                btn.Click += NavButton_Click;
                _leftNav.Controls.Add(btn);
                _navButtons[i] = btn;
            }

            // 底部版本信息
            _lblVersion = new Label();
            _lblVersion.Text = "V1.0 | 北斗时空数据平台";
            _lblVersion.ForeColor = AppTheme.TextMuted;
            _lblVersion.Font = new Font("微软雅黑", 7F);
            _lblVersion.AutoSize = true;
            _leftNav.Controls.Add(_lblVersion);

            // 右边框
            var rightLine = new Panel();
            rightLine.Dock = DockStyle.Right;
            rightLine.Width = 1;
            rightLine.BackColor = AppTheme.BorderColor;
            _leftNav.Controls.Add(rightLine);

            // 版本号跟随高度
            _leftNav.SizeChanged += (s, e) =>
            {
                _lblVersion.Location = new Point(14, _leftNav.Height - 24);
            };
        }

        /// <summary>
        /// 主内容区 - AutoScroll 允许小屏滚动
        /// </summary>
        private void InitializeMainContent()
        {
            _mainContent = new Panel();
            _mainContent.Dock = DockStyle.Fill;
            _mainContent.BackColor = AppTheme.BackgroundDark;
            _mainContent.AutoScroll = true;
            _rootTable.Controls.Add(_mainContent, 1, 0);
        }

        /// <summary>
        /// 右侧详情/状态面板
        /// </summary>
        private void InitializeRightPanel()
        {
            _rightPanel = new Panel();
            _rightPanel.Dock = DockStyle.Fill;
            _rightPanel.BackColor = AppTheme.BackgroundPanel;
            _rootTable.Controls.Add(_rightPanel, 2, 0);

            // 左边框
            _rightDividerLine = new Panel();
            _rightDividerLine.Dock = DockStyle.Left;
            _rightDividerLine.Width = 1;
            _rightDividerLine.BackColor = AppTheme.BorderColor;
            _rightPanel.Controls.Add(_rightDividerLine);

            // 当前模块标题
            _lblCurrentModule = new Label();
            _lblCurrentModule.Text = "养殖池整体监控";
            _lblCurrentModule.ForeColor = AppTheme.AccentCyan;
            _lblCurrentModule.Font = AppTheme.HeaderFont;
            _lblCurrentModule.Location = new Point(10, 8);
            _lblCurrentModule.AutoSize = true;
            _rightPanel.Controls.Add(_lblCurrentModule);

            // 分割线
            _dividerLine = new Panel();
            _dividerLine.Location = new Point(10, 30);
            _dividerLine.Size = new Size(228, 1);
            _dividerLine.BackColor = AppTheme.BorderColor;
            _rightPanel.Controls.Add(_dividerLine);
        }

        /// <summary>
        /// 计时器 - 更新北斗时间
        /// </summary>
        private void InitializeTimer()
        {
            var timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                _lblBeiDouTime.Text = "北斗授时: " + DemoData.GetBeiDouTime();
            };
            timer.Start();
        }

        /// <summary>
        /// 轻量通知条 (Toast)
        /// </summary>
        private void InitializeToast()
        {
            _toastBar = new Panel();
            _toastBar.Dock = DockStyle.Bottom;
            _toastBar.Height = 0;
            _toastBar.BackColor = AppTheme.PrimaryBlue;
            this.Controls.Add(_toastBar);

            _toastLabel = new Label();
            _toastLabel.ForeColor = AppTheme.TextPrimary;
            _toastLabel.Font = AppTheme.ContentFont;
            _toastLabel.AutoSize = true;
            _toastLabel.Location = new Point(12, 6);
            _toastBar.Controls.Add(_toastLabel);

            _toastTimer = new Timer();
            _toastTimer.Interval = 2000;
            _toastTimer.Tick += (s, e) =>
            {
                _toastBar.Height = 0;
                _toastTimer.Stop();
            };
        }

        /// <summary>
        /// 显示轻量通知
        /// </summary>
        private void ShowToast(string message)
        {
            _toastLabel.Text = "  " + message;
            _toastBar.Height = 26;
            _toastTimer.Stop();
            _toastTimer.Start();
        }

        /// <summary>
        /// 导航按钮点击事件
        /// </summary>
        private void NavButton_Click(object sender, EventArgs e)
        {
            int index = (int)((Button)sender).Tag;
            SwitchModule(index);
        }

        /// <summary>
        /// 切换模块
        /// </summary>
        private void SwitchModule(int index)
        {
            // 更新导航样式
            for (int i = 0; i < _navButtons.Length; i++)
            {
                if (i == index)
                    AppTheme.ActivateNavButton(_navButtons[i]);
                else
                    AppTheme.DeactivateNavButton(_navButtons[i]);
            }

            _activeModule = index;
            _lblCurrentModule.Text = _moduleNames[index];

            // 清空主内容区
            _mainContent.Controls.Clear();

            // 清空右侧面板 - 保留边框、标题、分割线
            var keepControls = new Control[] { _rightDividerLine, _lblCurrentModule, _dividerLine };
            _rightPanel.Controls.Clear();
            foreach (var c in keepControls)
                _rightPanel.Controls.Add(c);

            // 加载对应模块
            switch (index)
            {
                case 0: LoadPoolMonitor(); break;
                case 1: LoadVideoMonitor(); break;
                case 2: LoadDiseaseControl(); break;
                case 3: LoadProfitAnalysis(); break;
                case 4: LoadStandardSetting(); break;
                case 5: LoadRealtimeQuery(); break;
                case 6: LoadTrendChart(); break;
                case 7: LoadHistoryQuery(); break;
                case 8: LoadControlMode(); break;
                case 9: LoadControlSchedule(); break;
            }

            // 滚动到顶部
            if (_mainContent.Controls.Count > 0)
                _mainContent.ScrollControlIntoView(_mainContent.Controls[0]);
        }

        /// <summary>
        /// 内容区实际可用宽度（ClientSize 减去 padding 和 scrollbar 余量）
        /// </summary>
        private int CW
        {
            get
            {
                int w = _mainContent.ClientSize.Width - 24;
                return w > 300 ? w : 300;
            }
        }

        // ============================================================
        // 模块0: 养殖池整体监控
        // ============================================================
        private void LoadPoolMonitor()
        {
            AddPageTitle("养殖池整体监控", "实时监测各养殖池环境参数与鱼群健康状态");
            int cw = CW;

            // 统计卡片 - 用固定5列
            var statsPanel = CreateStatsRow(52, cw);
            AddStatCard(statsPanel, 0, 5, "养殖池总数", "8", "个", AppTheme.AccentCyan);
            AddStatCard(statsPanel, 1, 5, "正常池", "6", "个", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 2, 5, "关注池", "2", "个", AppTheme.AccentOrange);
            AddStatCard(statsPanel, 3, 5, "告警池", "0", "个", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 4, 5, "传感器在线", "24/24", "", AppTheme.AccentCyan);

            // 数据表格
            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 120);
            dgv.Size = new Size(cw, 280);
            dgv.DataSource = DemoData.GetPoolMonitorTable();
            _mainContent.Controls.Add(dgv);

            // 右侧 - 养殖池详情
            var detailCard = AppTheme.CreateCard("A1池详情", 228, 180);
            detailCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(detailCard);

            AppTheme.AddDataRow(detailCard, "品种:", "大黄鱼", 12, 42);
            AppTheme.AddDataRow(detailCard, "水温:", "23.5°C", 12, 62);
            AppTheme.AddDataRow(detailCard, "盐度:", "32.1ppt", 12, 82);
            AppTheme.AddDataRow(detailCard, "溶氧:", "7.2mg/L", 12, 102);
            AppTheme.AddDataRow(detailCard, "健康评分:", "95分", 12, 122, AppTheme.AccentGreen);
            AppTheme.AddDataRow(detailCard, "投喂状态:", "已投喂", 12, 142, AppTheme.AccentGreen);

            var statusCard = AppTheme.CreateCard("北斗定位信息", 228, 100);
            statusCard.Location = new Point(8, 236);
            _rightPanel.Controls.Add(statusCard);

            AppTheme.AddDataRow(statusCard, "经度:", "118.5672°E", 12, 42);
            AppTheme.AddDataRow(statusCard, "纬度:", "24.8765°N", 12, 62);
            AppTheme.AddDataRow(statusCard, "授时精度:", "±20ns", 12, 82);
        }

        // ============================================================
        // 模块1: 实时视频监控
        // ============================================================
        private void LoadVideoMonitor()
        {
            AddPageTitle("实时视频监控", "养殖场视频监控实时画面与云台控制");
            int cw = CW;
            int halfW = (cw - 10) / 2;

            string[] cameraNames = { "A区主摄像头", "B区主摄像头", "C区主摄像头", "外海浮标摄像头" };
            for (int i = 0; i < 4; i++)
            {
                int col = i % 2;
                int row = i / 2;
                var vp = new Panel();
                vp.Location = new Point(12 + col * (halfW + 10), 52 + row * 230);
                vp.Size = new Size(halfW, 216);
                vp.BackColor = Color.FromArgb(10, 15, 25);
                _mainContent.Controls.Add(vp);

                var lblCam = new Label();
                lblCam.Text = cameraNames[i];
                lblCam.ForeColor = AppTheme.AccentCyan;
                lblCam.Font = AppTheme.ContentFont;
                lblCam.Location = new Point(8, 6);
                lblCam.AutoSize = true;
                vp.Controls.Add(lblCam);

                var lblRec = new Label();
                lblRec.Text = "● REC";
                lblRec.ForeColor = AppTheme.AccentRed;
                lblRec.Font = new Font("Consolas", 9F, FontStyle.Bold);
                lblRec.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                lblRec.Location = new Point(halfW - 66, 6);
                lblRec.AutoSize = true;
                vp.Controls.Add(lblRec);

                var pb = new PictureBox();
                pb.Location = new Point(4, 24);
                pb.Size = new Size(halfW - 8, 156);
                pb.BackColor = Color.FromArgb(8, 12, 20);
                pb.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    using (var pen = new Pen(AppTheme.BorderColor, 1))
                    {
                        for (int x = 0; x < pb.Width; x += 40) g.DrawLine(pen, x, 0, x, pb.Height);
                        for (int y = 0; y < pb.Height; y += 40) g.DrawLine(pen, 0, y, pb.Width, y);
                    }
                    int cx = pb.Width / 2, cy = pb.Height / 2;
                    using (var cp = new Pen(AppTheme.AccentCyan, 1))
                    {
                        g.DrawLine(cp, cx - 20, cy, cx + 20, cy);
                        g.DrawLine(cp, cx, cy - 20, cx, cy + 20);
                    }
                    g.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), AppTheme.DataFont, Brushes.Gray, 6, pb.Height - 18);
                };
                vp.Controls.Add(pb);

                var lblTime = new Label();
                lblTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | 1920x1080 | 25fps";
                lblTime.ForeColor = AppTheme.TextMuted;
                lblTime.Font = new Font("微软雅黑", 7F);
                lblTime.Location = new Point(8, 186);
                lblTime.AutoSize = true;
                vp.Controls.Add(lblTime);
            }

            // 右侧 - 云台控制
            var ptzCard = AppTheme.CreateCard("云台控制", 228, 168);
            ptzCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(ptzCard);

            string[] ptzLabels = { "↑上", "←左", "归位", "→右", "↓下", "放大", "缩小", "聚焦+" };
            for (int i = 0; i < ptzLabels.Length; i++)
            {
                var btn = new Button();
                btn.Text = ptzLabels[i];
                btn.Size = new Size(62, 26);
                btn.Location = new Point(12 + (i % 3) * 68, 40 + (i / 3) * 32);
                btn.BackColor = AppTheme.BackgroundLight;
                btn.ForeColor = AppTheme.TextPrimary;
                btn.FlatStyle = FlatStyle.Flat;
                btn.Font = AppTheme.SmallFont;
                btn.Cursor = Cursors.Hand;
                string label = ptzLabels[i];
                btn.Click += (s, e) => ShowToast("云台指令已发送: " + label);
                ptzCard.Controls.Add(btn);
            }

            var alertCard = AppTheme.CreateCard("监控告警", 228, 108);
            alertCard.Location = new Point(8, 224);
            _rightPanel.Controls.Add(alertCard);

            string[] alerts = { "● B1区 溶氧偏低 告警", "● 全部摄像头在线", "● 存储空间剩余: 78.2%" };
            Color[] aColors = { AppTheme.AccentOrange, AppTheme.AccentGreen, AppTheme.TextSecondary };
            for (int i = 0; i < alerts.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = alerts[i];
                lbl.ForeColor = aColors[i];
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 40 + i * 22);
                lbl.AutoSize = true;
                alertCard.Controls.Add(lbl);
            }
        }

        // ============================================================
        // 模块2: 病虫防治
        // ============================================================
        private void LoadDiseaseControl()
        {
            AddPageTitle("病虫防治", "病害检测、风险评估与处理措施跟踪");
            int cw = CW;

            var statsPanel = CreateStatsRow(52, cw);
            AddStatCard(statsPanel, 0, 4, "本月检测", "156", "次", AppTheme.AccentCyan);
            AddStatCard(statsPanel, 1, 4, "已处理", "148", "次", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 2, 4, "处理中", "5", "次", AppTheme.AccentOrange);
            AddStatCard(statsPanel, 3, 4, "高风险", "1", "项", AppTheme.AccentRed);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 120);
            dgv.Size = new Size(cw, 280);
            dgv.DataSource = DemoData.GetDiseaseControlTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var riskCard = AppTheme.CreateCard("风险统计", 228, 120);
            riskCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(riskCard);

            AppTheme.AddDataRow(riskCard, "低风险:", "3项", 12, 42, AppTheme.AccentGreen);
            AppTheme.AddDataRow(riskCard, "中风险:", "2项", 12, 62, AppTheme.AccentOrange);
            AppTheme.AddDataRow(riskCard, "高风险:", "1项", 12, 82, AppTheme.AccentRed);
            AppTheme.AddDataRow(riskCard, "治愈率:", "95.2%", 12, 102, AppTheme.AccentGreen);

            var guideCard = AppTheme.CreateCard("防治指南", 228, 120);
            guideCard.Location = new Point(8, 176);
            _rightPanel.Controls.Add(guideCard);

            string[] tips = { "1. 定期采样检测", "2. 保持水质稳定", "3. 合理投喂密度", "4. 发现异常及时隔离" };
            for (int i = 0; i < tips.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = tips[i];
                lbl.ForeColor = AppTheme.TextSecondary;
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 40 + i * 22);
                lbl.AutoSize = true;
                guideCard.Controls.Add(lbl);
            }
        }

        // ============================================================
        // 模块3: 利润分析
        // ============================================================
        private void LoadProfitAnalysis()
        {
            AddPageTitle("利润分析", "各品种养殖成本、产出与利润率综合分析");
            int cw = CW;

            var statsPanel = CreateStatsRow(52, cw);
            AddStatCard(statsPanel, 0, 4, "总投入", "66.0", "万元", AppTheme.AccentOrange);
            AddStatCard(statsPanel, 1, 4, "预计产出", "98.3", "万元", AppTheme.AccentCyan);
            AddStatCard(statsPanel, 2, 4, "平均利润率", "49.1", "%", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 3, 4, "最优品种", "石斑鱼", "", AppTheme.AccentGreen);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 120);
            dgv.Size = new Size(cw, 190);
            dgv.DataSource = DemoData.GetProfitTable();
            _mainContent.Controls.Add(dgv);

            var chart = AppTheme.CreateBarChart(
                "月度利润趋势(万元)",
                DemoData.GetMonthLabels(),
                DemoData.GetMonthlyProfit(),
                cw, 190);
            chart.Location = new Point(12, 320);
            _mainContent.Controls.Add(chart);

            // 右侧
            var summaryCard = AppTheme.CreateCard("收益概览", 228, 178);
            summaryCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(summaryCard);

            AppTheme.AddDataRow(summaryCard, "本季收入:", "98.3万", 12, 42);
            AppTheme.AddDataRow(summaryCard, "本季成本:", "66.0万", 12, 62);
            AppTheme.AddDataRow(summaryCard, "净利润:", "32.3万", 12, 82, AppTheme.AccentGreen);
            AppTheme.AddDataRow(summaryCard, "同比增幅:", "+12.5%", 12, 102, AppTheme.AccentGreen);
            AppTheme.AddDataRow(summaryCard, "最高利润率:", "60.0%", 12, 122, AppTheme.AccentCyan);
            AppTheme.AddDataRow(summaryCard, "最低存活率:", "82.0%", 12, 142, AppTheme.AccentOrange);
        }

        // ============================================================
        // 模块4: 标准值设定
        // ============================================================
        private void LoadStandardSetting()
        {
            AddPageTitle("标准值设定", "水质参数标准范围与告警阈值配置");
            int cw = CW;

            var btnPanel = new Panel();
            btnPanel.Location = new Point(12, 50);
            btnPanel.Size = new Size(cw, 32);
            _mainContent.Controls.Add(btnPanel);

            var btnAdd = CreateActionButton("新增参数", AppTheme.AccentCyan);
            btnAdd.Location = new Point(0, 2);
            btnAdd.Click += (s, e) => ShowToast("已打开新增参数对话框");
            btnPanel.Controls.Add(btnAdd);

            var btnEdit = CreateActionButton("编辑选中", AppTheme.PrimaryLight);
            btnEdit.Location = new Point(92, 2);
            btnEdit.Click += (s, e) => ShowToast("已打开编辑参数对话框");
            btnPanel.Controls.Add(btnEdit);

            var btnDel = CreateActionButton("删除选中", AppTheme.AccentRed);
            btnDel.Location = new Point(184, 2);
            btnDel.Click += (s, e) => ShowToast("请确认删除选中参数");
            btnPanel.Controls.Add(btnDel);

            var btnExport = CreateActionButton("导出配置", AppTheme.BackgroundLight);
            btnExport.Location = new Point(276, 2);
            btnExport.Click += (s, e) => ShowToast("配置已导出到文件");
            btnPanel.Controls.Add(btnExport);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 90);
            dgv.Size = new Size(cw, 320);
            dgv.DataSource = DemoData.GetStandardTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var alertCard = AppTheme.CreateCard("告警级别说明", 228, 178);
            alertCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(alertCard);

            string[] alertInfo = {
                "一级告警: 立即处理",
                "  → 溶氧/氨氮超标",
                "二级告警: 密切关注",
                "  → 水温/pH异常",
                "三级告警: 记录提醒",
                "  → 透明度/其他"
            };
            for (int i = 0; i < alertInfo.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = alertInfo[i];
                lbl.ForeColor = i % 2 == 0 ? AppTheme.TextPrimary : AppTheme.TextMuted;
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 40 + i * 22);
                lbl.AutoSize = true;
                alertCard.Controls.Add(lbl);
            }
        }

        // ============================================================
        // 模块5: 数据实时查询
        // ============================================================
        private void LoadRealtimeQuery()
        {
            AddPageTitle("数据实时查询", "传感器实时采集数据与北斗时空数据查询");
            int cw = CW;

            var queryPanel = new Panel();
            queryPanel.Location = new Point(12, 50);
            queryPanel.Size = new Size(cw, 36);
            queryPanel.BackColor = AppTheme.BackgroundCard;
            _mainContent.Controls.Add(queryPanel);

            var lblPool = new Label();
            lblPool.Text = "池号:";
            lblPool.ForeColor = AppTheme.TextSecondary;
            lblPool.Font = AppTheme.ContentFont;
            lblPool.Location = new Point(8, 8);
            lblPool.AutoSize = true;
            queryPanel.Controls.Add(lblPool);

            var cbPool = new ComboBox();
            cbPool.Items.AddRange(new string[] { "全部", "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2" });
            cbPool.SelectedIndex = 0;
            cbPool.Location = new Point(40, 6);
            cbPool.Size = new Size(66, 24);
            cbPool.Font = AppTheme.SmallFont;
            queryPanel.Controls.Add(cbPool);

            var lblParam = new Label();
            lblParam.Text = "参数:";
            lblParam.ForeColor = AppTheme.TextSecondary;
            lblParam.Font = AppTheme.ContentFont;
            lblParam.Location = new Point(120, 8);
            lblParam.AutoSize = true;
            queryPanel.Controls.Add(lblParam);

            var cbParam = new ComboBox();
            cbParam.Items.AddRange(new string[] { "全部", "水温", "盐度", "溶氧", "pH", "氨氮" });
            cbParam.SelectedIndex = 0;
            cbParam.Location = new Point(152, 6);
            cbParam.Size = new Size(66, 24);
            cbParam.Font = AppTheme.SmallFont;
            queryPanel.Controls.Add(cbParam);

            var btnQuery = CreateActionButton("查询", AppTheme.AccentCyan);
            btnQuery.Location = new Point(230, 4);
            btnQuery.Size = new Size(60, 26);
            btnQuery.Click += (s, e) => ShowToast("查询完成，共返回 8 条记录");
            queryPanel.Controls.Add(btnQuery);

            var btnRefresh = CreateActionButton("刷新", AppTheme.PrimaryLight);
            btnRefresh.Location = new Point(298, 4);
            btnRefresh.Size = new Size(60, 26);
            btnRefresh.Click += (s, e) => ShowToast("数据已刷新");
            queryPanel.Controls.Add(btnRefresh);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 94);
            dgv.Size = new Size(cw, 300);
            dgv.DataSource = DemoData.GetRealtimeQueryTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var sourceCard = AppTheme.CreateCard("数据来源", 228, 118);
            sourceCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(sourceCard);

            string[] sources = { "● 北斗卫星授时定位", "● 水下传感器阵列", "● 浮标气象站", "● 水质监测探头" };
            for (int i = 0; i < sources.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = sources[i];
                lbl.ForeColor = AppTheme.AccentGreen;
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 40 + i * 22);
                lbl.AutoSize = true;
                sourceCard.Controls.Add(lbl);
            }
        }

        // ============================================================
        // 模块6: 趋势图查询
        // ============================================================
        private void LoadTrendChart()
        {
            AddPageTitle("趋势图查询", "水质参数趋势分析与多维度数据对比");
            int cw = CW;

            var queryPanel = new Panel();
            queryPanel.Location = new Point(12, 50);
            queryPanel.Size = new Size(cw, 30);
            queryPanel.BackColor = AppTheme.BackgroundCard;
            _mainContent.Controls.Add(queryPanel);

            var lblPool = new Label();
            lblPool.Text = "池号: A1    日期: 2026-06-09    参数: 全部";
            lblPool.ForeColor = AppTheme.TextSecondary;
            lblPool.Font = AppTheme.ContentFont;
            lblPool.Location = new Point(8, 6);
            lblPool.AutoSize = true;
            queryPanel.Controls.Add(lblPool);

            var tempChart = AppTheme.CreateLineChart(
                "水温趋势 (°C) - A1池",
                DemoData.GetTrendTimeLabels(),
                DemoData.GetTrendWaterTemp(),
                cw, 190);
            tempChart.Location = new Point(12, 88);
            _mainContent.Controls.Add(tempChart);

            var oxygenChart = AppTheme.CreateLineChart(
                "溶解氧趋势 (mg/L) - A1池",
                DemoData.GetTrendTimeLabels(),
                DemoData.GetTrendDissolvedOxygen(),
                cw, 190);
            oxygenChart.Location = new Point(12, 290);
            _mainContent.Controls.Add(oxygenChart);

            // 右侧
            var statCard = AppTheme.CreateCard("趋势统计", 228, 178);
            statCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(statCard);

            AppTheme.AddDataRow(statCard, "水温最高:", "24.5°C", 12, 42, AppTheme.AccentOrange);
            AppTheme.AddDataRow(statCard, "水温最低:", "22.0°C", 12, 62, AppTheme.AccentCyan);
            AppTheme.AddDataRow(statCard, "水温均值:", "23.26°C", 12, 82);
            AppTheme.AddDataRow(statCard, "溶氧最高:", "7.8mg/L", 12, 102);
            AppTheme.AddDataRow(statCard, "溶氧最低:", "6.5mg/L", 12, 122, AppTheme.AccentOrange);
            AppTheme.AddDataRow(statCard, "趋势判断:", "正常波动", 12, 142, AppTheme.AccentGreen);
        }

        // ============================================================
        // 模块7: 历史数据查询
        // ============================================================
        private void LoadHistoryQuery()
        {
            AddPageTitle("历史数据查询", "历史水质监测数据查询与导出");
            int cw = CW;

            var queryPanel = new Panel();
            queryPanel.Location = new Point(12, 50);
            queryPanel.Size = new Size(cw, 36);
            queryPanel.BackColor = AppTheme.BackgroundCard;
            _mainContent.Controls.Add(queryPanel);

            var lblQ = new Label();
            lblQ.Text = "池号: A1    起始: 2026-06-08    结束: 2026-06-09";
            lblQ.ForeColor = AppTheme.TextSecondary;
            lblQ.Font = AppTheme.ContentFont;
            lblQ.Location = new Point(8, 9);
            lblQ.AutoSize = true;
            queryPanel.Controls.Add(lblQ);

            var btnExport = CreateActionButton("导出Excel", AppTheme.AccentCyan);
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.Location = new Point(cw - 156, 4);
            btnExport.Size = new Size(72, 26);
            btnExport.Click += (s, e) => ShowToast("历史数据已导出为 Excel 文件");
            queryPanel.Controls.Add(btnExport);

            var btnPrint = CreateActionButton("打印报表", AppTheme.PrimaryLight);
            btnPrint.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrint.Location = new Point(cw - 76, 4);
            btnPrint.Size = new Size(72, 26);
            btnPrint.Click += (s, e) => ShowToast("报表已发送到打印机");
            queryPanel.Controls.Add(btnPrint);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 94);
            dgv.Size = new Size(cw, 360);
            dgv.DataSource = DemoData.GetHistoryTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var summaryCard = AppTheme.CreateCard("查询统计", 228, 178);
            summaryCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(summaryCard);

            AppTheme.AddDataRow(summaryCard, "记录总数:", "48条", 12, 42);
            AppTheme.AddDataRow(summaryCard, "时间跨度:", "48小时", 12, 62);
            AppTheme.AddDataRow(summaryCard, "采样间隔:", "2小时", 12, 82);
            AppTheme.AddDataRow(summaryCard, "异常记录:", "0条", 12, 102, AppTheme.AccentGreen);
            AppTheme.AddDataRow(summaryCard, "数据完整性:", "100%", 12, 122, AppTheme.AccentGreen);
        }

        // ============================================================
        // 模块8: 控制形式设置
        // ============================================================
        private void LoadControlMode()
        {
            AddPageTitle("控制形式设置", "养殖设备控制模式配置与运行状态管理");
            int cw = CW;

            var btnPanel = new Panel();
            btnPanel.Location = new Point(12, 50);
            btnPanel.Size = new Size(cw, 32);
            _mainContent.Controls.Add(btnPanel);

            var btnBatch = CreateActionButton("批量设置", AppTheme.AccentCyan);
            btnBatch.Location = new Point(0, 2);
            btnBatch.Click += (s, e) => ShowToast("已进入批量设置模式");
            btnPanel.Controls.Add(btnBatch);

            var btnAuto = CreateActionButton("全部自动", AppTheme.AccentGreen);
            btnAuto.Location = new Point(92, 2);
            btnAuto.Click += (s, e) => ShowToast("已将全部设备切换为自动模式");
            btnPanel.Controls.Add(btnAuto);

            var btnManual = CreateActionButton("全部手动", AppTheme.AccentOrange);
            btnManual.Location = new Point(184, 2);
            btnManual.Click += (s, e) => ShowToast("已将全部设备切换为手动模式");
            btnPanel.Controls.Add(btnManual);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 90);
            dgv.Size = new Size(cw, 320);
            dgv.DataSource = DemoData.GetControlModeTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var modeCard = AppTheme.CreateCard("控制模式说明", 228, 178);
            modeCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(modeCard);

            string[] modes = {
                "自动模式:",
                "  系统根据传感器数据",
                "  自动调节设备运行",
                "",
                "手动模式:",
                "  由操作员手动控制",
                "  设备开关与功率",
                "",
                "定时模式:",
                "  按预设时间计划",
                "  自动执行任务"
            };
            for (int i = 0; i < modes.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = modes[i];
                lbl.ForeColor = modes[i].EndsWith(":") ? AppTheme.AccentCyan : AppTheme.TextMuted;
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 40 + i * 14);
                lbl.AutoSize = true;
                modeCard.Controls.Add(lbl);
            }
        }

        // ============================================================
        // 模块9: 控制时间设置
        // ============================================================
        private void LoadControlSchedule()
        {
            AddPageTitle("控制时间设置", "设备定时任务计划与执行周期管理");
            int cw = CW;

            var btnPanel = new Panel();
            btnPanel.Location = new Point(12, 50);
            btnPanel.Size = new Size(cw, 32);
            _mainContent.Controls.Add(btnPanel);

            var btnNew = CreateActionButton("新建任务", AppTheme.AccentCyan);
            btnNew.Location = new Point(0, 2);
            btnNew.Click += (s, e) => ShowToast("已打开新建任务对话框");
            btnPanel.Controls.Add(btnNew);

            var btnEditTask = CreateActionButton("编辑任务", AppTheme.PrimaryLight);
            btnEditTask.Location = new Point(92, 2);
            btnEditTask.Click += (s, e) => ShowToast("已打开编辑任务对话框");
            btnPanel.Controls.Add(btnEditTask);

            var btnToggle = CreateActionButton("启用/停用", AppTheme.AccentGreen);
            btnToggle.Location = new Point(184, 2);
            btnToggle.Click += (s, e) => ShowToast("任务状态已切换");
            btnPanel.Controls.Add(btnToggle);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(12, 90);
            dgv.Size = new Size(cw, 320);
            dgv.DataSource = DemoData.GetControlScheduleTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var todayCard = AppTheme.CreateCard("今日任务", 228, 178);
            todayCard.Location = new Point(8, 44);
            _rightPanel.Controls.Add(todayCard);

            string[] todayTasks = {
                "06:30  晨间投喂 ✓",
                "08:00  水质采样 ✓",
                "09:00  数据上报 ✓",
                "11:30  午间投喂 ✓",
                "14:30  水质采样 ○",
                "17:30  晚间投喂 ○",
                "18:00  夜间照明 ○"
            };
            for (int i = 0; i < todayTasks.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = todayTasks[i];
                lbl.ForeColor = todayTasks[i].Contains("✓") ? AppTheme.AccentGreen : AppTheme.TextSecondary;
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 40 + i * 20);
                lbl.AutoSize = true;
                todayCard.Controls.Add(lbl);
            }
        }

        // ============================================================
        // 公共UI辅助方法
        // ============================================================

        private void AddPageTitle(string title, string description)
        {
            var lbl = new Label();
            lbl.Text = title;
            lbl.ForeColor = AppTheme.TextPrimary;
            lbl.Font = AppTheme.SubTitleFont;
            lbl.Location = new Point(12, 6);
            lbl.AutoSize = true;
            _mainContent.Controls.Add(lbl);

            var desc = new Label();
            desc.Text = description;
            desc.ForeColor = AppTheme.TextMuted;
            desc.Font = AppTheme.SmallFont;
            desc.Location = new Point(12, 28);
            desc.AutoSize = true;
            _mainContent.Controls.Add(desc);
        }

        /// <summary>
        /// 创建统计卡片行容器
        /// </summary>
        private Panel CreateStatsRow(int top, int width)
        {
            var p = new Panel();
            p.Location = new Point(12, top);
            p.Size = new Size(width, 58);
            _mainContent.Controls.Add(p);
            return p;
        }

        /// <summary>
        /// 创建统计卡片 - totalCards 参数固定传入，不依赖 Controls.Count
        /// </summary>
        private void AddStatCard(Panel parent, int index, int totalCards, string label, string value, string unit, Color valueColor)
        {
            int gap = 4;
            int cardWidth = (parent.Width - gap * (totalCards + 1)) / totalCards;
            if (cardWidth < 80) cardWidth = 80;

            var card = new Panel();
            card.Location = new Point(gap + index * (cardWidth + gap), 0);
            card.Size = new Size(cardWidth, 54);
            card.BackColor = AppTheme.BackgroundCard;
            parent.Controls.Add(card);

            var lbl = new Label();
            lbl.Text = label;
            lbl.ForeColor = AppTheme.TextMuted;
            lbl.Font = AppTheme.SmallFont;
            lbl.Location = new Point(8, 4);
            lbl.AutoSize = true;
            card.Controls.Add(lbl);

            var val = new Label();
            val.Text = value + (string.IsNullOrEmpty(unit) ? "" : " " + unit);
            val.ForeColor = valueColor;
            val.Font = new Font("Consolas", 12F, FontStyle.Bold);
            val.Location = new Point(8, 24);
            val.AutoSize = true;
            card.Controls.Add(val);
        }

        private DataGridView CreateStyledDataGridView()
        {
            var dgv = new DataGridView();
            dgv.BackgroundColor = AppTheme.BackgroundCard;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.GridColor = AppTheme.BorderColor;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.PrimaryMedium;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.AccentCyan;
            dgv.ColumnHeadersDefaultCellStyle.Font = AppTheme.ContentFont;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = AppTheme.PrimaryMedium;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeight = 28;

            dgv.DefaultCellStyle.BackColor = AppTheme.BackgroundCard;
            dgv.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
            dgv.DefaultCellStyle.Font = AppTheme.ContentFont;
            dgv.DefaultCellStyle.SelectionBackColor = AppTheme.PrimaryBlue;
            dgv.DefaultCellStyle.SelectionForeColor = AppTheme.TextPrimary;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 42, 68);
            dgv.RowHeadersVisible = false;

            return dgv;
        }

        private Button CreateActionButton(string text, Color bgColor)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Size = new Size(84, 26);
            btn.BackColor = bgColor;
            btn.ForeColor = AppTheme.TextPrimary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = AppTheme.SmallFont;
            btn.Cursor = Cursors.Hand;
            return btn;
        }
    }
}
