using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace faPiao.Controllers
{
    public class ExcelDAO
    {
        //定义excel后处理委托
        public delegate bool postHandler(Microsoft.Office.Interop.Excel._Worksheet ws);
        public delegate bool preHandler(Microsoft.Office.Interop.Excel._Worksheet ws);

        #region 获取excel文件，兼容2003和2007文件

        private string _excelObject = "Provider=Microsoft.{0}.OLEDB.{1};Data Source={2};Extended Properties=\"Excel {3};HDR={4};IMEX={5}\"";
        private string _filepath = string.Empty;
        //HDR=Yes，这代表第一行是标题，不做为数据使用 ，如果用HDR=NO，则表示第一行不是标题，做为数据来使用。系统默认的是YES
        private string _hdr = "Yes";
        /*当 IMEX=0 时为“汇出模式”，这个模式开启的 Excel 档案只能用来做“写入”用途。
　　      当 IMEX=1 时为“汇入模式”，这个模式开启的 Excel 档案只能用来做“读取”用途。
　　      当 IMEX=2 时为“连結模式”，这个模式开启的 Excel 档案可同时支援“读取”与“写入”用途。
        意义如下:
        0 ---输出模式;
        1---输入模式;
        2----链接模式(完全更新能力)*/
        private string _imex = "1";
        private OleDbConnection _con;

        #region 构造函数
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="filepath">文件路径</param>  
        public void ExcelHelper(string filepath)
        {
            this._filepath = filepath;
        }
        #endregion

        #region 方法
        /// <summary>  
        /// 获取模式（本方法中未使用）
        /// </summary>  
        /// <returns>模式</returns>  
        public System.Data.DataTable GetSchema()
        {
            System.Data.DataTable dtSchema = null;
            try
            {
                if (this.Connection.State != ConnectionState.Open) this.Connection.Open();
                dtSchema = this.Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                return dtSchema;
            }
            catch (Exception e)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Rows.Add();
                dt.Columns.Add("id", typeof(string));
                dt.Columns.Add("text", typeof(string));
                dt.Rows[0]["id"] = "111";
                dt.Rows[0]["text"] = e.Message;
                return dt;
            }
        }
        /// <summary>  
        ///读表
        /// </summary>  
        /// <param name="tableName">表名</param>    
        /// <returns>返回表</returns>  
        public System.Data.DataTable ReadTable(string tableName)
        {
            return this.ReadTable(tableName, "", "");
        }
        /// <summary>  
        ///读表
        /// </summary>  
        /// <param name="tableName">表名</param>
        /// <param name="Xiang">查询项</param>
        /// <returns>返回表</returns>  
        public System.Data.DataTable ReadTable(string tableName, string Xiang)
        {
            return this.ReadTable(tableName, "", Xiang);
        }
        /// <summary>  
        /// 读表  
        /// </summary>  
        /// <param name="tableName">表名</param>  
        /// <param name="criteria">查询条件</param> 
        /// <param name="Xiang">查询项</param> 
        /// <returns>返回表</returns>  
        public System.Data.DataTable ReadTable(string tableName, string criteria, string Xiang)
        {
            try
            {
                if (this.Connection.State != ConnectionState.Open)
                {
                    this.Connection.Open();
                }
                string cmdText = "SELECT " + Xiang + " FROM [{0}]";
                if (!string.IsNullOrEmpty(criteria))
                {
                    cmdText += " WHERE " + criteria;
                }
                string tableNameSuffix = string.Empty;
                tableNameSuffix = "$";
                OleDbCommand cmd = new OleDbCommand(string.Format(cmdText, tableName + tableNameSuffix));
                cmd.Connection = this.Connection;
                OleDbDataAdapter adpt = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                adpt.Fill(ds, tableName);
                if (ds.Tables.Count >= 1)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Rows.Add();
                dt.Columns.Add("错误信息");
                dt.Rows[0]["错误信息"] = e.Message;
                return dt;
            }
        }
        /// <summary>  
        /// 删除表
        /// </summary>  
        /// <param name="tableName">表名</param>  
        public void DropTable(string tableName)
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open();

            }
            string cmdText = "Drop Table [{0}]";
            using (OleDbCommand cmd = new OleDbCommand(string.Format(cmdText, tableName), this.Connection))
            {
                cmd.ExecuteNonQuery();

            }
            this.Connection.Close();
        }
        /// <summary>  
        /// 创表
        /// </summary>  
        /// <param name="tableName">表名</param>  
        /// <param name="tableDefinition">写入表的内容</param>  
        public void WriteTable(string tableName, Dictionary<string, string> tableDefinition)
        {
            using (OleDbCommand cmd = new OleDbCommand(this.GenerateCreateTable(tableName, tableDefinition), this.Connection))
            {
                if (this.Connection.State != ConnectionState.Open) this.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>  
        /// 添加新行 
        /// </summary>  
        /// <param name="dr">行数据</param>  
        public void AddNewRow(DataRow dr)
        {
            string command = this.GenerateInsertStatement(dr);
            ExecuteCommand(command);
        }
        /// <summary>  
        /// 执行语句
        /// </summary>  
        /// <param name="command">命令语句</param>  
        public void ExecuteCommand(string command)
        {
            using (OleDbCommand cmd = new OleDbCommand(command, this.Connection))
            {
                if (this.Connection.State != ConnectionState.Open) this.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>  
        /// 生成创建表脚本
        /// </summary>  
        /// <param name="tableName">表名</param>  
        /// <param name="tableDefinition">表定义</param>  
        /// <returns>创建表脚本</returns>  
        private string GenerateCreateTable(string tableName, Dictionary<string, string> tableDefinition)
        {
            StringBuilder sb = new StringBuilder();
            bool firstcol = true;
            sb.AppendFormat("CREATE TABLE [{0}](", tableName);
            firstcol = true;
            foreach (KeyValuePair<string, string> keyvalue in tableDefinition)
            {
                if (!firstcol)
                {
                    sb.Append(",");
                }
                firstcol = false;
                sb.AppendFormat("{0} {1}", keyvalue.Key, keyvalue.Value);
            }
            sb.Append(")");
            return sb.ToString();
        }
        /// <summary>  
        /// 生成插入语句脚本
        /// </summary>  
        /// <param name="dr">数据行</param>  
        /// <returns>插入语句脚本</returns>  
        private string GenerateInsertStatement(DataRow dr)
        {
            StringBuilder sb = new StringBuilder();
            bool firstcol = true;
            sb.AppendFormat("INSERT INTO [{0}](", dr.Table.TableName);
            foreach (DataColumn dc in dr.Table.Columns)
            {
                if (!firstcol)
                {
                    sb.Append(",");
                }
                firstcol = false;

                sb.Append(dc.Caption);
            }
            sb.Append(") VALUES(");
            firstcol = true;
            for (int i = 0; i <= dr.Table.Columns.Count - 1; i++)
            {
                if (!object.ReferenceEquals(dr.Table.Columns[i].DataType, typeof(int)))
                {
                    sb.Append("'");
                    sb.Append(dr[i].ToString().Replace("'", "''"));
                    sb.Append("'");
                }
                else
                {
                    sb.Append(dr[i].ToString().Replace("'", "''"));
                }
                if (i != dr.Table.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
        /// <summary>  
        /// 处理
        /// </summary>  
        public void Dispose()
        {
            if (this._con != null && this._con.State == ConnectionState.Open)
                this._con.Close();
            if (this._con != null)
                this._con.Dispose();
            this._con = null;
            this._filepath = string.Empty;
        }
        #endregion

        #region  属性
        /// <summary>  
        /// 获取连接字符串
        /// </summary>  
        public string ConnectionString
        {
            get
            {
                string result = string.Empty;
                if (String.IsNullOrEmpty(this._filepath))
                    return result;
                //检查文件格式
                FileInfo fi = new FileInfo(this._filepath);
                if (fi.Extension.Equals(".xls"))
                {
                    result = string.Format(this._excelObject, "Jet", "4.0", this._filepath, "8.0", this._hdr, this._imex);
                }
                else if (fi.Extension.Equals(".xlsx"))
                {
                    result = string.Format(this._excelObject, "Ace", "12.0", this._filepath, "12.0", this._hdr, this._imex);
                }
                return result;
            }
        }
        /// <summary>  
        /// 获取连接
        /// </summary>  
        public OleDbConnection Connection
        {
            get
            {
                if (_con == null)
                {
                    this._con = new OleDbConnection { ConnectionString = this.ConnectionString };
                }
                return this._con;
            }
        }

        /// <summary>  
        /// Gets or sets a HDR  
        /// </summary>  
        public string Hdr
        {
            get
            {
                return this._hdr;
            }
            set
            {
                this._hdr = value;
            }
        }

        /// <summary>  
        /// Gets or sets an IMEX  
        /// </summary>  
        public string Imex
        {
            get
            {
                return this._imex;
            }
            set
            {
                this._imex = value;
            }
        }
        #endregion

        public void InsertPicture(Microsoft.Office.Interop.Excel.Range rng, _Worksheet sheet, string PicturePath, double HeightOff)
        {
            // 单位磅
            double PicLeft, PicTop, PicNewWidth, PicOldWidth, PicNewHeight, WidthOff;

            Microsoft.Office.Interop.Excel.Shape shape = sheet.Shapes.AddPicture(PicturePath,
                Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0, -1, -1);

            PicNewHeight = (Convert.ToDouble(rng.Height) - 2 * HeightOff);
            PicNewWidth = shape.Width * PicNewHeight / shape.Height;

            WidthOff = (Convert.ToDouble(rng.Width) - PicNewWidth) / 2;
            if (WidthOff < HeightOff * 1.3)
            {
                WidthOff = HeightOff * 1.3;
                shape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoFalse;
                shape.Width = Convert.ToSingle(Convert.ToDouble(rng.Width) - 2 * WidthOff);
            }

            PicTop = Convert.ToDouble(rng.Top) + HeightOff;
            PicLeft = Convert.ToDouble(rng.Left) + WidthOff;

            shape.Height = (float)PicNewHeight;

            shape.Left = (float)PicLeft;
            shape.Top = (float)PicTop;




        }

        public static object[,] YHHT_Getobjs(string DataFile, string shtname, out int rowcount)
        {
            try
            {
                _Application _excel = new Application();
                _excel.Visible = false;
                Workbooks wbs = _excel.Application.Workbooks;
                _Workbook wkb = wbs.Open(DataFile);
                _Worksheet wsh = (_Worksheet)wkb.Worksheets[shtname];
                object[,] objs = null;
                double d_rowcount = 0;
                string temp = "";
                if (wsh.Range["H2"].Value != null)
                {
                    temp = wsh.Range["H2"].Value.ToString().Trim();
                }
                if (double.TryParse(temp, out d_rowcount))
                {
                    rowcount = (int)d_rowcount;
                    if (rowcount <= 0)
                    {
                        rowcount = 1000;
                    }
                }
                else
                {
                    rowcount = 1000;
                }

                objs = (object[,])wsh.Range["A1:H" + rowcount.ToString()].Value;
                wkb.Close(SaveChanges: false);
                wbs.Close();
                _excel.Quit();
                _excel = null;
                GC.Collect();
                return objs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        public static object[,] SC_Getobjs(string DataFile, string shtname, string liename, out int rowcount)
        {
            _Application _excel = new Application();
            try
            {
                _excel.Visible = false;
                Workbooks wbs = _excel.Application.Workbooks;
                _Workbook wkb = wbs.Open(DataFile);
                _Worksheet wsh = (_Worksheet)wkb.Worksheets[shtname];
                object[,] objs = null;
                rowcount = wsh.UsedRange.CurrentRegion.Rows.Count;

                objs = (object[,])wsh.Range["A1:" + liename + rowcount.ToString()].Value;
                wkb.Close(SaveChanges: false);
                wbs.Close();
                _excel.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(wbs);
                wbs = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wsh);
                wsh = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_excel);
                _excel = null;

                return objs;
            }
            catch (Exception e)
            {
                _excel.Quit();
                _excel = null;
                GC.Collect();
                throw e;
            }
        }

    }
}
