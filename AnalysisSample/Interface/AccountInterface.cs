using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2.Interface
{
    interface AccountInterface
    {
        /// <summary>
        ///  根据帐号获取账号信息(Email/Mobile 包含密码)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseOutput<AccountOutput> GetAccount(GetAccountInput input);
    }



    public enum Status
    {
        Noraml = 1,
    }

    public class GetAccountInput
    {
        public string Id { get; set; }
        //public Status AccountStatus { get; set; }
    }

    public class BaseOutput<T>
    {

        private string message;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message
        {
            get
            {

                return message;
            }
            set => message = "";
        }

        private T data;
        /// <summary>
        /// 数据
        /// </summary>
        public T Data
        {
            get
            {
                if (typeof(T) == typeof(string) && data == null)
                {
                    return (T)Convert.ChangeType(string.Empty, typeof(T));
                }
                return data;
            }
            set => data = value;
        }
    }
}
