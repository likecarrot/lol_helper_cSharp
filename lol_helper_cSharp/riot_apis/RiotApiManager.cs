using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using lol_helper_cSharp.riot_apis;
using QuickType;
using static lol_helper_cSharp.riot_apis.Consture;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace lol_helper_cSharp.riot_apis
{
    public class RiotApiManager
    {
        private static RiotApiManager instance;
        private bool _status;
        private CancellationTokenSource _cancellationTokenSource;
        private string _BasicAuth;
        private int _BasicPort;
        private string _base_url = "https://127.0.0.1:";
        private RiotApiManager()
        {
            _status = false;
            _cancellationTokenSource = null;
            _BasicAuth = string.Empty;
        }

        public SummonerInfo current_summoner;
        public bool Status => _status;
        public string BasicAuth => _BasicAuth;
        public int BasicPort => _BasicPort;
        public static RiotApiManager GetInstance()
        {
            if (instance == null)
            {
                instance = new RiotApiManager();
            }
            return instance;
        }

        public void SetStatus(bool status)
        {
            _status = status;
        }

        public async Task<bool> GetAndProcessCommandLine()
        {
            _BasicAuth = string.Empty;
            _BasicPort = 0;
            // 创建新的CancellationTokenSource和CancellationToken
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            try
            {
                // 执行异步操作获取命令行
                string output = await ExecuteWmicCommandAsync(cancellationToken);
                // 处理命令行
                ProcessOutput(output);
                // 更新状态
                if (string.IsNullOrEmpty(_BasicAuth))
                    _status = false;
                else
                    _status = true;
                return _status;
            }
            catch (OperationCanceledException)
            {
                // 操作被取消
                _BasicAuth = string.Empty;
                _status = false;
            }
            return _status;
        }

        public Task<string> ExecuteWmicCommandAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
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
                using (var reader = process.StandardOutput)
                {
                    var outputBuilder = new StringBuilder();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        outputBuilder.AppendLine(line);
                    }

                    return outputBuilder.ToString().Trim();
                }
            }, cancellationToken);
        }
        private void ProcessOutput(string output)
        {
            int authTokenPos = output.IndexOf("--remoting-auth-token=");
            if (authTokenPos != -1)
            {
                authTokenPos += "--remoting-auth-token=".Length;
                int authTokenEnd = output.IndexOf("\"", authTokenPos);
                if (authTokenEnd != -1)
                {
                    _BasicAuth = output.Substring(authTokenPos, authTokenEnd - authTokenPos);
                }
            }

            int portPos = output.IndexOf("--app-port=");
            if (portPos != -1)
            {
                portPos += "--app-port=".Length;
                int portEnd = output.IndexOf("\"", portPos);
                if (portEnd != -1)
                {
                    _BasicPort = int.Parse(output.Substring(portPos, portEnd - portPos));
                }
            }

            Console.WriteLine("_port:" + _BasicPort + "  _basic:" + _BasicAuth);
        }


        ///网络接口
        ///
        private string GetCredentials()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{_BasicAuth}"));
        }
        private HttpClient CreateHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            HttpClient client = new HttpClient(handler);
            string credentials = GetCredentials();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            return client;
        }


        //接受对局
        public async Task<bool> AcceptGame()
        {
            Console.WriteLine("call function: AcceptGame");

            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;
                    response = await client.PostAsync(_base_url + _BasicPort + ApiUrls.client_accept_matching_api,null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }

        //拒绝对局
        public async Task<bool> DeclineGame()
        {
            Console.WriteLine("call function: DeclineGame" );
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;
                    response = await client.PostAsync(_base_url + _BasicPort + ApiUrls.client_decline_matching_api,null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }

        //游戏结束时,自动再来一局,但是不会自动开启匹配,只是进入房间
        public async Task<bool> NextGame()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    // 添加Basic Authentication头部信息
                    HttpResponseMessage response;
                    response = await client.PostAsync(_base_url + _BasicPort + ApiUrls.auto_next_game_api,null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }

        //游戏结束时,自动再来一局,但是不会自动开启匹配,这个是进入匹配的
        public async Task<bool> SearchGame()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;
                    response = await client.PostAsync(_base_url + _BasicPort + ApiUrls.search_game_api,null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }

        public async Task<SummonerInfo> GetSummonerInfo(string summonerid = "")
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;

                    if (string.IsNullOrEmpty(summonerid))
                    {
                        response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.current_client_player_summoner_info_api);
                    }
                    else
                    {
                        response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.summonerid_get_summonerinfo_api+ summonerid);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var topLevel = SummonerInfo.FromJson(responseContent);
                        if (string.IsNullOrEmpty(summonerid))
                        {
                            current_summoner = topLevel;
                        }
                        return topLevel;
                    }
                    else
                    {
                        Console.WriteLine("HTTP请求返回非成功状态码: " + response.StatusCode);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(SummonerInfo);
        }

        public  async   Task<SummonerInfo>  GetSummonerInfoByName(string player_name = "")
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;

                    if (string.IsNullOrEmpty(player_name))
                    {
                        response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.current_client_player_summoner_info_api);
                    }
                    else
                    {
                        response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.gamename_get_summonerinfo_api + player_name);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var topLevel = SummonerInfo.FromJson(responseContent);
                        if (string.IsNullOrEmpty(player_name))
                        {
                            current_summoner = topLevel;
                        }
                        return topLevel;
                    }
                    else
                    {
                        Console.WriteLine("HTTP请求返回非成功状态码: " + response.StatusCode);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(SummonerInfo);
        }

        public async Task<ProfileSkin> GetProfileSkin()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;
                    response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_profile_skin);
                    if (response.IsSuccessStatusCode)
                    {
                        var ret = ProfileSkin.FromJson(await response.Content.ReadAsStringAsync());
                        return ret;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(ProfileSkin);
        }

        public async Task<byte[]> DownloadIcon(Consture.gamedata_resources_type type, long id)
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;
                    if (type == gamedata_resources_type.ICON)
                        response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_gamedata_icons + id + ".jpg");
                    else if (type == gamedata_resources_type.CHAMP_ICON)
                        response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_gamedata_champicons + id + ".png");
                    else if (type == gamedata_resources_type.SKIN)
                    {
                        long qu = id / 1000;
                        string url = string.Format(_base_url + _BasicPort + ApiUrls.get_champ_info, qu);
                        response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var champinfo = ChampInfo.FromJson(await response.Content.ReadAsStringAsync());
                            foreach (var item in champinfo.Skins)
                            {
                                if (item.Id == id)
                                {
                                    response = await client.GetAsync(_base_url + _BasicPort + item.LoadScreenPath);
                                    break;
                                }
                            }
                        }
                    }
                    else
                        response = new HttpResponseMessage();
                    if (response.IsSuccessStatusCode)
                    {
                        string contentType = response.Content.Headers.ContentType?.MediaType;
                        if (contentType != null && (contentType.Equals("image/jpeg") || contentType.Equals("image/png")))
                        {
                            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                            return imageBytes;
                        }
                    }
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        public async Task<Consture.client_status> GetClientStatus()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response;
                    response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.client_get_player_status_api);
                    if (response.IsSuccessStatusCode)
                    {
                        string contentType = await response.Content.ReadAsStringAsync();
                        return Consture.GetGameStatus(contentType);
                    }
                    return client_status.Error;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return client_status.Error;
        }

        public async Task<bool> SetLockChamp(int champ_id, bool completed)
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(_base_url + _BasicPort + ApiUrls.matching_get_myteam_summonerinfo_api);
                    if (response.IsSuccessStatusCode)
                    {
                        var champselectsession = ChampSelectSession.FromJson(await response.Content.ReadAsStringAsync());
                        LockChamp champ = new LockChamp();
                        champ.ChampionId = champ_id;
                        champ.Completed = completed;
                        HttpMethod patchMethod = new HttpMethod("PATCH");
                        HttpRequestMessage request = new HttpRequestMessage(patchMethod, _base_url + _BasicPort + ApiUrls.lock_champ_completed_api + champselectsession.LocalPlayerCellId);
                        request.Content = new StringContent(champ.ToString(), Encoding.UTF8, "application/json");
                        response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            var topLevel = SummonerInfo.FromJson(responseContent);
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }

        public async Task<bool> BenchChamp(int champ_id)
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(_base_url + _BasicPort + ApiUrls.aram_swap_champ_api + champ_id, null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var topLevel = SummonerInfo.FromJson(responseContent);
                        return true;
                    }
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }

        public async Task<ChampSelectSession> GetTeamSessions()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.matching_get_myteam_summonerinfo_api);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var champselectsession = ChampSelectSession.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return champselectsession;
                    }
                    return default(ChampSelectSession);
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(ChampSelectSession);
        }

        public async Task<Rankdatas> GetRankdatasForPuuid(string puuid = "")
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage;
                    if (string.IsNullOrEmpty(puuid))
                    {
                        responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.puuid_get_player_rank_data_api + current_summoner.Puuid);
                    }
                    else
                    {
                        responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.puuid_get_player_rank_data_api + puuid);
                    }

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var rankdatas = Rankdatas.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return rankdatas;
                    }
                    return default(Rankdatas);
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(Rankdatas);
        }

        public async Task<HistoryMatchDatas> GetHistoryMatchingdatas(string puuid, int min, int max)
        {
            string url;
            if (string.IsNullOrEmpty(puuid))
            {
                url = string.Format(_base_url + _BasicPort + ApiUrls.get_matchingdatas, current_summoner.Puuid, min, max);
            }
            else
            {
                url = string.Format(_base_url + _BasicPort + ApiUrls.get_matchingdatas, puuid, min, max);
            }
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage httpResponseMessage = await client.GetAsync(url);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var matchdatas = HistoryMatchDatas.FromJson(await httpResponseMessage.Content.ReadAsStringAsync());
                        return matchdatas;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(HistoryMatchDatas);
        }

        public async Task<TopChamps> GetTopChamps(string accoutid, int limited)
        {
            string url = string.Format(_base_url + _BasicPort + ApiUrls.get_top_champs, accoutid, limited);
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage httpResponseMessage = await client.GetAsync(url);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var topchamps = TopChamps.FromJson(await httpResponseMessage.Content.ReadAsStringAsync());
                        return topchamps;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(TopChamps);
        }

        public async Task<ChampList[]> GetOwnerChamps()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_owner_champions_api);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var champs = ChampList.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return champs;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(ChampList[]);
        }

        public async Task<ChampList[]> GetAllChamps()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_all_champions_api);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var champs = ChampList.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return champs;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(ChampList[]);
        }

        public async Task<GameFlow> GetCurrentGameMode()
        {
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_current_game_mode_api);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var gameflow = GameFlow.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return gameflow;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(GameFlow);
        }

        /// <summary>
        /// 大乱斗使用骰子
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetReroller()
        {
            Console.WriteLine("run:  function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage = await client.PostAsync(_base_url + _BasicPort + ApiUrls.use_reroller_api, null);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }

        public  async   Task<PlayerConfig> GetPlayerConfig()
        {
            Console.WriteLine("run:  function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_current_player_settings);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var gameflow = PlayerConfig.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return gameflow;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(PlayerConfig);
        }
        public  async   void SetPlayerConfig(PlayerConfig   config)
        {
            Console.WriteLine("run:  function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpMethod patchMethod = new HttpMethod("PATCH");

                    HttpRequestMessage request = new HttpRequestMessage(patchMethod, _base_url + _BasicPort + ApiUrls.set_current_player_settings);
                    request.Content = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = await client.SendAsync(request);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var gameflow = PlayerConfig.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return ;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return ;
        }

        public  async   Task<GameVersion> GetGameVersion1()
        {
            Console.WriteLine("run:  function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(_base_url + _BasicPort + ApiUrls.get_game_version);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var gameflow = GameVersion.FromJson(await responseMessage.Content.ReadAsStringAsync());
                        return gameflow;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    _status = false;
                }
                Console.WriteLine("HTTP请求异常: " + ex.Message + " function:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return default(GameVersion);
        }
    }
}
