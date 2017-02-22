using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WRC_CMS.Communication
{
    public class WebApiProxy
    {
        private static HttpClient __client = null;
        static WebApiProxy()
        {
            if (__client == null)
            {
                __client = new HttpClient();
                __client.Timeout = TimeSpan.FromMinutes(30);
                __client.BaseAddress = new Uri("http://192.168.35.124/WRCWebAPI/");
                //__client.BaseAddress = new Uri("http://localhost:51586/");

            }
        }

        public void ExecuteNonQuery(string commandName, Dictionary<string, string> paramData)
        {
            var json = ConvertToJsonString(paramData);
            Task.Run(() => CallAPIForExecuteNonQuery(commandName, json));
        }

        public async Task<DataSet> ExecuteDataset(string commandName, Dictionary<string, object> paramData)
        {
            var json = ConvertToJsonString(paramData);
            return await CallAPIForExecuteDS(commandName, json);
        }

        private string ConvertToJsonString(object data)
        {
            string jsonString = string.Empty;
            try
            {
                jsonString = JsonConvert.SerializeObject(data);
            }
            catch (JsonSerializationException exception)
            {
                Console.Write(exception.Message);
                throw;
            }
            return jsonString;

        }

        private void CallAPIForExecuteNonQuery(string commandName, string data)
        {
            HttpContent contentPost = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                __client.PostAsync(string.Format("view/Execute/{0}", commandName), contentPost);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<DataSet> CallAPIForExecuteDS(string commandName, string data)
        {
            HttpContent contentPost = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                var result = await __client.PostAsync(string.Format("view/ExecuteDS/{0}", commandName), contentPost);
                var compressedResoponse = result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(compressedResoponse))
                {
                    compressedResoponse = compressedResoponse.Substring(1).Substring(0, compressedResoponse.Length - 2);
                    string unCompressedData = GZip.GZipCompressDecompress.UnZip(compressedResoponse);

                    return JsonConvert.DeserializeObject<DataSet>(unCompressedData);

                }
                return new DataSet();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}