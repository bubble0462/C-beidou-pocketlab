using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DeepSeaAquacultureTerminal
{
    public class MainForm : Form
    {
        private const int TOPBAR_H = 50;
        private const int NAV_W = 180;
        private const int RIGHT_W = 248;

        private Panel _topBar;
        private Panel _topRightArea;
        private Panel _leftNav;
        private Panel _mainContent;
        private Panel _rightPanel;

        private Label _lblBeiDouTime;
        private Label _lblStatusLine;
        private Label _lblCurrentModule;
        private Panel _rDivLine;

        private Button[] _navButtons;
        private Label _lblVersion;

        private Panel _toastBar;
        private Label _toastLabel;
        private Timer _toastTimer;

        private bool _initialized = false;
        private int _activeModule = 0;

        private readonly string[] _moduleNames = new string[]
        {
            "养殖池整体监控", "实时视频监控", "病虫防治", "利润分析",
            "标准值设定", "数据实时查询", "趋势图查询", "历史数据查询",
            "控制形式设置", "控制时间设置"
        };

        public MainForm()
        {
            InitializeForm();
            CreateControls();
            StartClock();
            CreateToast();
            _initialized = true;
            DoLayout();
            SwitchModule(0);
        }

        private void InitializeForm()
        {
            this.Text = "基于北斗时空数据的深远海养殖辅助分析终端软件 V1.0";
            this.Size = new Size(1366, 768);
            this.MinimumSize = new Size(1024, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppTheme.BackgroundDark;
            this.DoubleBuffered = true;
        }

        // ============================================================
        // 创建所有控件 - 不设 Dock，全部由 DoLayout 定位
        // ============================================================
        private void CreateControls()
        {
            // --- 顶部栏 ---
            _topBar = new Panel();
            _topBar.BackColor = AppTheme.PrimaryDark;
            this.Controls.Add(_topBar);

            // 顶部栏内：右侧状态区（手动定位，不用 Dock）
            _topRightArea = new Panel();
            _topRightArea.BackColor = AppTheme.PrimaryDark;
            _topBar.Controls.Add(_topRightArea);

            _lblBeiDouTime = new Label();
            _lblBeiDouTime.Text = "北斗授时: " + DemoData.GetBeiDouTime();
            _lblBeiDouTime.ForeColor = AppTheme.AccentCyan;
            _lblBeiDouTime.Font = AppTheme.DataFont;
            _lblBeiDouTime.Location = new Point(4, 4);
            _lblBeiDouTime.AutoSize = true;
            _topRightArea.Controls.Add(_lblBeiDouTime);

            _lblStatusLine = new Label();
            _lblStatusLine.Text = "● 系统在线 | 传感器: 24/24 | 北斗信号: 正常";
            _lblStatusLine.ForeColor = AppTheme.AccentGreen;
            _lblStatusLine.Font = AppTheme.SmallFont;
            _lblStatusLine.Location = new Point(4, 24);
            _lblStatusLine.AutoSize = true;
            _topRightArea.Controls.Add(_lblStatusLine);

            // 顶部栏内：左侧标题
            var lblTitle = new Label();
            lblTitle.Text = "基于北斗时空数据的深远海养殖辅助分析终端软件 V1.0";
            lblTitle.ForeColor = AppTheme.TextPrimary;
            lblTitle.Font = new Font("微软雅黑", 13F, FontStyle.Bold);
            lblTitle.Location = new Point(16, 12);
            lblTitle.AutoSize = true;
            _topBar.Controls.Add(lblTitle);

            // 顶部栏底部分割线
            var topLine = new Panel();
            topLine.BackColor = AppTheme.PrimaryBlue;
            topLine.Location = new Point(0, TOPBAR_H - 2);
            topLine.Size = new Size(2000, 2);
            _topBar.Controls.Add(topLine);

            // --- 左侧导航 ---
            _leftNav = new Panel();
            _leftNav.BackColor = AppTheme.BackgroundPanel;
            this.Controls.Add(_leftNav);
            BuildLeftNav();

            // --- 右侧面板 ---
            _rightPanel = new Panel();
            _rightPanel.BackColor = AppTheme.BackgroundPanel;
            this.Controls.Add(_rightPanel);
            BuildRightPanel();

            // --- 主内容区 ---
            _mainContent = new Panel();
            _mainContent.BackColor = AppTheme.BackgroundDark;
            _mainContent.AutoScroll = true;
            this.Controls.Add(_mainContent);
        }

        /// <summary>
        /// 绝对布局 - 所有面板用 SetBounds 定位，不依赖任何 Dock
        /// </summary>
        private void DoLayout()
        {
            int cw = this.ClientSize.Width;
            int ch = this.ClientSize.Height;

            // 顶部栏
            _topBar.SetBounds(0, 0, cw, TOPBAR_H);

            // 顶部右侧状态区
            _topRightArea.SetBounds(cw - 290, 0, 290, TOPBAR_H);

            // 主体高度
            int bodyY = TOPBAR_H;
            int bodyH = ch - TOPBAR_H;

            // 左侧导航
            _leftNav.SetBounds(0, bodyY, NAV_W, bodyH);

            // 右侧面板
            _rightPanel.SetBounds(cw - RIGHT_W, bodyY, RIGHT_W, bodyH);

            // 主内容区
            int mainX = NAV_W;
            int mainW = cw - NAV_W - RIGHT_W;
            if (mainW < 200) mainW = 200;
            _mainContent.SetBounds(mainX, bodyY, mainW, bodyH);

            // 版本号
            _lblVersion.Location = new Point(14, _leftNav.Height - 22);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_initialized) DoLayout();
        }

        // ============================================================
        // 左侧导航内容
        // ============================================================
        private void BuildLeftNav()
        {
            var lblNav = new Label();
            lblNav.Text = "功能模块";
            lblNav.ForeColor = AppTheme.AccentCyan;
            lblNav.Font = AppTheme.HeaderFont;
            lblNav.Location = new Point(14, 8);
            lblNav.AutoSize = true;
            _leftNav.Controls.Add(lblNav);

            var sep = new Panel();
            sep.Location = new Point(10, 32);
            sep.Size = new Size(160, 1);
            sep.BackColor = AppTheme.BorderColor;
            _leftNav.Controls.Add(sep);

            _navButtons = new Button[10];
            for (int i = 0; i < 10; i++)
            {
                var btn = new Button();
                btn.Text = "  " + _moduleNames[i];
                btn.Location = new Point(0, 38 + i * 42);
                btn.Size = new Size(NAV_W, 40);
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
                btn.Click += (s, e) => SwitchModule((int)((Button)s).Tag);
                _leftNav.Controls.Add(btn);
                _navButtons[i] = btn;
            }

            // 右边框线
            var rightLine = new Panel();
            rightLine.BackColor = AppTheme.BorderColor;
            rightLine.Location = new Point(NAV_W - 1, 0);
            rightLine.Size = new Size(1, 2000);
            _leftNav.Controls.Add(rightLine);

            _lblVersion = new Label();
            _lblVersion.Text = "V1.0 | 北斗时空数据平台";
            _lblVersion.ForeColor = AppTheme.TextMuted;
            _lblVersion.Font = new Font("微软雅黑", 7F);
            _lblVersion.AutoSize = true;
            _leftNav.Controls.Add(_lblVersion);
        }

        // ============================================================
        // 右侧面板内容
        // ============================================================
        private void BuildRightPanel()
        {
            // 左边框线
            var leftLine = new Panel();
            leftLine.BackColor = AppTheme.BorderColor;
            leftLine.Location = new Point(0, 0);
            leftLine.Size = new Size(1, 2000);
            _rightPanel.Controls.Add(leftLine);

            _lblCurrentModule = new Label();
            _lblCurrentModule.Text = "养殖池整体监控";
            _lblCurrentModule.ForeColor = AppTheme.AccentCyan;
            _lblCurrentModule.Font = AppTheme.HeaderFont;
            _lblCurrentModule.Location = new Point(10, 8);
            _lblCurrentModule.AutoSize = true;
            _rightPanel.Controls.Add(_lblCurrentModule);

            _rDivLine = new Panel();
            _rDivLine.Location = new Point(10, 30);
            _rDivLine.Size = new Size(RIGHT_W - 18, 1);
            _rDivLine.BackColor = AppTheme.BorderColor;
            _rightPanel.Controls.Add(_rDivLine);
        }

        private void CreateToast()
        {
            _toastBar = new Panel();
            _toastBar.BackColor = AppTheme.PrimaryBlue;
            _toastBar.SetBounds(0, this.ClientSize.Height - 24, this.ClientSize.Width, 24);
            this.Controls.Add(_toastBar);

            _toastLabel = new Label();
            _toastLabel.ForeColor = AppTheme.TextPrimary;
            _toastLabel.Font = AppTheme.ContentFont;
            _toastLabel.AutoSize = true;
            _toastLabel.Location = new Point(12, 4);
            _toastBar.Controls.Add(_toastLabel);

            _toastTimer = new Timer { Interval = 2000 };
            _toastTimer.Tick += (s, e) => { _toastBar.Height = 0; _toastTimer.Stop(); };
        }

        private void StartClock()
        {
            var t = new Timer { Interval = 1000 };
            t.Tick += (s, e) => _lblBeiDouTime.Text = "北斗授时: " + DemoData.GetBeiDouTime();
            t.Start();
        }

        private void ShowToast(string msg)
        {
            _toastLabel.Text = "  " + msg;
            _toastBar.Height = 24;
            _toastBar.BringToFront();
            _toastTimer.Stop();
            _toastTimer.Start();
        }

        // ============================================================
        // 模块切换
        // ============================================================
        private void SwitchModule(int index)
        {
            for (int i = 0; i < _navButtons.Length; i++)
            {
                if (i == index) AppTheme.ActivateNavButton(_navButtons[i]);
                else AppTheme.DeactivateNavButton(_navButtons[i]);
            }
            _activeModule = index;
            _lblCurrentModule.Text = _moduleNames[index];

            _mainContent.Controls.Clear();

            // 清空右侧，保留边框线和标题/分割线
            _rightPanel.Controls.Clear();
            var leftLine = new Panel();
            leftLine.BackColor = AppTheme.BorderColor;
            leftLine.Location = new Point(0, 0);
            leftLine.Size = new Size(1, 2000);
            _rightPanel.Controls.Add(leftLine);
            _rightPanel.Controls.Add(_lblCurrentModule);
            _rightPanel.Controls.Add(_rDivLine);

            switch (index)
            {
                case 0: ModPoolMonitor(); break;
                case 1: ModVideoMonitor(); break;
                case 2: ModDiseaseControl(); break;
                case 3: ModProfitAnalysis(); break;
                case 4: ModStandardSetting(); break;
                case 5: ModRealtimeQuery(); break;
                case 6: ModTrendChart(); break;
                case 7: ModHistoryQuery(); break;
                case 8: ModControlMode(); break;
                case 9: ModControlSchedule(); break;
            }
            if (_mainContent.Controls.Count > 0)
                _mainContent.ScrollControlIntoView(_mainContent.Controls[0]);
        }

        /// <summary>主内容区可用宽度</summary>
        private int CW { get { return Math.Max(_mainContent.ClientSize.Width - 16, 200); } }

        // ============================================================
        // 模块 0: 养殖池整体监控
        // ============================================================
        private void ModPoolMonitor()
        {
            PageTitle("养殖池整体监控", "实时监测各养殖池环境参数与鱼群健康状态");
            int cw = CW;
            var sp = StatsRow(48, cw);
            SC(sp, 0, 5, "养殖池总数", "8", "个", AppTheme.AccentCyan);
            SC(sp, 1, 5, "正常池", "6", "个", AppTheme.AccentGreen);
            SC(sp, 2, 5, "关注池", "2", "个", AppTheme.AccentOrange);
            SC(sp, 3, 5, "告警池", "0", "个", AppTheme.AccentGreen);
            SC(sp, 4, 5, "传感器在线", "24/24", "", AppTheme.AccentCyan);
            Grid(cw, 108, 250, DemoData.GetPoolMonitorTable());

            RC("A1池详情", 168, c =>
            {
                DR(c, "品种:", "大黄鱼", 40);
                DR(c, "水温:", "23.5°C", 60);
                DR(c, "盐度:", "32.1ppt", 80);
                DR(c, "溶氧:", "7.2mg/L", 100);
                DR(c, "健康评分:", "95分", 120, AppTheme.AccentGreen);
                DR(c, "投喂状态:", "已投喂", 140, AppTheme.AccentGreen);
            });
            RC("北斗定位信息", 92, c =>
            {
                DR(c, "经度:", "118.5672°E", 40);
                DR(c, "纬度:", "24.8765°N", 60);
                DR(c, "授时精度:", "±20ns", 80);
            });
        }

        // ============================================================
        // 模块 1: 实时视频监控
        // ============================================================
        private void ModVideoMonitor()
        {
            PageTitle("实时视频监控", "养殖场视频监控实时画面与云台控制");
            int cw = CW, hw = (cw - 8) / 2;
            string[] cams = { "A区主摄像头", "B区主摄像头", "C区主摄像头", "外海浮标摄像头" };
            for (int i = 0; i < 4; i++)
            {
                int col = i % 2, row = i / 2;
                var vp = new Panel { Location = new Point(8 + col * (hw + 8), 48 + row * 218), Size = new Size(hw, 206), BackColor = Color.FromArgb(10, 15, 25) };
                _mainContent.Controls.Add(vp);
                vp.Controls.Add(new Label { Text = cams[i], ForeColor = AppTheme.AccentCyan, Font = AppTheme.ContentFont, Location = new Point(6, 4), AutoSize = true });
                vp.Controls.Add(new Label { Text = "● REC", ForeColor = AppTheme.AccentRed, Font = new Font("Consolas", 9F, FontStyle.Bold), Location = new Point(hw - 60, 4), AutoSize = true });
                var pb = new PictureBox { Location = new Point(4, 22), Size = new Size(hw - 8, 148), BackColor = Color.FromArgb(8, 12, 20) };
                pb.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    using (var p = new Pen(AppTheme.BorderColor)) { for (int x = 0; x < pb.Width; x += 40) g.DrawLine(p, x, 0, x, pb.Height); for (int y = 0; y < pb.Height; y += 40) g.DrawLine(p, 0, y, pb.Width, y); }
                    int cx = pb.Width / 2, cy = pb.Height / 2;
                    using (var cp = new Pen(AppTheme.AccentCyan)) { g.DrawLine(cp, cx - 20, cy, cx + 20, cy); g.DrawLine(cp, cx, cy - 20, cx, cy + 20); }
                    g.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), AppTheme.DataFont, Brushes.Gray, 4, pb.Height - 16);
                };
                vp.Controls.Add(pb);
                vp.Controls.Add(new Label { Text = DateTime.Now.ToString("HH:mm:ss") + " | 1920x1080 | 25fps", ForeColor = AppTheme.TextMuted, Font = new Font("微软雅黑", 7F), Location = new Point(6, 176), AutoSize = true });
            }
            RC("云台控制", 155, c =>
            {
                string[] pts = { "↑上", "←左", "归位", "→右", "↓下", "放大", "缩小", "聚焦+" };
                for (int i = 0; i < pts.Length; i++) { var b = new Button { Text = pts[i], Size = new Size(58, 24), Location = new Point(8 + (i % 3) * 64, 36 + (i / 3) * 28), BackColor = AppTheme.BackgroundLight, ForeColor = AppTheme.TextPrimary, FlatStyle = FlatStyle.Flat, Font = AppTheme.SmallFont, Cursor = Cursors.Hand }; string t = pts[i]; b.Click += (s, e) => ShowToast("云台: " + t); c.Controls.Add(b); }
            });
            RC("监控告警", 96, c =>
            {
                string[] a = { "● B1区 溶氧偏低", "● 摄像头全部在线", "● 存储: 78.2%" };
                Color[] ac = { AppTheme.AccentOrange, AppTheme.AccentGreen, AppTheme.TextSecondary };
                for (int i = 0; i < a.Length; i++) c.Controls.Add(new Label { Text = a[i], ForeColor = ac[i], Font = AppTheme.SmallFont, Location = new Point(8, 36 + i * 20), AutoSize = true });
            });
        }

        // ============================================================
        // 模块 2: 病虫防治
        // ============================================================
        private void ModDiseaseControl()
        {
            PageTitle("病虫防治", "病害检测、风险评估与处理措施跟踪");
            int cw = CW;
            var sp = StatsRow(48, cw);
            SC(sp, 0, 4, "本月检测", "156", "次", AppTheme.AccentCyan);
            SC(sp, 1, 4, "已处理", "148", "次", AppTheme.AccentGreen);
            SC(sp, 2, 4, "处理中", "5", "次", AppTheme.AccentOrange);
            SC(sp, 3, 4, "高风险", "1", "项", AppTheme.AccentRed);
            Grid(cw, 112, 250, DemoData.GetDiseaseControlTable());
            RC("风险统计", 110, c => { DR(c, "低风险:", "3项", 40, AppTheme.AccentGreen); DR(c, "中风险:", "2项", 58, AppTheme.AccentOrange); DR(c, "高风险:", "1项", 76, AppTheme.AccentRed); DR(c, "治愈率:", "95.2%", 94, AppTheme.AccentGreen); });
            RC("防治指南", 106, c =>
            {
                string[] t = { "1. 定期采样检测", "2. 保持水质稳定", "3. 合理投喂密度", "4. 发现异常及时隔离" };
                for (int i = 0; i < t.Length; i++) c.Controls.Add(new Label { Text = t[i], ForeColor = AppTheme.TextSecondary, Font = AppTheme.SmallFont, Location = new Point(8, 36 + i * 18), AutoSize = true });
            });
        }

        // ============================================================
        // 模块 3: 利润分析
        // ============================================================
        private void ModProfitAnalysis()
        {
            PageTitle("利润分析", "各品种养殖成本、产出与利润率综合分析");
            int cw = CW;
            var sp = StatsRow(48, cw);
            SC(sp, 0, 4, "总投入", "66.0", "万元", AppTheme.AccentOrange);
            SC(sp, 1, 4, "预计产出", "98.3", "万元", AppTheme.AccentCyan);
            SC(sp, 2, 4, "平均利润率", "49.1", "%", AppTheme.AccentGreen);
            SC(sp, 3, 4, "最优品种", "石斑鱼", "", AppTheme.AccentGreen);
            Grid(cw, 112, 170, DemoData.GetProfitTable());
            var ch = AppTheme.CreateBarChart("月度利润趋势(万元)", DemoData.GetMonthLabels(), DemoData.GetMonthlyProfit(), cw, 170);
            ch.Location = new Point(8, 294); _mainContent.Controls.Add(ch);
            RC("收益概览", 164, c => { DR(c, "本季收入:", "98.3万", 40); DR(c, "本季成本:", "66.0万", 58); DR(c, "净利润:", "32.3万", 76, AppTheme.AccentGreen); DR(c, "同比增幅:", "+12.5%", 94, AppTheme.AccentGreen); DR(c, "最高利润率:", "60.0%", 112, AppTheme.AccentCyan); DR(c, "最低存活率:", "82.0%", 130, AppTheme.AccentOrange); });
        }

        // ============================================================
        // 模块 4: 标准值设定
        // ============================================================
        private void ModStandardSetting()
        {
            PageTitle("标准值设定", "水质参数标准范围与告警阈值配置");
            int cw = CW;
            var bp = BtnRow(46, cw);
            AB(bp, 0, "新增参数", AppTheme.AccentCyan, "已打开新增参数对话框");
            AB(bp, 80, "编辑选中", AppTheme.PrimaryLight, "已打开编辑参数对话框");
            AB(bp, 160, "删除选中", AppTheme.AccentRed, "请确认删除");
            AB(bp, 240, "导出配置", AppTheme.BackgroundLight, "配置已导出");
            Grid(cw, 84, 290, DemoData.GetStandardTable());
            RC("告警级别说明", 164, c =>
            {
                string[] a = { "一级告警: 立即处理", "  → 溶氧/氨氮超标", "二级告警: 密切关注", "  → 水温/pH异常", "三级告警: 记录提醒", "  → 透明度/其他" };
                for (int i = 0; i < a.Length; i++) c.Controls.Add(new Label { Text = a[i], ForeColor = i % 2 == 0 ? AppTheme.TextPrimary : AppTheme.TextMuted, Font = AppTheme.SmallFont, Location = new Point(8, 36 + i * 20), AutoSize = true });
            });
        }

        // ============================================================
        // 模块 5: 数据实时查询
        // ============================================================
        private void ModRealtimeQuery()
        {
            PageTitle("数据实时查询", "传感器实时采集数据与北斗时空数据查询");
            int cw = CW;
            var qp = new Panel { Location = new Point(8, 46), Size = new Size(cw, 32), BackColor = AppTheme.BackgroundCard };
            _mainContent.Controls.Add(qp);
            qp.Controls.Add(new Label { Text = "池号:", ForeColor = AppTheme.TextSecondary, Font = AppTheme.ContentFont, Location = new Point(6, 7), AutoSize = true });
            var cb1 = new ComboBox(); cb1.Items.AddRange(new[] { "全部", "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2" }); cb1.SelectedIndex = 0; cb1.Location = new Point(36, 5); cb1.Size = new Size(56, 22); cb1.Font = AppTheme.SmallFont; qp.Controls.Add(cb1);
            qp.Controls.Add(new Label { Text = "参数:", ForeColor = AppTheme.TextSecondary, Font = AppTheme.ContentFont, Location = new Point(104, 7), AutoSize = true });
            var cb2 = new ComboBox(); cb2.Items.AddRange(new[] { "全部", "水温", "盐度", "溶氧", "pH", "氨氮" }); cb2.SelectedIndex = 0; cb2.Location = new Point(134, 5); cb2.Size = new Size(56, 22); cb2.Font = AppTheme.SmallFont; qp.Controls.Add(cb2);
            var bq = AB(qp, 200, "查询", AppTheme.AccentCyan, "查询完成，共8条"); bq.Size = new Size(52, 24); bq.Location = new Point(200, 4);
            var br = AB(qp, 260, "刷新", AppTheme.PrimaryLight, "数据已刷新"); br.Size = new Size(52, 24); br.Location = new Point(260, 4);
            Grid(cw, 86, 270, DemoData.GetRealtimeQueryTable());
            RC("数据来源", 106, c =>
            {
                string[] s = { "● 北斗卫星授时定位", "● 水下传感器阵列", "● 浮标气象站", "● 水质监测探头" };
                for (int i = 0; i < s.Length; i++) c.Controls.Add(new Label { Text = s[i], ForeColor = AppTheme.AccentGreen, Font = AppTheme.SmallFont, Location = new Point(8, 36 + i * 18), AutoSize = true });
            });
        }

        // ============================================================
        // 模块 6: 趋势图查询
        // ============================================================
        private void ModTrendChart()
        {
            PageTitle("趋势图查询", "水质参数趋势分析与多维度数据对比");
            int cw = CW;
            var qp = new Panel { Location = new Point(8, 46), Size = new Size(cw, 26), BackColor = AppTheme.BackgroundCard };
            _mainContent.Controls.Add(qp);
            qp.Controls.Add(new Label { Text = "池号: A1    日期: 2026-06-09    参数: 全部", ForeColor = AppTheme.TextSecondary, Font = AppTheme.ContentFont, Location = new Point(6, 4), AutoSize = true });
            var c1 = AppTheme.CreateLineChart("水温趋势 (°C) - A1池", DemoData.GetTrendTimeLabels(), DemoData.GetTrendWaterTemp(), cw, 180);
            c1.Location = new Point(8, 78); _mainContent.Controls.Add(c1);
            var c2 = AppTheme.CreateLineChart("溶解氧趋势 (mg/L) - A1池", DemoData.GetTrendTimeLabels(), DemoData.GetTrendDissolvedOxygen(), cw, 180);
            c2.Location = new Point(8, 268); _mainContent.Controls.Add(c2);
            RC("趋势统计", 164, c => { DR(c, "水温最高:", "24.5°C", 40, AppTheme.AccentOrange); DR(c, "水温最低:", "22.0°C", 58, AppTheme.AccentCyan); DR(c, "水温均值:", "23.26°C", 76); DR(c, "溶氧最高:", "7.8mg/L", 94); DR(c, "溶氧最低:", "6.5mg/L", 112, AppTheme.AccentOrange); DR(c, "趋势判断:", "正常波动", 130, AppTheme.AccentGreen); });
        }

        // ============================================================
        // 模块 7: 历史数据查询
        // ============================================================
        private void ModHistoryQuery()
        {
            PageTitle("历史数据查询", "历史水质监测数据查询与导出");
            int cw = CW;
            var qp = new Panel { Location = new Point(8, 46), Size = new Size(cw, 32), BackColor = AppTheme.BackgroundCard };
            _mainContent.Controls.Add(qp);
            qp.Controls.Add(new Label { Text = "池号: A1    起始: 2026-06-08    结束: 2026-06-09", ForeColor = AppTheme.TextSecondary, Font = AppTheme.ContentFont, Location = new Point(6, 8), AutoSize = true });
            var be = AB(qp, cw - 140, "导出Excel", AppTheme.AccentCyan, "已导出Excel"); be.Size = new Size(64, 24); be.Location = new Point(cw - 140, 4);
            var bp2 = AB(qp, cw - 68, "打印报表", AppTheme.PrimaryLight, "已发送打印机"); bp2.Size = new Size(64, 24); bp2.Location = new Point(cw - 68, 4);
            Grid(cw, 86, 320, DemoData.GetHistoryTable());
            RC("查询统计", 164, c => { DR(c, "记录总数:", "48条", 40); DR(c, "时间跨度:", "48小时", 58); DR(c, "采样间隔:", "2小时", 76); DR(c, "异常记录:", "0条", 94, AppTheme.AccentGreen); DR(c, "数据完整性:", "100%", 112, AppTheme.AccentGreen); });
        }

        // ============================================================
        // 模块 8: 控制形式设置
        // ============================================================
        private void ModControlMode()
        {
            PageTitle("控制形式设置", "养殖设备控制模式配置与运行状态管理");
            int cw = CW;
            var bp = BtnRow(46, cw);
            AB(bp, 0, "批量设置", AppTheme.AccentCyan, "已进入批量设置模式");
            AB(bp, 80, "全部自动", AppTheme.AccentGreen, "已切换自动模式");
            AB(bp, 160, "全部手动", AppTheme.AccentOrange, "已切换手动模式");
            Grid(cw, 84, 290, DemoData.GetControlModeTable());
            RC("控制模式说明", 164, c =>
            {
                string[] m = { "自动模式:", "  传感器数据自动调节", "手动模式:", "  操作员手动控制", "定时模式:", "  预设计划自动执行" };
                for (int i = 0; i < m.Length; i++) c.Controls.Add(new Label { Text = m[i], ForeColor = m[i].EndsWith(":") ? AppTheme.AccentCyan : AppTheme.TextMuted, Font = AppTheme.SmallFont, Location = new Point(8, 36 + i * 18), AutoSize = true });
            });
        }

        // ============================================================
        // 模块 9: 控制时间设置
        // ============================================================
        private void ModControlSchedule()
        {
            PageTitle("控制时间设置", "设备定时任务计划与执行周期管理");
            int cw = CW;
            var bp = BtnRow(46, cw);
            AB(bp, 0, "新建任务", AppTheme.AccentCyan, "已打开新建任务");
            AB(bp, 80, "编辑任务", AppTheme.PrimaryLight, "已打开编辑任务");
            AB(bp, 160, "启用/停用", AppTheme.AccentGreen, "任务状态已切换");
            Grid(cw, 84, 290, DemoData.GetControlScheduleTable());
            RC("今日任务", 164, c =>
            {
                string[] t = { "06:30  晨间投喂 ✓", "08:00  水质采样 ✓", "09:00  数据上报 ✓", "11:30  午间投喂 ✓", "14:30  水质采样 ○", "17:30  晚间投喂 ○", "18:00  夜间照明 ○" };
                for (int i = 0; i < t.Length; i++) c.Controls.Add(new Label { Text = t[i], ForeColor = t[i].Contains("✓") ? AppTheme.AccentGreen : AppTheme.TextSecondary, Font = AppTheme.SmallFont, Location = new Point(8, 36 + i * 18), AutoSize = true });
            });
        }

        // ============================================================
        // UI 辅助
        // ============================================================
        private void PageTitle(string title, string desc)
        {
            _mainContent.Controls.Add(new Label { Text = title, ForeColor = AppTheme.TextPrimary, Font = AppTheme.SubTitleFont, Location = new Point(8, 4), AutoSize = true });
            _mainContent.Controls.Add(new Label { Text = desc, ForeColor = AppTheme.TextMuted, Font = AppTheme.SmallFont, Location = new Point(8, 24), AutoSize = true });
        }
        private Panel StatsRow(int top, int cw)
        {
            var p = new Panel { Location = new Point(8, top), Size = new Size(cw, 48) };
            _mainContent.Controls.Add(p);
            return p;
        }
        private void SC(Panel parent, int idx, int total, string label, string value, string unit, Color color)
        {
            int gap = 4, w = Math.Max((parent.Width - gap * (total + 1)) / total, 70);
            var card = new Panel { Location = new Point(gap + idx * (w + gap), 0), Size = new Size(w, 44), BackColor = AppTheme.BackgroundCard };
            parent.Controls.Add(card);
            card.Controls.Add(new Label { Text = label, ForeColor = AppTheme.TextMuted, Font = AppTheme.SmallFont, Location = new Point(6, 2), AutoSize = true });
            card.Controls.Add(new Label { Text = value + (unit == "" ? "" : " " + unit), ForeColor = color, Font = new Font("Consolas", 12F, FontStyle.Bold), Location = new Point(6, 20), AutoSize = true });
        }
        private void Grid(int cw, int top, int height, DataTable dt)
        {
            var dgv = MkGrid(); dgv.Location = new Point(8, top); dgv.Size = new Size(cw, height); dgv.DataSource = dt; _mainContent.Controls.Add(dgv);
        }
        private DataGridView MkGrid()
        {
            var dgv = new DataGridView();
            dgv.BackgroundColor = AppTheme.BackgroundCard; dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.EnableHeadersVisualStyles = false; dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false; dgv.AllowUserToDeleteRows = false;
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
            dgv.ColumnHeadersHeight = 26;
            dgv.DefaultCellStyle.BackColor = AppTheme.BackgroundCard;
            dgv.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
            dgv.DefaultCellStyle.Font = AppTheme.ContentFont;
            dgv.DefaultCellStyle.SelectionBackColor = AppTheme.PrimaryBlue;
            dgv.DefaultCellStyle.SelectionForeColor = AppTheme.TextPrimary;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 42, 68);
            dgv.RowHeadersVisible = false;
            return dgv;
        }
        private Panel BtnRow(int top, int cw)
        {
            var p = new Panel { Location = new Point(8, top), Size = new Size(cw, 28) };
            _mainContent.Controls.Add(p);
            return p;
        }
        private Button AB(Panel parent, int x, string text, Color bg, string toast)
        {
            var b = new Button { Text = text, Size = new Size(74, 24), Location = new Point(x, 2), BackColor = bg, ForeColor = AppTheme.TextPrimary, FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, Font = AppTheme.SmallFont, Cursor = Cursors.Hand };
            b.Click += (s, e) => ShowToast(toast);
            parent.Controls.Add(b);
            return b;
        }

        /// <summary>右侧面板添加卡片 - 只跳过边框线(宽=1)和固定控件</summary>
        private void RC(string title, int height, Action<Panel> fill)
        {
            int top = 44;
            foreach (Control c in _rightPanel.Controls)
            {
                if (c == _lblCurrentModule || c == _rDivLine) continue;
                if (c.Size.Width <= 1 || c.Size.Height <= 1) continue; // 跳过边框线
                top = Math.Max(top, c.Location.Y + c.Height + 6);
            }
            var card = AppTheme.CreateCard(title, RIGHT_W - 16, height);
            card.Location = new Point(6, top);
            _rightPanel.Controls.Add(card);
            fill(card);
        }

        private void DR(Panel card, string label, string value, int y, Color? vc = null)
        {
            AppTheme.AddDataRow(card, label, value, 8, y, vc);
        }
    }
}
