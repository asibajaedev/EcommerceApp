using Blazored.LocalStorage;
using Ecommerce.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.WebAssembly.Extensiones
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public TokenHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sesion = await _localStorage.GetItemAsync<SesionDTO>("sesionUsuario");

            if (sesion != null && !string.IsNullOrWhiteSpace(sesion.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Token);
            }

            return await base.SendAsync(request, cancellationToken);
        }    
    }
}
