using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SFXs {
    buttonClick,
    matchSound,
    swipeSound,
    iceTile,
    breakableTile
}
[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] musicSprites;
    [SerializeField] private Sprite[] sfxSprites;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip swipeSound;
    [SerializeField] private AudioClip iceTileMeltingSound;
    [SerializeField] private AudioClip breakableTileBreakingSound;

    [Header("Others")]
    [SerializeField] private AudioSource[] _audioSources;

    private bool isMusicMuted, isSFXMuted;
    public static SoundController Instance;
    private void Awake(){
        if(Instance == null) Instance = this;

        isMusicMuted = GameSaver.Instance.dataSaver.isMusicMuted;
        isSFXMuted = GameSaver.Instance.dataSaver.isSFXMuted;

        _audioSources[0].mute = isMusicMuted;
        _audioSources[1].mute = isSFXMuted;

        PlayMusic();
    }
    private void PlayMusic()
    {
        _audioSources[0].clip = menuMusic;
        _audioSources[0].Play();
    }
    public void PlaySFX(SFXs clipName)
    {
        AudioClip clip = null;
        //Assign the clip
        if(clipName == SFXs.buttonClick)
            clip = buttonClickSound;
        else if(clipName == SFXs.matchSound)
            clip = matchSound;
        else if(clipName == SFXs.swipeSound)
            clip = swipeSound;
        else if(clipName == SFXs.iceTile)
            clip = iceTileMeltingSound;
        else
            clip = breakableTileBreakingSound;

        //Play the clip
        _audioSources[1].PlayOneShot(clip);
    }
    public void MuteMusic(Image _image)
    {
        if(!isMusicMuted)
        {
            isMusicMuted = true;
            _audioSources[0].mute = true;
            _image.sprite = musicSprites[1];
        }
        else
        {
            isMusicMuted = false;
            _audioSources[0].mute = false;
            _image.sprite = musicSprites[0];
        }
        GameSaver.Instance.dataSaver.isMusicMuted = isMusicMuted ? true : false;
    }
    public void MuteSFX(Image _image)
    {
        if(!isSFXMuted)
        {
            isSFXMuted = true;
            _audioSources[1].mute = true;
            _image.sprite = sfxSprites[1];
        }
        else
        {
            isSFXMuted = false;
            _audioSources[1].mute = false;
            _image.sprite = sfxSprites[0];
        }
        GameSaver.Instance.dataSaver.isSFXMuted = isSFXMuted ? true : false;
    }
    public void SetSFXImage(Image _sfxImage, bool isSFXMuted)
    {
        _sfxImage.sprite = isSFXMuted ? sfxSprites[1] : sfxSprites[0];
    }
    public void SetMusicImage(Image _musicImage, bool isMusicMuted)
    {
        _musicImage.sprite = isMusicMuted ? musicSprites[1] : musicSprites[0];
    }
}
