using UnityEngine;
using System.Collections.Generic;

namespace RichJoslin
{
    namespace BubbleShooterVR
    {
		public class GameMgr : MonoBehaviour
		{
			private static GameMgr _instance = null;
			public static GameMgr I { get { return _instance; } }

			public Material[] ballMaterials;

			public ShootController shootMgr { get; set; }
			public List<BallGrid> ballGrids { get; set; }

			public void Awake()
			{
				if (GameObject.FindObjectsOfType(this.GetType()).Length > 1)
				{
					Debug.LogWarning("Multiple instances of " + this.GetType().ToString() + " found. Deleting this one.");
					Destroy(this.gameObject);
					return;
				}
				_instance = this;

				this.ballGrids = new List<BallGrid>();
			}
		}
	}
}
