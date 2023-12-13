using RestSharp.Authenticators;
using RestSharp;
using Luval.Framework.Core.Configuration;
using Luval.Framework.Core;
using Luval.GPT.Data;
using Luval.GPT.Utilities;
using Luval.WebGPT.Data.ViewModel;

namespace Luval.WebGPT.Presenter
{
    public class ControllerClientPresenter
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRepository _repository;
        public ControllerClientPresenter(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor, IRepository repository)
        {
            _httpClient = httpClientFactory.CreateClient();
            _contextAccessor = contextAccessor;
            _repository = repository;
        }

        public Task<RestResponse> SendAsync(string route, string method, string payload)
        {
            var webUser = _contextAccessor.HttpContext.User.ToUser();
            var appUser = _repository.GetApplicationUser(webUser.ProviderName, webUser.ProviderKey);
            var token = new ValidationToken(appUser.Id);
            var key = token.Encrypt();

            var url = _contextAccessor.GetBaseUrl();
            var options = new RestClientOptions(_contextAccessor.GetBaseUrl());
            var client = new RestClient(options);
            var request = new RestRequest(route, ParseMethod(method));
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddJsonBody(payload);
            request.AddHeader("AppToken", key);
            return client.ExecuteAsync(request);
        }


        private Method ParseMethod(string method)
        {
            switch (method.ToUpperInvariant())
            {
                case "GET": return Method.Get;
                case "POST": return Method.Post;
                case "PUT": return Method.Put;
                case "DELETE": return Method.Delete;
                default: return Method.Get;
            }
        }
    }
}
