using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RichJoslin
{
    namespace BubbleShooterVR
    {
		public class GridBallSensorNode : MonoBehaviour
		{
			public Vector3 gridPosition { get; set; }

			public const float POP_DELAY = 0.01f;

			// for debugging
			void OnDrawGizmos()
			{
				// Gizmos.color = Color.yellow;
				// Gizmos.DrawSphere(transform.position, transform.localScale.x / 2);
			}

			public void OnTriggerEnter(Collider other)
			{
				GridBall parentGridBall = this.transform.parent.GetComponent<GridBall>();

				if (parentGridBall != null)
				{
					BallShot ballShotThatHitMe = other.GetComponent<BallShot>();
					if (ballShotThatHitMe != null)
					{
						// only sense flying balls
						if (ballShotThatHitMe.state == BallShot.State.Flying)
						{
							// before processing anything else, snap a new GridBall into place
							GridBall newGridBall = ballShotThatHitMe.SnapTo(parentGridBall.ballGrid, this);

							// TODO: everything below this belongs in a different class - maybe use command pattern

							// then start processing what happens after the ball exists
							switch (ballShotThatHitMe.ballType)
							{
								case BallType.PaintSplash:
									// turn all touched balls the same color
									foreach (GridBall neighborBall in newGridBall.neighbors)
									{
										neighborBall.ballColor = newGridBall.ballColor;
									}
									break;

								case BallType.Bomb:
									// TODO: pop all balls in a radius
									break;

								default:
									// basic match-3 popping
									GameMgr.I.StartCoroutine(this.PopChainLoop(newGridBall));
									break;
							}
						}
					}
					else
					{
						Debug.LogWarning("grid ball sensor hit with something other than a ball shot", this);
					}
				}
				else
				{
					Debug.LogWarning("orphaned grid ball sensor", this);
				}
			}

			public IEnumerator PopChainLoop(GridBall newGridBall)
			{
				// set this before we lose a reference to it when the new grid ball pops
				BallGrid currentBallGrid = newGridBall.ballGrid;

				currentBallGrid.SetState(BallGrid.State.Popping);

				List<GridBall> ballCluster = new List<GridBall>() { newGridBall };
				List<GridBall> searched = new List<GridBall>(ballCluster);
				Queue<GridBall> searchQueue = new Queue<GridBall>(ballCluster);
				List<GridBall> adjacentBallsThatSurvived = new List<GridBall>();

				while (searchQueue.Count > 0)
				{
					GridBall curBall = searchQueue.Dequeue();

					if (curBall.ballColor == newGridBall.ballColor)
					{
						if (!ballCluster.Contains(curBall)) ballCluster.Add(curBall);

						foreach (GridBall gb in curBall.neighbors)
						{
							if (!searched.Contains(gb))
							{
								searched.Add(gb);
								searchQueue.Enqueue(gb);
							}

						}
					}
					else
					{
						if (!adjacentBallsThatSurvived.Contains(curBall)) adjacentBallsThatSurvived.Add(curBall);
					}
				}
				if (ballCluster.Count >= 3)
				{
					foreach (GridBall gb in ballCluster)
					{
						gb.RemoveFromWall();
						yield return new WaitForSeconds(POP_DELAY);
					}
				}

				int loopSafety = 0;
				List<GridBall> searched2 = new List<GridBall>();
				while (adjacentBallsThatSurvived.Count > 0)
				{
					GridBall survivorBall = adjacentBallsThatSurvived[0];
					searched2.Add(survivorBall);
					adjacentBallsThatSurvived.RemoveAt(0);
					survivorBall.CheckIfConnectedToWall();
					if (!survivorBall.isConnectedToWall)
					{
						foreach (GridBall gb in survivorBall.neighbors)
						{
							if (!searched2.Contains(gb) && !adjacentBallsThatSurvived.Contains(gb))
							{
								adjacentBallsThatSurvived.Add(gb);
							}
						}
						survivorBall.RemoveFromWall();
						yield return new WaitForSeconds(POP_DELAY);
					}

					loopSafety++;
					if (loopSafety > 10000)
					{
						Debug.LogError("loop safety!");
						break;
					}
				}

				currentBallGrid.SetState(BallGrid.State.Default);
			}
		}
	}
}
