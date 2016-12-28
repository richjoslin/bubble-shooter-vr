using UnityEngine;
using System.Collections;
using RichJoslin.Pooling;

namespace RichJoslin
{
    namespace BubbleShooterVR
    {
		public class BallGrid : MonoBehaviour
		{

#region Finite State Machine

            public enum State
            {
                Default,
				Generating,
				Popping,
            }
			private State _state = State.Default;
			public State state {
				get {
					return this._state;
				}
			}
			public void SetState(State inState) {
				this._state = inState;
			}
			public void SetStateDeferred(State inState) {
				StartCoroutine(this.ChangeState(inState));
			}
			public IEnumerator ChangeState(State newState) {
				yield return new WaitForEndOfFrame();
				this._state = newState;
			}

#endregion

			public const float GRID_SPACING_Z = 0.35f;
			public const float GRID_SPACING_Y = 0.5f;
			public const float GRID_SPACING_X = 0.5f;
			public const float BALL_GENERATE_DELAY = 0.01f;
			public const float ROW_GENERATE_DELAY = 0.025f;

			public bool autoGenerate = false;
			public int zSize = 1;
			public int ySize = 1;
			public int xSize = 1;
			public bool zMinWall = false;
			public bool zMaxWall = true;
			public bool yMinWall = true;
			public bool yMaxWall = true;
			public bool xMinWall = true;
			public bool xMaxWall = true;

			public int xSizeAdjustedForSpacing { get { return Mathf.CeilToInt(xSize / 2f); } }

			public int zMin { get { return -Mathf.FloorToInt(this.zSize / 2); } }
			public int zMax { get { return this.zMin + this.zSize - 1; } }
			public int yMin { get { return -Mathf.FloorToInt(this.ySize / 2); } }
			public int yMax { get { return this.yMin + this.ySize - 1; } }
			public int xMin { get { return -Mathf.FloorToInt(this.xSizeAdjustedForSpacing / 2); } }
			public int xMax { get { return this.xMin + this.xSizeAdjustedForSpacing - 1; } }

			public IEnumerator Start()
			{
				while (!PoolManager.I.Finished) yield return null;
				GameMgr.I.ballGrids.Add(this);
				if (autoGenerate) this.Generate();
			}

			public void ClearBalls()
			{
				for (int z = this.zMax; z >= this.zMin; z--)
				{
					for (int y = this.yMin; y <= this.yMax; y++)
					{
						int xLeft = this.xMin;
						int xRight = this.xMax;

						if (Utils.IsOdd(xSize))
						{
							if ((xSize + 1) % 4 == 0)
							{
								if ((Utils.IsEven(z) && Utils.IsEven(y)) || (Utils.IsOdd(z) && Utils.IsOdd(y))) xLeft++;
							}
							else
							{
								if ((Utils.IsEven(z) && Utils.IsOdd(y)) || (Utils.IsEven(y) && Utils.IsOdd(z))) xRight--;
							}
						}

						for (int x = xLeft; x <= xRight; x++)
						{
							PoolManager.I.Return(GetGridBall(new Vector3i(x, y, z)));
						}
					}
				}
			}

			public void ClearWalls()
			{
				Transform extentsTF = this.transform.FindChild("Extents");

				Transform zMinExtentTF = extentsTF.FindChild("z min");
				while (zMinExtentTF.childCount > 0)
				{
					PoolManager.I.Return(zMinExtentTF.GetChild(0));
				}
				Transform zMaxExtentTF = extentsTF.FindChild("z max");
				while (zMaxExtentTF.childCount > 0)
				{
					PoolManager.I.Return(zMaxExtentTF.GetChild(0));
				}
				Transform yMinExtentTF = extentsTF.FindChild("y min");
				while (yMinExtentTF.childCount > 0)
				{
					PoolManager.I.Return(yMinExtentTF.GetChild(0));
				}
				Transform yMaxExtentTF = extentsTF.FindChild("y max");
				while (yMaxExtentTF.childCount > 0)
				{
					PoolManager.I.Return(yMaxExtentTF.GetChild(0));
				}
				Transform xMinExtentTF = extentsTF.FindChild("x min");
				while (xMinExtentTF.childCount > 0)
				{
					PoolManager.I.Return(xMinExtentTF.GetChild(0));
				}
				Transform xMaxExtentTF = extentsTF.FindChild("x max");
				while (xMaxExtentTF.childCount > 0)
				{
					PoolManager.I.Return(xMaxExtentTF.GetChild(0));
				}
			}

