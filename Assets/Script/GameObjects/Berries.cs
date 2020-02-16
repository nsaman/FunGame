using UnityEngine;

public class Berries : MonoBehaviour {

    void Start()
    {
        // todo, adjust resources based on size
        GetComponent<Inventory>().inventory.Add(new InventoryEntry(Tags.Food, 100));
    }
    
    void Update () {
	
	}
}
