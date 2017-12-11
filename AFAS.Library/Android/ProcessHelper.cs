using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AFAS.Library.Android
{
    public class ProcessHelper
    {
        private static Process GetProcess()
        {
            var mProcess = new Process();

            mProcess.StartInfo.CreateNoWindow = true;
            mProcess.StartInfo.UseShellExecute = false;
            mProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            mProcess.StartInfo.RedirectStandardInput = true;
            mProcess.StartInfo.RedirectStandardError = true;
            mProcess.StartInfo.RedirectStandardOutput = true;
            mProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            return mProcess;
        }

        public static RunResult Run(string exePath, string args)
        {
            var result = new RunResult();

            using (var p = GetProcess())
            {
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = args;
                p.Start();

                //获取正常信息
                if (p.StandardOutput.Peek() > -1)
                    result.OutputString = p.StandardOutput.ReadToEnd();

                //获取错误信息
                if (p.StandardError.Peek() > -1)
                    result.OutputString = p.StandardError.ReadToEnd();

                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                p.WaitForExit();

                result.ExitCode = p.ExitCode;
                result.Success = true;
            }

            return result;
        }

        public class RunResult
        {
            /// <summary>
            /// 当执行不成功时，OutputString会输出错误信息。
            /// </summary>
            public bool Success;
            public int ExitCode;
            public string OutputString;
            /// <summary>
            /// 调用RunAsContinueMode时，使用额外参数的顺序作为索引。
            /// 如：调用ProcessHelper.RunAsContinueMode(AdbExePath, "shell", new[] { "su", "ls /data/data", "exit", "exit" });
            /// 果：MoreOutputString[0] = su执行后的结果字符串；MoreOutputString[1] = ls ...执行后的结果字符串；MoreOutputString[2] = exit执行后的结果字符串
            /// </summary>
            public Dictionary<int, string> MoreOutputString;

            public new string ToString()
            {
                var str = new StringBuilder();
                str.AppendFormat("Success:{0}\nExitCode:{1}\nOutputString:{2}\nMoreOutputString:\n", Success, ExitCode, OutputString);
                if (MoreOutputString != null)
                    foreach (var v in MoreOutputString)
                        str.AppendFormat("{0}:{1}\n", v.Key, v.Value.Replace("\r", "\\Ⓡ").Replace("\n", "\\Ⓝ"));
                return str.ToString();
            }
        }
    }
}
