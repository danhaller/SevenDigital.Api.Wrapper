using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.Api.Wrapper.Serialization
{
	public interface IResponseDeserializer<out T>
	{
		T Deserialize(Response response);
	}
}