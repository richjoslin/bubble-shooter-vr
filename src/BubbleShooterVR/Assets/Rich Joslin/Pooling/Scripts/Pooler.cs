using UnityEngine;
using System.Collections;

namespace RichJoslin
{
	namespace Pooling
	{
		public class Pooler : MonoBehaviour
		{
			public GameObject prefab;
			public int poolSize;
			public int used;
			public string key;

			public bool IsEmpty
			{
				get
				{
					return this.transform.childCount <= 0;
				}
			}

			public bool IsFull
			{
				get
				{
					return this.gameObject.transform.childCount >= this.poolSize;
				}
			}

			public IEnumerator InitializeWithYield(Poolable poolable)
			{
				this.key = poolable.key;
				this.prefab = poolable.prefab;
				this.poolSize = poolable.amountToSpawn;
				while (this.transform.childCount < poolable.amountToSpawn)
				{
					this.ReturnOrAdd(Instantiate(poolable.prefab) as GameObject);

					if (PoolManager.I.yieldForFrameRate &&
					    (System.DateTime.UtcNow-PoolManager.I.lastYieldTime).TotalMilliseconds > PoolManager.I.TargetFrameTime)
					{
						yield return null;
						PoolManager.I.lastYieldTime = System.DateTime.UtcNow;
					}
				}
			}

			public void Initialize(Poolable poolable)
			{
				this.key = poolable.key;
				this.prefab = poolable.prefab;
				this.poolSize = poolable.amountToSpawn;
				while (this.transform.childCount < poolable.amountToSpawn)
				{
					this.ReturnOrAdd(Instantiate(poolable.prefab) as GameObject);
				}
			}

			public GameObject GetFirstInstance()
			{
				GameObject go = this.transform.GetChild(0).gameObject;
				go.transform.SetParent(null, false);
				return go;
			}

			public void Clear()
			{
				while (this.transform.childCount > 0)
				{
					GameObject go = this.transform.GetChild(0).gameObject;
					go.transform.SetParent(null, false);
					Destroy(go);
				}

				this.poolSize = 0;
				this.used = 0;
			}

			public void ReturnOrAdd(MonoBehaviour monoBehaviour)
			{
				if (monoBehaviour == null) return;
				this.ReturnOrAdd(monoBehaviour.gameObject);
			}

			public void ReturnOrAdd(GameObject go)
			{
				if (this != null && go != null && go.transform != null)
				{
					if (PoolManager.I.instanceToPooler.ContainsKey(go))
					{
						this.used -= 1;
						PoolManager.I.instanceToPooler.Remove(go);
					}

					IPooledInstanceReturn ipir = (IPooledInstanceReturn)go.GetComponent(typeof(IPooledInstanceReturn));
					if (ipir != null) ipir.OnReturn();

					go.transform.SetParent(this.gameObject.transform, false);
					go.transform.localPosition = Vector3.zero;

					go.SetLayerRecursively(PoolManager.I.gameObject.layer);

					go.SetActive(false);
				}
			}

			public GameObject Get(
				Vector3 position = default(Vector3),
				Quaternion rotation = default(Quaternion),
				Vector3 scale = default(Vector3),
				Transform parent = null,
				int layerMaskIndex = 0,
				Object arg = null)
			{
				GameObject go = null;

				if (this.IsEmpty)
				{
					if (!this.prefab)
					{
						return null;
					}
					else
					{
						this.poolSize += 1;
						go = (GameObject)Instantiate(this.prefab);
					}
				}
				else
				{
					go = this.GetFirstInstance();
				}

				if (go != null)
				{
					go.SetActive(true);

					if (parent != null) go.transform.SetParent(parent);

					go.transform.position = position;
					go.transform.rotation = rotation;
					if (scale == Vector3.zero) scale = Vector3.one; // this may cause issues if we want to to get an instance with scale 0
					go.transform.localScale = scale;

					if (layerMaskIndex >= 0) go.SetLayerRecursively(layerMaskIndex);

					IPooledInstanceGet ipig = (IPooledInstanceGet)go.GetComponent(typeof(IPooledInstanceGet));

					if (ipig != null)
					{
						if (arg!=null) ipig.OnGet(arg);
						else ipig.OnGet();
					}

					this.used += 1;

					PoolManager.I.instanceToPooler[go] = this;
				}

				return go;
			}

			public T Get<T>(
				Vector3 position = default(Vector3),
				Quaternion rotation = default(Quaternion),
				Vector3 scale = default(Vector3),
				Transform parent = null,
				int layerMaskIndex = 0,
				Object arg = null
			) where T : UnityEngine.Component
			{
				return this.Get(position, rotation, scale, parent, layerMaskIndex, arg).GetComponent<T>();
			}
		}
	}
}
