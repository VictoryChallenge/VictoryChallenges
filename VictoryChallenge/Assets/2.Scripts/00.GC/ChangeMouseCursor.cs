using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChangeMouseCursor : MonoBehaviour
{
    [SerializeField] private Texture2D _mouseCursor;
    [SerializeField] private Texture2D _mouseCursor2;
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

        Cursor.SetCursor(_mouseCursor, _hotspot, CursorMode.ForceSoftware);
    }

    private void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때 커서 변경
        if (Input.GetMouseButtonDown(0))
        {
            _hotspot = _hotspotIsCenter ? new Vector2(_mouseCursor2.width / 2, _mouseCursor2.height / 2) : _adjustHotspot;
            Cursor.SetCursor(_mouseCursor2, _hotspot, CursorMode.ForceSoftware);
        }

        // 마우스 왼쪽 버튼을 뗐을 때 원래 커서로 변경
        if (Input.GetMouseButtonUp(0))
        {
            _hotspot = _hotspotIsCenter ? new Vector2(_mouseCursor.width / 2, _mouseCursor.height / 2) : _adjustHotspot;
            Cursor.SetCursor(_mouseCursor, _hotspot, CursorMode.ForceSoftware);
        }
    }
}
