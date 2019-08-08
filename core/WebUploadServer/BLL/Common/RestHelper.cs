using AI3.Server.Common.Entity;
using RestSharp;

namespace WebUploadServer.BLL
{
    public class RestHelper
    {
        public IRestResponse Import(TemporaryEntity xml)
        {
            var request = new RestRequest("/test", Method.POST);
            var client = new RestClient("");
            request.AddXmlBody(xml);
            var response = client.Execute(request);
            return response;
        }
    }
}
