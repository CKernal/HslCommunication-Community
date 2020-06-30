using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HslCommunication.Profinet.Toyopuc
{
    /// <summary>
    /// 丰田工机PLC通讯类（TCP协议）
    /// </summary>
    public class ToyopucNet : NetworkDeviceBase<ToyopucMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化丰田工机PLC通讯对象
        /// </summary>
        public ToyopucNet()
        {
            WordLength = 1;
        }

        /// <summary>
        /// 实例化一个丰田工机PLC通讯对象
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public ToyopucNet(string ipAddress, int port)
        {
            WordLength = 1;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Read Support

        /// <summary>
        /// 从PLC中读取想要的数据，返回读取结果
        /// </summary>
        /// <param name="address">读取地址</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            // 获取指令
            var command = BuildReadCommand(address, length);
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(command);

            // 核心交互
            var read = ReadFromCoreServer(command.Content);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(read);

            // 错误代码验证
            if (read.Content[1] != 0) return new OperateResult<byte[]>(read.Content[4], StringResources.Language.ToyopucPleaseReferToManulDocument);

            // 数据解析，需要传入是否使用位的参数
            return ExtractActualData(SoftBasic.BytesArrayRemoveBegin(read.Content, 5), false);
        }

        /// <summary>
        /// 从PLC批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            // 获取指令
            var command = BuildReadCommand(address, length);
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(command);

            // 核心交互
            var read = ReadFromCoreServer(command.Content);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(read);

            // 错误代码验证
            if (read.Content[1] != 0) return new OperateResult<bool[]>(read.Content[4], StringResources.Language.ToyopucPleaseReferToManulDocument);

            
            // 数据解析，需要传入是否使用位的参数
            var extract = ExtractActualData(SoftBasic.BytesArrayRemoveBegin(read.Content, 5), true);
            if (!extract.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(extract);

            // 转化bool数组
            return OperateResult.CreateSuccessResult(extract.Content.Select(m => m == 0x01).Take(length).ToArray());
        }

        #endregion

        #region Write Override

        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write(string address, byte[] value)
        {
            // 解析指令
            OperateResult<byte[]> command = BuildWriteCommand(address, value);
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer(command.Content);
            if (!read.IsSuccess) return read;

            // 错误码校验
            if (read.Content[1] != 0) return new OperateResult(read.Content[4], StringResources.Language.ToyopucPleaseReferToManulDocument);

            // 成功
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 向PLC位写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write(string address, bool[] values)
        {
            byte[] value = values.Select(m => m ? (byte)0x01 : (byte)0x00).ToArray();

            // 解析指令
            OperateResult<byte[]> command = BuildWriteCommand(address, value);
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer(command.Content);
            if (!read.IsSuccess) return read;

            // 错误码校验
            if (read.Content[1] != 0) return new OperateResult(read.Content[4], StringResources.Language.ToyopucPleaseReferToManulDocument);

            // 成功
            return OperateResult.CreateSuccessResult();
        }

        #endregion

        #region Object Override

        /// <summary>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"ToyopucNet[{IpAddress}:{Port}]";
        }

        #endregion

        #region Static Method Helper

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度，小于512（200H）</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildReadCommand(string address, ushort length)
        {
            var analysis = ToyopucHelper.AnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // Command Code
            //0x1C Reading I/O Register Word
            //0x1D Writing I/O Register Word
            //0x26 Reading I/O Register Multi-Point Bit
            //0x27 Writing I/O Register Multi-Point Bit

            bool isBit = (analysis.Content1.DataType == 0x01);

            byte command = 0x00;
            byte[] coreCmd = null;
            if (isBit)
            {
                command = 0x26;
                coreCmd = ToyopucHelper.BuildReadBitCoreCommand(analysis.Content2, length);
            }
            else
            {
                command = 0x1C;
                coreCmd = ToyopucHelper.BuildReadWordCoreCommand(analysis.Content2, length);
            }
            ushort transferNumber = (ushort)(coreCmd.Length + 1);

            byte[] _PLCCommand = new byte[5 + coreCmd.Length];
            _PLCCommand[0] = 0x00;               // Frame Type
            _PLCCommand[1] = 0x00;               // Response Code
            _PLCCommand[2] = (byte)(transferNumber % 256);               // Transfer Number(L)
            _PLCCommand[3] = (byte)(transferNumber / 256);               // Transfer Number(H)
            _PLCCommand[4] = command;                                    // Command Code

            coreCmd.CopyTo(_PLCCommand, 5);

            return OperateResult.CreateSuccessResult(_PLCCommand);
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">数据值</param>
        /// <returns>带有成功标志的指令数据</returns>
        public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
        {
            var analysis = ToyopucHelper.AnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // Command Code
            //0x1C Reading I/O Register Word
            //0x1D Writing I/O Register Word
            //0x26 Reading I/O Register Multi-Point Bit
            //0x27 Writing I/O Register Multi-Point Bit

            bool isBit = (analysis.Content1.DataType == 0x01);

            byte command = 0x00;
            byte[] coreCmd = null;
            if (isBit)
            {
                command = 0x27;
                coreCmd = ToyopucHelper.BuildWriteBitCoreCommand(analysis.Content2, value);
            }
            else
            {
                command = 0x1D;
                coreCmd = ToyopucHelper.BuildWriteWordCoreCommand(analysis.Content2, value);
            }
            ushort transferNumber = (ushort)(coreCmd.Length + 1);

            byte[] _PLCCommand = new byte[5 + coreCmd.Length];
            _PLCCommand[0] = 0x00;               // Frame Type
            _PLCCommand[1] = 0x00;               // Response Code
            _PLCCommand[2] = (byte)(transferNumber % 256);               // Transfer Number(L)
            _PLCCommand[3] = (byte)(transferNumber / 256);               // Transfer Number(H)
            _PLCCommand[4] = command;                                    // Command Code

            coreCmd.CopyTo(_PLCCommand, 5);

            return OperateResult.CreateSuccessResult(_PLCCommand);
        }

        /// <summary>
        /// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
        /// </summary>
        /// <param name="response">反馈的数据内容</param>
        /// <param name="isBit">是否位读取</param>
        /// <returns>解析后的结果对象</returns>
        public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit)
        {
            if (isBit)
            {
                // 位读取
                return OperateResult.CreateSuccessResult(response);
            }
            else
            {
                // 字读取
                return OperateResult.CreateSuccessResult(response);
            }
        }

        #endregion
    }
}