			public void GenerateWalls()
			{
				Transform extentsTF = this.transform.FindChild("Extents");

				// shift the walls over slightly for any even sized rows (because the grid ends up offset)
				float zShift = zMax + zMin;
				float yShift = yMax + yMin;
				float xShift = xMax + xMin;
				if (xSize % 2 == 0 && xShift == 0) xShift = 1f;
				if ((xSize + 1) % 4 == 0) xShift++;
				extentsTF.transform.localPosition =
					new Vector3(
						xShift * (GRID_SPACING_X / 2f),
						yShift * (GRID_SPACING_Y / 2f),
						zShift * (GRID_SPACING_Z / 2f)
					);

				float zPadding = (zSize / 2f * GRID_SPACING_Z) + 0.68f;
				float yPadding = (ySize / 2f * GRID_SPACING_Y) + 0.63f;
				float xPadding = (xSize / 2f * GRID_SPACING_X) + 0.63f;

				float zWallDim = (this.zSize + 1f) / 2f;
				float yWallDim = (this.ySize + 1f) / 2f;
				float xWallDim = (this.xSize + 1f) / 2f;

				Transform zMinExtentTF = extentsTF.FindChild("z min");
				if (zMinWall)
				{
					zMinExtentTF.transform.localPosition = Vector3.back * zPadding;
					WallCube wallCube = WallCube.Generate(zMinExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, yWallDim, 1f);
				}

				Transform zMaxExtentTF = extentsTF.FindChild("z max");
				if (zMaxWall)
				{
					zMaxExtentTF.transform.localPosition = Vector3.forward * zPadding;
					WallCube wallCube = WallCube.Generate(zMaxExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, yWallDim, 1f);
				}

				Transform yMinExtentTF = extentsTF.FindChild("y min");
				if (yMinWall)
				{
					yMinExtentTF.transform.localPosition = Vector3.down * yPadding;
					WallCube wallCube = WallCube.Generate(yMinExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, 1f, zWallDim);
				}

				Transform yMaxExtentTF = extentsTF.FindChild("y max");
				if (yMaxWall)
				{
					yMaxExtentTF.transform.localPosition = Vector3.up * yPadding;
					WallCube wallCube = WallCube.Generate(yMaxExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, 1f, zWallDim);
				}

				Transform xMinExtentTF = extentsTF.FindChild("x min");
				if (xMinWall)
				{
					xMinExtentTF.transform.localPosition = Vector3.left * xPadding;
					WallCube wallCube = WallCube.Generate(xMinExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(1f, yWallDim, zWallDim);
				}

				Transform xMaxExtentTF = extentsTF.FindChild("x max");
				if (xMaxWall)
				{
					xMaxExtentTF.transform.localPosition = Vector3.right * xPadding;
					WallCube wallCube = WallCube.Generate(xMaxExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(1f, yWallDim, zWallDim);
				}
			}

			public void Generate()
			{
				this.StartCoroutine(this.GenerateLoop());
			}

			public IEnumerator GenerateLoop()
			{
				this.SetState(State.Generating);

				this.GenerateWalls();

				for (int z = this.zMax; z >= this.zMin; z--)
				{
					Transform wallTF = this.GetOrCreateLayer(z);
					int yBottom = this.yMin;
					int yTop = this.yMax;
					for (int y = yBottom; y <= yTop; y++)
					{
						Transform rowTF = this.GetOrCreateRow(z, y, wallTF);
						int xLeft = this.xMin;
						int xRight = this.xMax;

						if (Utils.IsOdd(xSize))
						{
							if ((xSize + 1) % 4 == 0)
							{
								if ((Utils.IsEven(z) && Utils.IsEven(y)) || (Utils.IsOdd(z) && Utils.IsOdd(y))) xLeft++;
							}
							else
							{
								if ((Utils.IsEven(z) && Utils.IsOdd(y)) || (Utils.IsEven(y) && Utils.IsOdd(z))) xRight--;
							}
						}

						for (int x = xLeft; x <= xRight; x++)
						{
							Transform colTF = this.GetOrCreateColumn(x, rowTF);
							GridBall gridBall = GridBall.Generate(colTF);
							gridBall.gridCoords = new Vector3(x, y, z);
						}
						yield return new WaitForSeconds(ROW_GENERATE_DELAY);
					}
				}

				this.SetState(State.Default);
			}

			public Transform GetOrCreateLayer(int z)
			{
				Transform parentTF = this.transform.FindChild("Balls");
				string layerName = string.Format("z {0}", z);
				Transform layerTF = parentTF.FindChild(layerName);
				if (layerTF == null)
				{
					layerTF = new GameObject(layerName).transform;
					layerTF.transform.SetParent(parentTF);
					layerTF.transform.localPosition = new Vector3(0, 0, z) * GRID_SPACING_Z;
					layerTF.transform.localRotation = Quaternion.identity;
				}
				return layerTF;
			}

			public Transform GetOrCreateRow(int z, int y, Transform parentTF)
			{
				string rowName = string.Format("y {0}", y);
				Transform rowTF = parentTF.FindChild(rowName);
				if (rowTF == null)
				{
					rowTF = new GameObject(rowName).transform;
					rowTF.transform.SetParent(parentTF);
					float x = 0;
					if (Mathf.Abs(z % 2) != Mathf.Abs(y % 2)) x = 1f;
					rowTF.transform.localPosition = new Vector3(x, y, 0) * GRID_SPACING_Y;
					rowTF.transform.localRotation = Quaternion.identity;
				}
				return rowTF;
			}

			public Transform GetOrCreateColumn(int x, Transform parentTF)
			{
				string colName = string.Format("x {0}", x);
				Transform colTF = parentTF.FindChild(colName);
				if (colTF == null)
				{
					colTF = new GameObject(colName).transform;
					colTF.transform.SetParent(parentTF);
					colTF.transform.localPosition = new Vector3(x, 0, 0) * GRID_SPACING_X * 2;
					colTF.transform.localRotation = Quaternion.identity;
				}
				return colTF;
			}

			public GridBall GetGridBall(Vector3i gridCoords)
			{
				string layerName = string.Format("z {0}", gridCoords.z);
				Transform layerTF = this.transform.FindChild(layerName);
				if (layerTF != null)
				{
					string rowName = string.Format("y {0}", gridCoords.y);
					Transform rowTF = layerTF.FindChild(rowName);
					if (rowTF != null)
					{
						string colName = string.Format("x {0}", gridCoords.x);
						Transform colTF = rowTF.FindChild(colName);
						if (colTF != null)
						{
							return colTF.GetComponentInChildren<GridBall>();
						}
					}
				}
				return null;
			}

			public static Vector3i GetNeighborCoords(Vector3i hitPos, Vector3i direction)
			{
				Vector3i newPos = hitPos + direction;
				if (direction.x > 0 && Mathf.Abs(hitPos.y % 2) == Mathf.Abs(hitPos.z % 2)) newPos.x--;
				if (direction.x < 0 && Mathf.Abs(hitPos.y % 2) != Mathf.Abs(hitPos.z % 2)) newPos.x++;
				return newPos;
			}
		}
	}
}
