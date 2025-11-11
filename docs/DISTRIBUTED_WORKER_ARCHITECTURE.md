# Distributed Worker Architecture for Engagement Automation

## Tổng Quan

Thay vì 1 app monolithic theo dõi tất cả bài viết, chúng ta sử dụng **distributed worker pattern**:

```
100 bài viết = 100 worker instances
Mỗi worker = 1 bài viết = Đơn giản & Scalable
```

### Lợi Ích So Với Monolithic

| Aspect | Monolithic (1 App) | Distributed Workers |
|--------|-------------------|---------------------|
| **Scalability** | ❌ Hard to scale | ✅ Scale by adding workers |
| **Fault Tolerance** | ❌ 1 error affects all | ✅ Isolated failures |
| **Resource Usage** | ❌ Heavy on 1 machine | ✅ Distributed across machines |
| **Complexity per Worker** | ❌ Complex | ✅ Simple, focused |
| **Deployment** | ❌ Monolithic deploy | ✅ Rolling updates |
| **Cost** | ❌ Powerful server needed | ✅ Many cheap workers |

---

## 1. ARCHITECTURE OVERVIEW

### 1.1 System Components

```
┌──────────────────────────────────────────────────────────────┐
│                    MASTER COORDINATOR                         │
│  - Quản lý danh sách bài viết                                │
│  - Phân phối jobs cho workers                                │
│  - Monitor worker health                                      │
│  - Aggregate metrics                                          │
└─────────────────────┬────────────────────────────────────────┘
                      │
         ┌────────────┼────────────┐
         ▼            ▼            ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│  Worker #1  │ │  Worker #2  │ │  Worker #N  │
│             │ │             │ │             │
│ Post ID: 1  │ │ Post ID: 2  │ │ Post ID: N  │
│             │ │             │ │             │
│ Monitor     │ │ Monitor     │ │ Monitor     │
│ Comments    │ │ Comments    │ │ Comments    │
│ Auto Reply  │ │ Auto Reply  │ │ Auto Reply  │
│ Auto Like   │ │ Auto Like   │ │ Auto Like   │
└─────────────┘ └─────────────┘ └─────────────┘
         │            │            │
         └────────────┼────────────┘
                      ▼
         ┌────────────────────────┐
         │   Shared Database      │
         │   Message Queue        │
         │   Metrics Store        │
         └────────────────────────┘
```

### 1.2 Worker Lifecycle

```
[Master assigns Post ID]
        ↓
[Worker starts & registers]
        ↓
[Poll/Subscribe to post comments]
        ↓
[Process each comment]
   ├─ Analyze sentiment
   ├─ Generate AI reply
   ├─ Post reply
   └─ Auto like
        ↓
[Report metrics to Master]
        ↓
[Heartbeat check]
        ↓
[Shutdown when post inactive or timeout]
```

---

## 2. WORKER IMPLEMENTATION

### 2.1 Simple Worker Structure

