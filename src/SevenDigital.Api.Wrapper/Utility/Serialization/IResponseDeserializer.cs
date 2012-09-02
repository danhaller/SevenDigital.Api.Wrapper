using System.Net.Http;
using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Utility.Serialization
{
    public interface IResponseDeserializer<T>
	{
        Task<T> Deserialize(HttpResponseMessage response);
	}
}