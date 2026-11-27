# panda-auth-share

**PandaAuth by PandaLabs** · [English](README.md)

**官方主页 [pandalabs.cc](https://pandalabs.cc/products/panda-auth/)** · 中文官网 [pandalabs.cn](https://pandalabs.cn/products/panda-auth/) · [PandaLabs 产品矩阵](https://pandalabs.cc/products/)

> PandaAuth 套件当前为 **Community Preview 0.2.0-preview.1**：已部署生产、面向早期社区试用；稳定版 Community Release 1.0.0 尚未发布。接入采用邀请或申请口径。

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

已有共享常量和模型定义，不代表已有完整客户端接入库。本仓组件包当前为 **0.2.0-preview.1** 源码候选；首个正式市场版前遵循组织0.x政策。包内包含本README及MIT许可证正文/元数据，公开供包另需发行接受；旧1.x材料保留历史身份。变化影响见各消费仓。

## 构建

需要 .NET SDK，版本选择见本仓 [global.json](global.json)（10.0.404，精确匹配稳定SDK）。本仓可脱离私有元仓独立构建。以下命令在本仓根目录执行。

```bash
dotnet build PandaAuth.Shared.slnx
```

这是库仓，没有独立服务端口或运行入口。对外契约变化应按兼容影响选择 SemVer，并更新消费方说明。

## Roadmap 与治理

产品级路线图、发行门禁和社区/商业边界在正式公开发行前仍由维护者治理；本 README 只描述可独立复现的 Share 构建边界。

- [安全政策](SECURITY.md)：选定私密报告渠道，启用状态未核验；不公开提交漏洞细节。
- [贡献指南](CONTRIBUTING.md)：本仓检查与统一贡献规则。
- [MIT License](LICENSE)：适用于自有代码和文档，具体范围见[许可说明](LICENSING.md)；第三方许可仍适用，品牌图片除外。

新增本人资料契约由 `PandaAuthProfileApi`、`ProfileScopes` 与私有 DTO 导出。PATCH 保留缺省与显式 null 的区别；Server 仍须验证重复属性、值及授权。这些类型不代表端点已启用或客户端已获 scope，见[拥有方任务 #323](https://github.com/PandaLabs2026/panda-auth/issues/323)。

## 当前预发布版本与F2主机契约

当前源码候选0.2.0-preview.1；包消费显式精确选择 `[0.2.0-preview.1]`。F2已收口：For输出pandacloud.cc，TryParse拒绝退役pandalabs.cn后缀；TenantContext仍要求规范地址。PandaMade的RetiredCloudIssuer不在KnownIssuers。版本重定基不恢复退役主机或扩大issuer/redirect信任。

Pandy更名并存窗口内（meta ADR-092），`TenantCanonicalHost.TryParse`在`-asst`之外同时把`-pandy`标签解析为`TenantProduct.PandaAssistant`。激活批次前`For`仍输出`-asst`，`TenantContext`仍要求该规范地址。`TenantCanonicalHost.AcceptedHosts`列出解析对给定租户、产品与分区接受的全部主机形态，规范形态在前；需要比对或登记回调主机的调用方用它代替单一的`For`结果。此源码新增不改变已发布的包。
