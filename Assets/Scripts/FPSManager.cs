using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    // ManagedUpdate()���Ă΂ꂽ�񐔂��J�E���g���܂��B
    int frameCount;
    // �O��t���[�����[�g��\�����Ă���̌o�ߎ��Ԃł��B
    float elapsedTime;

    float deltaTime = 0.0f;

    bool calculatedFlag = false;

    public void Init()
    {

    }

    public void Update()
    {
        // �Ă΂ꂽ�񐔂����Z���܂��B
        frameCount++;

        // �O�̃t���[������̌o�ߎ��Ԃ����Z���܂��B
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 1.0f)
        {
            // �o�ߎ��Ԃ�1�b�𒴂��Ă�����A�t���[�����[�g���v�Z���܂��B
            float fps = 1.0f * frameCount / elapsedTime;
            deltaTime = (float)(elapsedTime / frameCount);

            // �t���[���̃J�E���g�ƌo�ߎ��Ԃ����������܂��B
            frameCount = 0;
            elapsedTime = 0f;

            calculatedFlag = true;
        }
    }

    public float GetDeltaTime()
    {
        return deltaTime;
    }

    public bool GetCalculatedFlag()
    {
        return calculatedFlag;
    }
}
