using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.CommandLineUtils;

namespace GenerateProto
{
    public class Program
    {

        static void Main(string[] args)
        {
            /**
             * exe
             */
            string arg = string.Empty;
            var app = new CommandLineApplication();
            app.Command("path", (command) =>
            {
                command.Description = "配置解析路径";
                command.HelpOption("-?|-h|--help");

                var result = command.Argument("[path]",
                                           "", false);
                command.OnExecute(() =>
                {
                    arg = result.Value;

                    Console.WriteLine(result.Value);
                    return 1;
                });
            });

            app.HelpOption("-? | -h | --help");
            app.Execute(args);

            if (!string.IsNullOrEmpty(arg))
            {
                var exePath = Path.GetDirectoryName(Assembly.LoadFrom(arg).CodeBase);
                Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
                var appRoot = appPathMatcher.Match(exePath).Value + @"\protos";
                if (!Directory.Exists(appRoot))
                {
                    Directory.CreateDirectory(appRoot);
                    string fileName = string.Empty;

                    var interfaces = Assembly.LoadFrom(arg).DefinedTypes;

                    foreach (var item in interfaces)
                    {
                        if (item.IsInterface)
                        {
                            List<string> methods = new List<string>();
                            LinkedList<string> models = new LinkedList<string>();
                            fileName = appRoot + @"\" + item.Name + ".proto";
                            FileStream fs = new FileStream(fileName, FileMode.Create);
                            StreamWriter sw = new StreamWriter(fs);
                            List<Entity> header = new List<Entity>()
                            {
                                new Entity(){ Value="syntax = \"proto3\";"},
                                new Entity(){ Value="package " + item.Name + "Service;"}
                            };
                            header.ForEach(a => sw.WriteLine(a.Value));
                            foreach (var method in item.GetMethods())
                            {
                                var requestModelName = string.Empty;
                                var responseModelName = string.Empty;
                                Type type = null;

                                if (method.GetParameters()[0].ParameterType.IsGenericType)
                                {
                                    type = method.GetParameters()[0].ParameterType;
                                    var result = TypeConvert.ModelAnalysis(type, fileName, true, ref requestModelName);
                                    if (result != null)
                                        models.AddFirst(result);
                                }
                                else
                                {
                                    type = method.GetParameters()[0].ParameterType;
                                    var result = TypeConvert.ModelAnalysis(type, fileName, true, ref requestModelName);
                                    if (result != null)
                                        models.AddLast(result);
                                }

                                if (method.ReturnType.IsGenericType)
                                {
                                    type = method.ReturnType.GetGenericArguments().FirstOrDefault();
                                    var result = TypeConvert.ModelAnalysis(type, fileName, false, ref responseModelName);
                                    if (result != null)
                                        models.AddLast(result);
                                }
                                //else
                                //{
                                //    type = method.ReturnType.ReflectedType;
                                //    var result = TypeConvert.ModelAnalysis(type, fileName, false, ref responseModelName);
                                //    if (result != null)
                                //        models.AddLast(result);
                                //}

                                StringBuilder sb = new StringBuilder();
                                sb.Append("\trpc ").Append(method.Name).Append("(")
                                                .Append(requestModelName).Append(") returns(")
                                                .Append(responseModelName).Append(")").AppendLine("{");
                                sb.AppendLine("\t}");

                                methods.Add(sb.ToString());

                            }
                            sw.WriteLine("service " + item.Name + "Service {");
                           
                            methods.ForEach(a => sw.WriteLine(a));

                            sw.WriteLine("}");

                            models.ToList().ForEach(a => sw.WriteLine(a));

                            //清空缓冲区
                            sw.Flush();
                            //关闭流
                            sw.Close();
                            fs.Close();
                        }
                    }
                }
            }
        }
    }

}
