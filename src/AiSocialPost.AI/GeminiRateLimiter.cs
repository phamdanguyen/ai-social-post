namespace AiSocialPost.AI
{
    /// <summary>
    /// Rate limiter cho Gemini API - Tôn trọng free tier limits
    /// Free tier: 15 requests/minute, 1M tokens/minute, 1500 requests/day
    /// Safety margin: Max 10 requests/minute
    /// </summary>
    public class GeminiRateLimiter
    {
        private const int MaxRequestsPerMinute = 10;  // Safety: 10 instead of 15
        private readonly Queue<DateTime> _requestTimestamps = new();
        private readonly object _lock = new();

        public async Task WaitIfNeededAsync()
        {
            lock (_lock)
            {
                // Remove timestamps older than 1 minute
                var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
                while (_requestTimestamps.Count > 0 &&
                       _requestTimestamps.Peek() < oneMinuteAgo)
                {
                    _requestTimestamps.Dequeue();
                }

                // Check if at limit
                if (_requestTimestamps.Count >= MaxRequestsPerMinute)
                {
                    var oldestRequest = _requestTimestamps.Peek();
                    var waitTime = oldestRequest.AddMinutes(1) - DateTime.UtcNow;

                    if (waitTime > TimeSpan.Zero)
                    {
                        // Wait outside the lock
                        Task.Delay(waitTime).Wait();
                    }

                    // Clean up again after waiting
                    oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
                    while (_requestTimestamps.Count > 0 &&
                           _requestTimestamps.Peek() < oneMinuteAgo)
                    {
                        _requestTimestamps.Dequeue();
                    }
                }

                // Record this request
                _requestTimestamps.Enqueue(DateTime.UtcNow);
            }

            await Task.CompletedTask;
        }

        public int GetCurrentRequestCount()
        {
            lock (_lock)
            {
                var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
                return _requestTimestamps.Count(t => t >= oneMinuteAgo);
            }
        }
    }
}
