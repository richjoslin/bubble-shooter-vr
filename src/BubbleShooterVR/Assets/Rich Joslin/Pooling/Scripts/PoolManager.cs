using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RichJoslin
{
	namespace Pooling
	{
		public class PoolManager : MonoBehaviour
		{
			private static PoolManager _instance = null;
			public static PoolManager I { get { return _instance; } }

			public Poolable[] poolableTypes;

			public Dictionary<string, Pooler> keyToPooler { get; set; }
			public Dictionary<GameObject, Pooler> prefabToPooler { get; set; }

			public Dictionary<GameObject, Pooler> instanceToPooler { get; set; }

			public bool Finished { get; set; }

			public bool yieldForFrameRate = false;
			public System.DateTime lastYieldTime;

			public float TargetFrameTime { get { return 120f; } }

			public void Awake()
			{
				if (GameObject.FindObjectsOfType(this.GetType()).Length > 1)
				{
					Debug.LogWarning("Multiple instances of " + this.GetType().ToString() + " found. Deleting this one.");
					Destroy(this.gameObject);
				}
				_instance = this;

				this.Finished = false;

				this.keyToPooler = new Dictionary<string, Pooler>();
				this.prefabToPooler = new Dictionary<GameObject, Pooler>();
				this.instanceToPooler = new Dictionary<GameObject, Pooler>();

				Init();
			}

			public void Init()
			{
				StartCoroutine("CRInit");
			}

			public IEnumerator CRInit()
			{
				this.lastYieldTime = System.DateTime.UtcNow;

				foreach (Poolable poolable in this.poolableTypes)
				{
					GameObject newPoolerGO = new GameObject();
					newPoolerGO.SetLayerRecursively(PoolManager.I.gameObject.layer);
					newPoolerGO.name = poolable.prefab.name;
					newPoolerGO.transform.parent = this.transform;
					newPoolerGO.transform.localPosition = Vector3.zero;

					Pooler pooler = newPoolerGO.AddComponent<Pooler>();
					yield return StartCoroutine(pooler.InitializeWithYield(poolable));

					this.keyToPooler[poolable.key] = pooler;
					this.prefabToPooler[poolable.prefab] = pooler;
				}
				this.Finished = true;
			}

			public Pooler GetPooler(string inPoolKey)
			{
				inPoolKey = inPoolKey.Replace("(Clone)", "");

				Pooler pooler = null;
				if (this.keyToPooler.ContainsKey(inPoolKey)) pooler = this.keyToPooler[inPoolKey];
				return pooler;
			}

			public Pooler GetPooler(GameObject inPoolPrefab)
			{
				Pooler pooler = null;
				if (this.prefabToPooler.ContainsKey(inPoolPrefab))
				{
					pooler = this.prefabToPooler[inPoolPrefab];
				}
				// if passing in a prefab which doesn't have a pooler yet, create one
				else
				{
					GameObject newPoolerGO = new GameObject();
					newPoolerGO.SetLayerRecursively(PoolManager.I.gameObject.layer);
					newPoolerGO.name = inPoolPrefab.name;
					newPoolerGO.transform.parent = this.transform;
					newPoolerGO.transform.localPosition = Vector3.zero;

					Poolable poolable = new Poolable();
					poolable.key = inPoolPrefab.name;
					poolable.prefab = inPoolPrefab;
					poolable.amountToSpawn = 1;

					pooler = newPoolerGO.AddComponent<Pooler>();
					pooler.Initialize(poolable);
				}
				return pooler;
			}

			Poolable GetPoolableInfoByKey(string key)
			{
				foreach (Poolable p in this.poolableTypes)
				{
					if (p.key == key) return p;
				}
				return null;
			}

			Poolable GetPoolableInfoByPrefab(GameObject prefab)
			{
				foreach (Poolable p in this.poolableTypes)
				{
					if (p.prefab == prefab) return p;
				}
				return null;
			}

			public void ReturnAll()
			{
				foreach (GameObject go in this.instanceToPooler.Keys.ToList())
				{
					Return(go);
				}
			}

			public void Return(GameObject go)
			{
				if (go == null) return;
				if (!this.instanceToPooler.ContainsKey(go))
				{
					// TODO: currently anything being returned to the pooler that was not spawned by the pooler will just be destroyed
					// need to link the component type or prefab name or something so I can return anything as long as there is a pooler for it

					Debug.LogWarningFormat("PoolManager.Recycle() : Couldn't return {0}. There was no reference to a pooler for this object.", go.name);
					DestroyImmediate(go);
					return;
				}
				this.instanceToPooler[go].ReturnOrAdd(go);
			}

			public void Return(MonoBehaviour mb)
			{
				if (mb == null) return;
				this.Return(mb.gameObject);
			}

			public void Return(Transform tf)
			{
				if (tf == null) return;
				this.Return(tf.gameObject);
			}
		}
	}
}
