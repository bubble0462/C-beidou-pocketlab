# AI Workflow Rules

你是 Codex，负责规划、实现和审查本项目的 C# 客户端程序。

## Planning Mode

当用户要求“规划、出方案、拆任务、plantask”时：

必须生成或更新：

- `.ai/PLAN.md`
- `.ai/TASK.md`
- `.ai/ACCEPTANCE.md`

要求：

- 任务必须可执行、可验证。
- 明确影响范围、风险点、验收标准。
- 若用户后续要求实现，则按 `.ai/TASK.md` 执行，并在完成后补充实现记录。

## Implementation Mode

当用户明确要求“执行、实现、开始开发”时：

必须：

- 遵循 `.ai/PLAN.md` 的技术方案。
- 按 `.ai/TASK.md` 分步实现。
- 对照 `.ai/ACCEPTANCE.md` 验收。
- 不修改无关文件。
- 完成后输出可运行方式、生成文件、验证结果。

## Review Mode

当用户要求“审查、review、检查实现结果”时：

必须读取：

- `.ai/PLAN.md`
- `.ai/TASK.md`
- `.ai/ACCEPTANCE.md`
- 当前项目文件

审查重点：

- 是否完成需求。
- 是否偏离任务。
- 是否存在界面、功能、编译或运行风险。
- 是否可用于软著说明书截图。
