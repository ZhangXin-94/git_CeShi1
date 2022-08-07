using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Word;

namespace FaPiao.Models
{
    public class WordDao
    {
        #region 创建WORD
        /// <summary>
        /// 创建WORD
        /// </summary>
        /// <returns></returns>
        public void XinJianWord()
        {
            object oMissing = System.Reflection.Missing.Value;
            Application oWord;
            Document oDoc;
            oWord = new Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
        }
        #endregion

        #region 打开Word
        /// <summary>
        /// 打开Word
        /// </summary>
        /// <param name="file">文件地址</param>
        public void DaKaiWord(string file)
        {
            object oMissing = System.Reflection.Missing.Value;
            Application oWord;
            Document oDoc;
            oWord = new Application();
            oWord.Visible = true;
            object fileName = file;
            oDoc = oWord.Documents.Open(ref fileName,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
        }
        #endregion

        internal static bool WordDaoChu(string MoBanFile, System.Data.DataTable BiaoXinXi,string BaoCunPath)
        {
            object oMissing = System.Reflection.Missing.Value;

            _Application oWord;
            _Document oDoc;
            oWord = new Application();
            //false文件不显示,true显示
            oWord.Visible = false;
            Object saveChanges = oWord.Options.BackgroundSave;//关闭doc文档不提示保存
            //打开模版文件  
            object fileName = MoBanFile;
            object savePath = BaoCunPath;//文档另存为的路径
            oDoc = oWord.Documents.Add(ref fileName, ref oMissing, ref oMissing, ref oMissing);
            try
            {
                oDoc.Activate();

                object NeiRong = "NeiRong";
                Bookmark DX = oDoc.Bookmarks.get_Item(ref NeiRong);
                string Zhi = "";
                foreach (DataRow dx in BiaoXinXi.Rows) 
                {
                    Zhi += dx["NeiRong"].ToString();
                }
                DX.Range.Text = Zhi;

                //文档另存为  
                oDoc.SaveAs(ref savePath, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                oDoc.Close(ref saveChanges, ref oMissing, ref oMissing);//关闭文档   
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);     //关闭应用程序 

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
                oDoc = null;

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
                oWord = null;

                return true;
            }
            catch (Exception e)
            {
                oDoc.SaveAs(ref savePath, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                oDoc.Close(ref saveChanges, ref oMissing, ref oMissing);//关闭文档   
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);     //关闭应用程序 
                oDoc = null;
                oWord = null;
                GC.Collect();
                return false;
            }
        }
    }
}