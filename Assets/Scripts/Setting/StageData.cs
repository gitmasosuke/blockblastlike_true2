// Assets/Scripts/Settings/StageData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "BlockBrust/StageData")]




public class StageData : ScriptableObject
{
    [Header("�X�e�[�W���ʎq (0 ����n�܂�ԍ��Ȃ�)")]
    public int stageID;

    [Tooltip("�X�e�[�W���ɕ\������w�i�X�v���C�g�i���Ԃɐ؂�ւ��܂��j")]
    public Sprite[] backgroundSprites;

    [Tooltip("���ꂼ��̔w�i��؂�ւ���O�ɒB�����ׂ��g�Z��������h")]
    public int[] unlockThresholds;


    [Header("�Z���P�ʂ̏����u���b�N�z�u")]
    public Vector2Int[] initialBlockCells;

    [Header("�o���~�m�i�d�ݕt���j")]
    public WeightedPiece[] allowedPieces;


    [Header("�����z�u�u���b�N�p�X�v���C�g")]
    public Sprite initialBlockSprite;   // �� �ǉ�

    [Header("�X�e�[�W�����萔")]
    [Tooltip("�z�u�\�Ȏ萔�̏���B��������Q�[���I�[�o�[�ɂȂ�܂��B")]
    public int RestCount;
}


[System.Serializable]
public struct WeightedPiece
{
    public PieceData piece;  // �~�m�{��
    public int weight;       // �o�����̏d�݁i0�ȏ�̐����j
}