```csharp
public class PostWorker
{
    private readonly string _postId;
    private readonly string _platform;
    private readonly ISocialMediaClient _client;
    private readonly AIReplyGenerator _aiReply;
    private readonly IMessageQueue _queue;
    private readonly WorkerConfig _config;
    private CancellationTokenSource _cts;

    public PostWorker(string postId, string platform, WorkerConfig config)
    {
        _postId = postId;
        _platform = platform;
        _config = config;
        _cts = new CancellationTokenSource();
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"[Worker {_postId}] Starting...");

        try
        {
            // Register với Master
            await RegisterWithMasterAsync();

            // Main loop
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    // 1. Fetch new comments
                    var comments = await FetchNewCommentsAsync();

                    // 2. Process each comment
                    foreach (var comment in comments)
                    {
                        await ProcessCommentAsync(comment);
                    }

                    // 3. Auto-like recent comments
                    await AutoLikeCommentsAsync();

                    // 4. Send heartbeat
                    await SendHeartbeatAsync();

                    // 5. Wait before next poll
                    await Task.Delay(_config.PollIntervalMs, _cts.Token);
                }
                catch (Exception ex)
                {
                    await LogErrorAsync(ex);
                    await Task.Delay(5000); // Wait before retry
                }
            }
        }
        finally
        {
            await UnregisterAsync();
            Console.WriteLine($"[Worker {_postId}] Stopped.");
        }
    }

    private async Task<List<Comment>> FetchNewCommentsAsync()
    {
        var comments = await _client.GetCommentsAsync(_postId);

        // Filter: Only new comments since last check
        var lastCheckTime = await GetLastCheckTimeAsync();
        return comments
            .Where(c => c.CreatedAt > lastCheckTime)
            .ToList();
    }

    private async Task ProcessCommentAsync(Comment comment)
    {
        Console.WriteLine($"[Worker {_postId}] Processing comment: {comment.Id}");

        // 1. Check if already processed
        if (await IsProcessedAsync(comment.Id))
            return;

        // 2. Analyze sentiment
        var sentiment = await _aiReply.AnalyzeSentimentAsync(comment.Text);

        // 3. Decide action
        if (sentiment.RequiresHuman)
        {
            await NotifyHumanAsync(comment, sentiment);
            return;
        }

        // 4. Generate reply
        var reply = await _aiReply.GenerateReplyAsync(comment, sentiment);

        // 5. Post reply
        await _client.ReplyToCommentAsync(comment.Id, reply);

        // 6. Auto-like the comment
        await _client.LikeCommentAsync(comment.Id);

        // 7. Mark as processed
        await MarkAsProcessedAsync(comment.Id);

        // 8. Report metrics
        await ReportMetricAsync(new CommentMetric
        {
            WorkerId = _postId,
            CommentId = comment.Id,
            Action = "replied",
            Timestamp = DateTime.UtcNow
        });
    }

    private async Task AutoLikeCommentsAsync()
    {
        var recentComments = await _client.GetCommentsAsync(_postId, limit: 50);

        foreach (var comment in recentComments)
        {
            if (!await HasLikedAsync(comment.Id))
            {
                await _client.LikeCommentAsync(comment.Id);
                await Task.Delay(2000); // Rate limiting
            }
        }
    }

    private async Task SendHeartbeatAsync()
    {
        await _queue.PublishAsync("worker.heartbeat", new
        {
            WorkerId = _postId,
            Platform = _platform,
            Status = "active",
            Timestamp = DateTime.UtcNow
        });
    }

    public void Stop()
    {
        _cts.Cancel();
    }
}
```

### 2.2 Worker Configuration

```csharp
public class WorkerConfig
{
    public int PollIntervalMs { get; set; } = 30000; // 30 seconds
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 5000;
    public bool AutoLikeEnabled { get; set; } = true;
    public bool AutoReplyEnabled { get; set; } = true;
    public int MaxCommentsPerCycle { get; set; } = 10;
    public TimeSpan WorkerTimeout { get; set; } = TimeSpan.FromHours(24);
}
```

---

## 3. MASTER COORDINATOR

### 3.1 Master Service

