using FlashGroup.WordCensorship.API.DTOs;
using FlashGroup.WordCensorship.Data;
using FlashGroup.WordCensorship.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlashGroup.WordCensorship.Tests
{ 

    [TestClass]
    public class SensitiveWordServiceTests
    {
        private Mock<ISensitiveWordRepo> _repoMock;
        private SensitiveWordService _service;

        private static List<SensitiveWord> SampleWords => new()
        {
            new SensitiveWord { ID = 1, Word = "bad" },
            new SensitiveWord { ID = 2, Word = "ugly" }
        };

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<ISensitiveWordRepo>();
            var logger = NullLogger<SanitizeService>.Instance;

            _service = new SensitiveWordService(logger, _repoMock.Object);
        }

        #region GetAll
        [TestMethod]
        public async Task GetAll_ReturnsAllSensitiveWords()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            var result = await _service.GetAll();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("bad", result.First().Word);
            _repoMock.Verify(r => r.GetAll(), Times.Once);
        }

        #endregion

        #region Add Sensitive Word
        [TestMethod]
        public async Task AddSensitiveWord_NewWord_AddsSuccessfully()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            _repoMock.Setup(r => r.Create("mean"))
                     .ReturnsAsync(true);

            var result = await _service.AddSensitiveWord("mean");

            Assert.IsTrue(result);
            _repoMock.Verify(r => r.Create("mean"), Times.Once);
        }

        [TestMethod]
        public async Task AddSensitiveWord_ExistingWord_ThrowsInvalidDataException()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);
                     
            await Assert.ThrowsAsync<InvalidDataException>(() =>
                _service.AddSensitiveWord("BAD"));
                        
            _repoMock.Verify(r => r.Create(It.IsAny<string>()), Times.Never);
            _repoMock.Verify(r => r.GetAll(), Times.Once);
        }

        [TestMethod]
        public async Task AddSensitiveWord_DuplicateWord_ThrowsInvalidDataException()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            await Assert.ThrowsAsync<InvalidDataException>(() =>
                _service.AddSensitiveWord("BAD"));
        }
        #endregion

        #region Update Sensitive Word
        [TestMethod]
        public async Task UpdateSensitiveWord_FromWordNotFound_Throws()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            var request = new SensitiveWordUpdateRequest
            {
                FromWord = "mean",
                ToWord = "nice"
            };

            await Assert.ThrowsAsync<InvalidDataException>(() =>
                _service.UpdateSensitiveWord(request));
        }

        [TestMethod]
        public async Task UpdateSensitiveWord_ToWordDuplicate_ThrowsInvalidDataException()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            var request = new SensitiveWordUpdateRequest
            {
                FromWord = "bad",
                ToWord = "ugly"
            };

            await Assert.ThrowsAsync<InvalidDataException>(() =>
                _service.UpdateSensitiveWord(request));

            _repoMock.Verify(r => r.Update(It.IsAny<SensitiveWord>()), Times.Never);
            _repoMock.Verify(r => r.GetAll(), Times.Once);
        }


        [TestMethod]
        public async Task UpdateSensitiveWord_ValidRequest_UpdatesSuccessfully()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            _repoMock.Setup(r => r.Update(It.IsAny<SensitiveWord>()))
                     .ReturnsAsync(true);

            var request = new SensitiveWordUpdateRequest
            {
                FromWord = "bad",
                ToWord = "mean"
            };

            var result = await _service.UpdateSensitiveWord(request);

            Assert.IsTrue(result);
            _repoMock.Verify(r => r.Update(It.IsAny<SensitiveWord>()), Times.Once);
        }
        #endregion

        #region Remove Sensitive Word
        [TestMethod]
        public async Task RemoveSensitiveWord_NotFound_Throws()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            var request = new SensitiveWordRequest
            {
                Word = "mean"
            };

            await Assert.ThrowsAsync<InvalidDataException>(() =>
                _service.RemoveSensitiveWord(request));
        }

        [TestMethod]
        public async Task RemoveSensitiveWord_NotFound_ThrowsInvalidDataException()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            var request = new SensitiveWordRequest
            {
                Word = "mean"
            };

            await Assert.ThrowsAsync<InvalidDataException>(() =>
                _service.RemoveSensitiveWord(request));

            _repoMock.Verify(r => r.Delete(It.IsAny<SensitiveWord>()), Times.Never);
        }

        [TestMethod]
        public async Task RemoveSensitiveWord_ExistingWord_DeletesSuccessfully()
        {
            _repoMock.Setup(r => r.GetAll())
                     .ReturnsAsync(SampleWords);

            _repoMock.Setup(r => r.Delete(It.IsAny<SensitiveWord>()))
                     .ReturnsAsync(true);

            var request = new SensitiveWordRequest
            {
                Word = "bad"
            };

            var result = await _service.RemoveSensitiveWord(request);

            Assert.IsTrue(result);

            _repoMock.Verify(r => r.Delete(It.Is<SensitiveWord>(sw =>
                sw.Word == "bad"
            )), Times.Once);
        }
        #endregion
    }
}