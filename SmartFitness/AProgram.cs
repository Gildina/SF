using SmartFitness;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace iFitTest3
{
    class Program
    {
        static ttsRead tts ;
        Program()
        {
            tts = new ttsRead();
        }

        public static void Move()
        {
            Type type = Type.GetTypeFromProgID("SAPI.SpVoice");
            dynamic spVoice = Activator.CreateInstance(type);
            spVoice.Speak("开始健身");
            //ttsRead t = new ttsRead();
            //t.Read("start");
            
            PortDataListener a = PortDataListener.Instance;

           
        }


      
        public static int getStatus()
        {
            if (PortDataListener.isAction && PortDataListener.isOne)
            {
                return 1;
            }else if (PortDataListener.isAction && !PortDataListener.isOne)
            {
                return 3;
            }else if (!PortDataListener.isAction)
            {
                return 0;
            }

            return 0;
        }

        public static void setONE()
        {
            PortDataListener.isAction = true;
            PortDataListener.isOne = true;
        }
        public static void setMore()
        {
            PortDataListener.isAction = true;
            PortDataListener.isOne = false;
        }
        public static void setNone()
        {
            PortDataListener.isAction =  false;
            PortDataListener.isOne = false;
        }

        public static bool judge_start(double[,,] x, int j)
        {

            double d1 = (x[j, 1, 0] - x[j, 0, 0]);
            double t1 = (x[j, 1, 1] - x[j, 0, 1]);
            double d2 = (x[j, 2, 0] - x[j, 1, 0]);
            double t2 = (x[j, 2, 1] - x[j, 1, 1]);
            double d3 = (x[j, 3, 0] - x[j, 2, 0]);
            double t3 = (x[j, 3, 1] - x[j, 2, 1]);

            double dt1 = Math.Abs(d1 * t2 - d2 * t1);
            double dt2 = Math.Abs(d2 * t3 - d3 * t2);
            double dt3 = Math.Abs(d1 * t3 - d3 * t1);
            if (Cal_dis(x[j, 2, 0], x[j, 2, 1], x[j, 3, 0], x[j, 3, 1]) < 1.0)
            {
                return false;
            }
            if (dt1 < 0.30 || dt2 < 0.30 || dt3 < 0.30)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void record_start(double[,,] x, int j, double[] y)
        {
            y[0] = x[0, 0, 0];
            y[1] = x[0, 0, 1];
            y[2] = x[0, 1, 0];
            y[3] = x[0, 1, 1];
            y[4] = x[0, 2, 0];
            y[5] = x[0, 2, 1];
            y[6] = x[0, 3, 0];
            y[7] = x[0, 3, 1];
        }

        public static int track(double[,,] x, int j)
        {
            double x1 = Math.Sqrt((x[j, 2, 0] - x[j, 3, 0]) * (x[j, 2, 0] - x[j, 3, 0]) +
                       (x[j, 2, 1] - x[j, 3, 1]) * (x[j, 2, 1] - x[j, 3, 1]));
            double y1 = Math.Sqrt((x[j - 1, 2, 0] - x[j - 1, 3, 0]) * (x[j - 1, 2, 0] - x[j - 1, 3, 0]) +
                       (x[j - 1, 2, 1] - x[j - 1, 3, 1]) * (x[j - 1, 2, 1] - x[j - 1, 3, 1]));
            if (x1 - y1 > 0.05)
            {
                return 2;
            }
            else if (x1 - y1 < -0.05)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static double Cal_delta(double[,,] x, int j)
        {
            double max = 0;

            for (int i = 0; i < 4; i++)
            {
                double[,] tem1 = new double[4, 2];
                double[,] tem2 = new double[4, 2];
                for (int k = 0; k < 4; k++)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        tem1[k, n] = x[j, k, n];
                        tem2[k, n] = x[j - i - 1, k, n];
                    }
                }
                double ex = Cal_delta_part(tem1, tem2);
                if (ex > max)
                {
                    max = ex;
                }
            }
            return max;
        }

        public static double Cal_delta_part(double[,] x, double[,] y)
        {
            double z1 = System.Math.Sqrt((x[0, 0] - y[0, 0]) * (x[0, 0] - y[0, 0]) +
                            (x[0, 1] - y[0, 1]) * (x[0, 1] - y[0, 1]));
            double z2 = System.Math.Sqrt((x[1, 0] - y[1, 0]) * (x[1, 0] - y[1, 0]) +
                            (x[1, 1] - y[1, 1]) * (x[1, 1] - y[1, 1]));
            double z3 = System.Math.Sqrt((x[2, 0] - y[2, 0]) * (x[2, 0] - y[2, 0]) +
                            (x[2, 1] - y[2, 1]) * (x[2, 1] - y[2, 1]));
            double z4 = System.Math.Sqrt((x[3, 0] - y[3, 0]) * (x[3, 0] - y[3, 0]) +
                            (x[3, 1] - y[3, 1]) * (x[3, 1] - y[3, 1]));
            double max = z1;
            if (z2 > max)
            {
                max = z2;
            }
            if (z3 > max)
            {
                max = z3;
            }
            if (z4 > max)
            {
                max = z4;
            }
            return max;
        }

        public static void Sound(String str)
        {
            
            tts.Read(str);
        }

        public static bool Judge_near(double x, double[,,] y, int ord, int num)
        {
            double d = System.Math.Abs(x - y[num, ord, 2]);
            if (d < 0.02)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Judge_near_point(double x1, double y1, double x2, double y2)
        {
            double d = System.Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            if (d < 0.10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Judge_near_distance(double x1, double y1, double x2, double y2)
        {
            double d = x1 * y2 - x2 * y1;
            return true;
        }

        public static int Judge_track(double[,,] x, int j)
        {
            int[] ip = new int[10];
            int num_1 = 0, num_2 = 0;

            if (j > 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    ip[i] = track(x, j - 9 + i);
                    if (ip[i] == 1)
                    {
                        num_1++;
                    }
                    else
                    {
                        num_2++;
                    }
                }
                if (num_1 > 8)
                {
                    return 1;
                }
                if (num_2 > 8)
                {
                    return 2;
                }
                if (num_1 > 3 && num_1 < 7)
                {
                    return 3;
                }
                return 0;
            }
            else
            {
                return 0;
            }
        }

        public static double Cal_dis(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt((x1 - y1) * (x1 - y1) + (x2 - y2) * (x2 - y2));
        }

        public static bool judge_direct(double[,,] x, int j)
        {
            double d1 = (x[j, 1, 0] - x[j, 0, 0]);
            double t1 = (x[j, 1, 1] - x[j, 0, 1]);
            double d2 = (x[j, 2, 0] - x[j, 1, 0]);
            double t2 = (x[j, 2, 1] - x[j, 1, 1]);
            double d3 = (x[j, 3, 0] - x[j, 2, 0]);
            double t3 = (x[j, 3, 1] - x[j, 2, 1]);

            double dt1 = Math.Abs(d1 * t2 - d2 * t1);
            double dt2 = Math.Abs(d2 * t3 - d3 * t2);
            double dt3 = Math.Abs(d1 * t3 - d3 * t1);
            if (dt1 < 0.15 || dt2 < 0.15 || dt3 < 0.15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool judge_finish(double[,,] x, int j)
        {
            int i = 0;
            for (i = 0; i < 10; i++)
            {
//                Console.WriteLine((x[j - 9 + i, 2, 0] + "," + x[j - 9 + i, 2, 1] + "," + x[j - 9 + i, 3, 0] + "," + x[j - 9 + i, 3, 1]));
                double dis = Cal_dis(x[j - 9 + i, 2, 0], x[j - 9 + i, 2, 1], x[j - 9 + i, 3, 0], x[j - 9 + i, 3, 1]);
//                Console.WriteLine(dis);
                if (dis < 0.7)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
