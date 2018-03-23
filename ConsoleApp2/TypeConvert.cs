using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GenerateProto
{
    public static class TypeConvert
    {

        public static Dictionary<string, Entity> types = new Dictionary<string, Entity>();

        public static string Convert(this PropertyInfo propertyInfo)
        {
            string result = string.Empty;
            if (propertyInfo.PropertyType == typeof(int))
            {
                result = TypeEnum.Int32.ToString();
            }
            else if (propertyInfo.PropertyType == typeof(long) || propertyInfo.PropertyType == typeof(DateTime))
            {
                result = TypeEnum.Int64.ToString();
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                result = TypeEnum.Int32.ToString();
            }
            else if (propertyInfo.PropertyType.IsGenericType)
            {
                result = propertyInfo.PropertyType.Name;
            }
            else
            {
                result = propertyInfo.PropertyType.Name;
            }

            return result.ToLower();
        }

        public static string ModelAnalysis(Type type, string fileName, bool isRequest, ref string name, int i = 1)
        {
            name = type.Name + (isRequest ? "Request" : "Response");

            if (!types.ContainsKey(type.FullName))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("message " + name + "{");

                if (isRequest)
                    sb.Append("\tstring request_id=" + ++i).AppendLine(";");
                else
                {
                    sb.Append("\tint32 code =" + ++i).AppendLine(";");
                    sb.Append("\tstring message =" + ++i).AppendLine(";");
                }

                for (i = 0; i < type.GetProperties().Length; i++)
                {
                    var property = type.GetProperties()[i];
                    sb.AppendLine("\t" + property.Convert() + "  " + ParamtersFormat(property.Name) + "=" + (isRequest ? i + 2 : i + 3) + ";");
                }
                sb.AppendLine("}");

                types.Add(type.FullName, new Entity() { IsCommon = false, Value = sb.ToString(), FileName = fileName });

                return sb.ToString();
            }
            else
            {
                types[type.FullName].IsCommon = true;
                if (types[type.FullName].FileName != fileName)
                    return types[type.FullName].Value;
                else
                    return null;
            }
        }

        public static string ParamtersFormat(string param)
        {
            var reg = Regex.Matches(param, "[A-Z]");
            if (reg.Count > 1)
            {
                param = Regex.Replace(param, "([a-z?])[_ ]?([A-Z])", "$1_$2");
            }
            return param.ToLower();
        }
    }
}