```csharp
public class MasterCoordinator
{
    private readonly IRepository<PostedContent> _postRepo;
    private readonly IMessageQueue _queue;
    private readonly Dictionary<string, WorkerInfo> _activeWorkers;
    private readonly object _lock = new object();

    public async Task RunAsync()
    {
        Console.WriteLine("[Master] Starting coordinator...");

        // Start background tasks
        var tasks = new[]
        {
            MonitorWorkersAsync(),
            DistributeWorkAsync(),
            CollectMetricsAsync()
        };

        await Task.WhenAll(tasks);
    }

    private async Task DistributeWorkAsync()
    {
        while (true)
        {
            try
            {
                // 1. Get active posts (posted in last 24 hours)
                var activePosts = await _postRepo.FindAsync(p =>
                    p.PostedAt > DateTime.UtcNow.AddHours(-24) &&
                    p.Status == ContentStatus.Posted
                );

                foreach (var post in activePosts)
                {
                    // 2. Check if worker already exists
                    if (HasActiveWorker(post.Id))
                        continue;

                    // 3. Spawn new worker
                    await SpawnWorkerAsync(post);
                }

                await Task.Delay(TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Master] Error distributing work: {ex.Message}");
            }
        }
    }

    private async Task SpawnWorkerAsync(PostedContent post)
    {
        Console.WriteLine($"[Master] Spawning worker for post: {post.Id}");

        var workerInfo = new WorkerInfo
        {
            WorkerId = post.Id.ToString(),
            PostId = post.PlatformPostId,
            Platform = post.Platform,
            StartedAt = DateTime.UtcNow,
            Status = WorkerStatus.Starting
        };

        lock (_lock)
        {
            _activeWorkers[workerInfo.WorkerId] = workerInfo;
        }

        // Publish work assignment
        await _queue.PublishAsync("work.assign", new
        {
            WorkerId = workerInfo.WorkerId,
            PostId = post.PlatformPostId,
            Platform = post.Platform,
            Config = new WorkerConfig()
        });

        // Alternative: Start worker process directly
        // await StartWorkerProcessAsync(workerInfo);
    }

    private async Task MonitorWorkersAsync()
    {
        while (true)
        {
            try
            {
                var now = DateTime.UtcNow;

                lock (_lock)
                {
                    var deadWorkers = _activeWorkers
                        .Where(w => now - w.Value.LastHeartbeat > TimeSpan.FromMinutes(5))
                        .ToList();

                    foreach (var worker in deadWorkers)
                    {
                        Console.WriteLine($"[Master] Worker {worker.Key} is dead. Removing...");
                        _activeWorkers.Remove(worker.Key);

                        // Optionally restart
                        // await RestartWorkerAsync(worker.Value);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Master] Error monitoring: {ex.Message}");
            }
        }
    }

    private async Task CollectMetricsAsync()
    {
        // Subscribe to metrics from workers
        await _queue.SubscribeAsync("metrics.*", async (message) =>
        {
            var metric = JsonSerializer.Deserialize<WorkerMetric>(message);
            await SaveMetricAsync(metric);
        });
    }

    public async Task<WorkerStats> GetStatsAsync()
    {
        lock (_lock)
        {
            return new WorkerStats
            {
                TotalWorkers = _activeWorkers.Count,
                ActiveWorkers = _activeWorkers.Count(w => w.Value.Status == WorkerStatus.Active),
                IdleWorkers = _activeWorkers.Count(w => w.Value.Status == WorkerStatus.Idle),
                DeadWorkers = 0,
                Platforms = _activeWorkers
                    .GroupBy(w => w.Value.Platform)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
    }
}

public class WorkerInfo
{
    public string WorkerId { get; set; }
    public string PostId { get; set; }
    public string Platform { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;
    public WorkerStatus Status { get; set; }
    public int CommentsProcessed { get; set; }
    public int ErrorCount { get; set; }
}

public enum WorkerStatus
{
    Starting,
    Active,
    Idle,
    Error,
    Stopping
}
```

---

## 4. DEPLOYMENT STRATEGIES

### 4.1 Option 1: Separate Processes (Simple)

Mỗi worker là 1 process riêng trên cùng máy.

```csharp
public class ProcessWorkerLauncher
{
    public async Task StartWorkerAsync(string postId, string platform)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "AiSocialPost.Worker.exe",
            Arguments = $"--post-id {postId} --platform {platform}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = Process.Start(processInfo);

        // Monitor output
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"[Worker {postId}] {e.Data}");
        };

        process.BeginOutputReadLine();

        await process.WaitForExitAsync();
    }
}

// Worker main program
public class Program
{
    public static async Task Main(string[] args)
    {
        var postId = GetArgument(args, "--post-id");
        var platform = GetArgument(args, "--platform");

        var worker = new PostWorker(postId, platform, new WorkerConfig());
        await worker.RunAsync();
    }
}
```

**Pros:**
- ✅ Simple to implement
- ✅ Easy debugging
- ✅ Process isolation

**Cons:**
- ❌ Limited to 1 machine
- ❌ High overhead (many processes)

---

### 4.2 Option 2: Docker Containers (Recommended)

Mỗi worker là 1 Docker container.

#### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:6.0

WORKDIR /app
COPY bin/Release/net6.0/publish/ .

ENTRYPOINT ["dotnet", "AiSocialPost.Worker.dll"]
```

#### Docker Compose

```yaml
version: '3.8'

services:
  master:
    image: aisocialpost/master:latest
    environment:
      - DATABASE_URL=postgresql://user:pass@db:5432/aisocial
      - RABBITMQ_URL=amqp://rabbitmq:5672
    depends_on:
      - db
      - rabbitmq
    ports:
      - "8080:80"

  worker:
    image: aisocialpost/worker:latest
    environment:
      - DATABASE_URL=postgresql://user:pass@db:5432/aisocial
      - RABBITMQ_URL=amqp://rabbitmq:5672
    depends_on:
      - db
      - rabbitmq
    deploy:
      replicas: 10  # Start 10 workers initially
      resources:
        limits:
          cpus: '0.5'
          memory: 512M

  db:
    image: postgres:15
    environment:
      - POSTGRES_USER=user
      - POSTGRES_PASSWORD=pass
      - POSTGRES_DB=aisocial
    volumes:
      - db-data:/var/lib/postgresql/data

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"

volumes:
  db-data:
