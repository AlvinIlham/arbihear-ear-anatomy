using UnityEngine;

public class DropTarget : MonoBehaviour
{
    public string correctID; // misalnya "1", "2", dst.

    public void OnMatched(Draggable dragged)
    {
        if (dragged.name == correctID)
        {
            Debug.Log("Cocok! ✅");
            // Tambahkan efek benar
        }
        else
        {
            Debug.Log("Salah ❌");
            // Bisa hapus garis
        }
    }
}
