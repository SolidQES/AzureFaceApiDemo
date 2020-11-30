using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesPOC.Services
{
    public class FaceAPI
    {
        protected FaceClient FaceApiClient { get; private set; }

        public FaceAPI(string endpoint, string apiKey)
        {
            FaceApiClient = new FaceClient(new ApiKeyServiceClientCredentials(apiKey))
            {
                Endpoint = endpoint
            };
        }
    }
}
