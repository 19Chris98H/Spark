using UnityEngine;
using TMPro;

public class MissionInfo : MonoBehaviour, IUIElement
{
    public TMP_Text title;
    public TMP_Text description;
    public TMP_Text button;

    bool confirmed = false;

	void Start()
    {
        if (CurrentMission.missions == null)
            return;

        Mission currentMission = CurrentMission.missions[CurrentMission.currentMissionIndex];
        title.text = currentMission.name;
        description.text = currentMission.missionDescription;
        button.text = "Start";
    }

    public void OnClose()
    {
        if (!confirmed)
        {
            confirmed = true;
            button.text = "Continue";
        }

        gameObject.SetActive(false);
    }

	public void OnOpen() => gameObject.SetActive(true);

    public void OnBlocker()
    {
        if (confirmed)
            OnClose();
    }
}
