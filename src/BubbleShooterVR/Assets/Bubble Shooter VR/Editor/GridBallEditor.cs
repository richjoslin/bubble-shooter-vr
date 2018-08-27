using UnityEngine;
using UnityEditor;

namespace RichJoslin
{
	namespace BubbleShooterVR
	{
		[CustomEditor(typeof(GridBall))]
		public class GridBallEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();

				GridBall gridBall = (GridBall)target;

				if (GUILayout.Button("Refresh Material"))
				{
					EditorCommon.RefreshBallMaterialByColor(gridBall);
				}
			}
		}
	}
}
