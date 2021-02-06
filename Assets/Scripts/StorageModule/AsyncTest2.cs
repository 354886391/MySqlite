using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTest2 : MonoBehaviour
{

    private void Start()
    {
        Debug.Log("<color=red> before Test </color>");
        TestAsync();
        Debug.Log("<color=red> after Test </color>");
    }



    async void TestAsync()
    {
        Debug.Log("<color=yellow> before Task </color>");
        await Task.Run(() =>
        {
            Debug.Log("<color=blue> before Delay </color>");
            System.Threading.Thread.Sleep(3000);
            //await Task.Delay(3000);
            Debug.Log("<color=blue> after Delay </color>");
        });
        Debug.Log("<color=yellow> after Task </color>");
    }
}
