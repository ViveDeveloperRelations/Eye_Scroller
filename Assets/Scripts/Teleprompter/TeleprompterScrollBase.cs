using System.Collections.Generic;
using GamesTan.UI;
using UnityEngine;
    
public class TeleprompterCellData {
    public string text;

    //public override string ToString() {
    //    return $"Idx:{Idx} Name:{Name} Count:{Count}";
    //}
}

public class TeleprompterScrollBase : MonoBehaviour, ISuperScrollRectDataProvider {
    [Header("Basic")] public SuperScrollRect ScrollRect;
    public List<TeleprompterCellScriptableObject> teleprompterCellScriptableObjects =
        new List<TeleprompterCellScriptableObject>();
    private List<TeleprompterCellData> Datas = new List<TeleprompterCellData>();

    private void Awake() {
        for (int i = 0; i < teleprompterCellScriptableObjects.Count; i++) {
            Datas.Add(new TeleprompterCellData() {
                text = teleprompterCellScriptableObjects[i].text
            }); ;
        }

        ScrollRect.DoAwake(this);
        DoAwake();
    }

    protected virtual void DoAwake() {
    }

    public int GetCellCount() {
        return Datas.Count;
    }

    public void SetCell(GameObject cell, int index) {
        var item = cell.GetComponent<TeleprompterCell>();
        item.BindData(Datas[index]);
    }
}
