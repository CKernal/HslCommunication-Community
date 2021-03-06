﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{
    /// <summary>
    /// 丰田工机PLC协议解析规则
    /// </summary>
    public class ToyopucMessage : INetMessage
    {
        /// <summary>
        /// 消息头的指令长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 5; }//帧头(0x80)+回复码+传输数(L)+传输数(H)+命令码  共5个byte
        }

        /// <summary>
        /// 检查头子节的合法性
        /// </summary>
        /// <param name="token">特殊的令牌，有些特殊消息的验证</param>
        /// <returns></returns>
        public bool CheckHeadBytesLegal(byte[] token)
        {
            if (HeadBytes != null)
            {
                if ((HeadBytes[0] - SendBytes[0]) == 0x80) { return true; }
            }
            return false;
        }


        /// <summary>
        /// 从当前的头子节文件中提取出接下来需要接收的数据长度
        /// </summary>
        /// <returns>返回接下来的数据内容长度</returns>
        public int GetContentLengthByHeadBytes()
        {
            return BitConverter.ToUInt16(HeadBytes, 2) - 1;
        }

        /// <summary>
        /// 获取头子节里的消息标识
        /// </summary>
        /// <returns></returns>
        public int GetHeadBytesIdentity()
        {
            return 0;
        }

        /// <summary>
        /// 消息头字节
        /// </summary>
        public byte[] HeadBytes { get; set; }


        /// <summary>
        /// 消息内容字节
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}
