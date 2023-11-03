using UnityEngine;

public class CameraSizer : MonoBehaviour
{
    private Camera _camera;
    private Board board;

    public float cameraOffset = -10f;
    public float aspectRatio = 9f / 16f;
    public float padding = 2;
    public float yOffset;
    public Vector2 aspect;
    public bool resizeOnUpdate;
    private void Start()
    {
        _camera = GetComponent<Camera>();
        board = FindObjectOfType<Board>();

        aspectRatio = aspect.x / aspect.y;

        if(board != null)
            RepositionCamera();
        if(FindObjectOfType<ItemSpawnManager>() != null)
            RepositionCam();
    }
    private void Update()
    {
        if (board != null)
            RepositionCamera();
        if (FindObjectOfType<ItemSpawnManager>() != null && resizeOnUpdate)
            RepositionCam();
    }
    private void RepositionCam()
    {
        int width = ItemSpawnManager.Instance.boardWidth;
        int height = ItemSpawnManager.Instance.boardHeight;

        float centerX = width / 2;
        float centerY = height / 2 + yOffset;

        if (width % 2 == 0)
            centerX -= 0.5f;

        transform.position = new Vector3(centerX, centerY, cameraOffset);

        if (width >= height)
            _camera.orthographicSize = (width / 2f + padding) / aspectRatio;
        else
            _camera.orthographicSize = height / 2f + padding;
    }
    private void RepositionCamera()
    {
        float centerX = board.width / 2;
        float centerY = board.height / 2 + yOffset;

        if (board.width % 2 == 0)
            centerX -= 0.5f;

        transform.position = new Vector3(centerX, centerY, cameraOffset);

        if (board.width >= board.height)
            _camera.orthographicSize = (board.width / 2f + padding) / aspectRatio;
        else
            _camera.orthographicSize = board.height / 2f + padding;
    }   
}
