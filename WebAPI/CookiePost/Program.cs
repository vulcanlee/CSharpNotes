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

namespace CookiePost
{
    class Program
    {
        //static public Uri BaseAddress { get; set; } = new Uri("http://localhost");
        static public Uri BaseAddress { get; set; } = new Uri("http://vulcanwebapi.azurewebsites.net");
        static public string CookieName { get; set; } = ".AspNetCore.Cookies";
        //static public string RemoteLoginUrl = $"http://localhost:53495/api/Values/Login";
        //static public string RemoteLoginCheckUrl = $"http://localhost:53495/api/Values/LoginCheck";
        static public string RemoteLoginUrl = $"http://vulcanwebapi.azurewebsites.net/api/Values/Login";
        static public string RemoteLoginCheckUrl = $"http://vulcanwebapi.azurewebsites.net/api/Values/LoginCheck";

        static async Task Main(string[] args)
        {
            string ResponseCookie = "";
            var fooLoginInformation = new LoginInformation()
            {
                Account = "Vulcan",
                Password = "123",
            };
            var foo = await LoginPostAsync(fooLoginInformation);
            ResponseCookie = foo.Payload.ToString();
            Console.WriteLine($"使用 Post 方法，取得 Cookie，呼叫 Web API 的結果");
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
            foo = await LoginPostAsync(fooLoginInformation);
            Console.WriteLine($"使用 Post 方法，取得 Cookie，呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            foo = await LoginCheckGetAsync(ResponseCookie);
            Console.WriteLine($"使用 Get 方法，傳送 Cookie，呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            foo = await LoginCheckGetAsync(null);
            Console.WriteLine($"使用 Get 方法，沒有傳送 Cookie，呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();
        }

        private static async Task<APIResult> LoginPostAsync(LoginInformation loginInformation)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = RemoteLoginUrl;
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";

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
                                #region 取得 Cookie
                                var ResponseCookie = "";
                                IEnumerable<Cookie> responseCookies = handler.CookieContainer.GetCookies(BaseAddress).Cast<Cookie>();
                                var fooresponseCookie = responseCookies.FirstOrDefault(x => x.Name == CookieName);
                                if (fooresponseCookie != null)
                                {
                                    ResponseCookie = fooresponseCookie.Value.ToString();
                                }
                                else
                                {
                                    ResponseCookie = "";
                                }
                                #endregion

                                // 取得呼叫完成 API 後的回報內容
                                String strResult = await response.Content.ReadAsStringAsync();

                                fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                                fooAPIResult.Payload = ResponseCookie;
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

        private static async Task<APIResult> LoginCheckGetAsync(string responseCookie)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        ////請試著將底下敘述解除註解，並且執行看看，會發生甚麼事情？
                        //handler.AllowAutoRedirect = false;

                        #region 呼叫遠端 Web API
                        string FooUrl = RemoteLoginCheckUrl;
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        if (string.IsNullOrEmpty(responseCookie) == false)
                        {
                            var baseAddress = BaseAddress;
                            //var cookieContainer = handler.CookieContainer;
                            //cookieContainer.Add(baseAddress, new Cookie(CookieName, responseCookie));
                            handler.CookieContainer.Add(baseAddress, new Cookie(CookieName, responseCookie));
                        }

                        response = await client.GetAsync(fooFullUrl);
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            // Throws an exception if the System.Net.Http.HttpResponseMessage.IsSuccessStatusCode property 
                            // for the HTTP response is false
                            // 請試著將底下程式碼解除註解，看看會有何結果
                            //response.EnsureSuccessStatusCode();

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
