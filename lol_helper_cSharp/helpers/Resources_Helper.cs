using lol_helper_cSharp.riot_apis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_helper_cSharp.helpers
{
    /// <summary>
    /// 提供下载游戏数据的接口
    /// </summary>
    public class Resources_Helper
    {
        /// <summary>
        /// 构造时,检查是否存在 %appdata%\lol_helper目录
        /// </summary>

        private string run_folder;
        private string skin_folder;
        private string icon_folder;
        public Resources_Helper()
        {
            string folder=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder +="/lol_helper";
            if (!FolderIsExist(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string skins = folder + "/skins";
            if (!FolderIsExist(skins))
            {
                Directory.CreateDirectory(skins);
            }

            string icon = folder + "/icons";
            if (!FolderIsExist(icon))
            {
                Directory.CreateDirectory(icon);
            }

            skin_folder = skins;
            icon_folder = icon;
            run_folder = folder;
        }

        public async Task<string> DownloadAsync(Consture.gamedata_resources_type type,long id)
        {
            string files=null;
            switch (type)
            {
                case Consture.gamedata_resources_type.ICON:
                    files = string.Format("{0}/icons_{1}.jpg",icon_folder ,id);
                    break;
                case Consture.gamedata_resources_type.CHAMP_ICON:
                    files = string.Format("{0}/champion_icons_{1}.jpg",icon_folder, id);
                    break;
                case Consture.gamedata_resources_type.SKIN:
                    files = string.Format("{0}/skins_{1}.jpg",skin_folder ,id);
                    if (id==0)
                    {
                        id = 37045;
                    }
                    break;
            }
            if (!FileIsExist(files)||!FileIsZeroSize(files))
            {
                RiotApiManager apiManager = RiotApiManager.GetInstance();
                var content= await apiManager.DownloadIcon(type, id);
                File.WriteAllBytes(files, content);
            }
            return files;
        }

        /// <summary>
        /// 检查是不是存在 game_config.json 用于应用当前玩家的设置
        /// </summary>
        /// <returns></returns>
        public bool IsExistGameConfig()
        {
            string path = run_folder + "/game_config.json";
            return FileIsExist(path);
        }
        public string GetGameConfig()
        {
            string path = run_folder + "/game_config.json";
            return File.ReadAllText(path);
        }
        public void SaveGameConfig(string json_content)
        {
            string path = run_folder + "/game_config.json";
            File.WriteAllText(path, json_content);
        }

        public  string GetBaseRunPath()
        {
            if (!string.IsNullOrEmpty(run_folder))
            {
                return run_folder;
            }
            return default(string);
        }
        public bool FileIsExist(string path)
        {
            return File.Exists(path);
        }
        public bool FolderIsExist(string path)
        {
            return Directory.Exists(path);

        }

        public bool FileIsZeroSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            bool ret = fileInfo.Length > 0;
            return ret;
        }

        
    }
}
