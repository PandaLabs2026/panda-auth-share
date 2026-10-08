# PandaAuth Share 回滚说明

## 适用范围

Share 是被多仓引用的跨进程契约库，不是常驻服务；契约职责见 [Share README](../README.md)，六仓布局及消费者关系见 [PandaAuth WORKSPACE](../../panda-auth/WORKSPACE.md)，私有子仓约定见 [Share 子仓规则](../../panda-auth/docs/agents/panda-auth-share.md)。

## 消费者侧回退

- 变更后发现兼容问题时，先列出已更新的 Server、SDK、Admin、Me 等消费者及其 Share 提交/版本。多数消费者从同级 Share 源码建立 `ProjectReference`，因此只回退 Share 会使消费者编译或运行契约不一致。
- 将 Share 与受影响消费者配套恢复到彼此兼容、已验证的提交/版本；消费者仓分别按自己的门禁重建和验证，再依各自发布治理部署。若某一消费者已使用新契约，先规划兼容过渡，不可只移动 Share 的默认分支。
- 本仓没有独立服务镜像、数据库、运行时 volume 或生产部署回退。运行态恢复责任属于消费者与其 owning project。

## 验收与证据

记录变更涉及的消费者、对齐后的 Share/消费者提交或版本以及各消费者验证结果。契约有破坏性影响时按元仓规则补充版本说明与兼容决策。
