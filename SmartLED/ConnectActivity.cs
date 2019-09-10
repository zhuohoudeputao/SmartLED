using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Resources;
using System.Xml;
using SQLite;
using System.IO;

namespace SmartLED
{
    [Activity(Label = "ConnectActivity")]
    public class ConnectActivity : Activity
    {
        EditText ModuleWifiNameEditText;
        EditText ModuleWifiPasswordEditText;
        EditText RouterWifiNameEditText;
        EditText RouterWifiPasswordEditText;
        Button SaveButton;
        Button CancelButton;

        private SQLiteConnection sqliteConn;
        private const string TableName = "wifiInfo";//表名首字母小写
        private string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "UserInfo.db");//数据库和类名都大写

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_connect);
            // Create your application here

            ModuleWifiNameEditText = FindViewById<EditText>(Resource.Id.ModuleWifiNameEditText);
            ModuleWifiPasswordEditText = FindViewById<EditText>(Resource.Id.ModuleWifiPasswordEditText);
            RouterWifiNameEditText = FindViewById<EditText>(Resource.Id.RouterWifiNameEditText);
            RouterWifiPasswordEditText = FindViewById<EditText>(Resource.Id.RouterWifiPasswordEditText);
            SaveButton = FindViewById<Button>(Resource.Id.SaveButton);
            CancelButton = FindViewById<Button>(Resource.Id.CancelButton);

            //初始化数据库连接与表连接
            sqliteConn = new SQLiteConnection(dbPath);

            var userInfoTable = sqliteConn.GetTableInfo(TableName);
            if (userInfoTable.Count == 0)
            {
                sqliteConn.CreateTable<WifiInfo>();
            }

            //读出数据
            var wifiInfos = sqliteConn.Table<WifiInfo>();
            var moduleWifiInfo = wifiInfos.Where(p => p.Key == "ModuleWifi").FirstOrDefault();
            if (moduleWifiInfo != null)
            {
                ModuleWifiNameEditText.Text = moduleWifiInfo.Name;
                ModuleWifiPasswordEditText.Text = moduleWifiInfo.Value;
            }
            var routerWifiInfo = wifiInfos.Where(p => p.Key == "RouterWifi").FirstOrDefault();
            if (routerWifiInfo != null)
            {
                RouterWifiNameEditText.Text = routerWifiInfo.Name;
                RouterWifiPasswordEditText.Text = routerWifiInfo.Value;
            }

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            var wifiInfos = sqliteConn.Table<WifiInfo>();
            var wifiInfo = wifiInfos.Where(p => p.Key == "ModuleWifi").FirstOrDefault();
            if (wifiInfo == null)
            {
                WifiInfo model = new WifiInfo() { Key = "ModuleWifi", Name = ModuleWifiNameEditText.Text, Value = ModuleWifiPasswordEditText.Text};
                sqliteConn.Insert(model);
            }
            else
            {
                sqliteConn.Update(new WifiInfo() { Key = "ModuleWifi", Name = ModuleWifiNameEditText.Text, Value = ModuleWifiPasswordEditText.Text });
            }
            wifiInfo = wifiInfos.Where(p => p.Key == "RouterWifi").FirstOrDefault();
            if (wifiInfo == null)
            {
                WifiInfo model = new WifiInfo() { Key = "RouterWifi", Name = RouterWifiNameEditText.Text, Value = RouterWifiPasswordEditText.Text };
                sqliteConn.Insert(model);
            }
            else
            {
                sqliteConn.Update(new WifiInfo() { Key = "RouterWifi", Name = RouterWifiNameEditText.Text, Value = RouterWifiPasswordEditText.Text });
            }
            Toast.MakeText(this, "保存成功", ToastLength.Long).Show();
            StartActivity(new Android.Content.Intent(this, typeof(FunctionActivity)));
            Finish();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(FunctionActivity)));
            Finish();
        }

        

        /*
        private void WifiConnectButton_Click(object sender, EventArgs e)
        {
            while (tcpClient.Connected == false)
            {
                try
                {
                    tcpClient.Connect(System.Net.IPAddress.Parse("192.168.4.1"), 8080);
                }
                catch (SocketException exp)
                {
                    Toast.MakeText(this, exp.ToString(), ToastLength.Long).Show();
                }
            }
            ns = tcpClient.GetStream();
            if (tcpClient.Connected == true && ns.CanWrite)
            {
                byte[] signal = Encoding.ASCII.GetBytes("ssid" + WifiNameEditText.Text);
                ns.Write(signal, 0, signal.Length);
                Thread.Sleep(2000);
                signal = Encoding.ASCII.GetBytes("pswd" + WifiPasswordEditText.Text);
                ns.Write(signal, 0, signal.Length);
                Thread.Sleep(2000);
                signal = Encoding.ASCII.GetBytes("send");
                ns.Write(signal, 0, signal.Length);

                StartActivity(new Android.Content.Intent(this, typeof(FunctionActivity)));
            }

        }
        */
    }
}