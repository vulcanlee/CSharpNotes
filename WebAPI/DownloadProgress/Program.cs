using DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadProgress
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Thread t1 = new Thread(() =>
            {
                if (Console.ReadKey(true).KeyChar.ToString().ToUpperInvariant() == "C")
                    cts.Cancel();
            });

            t1.Start();

            var progressIndicator = new Progress<double>(ReportProgress);

            var fooResult = await DownloadImageAsync("vulcan.png", progressIndicator, token);
            if (fooResult.Success == true)
            {
                Process myProcess = new Process();
                try
                {
                    // true is the default, but it is important not to set it to false
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = fooResult.Payload.ToString();
                    myProcess.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine($"使用者中斷下載作業 {fooResult.Message} {Environment.NewLine}");
                Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
                Console.ReadKey();
            }
        }

        private static void ReportProgress(double obj)
        {
            Console.WriteLine($"下載完成進度 {obj}");
        }

        private static async Task<APIResult> DownloadImageAsync(string filename,
            IProgress<double> progress, CancellationToken token)
        {
            string ImgFilePath = $"My_{filename}";
            ImgFilePath = Path.Combine(Environment.CurrentDirectory, ImgFilePath);
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/Datas/";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}{filename}";

                        response = await client.GetAsync(fooFullUrl, HttpCompletionOption.ResponseHeadersRead);
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode == true)
                            {
                                #region 狀態碼為 OK
                                var total = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
                                var canReportProgress = total != -1 && progress != null;
                                using (var filestream = File.Open(ImgFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    using (var stream = await response.Content.ReadAsStreamAsync())
                                    {
                                        var totalRead = 0L;
                                        var buffer = new byte[4096];
                                        var isMoreToRead = true;

                                        do
                                        {
                                            token.ThrowIfCancellationRequested();

                                            var read = await stream.ReadAsync(buffer, 0, buffer.Length);

                                            if (read == 0)
                                            {
                                                isMoreToRead = false;
                                            }
                                            else
                                            {
                                                await filestream.WriteAsync(buffer, 0, read);

                                                totalRead += read;

                                                if (canReportProgress)
                                                {
                                                    progress.Report((totalRead * 1d) / (total * 1d) * 100);
                                                }
                                            }
                                            // 故意暫停，讓使用者可以取消下載
                                            await Task.Delay(200);
                                        } while (isMoreToRead);
                                    }
                                }
                                fooAPIResult = new APIResult
                                {
                                    Success = true,
                                    Message = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.Content),
                                    Payload = ImgFilePath,
                                };
                                #endregion
                            }
                            else
                            {
                                fooAPIResult = new APIResult
                                {
                                    Success = false,
                                    Message = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.RequestMessage),
                                    Payload = null,
                                };
                            }
                        }
                        else
                        {
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = "應用程式呼叫 API 發生異常",
                                Payload = null,
                            };
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = ex.Message,
                            Payload = ex,
                        };
                    }
                }
            }

            return fooAPIResult;
        }
    }
}
