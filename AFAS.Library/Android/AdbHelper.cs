using System;
using System.Collections.Generic;
using System.IO;

namespace AFAS.Library.Android
{

    public class AdbHelper
    {

        // static ILog m_log = LogManager.GetLogger(typeof(AdbHelper));

        /// <summary>
        /// adb.exe文件的路径，默认相对于当前应用程序目录取。
        /// </summary>
        public static string AdbExePath = "adb.exe";

        //小米盒子adb
        //static string ShellTemplate = " -s {0} shell \"su -c '{1}'\"";

        //模拟器adb
        static string ShellTemplate = " -s {0} shell su -c {1}";
        /// <summary>
        /// 当前ADB状态：
        /// adb get-state                - prints: offline | bootloader | device | unknown
        /// </summary>
        public enum AdbState
        {
            Offline, Bootloader, Device, Unknown, Error
        }

        /// <summary>
        /// 获取设备状态；多态设备时，获取的状态始终为unknwon
        /// adb get-state                - prints: offline | bootloader | device | unknown
        /// </summary>
        public static AdbState GetState()
        {
            //获取设备名称
            var result = ProcessHelper.Run(AdbExePath, "get-state");

            System.Diagnostics.Debug.WriteLine(result.ToString());

            if (result.Success)
            {
                switch (result.OutputString.Trim())
                {
                    case "offline":
                        return AdbState.Offline;
                    case "bootloader":
                        return AdbState.Bootloader;
                    case "device":
                        return AdbState.Device;
                    case "unknown":
                        return AdbState.Unknown;
                }
            }
            return AdbState.Error;
        }

        /// <summary>
        /// 启动ADB服务
        /// </summary>
        /// <returns></returns>
        public static bool StartServer()
        {
            return ProcessHelper.Run(AdbExePath, "start-server").Success;
        }

