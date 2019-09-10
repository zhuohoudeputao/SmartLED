using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Threading;
using System.Net;

namespace SmartLED
{


    [Activity(Label = "FunctionActivity")]
    public class FunctionActivity : Activity
    {
        //内部对象
        TcpClient tcpClient = new TcpClient();
        NetworkStream ns = null;

        Button NormalLightButton;
        Button LanternButton;
        Button StrongLightButton;
        Button ConnectButton;
        Button DisConnectButton;
        Button ConfigureButton;
        SeekBar LED1SeekBar;
        SeekBar LED2SeekBar;
        SeekBar LED3SeekBar;
        Spinner LEDChooserSpinner;
        TextView TemperatureHumidityTextView;

        enum LED
        {
            LED1,
            LED2,
            LED3
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_function);
            // Create your application here

            //获取控件引用
            NormalLightButton = FindViewById<Button>(Resource.Id.NormalLightButton);
            LanternButton = FindViewById<Button>(Resource.Id.LanternButton);
            StrongLightButton = FindViewById<Button>(Resource.Id.StrongLightButton);
            ConnectButton = FindViewById<Button>(Resource.Id.ConnectButton);
            DisConnectButton = FindViewById<Button>(Resource.Id.DisConnectButton);
            ConfigureButton = FindViewById<Button>(Resource.Id.ConfigureButton);
            LED1SeekBar = FindViewById<SeekBar>(Resource.Id.LED1SeekBar);
            LED2SeekBar = FindViewById<SeekBar>(Resource.Id.LED2SeekBar);
            LED3SeekBar = FindViewById<SeekBar>(Resource.Id.LED3SeekBar);
            LEDChooserSpinner = FindViewById<Spinner>(Resource.Id.LEDChooserSpinner);
            TemperatureHumidityTextView = FindViewById<TextView>(Resource.Id.TemperatureHumidityTextView);

            //事件绑定
            ConnectButton.Click += ConnectButton_Click;
            DisConnectButton.Click += DisConnectButton_Click;
            ConfigureButton.Click += ConfigureButton_Click;
            NormalLightButton.Click += NormalLightButton_Click;
            LanternButton.Click += LanternButton_Click;
            StrongLightButton.Click += StrongLightButton_Click;
            LED1SeekBar.StopTrackingTouch += LED1SeekBar_StopTrackingTouch;
            LED2SeekBar.StopTrackingTouch += LED2SeekBar_StopTrackingTouch;
            LED3SeekBar.StopTrackingTouch += LED3SeekBar_StopTrackingTouch;


        }

        private void ConfigureButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(ConnectActivity)));
            Finish();
        }

        private void LED1SeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            if (Send(LED.LED1, LED1SeekBar.Progress.ToString()) == true)
                Toast.MakeText(this, "已调整到亮度" + LED1SeekBar.Progress.ToString(), ToastLength.Long).Show();
            else
                Toast.MakeText(this, "发送失败", ToastLength.Long).Show();
        }

        private void LED2SeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            if (Send(LED.LED2, LED2SeekBar.Progress.ToString()) == true)
                Toast.MakeText(this, "已调整到亮度" + LED2SeekBar.Progress.ToString(), ToastLength.Long).Show();
            else
                Toast.MakeText(this, "发送失败", ToastLength.Long).Show();
        }

        private void LED3SeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            if (Send(LED.LED3, LED3SeekBar.Progress.ToString()) == true)
                Toast.MakeText(this, "已调整到亮度" + LED3SeekBar.Progress.ToString(), ToastLength.Long).Show();
            else
                Toast.MakeText(this, "发送失败", ToastLength.Long).Show();
        }


        private void StrongLightButton_Click(object sender, EventArgs e)
        {
            if (Send(LEDChooserSpinner.SelectedItemPosition, "9") == true)
            {
                Toast.MakeText(this, "已写入", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "无法写入", ToastLength.Long).Show();
            }
        }

        private void LanternButton_Click(object sender, EventArgs e)
        {
            if (Send(LEDChooserSpinner.SelectedItemPosition, "1") == true)
            {
                Toast.MakeText(this, "已写入", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "无法写入", ToastLength.Long).Show();
            }
        }

        private void NormalLightButton_Click(object sender, EventArgs e)
        {
            if (Send(LEDChooserSpinner.SelectedItemPosition, "5") == true)
            {
                Toast.MakeText(this, "已写入", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "无法写入", ToastLength.Long).Show();
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            /*
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
                Toast.MakeText(this, "tcpclient已连接", ToastLength.Long).Show();
                Thread T_H_thread = new Thread(new ThreadStart(Receive));
                T_H_thread.Start();
            }
            */

            TcpListener server = new TcpListener(IPAddress.Any, 8082);
            server.Start();
            while (true)
            {
                TcpClient tcpClient = server.AcceptTcpClient();
                Toast.MakeText(this, "tcpclient已连接", ToastLength.Long).Show();

                NetworkStream ns = tcpClient.GetStream();
                Thread.Sleep(1000);
                byte[] signal = Encoding.ASCII.GetBytes("5");
                ns.Write(signal, 0, signal.Length);
                Thread.Sleep(1000);
                signal = Encoding.ASCII.GetBytes("f");
                ns.Write(signal, 0, signal.Length);
                Thread.Sleep(1000);
                signal = Encoding.ASCII.GetBytes("F");
                ns.Write(signal, 0, signal.Length);
            }

        }

        private void Receive()
        {
            Byte[] data = new Byte[2];
            while (true)
            {
                if (ns.CanRead)
                {
                    int bytes = ns.Read(data, 0, data.Length);
                    string output = ((int)data[0]).ToString() + "℃," + ((int)data[1]).ToString() + "%";
                    TemperatureHumidityTextView.Text = output;
                    Thread.Sleep(1000);
                }
            }

        }

        private void DisConnectButton_Click(object sender, EventArgs e)
        {
            ns.Close();
            tcpClient.Close();
        }

        private bool SendByte(string data)
        {
            try
            {
                if (ns.CanWrite)
                {
                    byte[] signal = Encoding.ASCII.GetBytes(data);
                    ns.Write(signal, 0, signal.Length);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;

            }
        }

        private bool Send(LED LEDIndex, string data)
        {
            try
            {
                string signal = data;
                if (LEDIndex == LED.LED1)
                    signal = data;
                else if (LEDIndex == LED.LED2)
                    signal = ((char)(data.First() + 17)).ToString();
                else
                    signal = ((char)(data.First() + 49)).ToString();

                SendByte(signal);

                return true;
            }
            catch (Exception e)
            {
                return false;

            }
        }

        private bool Send(int LEDIndex, string data)
        {
            try
            {
                string signal = data;
                if (LEDIndex == 0)
                    signal = data;
                else if (LEDIndex == 1)
                    signal = ((char)(data.First() + 17)).ToString();
                else
                    signal = ((char)(data.First() + 49)).ToString();

                SendByte(signal);

                return true;
            }
            catch (Exception e)
            {
                return false;

            }
        }
    }
}