using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Collections;
using IronPython.Hosting;
using IronPython.Modules;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;


/// <summary>
/// Interpreter for IronPython.
/// </summary>
public class Interpreter
{
    /// <summary>
    /// The scope.
    /// 范围
    /// </summary>
    private ScriptScope  Scope;
 
    /// <summary>
    /// The engine.
    /// 引擎
    /// </summary>
    private ScriptEngine Engine;

    /// <summary>
    /// The source.
    /// 源
    /// </summary>
    private ScriptSource Source;

    /// <summary>
    /// The compiled.
    /// 编译
    /// </summary>
    private CompiledCode Compiled;

    /// <summary>
    /// The operation.
    /// 操作
    /// </summary>
    private ObjectOperations Operation;

    /// <summary>
    /// The python class.
    /// python 类
    /// </summary>
    private object PythonClass;

    /// <summary>
    /// Initializes a new instance of the <see cref="Interpreter"/> class.
    /// 
    /// </summary>
    public Interpreter()
    {
        Engine = Python.CreateEngine();
        Scope  = Engine.CreateScope();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Interpreter"/> class.
    /// 初始化一个Interpreter类的新实例
    /// </summary>
    /// <param name="source">Source.</param>
    public Interpreter(string src) : this()
    {
        Compile(src);
    }

    /// <summary>
    /// Compile the specified src.
    /// 编译指定的资源
    /// </summary>
    /// <param name="src">Source.</param>
    public string Compile(string src, Microsoft.Scripting.SourceCodeKind CodeKind =
                                      Microsoft.Scripting.SourceCodeKind.SingleStatement)
    {
        if(src == string.Empty)
            return string.Empty;

        LoadRuntime();


        Source = CodeKind == Microsoft.Scripting.SourceCodeKind.SingleStatement ?
                             Engine.CreateScriptSourceFromString(src, CodeKind) :
                             Engine.CreateScriptSourceFromFile(src);

        ErrorHandle errors = new ErrorHandle();

        MemoryStream stream = new MemoryStream();
        //Set IO Ouput of execution
        //设置执行任务
        Engine.Runtime.IO.SetOutput(stream, new StreamWriter(stream));

        Compiled  = Source.Compile(errors);
        Operation = Engine.CreateOperations();

        try {
            Compiled.Execute(Scope);
            return FormatOutput(ReadFromStream(stream));

        } catch(Exception ex) {
            return Engine.GetService<ExceptionOperations>().FormatException(ex);
        }
    }

    /// <summary>
    /// Formats the output of execution
    /// 格式化执行输出
    /// </summary>
    /// <returns>The output.</returns>
    /// <param name="output">Output.</param>
    private string FormatOutput(string output)
    {
        return string.IsNullOrEmpty(output) ? string.Empty 
        :      string.Join("\n", output.Remove(output.Length-1)
                                       .Split('\n')
                                       .Reverse().ToArray());
    }

    /// <summary>
    /// Reads MemoryStream.
    /// 读取内存流
    /// </summary>
    /// <returns>The from stream.</returns>
    /// <param name="ms">Ms.</param>
    private string ReadFromStream(MemoryStream ms) {

        int length = (int)ms.Length;
        Byte[] bytes = new Byte[ms.Length];
        ms.Seek(0, SeekOrigin.Begin);
        ms.Read(bytes, 0, length);

        return Encoding.GetEncoding("utf-8").GetString(bytes, 0, length);
    }

    /// <summary>
    /// Load runtime Assemblies of Unity3D
    /// 加载Unity3D的运行时程序集
    /// </summary>
    private void LoadRuntime()
    {
        Engine.Runtime.LoadAssembly(typeof(GameObject).Assembly);
    }

    public void AddRuntime<T>()
    {
        Engine.Runtime.LoadAssembly(typeof(T).Assembly);
    }

    public void AddRuntime(Type type)
    {
        Engine.Runtime.LoadAssembly(type.Assembly);
    }

    /// <summary>
    /// Gets the variable or class
    /// 获取变量或类
    /// </summary>
    /// <returns>The variable.</returns>
    /// <param name="name">Name.</param>
    public object GetVariable(string name)
    {
        return Operation.Invoke(Scope.GetVariable(name));
    }

    /// <summary>
    /// Calls the method.
    /// 调用这个方法
    /// </summary>
    /// <param name="name">Name.</param>
    public void InvokeMethod(object nameClass, string Method, params object[] parameters)
    {
        object output = new object();
        if(Operation.TryGetMember(nameClass, Method, out output)) {
            object Func = Operation.GetMember(nameClass, Method);
            Operation.Invoke(Func, parameters);
        }
    }

}
