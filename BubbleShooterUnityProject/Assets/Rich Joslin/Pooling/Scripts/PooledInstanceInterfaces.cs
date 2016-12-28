
namespace RichJoslin
{
	namespace Pooling
	{
		public interface IPooledInstanceGet
		{
			void OnGet(UnityEngine.Object arg = null);
		}

		public interface IPooledInstanceReturn
		{
			void OnReturn();
		}
	}
}
