using ClassLibrary1;
using GenerateProto.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateProto.Interface2
{
    interface Interface2
    {
        /// <summary>
        ///  根据帐号获取账号信息(Email/Mobile 包含密码)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseOutput<AccountOutput> GetAccount(GetAccountInput input);
    }

}
