using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace faPiao.Models
{
    public class Public
    {
        public static string temppath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["TuPian"].ToString().Trim());

        private static String clientId = "FCGMzAL0RyZ1FkjCjTq8QoqF";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "jGYP1TSFuESCWobq9hMdIzdaWFVLYjtN";

        #region post方法获取url地址和参数输出内容
        /// <summary>     
        ///获取url地址和参数输出内容     
        /// </summary> /// <param name="url">url</param>      
        /// <param name="str">请求参数</param>  
        public static String SendRequest(string url, string str)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "post";
            request.KeepAlive = true;
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();

            return result;
        }
       
        #endregion

        #region 获取access_token
        public static String getAccessToken()
        {
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
            paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + result + "]");
            string token = zhi.Rows[0]["access_token"].ToString();
            return token;
        }
        #endregion

        #region 图片、视频转base64
        public static String getFileBase64(String fileName)
        {
            FileStream filestream = new FileStream(temppath+fileName, FileMode.Open);
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string baser64 = Convert.ToBase64String(arr);
            filestream.Close();
            return baser64;
        }
        #endregion

        #region base64转图片
        public string Base64ToFile(string base64, string filename)
        {
            byte[] bt = Convert.FromBase64String(base64);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bt);
            Bitmap bitmap = new Bitmap(stream);
            string imgPath = temppath + filename + ".jpg";
            bitmap.Save(imgPath);
            return imgPath;
        }
        #endregion
    }
}