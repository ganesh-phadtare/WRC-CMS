using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                //__client.BaseAddress = new Uri("http://192.168.35.124/WRCWebAPI/");
            }
        }

        public void ExecuteNonQuery(string commandName, Dictionary<string, string> paramData)
        {
            var json = ConvertToJsonString(paramData);
            Task.Run(() => CallAPIForExecuteNonQuery(commandName, json));
        }

        public async Task<string> ExecuteDataset(string commandName, Dictionary<string, string> paramData)
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

        private async Task<string> CallAPIForExecuteDS(string commandName, string data)
        {
            HttpContent contentPost = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                var result = await __client.PostAsync(string.Format("view/ExecuteDS/{0}", commandName), contentPost);
                return Convert.ToString(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}