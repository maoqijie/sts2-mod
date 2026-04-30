# sts2-mod

开源《杀戮尖塔 2》Mod 工作区。

## 当前设计

当前角色方向是“捧腹奶龙”，核心机制是“表情姿态”。初步定稿见 [docs/pengfu-nailong-design.md](docs/pengfu-nailong-design.md)。

`PengfuNailongMod` 是当前实现工程，采用 loose-file 迭代模式（`has_pck=false`）。

## 构建

```bash
dotnet build "PengfuNailongMod.csproj"
```

## 仓库规则

- 手写文件不得超过 400 行。
- 文件变大前先按职责拆分，保持模块解耦。
- 不提交私有 Codex skill、本地游戏安装文件、日志、密钥或构建产物。
- 每次变更使用最小相关构建或运行时检查验证。

## 私有工具

本机 `sts2-modding` Codex skill 是私有工具，不属于这个开源仓库。

## 特别鸣谢

- [FastAiCode](https://new.fastaicode.top)
