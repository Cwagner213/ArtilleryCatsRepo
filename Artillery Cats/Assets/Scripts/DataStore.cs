//***********************************************************************************************************************************************************************************
// DataStore exists to store data about the level being played on for easy access of other scripts
// Last Updated: 7/12/18 7:35pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStore : MonoBehaviour {
    [SerializeField]
    public string level;
    [SerializeField]
    public int x, y;
    public string[,] mapgrid;
    public int catNum = 1;
}
