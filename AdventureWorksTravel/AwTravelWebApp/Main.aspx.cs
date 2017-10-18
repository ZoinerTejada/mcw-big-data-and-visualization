
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace PBIWebApp
{

     /* NOTE: This sample is to illustrate how to authenticate a Power BI web app. 
     * In a production application, you should provide appropriate exception handling and refactor authentication settings into 
     * a secure configuration. Authentication settings are hard-coded in the sample to make it easier to follow the flow of authentication. */
    public partial class Main : Page
    {
        string baseUri = "https://api.powerbi.com/beta/myorg/";

        string baseMLUri = "https://{0}.services.azureml.net/workspaces/{1}/services/{2}/execute?api-version=2.0&details=true";

        string baseWeatherUri = "http://api.wunderground.com/api/{0}/hourly10day/q/{1}.json";

        PBIWebApp.Properties.Settings settings = PBIWebApp.Properties.Settings.Default;

        List<Airport> _aiports = null;

        ForecastResult _forecast = null;
        DelayPrediction _prediction = null;

        public AuthenticationResult authResult { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitAirports();

            if (!IsPostBack)
            {
                txtDepartureDate.Text = DateTime.Now.AddDays(5).ToShortDateString();

                ddlOriginAirportCode.DataSource = _aiports;
                ddlOriginAirportCode.DataTextField = "AirportCode";
                ddlOriginAirportCode.DataValueField = "AirportWundergroundID";
                ddlOriginAirportCode.DataBind();

                ddlDestAirportCode.DataSource = _aiports;
                ddlDestAirportCode.DataTextField = "AirportCode";
                ddlDestAirportCode.DataValueField = "AirportWundergroundID";
                ddlDestAirportCode.DataBind();
                ddlDestAirportCode.SelectedIndex = 12;
            }

            //Test for AuthenticationResult
            if (Session["authResult"] != null)
            {
                //Get the authentication result from the session
                authResult = (AuthenticationResult)Session["authResult"];

                //Show Power BI Sign In and Map Panels
                mapPanel.Visible = true;
                signInStatus.Visible = true;

                //Set user and toek from authentication result
                userLabel.Text = authResult.UserInfo.DisplayableId;
                accessTokenTextbox.Text = authResult.AccessToken;

                getDashboards();
            }

            
        }

        protected void btnPredictDelays_Click(object sender, EventArgs e)
        {
            var departureDate = DateTime.Parse(txtDepartureDate.Text);
            departureDate.AddHours(double.Parse(txtDepartureHour.Text));

            var selectedAirport = ddlOriginAirportCode.SelectedItem;

            DepartureQuery query = new DepartureQuery()
            {
                DepartureDate = departureDate,
                DepartureDayOfWeek = ((int)departureDate.DayOfWeek) + 1, //Monday = 1
                Carrier = txtCarrier.Text,
                OriginAirportCode = ddlOriginAirportCode.SelectedItem.Text,
                OriginAirportWundergroundID = ddlOriginAirportCode.SelectedItem.Value,
                DestAirportCode = ddlDestAirportCode.SelectedItem.Text
            };

            
            GetWeatherForecast(query).Wait();

            if (_forecast == null)
            {
                throw new Exception("Forecast request did not succeed. Check Settings for Weather_APIKey.");
            }

            PredictDelays(query, _forecast).Wait();

            UpdateStatusDisplay(_prediction, _forecast);
        }

        private void InitAirports()
        {
            _aiports = new List<Airport>()
            {
                new Airport() {AirportCode ="SEA", AirportWundergroundID="98158.5.99999"},
                new Airport() { AirportCode ="ABQ", AirportWundergroundID="ABQ" },
                new Airport() { AirportCode ="ANC", AirportWundergroundID="ANC" },
                new Airport() { AirportCode ="ATL", AirportWundergroundID="ATL" },
                new Airport() { AirportCode ="AUS", AirportWundergroundID="78742.5.99999" },
                new Airport() { AirportCode ="CLE", AirportWundergroundID="CLE" },
                new Airport() { AirportCode ="DTW", AirportWundergroundID="DTW" },
                new Airport() { AirportCode ="JAX", AirportWundergroundID="32218.18.99999" },
                new Airport() { AirportCode ="MEM", AirportWundergroundID="38131.5.99999" },
                new Airport() { AirportCode ="MIA", AirportWundergroundID="33126.5.99999" },
                new Airport() { AirportCode ="ORD", AirportWundergroundID="60666.5.99999" },
                new Airport() { AirportCode ="PHX", AirportWundergroundID="PHX" },
                new Airport() {AirportCode ="SAN", AirportWundergroundID="92140.5.99999" },
                new Airport() {AirportCode ="SFO", AirportWundergroundID="SFO"},
                new Airport() {AirportCode ="SJC", AirportWundergroundID="SJC"},
                new Airport() {AirportCode ="SLC", AirportWundergroundID="SLC"},
                new Airport() {AirportCode ="STL", AirportWundergroundID="STL"},
                new Airport() {AirportCode ="TPA", AirportWundergroundID="TPA"}
            };
        }

        private void UpdateStatusDisplay(DelayPrediction prediction, ForecastResult forecast)
        {
            weatherForecast.ImageUrl = forecast.ForecastIconUrl;
            weatherForecast.ToolTip = forecast.Condition;

            if (String.IsNullOrWhiteSpace(settings.ML_APIKey))
            {
                lblPrediction.Text = "not configured";
                lblConfidence.Text = "(not configured)";
                return;
            }

            if (prediction == null)
            {
                throw new Exception("Prediction did not succeed. Check the Settings for ML_WorkspaceID, ML_ServiceID, and ML_APIKey.");
            }

            if (prediction.ExpectDelays)
            {
                lblPrediction.Text = "expect delays";
            }
            else
            {
                lblPrediction.Text = "no delays expected";
            }
            lblConfidence.Text = string.Format("{0:N2}", (prediction.Confidence * 100.0));
            
        }

        protected void signInButton_Click(object sender, EventArgs e)
        {
            //Create a query string
            //Create a sign-in NameValueCollection for query string
            var @params = new NameValueCollection
            {
                //Azure AD will return an authorization code. 
                //See the Redirect class to see how "code" is used to AcquireTokenByAuthorizationCode
                {"response_type", "code"},

                //Client ID is used by the application to identify themselves to the users that they are requesting permissions from. 
                //You get the client id when you register your Azure app.
                {"client_id", Properties.Settings.Default.ClientID},

                //Resource uri to the Power BI resource to be authorized
                {"resource", "https://analysis.windows.net/powerbi/api"},

                //After user authenticates, Azure AD will redirect back to the web app
                //{"redirect_uri", "http://localhost:13526/Redirect"}
                {"redirect_uri", PBIWebApp.Properties.Settings.Default.RedirectUrl}
            };

            //Create sign-in query string
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(@params);

            //Redirect authority
            //Authority Uri is an Azure resource that takes a client id to get an Access token
            string authorityUri = "https://login.windows.net/common/oauth2/authorize/";
            Response.Redirect(String.Format("{0}?{1}", authorityUri, queryString));
        }

        private void getDashboards()
        {
            string responseContent = string.Empty;

            //Configure datasets request
            System.Net.WebRequest request = System.Net.WebRequest.Create(String.Format("{0}dashboards", baseUri)) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", String.Format("Bearer {0}", authResult.AccessToken));

            //Get datasets response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();

                    //Deserialize JSON string
                    PBIDashboards PBIDashboards = JsonConvert.DeserializeObject<PBIDashboards>(responseContent);

                    //tb_dashboardsResult.Text = string.Empty;
                    //Find the AdventureWorks dashboard
                    foreach (PBIDashboard db in PBIDashboards.value)
                    {
                        if (db.displayName.ToLower().Equals("adventureworks"))
                        {
                            getTilesForDashboard(db.id);
                        }
                        System.Diagnostics.Debug.Write( String.Format("{0}\t{1}\n", db.id, db.displayName) );
                    }
                }
            }
        }

        private void getTilesForDashboard(string dashboardId)
        {
            string responseContent = string.Empty;

            //Configure datasets request
            System.Net.WebRequest request = System.Net.WebRequest.Create(String.Format("{0}Dashboards/{1}/Tiles", baseUri, dashboardId)) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", String.Format("Bearer {0}", authResult.AccessToken));

            //Get datasets response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();

                    //Deserialize JSON string
                    PBITiles PBITiles = JsonConvert.DeserializeObject<PBITiles>(responseContent);
                    
                    //Find the NumDelays tile
                    foreach (PBITile tile in PBITiles.value)
                    {
                        if (tile.title.ToLower().Equals("numdelays"))
                        {
                            txtIFrameURL.Text = tile.embedUrl;
                        }
                    }
                }
            }
        }

        public class StringTable
        {
            public string[] ColumnNames { get; set; }
            public string[,] Values { get; set; }
        }

        private async Task GetWeatherForecast(DepartureQuery departureQuery)
        {
            DateTime departureDate = departureQuery.DepartureDate;
            string fullWeatherURI = string.Format(baseWeatherUri, settings.Weather_APIKey, departureQuery.OriginAirportWundergroundID);
            _forecast = null;

            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(fullWeatherURI).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject jsonObj = JObject.Parse(result);

                        _forecast = (from f in jsonObj["hourly_forecast"]
                                     where f["FCTTIME"]["year"].Value<int>() == departureDate.Year &&
                                           f["FCTTIME"]["mon"].Value<int>() == departureDate.Month &&
                                           f["FCTTIME"]["mday"].Value<int>() == departureDate.Day &&
                                           f["FCTTIME"]["hour"].Value<int>() == departureDate.Hour
                                     select new ForecastResult()
                                     {
                                         WindSpeed = f["wspd"]["english"].Value<int>(),
                                         Precipitation = f["qpf"]["english"].Value<double>(),
                                         Pressure = f["mslp"]["english"].Value<double>(),
                                         ForecastIconUrl = f["icon_url"].Value<string>(),
                                         Condition = f["condition"].Value<string>()
                                     }).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Failed retrieving weather forecast: " + ex.ToString());
            }
        }

        private async Task PredictDelays(DepartureQuery query, ForecastResult forecast)
        {
            if (String.IsNullOrEmpty(settings.ML_APIKey))
            {
                return;
            }

            string fullMLUri = string.Format(baseMLUri, settings.ML_RegionPrefix, settings.ML_WorkspaceID, settings.ML_ServiceID);
            var departureDate = DateTime.Parse(txtDepartureDate.Text);

            _prediction = new DelayPrediction();

            try
            {
                using (var client = new HttpClient())
                {
                    var scoreRequest = new
                    {
                        Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {
                                    "OriginAirportCode", "Month", "DayofMonth",
                                    "CRSDepHour", "DayOfWeek", "Carrier",
                                    "DestAirportCode", "WindSpeed", "SeaLevelPressure", "HourlyPrecip"
                                },
                                Values = new string[,]
                                        {
                                            {
                                                query.OriginAirportCode, query.DepartureDate.Month.ToString(), query.DepartureDate.Day.ToString(),
                                                query.DepartureDate.Hour.ToString(), query.DepartureDayOfWeek.ToString(), query.Carrier,
                                                query.DestAirportCode,
                                                forecast.WindSpeed.ToString(),
                                                forecast.Pressure.ToString(),
                                                forecast.Precipitation.ToString()
                                            }
                                        }
                            }
                        },
                    },
                        GlobalParameters = new Dictionary<string, string>()
                        {
                        }
                    };

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ML_APIKey);
                    client.BaseAddress = new Uri(fullMLUri);
                    HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject jsonObj = JObject.Parse(result);

                        string prediction = jsonObj["Results"]["output1"]["value"]["Values"][0][10].ToString();
                        string confidence = jsonObj["Results"]["output1"]["value"]["Values"][0][11].ToString();

                        if (prediction.Equals("1"))
                        {
                            _prediction.ExpectDelays = true;
                            _prediction.Confidence = Double.Parse(confidence);
                        }
                        else if (prediction.Equals("0"))
                        {
                            _prediction.ExpectDelays = false;
                            _prediction.Confidence = Double.Parse(confidence);
                        }
                        else
                        {
                            _prediction = null;
                        }

                    }
                    else
                    {
                        _prediction = null;

                        Trace.Write(string.Format("The request failed with status code: {0}", response.StatusCode));

                        // Print the headers - they include the request ID and the timestamp, which are useful for debugging the failure
                        Trace.Write(response.Headers.ToString());

                        string responseContent = await response.Content.ReadAsStringAsync();
                        Trace.Write(responseContent);
                    }
                }
            }
            catch (Exception ex)
            {
                _prediction = null;
                System.Diagnostics.Trace.TraceError("Failed retrieving delay prediction: " + ex.ToString());
            }
        }
    }

    public class ForecastResult
    {
        public int WindSpeed;
        public double Precipitation; 
        public double Pressure;
        public string ForecastIconUrl;
        public string Condition;
    }

    public class DelayPrediction
    {
        public bool ExpectDelays;
        public double Confidence;
    }

    public class DepartureQuery
    {
        public string OriginAirportCode;
        public string OriginAirportWundergroundID;
        public string DestAirportCode;
        public DateTime DepartureDate;
        public int DepartureDayOfWeek;
        public string Carrier;
    }

    public class Airport
    {
        public string AirportCode { get; set; }
        public string AirportWundergroundID { get; set; }
    }
}