        /// <summary>
        /// 多态设备时，获取的状态始终为unknwon
        /// </summary>
        /// <returns></returns>
        public static string GetSerialNo()
        {
            return ProcessHelper.Run(AdbExePath, "get-serialno").OutputString.Trim();
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetDevices()
        {
            var result = ProcessHelper.Run(AdbExePath, "devices");

            var itemsString = result.OutputString;
            var items = itemsString.Split(new[] { "$", "#", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var itemsList = new List<String>();
            foreach (var item in items)
            {
                var tmp = item.Trim();

                //第一行不含\t所以排除
                if (tmp.IndexOf("\t") == -1)
                    continue;
                var tmps = item.Split('\t');
                itemsList.Add(tmps[0]);
            }

            itemsList.Sort();

            return itemsList.ToArray();
        }

        /// <summary>
        /// 修改路径，防止出现如&等于shell命令混淆的字符
        /// </summary>
        /// <param name="rawPath"></param>
        /// <returns></returns>
        static string PathNormalize(string rawPath)
        {
            string path = rawPath;
            path = path.Replace("&", "\\&");
            path = path.Replace("|", "\\|");
            path = path.Replace(">", "\\>");
            path = path.Replace(" ", "\\ ");
            return path;
        }

        /// <summary>


        /// 获取原始的文档属性数据
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetProperty(string deviceNo, string path)
        {
            path = PathNormalize(path);
            string cmd = "stat " + path;
            string args = System.String.Format(ShellTemplate, deviceNo, cmd);
            var result = ProcessHelper.Run(AdbExePath, args);

            var items = result.OutputString.Split(new[] { "$", "#", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return items;
        }

        /// <summary>
        /// 运行shell命令
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="cmd"></param>
        public static string[] RunShell(string deviceNo, string cmd)
        {
            string args = System.String.Format(ShellTemplate, deviceNo, cmd);
            var result = ProcessHelper.Run(AdbExePath, args);
            string[] items = new string[0];
            if (result.OutputString == null)
                return items;
            items = result.OutputString.Split(new[] { "$", "#", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return items;
        }

        /// <summary>

        /// 获取某目录下的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string[] ListDataFolder(string deviceNo, string path)
        {
            path = PathNormalize(path);
            string cmd = "ls " + path;
            string args = System.String.Format(ShellTemplate, deviceNo, cmd);
            var result = ProcessHelper.Run(AdbExePath, args);

            string[] items = new string[0];
            if (result.OutputString == null)
                return items;
            // m_log.Info("获取路径结果：" + result.ToString());
            items = result.OutputString.Split(new[] { "$", "#", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return items;
        }

        /// <summary>
        /// 按指定要求搜索文件
        /// </summary>
        /// <param name="pattern">通配搜索符</param>
        /// <param name="path">搜索路径</param>
        /// <param name="type">搜索类型</param>
        /// <returns></returns>
        /// 

        //static string searchFormatAll = "\"find {0} -name \\\"{1}\\\"\"";
        //static string searchFormat = "\"find {0} -type {1} -name \\\"{2}\\\"\"";

        static string searchFormatAll = "find {0} -name '{1}'";
        static string searchFormat = "find {0} -type {1} -name '{2}'";

        public static string[] SearchFiles(string deviceNo, string path, string pattern, char type)
        {
            path = PathNormalize(path);

            string cmd;
            if (type == 'a')
                cmd = string.Format(searchFormatAll, path, pattern);
            else
                cmd = string.Format(searchFormat, path, type, pattern); 


            string args = System.String.Format(ShellTemplate, deviceNo, cmd);
            var result = ProcessHelper.Run(AdbExePath, args);

            var items = result.OutputString.Split(new[] { "$", "#", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return items;
        }

        /// <summary>
        /// 拷贝文件到PC上
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="devPath"></param>
        /// <param name="pcPath"></param>
        /// <returns></returns>
        public static bool CopyFromDevice(string deviceNo, string devPath, string pcPath)
        {
            //使用Pull命令将数据库拷贝到Pc上
            //adb pull [-p] [-a] <remote> [<local>]
            devPath = PathNormalize(devPath);
            var result = ProcessHelper.Run(AdbExePath, string.Format("-s {0} pull -a {1} {2}", deviceNo, devPath, pcPath));
            // m_log.Info("推送PC时结果：" + result.ToString());
            if (!result.Success
                || result.ExitCode != 0
                || (result.OutputString != null && result.OutputString.Contains("failed")))
            {
                return false;
                throw new Exception("pull 执行返回的结果异常：" + result.OutputString);
            }
            return true;
        }

        #region 获取设备相关信息
        /// <summary>
        /// -s 0123456789ABCDEF shell getprop ro.product.brand
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="propKey"></param>
        /// <returns></returns>
        public static string GetDeviceProp(string deviceNo, string propKey)
        {
            var result = ProcessHelper.Run(AdbExePath, string.Format("-s {0} shell getprop {1}", deviceNo, propKey));
            return result.OutputString.Trim();
        }
        /// <summary>
        /// 型号：[ro.product.model]: [Titan-6575]
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public static string GetDeviceModel(string deviceNo)
        {
            return GetDeviceProp(deviceNo, "ro.product.model");
        }
        /// <summary>
        /// 牌子：[ro.product.brand]: [Huawei]
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public static string GetDeviceBrand(string deviceNo)
        {
            return GetDeviceProp(deviceNo, "ro.product.brand");
        }
        /// <summary>
        /// 设备指纹：[ro.build.fingerprint]: [Huawei/U8860/hwu8860:2.3.6/HuaweiU8860/CHNC00B876:user/ota-rel-keys,release-keys]
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public static string GetDeviceFingerprint(string deviceNo)
        {
            return GetDeviceProp(deviceNo, "ro.build.fingerprint");
        }
        /// <summary>
        /// 系统版本：[ro.build.version.release]: [4.1.2]
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public static string GetDeviceVersionRelease(string deviceNo)
        {
            return GetDeviceProp(deviceNo, "ro.build.version.release");
        }
        /// <summary>
        /// SDK版本：[ro.build.version.sdk]: [16]
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public static string GetDeviceVersionSdk(string deviceNo)
        {
            return GetDeviceProp(deviceNo, "ro.build.version.sdk");
        }
        #endregion
    }
}
