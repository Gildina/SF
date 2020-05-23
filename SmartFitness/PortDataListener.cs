using System.Collections;
using System;
using System.Collections.Generic;
using iFitTest3;
using SmartFitness;

namespace iFitTest3
{
    using System.IO.Ports;

    class PortDataListener
    {


        //是否做动作,true的时候才进行处理
        public static Boolean isAction= false;
        //默认多次动作
        public static Boolean isOne = false;

        private int numOfExe = 0;

        double[] position_start = new double[8];
        double[,,] datament = new double[1000, 4, 2];
        Boolean end = false, start = false, middle = false, wrong = false, finish = false;
        private int i = 0, j=0;

        //private static  Type type = Type.GetTypeFromProgID("SAPI.SpVoice");
        private dynamic spVoice = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpVoice"));
        //dynamic spVoice = Activator.CreateInstance(type);

        private static ttsRead myTTS = new ttsRead();

        public readonly Queue data_buffer = Queue.Synchronized(new Queue());

        private static volatile PortDataListener portDataListener;
        private static readonly object obj = new object();


        //新村的一个
        private List<byte> buffer = new List<byte>();
        private static SerialPort mySerialPort;

        private PortDataListener()
        {

            mySerialPort = new SerialPort("COM3");

            mySerialPort.BaudRate = 460800;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.DataReceived +=  new SerialDataReceivedEventHandler(DataReceivedHandler);

            if (!mySerialPort.IsOpen)
                mySerialPort.Open();
        }

//线程安全的单例,抄的
        public static PortDataListener Instance
        {
            get
            {
                if (null == portDataListener)
                {
                    lock (obj)
                    {
                        if (null == portDataListener)
                        {
                            portDataListener = new PortDataListener();
                        }
                    }

                }

                return portDataListener;
            }
        }


        public PortData ReaData()
        {

            if (this.data_buffer.Count > 0)
            {
                return (PortData) data_buffer.Dequeue();
            }
            else
            {
                return null;
            }

        }

