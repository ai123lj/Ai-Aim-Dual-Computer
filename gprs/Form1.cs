using Compunet.YoloSharp;
using Compunet.YoloSharp.Data;
using Microsoft.Win32;
using MWModle;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace gprs
{
    public partial class Form1 : Form
    {
        #region 变量定义
        //////////////////////////////// 常量定义 ////////////////////////////////
        private const int CaptureWidth = 640;
        private const int CaptureHeight = 640;

        //////////////////////////////// 枚举定义 ////////////////////////////////
        private enum FireMode { Sniper, Burst, Auto }//
        private enum TargetPosition { Head, Chest, Nose }
        private enum MouseState { PressedUnprocessed = 1, PressedProcessed = 2, ReleasedUnprocessed = 3, ReleasedProcessed = 4 }
        //1:按下，未处理  //2：按下，已处理  //3：抬起未处理  //4：抬起已处理
        //////////////////////////////// 游戏控制相关字段 ////////////////////////////////
        private MouseState _mouseStateR = MouseState.ReleasedProcessed;//鼠标状态
        private int _frameCount;//帧率计数
        private int _MouseCount;//帧率计数

        int fireID;
        int fireZhu = 2;
        int fireFu = 1;

        int xSensitivity;
        int ySensitivity;

        //////////////////////////////// 图像处理相关字段 ////////////////////////////////
        private Bitmap _captureBitmap = new(CaptureWidth, CaptureHeight, PixelFormat.Format24bppRgb);//存储的截取图像
        ////////////////////////////////////////////////////////
        private BlockingCollection<long> _imageQueue = new(1);
        //////////////////////////////// 串口通信相关字段 ////////////////////////////////
        private readonly SerialPort _serialPort = new()
        {
            BaudRate = 2000000,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One
        };
        private AutoResetEvent _MouseEvent = new(false);
        //////////////////////////////// MWCapture采集相关字段 ////////////////////////////////
        private MWCaptureWrapperPro _mwCaptureWrapperPro = new();  // 添加这一行
        private ManualResetEventSlim _consumerWaitingEvent = new(false);
        #endregion

        public Form1()
        {
            InitializeComponent();
            //SetupEventHandlers();
        }
        Stopwatch RunTime = new Stopwatch();
        private void Form1_Load(object sender, EventArgs e)
        {
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            radioButton1.Checked = true;

            //美乐威采集
            InitializeMWCaptureProAndStart();
            
            //网络采集
            Task.Run(() =>
            {
                return;
                const int PixelDataSize = CaptureWidth * CaptureHeight * 3; // 24bpp = 3 bytes per pixel

                TcpListener listener = new TcpListener(IPAddress.Any, 12345);
                listener.Start();

                TcpClient client = null;
                NetworkStream stream = null;
                byte[] pixelData = new byte[PixelDataSize]; // 固定缓冲区

                while (true)
                {
                    try
                    {
                        if (client == null)
                        {
                            client = listener.AcceptTcpClient();
                            stream = client.GetStream();
                            Console.WriteLine("客户端已连接");
                        }

                        // 直接读取固定大小的像素数据（无长度前缀）
                        int totalRead = 0;
                        while (totalRead < PixelDataSize)
                        {
                            int bytesRead = stream.Read(pixelData, totalRead, PixelDataSize - totalRead);
                            if (bytesRead == 0)
                                throw new Exception("连接中断");
                            totalRead += bytesRead;
                        }

                        if (!_consumerWaitingEvent.Wait(0))
                            continue; // 如果消费者不在等待，直接返回，丢弃此帧

                        // 从原始数据重建 Bitmap
                        //Bitmap receivedBitmap = new Bitmap(CaptureWidth, CaptureHeight, PixelFormat.Format24bppRgb);
                        BitmapData bmpData = _captureBitmap.LockBits(
                            new System.Drawing.Rectangle(0, 0, CaptureWidth, CaptureHeight),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format24bppRgb
                        );
                        Marshal.Copy(pixelData, 0, bmpData.Scan0, PixelDataSize);
                        _captureBitmap.UnlockBits(bmpData);

                        // 移除旧帧(如果有)，确保只处理最新帧
                        _imageQueue.TryTake(out var oldImage);
                        // 添加新帧(非阻塞，如果队列满会丢弃旧帧)
                        _imageQueue.TryAdd(0);
                        //_captureBitmap.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"接收错误: {ex.Message}");
                        try { stream?.Dispose(); } catch { }
                        try { client?.Dispose(); } catch { }
                        client = null;
                        stream = null;
                        Thread.Sleep(1000);
                    }
                }
                listener.Stop();
            });

            //本机采集
            Task.Run(() =>
            {
                return;
                Bitmap _captureBitmap1 = new(CaptureWidth, CaptureHeight, PixelFormat.Format24bppRgb);//存储的截取图像
                Graphics _captureGraphics = Graphics.FromImage(_captureBitmap1);
                System.Drawing.Rectangle region = new(Screen.PrimaryScreen.Bounds.Width / 2 - CaptureWidth / 2, Screen.PrimaryScreen.Bounds.Height / 2 - CaptureHeight / 2, CaptureWidth, CaptureHeight);//从屏幕截取的区域
                while (true)
                {
                    try
                    {
                        _captureGraphics.CopyFromScreen(region.X, region.Y, 0, 0, region.Size);
                    }
                    catch (Exception)
                    { }

                    if (!_consumerWaitingEvent.Wait(0))
                        continue; // 如果消费者不在等待，直接返回，丢弃此帧

                    BitmapClone(_captureBitmap1, ref _captureBitmap);

                    // 移除旧帧(如果有)，确保只处理最新帧
                    _imageQueue.TryTake(out var oldImage);
                    // 添加新帧(非阻塞，如果队列满会丢弃旧帧)
                    _imageQueue.TryAdd(0);

                }
            });

            //测试鼠标执行速度
            Task.Run(() =>
            {
                return;
                while (true)
                {
                    Thread.Sleep(1000);
                    if (_serialPort.IsOpen == false)
                    {
                        continue;
                    }
                    RunTime.Restart();
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);
                    SendMouseCMD(0, 0, 0);

                    long time = RunTime.ElapsedMilliseconds;
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox1.Text = time + "ms";
                    });
                }
            });


            //反应测试
            Task.Run(() =>
            {
                return;
                System.Drawing.Color pix = System.Drawing.Color.FromArgb(0, 0, 0);
                while (true)
                {
                    _consumerWaitingEvent.Set(); // 表示开始等待
                    var imageQueue = _imageQueue.Take();// 阻塞等待新图像(不占用CPU)
                    _consumerWaitingEvent.Reset(); // 表示结束等待

                    _frameCount++;
                    //流程待写
                    if (_serialPort.IsOpen == false)
                    {
                        continue;
                    }
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        pix = _captureBitmap.GetPixel(_captureBitmap.Width / 2, _captureBitmap.Height / 2);
                        continue;
                    }
                    if (pix.R == 0 && pix.G == 0 && pix.B == 0)
                        continue;
                    var pix2 = _captureBitmap.GetPixel(_captureBitmap.Width / 2, _captureBitmap.Height / 2);
                    if (Math.Abs(pix.R - pix2.R) > 4 ||
                        Math.Abs(pix.G - pix2.G) > 4 ||
                        Math.Abs(pix.B - pix2.B) > 4)
                    {
                        SendMouseCMD(1, 0, 0);
                        pix = System.Drawing.Color.FromArgb(0, 0, 0);
                        Thread.Sleep(10);
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        if (pictureBox1.Image != null)
                        {
                            pictureBox1.Image.Dispose();
                            pictureBox1.Image = null;
                        }
                        pictureBox1.Image = (Bitmap)_captureBitmap.Clone();
                    });
                }
            });

            //yolo瞄准
            Task.Run(() =>
            {
                try
                {
                    //return;
                    YoloPredictor _predictor = new("./yolov8m-pose.onnx");
                    System.Drawing.Rectangle Rectangle = new(CaptureWidth / 2 + 100, CaptureHeight / 2 + 90, 250, 250);////截取图像需要屏蔽的区域
                    System.Drawing.SolidBrush _maskBrush = new(System.Drawing.Color.Black);//屏蔽区域颜色
                    Graphics _captureGraphics = Graphics.FromImage(_captureBitmap);
                    while (true)
                    {
                        _consumerWaitingEvent.Set(); // 表示开始等待
                        var imageQueue = _imageQueue.Take();// 阻塞等待新图像(不占用CPU)
                        _consumerWaitingEvent.Reset(); // 表示结束等待

                        // 屏蔽枪械区域，防止干扰
                        _captureGraphics.FillRectangle(_maskBrush, Rectangle);

                        switch (fireID)
                        {
                            case 1:
                                switch (fireZhu)
                                {
                                    case 1: Yolo(_predictor, 1, 2); break;
                                    case 2: Yolo(_predictor, 1, 1); break;
                                    case 3: Yolo(_predictor, 1, 3); break;
                                    case 4: Yolo(_predictor, 2, 1); break;
                                    case 5: Yolo(_predictor, 3, 0); break;
                                    case 6: Yolo(_predictor, 3, 1); break;
                                }
                                break;
                            case 2:
                                switch (fireFu)
                                {
                                    case 1: Yolo(_predictor, 2, 0); break;
                                    case 2: Yolo(_predictor, 2, 1); break;
                                }
                                break;
                            default: break;
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (pictureBox1.Image != null)
                            {
                                pictureBox1.Image.Dispose();
                                pictureBox1.Image = null;
                            }
                            pictureBox1.Image = (Bitmap)_captureBitmap.Clone();
                        });
                        _frameCount++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    if (radioButton1.Checked)//穿越火线--
                    {
                        xSensitivity = 166;// 108;// 153;//115;//2560P 165  1080P 124//1080p 83
                        ySensitivity = 166;// 108;// 153;//115;//2560P 222  1080P 157//1080p 102
                    }
                    else if (radioButton2.Checked)
                    {
                        xSensitivity = 215;//2554p 160;//1080p 215
                        ySensitivity = 215;//2554p 158;//1080p 207
                    }
                    else if (radioButton3.Checked)
                    {
                        xSensitivity = 120;//240;
                        ySensitivity = 125;// 227;
                    }
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        textBox2.Text = _frameCount + " " + _MouseCount;
                        _frameCount = 0;
                        _MouseCount = 0;
                    });

                }
            });
        }

        private void InitializeMWCaptureProAndStart()
        {
            MWCaptureWrapperPro.Init();
            MWCaptureWrapperPro.RefreshDevices();
            _mwCaptureWrapperPro.set_mw_fourcc(MWFOURCC.MWFOURCC_BGR24);
            _mwCaptureWrapperPro.set_resolution(CaptureWidth, CaptureHeight);
            // 检查是否有可用设备
            int deviceCount = MWCaptureWrapperPro.GetChannelCount();
            if (deviceCount == 0)
            {
                System.Diagnostics.Debug.WriteLine("未发现MWCapture Pro设备");
                return;
            }

            // 记录可用设备信息
            for (int i = 0; i < deviceCount; i++)
            {
                LibMWCapture.MWCAP_CHANNEL_INFO channelInfo = new LibMWCapture.MWCAP_CHANNEL_INFO();
                MWCaptureWrapperPro.GetChannelInfobyIndex(i, ref channelInfo);
                string deviceInfo = $"{channelInfo.byBoardIndex}:{channelInfo.byChannelIndex} {channelInfo.szProductName}";
                System.Diagnostics.Debug.WriteLine($"MWCapture Pro设备 {i}: {deviceInfo}");
            }

            // 设置帧回调函数
            _mwCaptureWrapperPro.SetFrameCallback(OnFrameCaptured);

            // 设置并启动第一个设备
            if (_mwCaptureWrapperPro.set_device(0))
            {
                // 启动视频采集
                if (_mwCaptureWrapperPro.start_capture(true, false)) // 只启动视频采集，不启动音频        
                    System.Diagnostics.Debug.WriteLine($"MWCapture Pro初始化成功，已启动设备 0");
                else
                    System.Diagnostics.Debug.WriteLine("启动MWCapture Pro设备失败");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("设置MWCapture Pro设备失败");
            }
        }

        private void OnFrameCaptured(CRingBuffer.st_frame_t frame, int m_width, int m_height)
        {
            // 在这里处理捕获到的图像帧
            // 注意：此方法在视频采集线程中调用，如需更新UI，请使用BeginInvoke或Invoke
            if (!_consumerWaitingEvent.Wait(0))
                return; // 如果消费者不在等待，直接返回，丢弃此帧

            _mwCaptureWrapperPro.ConvertFrameToBitmapRGB24(frame, ref _captureBitmap);

            // 移除旧帧(如果有)，确保只处理最新帧
            _imageQueue.TryTake(out var oldImage);
            // 添加新帧(非阻塞，如果队列满会丢弃旧帧)
            _imageQueue.TryAdd(0);
        }

        public void Yolo(YoloPredictor _predictor, int fireMode, int location)
        {
            BitmapReadGameInfo(out int SnipeEN);

            #region 识别条件
            if (_serialPort.IsOpen == false)
            {
                return;
            }
            if (SnipeEN == 0)
                return;
            #endregion

            #region 识别与绘制
            //开始识别

            YoloResult<Pose> result = ProcessYoloDetection(_captureBitmap, _predictor);

            //把识别结果画到图像上，并寻找离准心最近的敌人
            int id = -1, min = 1000;
            for (int i = 0; i < result.Count; i++)// foreach (var item in result)
            {
                if (result[i].Confidence < 0.5)//置信度低则跳过
                    continue;

                if ((result[i][3].Point.Y + result[i][4].Point.Y) > (result[i][11].Point.Y + result[i][12].Point.Y))
                    continue;
                if ((result[i][11].Point.Y + result[i][12].Point.Y) > (result[i][15].Point.Y + result[i][16].Point.Y))
                    continue;

                int x = Math.Abs((result[i][5].Point.X + result[i][6].Point.X) / 2 - CaptureWidth / 2);
                if (x < min)//选一个离中心最近的
                {
                    min = x;
                    id = i;
                }
            }

            #endregion

            #region 结果处理

            //未识别到人，直接开枪
            if (id < 0)
                return;

            // 根据识别坐标，进行瞄准杀敌操作
            int targetX = 0;
            int targetY = 0;

            switch (location) // 根据指定射击位置，选择相应参数
            {
                case 0: // 额头
                    targetX = (result[id][3].Point.X + result[id][4].Point.X) / 2;
                    targetY = (result[id].Bounds.Y + (result[id][3].Point.Y + result[id][4].Point.Y) / 2) / 2;
                    break;
                case 1: // 胸
                    targetX = (result[id][5].Point.X + result[id][6].Point.X) / 2;
                    targetY = (result[id][6].Point.Y * 2 + result[id][11].Point.Y) / 3;
                    break;
                case 2: // 鼻子
                    targetX = (result[id][3].Point.X + result[id][4].Point.X) / 2;
                    targetY = (result[id][3].Point.Y + result[id][4].Point.Y) / 2;
                    break;
                case 3: // 胸部靠上
                    targetX = (result[id][5].Point.X + result[id][6].Point.X) / 2;
                    targetY = (result[id][5].Point.Y + result[id][6].Point.Y) / 2; ;// ((result[id][3].Point.Y + result[id][4].Point.Y )/2 + (result[id][5].Point.Y + result[id][6].Point.Y) / 2 *2) / 3;
                    break;
            }

            // 确保目标坐标在有效范围内，防止打墙上
            targetX = Math.Clamp(targetX, result[id].Bounds.X + 5, result[id].Bounds.X + result[id].Bounds.Width - 5);
            targetY = Math.Clamp(targetY, result[id].Bounds.Y + 5, result[id].Bounds.Y + result[id].Bounds.Height - 5);


            // 转换为鼠标的移动值
            int mouseXMove = (targetX - CaptureWidth / 2) * xSensitivity / 100;
            int mouseYMove = (targetY - CaptureHeight / 2) * ySensitivity / 100;

            // 根据射击模式发送指令
            if (fireMode == 1) // 狙击模式，打完自动切手枪
            {
                //Delay(10);
                //SendMouseCMD(0, mouseXMove, mouseYMove);// 鼠标模式1，移动鼠标、开枪
                //Delay(5);
                //SendMouseCMD(4, 0, 0);// 鼠标模式1，移动鼠标、开枪
                //Thread.Sleep(300);                              // 

                SendMouseCMD(4, mouseXMove, mouseYMove);// 鼠标模式4，移动鼠标、开枪、切枪到副武器
                Thread.Sleep(200);

                //if (mouseXMove > 0)
                //{
                //    SendMouseCMD(0, mouseXMove, mouseYMove);// 鼠标模式1，移动鼠标、开枪
                //    Thread.Sleep(2000);                                                // 
                //}
            }
            else if (fireMode == 2) // 点射模式
            {
                //SendMouseCMD(0, mouseXMove, mouseYMove);// 鼠标模式1，移动鼠标、开枪
                //Thread.Sleep(new Random().Next(140, 150));

            }
            else
            {
                //int absX = Math.Abs(mouseXMove);
                //int absY = Math.Abs(mouseYMove);
                //if (absX > 10 || absY > 10){
                //    mouseXMove = Math.Min(absX, 50);
                //    mouseYMove = Math.Min(absY, 50);
                //    if (absX > absY)
                //        mouseYMove = mouseYMove * absX / mouseXMove;
                //    SendMouseCMD(0, mouseXMove * Math.Sign(mouseXMove), mouseYMove * Math.Sign(mouseYMove));// 鼠标模式1，移动鼠标、开枪
                //    Thread.Sleep(new Random().Next(100, 100));                                                // 
                //}
                if (Math.Abs(mouseXMove) > 5 || Math.Abs(mouseYMove) > 0)
                {
                    SendMouseCMD(0, mouseXMove, mouseYMove);// 鼠标模式1，移动鼠标、开枪
                    Thread.Sleep(new Random().Next(40, 40));                                                // 
                }
            }

            #endregion
        }

        #region 键盘回调
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // 处理键盘按下事件
                if (wParam == (IntPtr)WM_KEYDOWN)
                {

                    switch ((Keys)vkCode)
                    {
                        case Keys.D1:
                            // 执行你的操作（注意跨线程调用）
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                fireID = 1;
                                switch (fireZhu)
                                {
                                    case 1: radioButton11.Checked = true; break;
                                    case 2: radioButton5.Checked = true; break;
                                    case 3: radioButton4.Checked = true; break;
                                    case 4: radioButton7.Checked = true; break;
                                    case 5: radioButton8.Checked = true; break;
                                    case 6: radioButton12.Checked = true; break;
                                }
                            });
                            break;
                        case Keys.D2:

                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                fireID = 2;
                                switch (fireFu)
                                {
                                    case 1: radioButton9.Checked = true; break;
                                    case 2: radioButton6.Checked = true; break;
                                }
                            });
                            break;
                        case Keys.D3:
                        case Keys.D4:

                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                fireID = 3;
                                radioButton11.Checked = false;
                                radioButton5.Checked = false;
                                radioButton4.Checked = false;
                                radioButton7.Checked = false;
                                radioButton8.Checked = false;
                                radioButton12.Checked = false;
                                radioButton9.Checked = false;
                                radioButton6.Checked = false;
                            });
                            break;
                        default: break;
                    }
                    // 阻止按键继续传递（返回非零值）
                    //return (IntPtr)1;
                }
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }
        #endregion

        #region 鼠标回调
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                // 解析鼠标结构体
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                //this.Invoke((MethodInvoker)delegate
                {
                    switch ((int)wParam)
                    {
                        case WM_LBUTTONDOWN:
                            //textBox1.Text = $"[鼠标左键按下] 坐标: ({hookStruct.pt.X}, {hookStruct.pt.Y})";
                            break;
                        case WM_LBUTTONUP:
                            break;
                        case WM_RBUTTONDOWN:
                            _mouseStateR = MouseState.PressedUnprocessed;
                            break;
                        case WM_RBUTTONUP:
                            _mouseStateR = MouseState.ReleasedUnprocessed;
                            break;
                        case WM_MOUSEMOVE:
                            // 根据需要启用移动事件（会产生大量消息）
                            // txtLog.AppendText($"[鼠标移动] 坐标: ({hookStruct.pt.X}, {hookStruct.pt.Y})\r\n");
                            break;
                        case WM_MOUSEWHEEL:
                            //int delta = (hookStruct.mouseData >> 16);
                            //textBox1.Text = $"[鼠标滚轮] 方向: {(delta > 0 ? "向上" : "向下")}";
                            break;
                    }
                }
                //);
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }
        #endregion


        private void BitmapReadGameInfo(out int SnipeEN)
        {
            // 锁定位图区域
            BitmapData bmpData = _captureBitmap.LockBits(
            new System.Drawing.Rectangle(0, 0, CaptureWidth, CaptureHeight),
            ImageLockMode.ReadOnly,
            _captureBitmap.PixelFormat);
            SnipeEN = 0;
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)bmpData.Scan0;
                    int bytesPerPixel = 3; // 24bpp = 3字节/像素
                    int stride = bmpData.Stride;
                    int centerX = bmpData.Width / 2;
                    int centerY = bmpData.Height / 2;

                    // 存储最红的点信息
                    int maxRed = 0;
                    int reddestX = 0;
                    int reddestY = 0;

                    // 检查3x3区域
                    for (int yOffset = 0; yOffset <= 2; yOffset++)
                    {
                        int y = centerY + yOffset;
                        if (y < 0 || y >= bmpData.Height) continue;

                        byte* currentLine = ptr + (y * stride);

                        for (int xOffset = -0; xOffset <= 2; xOffset++)
                        {
                            int x = centerX + xOffset;
                            if (x < 0 || x >= bmpData.Width) continue;

                            int pixelPos = x * bytesPerPixel;
                            byte blue = currentLine[pixelPos];
                            byte green = currentLine[pixelPos + 1];
                            byte red = currentLine[pixelPos + 2];

                            // 计算红色程度（可以调整这个公式）
                            int redness = red - (green + blue) / 2;

                            // 更新最红点信息
                            if (redness > maxRed)
                            {
                                maxRed = redness;
                                reddestX = x;
                                reddestY = y;
                            }
                        }
                    }


                    //reddestX

                    // 如果需要设置SnipeEN标志
                    if (maxRed > 250) // 可以根据需要调整阈值
                    {
                        SnipeEN = 1;
                    }

                    ////cf HD使用，检测最红点右边两个像素的颜色，以区分是否停稳
                    //byte* currentLine2 = ptr + (centerY * stride);
                    //int pixelPos2 = (centerX + 2) * bytesPerPixel;
                    //byte blue2 = currentLine2[pixelPos2];
                    //byte green2 = currentLine2[pixelPos2 + 1];
                    //byte red2 = currentLine2[pixelPos2 + 2];

                    //if (red2 > 0 && green2 > 0 && blue2 > 0)
                    //    SnipeEN = 0;

                    ////输出最红点坐标
                    //this.Invoke((MethodInvoker)delegate
                    //{
                    //    textBox1.Text = ($"最红点: ({reddestX}, {reddestY}), 红色值: {maxRed}");//,{red2},{green2},{blue2}");
                    //});
                }
            }
            finally
            {
                _captureBitmap.UnlockBits(bmpData);
            }
        }

        private void SendMouseCMD(int mode, int mouseXMove, int mouseYMove)
        {
            _serialPort.Write($"{{{mode},{mouseXMove},{mouseYMove}}}"); // 鼠标模式1，移动鼠标、开枪
            _MouseEvent.WaitOne();
            _MouseCount++;
        }

        private void SetupEventHandlers()
        {
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            //open_btn.Click += OpenPort_Click;
        }

        private YoloResult<Pose> ProcessYoloDetection(Bitmap frame, YoloPredictor _predictor)
        {
            var bmpData = frame.LockBits(new System.Drawing.Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                unsafe
                {
                    // 直接共享内存，零拷贝转换
                    using var image = SixLabors.ImageSharp.Image.WrapMemory<Rgb24>(
                        Configuration.Default,
                        (byte*)bmpData.Scan0,
                        bmpData.Height * bmpData.Stride,
                        bmpData.Width,
                        bmpData.Height);

                    return _predictor.Pose(image); // Clone如果需要独立内存
                }
            }
            finally
            {
                frame.UnlockBits(bmpData);
            }
        }

        public void Delay(int delayMilliseconds)
        {
            // 创建一个Stopwatch实例来测量时间
            Stopwatch stopwatch = new Stopwatch();

            // 开始Stopwatch
            stopwatch.Start();

            // 当Stopwatch记录的时间小于设定的延迟时间时，持续循环
            while (stopwatch.ElapsedMilliseconds < delayMilliseconds)
            {
                // 通过Thread.Sleep(0)释放当前线程的时间片，防止占用CPU
                Thread.Sleep(0);
            }
            // 停止Stopwatch
            stopwatch.Stop();
        }

        private unsafe Bitmap BitmapClone(Bitmap srcBitmap, ref Bitmap targetBitmap)
        {
            // 检查目标Bitmap是否存在且尺寸格式匹配
            //if (targetBitmap == null ||
            //    targetBitmap.Width != srcBitmap.Width ||
            //    targetBitmap.Height != srcBitmap.Height ||
            //    targetBitmap.PixelFormat != srcBitmap.PixelFormat)
            //{
            //    targetBitmap?.Dispose();
            //    targetBitmap = new Bitmap(
            //        srcBitmap.Width,
            //        srcBitmap.Height,
            //        srcBitmap.PixelFormat
            //    );
            //}

            // 锁定两者的位图数据区域
            BitmapData srcData = srcBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height),
                ImageLockMode.ReadOnly,
                srcBitmap.PixelFormat
            );

            BitmapData dstData = targetBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, targetBitmap.Width, targetBitmap.Height),
                ImageLockMode.WriteOnly,
                targetBitmap.PixelFormat
            );

            try
            {
                // 使用 unsafe 代码直接内存拷贝
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                // 计算总字节数（考虑 stride 对齐）
                int totalBytes = Math.Abs(srcData.Stride) * srcData.Height;

                // 使用 Buffer.MemoryCopy 进行高效内存拷贝
                Buffer.MemoryCopy(
                    srcPtr,      // 源指针
                    dstPtr,      // 目标指针
                    totalBytes,  // 目标缓冲区大小
                    totalBytes   // 要复制的字节数
                );
            }
            finally
            {
                // 确保总是解锁位图
                srcBitmap.UnlockBits(srcData);
                targetBitmap.UnlockBits(dstData);
            }

            return targetBitmap;
        }

        //////////////////////////////// 串口通信 ////////////////////////////////
        private void open_btn_Click(object sender, EventArgs e)
        {
            try
            {
                //如果串口是关闭的和端口号不为空,就给串口配置赋值
                if (_serialPort.IsOpen == false && port_cbb.Text != "")
                {
                    _serialPort.PortName = port_cbb.Text;
                    //打开串口
                    _serialPort.Open();
                    open_btn.Text = "关闭串口";
                }
                else
                {
                    _serialPort.Close();//关闭串口
                    open_btn.Text = "打开串口";                  
                }
            }
            catch (Exception ex)
            {
                port_cbb.Items.Clear();
                port_cbb.Text = "";
                MessageBox.Show(ex.ToString() + _serialPort.PortName.ToString());//抛出报错信息和端口号
            }
        }
        public void GetComList()
        {
            port_cbb.Items.Clear();
            port_cbb.Text = "";
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();
                foreach (string sName in sSubKeys)
                {
                    string sValue = (string)keyCom.GetValue(sName);
                    port_cbb.Items.Add(sValue);
                }
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting(); // 读取所有可用的数据
            if (indata.Equals("{ok}"))
                _MouseEvent.Set(); // 唤醒处理线程
        }

    }
}