# panda-auth-share 协作规则

## 职责与边界

本仓是 PandaAuth 跨进程共享契约库，只维护稳定的枚举、端点常量、Claim 常量及跨进程契约；不放数据库实体、持久化实现、服务业务逻辑或 SDK 行为。七仓布局与共享治理见元仓 `../panda-auth/WORKSPACE.md`、`../panda-auth/AGENTS.md`。

## 跨仓来源与安全

- Server、SDK、Website、Admin、Me 是消费者。修改契约前检索全部消费者；端点、Claim 或序列化形状变化按破坏性变更审查，并同步版本说明。
- 契约正本在本仓；消费者引用本仓契约，不复制字面量或从消费实现反推契约。
- 不提交密码、Token、私钥、真实连接串、env 内容或生产配置。
- 本仓无生成代码；不要手改其他仓生成物，品牌资产按元仓品牌管线流转。

## 验证

从本仓根目录运行：

```bash
dotnet build PandaAuth.Shared.slnx
git diff --check
```

当前 solution 只包含 `src/PandaAuth.Shared/PandaAuth.Shared.csproj`，没有测试项目；因此不声明本仓有独立测试命令。契约变更需另行构建/验证消费者仓，按 `WORKSPACE.md` 所列相对依赖执行。
