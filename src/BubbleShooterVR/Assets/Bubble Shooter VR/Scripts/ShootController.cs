using UnityEngine;
using System.Collections;
using RichJoslin.Pooling;

namespace RichJoslin
{
	namespace BubbleShooterVR
	{
		public class ShootController : MonoBehaviour
		{
			public Transform controllerTransform;
			public BallShot ballShot { get; set; }
			public bool isReady { get; set; }

			public void Awake()
			{
				this.isReady = false;
			}

			public IEnumerator Start()
			{
				while (!PoolManager.I.Finished) yield return null;
				GameMgr.I.shootMgr = this;
				this.isReady = true;
			}

			public void LateUpdate()
			{
				if (this.isReady)
				{
					if (this.ballShot != null)
					{
						// TODO: decouple this so you can choose which input to use
						if (this.ballShot.state == BallShot.State.Loaded &&
							OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.75f)
						{
							this.ballShot.Shoot();
						}
					}
					else
					{
						bool reload = true;

						foreach (BallGrid ballGrid in GameMgr.I.ballGrids)
						{
							if (ballGrid.state != BallGrid.State.Default) reload = false;
						}

						// TODO: decouple this so you can choose which input to use
						if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.25f) reload = false;

						if (reload)
						{
							this.ballShot = BallShot.Generate(this.controllerTransform);
						}
					}
				}
			}
		}
	}
}
