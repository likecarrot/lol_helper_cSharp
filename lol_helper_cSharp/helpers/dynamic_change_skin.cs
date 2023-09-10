using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lol_helper_cSharp.helpers
{
    public static class dynamic_change_skin
    {
        public static string version_check = "http://116.63.172.78:5000/api/check_version";
        public static string my_host_dll = "http://116.63.172.78:5000/api/dll_version";
        private static Resources_Helper helper = new Resources_Helper();

        public static async Task<bool> check_isenable(string game_version)
        {
            try
            {
                var client = new HttpClient();
                var data = new { version = game_version };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // 发送 POST 请求并获取响应内容
                HttpResponseMessage response2 = await client.PostAsync(version_check, content);
                string responseContent2 = await response2.Content.ReadAsStringAsync();
                if (response2.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            return false;
        }
        public static async Task<bool> download_dll(string game_version)
        {
            string appdata = helper.GetBaseRunPath() + "\\hid.dll";
            try
            {
                var client = new HttpClient();
                var data = new { version = game_version };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // 发送 POST 请求并获取响应内容
                HttpResponseMessage response2 = await client.PostAsync(my_host_dll, content);
                string responseContent2 = await response2.Content.ReadAsStringAsync();
                if (response2.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response2.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(appdata, imageBytes);
                    return true;
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            return false;
        }
        public static string GetSaveHidPath()
        {
            string result = Getcommandline();
            if (string.IsNullOrEmpty(result))
            {
                return "";
            }

            string gamePath = "";
            int pos = result.IndexOf("--app-log-file-path=");
            if (pos != -1)
            {
                pos += "--app-log-file-path=".Length;
                int end = result.IndexOf("\"", pos);
                if (end != -1)
                {
                    gamePath = result.Substring(pos, end - pos);
                }
            }
            pos = gamePath.IndexOf("/LeagueClient");
            gamePath = gamePath.Substring(0, pos);
            gamePath += "/game/hid.dll";

            return gamePath;
        }
        public static bool open()
        {
            string appdata = helper.GetBaseRunPath() + "\\hid.dll";
            if (helper.FileIsExist(appdata))
            {
                File.Copy(appdata, GetSaveHidPath());
                return true;
            }
            return false;
        }


        public static bool close()
        {
            string appdata = GetSaveHidPath();
            if (helper.FileIsExist(appdata))
            {
                File.Delete(appdata);
                return true;
            }
            return false;
        }

        private static string Getcommandline()
        {
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = "wmic",
                Arguments = "process where caption='LeagueClientUx.exe' get commandline",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                // 从标准输出流中读取执行结果
                string output = process.StandardOutput.ReadToEnd();

                // 等待进程执行完成
                process.WaitForExit();
                // 返回执行结果
                return output;
            }
            return string.Empty;
        }
    }
}
