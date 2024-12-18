using Moq;
using StackExchange.Redis;
using Web.Src.Service;

namespace WebTest.Src.Cache
{
    [TestClass]
    public class RedisCacheServiceTest
    {
        private Mock<IConnectionMultiplexer>? _mockRedis;
        private Mock<IDatabase>? _mockDatabase;
        private RedisCacheService? _redisCacheService;

        [TestInitialize]
        public void SetUp()
        {
            _mockRedis = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();

            _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_mockDatabase.Object);

            _redisCacheService = new RedisCacheService(_mockRedis.Object);
        }

        [TestMethod]
        public async Task GetCachedValueAsync_CacheHit_ReturnsValue()
        {
            const string key = "myKey";
            const string expectedValue = "cachedValue";
            _mockDatabase!.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(expectedValue);

            var result = await _redisCacheService!.GetCachedValueAsync(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public async Task GetCachedValueAsync_CacheMiss_ReturnsNull()
        {
            const string key = "myKey";
            _mockDatabase!.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((string)null!);

            var result = await _redisCacheService!.GetCachedValueAsync(key);

            Assert.IsNull(result);
        }
    }
}
