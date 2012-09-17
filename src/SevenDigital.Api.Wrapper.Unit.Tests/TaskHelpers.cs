using System.Threading.Tasks;

namespace SevenDigital.Api.Wrapper.Unit.Tests
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
