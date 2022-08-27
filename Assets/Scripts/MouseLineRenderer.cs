using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private Material lineMaterial;
	private Vector3[] arrayPos = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
	private Vector3 startPos = Vector3.zero;
	private Vector3 tmpPos = Vector3.zero;
	private Vector3 nextPos = Vector3.zero;
	private const float lineDistance = 1.0f;
	private float distance = 0.0f;
	private int linePosSize = 5;
	private bool drawStartFlag = false;
	private bool setLineFlag = false;

	private int debugCount = 0;

	enum Line
    {
		None,
		Draw,
    }
	private Line lineType = Line.None;

    public void Init()
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
	}

    // Update is called once per frame
    public void ManagedUpdate(RaycastHit info)
    {
		Debug.Log("startPos = " + startPos);

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

		if (Input.GetMouseButton(0))
		{
			lineRenderer.positionCount = linePosSize;
			debugCount++;

			if (!drawStartFlag)
			{
				startPos = info.point;
				drawStartFlag = true;
			}
			nextPos = info.point;
			nextPos.y = startPos.y;
			lineType = Line.Draw;
			setLineFlag = false;
		}
		else
		{
			if (lineType == Line.Draw)
			{
				setLineFlag = true;
			}
			distance = DifferenceWidthVector(arrayPos[0], arrayPos[2]).x;
			lineType = Line.None;
			startPos = Vector3.zero;
			drawStartFlag = false;
		}

		Debug.Log("nextPos = " + nextPos);
		Debug.Log("tmpPos = " + tmpPos);

		if (lineType == Line.None && setLineFlag)
		{
			for (int i = 0; i < linePosSize; i++)
			{
				// 追加した頂点の座標を設定
				this.lineRenderer.SetPosition(i, arrayPos[i]);
			}
		}

		if(lineType == Line.Draw)
		{
			for (int i = 0; i < linePosSize; i++)
			{
                var clickPos = Vector3.zero;
                clickPos = nextPos;
				clickPos.y = startPos.y;
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