```

#### Kubernetes (Advanced)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: worker-deployment
spec:
  replicas: 100  # 100 workers
  selector:
    matchLabels:
      app: aisocialpost-worker
  template:
    metadata:
      labels:
        app: aisocialpost-worker
    spec:
      containers:
      - name: worker
        image: aisocialpost/worker:latest
        env:
        - name: DATABASE_URL
          valueFrom:
            secretKeyRef:
              name: db-secret
              key: url
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
```

**Dynamic Scaling:**
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: worker-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: worker-deployment
  minReplicas: 10
  maxReplicas: 200
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

**Pros:**
- ✅ Easy to scale (spin up more containers)
- ✅ Resource isolation
- ✅ Can run on multiple machines
- ✅ Auto-restart on failure
- ✅ Health checks built-in

**Cons:**
- ⚠️ Requires Docker/K8s knowledge
- ⚠️ More complex setup

---

### 4.3 Option 3: Serverless (AWS Lambda / Azure Functions)

Mỗi worker là 1 serverless function.

```csharp
public class WorkerFunction
{
    [FunctionName("ProcessPost")]
    public async Task Run(
        [QueueTrigger("post-assignments")] PostAssignment assignment,
        ILogger log)
    {
        log.LogInformation($"Processing post: {assignment.PostId}");

        var worker = new PostWorker(
            assignment.PostId,
            assignment.Platform,
            new WorkerConfig()
        );

        // Run for limited time (Lambda max 15 mins)
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(14));
        await worker.RunAsync(cts.Token);
    }
}
```

**Pros:**
- ✅ Zero infrastructure management
- ✅ Auto-scaling
- ✅ Pay per execution

**Cons:**
- ❌ Execution time limits (15 mins AWS Lambda)
- ❌ Cold starts
- ❌ More expensive at scale

---

## 5. MESSAGE QUEUE COMMUNICATION

### 5.1 Using RabbitMQ

```csharp
public class RabbitMQService : IMessageQueue
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService(string connectionString)
    {
        var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare exchanges and queues
        _channel.ExchangeDeclare("aisocialpost", ExchangeType.Topic, durable: true);
        _channel.QueueDeclare("work.assign", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueDeclare("worker.heartbeat", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueDeclare("metrics", durable: true, exclusive: false, autoDelete: false);
    }

    public async Task PublishAsync(string routingKey, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: "aisocialpost",
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        await Task.CompletedTask;
    }

    public async Task SubscribeAsync(string queueName, Func<string, Task> handler)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await handler(message);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                // Requeue on error
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        await Task.CompletedTask;
    }
}
```

### 5.2 Work Distribution Pattern

```
Master                    RabbitMQ                Workers
  │                          │                       │
  ├─ Publish "work.assign" ─>│                       │
  │                          ├──> Worker 1 pulls     │
  │                          ├──> Worker 2 pulls     │
  │                          ├──> Worker 3 pulls     │
  │                          │                       │
  │                          │<─ "worker.heartbeat" ─┤
  │<─ Consume heartbeats ────┤                       │
  │                          │                       │
  │                          │<─ "metrics" ──────────┤
  │<─ Aggregate metrics ─────┤                       │
```

---

## 6. STORAGE STRATEGY

### 6.1 Shared Database

```sql
-- Worker registry
CREATE TABLE workers (
    worker_id VARCHAR(100) PRIMARY KEY,
    post_id VARCHAR(100) NOT NULL,
    platform VARCHAR(50) NOT NULL,
    started_at TIMESTAMP NOT NULL,
    last_heartbeat TIMESTAMP NOT NULL,
    status VARCHAR(20) NOT NULL,
    comments_processed INT DEFAULT 0,
    errors_count INT DEFAULT 0
);

-- Processed comments (avoid duplicates)
CREATE TABLE processed_comments (
    comment_id VARCHAR(100) PRIMARY KEY,
    post_id VARCHAR(100) NOT NULL,
    worker_id VARCHAR(100) NOT NULL,
    processed_at TIMESTAMP NOT NULL,
    action VARCHAR(50) NOT NULL,  -- 'replied', 'liked', 'escalated'
    INDEX idx_post_id (post_id),
    INDEX idx_worker_id (worker_id)
);

-- Worker metrics
CREATE TABLE worker_metrics (
    id BIGSERIAL PRIMARY KEY,
    worker_id VARCHAR(100) NOT NULL,
    metric_type VARCHAR(50) NOT NULL,  -- 'comment_replied', 'comment_liked', etc.
    metric_value FLOAT NOT NULL,
    timestamp TIMESTAMP NOT NULL,
    INDEX idx_worker_timestamp (worker_id, timestamp)
);
```

