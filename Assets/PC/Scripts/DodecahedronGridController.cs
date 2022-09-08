using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodecahedronGridController : MonoBehaviour
{
    // class members
    public Transform gridPrefab;
    public int row = 4;
    public int col = 4;

    private void Start()
    { // some function that builds the grid
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {

                Instantiate(gridPrefab, new Vector3(row, 0, col), Quaternion.identity);
            }

        }
    }
}
    
