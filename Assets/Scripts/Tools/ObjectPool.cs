using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
{
    private Queue<T> pool = new Queue<T>();
    private System.Func<T> addFunction;
    private System.Func<T> initFunction;

    /** 初始化 */
    public void init(System.Func<T> addFunc, int size) {
        addFunction = addFunc;
        for(int i = 0; i < size; i++) {
            recovery(addFunction());
        }
    }

    /** 取得物件 */
    public T getObject() {
        if (pool.Count != 0) {
            return pool.Dequeue();
        }
        else {
            return addFunction();
        }
    }

    /** 回收物件 */
    public void recovery(T item) {
        pool.Enqueue (item);
    }
}
