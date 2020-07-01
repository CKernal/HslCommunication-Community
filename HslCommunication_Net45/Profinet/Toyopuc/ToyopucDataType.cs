using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Toyopuc
{
    /// <summary>
    /// 丰田工机PLC数据类型
    /// </summary>
    public class ToyopucDataType
    {
        /// <summary>
        /// 实例化一个丰田工机PLC数据类型
        /// </summary>
        /// <param name="wordAddress"></param>
        /// <param name="byteAddress"></param>
        /// <param name="bitAddress"></param>
        /// <param name="type"></param>
        /// <param name="fromBase"></param>
        public ToyopucDataType(ushort wordAddress, ushort byteAddress, ushort bitAddress, byte type, int fromBase)
        {
            WordAddress = wordAddress;
            ByteAddress = byteAddress;
            BitAddress = bitAddress;
            FromBase = fromBase;
            if (type < 2) DataType = type;
        }

        /// <summary>
        /// 以word方式读写的基础地址
        /// </summary>
        public ushort WordAddress { get; private set; }

        /// <summary>
        /// 以byte方式读写的基础地址
        /// </summary>
        public ushort ByteAddress { get; private set; }

        /// <summary>
        /// 以bit方式读写的基础地址
        /// </summary>
        public ushort BitAddress { get; private set; }

        ///// <summary>
        ///// 类型的代号值
        ///// </summary>
        //public byte[] DataCode { get; private set; } = { 0x00, 0x00 };


        /// <summary>
        /// 数据的类型，0代表按字，1代表按位
        /// </summary>
        public byte DataType { get; private set; } = 0x00;

        /// <summary>
        /// 指示地址是10进制，还是16进制的
        /// </summary>
        public int FromBase { get; private set; }
        /// <summary>
        /// M内部继电器
        /// </summary>
        public readonly static ToyopucDataType M = new ToyopucDataType(0x0180, 0x0300, 0x1800, 0x01, 16);
        /// <summary>
        /// D数据寄存器
        /// </summary>
        public readonly static ToyopucDataType D = new ToyopucDataType(0x1000, 0x2000, 0x0000, 0x00, 16);
    }
}
