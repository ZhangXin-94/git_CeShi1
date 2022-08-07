using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Aspose.Pdf;
using FaPiao.Models;
using iocr_api_demo;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Spire.Pdf;

namespace faPiao.Controllers
{
    public class FaPiaoController : Controller
    {
        //
        // GET: /FaPiao/

        public ActionResult Index()
        {
            return View();
        }


        #region 源代码
        //url调用返回结果
        private static String SendRequest(string url, Encoding encoding, string fangfa)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = fangfa;
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), encoding);
            return sr.ReadToEnd();
        }

        //public static string temppath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["TuPian"].ToString().Trim());
        public static string temppath = "";

        //public static string template = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["Template"].ToString().Trim());
        public static string template = "";

        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存

        #region 发票识别
        private static String clientId = "ZE8xO1ccUamY7qMvWoUE81Y9";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "nHlQ1azeGoCdWHDYsIM5ruGxDjRi1LBQ";
        #endregion



        //#region 人脸识别
        //private static String clientId = "l2MIGpsVEvHxhwundcjbD8ti";
        ////百度云中开通对应服务应用的 Secret Key
        //private static String clientSecret = "EYrlZdHQy3dmA3l8EFEheLgRH8jQXSS2";
        //#endregion

        //#region 图像识别
        //private static String clientId = "vp1rFCZHsPXHsgdVVYTvGHga";
        //// 百度云中开通对应服务应用的 Secret Key
        //private static String clientSecret = "FSz7eEdvTN8gb7phUstlGpWNdQP9Dbzy";
        //#endregion

        //获取access_token
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
            Console.WriteLine(result);
            return result;
        }
        protected static string token;

        //图片、视频转base64
        public static String getFileBase64(String fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Open);
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string baser64 = Convert.ToBase64String(arr);
            filestream.Close();
            return baser64;
        }

        //base64转图片
        public string Base64ToFile(string base64, string filename)
        {
            byte[] bt = Convert.FromBase64String(base64);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bt);
            Bitmap bitmap = new Bitmap(stream);
            string imgPath = temppath + filename + ".jpg";
            bitmap.Save(imgPath);
            return imgPath;
        }

        #region 文字识别

        #region 通用票据识别
        public string receipt()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/receipt?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "通用发票.png");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region 增值税发票识别
        public string vatInvoice(string filename)
        {
            //获取
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/vat_invoice?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "测试1.jpg");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region 火车票识别
        public string trainTicket()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/train_ticket?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "火车票.png");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region 出租车票识别
        public string taxiReceipt()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/taxi_receipt?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "出租车票1.jpg");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region 定额发票识别
        public string quotaInvoice()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/quota_invoice?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "定额发票.png");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region 通用机打发票识别
        public string invoice()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/invoice?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "通用机打发票.png");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region 飞机行程单识别
        public string airTicket()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/air_ticket?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "飞机行程单.jpg");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region 混合发票识别
        public string PostHttp()
        {
            // fileUtils.cs 类下载地址
            // https://bj.bcebos.com/v1/iocr-movie/FileUtils.cs
            // iocr识别api_url
            String recognise_api_url = "https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise/finance";

            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string access_token = zhi.Rows[0]["access_token"].ToString();
            String image_path = temppath + "3.jpg";
            String image_b64 = FileUtils.getFileBase64(image_path);
            // iocr按模板id识别的请求bodys
            string templateSign = "mixed_receipt";
            string recognise_bodys = "access_token=" + access_token + "&templateSign=" + templateSign +
                            "&image=" + HttpUtility.UrlEncode(image_b64);
            // iocr按分类器id识别的请求bodys
            int classifierId = 10001;
            String classifier_bodys = "access_token=" + access_token + "&classifierId=" + classifierId + "&image=" + HttpUtility.UrlEncode(image_b64);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(recognise_api_url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            try
            {
                // 请求模板id
                byte[] btBodys = Encoding.UTF8.GetBytes(recognise_bodys);
                // 请求分类器id
                // byte[] btBodys = Encoding.UTF8.GetBytes(classifier_bodys);
                httpWebRequest.ContentLength = btBodys.Length;
                httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                string responseContent = streamReader.ReadToEnd();
                JObject jo_result = (JObject)JsonConvert.DeserializeObject(responseContent);
                int arr = (int)jo_result["error_code"];
                if (arr != 0)
                {
                    //错误
                }
                else
                {

                }
                return responseContent;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion

        #region pdf识别
        public string PDFShiBie()
        {
            string aa = "";
            string filepath = @"E:\开发项目\苏州绕城\数据回家\1\阳北\3.20\阳北出口客车预付.pdf";
            string base64 = getFileBase64(filepath);
            int pageNum = GetPageCount(filepath);

            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string access_token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/accurate?access_token=" + access_token;
            Encoding encoding = Encoding.UTF8;
            DataTable table = new DataTable();
            table.Columns.Add("XuHao", typeof(int));
            table.Columns.Add("NeiRong", typeof(string));
            for (int i = 1; i <= pageNum; i++)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "post";
                request.KeepAlive = true;
                String str = "pdf_file_num=" + i + "&pdf_file=" + HttpUtility.UrlEncode(base64);
                byte[] buffer = encoding.GetBytes(str);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string result = reader.ReadToEnd();
                JObject jo_result = (JObject)JsonConvert.DeserializeObject(result);

                JArray arr = (JArray)jo_result["words_result"];
                foreach (var jObject in arr)
                {
                    string words = jObject["words"].ToString();
                    aa = aa + words;
                }
                table.Rows.Add();
                table.Rows[i - 1]["XuHao"] = i;
                table.Rows[i - 1]["NeiRong"] = aa;
            }
            string MoBanPath = @"E:\开发项目\苏州绕城\数据回家\" + "PDFToWord模板.docx";
            string BaoCunPath = @"F:\PDFToWord结果.docx";
            bool bl = WordDao.WordDaoChu(MoBanPath, table, BaoCunPath);
            if (bl)
            {
                return "操作成功！";
            }
            else
            {
                return "操作失败！";
            }
        }
        //获取文件的页数
        public int GetPageCount(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs);
            //从流的当前位置到末尾读取流
            string pdfText = reader.ReadToEnd();
            Regex rgx = new Regex(@"/Type\s*/Page[^s]");
            MatchCollection matches = rgx.Matches(pdfText);
            int count = matches.Count;
            return count;
        }
        #endregion


        #region 身份证识别
        public string idcard()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string token = zhi.Rows[0]["access_token"].ToString();
            //请求地址
            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/idcard?access_token=" + token;

            // 图片的base64编码
            string base64 = getFileBase64(temppath + "身份证2.jpg");
            //id_card_side:front：身份证含照片的一面,back：身份证带国徽的一面
            String str = "id_card_side=" + "front" + "&image=" + HttpUtility.UrlEncode(base64);
            string result = SendRequest(host, str);
            //返回数据{"words_result":{"姓名":{"location":{定位},"words":"名字"},"民族":{"location":{定位},"words":"汉"},"住址":{定位},"words":"江苏省海门市"},"公民身份号码":{定位},"words":"320684199604116699"},"出生":{定位},"words":"19960411"},"性别":{定位},"words":"女"}},"idcard_number_type":1(- 1： 身份证正面所有字段全为空0： 身份证证号不合法，此情况下不返回身份证证号1： 身份证证号和性别、出生信息一致2： 身份证证号和性别、出生信息都不一致3： 身份证证号和出生信息不一致4： 身份证证号和性别信息不一致),"words_result_num":识别结果数,"image_status":"normal（normal-识别正常reversed_side-身份证正反面颠倒non_idcard-上传的图片中不包含身份证blurred-身份证模糊other_type_card-其他类型证照over_exposure-身份证关键字段反光或过曝over_dark-身份证欠曝（亮度过低）unknown-未知状态）","log_id":1423166200083399440}

            return result;
        }
        #endregion

        #endregion

        #region 人脸识别
        // 人脸注册
        public string add()
        {
            //获取access_token
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            //人脸注册调用接口
            string host = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/add?access_token=" + token;
            //人员证件照转base64
            string image = getFileBase64(temppath + "用户1.png");
            //请求参数：image:图片地址，image_type：图片类型，group_id：用户组id（百度云应用中创建），user_id:用户id,user_info：用户资料，quality_control：图片质量控制（NONE: 不进行控制LOW:较低的质量要求NORMAL: 一般的质量要求HIGH: 较高的质量要求默认 NONE），liveness_control：活体检测控制（NONE: 不进行控制LOW:较低的活体要求(高通过率 低攻击拒绝率)NORMAL: 一般的活体要求(平衡的攻击拒绝率, 通过率)HIGH: 较高的活体要求(高攻击拒绝率 低通过率)默认NONE）
            String str = "{\"image\":\"" + image + "\",\"image_type\":\"BASE64\",\"group_id\":\"RCXG\",\"user_id\":\"user1\",\"user_info\":\"用户\",\"quality_control\":\"LOW\",\"liveness_control\":\"NORMAL\"}";
            string result = SendRequest(host, str);
            //返回数据{"face_token": "人脸图片的唯一标识","location": {定位},"log_id":"请求标识码，随机数"}
            return result;
            //Baidu.Aip.Face.Face client = new Baidu.Aip.Face.Face(clientId, clientSecret);
            //client.Timeout = 60000;  // 修改超时时间
            //string image = getFileBase64(temppath + "张鑫1.png");
            //var imageType = "BASE64";
            ////注册人脸
            //var groupId = "BHZX";
            //var userId = "ZX";
            //// 如果有可选参数
            //var options = new Dictionary<string, object>{
            //            {"user_info", "申松身份证"},
            //            {"quality_control", "NORMAL"},
            //            {"liveness_control", "LOW"}
            //        };
            //// 带参数调用人脸注册
            //var result = client.UserAdd(image, imageType, groupId, userId, options);
            //JObject jo_result = (JObject)JsonConvert.DeserializeObject(result.ToString());
            //if ((string)jo_result["error_msg"] == "SUCCESS")
            //{
            //    return "face_token：" + (string)jo_result["result"]["face_token"];
            //}
            //else
            //{
            //    return (string)jo_result["error_msg"];
            //}
        }
        //获取用户人脸列表
        public string FaceGetlist()
        {
            Baidu.Aip.Face.Face client = new Baidu.Aip.Face.Face(clientId, clientSecret);
            client.Timeout = 60000;  // 修改超时时间
            var userId = "SS1";

            var groupId = "BHZX";

            // 调用获取用户人脸列表，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.FaceGetlist(userId, groupId);
            JObject jo_result = (JObject)JsonConvert.DeserializeObject(result.ToString());
            string FanHui = (string)jo_result["face-list"];
            return FanHui;
        }
        //人脸删除
        public string FaceDelete()
        {
            //获取access_token
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            //人脸注册调用接口
            string host = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/face/delete?access_token=" + token;

            //用户id
            var userId = "user1";
            //用户组id
            var groupId = "RCXG";
            //需要删除的人脸图片token
            var faceToken = "2fa64a88a9d5118916f9a303782a97d3";
            String str = "{\"user_id\":\"" + userId + "\",\"group_id\":\"" + groupId + "\",\"face_token\":\"" + faceToken + "\"}";
            string result = SendRequest(host, str);
            //返回数据{"error_code": "0","log_id":"请求标识码，随机数"}
            return result;
            //Baidu.Aip.Face.Face client = new Baidu.Aip.Face.Face(clientId, clientSecret);
            //client.Timeout = 60000;  // 修改超时时间
            //var userId = "SS1";

            //var groupId = "BHZX";

            //var faceToken = "";

            //// 调用人脸删除，可能会抛出网络等异常，请使用try/catch捕获
            //var result = client.FaceDelete(userId, groupId, faceToken);
            //JObject jo_result = (JObject)JsonConvert.DeserializeObject(result.ToString());
            //if ((string)jo_result["error_code"] == "0")
            //{
            //    return "操作成功！";
            //}
            //else
            //{
            //    return (string)jo_result["error_msg"];
            //}
        }
        //人脸检测
        public string DetectDemo()
        {
            //获取access_token
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            //人脸注册调用接口
            string host = "https://aip.baidubce.com/rest/2.0/face/v3/detect?access_token=" + token;
            //人员证件照转base64
            string image = getFileBase64(temppath + "用户1.png");
            //face_field包括age年龄,beauty美丑打分,expression表情,face_shape脸型,gender性别,glasses是否戴眼镜,quality人脸质量信息,逗号分隔.
            string face_field = "age,gender";
            String str = "{\"image\":\"" + image + "\",\"image_type\":\"BASE64\",\"face_field\":\"" + face_field + "\"}";
            string result = SendRequest(host, str);

            //返回数据{	 "face_num": 人脸数量, "face_list": [ { "face_token": "人脸图片唯一标识", "location": { 定位},"face_probability":人脸置信度, "age": 年龄,  "gender": { "type": "male性别", "probability": 性别置性度 } }  ]}
            return result;
            //        Baidu.Aip.Face.Face client = new Baidu.Aip.Face.Face(clientId, clientSecret);
            //        client.Timeout = 60000;  // 修改超时时间
            //        string image = getFileBase64(temppath + "崔伟铭1.jpg");

            //        var imageType = "BASE64";
            //        // 如果有可选参数age,beauty,expression,face_shape,gender,glasses,landmark,landmark150,quality,eye_status,emotion,face_type,mask,spoofing
            //        var options = new Dictionary<string, object>{
            //    {"face_field", "age,beauty,expression,face_shape,gender"},
            //    {"face_type", "LIVE"},
            //    {"liveness_control", "LOW"}
            //};
            //        // 带参数调用人脸检测
            //        var result = client.Detect(image, imageType, options);
            //        JObject jo_result = (JObject)JsonConvert.DeserializeObject(result.ToString());
            //        if ((string)jo_result["error_msg"] == "SUCCESS")
            //        {
            //            return "face_token：" + (string)jo_result["result"]["face_list"][0]["face_token"] + ";年龄：" + (string)jo_result["result"]["face_list"][0]["age"] + ";美丑打分：" + (string)jo_result["result"]["face_list"][0]["beauty"] + ";表情：" + (string)jo_result["result"]["face_list"][0]["expression"]["type"] + ";脸型：" + (string)jo_result["result"]["face_list"][0]["face_shape"]["type"] + ";性别：" + (string)jo_result["result"]["face_list"][0]["gender"]["type"];
            //        }
            //        else
            //        {
            //            return (string)jo_result["error_msg"];
            //        }
        }
        // 人脸搜索
        public string faceSearch()
        {
            //获取access_token
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            token = zhi.Rows[0]["access_token"].ToString();
            //人脸注册调用接口
            string host = "https://aip.baidubce.com/rest/2.0/face/v3/search?access_token=" + token;

            //人员证件照转base64
            string image = getFileBase64(temppath + "申松1.jpg");
            //用户组标识
            var group_id_list = "BHZX";
            String str = "{\"image\":\"" + image + "\",\"image_type\":\"BASE64\",\"group_id_list\":\"" + group_id_list + "\"}";
            string result = SendRequest(host, str);
            //返回数据 { "face_token": "人脸标志",  "user_list": [  {  "group_id" : "用户所属的group_id", "user_id": "用户的user_id",  "user_info": "注册用户时携带的user_info", "score": 99.3用户的匹配得分   }] }最匹配的用户信息
            return result;
            //        Baidu.Aip.Face.Face client = new Baidu.Aip.Face.Face(clientId, clientSecret);
            //        client.Timeout = 60000;  // 修改超时时间
            //        string image = getFileBase64(temppath + "申松1.jpg");

            //        var imageType = "BASE64";

            //        var group_id_list = "BHZX";

            //        var options = new Dictionary<string, object>{
            //    {"quality_control", "LOW"}
            //};
            //        // 带参数调用人脸检测
            //        var result = client.Search(image, imageType, group_id_list, options);
            //        JObject jo_result = (JObject)JsonConvert.DeserializeObject(result.ToString());
            //        if ((string)jo_result["error_msg"] == "SUCCESS")
            //        {
            //            return "face_token：" + (string)jo_result["result"]["face_token"] + ";组标识：" + (string)jo_result["result"]["user_list"][0]["group_id"] + ";用户标识：" + (string)jo_result["result"]["user_list"][0]["user_id"] + ";用户信息：" + (string)jo_result["result"]["user_list"][0]["user's info"] + ";得分：" + (string)jo_result["result"]["user_list"][0]["score"];
            //        }
            //        else
            //        {
            //            return (string)jo_result["error_msg"];
            //        }
        }
        //人脸实名认证(需要企业认证)
        public string PersonVerify()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/face/v3/person/verify?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            var image = temppath + "申松3.png";

            String str = "{\"image\":\"" + getFileBase64(image) + "\",\"image_type\":\"BASE64\",\"id_card_number\":\"140102198204285198\",\"name\":\"申松\",\"quality_control\":\"NORMAL\",\"liveness_control\":\"LOW\"}";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            JObject jo_result = (JObject)JsonConvert.DeserializeObject(result);
            if ((string)jo_result["err_msg"] == "SUCCESS")
            {
                return "相似度：" + (string)jo_result["result"]["score"];
            }
            else
            {
                return (string)jo_result["err_msg"];
            }

        }

        //视频活体检测(0:眨眼  2:右转 3:左转  4:抬头  5:低头)
        public string videoFace()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string token = zhi.Rows[0]["access_token"].ToString();

            #region 随机校验码接口
            string session_id = "";
            string host = "https://aip.baidubce.com/rest/2.0/face/v1/faceliveness/sessioncode?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "post";
            request.KeepAlive = true;
            String str = "type=1&min_code_length=1&max_code_length=1";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            JObject jo_result = (JObject)JsonConvert.DeserializeObject(result);
            if ((string)jo_result["err_msg"] == "SUCCESS")
            {
                session_id = (string)jo_result["result"]["session_id"];
            }
            else
            {
                return "获取验证码错误！";
            }
            #endregion

            #region 视频活体检测
            string host2 = "https://aip.baidubce.com/rest/2.0/face/v1/faceliveness/verify?access_token=" + token;
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(host2);
            request2.ContentType = "application/x-www-form-urlencoded";
            request2.Method = "post";
            request2.KeepAlive = true;
            string videopath = getFileBase64(temppath + "视频2.MP4");
            String str2 = "type_identify=action&video_base64=" + HttpUtility.UrlEncode(videopath) + "&session_id=" + session_id;
            byte[] buffer2 = encoding.GetBytes(str2);
            request2.ContentLength = buffer2.Length;
            request2.GetRequestStream().Write(buffer2, 0, buffer2.Length);
            HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();
            StreamReader reader2 = new StreamReader(response2.GetResponseStream(), Encoding.UTF8);
            string result2 = reader2.ReadToEnd();
            JObject jo_result2 = (JObject)JsonConvert.DeserializeObject(result2);
            #endregion

            if ((string)jo_result2["err_msg"] == "SUCCESS")
            {
                return "质量最佳图片：" + Base64ToFile((string)jo_result2["result"]["best_image"]["pic"], "质量最佳图片");
            }
            else
            {
                return (string)jo_result2["err_msg"];
            }
        }

        #endregion

        #region 图片搜索
        // 相似图检索—入库
        public string similarAdd()
        {
            /**
             * 请求参数：image：图片数据base64；brief:brief信息请尽量填写可关联至本地图库的图片id或者图片url、图片名称等信息;tags:1 - 65535范围内的整数,检索时可圈定分类维度进行检索
             * 返回参数：log_id:唯一的log id;cont_sign:图片的签名信息，请务必保存至本地
             * 
             * */

            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/add?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "坑槽5.jpg");
            String str = "brief=" + "{\"name\":\"坑槽5\", \"id\":\"5\"}" + "&image=" + HttpUtility.UrlEncode(base64) + "&tags=" + "5,5";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }

        // 相似图检索—检索
        public string similarSearch()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string token = zhi.Rows[0]["access_token"].ToString();

            string host = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/search?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "坑槽2.jpg");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }
        // 相似图检索—删除
        public string similarDelete()
        {
            string FanHui = getAccessToken();
            DataTable zhi = JsonConvert.DeserializeObject<DataTable>("[" + FanHui + "]");
            string token = zhi.Rows[0]["access_token"].ToString();
            string host = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/delete?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            string base64 = getFileBase64(temppath + "坑槽2.jpg");
            String str = "image=" + HttpUtility.UrlEncode(base64);
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            return result;
        }

        #endregion

        /// <summary>     
        ///获取url地址和参数输出内容     
        /// </summary> /// <param name="url">url</param>      
        /// <param name="str">请求参数</param>  
        private static String SendRequest(string url, string str)
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

        public string pdfToword()
        {
            string filepath = temppath + "初中数学公式大全(2).pdf";
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(filepath);//pdf物理路径
            Console.WriteLine("转换中请耐心等待.....");
            doc.SaveToFile(@"C:\Users\zx\Desktop\pdf转word\初衷数学公式大全（2）.docx", FileFormat.DOCX);//生成word的物理路径
            return "";
        }

        #region 文件下载
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="UIID">界面标识</param>
        /// <param name="FileUrl">文件地址</param>
        /// <returns></returns>
        public ActionResult FileDown(string UIID, string FileUrl)
        {
            try
            {
                var path = Server.MapPath(FileUrl);
                var name = Path.GetFileName(path);
                return File(path, "application/x-zip-compressed", Url.Encode(name));
            }
            catch (Exception)
            {
                return View();
            }
        }
        #endregion
        #endregion
    }
}
