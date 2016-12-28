using UnityEngine;
using System.Collections.Generic;
using RichJoslin.Pooling;

namespace RichJoslin
{
	namespace BubbleShooterVR
	{
		public class GridBall : Ball
		{
			public const float testSphereRadius = 0.42f;
			public const float WALL_TEST_RADIUS = 0.6f;

			public BallGrid ballGrid { get; set; }
			public Vector3 gridCoords { get; set; }
			public List<GridBall> neighbors { get; set; }
			public bool isConnectedToWall { get; set; }

			// for debugging
			void OnDrawGizmos()
			{
				// Gizmos.color = new Color(1f, 1f, 0, 0.5f);
				// Gizmos.DrawSphere(transform.position, WALL_TEST_RADIUS);
			}

			public void Awake()
			{
				this.DetectParentBallGrid();
				this.RefreshGridCoords();
				this.neighbors = this.GetNeighbors();
			}

            public static GridBall Generate(Transform parentTF, bool wasBallShot = false)
            {
				GridBall gridBall = PoolManager.I.GetPooler("GridBall").Get<GridBall>();
                gridBall.transform.SetParent(parentTF);
				gridBall.transform.localPosition = Vector3.zero;
                gridBall.transform.localScale = Vector3.one * 0.7f;
				gridBall.transform.localRotation = Quaternion.identity;
				gridBall.DetectParentBallGrid();

				float a = 1f;
				if (!wasBallShot)
				{
					if (Random.Range(1, 100) == 50) a = 0.5f;
				}
				gridBall.SetRandomColor(a);

				gridBall.neighbors = gridBall.GetNeighbors();
				foreach (GridBall gb in gridBall.neighbors)
				{
					if (!gb.neighbors.Contains(gridBall)) gb.neighbors.Add(gridBall);
				}
                return gridBall;
            }

			public void DetectParentBallGrid()
			{
				// this can be called in Awake, when there is no parent yet
				if (this.transform.parent != null)
				{
					this.ballGrid = this.transform.parent.parent.parent.parent.parent.GetComponent<BallGrid>();
				}
			}

			public void RefreshGridCoords()
			{
				int z = 0;
				int y = 0;
				int x = 0;

				// this can be called in Awake, when there is no parent yet
				if (this.transform.parent != null)
				{
					int.TryParse(this.transform.parent.parent.parent.name.Replace("z ", ""), out z);
					int.TryParse(this.transform.parent.parent.name.Replace("y ", ""), out y);
					int.TryParse(this.transform.parent.name.Replace("x ", ""), out x);
				}

				this.gridCoords = new Vector3i(x, y, z);
			}

			public List<GridBall> GetNeighbors()
			{
				List<Collider> collidersList = new List<Collider>(Physics.OverlapSphere(this.transform.position, testSphereRadius));
				List<GridBall> neighbors = new List<GridBall>();
				for (int i = collidersList.Count - 1; i >= 0; i--)
				{
					GridBall neighborBall = collidersList[i].GetComponent<GridBall>();
					if (neighborBall != null) neighbors.Add(neighborBall);
				}
				return neighbors;
			}

			public void CheckIfConnectedToWall()
			{
				this.isConnectedToWall = false;

				int loopSafety = 0;

				List<GridBall> searched = new List<GridBall>() { this };
				Stack<GridBall> searchStack = new Stack<GridBall>();
				searchStack.Push(this);
				while (searchStack.Count > 0)
				{
					GridBall gb1 = searchStack.Pop();

					// gb1.SetColor(BallColor.Default); // for debugging

					List<Collider> collidersList = new List<Collider>(Physics.OverlapSphere(gb1.transform.position, WALL_TEST_RADIUS));
					for (int i = collidersList.Count - 1; i >= 0; i--)
					{
						WallCube WallCube = collidersList[i].GetComponent<WallCube>();
						if (WallCube != null)
						{
							gb1.isConnectedToWall = true;
							this.isConnectedToWall = true;
							return;
						}
					}

					foreach (GridBall gb2 in gb1.neighbors)
					{
						if (!searched.Contains(gb2))
						{
							searched.Add(gb2);
							searchStack.Push(gb2);
						}
					}

					loopSafety++;
					if (loopSafety > 10000)
					{
						Debug.LogError("loop safety!");
						break;
					}
				}
			}

			// tell all my neighbors I'm leaving so they remove me from their neighbor list
			public void RemoveFromWall()
			{
				// iterate a clean list because the list could be modified externally
				List<GridBall> cleanList = new List<GridBall>(this.neighbors);
				foreach (GridBall gb in cleanList)
				{
					gb.neighbors.Remove(this);
				}
				this.Recycle();
			}

			public override void Recycle()
			{
				this.ballGrid = null;
				this.gameObject.SetActive(true);
				this.neighbors = new List<GridBall>();
				base.Recycle();
				PoolManager.I.Return(this);
			}
		}
	}
}
