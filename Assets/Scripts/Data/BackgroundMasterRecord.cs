// Assets/Scripts/Data/BackgroundMasterRecord.cs
using UnityEngine;

[System.Serializable]
public class BackgroundMasterRecord
{
    [Tooltip("�w�i����ӂɎ��ʂ���ID")]
    public int backgroundID;

    [Tooltip("�w�i�摜 (Art/Backgrounds �ȉ��̃X�v���C�g)")]
    public Sprite backgroundImage;

    [Tooltip("���̔w�i�Ɋ֘A�t���A�C�h��ID")]
    public int idolID;

    [Tooltip("���̔w�i��������X�e�[�WID")]
    public int stageID;

    [Tooltip("�X�e�[�W���ł��̔w�i�̖���")]
    public int countInStage;

    [Tooltip("���̔w�i���N���A�ς݂��ǂ���")]
    public bool isCleared;

    [Header("���N���A���ɕ\������}�X�N�X�v���C�g")]
    public Sprite maskSprite;        // �� ������ǉ��I
}