### 6.2 Redis for State Management

```csharp
public class RedisStateStore
{
    private readonly IDatabase _redis;

    public async Task<bool> IsCommentProcessedAsync(string commentId)
    {
        return await _redis.KeyExistsAsync($"comment:{commentId}");
    }

    public async Task MarkCommentProcessedAsync(string commentId, string workerId)
    {
        await _redis.StringSetAsync(
            $"comment:{commentId}",
            workerId,
            expiry: TimeSpan.FromDays(7)
        );
    }

    public async Task<DateTime?> GetLastCheckTimeAsync(string postId)
    {
        var value = await _redis.StringGetAsync($"post:{postId}:last_check");
        return value.HasValue ? DateTimeOffset.Parse(value).UtcDateTime : null;
    }

    public async Task SetLastCheckTimeAsync(string postId, DateTime time)
    {
        await _redis.StringSetAsync(
            $"post:{postId}:last_check",
            time.ToString("O"),
            expiry: TimeSpan.FromHours(48)
        );
    }
}
```

---

## 7. MONITORING & OBSERVABILITY

### 7.1 Metrics Dashboard

```csharp
public class MetricsCollector
{
    public async Task<DashboardMetrics> GetDashboardAsync()
    {
        return new DashboardMetrics
        {
            TotalWorkers = await GetWorkerCountAsync(),
            ActivePosts = await GetActivePostCountAsync(),
            CommentsProcessedToday = await GetCommentsProcessedTodayAsync(),
            RepliesPostedToday = await GetRepliesPostedTodayAsync(),
            LikesGivenToday = await GetLikesGivenTodayAsync(),
            AverageResponseTime = await GetAverageResponseTimeAsync(),
            ErrorRate = await GetErrorRateAsync(),
            PlatformBreakdown = await GetPlatformBreakdownAsync()
        };
    }
}
```

### 7.2 Health Checks

```csharp
public class WorkerHealthCheck : IHealthCheck
{
    private readonly MasterCoordinator _master;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context)
    {
        var stats = await _master.GetStatsAsync();

        if (stats.DeadWorkers > stats.TotalWorkers * 0.2)
        {
            return HealthCheckResult.Unhealthy(
                $"Too many dead workers: {stats.DeadWorkers}/{stats.TotalWorkers}"
            );
        }

        if (stats.ActiveWorkers == 0)
        {
            return HealthCheckResult.Degraded("No active workers");
        }

        return HealthCheckResult.Healthy(
            $"Workers: {stats.ActiveWorkers} active, {stats.IdleWorkers} idle"
        );
    }
}
```

### 7.3 Logging

```csharp
// Structured logging
_logger.LogInformation(
    "Comment processed: {CommentId} on {Platform} by {WorkerId}",
    comment.Id,
    platform,
    workerId
);

// Metrics
_metrics.RecordCounter("comments.processed", 1, new Dictionary<string, string>
{
    ["platform"] = platform,
    ["worker_id"] = workerId,
    ["action"] = "replied"
});
```

---

## 8. COST ANALYSIS

### 8.1 Resource Usage per Worker

**Single Worker:**
- CPU: 50-100 MHz
- RAM: 128-256 MB
- Network: 1-5 Mbps

**100 Workers:**
- CPU: 5-10 cores
- RAM: 12.8-25.6 GB
- Network: 100-500 Mbps

### 8.2 Deployment Costs

#### Option 1: Single Server
```
1 server (16 cores, 32GB RAM): $200/month
Can run ~100 workers
Cost per worker: $2/month
```

#### Option 2: Docker on Cloud (AWS/Azure)
```
10 × t3.medium (2 cores, 4GB): $40/month each = $400/month
Each can run ~10 workers = 100 workers total
Cost per worker: $4/month
```

#### Option 3: Kubernetes
```
Managed K8s cluster: $70/month
10 × worker nodes (2 cores, 4GB): $30/month each = $300/month
Total: $370/month for 100 workers
Cost per worker: $3.70/month
```

#### Option 4: Serverless
```
AWS Lambda:
- 100 workers × 24 hours × 30 days = 72,000 hours/month
- At 128MB: $0.0000000021/ms
- Average 30s execution = $0.00063/invocation
- With 2 invocations/min = ~$54,000/month ❌ TOO EXPENSIVE

Not recommended for long-running workers!
```

**Recommendation**: Docker on cloud or K8s ($300-400/month)

