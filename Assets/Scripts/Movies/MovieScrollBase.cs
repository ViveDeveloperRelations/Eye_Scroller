using System.Collections.Generic;
using GamesTan.UI;
using UnityEngine;
    
public class MovieCellData {
    public int Idx;
    public string Name;
    public int Count;

    public override string ToString() {
        return $"Idx:{Idx} Name:{Name} Count:{Count}";
    }
}

    
public class MovieScrollBase : MonoBehaviour, ISuperScrollRectDataProvider {
    [Header("Basic")] public SuperScrollRect ScrollRect;
    public int Count = 500;
    private List<MovieCellData> Datas = new List<MovieCellData>();
    private void Awake() {
        for (int i = 0; i < Count; i++) {
            Datas.Add(new MovieCellData() {
                Idx = i,
                Name = "Cell " + i,
                Count = UnityEngine.Random.Range(1, 10)
            });
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
        var item = cell.GetComponent<MovieCell>();
        item.BindData(Datas[index]);
    }
}
