using System;
using System.Data;

namespace DeepSeaAquacultureTerminal
{
    /// <summary>
    /// 演示数据 - 模拟深远海养殖业务数据
    /// </summary>
    public static class DemoData
    {
        // 北斗时间
        public static string GetBeiDouTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " BDT";
        }

        // 养殖池数据表
        public static DataTable GetPoolMonitorTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("池号", typeof(string));
            dt.Columns.Add("品种", typeof(string));
            dt.Columns.Add("水温(°C)", typeof(string));
            dt.Columns.Add("盐度(ppt)", typeof(string));
            dt.Columns.Add("溶氧(mg/L)", typeof(string));
            dt.Columns.Add("pH值", typeof(string));
            dt.Columns.Add("投喂状态", typeof(string));
            dt.Columns.Add("健康评分", typeof(string));
            dt.Columns.Add("状态", typeof(string));

            dt.Rows.Add("A1", "大黄鱼", "23.5", "32.1", "7.2", "8.1", "已投喂", "95", "正常");
            dt.Rows.Add("A2", "大黄鱼", "22.8", "31.8", "6.9", "8.0", "已投喂", "92", "正常");
            dt.Rows.Add("A3", "石斑鱼", "24.1", "32.5", "7.5", "8.2", "未投喂", "98", "正常");
            dt.Rows.Add("B1", "金鲳鱼", "25.0", "33.0", "6.5", "7.9", "已投喂", "88", "注意");
            dt.Rows.Add("B2", "金鲳鱼", "24.6", "32.8", "7.0", "8.1", "已投喂", "91", "正常");
            dt.Rows.Add("B3", "军曹鱼", "23.2", "31.5", "7.8", "8.3", "未投喂", "96", "正常");
            dt.Rows.Add("C1", "鲈鱼", "22.5", "31.2", "6.3", "7.8", "已投喂", "82", "注意");
            dt.Rows.Add("C2", "鲈鱼", "23.0", "32.0", "7.1", "8.0", "已投喂", "93", "正常");

