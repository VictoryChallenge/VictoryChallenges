using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMouseCursor : MonoBehaviour
{
    [SerializeField] private Texture2D _mouseCursor;
    private bool _hotspotIsCenter = false;
    private Vector2 _adjustHotspot = Vector2.zero;
    private Vector2 _hotspot;

    private void Start()
    {
        StartCoroutine(ChangeCursor());
    }

    public IEnumerator ChangeCursor()
    {
        yield return new WaitForEndOfFrame();

        _hotspot = _adjustHotspot;

        Cursor.SetCursor(_mouseCursor, _hotspot, CursorMode.Auto);
    }
}
