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

namespace LongTimeAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("遠端 Web API 需要花費 5 秒鐘，才會回傳結果內容");
            Console.WriteLine($"使用 Get 方法呼叫 Web API ，並且會逾期的結果 ( HttpClient 限時 4 秒內要完成 )");
            var foo = JsonPutAsync(4).Result;
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            if (foo.Success == true)
            {
                var item = JsonConvert.DeserializeObject<APIData>(foo.Payload.ToString());
                Console.WriteLine($"Id : {item.Id}");
                Console.WriteLine($"Name : {item.Name}");
                Console.WriteLine($"Filename : {item.Filename}");
            }
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Continue...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"使用 Get 方法呼叫 Web API ，並且不會逾期的結果 ( HttpClient 限時 6 秒內要完成 )");
            foo = JsonPutAsync(6).Result;
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            if (foo.Success == true)
            {
                var item = JsonConvert.DeserializeObject<APIData>(foo.Payload.ToString());
                Console.WriteLine($"Id : {item.Id}");
                Console.WriteLine($"Name : {item.Name}");
                Console.WriteLine($"Filename : {item.Filename}");
            }
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

        }

        private static async Task<APIResult> JsonPutAsync(int sec = 4)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        client.Timeout = TimeSpan.FromSeconds(sec);
                        #region 呼叫遠端 Web API
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/values/LongTimeGet";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        response = await client.GetAsync(fooFullUrl);
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
