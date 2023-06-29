using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class SoundManager : MonoBehaviour
    {
        // Player prefs keys to store current OST and SFX volume and enabled booleans
        private const string OST_KEY = "OST_Volume";
        private const string SFX_KEY = "SFX_Volume";
        private const string OST_ENABLED_KEY = "OST_Enabled";
        private const string SFX_ENABLED_KEY = "SFX_Enabled";

        /// <summary>
        /// The General OST Volume
        /// </summary>
        public static float OSTVolume
        {
            set
            {
                Instance.ostVolume = Mathf.Clamp01(value);
            }

            get
            {
                return Instance.ostVolume;
            }
        }

        /// <summary>
        /// The General SFX Volume
        /// </summary>
        public static float SFXVolume 
        {
            set
            {
                Instance.sfxVolume = Mathf.Clamp01(value);
            }
            
            get
            {
                return Instance.sfxVolume;
            }
        }

        /// <summary>
        /// Enables whether the OST should play or not, effectively this sets the internal volume of the OST to 0
        /// </summary>
        public static bool OSTEnabled
        {
            set
            {
                Instance.ostEnabled = value;
            }

            get
            {
                return Instance.ostEnabled;
            }
        }

        /// <summary>
        /// Enables whether the SFX should play or not, effectively this sets the internal volume of the SFX to 0
        /// </summary>
        public static bool SFXEnabled
        {
            set
            {
                Instance.sfxEnabled = value;
            }

            get
            {
                return Instance.sfxEnabled;
            }
        }

        /// <summary>
        /// The duration of the transition between OSTs
        /// </summary>
        public float OSTTransitionDuration
        {
            set
            {
                // Don't allow the value to be set to a negative value
                ostTransitionDuration = Mathf.Max(value, 0);
            }

            get
            {
                return ostTransitionDuration;
            }
        }

        [SerializeField]
        [Range(0, 1)]
        private float sfxVolume = 1;
        [SerializeField]
        [Range(0, 1)]
        private float ostVolume = 0.5f;
        [SerializeField]
        private bool sfxEnabled = true;
        [SerializeField]
        private bool ostEnabled = true; 
        [SerializeField]
        private float ostTransitionDuration = 1;

        private OST activeOST;
        private bool initted = false;

        /// <summary>
        /// The main singleton Instance of the SoundManager
        /// </summary>
        public static SoundManager Instance 
        {
            get
            {
                if(donotuse_instance == null)
                {
                    donotuse_instance = (new GameObject("Sound Manager")).AddComponent<SoundManager>();

                    donotuse_instance.Init();
                }

                return donotuse_instance;
            }
        }   
        private static SoundManager donotuse_instance;


        /// <summary>
        /// Plays a given OST, transitioning from the previous OST if needed
        /// <param name="ost">The OST to be played.</param>
        /// </summary>
        public static void PlayOST(OST ost)
        {
            Instance.activeOST = ost;
        }

        /// <summary>
        /// Plays a random OST from the given list of OSTs, transitioning from the previous OST if needed
        /// <param name="osts">The list of OSTs that will have one of it's OSTs randomly selected to be played.</param>
        /// </summary>
        public static void PlayOST(List<OST> osts)
        {
            Instance.activeOST = osts.Random();
        }

        /// <summary>
        /// Plays a given SFX
        /// <param name="sfx">The SFX to be played.</param>
        /// </summary>
        public static void PlaySFX(SFX sfx)
        {
            if(sfx.Clip == null) return;

            Instance.StartCoroutine(SFXCoroutine(sfx));
        }

        /// <summary>
        /// Plays a random SFX from the given list of SFXs
        /// <param name="sfxs">The list of SFXs that will have one of it's SFXs randomly selected to be played.</param>
        /// </summary>
        public static void PlaySFX(List<SFX> sfxs)
        {
            SFX targetSFX = sfxs.Random();

            if(targetSFX.Clip == null) return;

            Instance.StartCoroutine(SFXCoroutine(targetSFX));
        }

        private void Init()
        {
            if(!initted)
            {
                initted = true;

                // Set SoundManager as a child of the main camera
                Transform mainCamera = Camera.main.transform;
                transform.SetParent(mainCamera, false);
                transform.position = mainCamera.position;

                // Load the saved volumes and enabled values
                ostVolume = PlayerPrefs.GetFloat(OST_KEY, 0.5f);
                sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1);                    
                ostEnabled = PlayerPrefs.GetInt(OST_ENABLED_KEY, 1) == 1;
                sfxEnabled = PlayerPrefs.GetFloat(SFX_ENABLED_KEY, 1) == 1;

                // Start the main OST coroutine
                StartCoroutine(OSTCoroutine());
            }
        }

        private void Start()
        {
            if(donotuse_instance == null)
            {
                donotuse_instance = this;
            }
            else
            {
                if(donotuse_instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            Init();
        }

        private void OnDestroy()
        {
            if(donotuse_instance == this)
            {
                // Store the current OST and SFX volume and enabled values in the PlayerPrefs
                PlayerPrefs.SetFloat(OST_KEY, SoundManager.OSTVolume);
                PlayerPrefs.SetFloat(SFX_KEY, SoundManager.SFXVolume);
                PlayerPrefs.SetInt(OST_ENABLED_KEY, SoundManager.OSTEnabled ? 1 : 0);
                PlayerPrefs.SetInt(SFX_ENABLED_KEY, SoundManager.SFXEnabled ? 1 : 0);
                
                donotuse_instance = null;
            }
        }

        private static IEnumerator OSTCoroutine()
        {
            // Initialize the OST's AudioSource and set it's parent to be the SoundManager
            string sourceName = "OST Source - Now Playing: ";

            AudioSource source = (new GameObject(sourceName + "N/A")).AddComponent<AudioSource>();

            source.transform.parent = Instance.transform;
            source.transform.position = Instance.transform.position;
            source.transform.SetAsFirstSibling();
            source.loop = true;
            source.volume = 0;

            while(true)
            {   
                // Wait for a OST to be set
                if(Instance.activeOST.Clip == null)
                    yield return new WaitWhile(() => Instance.activeOST.Clip == null);

                float halfTransitionDuration = Instance.ostTransitionDuration / 2;

                // Tween the volume of the previous OST to 0
                if(source.clip != null)
                {
                    Tweener.TweenValue(() => source.volume,
                                       (x) => source.volume = x,
                                       0,
                                       halfTransitionDuration)
                                       .SetTweenType(Tweener.TweenType.Smooth);

                    yield return new WaitForSeconds(halfTransitionDuration);
                }

                // Change the source to play the current OST
                source.Stop();
                source.clip = Instance.activeOST.Clip;
                source.gameObject.name = sourceName + (source.clip == null ? "N/A" : source.clip.name);
                source.time = 0;
                source.Play();

                halfTransitionDuration = Instance.ostTransitionDuration / 2;

                // Tween the volume of the current OST to the current volume
                Tweener.TweenValue(() => source.volume,
                                   (x) => source.volume = x,
                                   GetOSTVolume(Instance.activeOST.Volume),
                                   halfTransitionDuration)
                                   .SetTweenType(Tweener.TweenType.Smooth);

                yield return new WaitForSeconds(halfTransitionDuration);

                // wait for the OST to be changed, updating the OST source volume if it is changed
                bool waiting = true;
                while(waiting)
                {
                    if(Instance.activeOST.Clip != source.clip)
                    {
                        waiting = false;
                    }
                    else
                    {
                        float vol = GetOSTVolume(Instance.activeOST.Volume);

                        if(source.volume != vol)
                            source.volume = vol;
                    }

                    yield return null;
                }
            }
        }

        private static IEnumerator SFXCoroutine(SFX sfx)
        {
            // Initialize the SFX's AudioSource and set it's parent to be the SoundManager
            AudioSource source = (new GameObject("SFX: " + sfx.Clip.name)).AddComponent<AudioSource>();

            source.transform.parent = Instance.transform;
            source.transform.position = Instance.transform.position;
            source.volume = GetSFXVolume(sfx.Volume);
            source.clip = sfx.Clip;
            source.time = sfx.StartTime >= 0 ? sfx.StartTime : 0;
            
            // Delay the start of the SFX if the StartTime is negative
            if(sfx.StartTime < 0)
            {
                source.Stop();
                yield return new WaitForSeconds(-sfx.StartTime);
            }
            source.Play();

            yield return new WaitWhile(() => source.isPlaying);
            Destroy(source.gameObject);
        }

        private static float GetOSTVolume(float volume)
        {
            // Calculate final OST volume, applying the power of the simplified Euler's Number
            return SoundManager.OSTEnabled ? Mathf.Pow((volume/100) * SoundManager.OSTVolume, 2.7f) : 0;
        }

        private static float GetSFXVolume(float volume)
        {
            // Calculate final SFX volume, applying the power of the simplified Euler's Number
            return SoundManager.SFXEnabled ? Mathf.Pow((volume/100) * SoundManager.SFXVolume, 2.7f) : 0;
        }
    }

    [System.Serializable]
    public struct SFX
    {
        public AudioClip Clip;
        public float StartTime;
        [Range(0f,100f)]
        public float Volume;
    }

    [System.Serializable]
    public struct OST
    {
        public AudioClip Clip;
        [Range(0f,100f)]
        public float Volume;
    }
}

