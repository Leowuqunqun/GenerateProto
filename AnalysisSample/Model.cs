using System;
using System.Collections.Generic;

namespace ClassLibrary1
{

    public class AccountOutput
    {
        /// <summary>
        /// 创建者
        /// </summary>
        public Account CreateAccount { get; set; }
        /// <summary>
        /// 成员
        /// </summary>
        public List<Member> Members { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }

    public class Account
    {

        public DateTime CreateTime { get; set; }
        public string AccountName { get; set; }
    }

    public class Member
    {
        public Account Accounts { get; set; }
    }
}
