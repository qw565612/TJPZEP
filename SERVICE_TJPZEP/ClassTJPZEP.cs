using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace SERVICE_TJPZEP
{

    public class ClassTJPZEP
    {
        public string strDbConn = ConfigurationManager.ConnectionStrings["strDbConn"].ConnectionString;

        public int GALLERY_FILE_UPLOAD(string IDX, string GALLERY_FILE_NAME, string GALLERY_FILE_TITLE)
        {
            int retval = 0;
            DAStoreProcedure myDA_N = new DAStoreProcedure();
            DataTable myParDT = myDA_N.fn_CreateParameterTable();
            myDA_N.fn_CreateParameterrRow(myParDT, "IDX", IDX, "string");
            myDA_N.fn_CreateParameterrRow(myParDT, "GALLERY_FILE_NAME", GALLERY_FILE_NAME, "string");
            myDA_N.fn_CreateParameterrRow(myParDT, "GALLERY_FILE_TITLE", GALLERY_FILE_TITLE, "string");

            retval = myDA_N.InsertBySP(strDbConn, "GALLERY_FILE_UPLOAD", myParDT);


            return retval;
        }


        public int NEWS_FILE_UPLOAD(string IDX, string NEWS_FILE_NAME, string NEWS_FILE_TITLE)
        {
            int retval = 0;
            DAStoreProcedure myDA_N = new DAStoreProcedure();
            DataTable myParDT = myDA_N.fn_CreateParameterTable();
            myDA_N.fn_CreateParameterrRow(myParDT, "IDX", IDX, "string");
            myDA_N.fn_CreateParameterrRow(myParDT, "NEWS_FILE_NAME", NEWS_FILE_NAME, "string");
            myDA_N.fn_CreateParameterrRow(myParDT, "NEWS_FILE_TITLE", NEWS_FILE_TITLE, "string");

            retval = myDA_N.InsertBySP(strDbConn, "NEWS_FILE_UPLOAD", myParDT);


            return retval;
        }
    }


}