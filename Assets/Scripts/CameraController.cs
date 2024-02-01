using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;         // Speed for panning
    public float panBorderThickness = 10f;   // Thickness of the screen border for panning

    public float zoomSpeed = 5f;        // Speed for zooming
    public float minY = 10f;            // Minimum height for the camera
    public float maxY = 80f;            // Maximum height for the camera

    public Terrain terrain;             // Reference to the terrain

    private Vector2 touchStartPos;

    void Update()
    {
        HandleTouchInput();
        HandleKeyboardInput();
        RestrictCameraToBounds();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    Vector2 delta = touch.position - touchStartPos;
                    transform.Translate(-delta.x * panSpeed * Time.deltaTime, 0, -delta.y * panSpeed * Time.deltaTime);
                    touchStartPos = touch.position;
                    break;
            }
        }

        // Zooming with pinch gesture
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - difference * zoomSpeed, minY, maxY);
        }
    }

    void HandleKeyboardInput()
    {
        // Additional controls can be added here
    }

    void RestrictCameraToBounds()
    {
        if (terrain != null)
        {
            // Get the terrain size
            Vector3 terrainSize = terrain.terrainData.size;

            // Calculate the camera boundaries based on the rotated axes
            float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float cameraHalfHeight = Camera.main.orthographicSize;

            float minX = cameraHalfWidth;
            float maxX = terrainSize.x - cameraHalfWidth;

            float minZ = cameraHalfHeight;
            float maxZ = terrainSize.z - cameraHalfHeight;

            // Restrict camera position based on terrain size and rotated axes
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minX, maxX),
                transform.position.y,
                Mathf.Clamp(transform.position.z, minZ, maxZ)
            );
        }
    }
}
