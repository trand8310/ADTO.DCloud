# SignalR 在线用户（Redis）在服务关闭时未清理的治理方案（基于当前代码）

## 1. 问题根因（对应当前代码）

当前在线用户写入 Redis 由 `OnlineClientHubBase.OnConnectedAsync` 触发，断开删除由 `OnlineClientHubBase.OnDisconnectedAsync` 触发。正常断开时会调用 `OnlineClientManager.RemoveAsync`，再由 `RedisOnlineClientStore.TryRemoveInternalAsync` 从 Redis 的 hash/set 中清掉连接。

但当服务进程异常退出、容器被 kill、节点崩溃时，`OnDisconnectedAsync` 很可能不会被执行，导致 Redis 中残留僵尸连接。

## 2. 结合你现有实现，推荐采用“多层兜底”

### A. 启动时一次性清理“本节点遗留连接”

- 新增一个 HostedService（例如 `OnlineClientCleanupHostedService`），在 `StartAsync` 里做一次扫描清理。
- 扫描 `ADTOSharp.RealTime.OnlineClients.Clients`（你现在 `_clientStoreKey` 的默认 key）。
- 对每个 connectionId 通过 SignalR HubContext / ConnectionManager 判定是否仍存活（或判定所属节点是否为当前节点且已不存在）。
- 不存活则执行与 `TryRemoveInternalAsync` 同等的原子删除（同时删 hash + user set）。

> 这是“冷启动补偿”，解决上次异常退出留下的脏数据。

### B. 心跳 + TTL（核心）

- 在 `AddAsync` 时为每个连接写一个心跳 key，比如：
  - `ADTOSharp.RealTime.OnlineClients.Heartbeat:{connectionId}`
- 设置较短 TTL（如 60~120 秒）。
- 客户端每 20~30 秒调用一次轻量 hub 方法刷新 TTL。
- 后台清理任务周期性扫描 `Clients` hash：
  - 若 heartbeat key 不存在，则判定连接已失活，执行 Remove。

> 这可以避免“服务没收到断开事件”的天然缺陷，是分布式场景最稳妥方案。

### C. 进程优雅停机时主动清理（非强一致，但有价值）

- 利用你现有的 `IHostApplicationLifetime.ApplicationStopping` 挂钩（当前框架里已在 `UseADTOSharp` 使用）。
- 在 stop 回调中调用“按实例清理”逻辑，尽量在优雅停机窗口内删除连接。

> 对 `kill -9` / 节点宕机无效，所以必须和 B 组合。

## 3. 为了支持“按实例清理”，建议给 OnlineClient 增加实例标记

当前 `OnlineClient` 只有 connectionId/tenantId/userId，没有节点信息。建议在 `Properties` 增加：

- `instanceId`：应用实例唯一标识（启动时生成 GUID）
- `lastHeartbeatUtc`：最近心跳时间（可选）

这样在服务停机或重启时可只清理当前实例写入的数据，避免误删其他节点在线连接。

## 4. Redis 数据结构建议（兼容你当前结构）

在保留现有：
- `...OnlineClients.Clients`（hash）
- `...OnlineClients.Users:{tenant,user}`（set）

基础上新增：
- `...OnlineClients.Heartbeat:{connectionId}`（string + TTL）
- `...OnlineClients.Instance:{instanceId}`（set，保存本实例 connectionId）

删除连接时一次性删除：
1. `HDEL Clients connectionId`
2. `SREM Users:{...} connectionId`
3. `DEL Heartbeat:{connectionId}`
4. `SREM Instance:{instanceId} connectionId`

建议继续用 Lua 保证原子性。

## 5. 参数建议（可配置）

- `HeartbeatIntervalSeconds`: 30
- `HeartbeatTtlSeconds`: 90
- `CleanupScanBatchSize`: 500
- `StartupCleanupEnabled`: true
- `GracefulShutdownCleanupTimeoutSeconds`: 10

## 6. 风险与注意事项

- 心跳频率过高会增加 Redis 写压力，建议先 30s 起步。
- 清理任务要分批（SCAN/HSCAN）避免阻塞 Redis。
- 多节点部署下，必须避免“误删别人节点的连接”；instanceId 是关键。
- 用户多端登录时，同一用户会有多个 connectionId，不可按 user 直接删。

## 7. 落地顺序（最小风险）

1. 先加“心跳 key + 后台清理任务”（收益最大）
2. 再加“实例维度索引 + 停机清理”
3. 最后补“启动补偿清理”与监控指标（僵尸连接数、清理次数、误删率）

---

如果你认可这套方案，下一步我可以按你当前项目结构给出**具体改造点清单（到类/方法级）**，包括：
- 要改哪些文件
- 每个文件加哪些方法
- Lua 脚本如何调整
- appsettings 增加哪些配置项
