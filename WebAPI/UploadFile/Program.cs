using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UploadFile
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await UploadImageAsync("vulcan.png");
            Process myProcess = new Process();
            try
            {
                // true is the default, but it is important not to set it to false
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = "https://vulcanwebapi.azurewebsites.net/Datas/Myvulcan.png";
                myProcess.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();
        }

        public static async Task<APIResult> UploadImageAsync(string filename)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        //string FooUrl = $"http://localhost:53494/api/Upload";
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/Upload";
                        HttpResponseMessage response = null;


                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        #region 將剛剛拍照的檔案，上傳到網路伺服器上(使用 Multipart 的規範)
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
    }
}
