using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GenerateProto
{
    public static class TypeConvert
    {

        public static Dictionary<string, Entity> types = new Dictionary<string, Entity>();

        public static Dictionary<string, string> entityTypes = new Dictionary<string, string>();

        public static Dictionary<string, string> responseModels = new Dictionary<string, string>();



        public static string Convert(this PropertyInfo propertyInfo, string fileName = "")
        {

            Type type = propertyInfo.PropertyType;
            string result = string.Empty;
            if (type == typeof(int))
            {
                result = TypeEnum.Int32.ToString().ToLower();
            }
            else if (type == typeof(long) || type == typeof(DateTime))
            {
                result = TypeEnum.Int64.ToString().ToLower();
            }
            else if (type.IsEnum)
            {
                result = TypeEnum.Int32.ToString().ToLower();
            }
            else if (type.IsConstructedGenericType == false && type.IsClass && type.Name != "String")
            {
                //如果是Class 需要获取父类的属性
                if (type.BaseType != null)
                {

                    if (!entityTypes.ContainsKey(type.Name))
                    {
                        string name = "";
                        var entity = TypeConvert.ModelAnalysis(type, fileName, CategoryEnum.Entity, ref name);
                        entityTypes.Add(name, entity);
                    }
                }
                result = type.Name;
            }
            else if (type.IsConstructedGenericType)
            {
                string name = "";

                if (type.GetGenericArguments().Count() > 0 && !entityTypes.ContainsKey(type.GetGenericArguments()[0].Name))
                {
                    TypeConvert.ModelAnalysis(type, fileName, CategoryEnum.Entity, ref name);
                }

                result = "repeated " + name;

            }
            else
            {
                result = type.Name.ToLower();
            }

            return result;
        }

        public static string ModelAnalysis(Type type, string fileName, CategoryEnum category, ref string name, int i = 0)
        {

            if (type.IsConstructedGenericType)
            {
                name = type.GetGenericArguments()[0].Name;
            }
            else
            {
                name = type.Name;

            }

            StringBuilder sb = new StringBuilder();
            sb.Append("message ");

            if (!types.ContainsKey(type.FullName))
            {
                bool isRequest = false;

                if (CategoryEnum.Request == category)
                {
                    isRequest = true;
                    name += "Request";
                    sb.AppendLine(name + "{");

                    sb.Append("\tstring request_id=" + ++i).AppendLine(";");

                }
                else if (CategoryEnum.Response == category)
                {
                    var oldName = name;
                    name += "Response";

                    sb.AppendLine(name + "{");
                    sb.Append("\tint32 code =" + ++i).AppendLine(";");
                    sb.Append("\tstring message =" + ++i).AppendLine(";");
                    sb.Append("\t" + oldName + " data=" + ++i).AppendLine(";");
                    //如果是Class 需要获取父类的属性
                    if (type.BaseType != null)
                    {

                        if (!entityTypes.ContainsKey(type.Name))
                        {
                            string newName = "";
                            var entity = TypeConvert.ModelAnalysis(type, fileName, CategoryEnum.Entity, ref newName);
                            entityTypes.Add(newName, entity);
                            sb.AppendLine("}");
                            return sb.ToString();

                        }
                    }
                }
                else if (CategoryEnum.Entity == category)
                {
                    sb.AppendLine(name + " {");

                }

                for (i = 0; i < type.GetProperties().Length; i++)
                {
                    var property = type.GetProperties()[i];
                    sb.AppendLine("\t" + property.Convert(fileName) + "  " + ParamtersFormat(property.Name) + "=" + (isRequest ? i + 2 : i + 1) + ";");
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
