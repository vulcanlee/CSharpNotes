﻿using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MultiPartPost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fooAPIData = new APIData()
            {
                Id = 777,
                Name = "VulcanSource",
                Filename = "",
            };
            var foo = await FormDataPostAsync(fooAPIData);
            Console.WriteLine($"使用 multipart/form-data MIME 類型編碼 格式與使用 Post 方法呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            fooAPIData = new APIData()
            {
                Id = 123,
                Name = "VulcanSource",
                Filename = "",
            };
            foo = await FormDataPostAsync(fooAPIData);
            Console.WriteLine($"使用 multipart/form-data MIME 類型編碼 格式與使用 Post 方法呼叫 Web API 的結果");
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            Console.WriteLine($"Payload : {foo.Payload}");
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();
        }


        private static async Task<APIResult> FormDataPostAsync(APIData apiData)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/Values/FormUrlencodedPost";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        //https://docs.microsoft.com/zh-tw/dotnet/csharp/language-reference/keywords/nameof
                        #region 使用 MultipartFormDataContent 產生要 Post 的資料
                        // 準備要 Post 的資料
                        Dictionary<string, string> formDataDictionary = new Dictionary<string, string>()
                        {
                            {nameof(APIData.Id), apiData.Id.ToString() },
                            {nameof(APIData.Name), apiData.Name },
                            {nameof(APIData.Filename), apiData.Filename }
                        };

                        // https://msdn.microsoft.com/zh-tw/library/system.net.http.multipartformdatacontent(v=vs.110).aspx
                        using (var content = new MultipartFormDataContent())
                        {
                            foreach (var keyValuePair in formDataDictionary)
                            {
                                content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                            }
                            response = await client.PostAsync(fooFullUrl, content);
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
