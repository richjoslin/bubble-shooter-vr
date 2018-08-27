using UnityEngine;

namespace RichJoslin
{
	public static class ExtensionMethods
	{
		public static void SetLayerRecursively(this GameObject value, int inLayer)
		{
			value.layer = inLayer;

			foreach (Transform child in value.transform)
			{
				SetLayerRecursively(child.gameObject, inLayer);
			}
		}
	}
}
