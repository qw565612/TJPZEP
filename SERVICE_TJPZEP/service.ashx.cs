using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//using System;
//using System.Web;
using System.Net;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SERVICE_TJPZEP
{
    /// <summary>
    /// service 的摘要描述
    /// </summary>
    public class service : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string strDbConn = ConfigurationManager.ConnectionStrings["strDbConn"].ConnectionString;

                var reader = new System.IO.StreamReader(context.Request.InputStream);
                var str = reader.ReadToEnd();
                JObject json = JObject.Parse(str);

                string PUBLIC = "0";/*公眾查詢 - 對外公開網頁用*/

                string TYPE = string.Empty;
                string SP_NAME = string.Empty;
                string TOKEN = string.Empty;

                var dataParameterContain = "";
                foreach (var e in json)
                {
                    if (e.Key == "PUBLIC")
                    {
                        PUBLIC = e.Value.ToString();
                    }
                    else if (e.Key == "TYPE")
                    {
                        TYPE = e.Value.ToString();
                    }
                    else if (e.Key == "FUNCTION_NAME")
                    {
                        SP_NAME = e.Value.ToString();
                    }
                    else if (e.Key == "TOKEN")
                    {
                        TOKEN = e.Value.ToString();
                    }
                    else
                    {
                        dataParameterContain += "\"" + e.Key + "\":\"" + e.Value + "\",";
                    }
                }

                dataParameterContain = dataParameterContain.TrimEnd(',');
                var dataPar = @"{" + dataParameterContain + "}";
                JObject jsonPar = JObject.Parse(dataPar);
                foreach (var e in jsonPar)
                {
                    var ParKey = e.Key;
                    var ParVal = e.Value.ToString();
                }

                string RetJson = "";

                if (TYPE == string.Empty || SP_NAME == string.Empty)
                {
                    DataTable myDT;
                    myDT = MessageDT("-1", "遺失必要參數");
                    RetJson = DataSetToJSON("-1", "遺失必要參數", myDT);
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(RetJson);
                    return;
                }

                //else if ("I,U,D,Q".IndexOf(TYPE) >= 0 && TOKEN == string.Empty)
                else if ("I,U,D,Q".IndexOf(TYPE) >= 0 && TOKEN == string.Empty && PUBLIC == "0")
                {
                    DataTable myDT;
                    myDT = MessageDT("-1", "遺失必要參數");
                    RetJson = DataSetToJSON("-1", "遺失必要參數", myDT);
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(RetJson);
                    return;
                }

                #region /*登入驗證*/
                if (TYPE == "L")
                {

                    DataTable myDT;
                    DAStoreProcedure myDA = new DAStoreProcedure();

                    DataTable myParDT = myDA.fn_CreateParameterTable();
                    foreach (var e in jsonPar)
                    {
                        var ParKey = e.Key;
                        var ParVal = e.Value.ToString();
                        if (ParKey == "PWD")
                        {
                            ParVal = sha512(ParVal);
                        }

                        myDA.fn_CreateParameterrRow(myParDT, ParKey, ParVal, "string");

                    }
                    myDT = myDA.GetDTBySP(strDbConn, SP_NAME, myParDT);
                    RetJson = DataSetToJSON("1", "執行成功", myDT);
                }
                #endregion

                #region /*查詢*/

                if (TYPE == "Q")
                {
                    DataTable myDT;

                    #region/*非公開資料  PUBLIC == "0" 一定要檢查 TOKEN 合法性 */
                    if (PUBLIC == "0")
                    {
                        int retChkToken = checkToken(TOKEN, TYPE);

                        if (retChkToken < 0)
                        {
                            switch (retChkToken)
                            {
                                case -96:
                                    myDT = MessageDT("-96", "登入權限不足");
                                    RetJson = DataSetToJSON("-96", "登入權限不足" + TOKEN, myDT);
                                    break;

                                case -97:
                                    myDT = MessageDT("-97", "登入逾時");
                                    RetJson = DataSetToJSON("-97", "登入逾時" + TOKEN, myDT);
                                    break;
                                case -98:
                                    myDT = MessageDT("-98", "查無登入資訊");
                                    RetJson = DataSetToJSON("-98", "查無登入資訊" + TOKEN, myDT);
                                    break;
                            }
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(RetJson);
                            return;
                        }
                    }
                    #endregion

                    DAStoreProcedure myDA = new DAStoreProcedure();

                    DataTable myParDT = myDA.fn_CreateParameterTable();
                    foreach (var e in jsonPar)
                    {
                        var ParKey = e.Key;
                        var ParVal = e.Value.ToString();
                        myDA.fn_CreateParameterrRow(myParDT, ParKey, ParVal, "string");
                    }
                    if (PUBLIC == "0")/*非公開資料 一定要傳TOKEN*/
                    {
                        myDA.fn_CreateParameterrRow(myParDT, "TOKEN", TOKEN, "string");
                    }

                    myDT = myDA.GetDTBySP(strDbConn, SP_NAME, myParDT);

                    /*查詢DB*/

                    RetJson = DataSetToJSON("1", "執行成功" + TOKEN, myDT);
                }
                #endregion

                #region /*新增  一定要檢查 TOKEN 合法性*/
                if (TYPE == "I")
                {
                    DataTable myDT;
                    #region/*檢查 TOKEN 合法性*/
                    int retChkToken = checkToken(TOKEN, TYPE);

                    if (retChkToken < 0)
                    {
                        switch (retChkToken)
                        {
                            case -96:
                                myDT = MessageDT("-96", "登入權限不足");
                                RetJson = DataSetToJSON("-96", "登入權限不足" + TOKEN, myDT);
                                break;

                            case -97:
                                myDT = MessageDT("-97", "登入逾時");
                                RetJson = DataSetToJSON("-97", "登入逾時" + TOKEN, myDT);
                                break;
                            case -98:
                                myDT = MessageDT("-98", "查無登入資訊");
                                RetJson = DataSetToJSON("-98", "查無登入資訊" + TOKEN, myDT);
                                break;
                        }
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(RetJson);
                        return;
                    }
                    #endregion

                    DAStoreProcedure myDA = new DAStoreProcedure();
                    DataTable myParDT = myDA.fn_CreateParameterTable();
                    foreach (var e in jsonPar)
                    {
                        var ParKey = e.Key;
                        var ParVal = e.Value.ToString();
                        myDA.fn_CreateParameterrRow(myParDT, ParKey, ParVal, "string");
                    }
                    myDT = myDA.GetDTBySP(strDbConn, SP_NAME, myParDT);
                    //string retIDx = myDA.InsertBySPGetIdx .UpdateBySP_GetMessage(strDbConn, SP_NAME, myParDT);


                    /*新增Row*/
                    //myDT = MessageDT("1", "新增完成");
                    RetJson = DataSetToJSON("1", "新增完成", myDT);

                }
                #endregion

                #region /*修改  一定要檢查 TOKEN 合法性*/

                if (TYPE == "U")
                {
                    DataTable myDT;
                    #region/*檢查 TOKEN 合法性*/
                    int retChkToken = checkToken(TOKEN, TYPE);

                    if (retChkToken < 0)
                    {
                        switch (retChkToken)
                        {
                            case -96:
                                myDT = MessageDT("-96", "登入權限不足");
                                RetJson = DataSetToJSON("-96", "登入權限不足" + TOKEN, myDT);
                                break;

                            case -97:
                                myDT = MessageDT("-97", "登入逾時");
                                RetJson = DataSetToJSON("-97", "登入逾時" + TOKEN, myDT);
                                break;
                            case -98:
                                myDT = MessageDT("-98", "查無登入資訊");
                                RetJson = DataSetToJSON("-98", "查無登入資訊" + TOKEN, myDT);
                                break;
                        }
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(RetJson);
                        return;
                    }
                    #endregion

                    DAStoreProcedure myDA = new DAStoreProcedure();
                    DataTable myParDT = myDA.fn_CreateParameterTable();
                    foreach (var e in jsonPar)
                    {
                        var ParKey = e.Key;
                        var ParVal = e.Value.ToString();
                        myDA.fn_CreateParameterrRow(myParDT, ParKey, ParVal, "string");
                    }
                    int retval = myDA.UpdateBySP(strDbConn, SP_NAME, myParDT);

                    /*更新DB*/
                    myDT = MessageDT("1", "更新完成");
                    RetJson = DataSetToJSON("1", "更新完成", myDT);

                }
                #endregion

                #region /*刪  一定要檢查 TOKEN 合法性*/

                if (TYPE == "D")
                {
                    DataTable myDT;
                    #region/*檢查 TOKEN 合法性*/
                    int retChkToken = checkToken(TOKEN, TYPE);

                    if (retChkToken < 0)
                    {
                        switch (retChkToken)
                        {
                            case -96:
                                myDT = MessageDT("-96", "登入權限不足");
                                RetJson = DataSetToJSON("-96", "登入權限不足" + TOKEN, myDT);
                                break;

                            case -97:
                                myDT = MessageDT("-97", "登入逾時");
                                RetJson = DataSetToJSON("-97", "登入逾時" + TOKEN, myDT);
                                break;
                            case -98:
                                myDT = MessageDT("-98", "查無登入資訊");
                                RetJson = DataSetToJSON("-98", "查無登入資訊" + TOKEN, myDT);
                                break;
                        }
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(RetJson);
                        return;
                    }
                    #endregion

                    DAStoreProcedure myDA = new DAStoreProcedure();

                    /*更新DB*/
                    myDT = MessageDT("1", "刪除完成");
                    RetJson = DataSetToJSON("1", "刪除完成", myDT);

                }
                #endregion


                context.Response.ContentType = "text/plain";
                context.Response.Write(RetJson);
            }
            catch (Exception ex)
            {
                DataTable myDT = MessageDT("-99", "系統發生錯誤：" + ex.Message);
                string RetJson = DataSetToJSON("-99", "系統發生錯誤：" + ex.Message, myDT);
                context.Response.ContentType = "text/plain";
                context.Response.Write(RetJson);
                //context.Response.Write("Error :" + ex.Message);
            }
        }

        private int checkToken(string TOKEN, string TYPE)
        {
            int retCode = -98;/*查無登入資訊*/

            string strDbConn = ConfigurationManager.ConnectionStrings["strDbConn"].ConnectionString;
            DataTable myDT;
            DAStoreProcedure myDA = new DAStoreProcedure();
            DataTable myParDT = myDA.fn_CreateParameterTable();

            myDA.fn_CreateParameterrRow(myParDT, "TOKEN", TOKEN, "string");
            myDT = myDA.GetDTBySP(strDbConn, "USER_TOKEN_CHECK", myParDT);

            if (myDT.Rows.Count == 1)
            {
                int U_Permit = int.Parse(myDT.Rows[0]["U_Permit"].ToString());
                DateTime Delay = DateTime.Parse(myDT.Rows[0]["Delay"].ToString());

                retCode = U_Permit;
                if (DateTime.Now > Delay)
                {
                    retCode = -97;/*Token　逾時*/
                }

                if (U_Permit == 0)/*尚未設定*/
                {
                    retCode = -96;
                }
                else if (U_Permit == 2 && "I,U,D".IndexOf(TYPE) >= 0)/*只有查詢權限 但是執行更新及修改 的 FUNCTION*/
                {
                    retCode = -96;
                }

            }
            return retCode;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string DataSetToJSON(string code, string Message, DataTable dt)
        {
            /*https://www.c-sharpcorner.com/uploadfile/4d56e1/how-to-create-a-datatable-and-add-it-to-a-dataset/
            https://amychang2014.pixnet.net/blog/post/286405198 */

            DataSet dataSet = new DataSet();

            //=====訊息===========================================
            // Add a DataTable to DataSet directly
            DataTable objDt1 = dataSet.Tables.Add("Message");
            //  add some columns here
            // 建立欄位
            objDt1.Columns.Add("Message_Code", typeof(int));
            objDt1.Columns.Add("Message_Content", typeof(string));

            // 新增資料到DataTable
            DataRow row;
            row = objDt1.NewRow();
            row["Message_Code"] = code;
            row["Message_Content"] = Message;
            objDt1.Rows.Add(row);

            //=====資料=========================================== 
            // Creating the table and adding it to the DataSet
            DataTable objDt2 = new DataTable("Data");
            if (dt != null)
            {
                objDt2 = dt;
                objDt2.TableName = "Data";
            }

            //  add some columns here
            dataSet.Tables.Add(objDt2);



            dataSet.AcceptChanges();
            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            return json;
        }

        private DataTable MessageDT(string CODE, string MESSAGE)
        {
            DataTable myDT;
            myDT = new DataTable("MESSAGE");
            myDT.Columns.Add("AUTH_CODE", typeof(String));
            myDT.Columns.Add("MESSAGE", typeof(String));
            DataRow dr = myDT.NewRow();
            dr["AUTH_CODE"] = CODE; // or dr[0]="Mohammad"; 
            dr["MESSAGE"] = MESSAGE + "(" + CODE + ")"; // or dr[0]="Mohammad"; 
            myDT.Rows.Add(dr);
            myDT.TableName = "Message";
            return myDT;
        }

        public static string sha512(string inputString)
        {
            /*SHA512 sha512 = SHA512.Create();
             byte[] bytes = Encoding.UTF8.GetBytes(inputString);
             byte[] hash = sha512.ComputeHash(bytes);
             return GetStringFromHash(hash);*/
            return inputString;
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}