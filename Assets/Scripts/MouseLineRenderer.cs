using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private Material lineMaterial;
	private PointManager pointScript;
	private Transform tf;
	private Transform pTf;
	private Vector3[] arrayPos = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
	private Vector3 startPos = Vector3.zero;
	private Vector3 nextPos = Vector3.zero;
	private Vector3 velocity = Vector3.zero;
	private const float lineDistance = 1.0f;
	private float distance = 0.0f;
	private float speed = 10f;
	private int linePosSize = 5;
	private bool drawStartFlag = false;
	private bool setLineFlag = false;
	private bool moveFlag = false;

	private int debugCount = 0;

	enum Line
    {
		None,
		Draw,
    }
	private Line lineType = Line.None;

    public void Init(GameObject p)
    {
		if (lineRenderer != null)
		{
			lineRenderer.positionCount = linePosSize;
			// マテリアル設定
			if (lineMaterial != null)
			{
				lineRenderer.material = lineMaterial;
			}
			// 色設定
			{
				var colorKeys = new[]
				{
					new GradientColorKey( Color.red, 0 ),
					new GradientColorKey( Color.blue, 1 ),
				};
				var alphaKeys = new[]
				{
					new GradientAlphaKey( 1, 0 ),
					new GradientAlphaKey( 1, 0 ),
				};
				var gradient = new Gradient();
				gradient.SetKeys(colorKeys, alphaKeys);
				lineRenderer.colorGradient = gradient;
			}
			// 線の太さ。
			lineRenderer.startWidth = 0.1f;
			lineRenderer.endWidth = 0.1f;
		}
		pTf = p.transform;
		pointScript = p.GetComponent<PointManager>();
		tf = this.transform;
	}

    // Update is called once per frame
    public void ManagedUpdate(RaycastHit info)
    {
		CreateLine(info);

		DrawLine();

		Debug.Log("startPos = " + startPos);
		Debug.Log("nextPos = " + nextPos);
	}

	// ラインを作る処理
	private void CreateLine(RaycastHit info)
    {
		// 真ん中のボタンをクリックで、引いたラインを消す処理(ラインが細すぎた場合も)
		if (Input.GetMouseButtonDown(2) || distance < lineDistance && setLineFlag)
		{
			setLineFlag = false;
			lineType = Line.None;
			lineRenderer.positionCount = linePosSize;
			for (int i = 0; i < linePosSize; i++)
			{
				this.lineRenderer.SetPosition(i, Vector3.zero);
			}
		}

		// 左のボタンをクリックで、ラインを引く
		if (Input.GetMouseButton(0) && !setLineFlag)
		{
			// 四角を描くために必要な頂点を代入する
			lineRenderer.positionCount = linePosSize;
			debugCount++;

			// 引き始めだったら
			if (!drawStartFlag)
			{
				startPos = info.point;
				drawStartFlag = true;
			}
			nextPos = info.point;
			nextPos.y = startPos.y;  // 高さは変えない
			lineType = Line.Draw;
			setLineFlag = false;  // まだラインを保存しない
		}
		else  // 左が押されていなかったら
		{
			// ラインを引く処理を行っていたら
			if (lineType == Line.Draw)
			{
				setLineFlag = true;  // その状態のラインを保存する
			}
			distance = DifferenceWidthVector(arrayPos[0], arrayPos[2]).x;
			lineType = Line.None;
			startPos = Vector3.zero;
			drawStartFlag = false;  // 引き始めに変更
		}
	}

	// ラインを引く処理
	private void DrawLine()
    {
		// 作ったラインを引き続ける
		if (lineType == Line.None && setLineFlag)
		{
			moveFlag = pointScript.GetMoveFlag();
			for (int i = 0; i < linePosSize; i++)
			{
				// 追加した頂点の座標を設定
				this.lineRenderer.SetPosition(i, arrayPos[i]);
				if (moveFlag)
				{
					velocity = arrayPos[i] + tf.forward * speed * Time.deltaTime;
				}
			}
			Vector3 tPos = Vector3.Lerp(arrayPos[0], arrayPos[2], 0.5f);
			tPos.y = startPos.y;
			tf.position = tPos;
			tf.LookAt(pTf);
		}

		// 作っているラインを可視化する
		if (lineType == Line.Draw)
		{
			for (int i = 0; i < linePosSize; i++)
			{
				var clickPos = Vector3.zero;
				clickPos = nextPos;
				clickPos.y = startPos.y;
				// 四角になるラインにする
				switch (i)
				{
					case 1:
						clickPos.z = startPos.z;
						break;
					case 2:
						break;
					case 3:
						clickPos.x = startPos.x;
						break;
					case 4:
						clickPos.x = startPos.x;
						clickPos.z = startPos.z;
						break;
					default:
						clickPos.x = startPos.x;
						clickPos.z = startPos.z;
						break;
				}

				// 追加した頂点の座標を設定
				this.lineRenderer.SetPosition(i, clickPos);
				arrayPos[i] = clickPos;
			}
		}
	}

	public Vector3 GetStartLinePos()
	{
		return arrayPos[0];
	}

	public Vector3 GetEndLinePos()
    {
		return arrayPos[2];
	}

	private Vector3 DifferenceWidthVector(Vector3 pos1, Vector3 pos2)
    {
		Vector3 pos = Vector3.zero;
		if(pos1.x > pos2.x)
        {
			pos.x = pos1.x - pos2.x;
			return pos;
        }
		else
        {
			pos.x = pos2.x - pos1.x;
			return pos;
        }
    }

	public bool GetSetLineFlag()
    {
		return setLineFlag;
    }
}