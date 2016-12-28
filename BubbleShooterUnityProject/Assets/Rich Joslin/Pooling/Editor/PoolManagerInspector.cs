using UnityEditor;

namespace RichJoslin
{
	namespace Pooling
	{
		[CustomEditor(typeof(PoolManager))]
		public class PoolManagerInspector : Editor
		{
			public override void OnInspectorGUI()
			{
				this.serializedObject.Update();
				PoolableEditorList.Show(this.serializedObject.FindProperty("poolableTypes"), EditorListOption.Buttons);
				this.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
