using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SERVICE_TJPZEP
{
    /// <summary>
    /// fileUpload 的摘要描述
    /// </summary>
    public class fileUpload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string para1 = context.Request.Params.Get("key") ?? "";
            string idx = context.Request.Params.Get("idx") ?? "";
            string title = context.Request.Params.Get("title") ?? "";

            if (context.Request.Files.Count > 0)
            {
                HttpFileCollection files = context.Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];
                    string fname = context.Server.MapPath("./uploads/" + file.FileName);

                    if (para1 == "gallery")
                    {
                        fname = context.Server.MapPath("./photo/" + file.FileName);
                        ClassTJPZEP myClass = new ClassTJPZEP();
                        int retValue = myClass.GALLERY_FILE_UPLOAD(idx, file.FileName, title);
                    }
                    else if (para1 == "news") // 如果 key 為 "news"，存到 ./file/ 資料夾
                    {
                        fname = context.Server.MapPath("./file/" + file.FileName);
                        ClassTJPZEP myClass = new ClassTJPZEP();
                        int retValue = myClass.NEWS_FILE_UPLOAD(idx, file.FileName, title);
                    }

                    file.SaveAs(fname);
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("上傳成功!!");
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
