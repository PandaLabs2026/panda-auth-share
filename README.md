# panda-auth-share

PandaAuth 跨进程共享层：`PandaAuth.Shared`（枚举、端点常量、Claim 常量等对外契约）与 `PandaAuth.Sdk`（内部业务应用接入 SDK，Phase 1 实装）。

## 设计约束

- **不含数据库实体**——Users/Clients 是 panda-auth-server 的私有持久化模型，其他服务一律通过 Admin API / OIDC 协议访问
- `PandaAuth.Shared` 只放"多个仓库都要认识"的稳定契约，改动需要同步评估 server / webadmin / 业务端三个消费方

## 构建

```bash
dotnet build PandaAuth.Shared.slnx
```

克隆约定：与 panda-auth-server 等仓同级目录克隆（工作区布局见 panda-auth 元仓库 WORKSPACE.md）。
