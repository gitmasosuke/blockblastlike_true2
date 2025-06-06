// Assets/Scripts/Data/StageMasterRecord.cs
using UnityEngine;

[System.Serializable]
public class StageMasterRecord
{
    [Tooltip("�X�e�[�W����ӂɎ��ʂ��� ID")]
    public int stageID;

    [Tooltip("����X�e�[�W�Ȃ� true")]
    public bool isLimited;

    [Tooltip("�}�b�v�̊K�w (1,2,3�c �̂悤��)")]
    public int tier;

    [Tooltip("���̃X�e�[�W�Ɋ��蓖�Ă��Ă���w�i����")]
    public int backgroundCount;

    [Tooltip("�X�e�[�W�Z���N�g���ɕ\������T���l�C��")]
    public Sprite thumbnail;       // �� ������ǉ�

    [Tooltip("���̃X�e�[�W�ɑΉ�����A�C�h��ID")]
    public int idolID;    // �� �ǉ�

    [Tooltip("�Q�[���{�҂Ŏg���f�[�^")]
    public StageData stageData;
}