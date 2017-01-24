using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GJJ2017StockWave {
	
	public class Wave : MonoBehaviour {

		public float xUnit = 0.1f;
		public float yMagnitudePerStockPrice = 10f;
		public float marginRight = 50f;  // pixels
		public float marginVerticalToScreen = 30f;  // pixels
		public float paddingVerticalToScreen = 30f;  // pixels

		public RectTransform emotion;

		public static Wave instance;

		float xMax, yMax, yMin, yPadding;

		LineRenderer lineRenderer;

		Vector3[] positions;
		Vector3 initialPosition;

		// Use this for initialization
		void Start () {
			instance = this;

			Vector2 startPos = Camera.main.ScreenToWorldPoint (new Vector2 (0, Camera.main.pixelHeight / 2));
			transform.position = new Vector3 (startPos.x, startPos.y, transform.position.z);  // 放到屏幕左边缘中心

			float xMaxScreen = Camera.main.pixelWidth - Mathf.Abs (emotion.anchoredPosition.x) - emotion.rect.width - marginRight;
			Vector3 xMaxWorld = Camera.main.ScreenToWorldPoint(new Vector2 (xMaxScreen, 0));
			xMax = transform.InverseTransformPoint (xMaxWorld).x;

			Vector3 yMaxWorld = Camera.main.ScreenToWorldPoint (new Vector2 (0, Camera.main.pixelHeight - marginVerticalToScreen));
			yMax = transform.InverseTransformPoint (yMaxWorld).y;

			Vector3 yMinWorld = Camera.main.ScreenToWorldPoint (new Vector2 (0, marginVerticalToScreen));
			yMin = transform.InverseTransformPoint (yMinWorld).y;

			Vector3 yPaddingWorld = Camera.main.ScreenToWorldPoint (new Vector2 (0, Camera.main.pixelHeight / 2 + paddingVerticalToScreen));
			yPadding = transform.InverseTransformPoint (yPaddingWorld).y;

			lineRenderer = GetComponent<LineRenderer> ();
			positions = new Vector3[lineRenderer.numPositions * 100];
			positions[0] = initialPosition = lineRenderer.GetPosition (0);
		}

		public void Reset () {
			lineRenderer.numPositions = 1;
			lineRenderer.SetPosition (0, initialPosition);
		}
		
		public void UpdateWave () {
			if (positions.Length <= lineRenderer.numPositions) {
				Vector3[] newPositions = new Vector3[2 * lineRenderer.numPositions];
				Array.Copy (positions, newPositions, positions.Length);
				positions = newPositions;
			}

			Vector3 newPos = lineRenderer.GetPosition (lineRenderer.numPositions - 1);
			newPos.x += xUnit;
			newPos.y += yMagnitudePerStockPrice * StockWaveGame.instance.GetStockPriceDelta();

			if (newPos.x <= xMax) {
				positions[lineRenderer.numPositions] = newPos;
				lineRenderer.numPositions = lineRenderer.numPositions + 1;
				lineRenderer.SetPosition (lineRenderer.numPositions - 1, newPos);
			} else {
				// Remove index 0
				for (int i = 0; i < lineRenderer.numPositions - 1; ++i) {
					positions [i] = positions [i + 1];
				}
				positions [lineRenderer.numPositions - 1] = newPos;
				for (int i = 0; i < lineRenderer.numPositions; ++i) {
					positions [i].x -= xUnit;
				}
				lineRenderer.SetPositions (positions);
			}

			newPos = positions [lineRenderer.numPositions - 1];
			if (newPos.y > yMax || newPos.y < yMin) {
				float yOld = newPos.y;
				float yNew = newPos.y > yMax ? yMax - yPadding : yMin + yPadding;
				float yDelta = yNew - yOld;
				for (int i = 0; i < lineRenderer.numPositions; ++i) {
					positions [i].y += yDelta;
				}
				lineRenderer.SetPositions (positions);
			}

			// stock price
			#if false
			newPos = positions [lineRenderer.numPositions - 1];
			Vector3 newPosWorld = transform.TransformPoint (newPos);
			Vector3 newPosScreen = Camera.main.WorldToScreenPoint (newPosWorld);
			Vector2 newPosCanvas = newPosScreen;
			newPosCanvas.y -= Camera.main.pixelHeight / 2;
			stockPriceText.rectTransform.anchoredPosition = newPosCanvas;
			#endif
		}
	}
}