using UnityEngine;

namespace RichJoslin
{
	namespace Pooling
	{
		[System.Serializable]
		public class Poolable
		{
			public string key;
			public GameObject prefab;
			public int amountToSpawn;
		}
	}
}
