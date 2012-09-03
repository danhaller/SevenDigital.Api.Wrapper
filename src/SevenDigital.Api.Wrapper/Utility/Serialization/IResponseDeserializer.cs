using SevenDigital.Api.Wrapper.Utility.Http;

namespace SevenDigital.Api.Wrapper.Utility.Serialization
{
    public interface IResponseDeserializer<T>
	{
        T Deserialize(Response response);
	}
}