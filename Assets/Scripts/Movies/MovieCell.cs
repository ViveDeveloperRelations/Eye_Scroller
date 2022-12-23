using GamesTan.UI;
using UnityEngine;
using UnityEngine.UI;

public class MovieCell : MonoBehaviour, IScrollCell {
    public Button BtnItem;
    public Text TextCount;
    public Text TextName;

    private MovieCellData _data;

    public void BindData(MovieCellData data) {
        _data = data;
        //BtnItem.onClick.RemoveListener(OnClick_BtnItem);
        //BtnItem.onClick.AddListener(OnClick_BtnItem);
        //TextCount.text = data.Count.ToString();
        //TextName.text = data.Name.ToString();
        //name = "Cell " + data.Idx;
    }

    void OnClick_BtnItem() {
        UnityEngine.Debug.Log(" Click " + _data);
    }
}