        public void DataReceivedHandler(
            object sender,
            SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort1 = (SerialPort) sender;
            int n = serialPort1.BytesToRead; //待读字节个数
            byte[] buf = new byte[n]; //创建n个字节的缓存
            serialPort1.Read(buf, 0, n); //读到在数据存储到buf

            
            //1.缓存数据
            buffer.AddRange(buf); //不断地将接收到的数据加入到buffer链表中
            //2.完整性判断
            while (buffer.Count >= 896 && isAction) //至少包含帧头（2字节）、长度（1字节）、功能位（1字节）；根据设计不同而不同
            {
                if (isOne)
                {
                    Console.WriteLine("进行danci数据处理------------");
                }
                
                else
                {
                    Console.WriteLine("进行多次处理");
                }
                //2.1 查找数据头
                if (buffer[0] == 85 && buffer[895] == 238) //传输数据有帧头，用于判断. 找到帧头  AA AA 0A 
                {

                    byte[] tmp = new byte[896];
                    Array.Copy(buffer.ToArray(), 0, tmp, 0, 896);
                    PortData data = new PortData(tmp);

                    if (hasNull(data))
                    {
                        buffer.RemoveRange(0, 895);
                    }
                    else
                    {
                        while (data_buffer.Count>300)
                        {
                            data_buffer.Clear();
                            
                        }
                        

                        if (data != null)
                        {
//                            data.print();
//                            Console.WriteLine("有数据");

                            //把数据搞出来
                            for (int m = 0; m < 4; m++)
                            {
                                position_start[m * 2 + 0] = (double)data.tags[m].X / 1000;
                                position_start[m * 2 + 1] = (double)data.tags[m].Y / 1000;
                                datament[j, m, 0] = position_start[m * 2 + 0];
                                datament[j, m, 1] = position_start[m * 2 + 1];
                            }

                            //没有开始的时候输出delta
                            //                    if (j > 5 && start == false)
                            //                        Console.WriteLine(Cal_delta(datament, j));

                            if (start == false && j > 5 && Program.Cal_delta(datament, j) < 0.3)
                            {
                                if (Program.judge_start(datament, j) == true)
                                {
                                    //spVoice.Speak(string.Empty, 2);
                                    spVoice.speak("开始", 1);
                                    //myTTS.Read("begin");
                                    Console.WriteLine("bdgin");
                                    //Console.WriteLine(j);
                                    start = true;
                                    Program.record_start(datament, j, position_start);
                                }
                            }

                            if (start == true && end == false)
                            {
                                int sum = 0;

                                if (Program.Judge_track(datament, j) == 0)
                                {
                                    ;
                                }

                                if (Program.Judge_track(datament, j) == 1)
                                {
                                    ;
                                }

                                if (Program.Judge_track(datament, j) == 2)
                                {
                                    ;
                                }

                                if (Program.Judge_track(datament, j) == 3)
                                {
                                    middle = true;
                                    finish = Program.judge_finish(datament, j);
                                }

                                if (middle == true)
                                {
                                    double dis = Program.Cal_dis(datament[j, 2, 0], datament[j, 2, 1],
                                                    datament[j, 3, 0], datament[j, 3, 1]);

                                    if (dis > 1.5)
                                    {
                                        for (i = 0; i < 10; i++)
                                        {
                                            if (Program.judge_direct(datament, j - 9 + i))
                                            {
                                                sum++;
                                            }
                                        }

                                        if (sum < 7)
                                        {
                                            wrong = true;
                                        }
                                        end = true;
                                        Console.WriteLine(sum);
                                    }
                                }
                            }

                            if (end == true)
                            {
                                //如果一次动作的,直接不做了
                                if (isOne)
                                    isAction = false;
                                numOfExe += 1;
                                if (wrong == true)
                                {
                                    spVoice.Speak(string.Empty, 2);
                                    spVoice.speak("动作错误，请保持手臂垂直！", 1);
                                    Console.WriteLine("wrong");
                                    //myTTS.Read("The NO."+numOfExe+" Wrong action,please keep your arms vertical！");
                                }
                                if (wrong == false && finish == false)
                                {
                                    spVoice.Speak(string.Empty, 2);
                                    spVoice.speak("您上一个动作幅度未达标！", 1);
                                    Console.WriteLine("未达标");
                                    //myTTS.Read("The num "+numOfExe+" your action do not reach the standard！");
                                    //MessageBox.Show("您上一个动作幅度未达标！");
                                }
                                if (wrong == false && finish == true)
                                {
                                    spVoice.Speak(string.Empty, 2);
                                    spVoice.speak("您的动作很标准，没有问题!", 1);
                                    Console.WriteLine("right");
                                    //myTTS.Read("The NO."+numOfExe+" your action has no problem!");
                                    //MessageBox.Show("您的动作很标准，没有问题!");
                                }

                                end = false;
                                middle = false;
                                wrong = false;
                                start = false;

                                j = -1;
                            }
                            j++;
                        }
                        buffer.RemoveRange(0, 895);
                    }
                   

                }
                else //帧头不正确时，记得清除
                {
                    buffer.RemoveAt(0); //清除第一个字节，继续检测下一个。
                }

                //            SerialPort sp = (SerialPort) sender;
                //            int n = sp.BytesToRead;
                //
                //
                //            while (n >= 896)
                //            {
                //
                //
                //                byte[] buffer = new byte[896];
                //
                //                sp.Read(buffer, 0, 896);
                //                try
                //                {
                //                    PortData data = new PortData(buffer);
                //
                //                    while (this.data_buffer.Count > 20)
                //                    {
                //                        this.data_buffer.Dequeue();
                //                    }
                //                    this.data_buffer.Enqueue(data);
                //                }
                //                catch (SomeTagIsNullException exception)
                //                {
                //                    Console.WriteLine(exception.Message);
                //                    Console.WriteLine("有一个标签没有数据,");
                //                }
                //
                //                n -= 896;
                //            }
            }

            if (!isAction)
            {
                Console.WriteLine("不进行数据处理");
                buffer.Clear();
                data_buffer.Clear();
                end = false;
                middle = false;
                wrong = false;
                start = false;
            }
        }


        private static bool hasNull(PortData data)
        {
            foreach (var VARIABLE in data.tags)
            {
                if (VARIABLE == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
