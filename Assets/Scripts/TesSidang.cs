using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TesSidang : MonoBehaviour
{
    List<int> input = new List<int>() {1,2,3,4,5,6,7,8,9,10};
    List<int> ganjilList = new List<int>(5);
    List<int> genapList = new List<int>(5);
    int genap = 0;
    int ganjil = 0;
    
    private void Start()
    {
        foreach (var item in input){
            if(item % 2 == 1){
                ganjilList.Add(item);
                ganjil = ganjil+1;
            }
            if(item % 2 == 0){
                genapList.Add(item);
                genap = genap+1;
            }
        }

        Debug.Log("jumlah angka ganjil = " + ganjil);
        Debug.Log("angka ganjil = ");
        foreach (var item in ganjilList){
            Debug.Log(item);
        }
        Debug.Log("jumlah angka genap = " + genap);
        Debug.Log("angka genap = ");
        foreach (var item in genapList){
            Debug.Log(item);
        }
    }
    
}