# panda-auth-share

PandaAuth 跨进程共享契约层：`PandaAuth.Shared`（枚举、端点常量、Claim 常量等）。

## 设计约束

- **不含数据库实体**——Users/Clients 是 panda-auth-server 的私有持久化模型，其他服务一律通过 Admin API / OIDC 协议访问
- **不含 SDK**——接入 SDK 已独立为 [panda-auth-sdk](../panda-auth-sdk)（消费方多、需要独立 NuGet 版本节奏）；本仓只放"多个仓库都要认识"的最稳定契约，改动需同步评估 server / webadmin / me / 业务端等消费方

## 构建

```bash
dotnet build PandaAuth.Shared.slnx
```

克隆约定：与 panda-auth-server 等仓同级目录克隆（工作区布局见元仓 WORKSPACE.md）。
