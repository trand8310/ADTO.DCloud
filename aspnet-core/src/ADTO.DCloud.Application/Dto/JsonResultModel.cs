

using System.Collections.Generic;

namespace ADTO.DCloud.Dto
{
    public class JsonResultModel<T> where T : class
    {
        public JsonResultModel()
        {
            Success = false;
            Properties = new Dictionary<string, object>();
        }
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResultModel<T> SuccessInfo(string message = "操作成功", T data = null)
        {
            this.Message = message;
            this.Success = true;
            this.Data = data;
            return this;
        }

        public JsonResultModel<T> SuccessInfo(string message = "操作成功", T data = null, Dictionary<string, object> properties = null)
        {
            this.Message = message;
            this.Success = true;
            this.Data = data;
            if (properties != null)
            {
                this.Properties = properties;
            }

            return this;
        }
        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResultModel<T> ErrorInfo(string message = "操作失败", T data = null)
        {
            this.Success = false;
            this.Message = message;
            this.Data = data;
            return this;
        }


    }


    public class JsonResultModel
    {
        public JsonResultModel()
        {
            Success = false;
        }
        public bool Success { get; set; }
        public string Data { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResultModel SuccessInfo(string message = "操作成功", string data = null)
        {
            this.Message = message;
            this.Success = true;
            if (string.IsNullOrEmpty(data))
            {
                this.Data = data;
            }
            return this;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResultModel ErrorInfo(string message = "操作失败", string data = null)
        {
            this.Success = false;
            this.Message = message;
            if (string.IsNullOrEmpty(data))
            {
                this.Data = data;
            }
            return this;
        }
    }
}
