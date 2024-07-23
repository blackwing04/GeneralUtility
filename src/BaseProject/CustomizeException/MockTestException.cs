using System;

namespace CustomizeException
{
    /// <summary>
    /// 單元測試模擬的例外
    /// </summary>
    public class MockTestException : Exception
    {
        /// <summary>
        /// 單元測試模擬的例外
        /// </summary>
        public MockTestException() : base("For Mock Test")
        {
        }
    }
}