            return dt;
        }

        // 病虫防治数据表
        public static DataTable GetDiseaseControlTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("池号", typeof(string));
            dt.Columns.Add("检测时间", typeof(string));
            dt.Columns.Add("病害类型", typeof(string));
            dt.Columns.Add("风险等级", typeof(string));
            dt.Columns.Add("处理措施", typeof(string));
            dt.Columns.Add("负责人", typeof(string));
            dt.Columns.Add("状态", typeof(string));

            dt.Rows.Add("A1", "2026-06-09 08:30", "白点病", "低风险", "盐度调节+升温", "张工", "已处理");
            dt.Rows.Add("B1", "2026-06-09 09:15", "车轮虫", "中风险", "淡水浴+药浴", "李工", "处理中");
            dt.Rows.Add("C1", "2026-06-09 10:00", "肠炎", "低风险", "饲料调整+益生菌", "王工", "观察中");
            dt.Rows.Add("A3", "2026-06-09 07:45", "弧菌感染", "高风险", "消毒隔离+抗生素", "陈工", "紧急处理");
            dt.Rows.Add("B2", "2026-06-09 11:20", "锚头蚤", "低风险", "有机磷药浴", "张工", "已处理");
            dt.Rows.Add("C2", "2026-06-08 16:00", "水霉病", "中风险", "食盐浸泡+升温", "李工", "恢复中");

            return dt;
        }

        // 利润分析数据
        public static DataTable GetProfitTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("品种", typeof(string));
            dt.Columns.Add("养殖批次", typeof(string));
            dt.Columns.Add("投入(万元)", typeof(string));
            dt.Columns.Add("预计产出(万元)", typeof(string));
            dt.Columns.Add("利润率(%)", typeof(string));
            dt.Columns.Add("存活率(%)", typeof(string));
            dt.Columns.Add("周期(月)", typeof(string));
            dt.Columns.Add("评价", typeof(string));

            dt.Rows.Add("大黄鱼", "2026-A1", "12.5", "18.8", "50.4", "96.2", "8", "优质");
            dt.Rows.Add("石斑鱼", "2026-A3", "15.0", "24.0", "60.0", "98.1", "10", "优质");
            dt.Rows.Add("金鲳鱼", "2026-B1", "8.5", "12.8", "50.6", "88.5", "6", "良好");
            dt.Rows.Add("军曹鱼", "2026-B3", "20.0", "28.5", "42.5", "97.0", "12", "良好");
            dt.Rows.Add("鲈鱼", "2026-C1", "10.0", "14.2", "42.0", "82.0", "7", "关注");

            return dt;
        }

        // 标准值设定
        public static DataTable GetStandardTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("参数名称", typeof(string));
            dt.Columns.Add("标准下限", typeof(string));
            dt.Columns.Add("标准上限", typeof(string));
            dt.Columns.Add("单位", typeof(string));
            dt.Columns.Add("适用品种", typeof(string));
            dt.Columns.Add("告警级别", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));

            dt.Rows.Add("水温", "18.0", "28.0", "°C", "通用", "二级告警", "2026-05-01");
            dt.Rows.Add("盐度", "28.0", "35.0", "ppt", "海水鱼", "一级告警", "2026-05-01");
            dt.Rows.Add("溶解氧", "5.0", "9.0", "mg/L", "通用", "一级告警", "2026-05-01");
            dt.Rows.Add("pH值", "7.5", "8.5", "", "通用", "二级告警", "2026-05-01");
            dt.Rows.Add("氨氮", "0.0", "0.5", "mg/L", "通用", "一级告警", "2026-05-01");
            dt.Rows.Add("亚硝酸盐", "0.0", "0.1", "mg/L", "通用", "二级告警", "2026-05-01");
            dt.Rows.Add("透明度", "30", "80", "cm", "通用", "三级告警", "2026-05-01");

            return dt;
        }

        // 实时查询数据
        public static DataTable GetRealtimeQueryTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("传感器ID", typeof(string));
            dt.Columns.Add("池号", typeof(string));
            dt.Columns.Add("参数", typeof(string));
            dt.Columns.Add("当前值", typeof(string));
            dt.Columns.Add("标准范围", typeof(string));
            dt.Columns.Add("采样时间", typeof(string));
            dt.Columns.Add("数据来源", typeof(string));
            dt.Columns.Add("状态", typeof(string));

            dt.Rows.Add("BD-SN-001", "A1", "水温", "23.5", "18-28°C", "2026-06-09 14:30", "北斗+传感", "正常");
            dt.Rows.Add("BD-SN-002", "A1", "盐度", "32.1", "28-35ppt", "2026-06-09 14:30", "北斗+传感", "正常");
            dt.Rows.Add("BD-SN-003", "A1", "溶氧", "7.2", "5-9mg/L", "2026-06-09 14:30", "北斗+传感", "正常");
            dt.Rows.Add("BD-SN-004", "A2", "水温", "22.8", "18-28°C", "2026-06-09 14:30", "北斗+传感", "正常");
            dt.Rows.Add("BD-SN-005", "B1", "溶氧", "4.8", "5-9mg/L", "2026-06-09 14:30", "北斗+传感", "偏低");
            dt.Rows.Add("BD-SN-006", "C1", "pH", "7.4", "7.5-8.5", "2026-06-09 14:30", "北斗+传感", "偏低");
            dt.Rows.Add("BD-SN-007", "A3", "氨氮", "0.12", "0-0.5mg/L", "2026-06-09 14:30", "北斗+传感", "正常");
            dt.Rows.Add("BD-SN-008", "B3", "水温", "23.2", "18-28°C", "2026-06-09 14:30", "北斗+传感", "正常");

            return dt;
        }

        // 历史数据查询
        public static DataTable GetHistoryTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("记录时间", typeof(string));
            dt.Columns.Add("池号", typeof(string));
            dt.Columns.Add("水温(°C)", typeof(string));
            dt.Columns.Add("盐度(ppt)", typeof(string));
            dt.Columns.Add("溶氧(mg/L)", typeof(string));
            dt.Columns.Add("pH值", typeof(string));
            dt.Columns.Add("氨氮(mg/L)", typeof(string));
            dt.Columns.Add("操作员", typeof(string));

            dt.Rows.Add("2026-06-09 14:00", "A1", "23.5", "32.1", "7.2", "8.1", "0.10", "系统");
            dt.Rows.Add("2026-06-09 12:00", "A1", "23.8", "32.0", "7.0", "8.1", "0.11", "系统");
            dt.Rows.Add("2026-06-09 10:00", "A1", "24.0", "31.9", "6.8", "8.0", "0.12", "系统");
            dt.Rows.Add("2026-06-09 08:00", "A1", "23.2", "32.2", "7.3", "8.2", "0.09", "系统");
            dt.Rows.Add("2026-06-09 06:00", "A1", "22.5", "32.3", "7.5", "8.2", "0.08", "系统");
            dt.Rows.Add("2026-06-08 22:00", "A1", "22.0", "32.5", "7.6", "8.3", "0.07", "系统");
            dt.Rows.Add("2026-06-08 18:00", "A1", "23.0", "32.1", "7.1", "8.1", "0.10", "系统");
            dt.Rows.Add("2026-06-08 14:00", "A1", "24.2", "31.8", "6.6", "8.0", "0.13", "系统");

            return dt;
        }

        // 控制形式设置
        public static DataTable GetControlModeTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("设备名称", typeof(string));
            dt.Columns.Add("设备类型", typeof(string));
            dt.Columns.Add("控制模式", typeof(string));
            dt.Columns.Add("关联池号", typeof(string));
            dt.Columns.Add("当前状态", typeof(string));
            dt.Columns.Add("运行功率", typeof(string));
            dt.Columns.Add("最近操作", typeof(string));
            dt.Columns.Add("操作人", typeof(string));

            dt.Rows.Add("增氧机-A1", "增氧设备", "自动", "A1,A2", "运行中", "2.2kW", "2026-06-09 13:00", "系统");
            dt.Rows.Add("投饵机-B1", "投喂设备", "定时", "B1,B2", "待机", "0.8kW", "2026-06-09 12:00", "张工");
            dt.Rows.Add("水泵-A区", "循环设备", "手动", "A1,A2,A3", "运行中", "3.0kW", "2026-06-09 08:00", "李工");
            dt.Rows.Add("温控-B区", "温控设备", "自动", "B1,B2,B3", "停止", "5.0kW", "2026-06-08 22:00", "系统");
            dt.Rows.Add("照明-全区", "照明设备", "定时", "全部", "关闭", "1.5kW", "2026-06-09 06:00", "系统");
            dt.Rows.Add("监测浮标-01", "北斗传感器", "自动", "外海", "运行中", "0.2kW", "2026-06-09 14:30", "系统");

            return dt;
        }

        // 控制时间设置
        public static DataTable GetControlScheduleTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("任务名称", typeof(string));
            dt.Columns.Add("设备", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("结束时间", typeof(string));
            dt.Columns.Add("周期", typeof(string));
            dt.Columns.Add("关联池号", typeof(string));
            dt.Columns.Add("优先级", typeof(string));
            dt.Columns.Add("状态", typeof(string));

            dt.Rows.Add("晨间投喂", "投饵机-A1", "06:30", "07:00", "每日", "A1,A2", "高", "启用");
            dt.Rows.Add("午间投喂", "投饵机-B1", "11:30", "12:00", "每日", "B1,B2", "高", "启用");
            dt.Rows.Add("晚间投喂", "投饵机-A1", "17:30", "18:00", "每日", "A1,A2,A3", "高", "启用");
            dt.Rows.Add("增氧循环", "增氧机-A1", "00:00", "23:59", "每日", "A1,A2", "中", "启用");
            dt.Rows.Add("水质采样", "监测浮标-01", "08:00", "08:30", "每4小时", "外海", "高", "启用");
            dt.Rows.Add("数据上报", "北斗通信", "09:00", "09:15", "每日", "全部", "高", "启用");
            dt.Rows.Add("消毒作业", "水泵-A区", "02:00", "04:00", "每周", "A1,A2,A3", "中", "启用");
            dt.Rows.Add("夜间照明", "照明-全区", "18:00", "06:00", "每日", "全部", "低", "停用");

            return dt;
        }

        // 趋势图数据
        public static string[] GetTrendTimeLabels()
        {
            return new string[] { "06:00", "08:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00", "22:00" };
        }

        public static float[] GetTrendWaterTemp()
        {
            return new float[] { 22.5f, 23.2f, 24.0f, 24.5f, 23.8f, 23.5f, 23.0f, 22.8f, 22.0f };
        }

        public static float[] GetTrendDissolvedOxygen()
        {
            return new float[] { 7.5f, 7.3f, 6.8f, 6.5f, 7.0f, 7.2f, 7.4f, 7.6f, 7.8f };
        }

        public static float[] GetTrendPH()
        {
            return new float[] { 8.2f, 8.1f, 8.0f, 7.9f, 8.0f, 8.1f, 8.2f, 8.3f, 8.3f };
        }

        // 月度利润趋势
        public static string[] GetMonthLabels()
        {
            return new string[] { "1月", "2月", "3月", "4月", "5月", "6月" };
        }

        public static float[] GetMonthlyProfit()
        {
            return new float[] { 5.2f, 6.8f, 8.1f, 7.5f, 9.3f, 10.2f };
        }

        public static float[] GetMonthlyCost()
        {
            return new float[] { 8.0f, 7.5f, 9.0f, 8.2f, 8.8f, 9.5f };
        }
    }
}
