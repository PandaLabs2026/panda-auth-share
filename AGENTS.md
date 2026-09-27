# panda-auth-share 协作规则

PandaAuth 跨进程共享契约库。共享规则、七仓布局和跨仓发布流程见同级元仓 `../panda-auth/AGENTS.md` 与 `WORKSPACE.md`。

- 只放稳定的枚举、端点常量、Claim 常量和跨进程契约；不放数据库实体、持久化实现、服务业务逻辑或 SDK 行为。
- 修改契约前检索 Server、SDK、Website、Admin、Me 的消费者；路径/Claim 改动按破坏性变更处理，并同步版本说明。
- 不提交密码、Token、私钥、env 内容或生产配置。
- 验证：`dotnet build PandaAuth.Shared.slnx`、`git diff --check`。
