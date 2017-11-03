using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProgressHandler
{
    public class APIResult
    {
        /// <summary>
        /// 此次呼叫 API 是否成功
        /// </summary>
        public bool Success { get; set; } = true;
        /// <summary>
        /// 呼叫 API 失敗的錯誤訊息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 呼叫此API所得到的其他內容
        /// </summary>
        public object Payload { get; set; }
    }

    delegate void HttpProgressDelegate(object request, HttpProgressEventArgs e);

    class Program
    {
        //public static event EventHandler<HttpProgressEventArgs> HttpReceiveProgress;
        //public static event EventHandler<HttpProgressEventArgs> HttpSendProgress;

        static async Task Main(string[] args)
        {

            //HttpReceiveProgress += Program_HttpReceiveProgress;
            //HttpSendProgress += Program_HttpSendProgress; ;

            Console.WriteLine($"上傳圖片且有進度回報");
            await UploadImageAsync("vulcan.png", Program_HttpSendProgress, Program_HttpReceiveProgress);
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"下載圖片且有進度回報");
            await DownloadImageAsync("vulcan.png", Program_HttpSendProgress, Program_HttpReceiveProgress);
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();
        }

        private static void Program_HttpSendProgress(object sender, HttpProgressEventArgs e)
        {
            Console.WriteLine($"Send : {e.ProgressPercentage}");
        }

        private static void Program_HttpReceiveProgress(object sender, HttpProgressEventArgs e)
        {
            Console.WriteLine($"Receive : {e.ProgressPercentage}");
        }

        public static async Task<APIResult> UploadImageAsync(string filename,
            HttpProgressDelegate onHttpRequestProgress,
            HttpProgressDelegate onHttpResponseProgress)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            { 
                // System.Net.Http.Formatting.Extension
                ProgressMessageHandler progressMessageHandler = new ProgressMessageHandler();
                progressMessageHandler.InnerHandler = handler;
                progressMessageHandler.HttpReceiveProgress += new EventHandler<HttpProgressEventArgs>(onHttpResponseProgress);
                progressMessageHandler.HttpSendProgress += new EventHandler<HttpProgressEventArgs>(onHttpRequestProgress);

                //progressMessageHandler.HttpReceiveProgress += (s, e) =>
                //{
                //    Console.WriteLine($"Receive : {e.ProgressPercentage}");
                //};
                //progressMessageHandler.HttpSendProgress += (s, e) =>
                //{
                //    Console.WriteLine($"Send : {e.ProgressPercentage}");
                //};

                //progressMessageHandler.HttpReceiveProgress += HttpReceiveProgress;
                //progressMessageHandler.HttpSendProgress += HttpSendProgress;
                using (HttpClient client = new HttpClient(progressMessageHandler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/Upload";
                        HttpResponseMessage response = null;


                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        #region 將檔案上傳到網路伺服器上(使用 Multipart 的規範)
                        // 規格說明請參考 https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html
                        using (var content = new MultipartFormDataContent())
                        {
                            var rootPath = Directory.GetCurrentDirectory();
                            // 取得這個圖片檔案的完整路徑
                            var path = Path.Combine(rootPath, filename);

                            // 開啟這個圖片檔案，並且讀取其內容
                            using (var fs = File.Open(path, FileMode.Open))
                            {
                                var fooSt = $"My{filename}";
                                var streamContent = new StreamContent(fs);
                                streamContent.Headers.Add("Content-Type", "application/octet-stream");
                                streamContent.Headers.Add("Content-Disposition", "form-data; name=\"files\"; filename=\"" + fooSt + "\"");
                                content.Add(streamContent, "file", filename);

                                // 上傳到遠端伺服器上
                                response = await client.PostAsync(fooFullUrl, content);
                            }
                        }
                        #endregion
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode == true)
                            {
                                // 取得呼叫完成 API 後的回報內容
                                String strResult = await response.Content.ReadAsStringAsync();
                                fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
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


        private static async Task<APIResult> DownloadImageAsync(string filename,
            HttpProgressDelegate onHttpRequestProgress,
            HttpProgressDelegate onHttpResponseProgress)
        {
            string ImgFilePath = $"My_{filename}";
            ImgFilePath = Path.Combine(Environment.CurrentDirectory, ImgFilePath);
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                ProgressMessageHandler progressMessageHandler = new ProgressMessageHandler(handler);
                progressMessageHandler.InnerHandler = handler;
                progressMessageHandler.HttpReceiveProgress += new EventHandler<HttpProgressEventArgs>(onHttpResponseProgress);
                progressMessageHandler.HttpSendProgress += new EventHandler<HttpProgressEventArgs>(onHttpRequestProgress);

                //progressMessageHandler.HttpReceiveProgress += (s, e) =>
                //{
                //    Console.WriteLine($"Receive : {e.ProgressPercentage}");
                //};
                //progressMessageHandler.HttpSendProgress += (s, e) =>
                //{
                //    Console.WriteLine($"Send : {e.ProgressPercentage}");
                //};
                using (HttpClient client = new HttpClient(progressMessageHandler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/Datas/";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}{filename}";

                        response = await client.GetAsync(fooFullUrl);
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode == true)
                            {
                                //byte[] foo = await response.Content.ReadAsByteArrayAsync();
                                using (var filestream = File.Open(ImgFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    using (var stream = await response.Content.ReadAsStreamAsync())
                                    {
                                        stream.CopyTo(filestream);
                                    }
                                }
                                fooAPIResult = new APIResult
                                {
                                    Success = true,
                                    Message = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.Content),
                                    Payload = ImgFilePath,
                                };
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
