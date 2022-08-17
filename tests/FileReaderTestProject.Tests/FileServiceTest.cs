using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FileReader.Helpers;
using System.Diagnostics;

namespace FileReader.Services
{
    [TestFixture]

    public class FileServiceTest
    {
        private FileService _fileService;
        private string _sampleFilename;
        [SetUp]
        public void SetUp()
        {
            _fileService = new FileService();
            _sampleFilename = "test.txt";
        }

        [Test]
        public async Task GetFile_InputIsSampleFilename_ReturnBytes()
        {
            var result = await _fileService.GetFile(_sampleFilename);

            Assert.IsTrue(result != null && result.Length > 0, $"GetFile returned empty result. Filename:{_sampleFilename}");
        }

        [Test]
        public async Task GetFile_InputIsNull_ReturnNull()
        {
            var result = await _fileService.GetFile(null);

            Assert.IsTrue(result == null, $"GetFile returned not null");
        }

        [Test]
        public void GetFile_InputIsSampleFilename_MultiplyReadingReturnSameBytes()
        {
            var cntReadings = 10;
            var readingTasks = new Task<byte[]?>[cntReadings];

            for (int i = 0; i < cntReadings; i++)
            {
                readingTasks[i] = _fileService.GetFile(_sampleFilename);
            }

            Task.WaitAll(readingTasks);

            var result = false;

            if (readingTasks.Select(x => x.Result).Distinct().Count() == 1)
                result = true;

            Assert.IsTrue(result, $"Request with same file name retuns different values");
        }

        [Test]
        public async Task GetFile_InputIsSampleFilename_MultiplyReadingSameFileSpendSameTimeLikeReadingSoloFile()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            await _fileService.GetFile(_sampleFilename);
            var soloReadersTime = Math.Round(s.Elapsed.TotalSeconds);

            var cntReaders = 10;
            var readersTasks = new Task<byte[]?>[cntReaders];

            s.Restart();
            for (int i = 0; i < cntReaders; i++)
            {
                readersTasks[i] = _fileService.GetFile(_sampleFilename);
            }

            Task.WaitAll(readersTasks);

            var multiplyReadersTime = Math.Round(s.Elapsed.TotalSeconds);

            Assert.IsTrue(multiplyReadersTime == soloReadersTime, $"GetFile not return {_sampleFilename}");
        }

        [Test]
        public async Task GetFile_InputIsSampleFilename_MultiplyReadingSameFileInDifferentThreadsSpendSameTimeLikeReadingSoloFile_StressTest()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            await _fileService.GetFile(_sampleFilename);
            var soloReadersTime = Math.Round(s.Elapsed.TotalSeconds);

            var cntReaders = 10000;
            var threadTasks = new Task<Task<byte[]?>>[cntReaders];
            var readersTasks = new Task<byte[]?>[cntReaders];

            s.Restart();
            for (int i = 0; i < cntReaders; i++)
            {
                threadTasks[i] = Task.Factory.StartNew(async () =>
                {
                    return await _fileService.GetFile(_sampleFilename);
                });
            }

            Task.WaitAll(threadTasks);

            for (int i = 0; i < cntReaders; i++)
                readersTasks[i] = threadTasks[i].Result;

            Task.WaitAll(readersTasks);
            s.Stop();

            var multiplyReadersTime = Math.Round(s.Elapsed.TotalSeconds);

            var differentFileDataCnt = readersTasks.Select(x => x.Result).Distinct().Count();

            Assert.IsTrue((multiplyReadersTime == soloReadersTime) && differentFileDataCnt == 1,
                $"{cntReaders} readers crash functions. Multiply readers spend on reading: ${multiplyReadersTime} sec," +
                $"Solo readers spend on reading: ${soloReadersTime} sec, different file data count: ${differentFileDataCnt}");
        }
    }
}