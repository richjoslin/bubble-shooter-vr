using UnityEngine;
using System.Collections;
using RichJoslin.Pooling;

namespace RichJoslin
{
    namespace BubbleShooterVR
    {
        public class BallShot : Ball
        {
			public const float SHOOT_FORCE = 20f;

			public Rigidbody ballRigidBody { get; set; }
			public Collider ballCollider { get; set; }

#region Finite State Machine

            public enum State
            {
                Pooled,
                Default,
                Loaded,
                Flying,
				Snapping,
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

			public void Awake()
			{
				this.ballCollider = this.GetComponent<Collider>();
				this.ballRigidBody = this.GetComponent<Rigidbody>();
			}

            public static BallShot Generate(Transform gunTF)
            {
				BallShot ballShot = PoolManager.I.GetPooler("BallShot").Get<BallShot>();
                ballShot.transform.SetParent(gunTF);
                ballShot.transform.localScale = Vector3.one * 0.30f;
				ballShot.transform.localPosition = new Vector3(0f, -0.012f, 0.46f);
				ballShot.transform.localRotation = Quaternion.identity;
				ballShot.ballRigidBody.velocity = Vector3.zero;
				ballShot.ballRigidBody.angularVelocity = Vector3.zero;
				ballShot.SetState(State.Loaded);
				BallColor randomColor = (BallColor)Random.Range(1, (int)BallColor.LENGTH);
				ballShot.SetColor(randomColor);
                return ballShot;
            }

            public void Shoot()
            {
				Vector3 shootDirection = (this.transform.position - this.transform.parent.position).normalized;
				this.ballRigidBody.velocity = shootDirection * SHOOT_FORCE;
				this._state = State.Flying;
				this.SetColor(this.ballColor, a: 1f);
				this.transform.SetParent(null);
				this.ballCollider.enabled = true;
				this.transform.localScale = Vector3.one * 0.75f;
				this.StartCoroutine("CountdownToRecycle");
            }

			public GridBall SnapTo(BallGrid ballGrid, GridBallSensorNode sensorNode)
			{
				this._state = State.Snapping;
				this.StopCoroutine("CountdownToRecycle");
				this.ballCollider.enabled = false;
				this.ballRigidBody.velocity = Vector3.zero;
				this.ballRigidBody.angularVelocity = Vector3.zero;

				// instantiate a grid ball in the correct spot and recycle this ball
				GridBall gridBall = sensorNode.GetComponentInParent<GridBall>();

				Vector3i direction = new Vector3i(
					Mathf.RoundToInt(Utils.RoundAwayFromZero(sensorNode.transform.localPosition.x)),
					Mathf.RoundToInt(Utils.RoundAwayFromZero(sensorNode.transform.localPosition.y)),
					Mathf.RoundToInt(Utils.RoundAwayFromZero(sensorNode.transform.localPosition.z))
				);

				Vector3i newPos = BallGrid.GetNeighborCoords(new Vector3i(gridBall.gridCoords), direction);

				Transform wallTF = ballGrid.GetOrCreateLayer(Mathf.RoundToInt(newPos.z));
				Transform rowTF = ballGrid.GetOrCreateRow(Mathf.RoundToInt(newPos.z), Mathf.RoundToInt(newPos.y), wallTF);
				Transform colTF = ballGrid.GetOrCreateColumn(Mathf.RoundToInt(newPos.x), rowTF);
				GridBall newGridBall = colTF.GetComponentInChildren<GridBall>();
				if (newGridBall == null)
				{
					newGridBall = GridBall.Generate(colTF, wasBallShot: true);
					newGridBall.RefreshGridCoords();
				}
				else
				{
					Debug.LogWarningFormat("grid ball already exists at zyx {0}", newPos.ToString());
				}
				newGridBall.SetColor(this.ballColor);

				this.Recycle();

				return newGridBall;
			}

			public IEnumerator CountdownToRecycle()
			{
				yield return new WaitForSeconds(2f);
				if (this._state == State.Flying) this.Recycle();
			}

			public override void Recycle()
			{
				base.Recycle();
				if (GameMgr.I.shootMgr.ballShot == this) GameMgr.I.shootMgr.ballShot = null;
				this.ballCollider.enabled = true;
				this.ballRigidBody.velocity = Vector3.zero;
				this.ballRigidBody.angularVelocity = Vector3.zero;
				if (this._state != State.Pooled)
				{
					this._state = State.Pooled;
					PoolManager.I.Return(this);
				}
            }
        }
    }
}
