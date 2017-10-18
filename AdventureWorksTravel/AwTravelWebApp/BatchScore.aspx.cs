using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace PBIWebApp
{
    public partial class BatchScore : System.Web.UI.Page
    {

        string baseUri = "https://{0}.services.azureml.net/workspaces/{1}/services/{2}/jobs";
        PBIWebApp.Properties.Settings settings = PBIWebApp.Properties.Settings.Default;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnSubmitJob_Click(object sender, EventArgs e)
        {
            SubmitJob().Wait();
        }

        protected void btnStartJob_Click(object sender, EventArgs e)
        {
            StartJob().Wait();
        }

        protected void btnCheckJobStatus_Click(object sender, EventArgs e)
        {
            GetJobStatus().Wait();
        }

        private async Task SubmitJob()
        {
            string responseContent = string.Empty;
            
            string submitUri = string.Format(baseUri, settings.ML_RegionPrefix, settings.ML_WorkspaceID, settings.ML_ServiceID);

            System.Net.WebClient webClient = new System.Net.WebClient();

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(";BlobEndpoint.*");
            string blobConnString = regex.Replace(settings.ML_StorageAccount, "");

            using (HttpClient client = new HttpClient())
            {
                var request = new BatchExecutionRequest()
                {

                    Input = new AzureBlobDataReference()
                    {
                        ConnectionString = blobConnString,
                        RelativeLocation = txtSourceRelativeLocation.Text
                    },

                    Outputs = new Dictionary<string, AzureBlobDataReference>()
                    {
                        {
                            "output1",
                            new AzureBlobDataReference()
                            {
                                ConnectionString = blobConnString,
                                RelativeLocation = txtDestRelativeLocation.Text
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ML_APIKey);

                var response = await client.PostAsJsonAsync(submitUri + "?api-version=2.0", request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    //await WriteFailedResponse(response);
                    return;
                }

                string jobId = await response.Content.ReadAsAsync<string>();

                txtJobID.Text = jobId;

            }  
        }

        private async Task StartJob()
        {
            string startUri = string.Format(baseUri, settings.ML_RegionPrefix, settings.ML_WorkspaceID, settings.ML_ServiceID);

            System.Net.WebClient webClient = new System.Net.WebClient();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ML_APIKey);
                var response = await client.PostAsync(startUri + "/" + txtJobID.Text + "/start?api-version=2.0", null).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    txtJobStatus.Text = "Error. " + response.ReasonPhrase;
                    return;
                }
            }
        }

        private async Task GetJobStatus()
        {
            long TimeOutInMilliseconds = 10000;

            string jobStatusUri = string.Format(baseUri, settings.ML_RegionPrefix, settings.ML_WorkspaceID, settings.ML_ServiceID) + "/" + txtJobID.Text + "?api-version=2.0";
            string jobId = txtJobID.Text;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ML_APIKey);

                //Stopwatch watch = Stopwatch.StartNew();
                bool done = false;
                //while (!done)
                //{
                    var response = await client.GetAsync(jobStatusUri).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {

                        txtJobStatus.Text = "Error. " + response.ReasonPhrase;
                        return;
                    }

                    BatchScoreStatus status = await response.Content.ReadAsAsync<BatchScoreStatus>();

                    //if (watch.ElapsedMilliseconds > TimeOutInMilliseconds)
                    //{
                    //    done = true;
                    //    Console.WriteLine(string.Format("Timed out. Deleting job {0} ...", jobId));
                    //    await client.DeleteAsync(jobStatusUri);
                    //}

                    switch (status.StatusCode)
                    {
                        case BatchScoreStatusCode.NotStarted:
                            //Console.WriteLine(string.Format("Job {0} not yet started...", jobId));
                            txtJobStatus.Text = "Job not yet started.";
                            break;
                        case BatchScoreStatusCode.Running:
                            //Console.WriteLine(string.Format("Job {0} running...", jobId));
                            txtJobStatus.Text = "Job is running.";
                            break;
                        case BatchScoreStatusCode.Failed:
                            //Console.WriteLine(string.Format("Job {0} failed!", jobId));
                            //Console.WriteLine(string.Format("Error details: {0}", status.Details));
                            txtJobStatus.Text = "Job failed. Error details" + status.Details;
                            done = true;
                            break;
                        case BatchScoreStatusCode.Cancelled:
                            //Console.WriteLine(string.Format("Job {0} cancelled!", jobId));
                            txtJobStatus.Text = "Job was cancelled";
                            done = true;
                            break;
                        case BatchScoreStatusCode.Finished:
                            done = true;
                            //Console.WriteLine(string.Format("Job {0} finished!", jobId));

                            txtJobStatus.Text = "Job is finished.";
                            
                            break;
                    }

                    //if (!done)
                    //{
                    //    Thread.Sleep(1000); // Wait one second
                    //}
                //}
            }
        }

        public class AzureBlobDataReference
        {
            // Storage connection string used for regular blobs. It has the following format:
            // DefaultEndpointsProtocol=https;AccountName=ACCOUNT_NAME;AccountKey=ACCOUNT_KEY
            // It's not used for shared access signature blobs.
            public string ConnectionString { get; set; }

            // Relative uri for the blob, used for regular blobs as well as shared access 
            // signature blobs.
            public string RelativeLocation { get; set; }

            // Base url, only used for shared access signature blobs.
            public string BaseLocation { get; set; }

            // Shared access signature, only used for shared access signature blobs.
            public string SasBlobToken { get; set; }
        }

        public enum BatchScoreStatusCode
        {
            NotStarted,
            Running,
            Failed,
            Cancelled,
            Finished
        }

        public class BatchScoreStatus
        {
            // Status code for the batch scoring job
            public BatchScoreStatusCode StatusCode { get; set; }


            // Locations for the potential multiple batch scoring outputs
            public IDictionary<string, AzureBlobDataReference> Results { get; set; }

            // Error details, if any
            public string Details { get; set; }
        }

        public class BatchExecutionRequest
        {
            public AzureBlobDataReference Input { get; set; }
            public IDictionary<string, string> GlobalParameters { get; set; }

            // Locations for the potential multiple batch scoring outputs
            public IDictionary<string, AzureBlobDataReference> Outputs { get; set; }
        }


    }
}