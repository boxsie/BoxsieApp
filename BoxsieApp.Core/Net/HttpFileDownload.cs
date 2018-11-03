using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BoxsieApp.Core.Net
{
    public class HttpFileDownload : IDisposable
    {
        private const int BufferSize = 1024 * 4;

        private HttpClient _client;

        public HttpFileDownload()
        {
            _client = new HttpClient {Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite)};
        }

        public async Task<long> GetFileSizeAsync(string fileUrl)
        {
            try
            {
                using (var response = await _client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    return response.Content.Headers.ContentLength ?? 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;
        }

        public async Task DownloadAsync(string fileUrl, string filePath, Action<FileDownloadProgress> progressUpdate)
        {
            try
            {
                using (var response = await _client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var fileProgress = new FileDownloadProgress {TotalBytes = response.Content.Headers.ContentLength ?? 0};
                        
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[BufferSize];
                        var stopwatch = new Stopwatch();

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true))
                        {
                            stopwatch.Start();

                            var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                            while (bytesRead > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);

                                fileProgress.CurrentBytes += bytesRead;
                                fileProgress.TotalSeconds = stopwatch.Elapsed.TotalSeconds;

                                bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                                progressUpdate(fileProgress);
                            }

                            stopwatch.Stop();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}