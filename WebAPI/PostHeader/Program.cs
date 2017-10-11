using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PostHeader
{
    class Program
    {
        static void Main(string[] args)
        {
            var fooLoginInformation = new LoginInformation()
            {
                Account = "Vulcan",
                Password = "123",
                VerifyCode = "123"
            };
            var foo = JsonPostAsync(fooLoginInformation, true).Result;
            Console.WriteLine($"使用 Post 方法，搭配 JSON 與 Header，呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            fooLoginInformation = new LoginInformation()
            {
                Account = "Vuln",
                Password = "13",
                VerifyCode = "123"
            };
            foo = JsonPostAsync(fooLoginInformation, true).Result;
            Console.WriteLine($"使用 Post 方法，搭配 JSON 與 Header，呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            fooLoginInformation = new LoginInformation()
            {
                Account = "Vulcan",
                Password = "123",
                VerifyCode = "888"
            };
            foo = JsonPostAsync(fooLoginInformation, true).Result;
            Console.WriteLine($"使用 Post 方法，搭配 JSON 與 Header，呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            fooLoginInformation = new LoginInformation()
            {
                Account = "Vulcan",
                Password = "123",
                VerifyCode = "888"
            };
            foo = JsonPostAsync(fooLoginInformation, false).Result;
            Console.WriteLine($"使用 Post 方法，搭配 JSON 與 Header，呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

        }

        private static async Task<APIResult> JsonPostAsync(LoginInformation loginInformation,
            bool sendHeader)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        //string FooUrl = $"http://localhost:53495/api/values/HeaderPost";
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/Values/HeaderPost";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";
                        if (sendHeader == true)
                        {
                            client.DefaultRequestHeaders.Add("VerifyCode", loginInformation.VerifyCode);
                        }

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        var fooJSON = JsonConvert.SerializeObject(loginInformation);
                        using (var fooContent = new StringContent(fooJSON, Encoding.UTF8, "application/json"))
                        {
                            response = await client.PostAsync(fooFullUrl, fooContent);
                        }
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
