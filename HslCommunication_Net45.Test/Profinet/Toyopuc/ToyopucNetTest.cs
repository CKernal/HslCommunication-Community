using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Profinet.Toyopuc;

namespace HslCommunication_Net45.Test.Profinet.Toyopuc
{
    /// <summary>
    /// ToyopucNetTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ToyopucNetTest
    {
        public ToyopucNetTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ToyopucNetUnitTest()
        {
            ToyopucNet plc = new ToyopucNet("127.0.0.1", 8899);
            if (!plc.ConnectServer().IsSuccess)
            {
                Console.WriteLine("无法连接PLC，将跳过单元测试。等待网络正常时，再进行测试");
                return;
            }

            // 开始单元测试，从bool类型开始测试
            string address = "M200";
            bool[] boolTmp = new bool[] { true, true, false, true, false, true, false };
            Assert.IsTrue(plc.Write(address, true).IsSuccess);
            Assert.IsTrue(plc.ReadBool(address).Content == true);
            Assert.IsTrue(plc.Write(address, boolTmp).IsSuccess);
            bool[] readBool = plc.ReadBool(address, (ushort)boolTmp.Length).Content;
            for (int i = 0; i < boolTmp.Length; i++)
            {
                Assert.IsTrue(readBool[i] == boolTmp[i]);
            }

            address = "D300";
            // short类型
            Assert.IsTrue(plc.Write(address, (short)12345).IsSuccess);
            Assert.IsTrue(plc.ReadInt16(address).Content == 12345);
            short[] shortTmp = new short[] { 123, 423, -124, 5313, 2361 };
            Assert.IsTrue(plc.Write(address, shortTmp).IsSuccess);
            short[] readShort = plc.ReadInt16(address, (ushort)shortTmp.Length).Content;
            for (int i = 0; i < readShort.Length; i++)
            {
                Assert.IsTrue(readShort[i] == shortTmp[i]);
            }

            // ushort类型
            Assert.IsTrue(plc.Write(address, (ushort)51234).IsSuccess);
            Assert.IsTrue(plc.ReadUInt16(address).Content == 51234);
            ushort[] ushortTmp = new ushort[] { 5, 231, 12354, 5313, 12352 };
            Assert.IsTrue(plc.Write(address, ushortTmp).IsSuccess);
            ushort[] readUShort = plc.ReadUInt16(address, (ushort)ushortTmp.Length).Content;
            for (int i = 0; i < ushortTmp.Length; i++)
            {
                Assert.IsTrue(readUShort[i] == ushortTmp[i]);
            }
        }
    }
}
