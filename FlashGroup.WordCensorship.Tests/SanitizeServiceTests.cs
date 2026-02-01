using FlashGroup.WordCensorship.Data;
using FlashGroup.WordCensorship.Domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlashGroup.WordCensorship.Tests
{
    [TestClass]
    public class SanitizeServiceTests
    {
        private Mock<ISensitiveWordRepo> _repoMock;
        private SanitizeService _service;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<ISensitiveWordRepo>();
            var logger = LoggerFactory.Create(b => b.AddDebug())
                                      .CreateLogger<SanitizeService>();

            _service = new SanitizeService(logger, _repoMock.Object);
        }

        [TestMethod]
        public async Task SanitizePhrase_NullInput_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.SanitizePhrase(null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public async Task SanitizePhrase_EmptyPhrase_ThrowsArgumentNullException(string phrase)
        {
            var request = new SanitizePhase { Phrase = phrase };

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.SanitizePhrase(request));
        }

        [TestMethod]
        public async Task SanitizePhrase_ContainsSensitiveWord_CensorsCorrectly()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(new List<SensitiveWord>
                     {
                 new SensitiveWord { Word = "bad" }
                     });

            var request = new SanitizePhase { Phrase = "This is bad" };

            var result = await _service.SanitizePhrase(request);

            Assert.AreEqual("This is ***", result.Phrase);
        }
    }
}
