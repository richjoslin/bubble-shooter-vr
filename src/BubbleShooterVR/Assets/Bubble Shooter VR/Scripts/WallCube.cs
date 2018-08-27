using UnityEngine;
using RichJoslin.Pooling;

namespace RichJoslin
{
    namespace BubbleShooterVR
    {
		public class WallCube : MonoBehaviour
		{
            public static WallCube Generate(Transform parentTF, Vector3 localPos)
            {
                WallCube wallCube = PoolManager.I.GetPooler("WallCube").Get<WallCube>();
                wallCube.transform.SetParent(parentTF);
                wallCube.transform.localPosition = localPos;
                wallCube.transform.localScale = Vector3.one;
				wallCube.transform.rotation = Quaternion.identity;
                return wallCube;
            }
		}
	}
}
