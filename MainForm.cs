using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DeepSeaAquacultureTerminal
{
    public class MainForm : Form
    {
        // 界面元素
        private Panel _topBar;
        private Panel _leftNav;
        private Panel _mainContent;
        private Panel _rightPanel;
        private Label _lblTitle;
        private Label _lblBeiDouTime;
        private Label _lblStatusLine;
        private Label _lblCurrentModule;
        private Panel _dividerLine;
        private Panel _rightDividerLine;
        private Timer _timer;
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
            // WinForms Dock 顺序：先加非 Fill 的，最后加 Fill 的
            InitializeRightPanel();
            InitializeLeftNav();
            InitializeMainContent();
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
            this.Size = new Size(1440, 900);
            this.MinimumSize = new Size(1024, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppTheme.BackgroundDark;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Icon = null;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 顶部状态栏 - 右侧控件用 Anchor 跟随
        /// </summary>
        private void InitializeTopBar()
        {
            _topBar = new Panel();
            _topBar.Dock = DockStyle.Top;
            _topBar.Height = 56;
            _topBar.BackColor = AppTheme.PrimaryDark;
            this.Controls.Add(_topBar);

            // 软件标题
            _lblTitle = new Label();
            _lblTitle.Text = "基于北斗时空数据的深远海养殖辅助分析终端软件 V1.0";
            _lblTitle.ForeColor = AppTheme.TextPrimary;
            _lblTitle.Font = new Font("微软雅黑", 14F, FontStyle.Bold);
            _lblTitle.Location = new Point(16, 6);
            _lblTitle.AutoSize = true;
            _topBar.Controls.Add(_lblTitle);

            // 北斗时间 - Anchor 到右上
            _lblBeiDouTime = new Label();
            _lblBeiDouTime.Text = "北斗授时: " + DemoData.GetBeiDouTime();
            _lblBeiDouTime.ForeColor = AppTheme.AccentCyan;
            _lblBeiDouTime.Font = AppTheme.DataFont;
            _lblBeiDouTime.Location = new Point(_topBar.Width - 280, 6);
            _lblBeiDouTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _lblBeiDouTime.AutoSize = true;
            _topBar.Controls.Add(_lblBeiDouTime);

            // 状态指示 - Anchor 到右下
            _lblStatusLine = new Label();
            _lblStatusLine.Text = "● 系统在线 | 传感器: 24/24 | 北斗信号: 正常";
            _lblStatusLine.ForeColor = AppTheme.AccentGreen;
            _lblStatusLine.Font = AppTheme.SmallFont;
            _lblStatusLine.Location = new Point(_topBar.Width - 280, 30);
            _lblStatusLine.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _lblStatusLine.AutoSize = true;
            _topBar.Controls.Add(_lblStatusLine);

            // 底部分割线
            var line = new Panel();
            line.Dock = DockStyle.Bottom;
            line.Height = 2;
            line.BackColor = AppTheme.PrimaryBlue;
            _topBar.Controls.Add(line);
        }

        /// <summary>
        /// 左侧导航栏 - 收窄到 180px
        /// </summary>
        private void InitializeLeftNav()
        {
            _leftNav = new Panel();
            _leftNav.Dock = DockStyle.Left;
            _leftNav.Width = 180;
            _leftNav.BackColor = AppTheme.BackgroundPanel;
            this.Controls.Add(_leftNav);

            // 导航标题
            var lblNavTitle = new Label();
            lblNavTitle.Text = "功能模块";
            lblNavTitle.ForeColor = AppTheme.AccentCyan;
            lblNavTitle.Font = AppTheme.HeaderFont;
            lblNavTitle.Location = new Point(14, 10);
            lblNavTitle.AutoSize = true;
            _leftNav.Controls.Add(lblNavTitle);

            // 分割线
            var line = new Panel();
            line.Location = new Point(10, 36);
            line.Size = new Size(160, 1);
            line.BackColor = AppTheme.BorderColor;
            _leftNav.Controls.Add(line);

            // 导航按钮
            _navButtons = new Button[10];
            for (int i = 0; i < 10; i++)
            {
                var btn = new Button();
                btn.Text = "  " + _moduleNames[i];
                btn.Location = new Point(0, 42 + i * 42);
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

            // 首次布局：版本号定位到底部
            _leftNav.SizeChanged += (s, e) =>
            {
                _lblVersion.Location = new Point(14, _leftNav.Height - 26);
            };
        }

        /// <summary>
        /// 右侧详情/状态面板 - 收窄到 250px
        /// </summary>
        private void InitializeRightPanel()
        {
            _rightPanel = new Panel();
            _rightPanel.Dock = DockStyle.Right;
            _rightPanel.Width = 250;
            _rightPanel.BackColor = AppTheme.BackgroundPanel;
            this.Controls.Add(_rightPanel);

            // 当前模块标题
            _lblCurrentModule = new Label();
            _lblCurrentModule.Text = "养殖池整体监控";
            _lblCurrentModule.ForeColor = AppTheme.AccentCyan;
            _lblCurrentModule.Font = AppTheme.HeaderFont;
            _lblCurrentModule.Location = new Point(10, 10);
            _lblCurrentModule.AutoSize = true;
            _rightPanel.Controls.Add(_lblCurrentModule);

            // 分割线（用字段引用以便模块切换时重定位）
            _dividerLine = new Panel();
            _dividerLine.Location = new Point(10, 32);
            _dividerLine.Size = new Size(230, 1);
            _dividerLine.BackColor = AppTheme.BorderColor;
            _rightPanel.Controls.Add(_dividerLine);

            // 左边框
            _rightDividerLine = new Panel();
            _rightDividerLine.Dock = DockStyle.Left;
            _rightDividerLine.Width = 1;
            _rightDividerLine.BackColor = AppTheme.BorderColor;
            _rightPanel.Controls.Add(_rightDividerLine);
        }

        /// <summary>
        /// 主内容区 - 必须最后添加，带 AutoScroll
        /// </summary>
        private void InitializeMainContent()
        {
            _mainContent = new Panel();
            _mainContent.Dock = DockStyle.Fill;
            _mainContent.BackColor = AppTheme.BackgroundDark;
            _mainContent.AutoScroll = true;
            this.Controls.Add(_mainContent);
        }

        /// <summary>
        /// 计时器 - 更新北斗时间
        /// </summary>
        private void InitializeTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += (s, e) =>
            {
                _lblBeiDouTime.Text = "北斗授时: " + DemoData.GetBeiDouTime();
            };
            _timer.Start();
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
            _toastBar.Height = 28;
            _toastTimer.Stop();
            _toastTimer.Start();
        }

        /// <summary>
        /// 导航按钮点击事件
        /// </summary>
        private void NavButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int index = (int)btn.Tag;
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

            // 清空内容 - 保留固定控件
            _mainContent.Controls.Clear();

            // 清空右侧面板 - 保留标题、分割线和边框
            _rightPanel.Controls.Clear();
            _rightPanel.Controls.Add(_lblCurrentModule);
            _rightPanel.Controls.Add(_dividerLine);
            _rightPanel.Controls.Add(_rightDividerLine);

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

            _mainContent.ScrollControlIntoView(_mainContent.Controls.Count > 0
                ? _mainContent.Controls[0] : null);
        }

        // ============================================================
        // 获取内容区实际可用宽度（减去 Padding 和 ScrollBar）
        // ============================================================
        private int ContentWidth
        {
            get
            {
                int w = _mainContent.ClientSize.Width - 32;
                return w > 200 ? w : 200;
            }
        }

        // ============================================================
        // 模块0: 养殖池整体监控
        // ============================================================
        private void LoadPoolMonitor()
        {
            AddPageTitle("养殖池整体监控", "实时监测各养殖池环境参数与鱼群健康状态");

            var cw = ContentWidth;

            // 顶部统计卡片
            var statsPanel = new Panel();
            statsPanel.Location = new Point(16, 56);
            statsPanel.Size = new Size(cw, 72);
            statsPanel.BackColor = AppTheme.BackgroundDark;
            _mainContent.Controls.Add(statsPanel);

            AddStatCard(statsPanel, 0, "养殖池总数", "8", "个", AppTheme.AccentCyan);
            AddStatCard(statsPanel, 1, "正常池", "6", "个", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 2, "关注池", "2", "个", AppTheme.AccentOrange);
            AddStatCard(statsPanel, 3, "告警池", "0", "个", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 4, "传感器在线", "24/24", "", AppTheme.AccentCyan);

            // 数据表格
            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 138);
            dgv.Size = new Size(cw, 300);
            dgv.DataSource = DemoData.GetPoolMonitorTable();
            _mainContent.Controls.Add(dgv);

            // 右侧面板 - 养殖池详情
            var detailCard = AppTheme.CreateCard("A1池详情", 228, 190);
            detailCard.Location = new Point(10, 50);
            _rightPanel.Controls.Add(detailCard);

            AppTheme.AddDataRow(detailCard, "品种:", "大黄鱼", 12, 42);
            AppTheme.AddDataRow(detailCard, "水温:", "23.5°C", 12, 63);
            AppTheme.AddDataRow(detailCard, "盐度:", "32.1ppt", 12, 84);
            AppTheme.AddDataRow(detailCard, "溶氧:", "7.2mg/L", 12, 105);
            AppTheme.AddDataRow(detailCard, "健康评分:", "95分", 12, 126, AppTheme.AccentGreen);
            AppTheme.AddDataRow(detailCard, "投喂状态:", "已投喂", 12, 147, AppTheme.AccentGreen);

            var statusCard = AppTheme.CreateCard("北斗定位信息", 228, 110);
            statusCard.Location = new Point(10, 255);
            _rightPanel.Controls.Add(statusCard);

            AppTheme.AddDataRow(statusCard, "经度:", "118.5672°E", 12, 42);
            AppTheme.AddDataRow(statusCard, "纬度:", "24.8765°N", 12, 63);
            AppTheme.AddDataRow(statusCard, "授时精度:", "±20ns", 12, 84);
        }

        // ============================================================
        // 模块1: 实时视频监控
        // ============================================================
        private void LoadVideoMonitor()
        {
            AddPageTitle("实时视频监控", "养殖场视频监控实时画面与云台控制");

            var cw = ContentWidth;
            int halfW = (cw - 12) / 2;

            string[] cameraNames = { "A区主摄像头", "B区主摄像头", "C区主摄像头", "外海浮标摄像头" };
            for (int i = 0; i < 4; i++)
            {
                int col = i % 2;
                int row = i / 2;
                var videoPanel = new Panel();
                videoPanel.Location = new Point(16 + col * (halfW + 12), 56 + row * 240);
                videoPanel.Size = new Size(halfW, 224);
                videoPanel.BackColor = Color.FromArgb(10, 15, 25);
                _mainContent.Controls.Add(videoPanel);

                var lblCam = new Label();
                lblCam.Text = cameraNames[i];
                lblCam.ForeColor = AppTheme.AccentCyan;
                lblCam.Font = AppTheme.ContentFont;
                lblCam.Location = new Point(8, 6);
                lblCam.AutoSize = true;
                videoPanel.Controls.Add(lblCam);

                var lblRec = new Label();
                lblRec.Text = "● REC";
                lblRec.ForeColor = AppTheme.AccentRed;
                lblRec.Font = new Font("Consolas", 9F, FontStyle.Bold);
                lblRec.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                lblRec.Location = new Point(halfW - 68, 6);
                lblRec.AutoSize = true;
                videoPanel.Controls.Add(lblRec);

                var pictureBox = new PictureBox();
                pictureBox.Location = new Point(4, 26);
                pictureBox.Size = new Size(halfW - 8, 164);
                pictureBox.BackColor = Color.FromArgb(8, 12, 20);
                pictureBox.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    using (var pen = new Pen(AppTheme.BorderColor, 1))
                    {
                        for (int x = 0; x < pictureBox.Width; x += 40)
                            g.DrawLine(pen, x, 0, x, pictureBox.Height);
                        for (int y = 0; y < pictureBox.Height; y += 40)
                            g.DrawLine(pen, 0, y, pictureBox.Width, y);
                    }
                    int cx = pictureBox.Width / 2;
                    int cy = pictureBox.Height / 2;
                    using (var crossPen = new Pen(AppTheme.AccentCyan, 1))
                    {
                        g.DrawLine(crossPen, cx - 20, cy, cx + 20, cy);
                        g.DrawLine(crossPen, cx, cy - 20, cx, cy + 20);
                    }
                    g.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        AppTheme.DataFont, Brushes.Gray, 6, pictureBox.Height - 20);
                };
                videoPanel.Controls.Add(pictureBox);

                var lblTime = new Label();
                lblTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | 1920x1080 | 25fps";
                lblTime.ForeColor = AppTheme.TextMuted;
                lblTime.Font = new Font("微软雅黑", 7F);
                lblTime.Location = new Point(8, 196);
                lblTime.AutoSize = true;
                videoPanel.Controls.Add(lblTime);
            }

            // 右侧 - 云台控制
            var ptzCard = AppTheme.CreateCard("云台控制", 228, 180);
            ptzCard.Location = new Point(10, 50);
            _rightPanel.Controls.Add(ptzCard);

            string[] ptzLabels = { "↑上", "←左", "归位", "→右", "↓下", "放大", "缩小", "聚焦+" };
            for (int i = 0; i < ptzLabels.Length; i++)
            {
                var btn = new Button();
                btn.Text = ptzLabels[i];
                btn.Size = new Size(65, 28);
                btn.Location = new Point(14 + (i % 3) * 70, 42 + (i / 3) * 34);
                btn.BackColor = AppTheme.BackgroundLight;
                btn.ForeColor = AppTheme.TextPrimary;
                btn.FlatStyle = FlatStyle.Flat;
                btn.Font = AppTheme.SmallFont;
                btn.Cursor = Cursors.Hand;
                btn.Click += (s, e) => ShowToast("云台指令已发送: " + btn.Text);
                ptzCard.Controls.Add(btn);
            }

            var alertCard = AppTheme.CreateCard("监控告警", 228, 120);
            alertCard.Location = new Point(10, 245);
            _rightPanel.Controls.Add(alertCard);

            string[] alerts = {
                "● B1区 溶氧偏低 告警",
                "● 全部摄像头在线",
                "● 存储空间剩余: 78.2%"
            };
            Color[] alertColors = { AppTheme.AccentOrange, AppTheme.AccentGreen, AppTheme.TextSecondary };
            for (int i = 0; i < alerts.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = alerts[i];
                lbl.ForeColor = alertColors[i];
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 42 + i * 22);
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

            var cw = ContentWidth;

            var statsPanel = new Panel();
            statsPanel.Location = new Point(16, 56);
            statsPanel.Size = new Size(cw, 64);
            _mainContent.Controls.Add(statsPanel);

            AddStatCard(statsPanel, 0, "本月检测", "156", "次", AppTheme.AccentCyan);
            AddStatCard(statsPanel, 1, "已处理", "148", "次", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 2, "处理中", "5", "次", AppTheme.AccentOrange);
            AddStatCard(statsPanel, 3, "高风险", "1", "项", AppTheme.AccentRed);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 130);
            dgv.Size = new Size(cw, 300);
            dgv.DataSource = DemoData.GetDiseaseControlTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var riskCard = AppTheme.CreateCard("风险统计", 228, 130);
            riskCard.Location = new Point(10, 50);
            _rightPanel.Controls.Add(riskCard);

            AppTheme.AddDataRow(riskCard, "低风险:", "3项", 12, 42, AppTheme.AccentGreen);
            AppTheme.AddDataRow(riskCard, "中风险:", "2项", 12, 63, AppTheme.AccentOrange);
            AppTheme.AddDataRow(riskCard, "高风险:", "1项", 12, 84, AppTheme.AccentRed);
            AppTheme.AddDataRow(riskCard, "治愈率:", "95.2%", 12, 105, AppTheme.AccentGreen);

            var guideCard = AppTheme.CreateCard("防治指南", 228, 130);
            guideCard.Location = new Point(10, 195);
            _rightPanel.Controls.Add(guideCard);

            string[] tips = { "1. 定期采样检测", "2. 保持水质稳定", "3. 合理投喂密度", "4. 发现异常及时隔离" };
            for (int i = 0; i < tips.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = tips[i];
                lbl.ForeColor = AppTheme.TextSecondary;
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 42 + i * 22);
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

            var cw = ContentWidth;

            var statsPanel = new Panel();
            statsPanel.Location = new Point(16, 56);
            statsPanel.Size = new Size(cw, 64);
            _mainContent.Controls.Add(statsPanel);

            AddStatCard(statsPanel, 0, "总投入", "66.0", "万元", AppTheme.AccentOrange);
            AddStatCard(statsPanel, 1, "预计产出", "98.3", "万元", AppTheme.AccentCyan);
            AddStatCard(statsPanel, 2, "平均利润率", "49.1", "%", AppTheme.AccentGreen);
            AddStatCard(statsPanel, 3, "最优品种", "石斑鱼", "", AppTheme.AccentGreen);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 130);
            dgv.Size = new Size(cw, 200);
            dgv.DataSource = DemoData.GetProfitTable();
            _mainContent.Controls.Add(dgv);

            var profitChart = AppTheme.CreateBarChart(
                "月度利润趋势(万元)",
                DemoData.GetMonthLabels(),
                DemoData.GetMonthlyProfit(),
                cw, 200);
            profitChart.Location = new Point(16, 342);
            _mainContent.Controls.Add(profitChart);

            // 右侧
            var summaryCard = AppTheme.CreateCard("收益概览", 228, 190);
            summaryCard.Location = new Point(10, 50);
            _rightPanel.Controls.Add(summaryCard);

            AppTheme.AddDataRow(summaryCard, "本季收入:", "98.3万", 12, 42);
            AppTheme.AddDataRow(summaryCard, "本季成本:", "66.0万", 12, 63);
            AppTheme.AddDataRow(summaryCard, "净利润:", "32.3万", 12, 84, AppTheme.AccentGreen);
            AppTheme.AddDataRow(summaryCard, "同比增幅:", "+12.5%", 12, 105, AppTheme.AccentGreen);
            AppTheme.AddDataRow(summaryCard, "最高利润率:", "60.0%", 12, 126, AppTheme.AccentCyan);
            AppTheme.AddDataRow(summaryCard, "最低存活率:", "82.0%", 12, 147, AppTheme.AccentOrange);
        }

        // ============================================================
        // 模块4: 标准值设定
        // ============================================================
        private void LoadStandardSetting()
        {
            AddPageTitle("标准值设定", "水质参数标准范围与告警阈值配置");

            var cw = ContentWidth;

            var btnPanel = new Panel();
            btnPanel.Location = new Point(16, 54);
            btnPanel.Size = new Size(cw, 34);
            _mainContent.Controls.Add(btnPanel);

            var btnAdd = CreateActionButton("新增参数", AppTheme.AccentCyan);
            btnAdd.Location = new Point(0, 2);
            btnAdd.Click += (s, e) => ShowToast("已打开新增参数对话框");
            btnPanel.Controls.Add(btnAdd);

            var btnEdit = CreateActionButton("编辑选中", AppTheme.PrimaryLight);
            btnEdit.Location = new Point(100, 2);
            btnEdit.Click += (s, e) => ShowToast("已打开编辑参数对话框");
            btnPanel.Controls.Add(btnEdit);

            var btnDel = CreateActionButton("删除选中", AppTheme.AccentRed);
            btnDel.Location = new Point(200, 2);
            btnDel.Click += (s, e) => ShowToast("请确认删除选中参数");
            btnPanel.Controls.Add(btnDel);

            var btnExport = CreateActionButton("导出配置", AppTheme.BackgroundLight);
            btnExport.Location = new Point(300, 2);
            btnExport.Click += (s, e) => ShowToast("配置已导出到文件");
            btnPanel.Controls.Add(btnExport);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 96);
            dgv.Size = new Size(cw, 340);
            dgv.DataSource = DemoData.GetStandardTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var alertCard = AppTheme.CreateCard("告警级别说明", 228, 190);
            alertCard.Location = new Point(10, 50);
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
                lbl.Location = new Point(12, 42 + i * 22);
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

            var cw = ContentWidth;

            var queryPanel = new Panel();
            queryPanel.Location = new Point(16, 54);
            queryPanel.Size = new Size(cw, 38);
            queryPanel.BackColor = AppTheme.BackgroundCard;
            _mainContent.Controls.Add(queryPanel);

            var lblPool = new Label();
            lblPool.Text = "池号:";
            lblPool.ForeColor = AppTheme.TextSecondary;
            lblPool.Font = AppTheme.ContentFont;
            lblPool.Location = new Point(10, 9);
            lblPool.AutoSize = true;
            queryPanel.Controls.Add(lblPool);

            var cbPool = new ComboBox();
            cbPool.Items.AddRange(new string[] { "全部", "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2" });
            cbPool.SelectedIndex = 0;
            cbPool.Location = new Point(44, 7);
            cbPool.Size = new Size(72, 24);
            cbPool.Font = AppTheme.SmallFont;
            queryPanel.Controls.Add(cbPool);

            var lblParam = new Label();
            lblParam.Text = "参数:";
            lblParam.ForeColor = AppTheme.TextSecondary;
            lblParam.Font = AppTheme.ContentFont;
            lblParam.Location = new Point(130, 9);
            lblParam.AutoSize = true;
            queryPanel.Controls.Add(lblParam);

            var cbParam = new ComboBox();
            cbParam.Items.AddRange(new string[] { "全部", "水温", "盐度", "溶氧", "pH", "氨氮" });
            cbParam.SelectedIndex = 0;
            cbParam.Location = new Point(164, 7);
            cbParam.Size = new Size(72, 24);
            cbParam.Font = AppTheme.SmallFont;
            queryPanel.Controls.Add(cbParam);

            var btnQuery = CreateActionButton("查询", AppTheme.AccentCyan);
            btnQuery.Location = new Point(250, 5);
            btnQuery.Size = new Size(65, 28);
            btnQuery.Click += (s, e) => ShowToast("查询完成，共返回 8 条记录");
            queryPanel.Controls.Add(btnQuery);

            var btnRefresh = CreateActionButton("刷新", AppTheme.PrimaryLight);
            btnRefresh.Location = new Point(325, 5);
            btnRefresh.Size = new Size(65, 28);
            btnRefresh.Click += (s, e) => ShowToast("数据已刷新");
            queryPanel.Controls.Add(btnRefresh);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 100);
            dgv.Size = new Size(cw, 320);
            dgv.DataSource = DemoData.GetRealtimeQueryTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var sourceCard = AppTheme.CreateCard("数据来源", 228, 130);
            sourceCard.Location = new Point(10, 50);
            _rightPanel.Controls.Add(sourceCard);

            string[] sources = { "● 北斗卫星授时定位", "● 水下传感器阵列", "● 浮标气象站", "● 水质监测探头" };
            for (int i = 0; i < sources.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = sources[i];
                lbl.ForeColor = AppTheme.AccentGreen;
                lbl.Font = AppTheme.SmallFont;
                lbl.Location = new Point(12, 42 + i * 22);
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

            var cw = ContentWidth;

            var queryPanel = new Panel();
            queryPanel.Location = new Point(16, 54);
            queryPanel.Size = new Size(cw, 34);
            queryPanel.BackColor = AppTheme.BackgroundCard;
            _mainContent.Controls.Add(queryPanel);

            var lblPool = new Label();
            lblPool.Text = "池号: A1    日期: 2026-06-09    参数: 全部";
            lblPool.ForeColor = AppTheme.TextSecondary;
            lblPool.Font = AppTheme.ContentFont;
            lblPool.Location = new Point(10, 8);
            lblPool.AutoSize = true;
            queryPanel.Controls.Add(lblPool);

            var tempChart = AppTheme.CreateLineChart(
                "水温趋势 (°C) - A1池",
                DemoData.GetTrendTimeLabels(),
                DemoData.GetTrendWaterTemp(),
                cw, 200);
            tempChart.Location = new Point(16, 96);
            _mainContent.Controls.Add(tempChart);

            var oxygenChart = AppTheme.CreateLineChart(
                "溶解氧趋势 (mg/L) - A1池",
                DemoData.GetTrendTimeLabels(),
                DemoData.GetTrendDissolvedOxygen(),
                cw, 200);
            oxygenChart.Location = new Point(16, 308);
            _mainContent.Controls.Add(oxygenChart);

            // 右侧
            var statCard = AppTheme.CreateCard("趋势统计", 228, 190);
            statCard.Location = new Point(10, 50);
            _rightPanel.Controls.Add(statCard);

            AppTheme.AddDataRow(statCard, "水温最高:", "24.5°C", 12, 42, AppTheme.AccentOrange);
            AppTheme.AddDataRow(statCard, "水温最低:", "22.0°C", 12, 63, AppTheme.AccentCyan);
            AppTheme.AddDataRow(statCard, "水温均值:", "23.26°C", 12, 84);
            AppTheme.AddDataRow(statCard, "溶氧最高:", "7.8mg/L", 12, 105);
            AppTheme.AddDataRow(statCard, "溶氧最低:", "6.5mg/L", 12, 126, AppTheme.AccentOrange);
            AppTheme.AddDataRow(statCard, "趋势判断:", "正常波动", 12, 147, AppTheme.AccentGreen);
        }

        // ============================================================
        // 模块7: 历史数据查询
        // ============================================================
        private void LoadHistoryQuery()
        {
            AddPageTitle("历史数据查询", "历史水质监测数据查询与导出");

            var cw = ContentWidth;

            var queryPanel = new Panel();
            queryPanel.Location = new Point(16, 54);
            queryPanel.Size = new Size(cw, 38);
            queryPanel.BackColor = AppTheme.BackgroundCard;
            _mainContent.Controls.Add(queryPanel);

            var lblQ = new Label();
            lblQ.Text = "池号: A1    起始: 2026-06-08    结束: 2026-06-09";
            lblQ.ForeColor = AppTheme.TextSecondary;
            lblQ.Font = AppTheme.ContentFont;
            lblQ.Location = new Point(10, 10);
            lblQ.AutoSize = true;
            queryPanel.Controls.Add(lblQ);

            var btnExport = CreateActionButton("导出Excel", AppTheme.AccentCyan);
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.Location = new Point(cw - 170, 5);
            btnExport.Size = new Size(78, 28);
            btnExport.Click += (s, e) => ShowToast("历史数据已导出为 Excel 文件");
            queryPanel.Controls.Add(btnExport);

            var btnPrint = CreateActionButton("打印报表", AppTheme.PrimaryLight);
            btnPrint.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrint.Location = new Point(cw - 82, 5);
            btnPrint.Size = new Size(78, 28);
            btnPrint.Click += (s, e) => ShowToast("报表已发送到打印机");
            queryPanel.Controls.Add(btnPrint);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 100);
            dgv.Size = new Size(cw, 380);
            dgv.DataSource = DemoData.GetHistoryTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var summaryCard = AppTheme.CreateCard("查询统计", 228, 190);
            summaryCard.Location = new Point(10, 50);
            _rightPanel.Controls.Add(summaryCard);

            AppTheme.AddDataRow(summaryCard, "记录总数:", "48条", 12, 42);
            AppTheme.AddDataRow(summaryCard, "时间跨度:", "48小时", 12, 63);
            AppTheme.AddDataRow(summaryCard, "采样间隔:", "2小时", 12, 84);
            AppTheme.AddDataRow(summaryCard, "异常记录:", "0条", 12, 105, AppTheme.AccentGreen);
            AppTheme.AddDataRow(summaryCard, "数据完整性:", "100%", 12, 126, AppTheme.AccentGreen);
        }

        // ============================================================
        // 模块8: 控制形式设置
        // ============================================================
        private void LoadControlMode()
        {
            AddPageTitle("控制形式设置", "养殖设备控制模式配置与运行状态管理");

            var cw = ContentWidth;

            var btnPanel = new Panel();
            btnPanel.Location = new Point(16, 54);
            btnPanel.Size = new Size(cw, 34);
            _mainContent.Controls.Add(btnPanel);

            var btnBatch = CreateActionButton("批量设置", AppTheme.AccentCyan);
            btnBatch.Location = new Point(0, 2);
            btnBatch.Click += (s, e) => ShowToast("已进入批量设置模式");
            btnPanel.Controls.Add(btnBatch);

            var btnAuto = CreateActionButton("全部自动", AppTheme.AccentGreen);
            btnAuto.Location = new Point(100, 2);
            btnAuto.Click += (s, e) => ShowToast("已将全部设备切换为自动模式");
            btnPanel.Controls.Add(btnAuto);

            var btnManual = CreateActionButton("全部手动", AppTheme.AccentOrange);
            btnManual.Location = new Point(200, 2);
            btnManual.Click += (s, e) => ShowToast("已将全部设备切换为手动模式");
            btnPanel.Controls.Add(btnManual);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 96);
            dgv.Size = new Size(cw, 340);
            dgv.DataSource = DemoData.GetControlModeTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var modeCard = AppTheme.CreateCard("控制模式说明", 228, 190);
            modeCard.Location = new Point(10, 50);
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
                lbl.Location = new Point(12, 42 + i * 16);
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

            var cw = ContentWidth;

            var btnPanel = new Panel();
            btnPanel.Location = new Point(16, 54);
            btnPanel.Size = new Size(cw, 34);
            _mainContent.Controls.Add(btnPanel);

            var btnNewTask = CreateActionButton("新建任务", AppTheme.AccentCyan);
            btnNewTask.Location = new Point(0, 2);
            btnNewTask.Click += (s, e) => ShowToast("已打开新建任务对话框");
            btnPanel.Controls.Add(btnNewTask);

            var btnEditTask = CreateActionButton("编辑任务", AppTheme.PrimaryLight);
            btnEditTask.Location = new Point(100, 2);
            btnEditTask.Click += (s, e) => ShowToast("已打开编辑任务对话框");
            btnPanel.Controls.Add(btnEditTask);

            var btnEnableTask = CreateActionButton("启用/停用", AppTheme.AccentGreen);
            btnEnableTask.Location = new Point(200, 2);
            btnEnableTask.Click += (s, e) => ShowToast("任务状态已切换");
            btnPanel.Controls.Add(btnEnableTask);

            var dgv = CreateStyledDataGridView();
            dgv.Location = new Point(16, 96);
            dgv.Size = new Size(cw, 340);
            dgv.DataSource = DemoData.GetControlScheduleTable();
            _mainContent.Controls.Add(dgv);

            // 右侧
            var todayCard = AppTheme.CreateCard("今日任务", 228, 190);
            todayCard.Location = new Point(10, 50);
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
                lbl.Location = new Point(12, 42 + i * 22);
                lbl.AutoSize = true;
                todayCard.Controls.Add(lbl);
            }
        }

        // ============================================================
        // 公共UI辅助方法
        // ============================================================

        /// <summary>
        /// 添加页面标题和描述
        /// </summary>
        private void AddPageTitle(string title, string description)
        {
            var lbl = new Label();
            lbl.Text = title;
            lbl.ForeColor = AppTheme.TextPrimary;
            lbl.Font = AppTheme.SubTitleFont;
            lbl.Location = new Point(16, 6);
            lbl.AutoSize = true;
            _mainContent.Controls.Add(lbl);

            var desc = new Label();
            desc.Text = description;
            desc.ForeColor = AppTheme.TextMuted;
            desc.Font = AppTheme.SmallFont;
            desc.Location = new Point(16, 30);
            desc.AutoSize = true;
            _mainContent.Controls.Add(desc);
        }

        /// <summary>
        /// 创建统计卡片 - 自适应宽度
        /// </summary>
        private void AddStatCard(Panel parent, int index, string label, string value, string unit, Color valueColor)
        {
            int totalCards = 5;
            if (parent.Controls.Count > 0 && parent.Controls.Count < 5)
                totalCards = parent.Controls.Count;

            // 根据实际父容器宽度计算
            int gap = 6;
            int cardWidth = (parent.Width - gap * (totalCards + 1)) / totalCards;
            if (cardWidth < 100) cardWidth = 100;

            var card = new Panel();
            card.Location = new Point(gap + index * (cardWidth + gap), 0);
            card.Size = new Size(cardWidth, 64);
            card.BackColor = AppTheme.BackgroundCard;
            parent.Controls.Add(card);

            var lbl = new Label();
            lbl.Text = label;
            lbl.ForeColor = AppTheme.TextMuted;
            lbl.Font = AppTheme.SmallFont;
            lbl.Location = new Point(10, 6);
            lbl.AutoSize = true;
            card.Controls.Add(lbl);

            var val = new Label();
            val.Text = value + (string.IsNullOrEmpty(unit) ? "" : " " + unit);
            val.ForeColor = valueColor;
            val.Font = new Font("Consolas", 13F, FontStyle.Bold);
            val.Location = new Point(10, 28);
            val.AutoSize = true;
            card.Controls.Add(val);
        }

        /// <summary>
        /// 创建统一样式DataGridView
        /// </summary>
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
            dgv.ColumnHeadersHeight = 30;

            dgv.DefaultCellStyle.BackColor = AppTheme.BackgroundCard;
            dgv.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
            dgv.DefaultCellStyle.Font = AppTheme.ContentFont;
            dgv.DefaultCellStyle.SelectionBackColor = AppTheme.PrimaryBlue;
            dgv.DefaultCellStyle.SelectionForeColor = AppTheme.TextPrimary;
            dgv.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 42, 68);
            dgv.RowHeadersVisible = false;

            return dgv;
        }

        /// <summary>
        /// 创建操作按钮
        /// </summary>
        private Button CreateActionButton(string text, Color bgColor)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Size = new Size(92, 28);
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
