using UnityEditor;
using UnityEngine;

namespace RichJoslin
{
	namespace Pooling
	{
		[CustomPropertyDrawer(typeof(Poolable))]
		public class PoolableDrawer  : PropertyDrawer
		{
			public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			{
				return label != GUIContent.none && Screen.width < 333 ? (16f + 18f) : 16f;
			}

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				label = EditorGUI.BeginProperty(position, label, property);

				Rect contentPosition = EditorGUI.PrefixLabel(position, label);

				if (Application.isPlaying)
				{
					GameObject prefab = property.FindPropertyRelative("prefab").objectReferenceValue as GameObject;

					if (prefab!=null && PoolManager.I != null && PoolManager.I.GetPooler(prefab) != null)
					{
						float totalWidth = contentPosition.width - 180f;
						float nameWidth = totalWidth * 0.4f;
						float prefabWidth = totalWidth * 0.4f;
						float spawnedWidth = totalWidth * 0.1f;
						float usedWidth = totalWidth * 0.1f;

						GUI.enabled = false;

						contentPosition.width = 26f;
						EditorGUI.LabelField(contentPosition, "Key");
						contentPosition.x += 26f;

						contentPosition.width = nameWidth;
						EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("key"), GUIContent.none);
						contentPosition.x += nameWidth + 15f;

						contentPosition.width = 40f;
						EditorGUI.LabelField(contentPosition, "Prefab");
						contentPosition.x += 40f;

						contentPosition.width = prefabWidth;
						EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("prefab"), GUIContent.none);
						contentPosition.x += prefabWidth + 15f;

						contentPosition.width = 55f;
						EditorGUI.LabelField(contentPosition, "Spawned");
						contentPosition.x += 55f;

						contentPosition.width = spawnedWidth;
						EditorGUI.IntField(contentPosition,  GUIContent.none, PoolManager.I.GetPooler(prefab).poolSize);
						contentPosition.x += spawnedWidth;

						contentPosition.width = 30f;
						EditorGUI.LabelField(contentPosition, "Used");
						contentPosition.x += 30f;

						contentPosition.width = usedWidth;
						EditorGUI.IntField(contentPosition, GUIContent.none, PoolManager.I.GetPooler(prefab).used);
						contentPosition.x += usedWidth;

						GUI.enabled = true;
					}
				}
				else
				{
					float totalWidth = contentPosition.width - 180f;
					float nameWidth = totalWidth * 0.45f;
					float prefabWidth = totalWidth * 0.45f;
					float amountWidth = totalWidth * 0.1f;

					contentPosition.width = 26f;
					EditorGUI.LabelField(contentPosition, "Key");
					contentPosition.x += 26f;

					contentPosition.width = nameWidth;
					EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("key"), GUIContent.none);
					contentPosition.x += nameWidth + 15f;

					contentPosition.width = 40f;
					EditorGUI.LabelField(contentPosition, "Prefab");
					contentPosition.x += 40f;

					contentPosition.width = prefabWidth;
					EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("prefab"), GUIContent.none);
					contentPosition.x += prefabWidth + 15f;

					contentPosition.width = 50f;
					EditorGUI.LabelField(contentPosition, "Amount");
					contentPosition.x += 50f;

					contentPosition.width = amountWidth;
					EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("amountToSpawn"), GUIContent.none);
					contentPosition.x += amountWidth;
				}

				EditorGUI.EndProperty();
			}
		}
	}
}
