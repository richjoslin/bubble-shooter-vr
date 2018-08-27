using UnityEngine;
using UnityEditor;

namespace RichJoslin
{
	namespace BubbleShooterVR
	{
		public class EditorCommon
		{
			public static void RefreshBallMaterialByColor(GridBall gridBall)
			{
				string matPath = "Assets/Bubble Shooter VR/Materials";
				switch (gridBall.ballColor)
				{
					case BallColor.Red   : matPath = string.Format("{0}/Ball Red.mat"    , matPath); break;
					case BallColor.Blue  : matPath = string.Format("{0}/Ball Blue.mat"   , matPath); break;
					case BallColor.Yellow: matPath = string.Format("{0}/Ball Yellow.mat" , matPath); break;
					case BallColor.Green : matPath = string.Format("{0}/Ball Green.mat"  , matPath); break;
					case BallColor.Orange: matPath = string.Format("{0}/Ball Orange.mat" , matPath); break;
					case BallColor.Violet: matPath = string.Format("{0}/Ball Violet.mat" , matPath); break;
					default              : matPath = string.Format("{0}/Ball Default.mat", matPath); break;
				}
				gridBall.modelRenderer.material = AssetDatabase.LoadAssetAtPath<Material>(matPath);
			}
		}
	}
}
