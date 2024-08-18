using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    private Grid grid;
    public Transform Cube;

    void Start()
    {
        grid = GetComponent<Grid>();

        for(int i = 0; i<5;i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Instantiate(Cube, grid.GetCellCenterWorld(new Vector3Int(i, 0, j)), Quaternion.identity);
            }
        }
    }


}
