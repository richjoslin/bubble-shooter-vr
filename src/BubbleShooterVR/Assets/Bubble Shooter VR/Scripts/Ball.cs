using UnityEngine;

namespace RichJoslin
{
    namespace BubbleShooterVR
    {
		public enum BallColor : int
		{
			Default = 0,
			Red,
			Blue,
			Yellow,
			Green,
			Orange,
			Violet,
			LENGTH,
		}

		public enum BallType
		{
			Default,
			PaintSplash,
			Bomb,
		}

		public abstract class Ball : MonoBehaviour
		{
			public BallType ballType;
			public BallColor ballColor;
			public GameObject model;
			public Renderer modelRenderer;

			public void SetRandomColor(float a = 1f)
			{
				this.SetColor((BallColor)Random.Range(1, (int)BallColor.LENGTH), a);
			}

			public void SetColor(BallColor inBallColor, float a = 1f)
			{
				this.ballColor = inBallColor;
				this.modelRenderer.sharedMaterial = GameMgr.I.ballMaterials[(int)inBallColor];
				Color matColor = this.modelRenderer.sharedMaterial.color;
				matColor.a = a;
				this.modelRenderer.material.color = matColor;
			}

            public virtual void Recycle()
            {
				this.StopAllCoroutines();
				this.transform.rotation = Quaternion.identity;
				this.SetColor(BallColor.Default);
				this.ballType = BallType.Default;
            }
		}
	}
}
