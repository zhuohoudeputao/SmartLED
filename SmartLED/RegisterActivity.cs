using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace SmartLED
{
    [Activity(Label = "注册",Theme ="@style/AppTheme")]
    public class RegisterActivity : Activity
    {
        //通用变量
        private SQLiteConnection sqliteConn;
        private const string TableName = "userInfo";//表名首字母小写
        private string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "UserInfo.db");//数据库和类名都大写

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_register);
            // Create your application here

            //获取控件引用
            EditText NameEditText = FindViewById<EditText>(Resource.Id.NameEditText);
            EditText PasswordEditText = FindViewById<EditText>(Resource.Id.PasswordEditText);
            EditText PasswordEditText2 = FindViewById<EditText>(Resource.Id.PasswordEditText2);
            Button RegisterButton = FindViewById<Button>(Resource.Id.RegisterButton);

            //初始化数据库连接与表连接
            sqliteConn = new SQLiteConnection(dbPath);

            var userInfoTable = sqliteConn.GetTableInfo(TableName);
            if (userInfoTable.Count == 0)
            {
                sqliteConn.CreateTable<UserInfo>();
            }

            //注册按钮点击事件
            RegisterButton.Click += (sender, e) =>
            {
                string name = NameEditText.Text;
                string pwd = PasswordEditText.Text;
                string pwd2 = PasswordEditText2.Text;
                if (name == "" || pwd == "" || pwd2 == "")
                {
                    Toast.MakeText(this, "请补全信息", ToastLength.Long).Show();
                    return;
                }
                var userInfos = sqliteConn.Table<UserInfo>();
                var userInfo = userInfos.Where(p => p.Name == name).FirstOrDefault();
                if (userInfo != null)
                {
                    Toast.MakeText(this, "该账户名已被注册，请更换账户名", ToastLength.Long).Show();
                    return;
                }
                if (pwd != pwd2)
                {
                    Toast.MakeText(this, "请输入相同的密码以确认", ToastLength.Long).Show();
                    return;
                }
                UserInfo model = new UserInfo() { Name = name, Pwd = pwd, Remember = 0, Lastest = 1 };
                sqliteConn.Insert(model);
                Toast.MakeText(this, "注册成功", ToastLength.Long).Show();
                StartActivity(new Android.Content.Intent(this, typeof(MainActivity)));
                Finish();
            };
        }
    }
}