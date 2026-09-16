# panda-auth-share

**PandaAuth by PandaLabs** · [English](README.en.md)

> 研发阶段，尚无正式受支持发行版；接入采用邀请或申请口径。已有实现不等于已完成发行验证。

## 职责与边界

PandaAuth 跨进程共享契约层。源码位于 [PandaAuth.Shared](src/PandaAuth.Shared)，包含用户状态枚举、协议端点及 Claim 常量。

本仓不放数据库实体、持久化实现或 SDK。SDK 独立位于 [panda-auth-sdk](https://github.com/PandaLabs2026/panda-auth-sdk)；Server、SDK、Me 通过相对路径引用本仓，改动须核对消费者。

## 协议端点

对外端点契约即 `PandaAuthEndpoints` 中的常量（相对 Issuer 的路径），消费方不得自行复制字面量：

| 常量 | 路径 | 说明 |
| --- | --- | --- |
| `Authorization` | `/connect/authorize` | 授权端点 |
| `Token` | `/connect/token` | 令牌端点 |
| `Userinfo` | `/connect/userinfo` | 用户信息端点 |
| `Logout` | `/connect/logout` | 会话结束端点 |
| `Introspection` | `/connect/introspect` | 令牌自省端点 |
| `Revocation` | `/connect/revoke` | 令牌吊销端点 |
| `JsonWebKeySet` | `/.well-known/jwks` | 签名公钥集，即 discovery 文档的 `jwks_uri` |
| `OpenIdConfiguration` | `/.well-known/openid-configuration` | OIDC discovery 文档 |

上表取值已于 2026-09-16 逐条对生产 discovery 核验一致（8/8）。常量与路径一致不等于端点已通过协议测试。

## 当前实现与限制

已有共享常量和模型定义，不代表已有完整客户端接入库。本轮未验证跨仓构建或兼容组合；变化影响见各消费仓及能力矩阵。

## 构建

需要 .NET SDK，版本选择见本仓 [global.json](global.json)（当前请求 10.0.112，允许 latestFeature roll-forward）。七仓按[工作区布局](https://github.com/PandaLabs2026/panda-auth/blob/main/WORKSPACE.md)同级克隆，跨仓链接需要对应访问权限。以下命令在本仓根目录执行；本轮仅静态核对命令，未执行构建或启动。

```bash
dotnet build PandaAuth.Shared.slnx
```

这是库仓，没有独立服务端口或运行入口。对外契约变化应按兼容影响选择 SemVer，并更新消费方说明。

## Roadmap 与治理

实现目标见[能力矩阵](https://github.com/PandaLabs2026/panda-auth/blob/main/docs/open-source/capabilities.md)与[发布门禁](https://github.com/PandaLabs2026/panda-auth/blob/main/docs/open-source/release-readiness.md)。实际业务需求驱动路线图，社区请求按方向和维护成本评估，不承诺交付。[社区/商业边界](https://github.com/PandaLabs2026/panda-auth/blob/main/docs/open-source/strategy.md)表示能力归属，不代表商业模块已经交付。

- [安全政策](SECURITY.md)：选定私密报告渠道，启用状态未核验；不公开提交漏洞细节。
- [贡献指南](CONTRIBUTING.md)：本仓检查与统一贡献规则。
- [MIT License](LICENSE)：适用于自有代码和文档，具体范围见[许可说明](LICENSING.md)；第三方许可仍适用，品牌图片除外。
