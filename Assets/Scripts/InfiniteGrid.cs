using UnityEngine;

public class InfiniteGrid : MonoBehaviour
{
    public float cellSize = 1f;
    public Color lineColor = Color.white;
    public float lineThickness = 0.1f;
    
    private Camera mainCamera;
    private Material lineMaterial;

    private void Start()
    {
        mainCamera = Camera.main;
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = lineColor;
    }

    private void OnPostRender()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        Vector3 camPosition = mainCamera.transform.position;
        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;

        float xStart = Mathf.Floor(camPosition.x / cellSize) * cellSize - camWidth / 2f;
        float xEnd = xStart + camWidth + cellSize;
        float yStart = Mathf.Floor(camPosition.y / cellSize) * cellSize - camHeight / 2f;
        float yEnd = yStart + camHeight + cellSize;

        GL.PushMatrix();
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        for (float x = xStart; x < xEnd; x += cellSize)
        {
            GL.Vertex(new Vector3(x, yStart, 0));
            GL.Vertex(new Vector3(x, yEnd, 0));
        }

        for (float y = yStart; y < yEnd; y += cellSize)
        {
            GL.Vertex(new Vector3(xStart, y, 0));
            GL.Vertex(new Vector3(xEnd, y, 0));
        }

        GL.End();
        GL.PopMatrix();
    }
}
