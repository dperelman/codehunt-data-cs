using System;

using Newtonsoft.Json;
using RestSharp;

namespace codehunt.rest
{
    public class CodeHuntClient
    {
        private readonly RestClient client;
        private readonly string clientId, clientSecret;

        public CodeHuntClient(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            if (clientId == null || clientSecret == null)
            {
                throw new Exception("Please request a client_id and client_secret from codehunt@microsoft.com.");
            }

            this.client = new RestClient("https://api.codehunt.com/api/");
            this.client.AddDefaultHeader("Authorization", this.GetAuthHeader());
        }

        private struct TokenReponse
        {
            public string access_token;
        }

        private string GetAuthHeader()
        {
            var request = new RestRequest("token", Method.POST);
            request.AddParameter("grant_type", "client_credentials",
                type: ParameterType.QueryString);
            request.AddParameter("client_id", this.clientId,
                type: ParameterType.QueryString);
            request.AddParameter("client_secret", this.clientSecret,
                type: ParameterType.QueryString);
            return "Bearer " + JsonConvert.DeserializeObject<TokenReponse>(client.Execute(request).Content).access_token;
        }

        public struct Program
        {
            public string language;
            public string text;

            public static Program FromIExplorable(IExplorable attempt)
            {
                return new Program
                {
                    text = attempt.Text,
                    language = attempt.Language.ToCodeHuntAPIString(),
                };
            }
        }

        private struct ExplorationBody
        {
            public Program program;
            public string challengeId;

            public static ExplorationBody FromIExplorable(IExplorable attempt)
            {
                return new ExplorationBody
                {
                    program = Program.FromIExplorable(attempt),
                    challengeId = attempt.ChallengeId,
                };
            }
        }

        private struct ExplorationResponse
        {
            public string id;
        }


        public Exploration explore(IExplorable attempt, bool wait = false)
        {
            var postRequest = new RestRequest("explorations", Method.POST);
            postRequest.RequestFormat = DataFormat.Json;
            postRequest.AddBody(ExplorationBody.FromIExplorable(attempt));
            string id = JsonConvert.DeserializeObject<ExplorationResponse>(client.Execute(postRequest).Content).id;

            var request = new RestRequest("explorations/" + id, Method.GET);
            var result = client.Execute(request);
            ExplorationJson exp = JsonConvert.DeserializeObject<ExplorationJson>(result.Content);

            while (wait && !exp.isComplete)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                result = client.Execute(request);
                exp = JsonConvert.DeserializeObject<ExplorationJson>(result.Content);
            }

            return new Exploration(attempt, exp);
        }

        public Translation translate(IExplorable attempt, bool wait = false)
        {
            var postRequest = new RestRequest("translate", Method.POST);
            postRequest.AddParameter("language", "CSharp", ParameterType.QueryString);
            postRequest.RequestFormat = DataFormat.Json;
            postRequest.AddBody(Program.FromIExplorable(attempt));
            return new Translation(attempt, JsonConvert.DeserializeObject<TranslationJson>(client.Execute(postRequest).Content));
        }
    }
}

