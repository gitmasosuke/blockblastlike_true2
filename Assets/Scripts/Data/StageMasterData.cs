// Assets/Scripts/Data/StageMasterData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "StageMasterData",
    menuName = "Game/Stage Master Data",
    order = 10
)]

[System.Serializable]
public class StageRecord
{
    public int stageID;     // �X�e�[�W�ԍ�
    public bool isLimited;   // ����t���O
    public int tier;        // �K�w�^�e�B�A
    public Sprite thumbnail;   // �T���l�C���摜
    // �c�K�v�ł���Δw�i�����Ȃǂ��ǉ��c
}

public class StageMasterData : ScriptableObject
{
    [Tooltip("�X�e�[�W�I����ʂŕ\�����邷�ׂẴX�e�[�W���")]
    public List<StageMasterRecord> stages = new List<StageMasterRecord>();
}