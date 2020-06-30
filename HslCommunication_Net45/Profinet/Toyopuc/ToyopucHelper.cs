using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HslCommunication.Profinet.Toyopuc
{
    /// <summary>
    /// Toyopuc工具类
    /// </summary>
    public class ToyopucHelper
    {
        /// <summary>
        /// 解析Toyopuc数据地址
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static OperateResult<ToyopucDataType, ushort> AnalysisAddress(string address)
        {
            var result = new OperateResult<ToyopucDataType, ushort>();
            try
            {
                switch (address[0])
                {
                    case 'M':
                    case 'm':
                        {
                            result.Content1 = ToyopucDataType.M;
                            ushort addr = Convert.ToUInt16(address.Substring(1), ToyopucDataType.M.FromBase);
                            result.Content2 = (ushort)(ToyopucDataType.M.BitAddress + addr);
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content1 = ToyopucDataType.D;
                            ushort addr = Convert.ToUInt16(address.Substring(1), ToyopucDataType.D.FromBase);
                            result.Content2 = (ushort)(ToyopucDataType.D.WordAddress + addr);
                            break;
                        }
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 生成读字命令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] BuildReadWordCoreCommand(ushort address, ushort length)
        {
            byte[] _PLCCommand = new byte[4];
            _PLCCommand[0] = (byte)(address % 256);               // Address(L)
            _PLCCommand[1] = (byte)(address / 256);               // Address(H)
            _PLCCommand[2] = (byte)(length % 256);                // Word No.(L)
            _PLCCommand[3] = (byte)(length / 256);                // Word No.(H)
            return _PLCCommand;
        }

        /// <summary>
        /// 生成读位命令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] BuildReadBitCoreCommand(ushort address, ushort length)
        {
            byte[] _PLCCommand = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                ushort addr = (ushort)(address + i);
                int index = i * 2;
                _PLCCommand[index] = (byte)(addr % 256);               // Address(L)
                _PLCCommand[index + 1] = (byte)(addr / 256);           // Address(H)
            }
            return _PLCCommand;
        }


        /// <summary>
        /// 生成写字命令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] BuildWriteWordCoreCommand(ushort address, byte[] data)
        {
            byte[] _PLCCommand = new byte[data.Length + 2];
            _PLCCommand[0] = (byte)(address % 256);               // Address(L)
            _PLCCommand[1] = (byte)(address / 256);               // Address(H)

            data.CopyTo(_PLCCommand, 2);
            return _PLCCommand;
        }

        /// <summary>
        /// 生成写位命令
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] BuildWriteBitCoreCommand(ushort address, byte[] data)
        {
            byte[] _PLCCommand = new byte[data.Length * 3];
            for (int i = 0; i < data.Length; i++)
            {
                ushort addr = (ushort)(address + i);
                int index = i * 3;

                _PLCCommand[index] = (byte)(addr % 256);               // Address(L)
                _PLCCommand[index + 1] = (byte)(addr / 256);           // Address(H)
                _PLCCommand[index + 2] = data[i];
            }
            return _PLCCommand;
        }
    }
}
