using System;
using System.Collections.Generic;
using System.Text;
using Taurus.Core;
using CYQ.Data;
using CYQ.Data.Table;
using CYQ.Data.Xml;
using CYQ.Data.Tool;
using System.Web;
using System.IO;
using Taurus.Logic;

namespace Taurus.Controllers
{
    #region CodeFirst?????ݱ?
    public class Connection
    {
        public const string TxtConn = "txt path={0}App_Data";
        public const string XmlConn = "xml path={0}App_Data";
    }

    public class Users : CYQ.Data.Orm.OrmBase
    {
        public Users()
        {
            base.SetInit(this, "Users", Connection.TxtConn);
        }
        private int _ID;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private bool _IsEnabled;

        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; }
        }
        private int _Sex;

        public int Sex
        {
            get { return _Sex; }
            set { _Sex = value; }
        }
        private string _HeadImgUrl;

        public string HeadImgUrl
        {
            get { return _HeadImgUrl; }
            set { _HeadImgUrl = value; }
        }
        private int _UserType;

        public int UserType
        {
            get { return _UserType; }
            set { _UserType = value; }
        }

        private DateTime _CreateTime;

        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }

    }
    public class UserType : CYQ.Data.Orm.OrmBase
    {
        public UserType()
        {
            base.SetInit(this, "UserType", Connection.XmlConn);
        }
        private int _ID;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        private string _TypeName;

        public string TypeName
        {
            get { return _TypeName; }
            set { _TypeName = value; }
        }

    }

    #endregion

    public class DemoController : Controller
    {
        //protected override void BeforeInvoke(string methodName)
        //{
        //    if(methodName=="About")
        //    {
        //        Write("?Ƿ???ʾ");
        //        CancelInvoke = true;
        //        CancelLoadHtml = true;
        //    }
        //    base.BeforeInvoke(methodName);
        //}
        #region Controller????
        public override void Default()
        {
            if (IsHttpGet)
            {
                InitData();
                MDataTable utTable = null;
                using (UserType ut = new UserType())
                {
                    utTable = ut.Select();
                }

                // View.OnForeach += new XHtmlAction.SetForeachEventHandler(View_OnForeach);
                // utTable.Bind(View);//ȡusertypeView??defaultView?ڵ㡣

                utTable.Bind(View, "ddl" + utTable.TableName);//????????????ָ???ڵ????ơ????ñ???????Ϊ?˲?д??ddlUserType??

                MDataTable dt;
                //UI ????View
                using (Users demo = new Users())
                {
                    if (demo.Fill())
                    {
                        demo.UI.SetToAll(View);

                        View.LoadData(demo, "");
                    }
                    Pager pager = new Pager(View);
                    dt = demo.Select(pager.PageIndex, pager.PageSize);
                    pager.Bind(dt.RecordsAffected);//?󶨷?ҳ?ؼ???
                }
                #region ??????
                dt.JoinOnName = "UserType";
                dt.Conn = Connection.XmlConn;//???????Ļ???һ?㣨Users????txt???ݿ⣬UserType??xml???ݿ⣩
                dt = dt.Join("UserType", "ID", "TypeName");
                #endregion
                View.OnForeach += new XHtmlAction.SetForeachEventHandler(View_OnForeach);//formater
                dt.Bind(View);//ȡUsersView??defaultView?ڵ㡣
            }
            //if (IsHttpPost)
            //{
            //    BtnEvent();
            //}

            //View ???? UI
            //View.LoadData(ut.Select<UserType>());
            //View.SetForeach();

            // View.LoadData(dt.Rows[dt.Rows.Count - 1], "txt");// ?Զ???ʽ????ǩ??

            //View.LoadData(dt);//װ??????
            //
            //View.SetForeach("divView2", SetType.InnerXml);
            //View.SetForeach("divView3", "?Զ??壺{0} -${Name} -{2}<br />");



        }
        public void Show()
        {

        }
        #region ??ť?¼?
        public void BtnAdd()
        {

            using (Users u = new Users())
            {
                string path = SavePic();
                if (path != null)
                {
                    u.HeadImgUrl = path;
                }
                if (u.Insert(true))
                {
                    Reflesh(u.ID);
                }
            }
        }
        public void BtnUpdate()
        {
            using (Users u = new Users())
            {
                //u.LoadFrom(true);
                //Users u2 = u.RawData.ToEntity<Users>();
                //string name = u.Name;
                //u.Name = "111";
                string path = SavePic();
                if (path != null)
                {
                    u.HeadImgUrl = path;
                }
                if (u.Update(null, true))
                {
                    Reflesh(u.ID);
                }
            }
        }
        public void BtnDelete()
        {

            using (Users u = new Users())
            {
                u.Delete();//id  ?id=xxx
                Reflesh(1);
            }
        }
        #endregion
        #endregion

        #region ???????̷???

        private void InitData()
        {
            if (!DBTool.Exists("UserType", "U", "xml path={0}App_Data"))
            {
                using (UserType ut = new UserType())
                {
                    ut.Delete("1=1");//Clear All Data
                    for (int i = 1; i < 5; i++)
                    {
                        ut.TypeName = "Type" + i;
                        ut.Insert(InsertOp.None);
                    }
                }
            }
        }

        private void Reflesh(int id)
        {
            Context.Response.Redirect(Context.Request.Url.LocalPath + "?id=" + id, true);
        }


        private string SavePic()
        {
            if (Context.Request.Files != null)
            {
                HttpPostedFile file = Context.Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    string path = "/Upload/" + file.FileName;
                    string folder = AppConfig.WebRootPath + "Upload/";
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    file.SaveAs(folder + file.FileName);
                    return path;
                }
            }
            return null;
        }

        //private void BtnEvent()
        //{
        //    if (IsClick("btnAdd"))
        //    {
        //        using (Users u = new Users())
        //        {
        //            string path = SavePic();
        //            if (path != null)
        //            {
        //                u.HeadImgUrl = path;
        //            }
        //            if (u.Insert(true))
        //            {
        //                Reflesh(u.ID);
        //            }
        //        }
        //    }
        //    if (IsClick("btnUpdate"))
        //    {
        //        using (Users u = new Users())
        //        {
        //            string path = SavePic();
        //            if (path != null)
        //            {
        //                u.HeadImgUrl = path;
        //            }
        //            if (u.Update(null, true))
        //            {
        //                Reflesh(u.ID);
        //            }
        //        }
        //    }
        //    else if (IsClick("btnDelete"))
        //    {
        //        using (Users u = new Users())
        //        {
        //            u.Delete();
        //            Reflesh(1);
        //        }
        //    }
        //}

        string View_OnForeach(string text, Dictionary<string, string> values, int rowIndex)
        {
            values["Sex"] = values["Sex"] == "1" ? "Boy" : "Girl";
            return text;
        }
        #endregion
    }
}
