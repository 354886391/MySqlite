using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTest : MonoBehaviour
{

    void Start()
    {
        MethodA();
        MethodB();
    }

    private async void MethodA()
    {
        for (int i = 0; i < 5; i++)
        {
            Debug.Log("<color=red>Before Method 1</color>");
            Thread.Sleep(500);
        }
        await Task.Run(() =>
         {
             for (int i = 0; i < 5; i++)
             {
                 Debug.Log("<color=red>Method 1</color>");
                 Thread.Sleep(500);
             }
         });
        for (int i = 0; i < 5; i++)
        {
            Debug.Log("<color=red>After Method 1</color>");
            Thread.Sleep(500);
        }
    }

    private void MethodB()
    {
        for (int i = 0; i < 15; i++)
        {
            Debug.Log("<color=blue>Method 2</color>");
            Thread.Sleep(500);
        }
    }

    // 1. async修饰的方法,在没有遇到await之前,都是同步方法.
    // 2. 遇到await线程开始上下文切换(异步的开始)
    // 3. await后的同步方法,实际并不在主线程中运行,所以After Method 1并没有阻塞Method 2
}
