using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using SQLite;
using System.IO;

namespace SmartLED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme",MainLauncher =true)]
    public class MainActivity : AppCompatActivity
    {
        //通用变量
        private SQLiteConnection sqliteConn;
        private const string TableName = "userInfo";//表名首字母小写
        private string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "UserInfo.db");//数据库和类名都大写

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            //获取控件引用
            EditText NameEditText = FindViewById<EditText>(Resource.Id.NameEditText);
            EditText PasswordEditText = FindViewById<EditText>(Resource.Id.PasswordEditText);
            CheckBox RememberCheckBox = FindViewById<CheckBox>(Resource.Id.RememberCheckBox);
            Button SigninButton = FindViewById<Button>(Resource.Id.SigninButton);
            Button RegisterButton = FindViewById<Button>(Resource.Id.RegisterButton);

            //初始化数据库连接与表连接
            sqliteConn = new SQLiteConnection(dbPath);

            var userInfoTable = sqliteConn.GetTableInfo(TableName);
            if (userInfoTable.Count == 0)
            {
                sqliteConn.CreateTable<UserInfo>();
            }

            //读出上次登录数据
            var userInfos = sqliteConn.Table<UserInfo>();
            var lastestUserInfo = userInfos.Where(p=>p.Lastest == 1).FirstOrDefault();
            if (lastestUserInfo != null)
            {
                NameEditText.Text = lastestUserInfo.Name;
                if (lastestUserInfo.Remember == 1)
                {
                    PasswordEditText.Text = lastestUserInfo.Pwd;
                    RememberCheckBox.Checked = true;
                }
            }
            //登录按钮点击事件绑定
            SigninButton.Click += (sender, e) =>
            {
                string name = NameEditText.Text;
                string pwd = PasswordEditText.Text;
                int remember = RememberCheckBox.Checked == true ? 1 : 0;
                var userInfo = userInfos.Where(p => p.Pwd == pwd && p.Name == name).FirstOrDefault();
                if (userInfo == null)
                {
                    Toast.MakeText(this, "用户名或密码不正确", ToastLength.Long).Show();
                    return;
                }
                else
                {
                    sqliteConn.Execute("Update userInfo Set Lastest = 0");
                    sqliteConn.Update(new UserInfo() { Name = name, Pwd = pwd, Remember = remember, Lastest = 1 });
                    Toast.MakeText(this, "登陆成功", ToastLength.Long).Show();
                }

                StartActivity(new Android.Content.Intent(this, typeof(FunctionActivity)));
                Finish();
            };

            //注册按钮点击事件
            RegisterButton.Click += (sender, e) =>
            {
                StartActivity(new Android.Content.Intent(this, typeof(RegisterActivity)));
            };

            
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}