using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Integration.Tests
{
	public static class TaskHelpers
	{
		public static T Await<T>(this Task<T> task)
		{
			task.Wait();
			return task.Result;
		}
	}
}
