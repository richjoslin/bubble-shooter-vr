using UnityEngine;

namespace RichJoslin
{
	public struct Vector3i
	{
		public int x;
		public int y;
		public int z;

		public Vector3i(int xArg, int yArg, int zArg)
		{
			this.x = xArg;
			this.y = yArg;
			this.z = zArg;
		}
		public Vector3i(Vector3 v1)
		{
			this.x = Mathf.RoundToInt(v1.x);
			this.y = Mathf.RoundToInt(v1.y);
			this.z = Mathf.RoundToInt(v1.z);
		}

		public override string ToString()
		{
			return string.Format("Vector3i({0}, {1}, {2})", this.x, this.y, this.z);
		}

		public static Vector3i operator +(Vector3i v1, Vector3i v2)
		{
			return new Vector3i(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
		}

		public static Vector3i operator +(Vector3 v1, Vector3i v2)
		{
			return new Vector3i(Mathf.RoundToInt(v1.x) + v2.x, Mathf.RoundToInt(v1.y) + v2.y, Mathf.RoundToInt(v1.z) + v2.z);
		}

		public static Vector3i operator +(Vector3i v1, Vector3 v2)
		{
			return new Vector3i(v1.x + Mathf.RoundToInt(v2.x), v1.y + Mathf.RoundToInt(v2.y), v1.z + Mathf.RoundToInt(v2.z));
		}

		public static implicit operator Vector3(Vector3i v1)
		{
			return new Vector3(v1.x, v1.y, v1.z);
		}
	}
}
