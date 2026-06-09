# 技术方案

## 需求概述

在 `D:\Users\bubble\Desktop\bubble_test\C#基于北斗时空数据的深远海养殖辅助分析终端软件` 中实现一个好看的 C# 客户端程序，用于展示“基于北斗时空数据的深远海养殖辅助分析终端软件 V1.0”的桌面终端界面，并可用于后续软著补正截图。

## 技术方案

本机当前未确认可用 .NET SDK，优先采用 Windows 自带 .NET Framework 编译器 `csc.exe` 可编译的 WinForms 客户端方案。

技术路线：

- 语言：C#
- UI：Windows Forms
- 目标运行环境：Windows 10/Windows 11
- 编译方式：`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`
- 入口文件：`Program.cs`
- 主界面：`MainForm.cs`
- 主题样式：`AppTheme.cs`
- 演示数据：`DemoData.cs`

界面风格：

- 桌面终端软件风格，不做 Web 后台。
- 深海蓝顶部栏 + 左侧模块导航 + 主工作区。
- 卡片、表格、状态标签、时间线、控制按钮组合。
- 信息密度适中，截图看起来像真实业务系统。

核心页面：

1. 养殖池整体监控
2. 实时视频监控
3. 病虫防治
4. 利润分析
5. 标准值设定
6. 数据实时查询
7. 趋势图查询
8. 历史数据查询
9. 控制形式设置
10. 控制时间设置

## 影响范围

- 新增项目源代码文件。
- 新增编译脚本。
- 新增运行说明。
- 不影响 `D:\Users\bubble\Desktop\软著（补）` 下现有补正材料，除非用户后续要求同步截图或文档。

## 风险点

- 风险：本机没有 .NET SDK，不能使用现代 WPF/WinUI。
  - 缓解：使用 .NET Framework WinForms，使用系统 `csc.exe` 编译。

- 风险：界面过于简单像模板。
  - 缓解：加入北斗授时、养殖池状态、传感器指标、病虫风险、利润、控制计划等专用业务元素。

- 风险：截图与软著补正2的 C# 源码模块不对应。
  - 缓解：页面模块命名和字段与 `软著补正2-v2` 的 C# 材料保持一致。

## 依赖

- Windows Forms / System.Drawing / System.Windows.Forms
- Windows 自带 .NET Framework 编译器 `csc.exe`
- 无需外部 NuGet 包
