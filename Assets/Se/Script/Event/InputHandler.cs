using System.Runtime.CompilerServices;
using UnityEngine;
namespace SEGames
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private Camera _cam;

        //private void Update()
        //{

        //    if (!Input.GetMouseButtonDown(0)) return;

        //    var worldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
        //    var hit = Physics2D.Raycast(worldPos, Vector2.zero); Debug.Log(" update 1 " + hit);
        //    if (hit.transform.tag == "Tile")
        //    {
        //        Debug.Log("tag " + hit.transform.tag);
        //    }
        //    if (hit.collider == null) return;

        //    Debug.Log(" update ");
        //    var tile = hit.collider.GetComponent<TileBehaviour>();
        //    Debug.Log(" Name : " + tile.name);
        //    tile?.OnTapped(); // tile calls SEBus.Emit — no FindObjectOfType
        //}

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; // Declare a RaycastHit variable

            // Perform the raycast and check if it hits anything
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
            }

            //Vector3 worldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            //worldPos.z = 0f;
            //Debug.DrawRay(worldPos, Vector2.zero, Color.red, 100f);
            //RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);


            if (hit.collider == null)
            {
                Debug.Log("No object hit");
                return;
            }

            Debug.Log("update : " + hit.transform.name);

            if (hit.transform.CompareTag("Tile"))
            {
                Debug.Log("tag: " + hit.transform.tag);
            }

            Debug.Log("update");

            TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();

            if (tile != null)
            {
                Debug.Log("Name: " + tile.name);
                tile.OnTapped();
            }
        }
    }
}