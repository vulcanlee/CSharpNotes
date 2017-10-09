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

namespace FormDataPut
{
    class Program
    {
        static void Main(string[] args)
        {
            var fooAPIData = new APIData()
            {
                Id = 777,
                Name = "VulcanSource",
                Filename = "",
            };
            var foo = FormDataPostAsync(fooAPIData).Result;
            Console.WriteLine($"使用 form-urlencoded 格式與使用 Post 方法呼叫 Web API 的結果");
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
            foo = FormDataPostAsync(fooAPIData).Result;
            Console.WriteLine($"使用 form-urlencoded 格式與使用 Post 方法呼叫 Web API 的結果");
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
                        //string FooUrl = $"http://localhost:53494/api/Upload";
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/Values/FormUrlencodedPost";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}";
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //https://docs.microsoft.com/zh-tw/dotnet/csharp/language-reference/keywords/nameof

                        #region 使用 FormUrlEncodedContent 產生要 Post 的資料
                        //// 方法一： 使用字串名稱用法
                        //var formData = new FormUrlEncodedContent(new[] {
                        //    new KeyValuePair<string, string>("Id", apiData.Id.ToString()),
                        //    new KeyValuePair<string, string>("Name", apiData.Name),
                        //    new KeyValuePair<string, string>("Filename", apiData.Filename)
                        //});

                        // 方法二： 強型別用法
                        Dictionary<string, string> formDataDictionary = new Dictionary<string, string>()
                        {
                            {nameof(APIData.Id), apiData.Id.ToString() },
                            {nameof(APIData.Name), apiData.Name },
                            {nameof(APIData.Filename), apiData.Filename }
                        };
                        var formData = new FormUrlEncodedContent(formDataDictionary);
                        #endregion

                        response = await client.PostAsync(fooFullUrl, formData);
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();

                            switch (response.StatusCode)
                            {
                                case HttpStatusCode.OK:
                                    #region 狀態碼為 OK
                                    fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                                    #endregion
                                    break;

                                default:
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.Content),
                                        Payload = null,
                                    };
                                    break;
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