---

## 9. BEST PRACTICES

### 9.1 Worker Design

✅ **DO:**
- Keep workers simple and focused
- Use idempotent operations
- Handle failures gracefully
- Send regular heartbeats
- Clean up resources on exit
- Log all actions
- Report metrics

❌ **DON'T:**
- Share state between workers
- Store critical data in memory
- Ignore errors
- Run indefinitely without checks
- Hard-code configuration

### 9.2 Scalability

✅ **DO:**
- Scale horizontally (more workers)
- Use message queues for coordination
- Distribute across multiple machines
- Monitor resource usage
- Auto-scale based on load

❌ **DON'T:**
- Scale vertically (bigger server)
- Use shared files for communication
- Run all workers on 1 machine
- Ignore capacity limits

### 9.3 Fault Tolerance

✅ **DO:**
- Retry failed operations
- Detect and restart dead workers
- Use circuit breakers
- Implement graceful degradation
- Have fallback strategies

❌ **DON'T:**
- Fail silently
- Retry indefinitely
- Ignore health checks
- Let errors cascade

---

## 10. IMPLEMENTATION ROADMAP

### Phase 1: Prototype (Week 1-2)
- [ ] Implement basic worker
- [ ] Implement master coordinator
- [ ] Test with 10 workers locally
- [ ] Basic metrics collection

### Phase 2: Production (Week 3-4)
- [ ] Add message queue (RabbitMQ)
- [ ] Implement health checks
- [ ] Add monitoring dashboard
- [ ] Docker containerization

### Phase 3: Scale (Week 5-6)
- [ ] Deploy to cloud
- [ ] Test with 100+ workers
- [ ] Implement auto-scaling
- [ ] Load testing

### Phase 4: Optimize (Week 7-8)
- [ ] Performance tuning
- [ ] Cost optimization
- [ ] Advanced monitoring
- [ ] Documentation

---

## 11. EXAMPLE: COMPLETE SETUP

### Master Program

```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        // Configuration
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build());

        // Services
        services.AddDbContext<AppDbContext>();
        services.AddSingleton<IMessageQueue, RabbitMQService>();
        services.AddSingleton<MasterCoordinator>();

        // Build
        var provider = services.BuildServiceProvider();
        var master = provider.GetRequiredService<MasterCoordinator>();

        // Run
        Console.WriteLine("Starting Master Coordinator...");
        await master.RunAsync();
    }
}
```

### Worker Program

```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        // Get assignment from queue
        var queue = new RabbitMQService(Environment.GetEnvironmentVariable("RABBITMQ_URL"));

        await queue.SubscribeAsync("work.assign", async (message) =>
        {
            var assignment = JsonSerializer.Deserialize<WorkAssignment>(message);

            var worker = new PostWorker(
                assignment.PostId,
                assignment.Platform,
                assignment.Config
            );

            await worker.RunAsync();
        });

        // Keep alive
        await Task.Delay(Timeout.Infinite);
    }
}
```

### Docker Commands

```bash
# Build images
docker build -t aisocialpost/master:latest -f Dockerfile.master .
docker build -t aisocialpost/worker:latest -f Dockerfile.worker .

# Start infrastructure
docker-compose up -d db rabbitmq

# Start master
docker-compose up -d master

# Scale workers
docker-compose up -d --scale worker=100
```

---

## 12. CONCLUSION

### Summary

**Distributed Worker Pattern** cho engagement automation:

✅ **Pros:**
- Highly scalable (add more workers easily)
- Fault tolerant (1 worker fail ≠ system fail)
- Simple per-worker logic
- Resource efficient
- Easy to monitor and debug
- Can run on multiple machines

⚠️ **Cons:**
- More complex infrastructure
- Requires coordination mechanism
- Network overhead
- Initial setup time

### Recommendations

**For Small Scale (<50 posts):**
- Single app with threading
- Simple and cost-effective

**For Medium Scale (50-500 posts):**
- Docker containers
- RabbitMQ for coordination
- 1-2 servers

**For Large Scale (500+ posts):**
- Kubernetes orchestration
- Auto-scaling
- Multi-region deployment
- Advanced monitoring

### Next Steps

1. Start with simple prototype (10 workers)
2. Test thoroughly
3. Containerize
4. Deploy to cloud
5. Scale gradually
6. Monitor and optimize

**Development Time**: 6-8 weeks
**Cost**: $300-500/month (100 workers)
**Scalability**: Unlimited (add more workers!)

🚀 Ready to build distributed system!
