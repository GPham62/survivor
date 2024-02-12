using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob
{
    public partial class SoundManager : SingletonMono<SoundManager>
    {
        //public void PlaySoundInstance(string soundPath, bool forceSound = false)
        //{
        //    if (!IsSoundOn && !forceSound)
        //    {
        //        return;
        //    }
        //    if (!m_dicEventInstance.ContainsKey(soundPath))
        //    {
        //        EventInstance eventInstance = RuntimeManager.CreateInstance(soundPath);
        //        eventInstance.start();
        //        m_dicEventInstance.Add(soundPath, eventInstance);
        //    }
        //    else
        //    {
        //        m_dicEventInstance[soundPath].start();
        //    }
        //}
        public void ChangeStateSoundInstance(string soundPath, bool isPause)
        {
            if (m_dicEventInstance.ContainsKey(soundPath))
                m_dicEventInstance[soundPath].setPaused(isPause);
        }
    }
}