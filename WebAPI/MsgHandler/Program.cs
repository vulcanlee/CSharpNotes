using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MsgHandler
{
    class Global
    {
        public static string APIKey { get; set; } = "123";
    }
    class MyMessageHandler1 : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("MyMessageHandler - 準備要執行");
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine($"MyMessageHandler - 執行完畢，狀態碼為 {response.StatusCode}");
            return response;
        }
    }

    class MyMessageHandler2 : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add custom functionality here, before or after base.SendAsync()
            Console.WriteLine("MyMessageHandler2 - 準備要執行");
            request.Headers.TryAddWithoutValidation("APIKey", Global.APIKey);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine($"MyMessageHandler2 - 執行完畢，狀態碼為 {response.StatusCode}");
            return response;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var foo = await HttpGetAsync();
            Console.WriteLine($"使用 Get 方法呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

        }

        private static async Task<APIResult> HttpGetAsync()
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                var handler1 = new MyMessageHandler1();
                var handler2 = new MyMessageHandler2();
                handler1.InnerHandler = handler2;
                handler2.InnerHandler = handler;
                using (HttpClient client = new HttpClient(handler1))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/values/CustHandler";
                        //string FooUrl = $"http://localhost:53495/api/values/CustHandler";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Type
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        response = await client.GetAsync(fooFullUrl);
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode == true)
                            {
                                var fooCT = response.Headers.FirstOrDefault(x => x.Key == "APIKeyEcho");
                                var fooCT2 = fooCT.Value.FirstOrDefault();
                                Console.WriteLine($"APIKeyEcho={fooCT2}");

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
