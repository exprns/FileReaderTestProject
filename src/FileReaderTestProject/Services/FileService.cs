using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FileReader.Helpers;

namespace FileReader.Services
{
    public interface IFileService
    {
        Task<byte[]?> GetFile(string fileName);
    }

    public class TaskWithCnt
    {
        public Task<byte[]?> ReadingOperation { get; set; }
        public int ReadersCount { get; set; }

    }

    public class FileService : IFileService
    {
        private ConcurrentDictionary<string, TaskWithCnt> _currentReadingFiles = new ConcurrentDictionary<string, TaskWithCnt>();
        /// <summary>
        /// Get file data by filename. Don't read same file twice at the moment (send first reading result).
        /// </summary>
        /// <param name="filename"> file name </param>
        /// <returns> File data in bytes </returns>
        /// <remarks> If filename is not correct will return null. </remarks> 
        public async Task<byte[]?> GetFile(string fileName)
        {
            if (fileName == null)
                return null;

            _currentReadingFiles.TryAdd(fileName, new TaskWithCnt()
            {
                ReadingOperation = ReadFile(fileName),
                ReadersCount = 0
            });

            _currentReadingFiles.TryGetValue(fileName, out var data2);

            data2.ReadersCount++;

            var data = await data2.ReadingOperation;

            data2.ReadersCount--;

            if (data2.ReadersCount == 0)
                _currentReadingFiles.Remove(fileName, out var val);


            return data;
        }

        // public class FileService : IFileService
        //     {
        //         private ConcurrentDictionary<string, Task<byte[]?>> _currentReadingFiles = new ConcurrentDictionary<string, Task<byte[]?>>();
        //         private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        //         Task<byte[]?> zeroTask = null;
        //         /// <summary>
        //         /// Get file data by filename. Don't read same file twice at the moment (send first reading result).
        //         /// </summary>
        //         /// <param name="filename"> file name </param>
        //         /// <returns> File data in bytes </returns>
        //         /// <remarks> If filename is not correct will return null. </remarks> 
        //         public async Task<byte[]?> GetFile(string fileName)
        //         {
        //             try
        //             {
        //                 if (fileName == null)
        //                     return null;

        //                 await semaphoreSlim.WaitAsync();
        //                 if (_currentReadingFiles.TryAdd(fileName, zeroTask))
        //                 {
        //                     _currentReadingFiles[fileName] = ReadFile(fileName);
        //                     Console.WriteLine("Create");
        //                 }
        //                 var task = _currentReadingFiles[fileName];
        //                 semaphoreSlim.Release();
        //                 var data = await task;

        //                 await semaphoreSlim.WaitAsync();
        //                 if (_currentReadingFiles.TryRemove(fileName, out var val))
        //                     Console.WriteLine("delete");

        //                 semaphoreSlim.Release();

        //                 zeroTask = null;

        //                 return data;
        //             }

        //             catch (Exception ex)
        //             {
        //                 { }
        //             }
        //             return null;
        //         }

        private int _readingDelayMs = 2000;

        /// <summary>
        /// Read file data by filename.
        /// </summary>
        /// <param name="filename"> file name </param>
        /// <returns> File data in bytes </returns>
        /// <remarks> If filename is not correct will return null. </remarks> 
        private async Task<byte[]?> ReadFile(string fileName)
        {
            await Task.Delay(_readingDelayMs);
            return FileHelperMock.GetFileDataMock(fileName);
        }
    }
}