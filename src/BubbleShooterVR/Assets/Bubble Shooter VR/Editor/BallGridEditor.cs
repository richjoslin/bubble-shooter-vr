using UnityEngine;
using UnityEditor;

namespace RichJoslin
{
	namespace BubbleShooterVR
	{
		[CustomEditor(typeof(BallGrid))]
		public class BallGridEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();

				BallGrid ballGrid = (BallGrid)target;

				if (GUILayout.Button("Generate"))
				{
					BallGridEditor.GenerateRandomBallGrid(ballGrid);
				}

				if (GUILayout.Button("Clear Balls"))
				{
					BallGridEditor.ClearBalls(ballGrid);
				}
			}

			public static void ClearBalls(BallGrid ballGrid)
			{
				for (int z = ballGrid.zMax; z >= ballGrid.zMin; z--)
				{
					for (int y = ballGrid.yMin; y <= ballGrid.yMax; y++)
					{
						int xLeft = ballGrid.xMin;
						int xRight = ballGrid.xMax;

						if (Utils.IsOdd(ballGrid.xSize))
						{
							if ((ballGrid.xSize + 1) % 4 == 0)
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
							DestroyImmediate(ballGrid.GetGridBall(new Vector3i(x, y, z)));
						}
					}
				}
			}

			public static void GenerateWalls(BallGrid ballGrid)
			{
				Transform extentsTF = ballGrid.transform.FindChild("Extents");

				// shift the walls over slightly for any even sized rows (because the grid ends up offset)
				float zShift = ballGrid.zMax + ballGrid.zMin;
				float yShift = ballGrid.yMax + ballGrid.yMin;
				float xShift = ballGrid.xMax + ballGrid.xMin;
				if (ballGrid.xSize % 2 == 0 && xShift == 0) xShift = 1f;
				if ((ballGrid.xSize + 1) % 4 == 0) xShift++;
				extentsTF.transform.localPosition =
					new Vector3(
						xShift * (BallGrid.GRID_SPACING_X / 2f),
						yShift * (BallGrid.GRID_SPACING_Y / 2f),
						zShift * (BallGrid.GRID_SPACING_Z / 2f)
					);

				float zPadding = (ballGrid.zSize / 2f * BallGrid.GRID_SPACING_Z) + 0.68f;
				float yPadding = (ballGrid.ySize / 2f * BallGrid.GRID_SPACING_Y) + 0.63f;
				float xPadding = (ballGrid.xSize / 2f * BallGrid.GRID_SPACING_X) + 0.63f;

				float zWallDim = (ballGrid.zSize + 1f) / 2f;
				float yWallDim = (ballGrid.ySize + 1f) / 2f;
				float xWallDim = (ballGrid.xSize + 1f) / 2f;

				Transform zMinExtentTF = extentsTF.FindChild("z min");
				if (ballGrid.zMinWall)
				{
					zMinExtentTF.transform.localPosition = Vector3.back * zPadding;
					WallCube wallCube = BallGridEditor.GenerateWallCube(zMinExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, yWallDim, 1f);
				}

				Transform zMaxExtentTF = extentsTF.FindChild("z max");
				if (ballGrid.zMaxWall)
				{
					zMaxExtentTF.transform.localPosition = Vector3.forward * zPadding;
					WallCube wallCube = BallGridEditor.GenerateWallCube(zMaxExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, yWallDim, 1f);
				}

				Transform yMinExtentTF = extentsTF.FindChild("y min");
				if (ballGrid.yMinWall)
				{
					yMinExtentTF.transform.localPosition = Vector3.down * yPadding;
					WallCube wallCube = BallGridEditor.GenerateWallCube(yMinExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, 1f, zWallDim);
				}

				Transform yMaxExtentTF = extentsTF.FindChild("y max");
				if (ballGrid.yMaxWall)
				{
					yMaxExtentTF.transform.localPosition = Vector3.up * yPadding;
					WallCube wallCube = BallGridEditor.GenerateWallCube(yMaxExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(xWallDim, 1f, zWallDim);
				}

				Transform xMinExtentTF = extentsTF.FindChild("x min");
				if (ballGrid.xMinWall)
				{
					xMinExtentTF.transform.localPosition = Vector3.left * xPadding;
					WallCube wallCube = BallGridEditor.GenerateWallCube(xMinExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(1f, yWallDim, zWallDim);
				}

				Transform xMaxExtentTF = extentsTF.FindChild("x max");
				if (ballGrid.xMaxWall)
				{
					xMaxExtentTF.transform.localPosition = Vector3.right * xPadding;
					WallCube wallCube = BallGridEditor.GenerateWallCube(xMaxExtentTF, Vector3.zero);
					wallCube.transform.localScale = new Vector3(1f, yWallDim, zWallDim);
				}
			}

			public static void GenerateRandomBallGrid(BallGrid ballGrid)
			{
				for (int z = ballGrid.zMax; z >= ballGrid.zMin; z--)
				{
					Transform wallTF = ballGrid.GetOrCreateLayer(z);
					int yBottom = ballGrid.yMin;
					int yTop = ballGrid.yMax;
					for (int y = yBottom; y <= yTop; y++)
					{
						Transform rowTF = ballGrid.GetOrCreateRow(z, y, wallTF);
						int xLeft = ballGrid.xMin;
						int xRight = ballGrid.xMax;

						if (Utils.IsOdd(ballGrid.xSize))
						{
							if ((ballGrid.xSize + 1) % 4 == 0)
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
							Transform colTF = ballGrid.GetOrCreateColumn(x, rowTF);
							GridBall gridBall = BallGridEditor.GenerateGridBall(colTF);
							gridBall.RefreshGridCoords();
						}
					}
				}
			}

			public static GridBall GenerateGridBall(Transform parentTF)
			{
				GameObject gridBallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Bubble Shooter VR/Prefabs/GridBall.prefab");
				GameObject gridBallGO = (GameObject)PrefabUtility.InstantiatePrefab(gridBallPrefab);
				GridBall gridBall = gridBallGO.GetComponent<GridBall>();
                gridBall.transform.SetParent(parentTF);
				gridBall.transform.localPosition = Vector3.zero;
                gridBall.transform.localScale = Vector3.one * 0.7f;
				gridBall.transform.localRotation = Quaternion.identity;
				gridBall.ballColor = (BallColor)Random.Range(1, (int)BallColor.LENGTH);
				EditorCommon.RefreshBallMaterialByColor(gridBall);
                return gridBall;
			}

			public static WallCube GenerateWallCube(Transform parentTF, Vector3 localPos)
			{
				GameObject wallCubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Bubble Shooter VR/Prefabs/WallCube.prefab");
				GameObject wallCubeGO = (GameObject)PrefabUtility.InstantiatePrefab(wallCubePrefab);
				WallCube wallCube = wallCubeGO.GetComponent<WallCube>();
                wallCube.transform.SetParent(parentTF);
                wallCube.transform.localPosition = localPos;
                wallCube.transform.localScale = Vector3.one;
				wallCube.transform.rotation = Quaternion.identity;
                return wallCube;
			}
		}
	}
}
