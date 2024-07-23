using StaticUtil.Models.DAO;
using System;

namespace StaticUtil.Generic
{
    public static class ResultUtil
    {
        /// <summary>
        /// 處理成功的結果
        /// </summary>
        /// <param name="dbResult">結果物件</param>
        /// <param name="message">訊息</param>
        public static void HandleSuccessfulResult(
            NonQueryResultModel dbResult,
            string message)
        {
            dbResult.IsOperationSuccessful = true;
            dbResult.ResultString = message;
        }
        /// <summary>
        /// 處理失敗的結果
        /// </summary>
        /// <param name="dbResult">結果物件</param>
        /// <param name="message">訊息</param>
        /// <param name="ex">內部錯誤(可空)</param>
        public static void HandleFailedResult(
            NonQueryResultModel dbResult,
            string message,
            Exception ex = null)
        {
            dbResult.IsOperationSuccessful = false;
            dbResult.ResultString = message;
            dbResult.InnerException = ex;
        }

        /// <summary>
        /// 處理成功的結果
        /// </summary>
        /// <param name="dbResult">結果物件</param>
        /// <param name="message">訊息</param>
        /// <param name="result">資料庫查詢的結果</param>
        public static void HandleSuccessfulResult<T>(
            DbQueryResultModel<T> dbResult,
            string message,
            T result)
        {
            dbResult.IsOperationSuccessful = true;
            dbResult.ResultString = message;
            dbResult.Result = result;
        }
        /// <summary>
        /// 處理成功的結果(但查詢的內容是空的)
        /// </summary>
        /// <param name="dbResult">結果物件</param>
        /// <param name="message">訊息</param>
        public static void HandleSuccessfulResult<T>(
        DbQueryResultModel<T> dbResult,
        string message)
        {
            dbResult.IsOperationSuccessful = true;
            dbResult.ResultString = message;
            dbResult.Result = default;
        }
        /// <summary>
        /// 處理失敗的結果
        /// </summary>
        /// <param name="dbResult">結果物件</param>
        /// <param name="message">訊息</param>
        /// <param name="ex">內部錯誤(可空)</param>
        public static void HandleFailedResult<T>(
            DbQueryResultModel<T> dbResult,
            string message,
            Exception ex = null)
        {
            dbResult.IsOperationSuccessful = false;
            dbResult.ResultString = message;
            dbResult.InnerException = ex;
        }
    }
}
