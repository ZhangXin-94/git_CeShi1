using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using faPiao.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace faPiao.Controllers
{
    public class BodyFenXiController : Controller
    {
        //
        // GET: /BodyFenXi/ 人体分析

        public ActionResult BodyIndex()
        {
            return View();
        }

        // 人体关键点识别
        public string body_analysis()
        {
            #region 请求参数
            //image:图片转base64，转后大小不能超过4M,支持图片的格式：jpg、bmp、png，最短边至少50px，最长边4096px
            #endregion 
            #region 返回参数
            //person_num:人体数目
            //log_id:唯一的log id，用于问题定位
            //person_info:人体姿态信息
            //+location:位置
            //++score:得分，越接近1表示识别准确的概率越大
            //+body_parts:身体部位信息
            //++top_head	头顶
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_eye	左眼
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_eye	右眼
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++nose	是	object	鼻子
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_ear		左耳
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_ear	是	object	右耳
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_mouth_corner	是	object	左嘴角
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_mouth_corner	是	object	右嘴角
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++neck	是	object	颈部
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_shoulder	是	object	左肩
            //+++x		x坐标
            //+++y		y坐标
            //+++score	概率分数
            //++right_shoulder	右肩
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_elbow	左手肘
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_elbow	右手肘
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_wrist	左手腕
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_wrist	右手腕
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_hip	左髋部
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_hip	右髋部
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_knee	左膝
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_knee	右膝
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++left_ankle	左脚踝
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            //++right_ankle	右脚踝
            //+++x		x坐标
            //+++y		y坐标
            //+++score		概率分数
            #endregion
            string FileName = "崔伟铭1.jpg";

            string token = Public.getAccessToken();
            string host = "https://aip.baidubce.com/rest/2.0/image-classify/v1/body_analysis?access_token=" + token;

            // 图片的base64编码
            string base64 = Public.getFileBase64(FileName);
            String str = "image=" + HttpUtility.UrlEncode(base64);
            string result = Public.SendRequest(host, str);
            //string result = "{\"person_num\":1,\"person_info\":[{\"body_parts\":{\"nose\":{\"score\":0.8792576789855957,\"x\":294.21093750,\"y\":369.42968750},\"right_knee\":{\"score\":0.02600745297968388,\"x\":279.05468750,\"y\":771.07031250},\"left_hip\":{\"score\":0.01350531727075577,\"x\":415.46093750,\"y\":808.96093750},\"right_ankle\":{\"score\":0.02618700638413429,\"x\":294.21093750,\"y\":793.80468750},\"right_wrist\":{\"score\":0.01089955121278763,\"x\":112.3359222412109,\"y\":771.07031250},\"left_eye\":{\"score\":0.8630960583686829,\"x\":354.83593750,\"y\":308.80468750},\"left_mouth_corner\":{\"score\":0.8870836496353149,\"x\":347.25781250,\"y\":437.63281250},\"right_elbow\":{\"score\":0.02759306505322456,\"x\":21.39842033386230,\"y\":778.64843750},\"left_knee\":{\"score\":0.04995160922408104,\"x\":339.67968750,\"y\":771.07031250},\"neck\":{\"score\":0.7558135986328125,\"x\":294.21093750,\"y\":566.46093750},\"top_head\":{\"score\":0.7492921352386475,\"x\":294.21093750,\"y\":111.7734146118164},\"right_ear\":{\"score\":0.8726317882537842,\"x\":157.8046722412109,\"y\":346.69531250},\"left_ear\":{\"score\":0.8491339683532715,\"x\":430.61718750,\"y\":339.11718750},\"left_elbow\":{\"score\":0.04444315284490585,\"x\":597.33593750,\"y\":840},\"right_shoulder\":{\"score\":0.6817380785942078,\"x\":36.55467224121094,\"y\":702.86718750},\"right_eye\":{\"score\":0.8574263453483582,\"x\":241.16406250,\"y\":301.22656250},\"right_mouth_corner\":{\"score\":0.8977979421615601,\"x\":256.32031250,\"y\":437.63281250},\"left_ankle\":{\"score\":0.07306844741106033,\"x\":324.52343750,\"y\":793.80468750},\"right_hip\":{\"score\":0.03925750777125359,\"x\":82.02342224121094,\"y\":771.07031250},\"left_wrist\":{\"score\":0.01386828906834126,\"x\":468.50781250,\"y\":808.96093750},\"left_shoulder\":{\"score\":0.6426456570625305,\"x\":567.02343750,\"y\":702.86718750}},\"location\":{\"score\":0.9995639920234680,\"top\":61.80377578735352,\"left\":0.0,\"width\":596.8062744140625,\"height\":776.0782470703125}}],\"log_id\":1492013416051919195}";
            JObject jo_result = (JObject)JsonConvert.DeserializeObject(result);
            //人数
            int RenShu = (int)jo_result["person_num"];
            //详细内容
            dynamic obj = jo_result["person_info"];
            foreach (dynamic dyn in obj)
            {
                string body_parts = dyn["body_parts"].ToString();
            }

            return result;
        }

    }
}
