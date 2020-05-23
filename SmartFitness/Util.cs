using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFitTest3
{
    class PortData
    {
        public Tag[] tags = new Tag[4];

        public PortData(byte[] buffer)
        {
            byte tmp_id;
            byte[] tmp = new byte[27];
            for (int i = 0; i < 30; i++)
            {
                tmp_id = buffer[2 + 27 * i];
                if (tmp_id < 4    && tags[tmp_id] == null)
                {
                    Array.Copy(buffer, 2 + 27 * i, tmp, 0, 27);
                    tags[tmp_id] = new Tag(tmp);
                   
                }
            }

            //存在有的id不存在的时候
            foreach (var VARIABLE in tags)
            {
                if (VARIABLE == null)
                {
//                    throw new SomeTagIsNullException("有一个标签没有数据");
                }
            }
        }

        public void print()
        {
            foreach (var tmp in tags)
            {
                try
                {
                    Console.WriteLine("tag" + tmp.id + ":" + tmp.id + " " + tmp.X + " " + tmp.Y +
                                      " " + tmp.Z);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


            }
            Console.WriteLine();
        }
    }

    //tags
    class Tag
    {
        public byte id;
        public int X;
        public int Y;
        public int Z;



        static unsafe int BytesToInt24(byte[] byteArray)
        {
            int data = 0;
            byte* pdata = (byte*)(&data);
            pdata[0] = 0;
            pdata[1] = byteArray[0];
            pdata[2] = byteArray[1];
            pdata[3] = byteArray[2];
            return data / 256; //低位赋0，为保留符号有效性，有效数据分别赋值到高三位，最后除以256，返回真实值 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">大小为27</param>
        public Tag(byte[] buffer)
        {
            this.id = buffer[0];

            byte[] tmp = new byte[3];
            Array.Copy(buffer, 2, tmp, 0, 3);
            this.X = BytesToInt24(tmp);
            Array.Copy(buffer, 5, tmp, 0, 3);
            this.Y = BytesToInt24(tmp);
            Array.Copy(buffer, 8, tmp, 0, 3);
            this.Z = BytesToInt24(tmp);
        }
    }
}

class SomeTagIsNullException : ApplicationException
{
    //public SomeTagIsNullException(){}  
    public SomeTagIsNullException(string message) : base(message) { }

    public override string Message
    {
        get
        {
            return base.Message;
        }
    }
}
