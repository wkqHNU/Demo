using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure.Tool
{
    public class CsvFileHelper
    {
        /// <summary>
        /// Csv转dataTable
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable Csv2DataTable(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                sr = new StreamReader(fileName, System.Text.Encoding.Default);
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;

                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    aryLine = strLine.Split(',');
                    if (IsFirst == true)
                    {
                        IsFirst = false;
                        columnCount = aryLine.Length;
                        //创建列,并命名字段名称
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(aryLine[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                sr.Close();
                fs.Close();
            }
            return dt;
        }
        public static void DataTable2Csv(DataTable dt, string fullPath)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                var fi = new FileInfo(fullPath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                sw = new StreamWriter(fs, Encoding.UTF8);
                var data = "";
                //写出列名称
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName;
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写出各行数据
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        var str = dt.Rows[i][j].ToString();
                        str = str.Replace("\"", "\"\""); //替换英文冒号 英文冒号需要换成两个冒号
                        if (str.Contains(',') || str.Contains('"')
                            || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                        {
                            str = string.Format("\"{0}\"", str);
                        }

                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
            }
            catch { }
            finally
            {
                sw.Close();
                fs.Close();
            }
        }
    }
}
