using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WetMe.Services
{
    public class RestService
    {
        RestClient restClient;

        public RestService()
        {
            var option = new RestClientOptions("http://192.168.1.111:80");
            restClient = new RestClient(option);
        }

        public bool IsConnected()
        {
            bool ret = false;

            var request = new RestRequest("/", Method.Get);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                ret = !String.IsNullOrEmpty(response.Content);
            }

            return ret;
        }

        public int GetWaterLevel()
        {
            int ret = 1024;

            var request = new RestRequest("/water", Method.Get);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                ret = Convert.ToInt32(response.Content);
            }

            return ret;
        }

        public bool SetManualMode()
        {
            bool ret = false;

            var request = new RestRequest("/manual", Method.Post);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                ret = response.Content == "MODE: manual";
            }

            return ret;
        }

        public bool SetTimerMode(int timeout)
        {
            bool ret = false;

            var request = new RestRequest("/timer", Method.Post);
            request.AddParameter("timeout", timeout);

            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                ret = response.Content == "MODE: timer";
            }

            return ret;
        }

        public bool SetSensorMode(int lowLevel)
        {
            bool ret = false;

            var request = new RestRequest("/sensor", Method.Post);
            request.AddParameter("lowLevel", lowLevel);

            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                ret = response.Content == "MODE: sensor";
            }

            return ret;
        }

        public bool StopPump()
        {
            bool ret = false;

            var request = new RestRequest("/off", Method.Post);

            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                ret = response.Content == "MODE: none";
            }

            return ret;
        }

        public string GetMode()
        {
            string ret = "None";

            var request = new RestRequest("/mode", Method.Get);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                ret = response.Content;
            }

            return ret;
        }
    }
}
