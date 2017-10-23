using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UploadFileAndData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fooLoginInformation = new LoginInformation()
            {
                Account = "Vulcan",
                Password = "123",
                VerifyCode = "abc"
            };
            var foo = await UploadImageAsync("Readme.txt", fooLoginInformation);
            fooLoginInformation = JsonConvert.DeserializeObject<LoginInformation>(foo.Payload.ToString());
            Console.WriteLine($"使用 MultiPart/Form-Data 格式傳送文字檔案與資料、使用 Post 方法呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"Account : {fooLoginInformation.Account}");
            Console.WriteLine($"Password : {fooLoginInformation.Password}");
            Console.WriteLine($"文字檔案內容 : {fooLoginInformation.VerifyCode}");
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();
        }

        public static async Task<APIResult> UploadImageAsync(string filename, LoginInformation loginInformation)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/Upload/FileAndData";
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

                            Dictionary<string, string> formDataDictionary = new Dictionary<string, string>()
                            {
                                { nameof(loginInformation.Account), loginInformation.Account },
                                { nameof(loginInformation.Password), loginInformation.Password },
                                { nameof(loginInformation.VerifyCode), loginInformation.VerifyCode }
                            };

                            foreach (var keyValuePair in formDataDictionary)
                            {
                                content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                            }

                            var rootPath = Directory.GetCurrentDirectory();
                            // 取得這個圖片檔案的完整路徑
                            var path = Path.Combine(rootPath, filename);

                            // 開啟這個圖片檔案，並且讀取其內容
                            using (var fs = File.Open(path, FileMode.Open))
                            {
                                var fooSt = $"My{filename}";
                                var streamContent = new StreamContent(fs);
                                streamContent.Headers.Add("Content-Type", "text/plain");
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
