using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MeaterSharp
{
    namespace Types
    {
        #region Internal Types
        internal class MeaterResponse
        {
            [JsonProperty]
            public string status = "";
            [JsonProperty]
            public int statusCode = -1;
        }

        internal class MeaterDevicesResponse : MeaterResponse
        {
            [JsonProperty]
            public MeaterDevices? data = null;
        }

        internal class MeaterDeviceResponse : MeaterResponse
        {
            [JsonProperty]
            public MeaterDevice? data = null;
        }

        internal class LoginPayload
        {
            [JsonProperty]
            public string token = "";
            [JsonProperty]
            public string userId = "";
        }

        internal class LoginResponse : MeaterResponse
        {
            [JsonProperty]
            public LoginPayload? data = null;
        }
        #endregion
        #region Public Types
        public class ProbeTemperature
        {
            [JsonProperty("internal")]
            private float internalTemp = 0.0f;
            [JsonProperty("ambient")]
            private float ambientTemp = 0.0f;

            /// <summary>
            /// Function <c>GetInternalTemp</c> returns the internal probe temperature.
            /// </summary>
            /// <returns>the internal probe temperature</returns>
            public float GetInternalTemp() => internalTemp;

            /// <summary>
            /// Function <c>GetAmbientTemp</c> returns the ambient probe temperature.
            /// </summary>
            /// <returns>the ambient probe temperature</returns>
            public float? GetAmbientTemp() => ambientTemp;
        }

        public class CookTemperature
        {
            [JsonProperty]
            private float target = 0.0f;
            [JsonProperty]
            private float peak = 0.0f;
            /// <summary>
            /// Function <c>GetTarget()</c> returns the target temperature for the cook.
            /// </summary>
            /// <returns>the target temperature for the cook</returns>
            public float? GetTarget() => target;
            /// <summary>
            /// Function <c>GetPeak()</c> returns the peak temperature for the cook.
            /// </summary>
            /// <returns>the peak temperature for the cook</returns>
            public float? GetPeak() => peak;
        }

        public class MeaterAuthException : Exception { }

        /// <summary>
        /// <c>MeaterDevices</c> contains an array of <c>MeaterDevice</c>s.
        /// </summary>
        public class MeaterDevices
        {
            [JsonProperty]
            private MeaterDevice[]? devices = null;
            /// <summary>
            /// Function <c>GetDevices()</c> returns an array of <c>MeaterDevice</c>s.
            /// </summary>
            /// <returns>an array of <c>MeaterDevice</c>s</returns>
            public MeaterDevice[] GetDevices() {
                if(devices == null)
                {
                    return new MeaterDevice[0];
                }

                return devices;
            }
        }

        /// <summary>
        /// <c>CookTime</c> represents the elapsed and remaining time for a cook.
        /// </summary>
        public class CookTime
        {
            [JsonProperty]
            public int elapsed = 0;
            [JsonProperty]
            public int remaining = 0;

            /// <summary>
            /// Function <c>GetElapsedTime()</c> returns the elapsed cook time in seconds.
            /// </summary>
            /// <returns>the elapsed cook time in seconds</returns>
            public int? GetElapsedTime() => elapsed;
            /// <summary>
            /// Function <c>GetRemainingTime()</c> returns the remaining cook time in seconds.
            /// When the remaining time is unknown or still calculating, this will be <c>-1</c>.
            /// </summary>
            /// <returns>the elapsed cook time in seconds</returns>
            public int? GetRemainingTime() => remaining;
            /// <summary>
            /// Function <c>GetElapsedTimeMins()</c> returns the elapsed cook time in minutes.
            /// </summary>
            /// <returns>the elapsed cook time in minutes</returns>
            public float GetElapsedTimeMins() { return elapsed / 60.0f; }
            /// <summary>
            /// Function <c>GetRemainingTimeMins()</c> returns the remaining cook time in minutes.
            /// When the remaining time is unknown or still calculating, this will be <c>-1.0f</c>.
            /// </summary>
            /// <returns>the elapsed cook time in minutes</returns>
            public float GetRemainingTimeMins() 
            {
                if(remaining == -1)
                {
                    return -1.0f;
                }

                return remaining / 60.0f; 
            }
            
        }

        /// <summary>
        /// The Cook state.
        /// (Not Started, Configured, Started, Ready For Resting, Resting, Slightly Underdone, Finished, Slightly Overdone, OVERCOOK!)
        /// </summary>
        public enum ECookState
        {
            NotStarted,
            Configured,
            Started,
            ReadyForResting,
            Resting,
            SlightlyUnderdone,
            Finished,
            SlightlyOverdone,
            Overook,
            Unknown
        }

        /// <summary>
        /// <c>MeaterCook</c> represents a cook defined through the Meater mobile app in Meater Cloud.
        /// <see href="https://support.meater.com/faqs/get-started-meater"/>
        /// </summary>
        public class MeaterCook
        {
            [JsonProperty]
            private string id = "";
            [JsonProperty]
            private string name = "";
            [JsonProperty]
            private string state = "";
            [JsonProperty]
            private CookTemperature? temperature = null;
            [JsonProperty]
            private CookTime? time = null;

            /// <summary>
            /// Function <c>GetId()</c> returns the unique Id for the cook.
            /// </summary>
            /// <returns>the unique Id for the cook as a <c>string</c></returns>
            public string? GetId() => id;
            /// <summary>
            /// Function <c>GetName()</c> returns the name of the cook.
            /// </summary>
            /// <returns>the name of the cook.</returns>
            public string? GetName() => name;
            /// <summary>
            /// Function <c>GetCookState()</c> returns the state of the cook.
            /// </summary>
            /// <returns>the current state of the cook as a <c>ECookState</c> enum</returns>
            public ECookState GetCookState() {
                switch (state)
                {
                    case "Not Started":
                        return ECookState.NotStarted;
                    case "Configured":
                        return ECookState.Configured;
                    case "Started":
                        return ECookState.Started;
                    case "Ready For Resting":
                        return ECookState.ReadyForResting;
                    case "Resting":
                        return ECookState.Resting;
                    case "Slightly Underdone":
                        return ECookState.SlightlyUnderdone;
                    case "Finished":
                        return ECookState.Finished;
                    case "Slightly Overdone":
                        return ECookState.SlightlyOverdone;
                    case "OVERCOOK!":
                        return ECookState.Overook;
                }

                return ECookState.Unknown;
            }
            /// <summary>
            /// Function <c>GetCookTemperature()</c> returns a <c>CookTemperature</c> object containing the target and peak temperatures.
            /// </summary>
            /// <returns>a <c>CookTemperature</c> object containing the target and peak temperatures</returns>
            public CookTemperature? GetCookTemperature() => temperature;
            /// <summary>
            /// Function <c>GetCookTime()</c> returns a <c>CookTime</c> object containing the elapsed and remaining time.
            /// </summary>
            /// <returns>a <c>CookTime</c> object containing the elapsed and remaining time</returns>
            public CookTime? GetCookTime() => time;

        }
        /// <summary>
        /// <c>MeaterDevice</c> represents a Meater Cloud device received from the backend.
        /// </summary>
        public class MeaterDevice
        {
            [JsonProperty]
            private string id = "";
            [JsonProperty]
            private ProbeTemperature? temperature = null;
            [JsonProperty]
            private MeaterCook? cook = null;
            [JsonProperty]
            private int updated_at = 0;

            /// <summary>
            /// Function <c>GetId()</c> returns the Id for the given Meater Device.
            /// </summary>
            /// <returns>the <c>Id</c> as a <c>string</c></returns>
            public string GetId() => id;
            /// <summary>
            /// Function <c>GetProbeTemp()</c> returns a <c>ProbeTemperature</c> object containing the internal and ambient temperatures.
            /// </summary>
            /// <returns>a <c>ProbeTemperature</c> object containing the internal and ambient temperatures</returns>
            public ProbeTemperature? GetProbeTemp() => temperature;
            /// <summary>
            /// Function <c>GetCook()</c> returns a <c>MeaterCook</c> object containing data for the currently defined cook in Meater Cloud.
            /// </summary>
            /// <returns>a <c>MeaterCook</c> object containing data for the currently defined cook in Meater Cloud</returns>
            public MeaterCook? GetCook() => cook;
            /// <summary>
            /// Function <c>GetLastUpdateTime()</c> returns the time the device data was last updated at as a UNIX timestamp.
            /// </summary>
            /// <returns>the time the device data was last updated at as a UNIX timestamp</returns>
            public int GetLastUpdateTime() => updated_at;
        }
        #endregion
    }

    public static class Core
    {
        #region Private Members
        private static MeaterInternal? _meater = null;
        private static bool _bAuthorized = false;
        #endregion

        #region Public Functions
        /// <summary>
        /// Function <c>Init</c> initializes MeaterSharp with a Meater Cloud (<paramref name="username"/>) and (<paramref name="password"/>).
        /// This is required to be called before any API requests are made.
        /// </summary>
        /// <param name="username">the Meater Cloud username.</param>
        /// <param name="password">the Meater Cloud password.</param>
        public static async Task<bool> Init(string username, string password)
        {
            if (_meater != null)
            {
                _meater = null;
            }

            _meater = new MeaterInternal();

            _bAuthorized = await _meater.Login(username, password);

            return _bAuthorized;
        }

        /// <summary>
        /// Function <c>IsAuthorized</c> reports if MeaterSharp has authorized with the Meater Cloud backend via <c>Init()</c>.
        /// </summary>
        /// <returns>whether <c>Init</c> has already been called and a JWT token acquired</returns>
        public static bool IsAuthorized()
        {
            return _bAuthorized;
        }

        /// <summary>
        /// Function <c>Init</c> initializes MeaterSharp  with a previously acquired JWT token, bypassing the need to login.
        /// </summary>
        /// <param name="token">The JWT token used for Meater Cloud API requests</param>
        public static void Init(string token)
        {
            _meater = new MeaterInternal(token);

            _bAuthorized = true;
        }

        /// <summary>
        /// Function <c>GetDevices()</c> makes an API Request (<c>[GET]/devices</c>) and returns a <c>MeaterDevices</c> object containing an array of <c>MeaterDevice</c>s, which includes their status, current temperatures, etc.
        /// </summary>
        /// <returns>a <c>MeaterDevices</c> object containing an array of <c>MeaterDevice</c>s</returns>
        /// <exception cref="Types.MeaterAuthException">Throws an exception if MeaterSharp hasn't been authorized with <c>Init()</c>.</exception>
        public static async Task<Types.MeaterDevices> GetDevices()
        {
            if (!_bAuthorized || _meater == null)
            {
                throw new Types.MeaterAuthException();
            }

            return await _meater.GetDevices();
        }

        /// <summary>
        /// Function <c>GetDevices()</c> makes an API Request (<c>[GET]/devices/id</c>) and returns a <c>MeaterDevice</c> object for the given <paramref name="Id"/>, which includes their status, current temperatures, etc.
        /// </summary>
        /// <returns>a <c>MeaterDevice</c> object for the given <paramref name="Id"/></returns>
        /// <exception cref="Types.MeaterAuthException">Throws an exception if MeaterSharp hasn't been authorized with <c>Init()</c>.</exception>
        public static async Task<Types.MeaterDevice> GetDevice(string Id)
        {
            if (!_bAuthorized || _meater == null)
            {
                throw new Types.MeaterAuthException();
            }

            return await _meater.GetDevice(Id);
        }
        #endregion
        #region Internal
        private class MeaterInternal
        {
            private HttpClient? client = null;
            private string jwtToken = "";
            public MeaterInternal() { }

            public MeaterInternal(string token)
            {
                jwtToken = token;
            }
            public string GetJwtToken() { return jwtToken; }
            public async Task<bool> Login(string username, string password)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("https://public-api.cloud.meater.com/v1/");
                object login = new { email = username, password = password };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                HttpResponseMessage resp = await client.PostAsync("login", content);

                if (resp.IsSuccessStatusCode)
                {
                    string respContent = await resp.Content.ReadAsStringAsync();
                    Types.LoginResponse? loginResp = JsonConvert.DeserializeObject<Types.LoginResponse>(respContent);
                    if (loginResp != null && loginResp.data != null)
                    {
                        jwtToken = loginResp.data.token;
                    }

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    return true;
                }

                return false;
            }

            private void ValidateResponse(HttpResponseMessage response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new Exception("Bad Request");
                        case HttpStatusCode.Unauthorized:
                            _bAuthorized = false;
                            jwtToken = "";
                            throw new Types.MeaterAuthException();
                        case HttpStatusCode.NotFound:
                            throw new Exception("Not Found");
                        case HttpStatusCode.TooManyRequests:
                            throw new Exception("Rate limited");
                        case HttpStatusCode.InternalServerError:
                            throw new Exception("Internal Server Error");
                    }

                    throw new Exception("Unhandled Http Status");
                }
            }

            public async Task<Types.MeaterDevices> GetDevices()
            {
                if (client != null)
                {
                    HttpResponseMessage resp = await client.GetAsync("devices");

                    ValidateResponse(resp);

                    string stringResp = await resp.Content.ReadAsStringAsync();

                    Types.MeaterDevicesResponse? deviceResp = JsonConvert.DeserializeObject<Types.MeaterDevicesResponse>(stringResp);
                    if (deviceResp != null && deviceResp.data != null)
                    {
                        return deviceResp.data;
                    }
                }

                return new Types.MeaterDevices();
            }

            public async Task<Types.MeaterDevice> GetDevice(string id)
            {
                if (client != null)
                {
                    HttpResponseMessage resp = await client.GetAsync("devices/" + id);

                    ValidateResponse(resp);

                    string stringResp = await resp.Content.ReadAsStringAsync();

                    Types.MeaterDeviceResponse? deviceResp = JsonConvert.DeserializeObject<Types.MeaterDeviceResponse>(stringResp);

                    if (deviceResp != null && deviceResp.data != null)
                    {
                        return deviceResp.data;
                    }
                }

                return new Types.MeaterDevice();
            }
        }
        #endregion

    }
}