using UnityEngine;
using UnityEngine.Audio;

namespace DEEP.Stage
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup musicMixer;
        [SerializeField] private double songSwapLoadTime = 1;

        private AudioSource introSource = null;
        private AudioSource[] loopSource = new AudioSource[2]; // Two audio sources for a seamless loop
        private double nextEventTime;
        private bool running = false, paused = false;
        private int source = 0;
        
        public static MusicPlayer instance;

        private void Awake()
        {
            if(instance == null)
                instance = this;
            else if(instance != this)
                Destroy(this.gameObject);

            // Creates audio sources
            AudioSource[] sources = new AudioSource[3];
            for(int i = 0; i < 3; i++)
            {
                sources[i] = gameObject.AddComponent<AudioSource>();
                sources[i].outputAudioMixerGroup = musicMixer;
            }

            // Sets two audio sources for the loop, one each the intro and outro
            introSource = sources[0];
            loopSource[0] = sources[1];
            loopSource[1] = sources[2];
        }

        /// <summary>
        ///     Starts song playback without intro.
        /// </summary>
        public void StartSong(AudioClip loopClip)
        {
            StartSong(loopClip, false, null);
        }        

        /// <summary>
        ///     Starts song playback with intro.
        /// </summary>
        public void StartSong(AudioClip introClip, AudioClip loopClip)
        {
            StartSong(loopClip, true, introClip);
        }

        /// <summary>
        ///     This method is used to start the intro-then-loop process.
        ///     The song swap load time is important because in order to syncronize the clips, we need to schedule them exactly. And to schedule clips,
        /// we need a loading time because the scheduling operation takes some time.
        /// </summary>
        private void StartSong(AudioClip loopClip, bool hasIntro, AudioClip introClip)
        {
            // Sets clips
            introSource.clip = introClip;
            loopSource[0].clip = loopClip;
            loopSource[1].clip = loopClip;

            // Set start time with enough loading time buffer
            double startTime = AudioSettings.dspTime + songSwapLoadTime;

            if(hasIntro)
            {
                // Starts Intro
                introSource.PlayScheduled(startTime);

                // Set swap time to loop start
                double introClipDuration = (double)introClip.samples / introClip.frequency;
                nextEventTime = startTime + introClipDuration;
            }
            else
            {
                // Starts loop right away
                loopSource[source].PlayScheduled(startTime);

                // Set swap time to loop end
                double loopClipDuration = (double)loopClip.samples / loopClip.frequency;
                nextEventTime = startTime + loopClipDuration;
            }

            // Sets up looping checks in update
            running = true;
        }

        /// <summary>
        ///     Handles the looping
        /// </summary>
        private void Update()
        {
            if(!running)
                return;

            double time = AudioSettings.dspTime;

            if (time + songSwapLoadTime > nextEventTime)
            {
                // We are now nearing the time at which the sound should play,
                // so we will schedule it now in order for the system to have enough time
                // to prepare the playback at the specified time. This may involve opening
                // buffering a streamed file and should therefore take any worst-case delay into account.
                
                // Swap source
                source = 1 - source;

                // Schedules play
                loopSource[source].PlayScheduled(nextEventTime);

                // Place the next event
                double loopClipDuration = (double)loopSource[0].clip.samples / loopSource[0].clip.frequency;
                nextEventTime += loopClipDuration;
            }
        }

        /// <summary>
        ///     Pauses audio listener because that's the only way to pause the scheduled playbacks. That means that for UI sounds and such
        /// they'll need to have the AudioSource.ignoreListenerPause property set to true. Blame Unity.
        /// </summary>
        public void Pause()
        {
            paused = !paused;
            AudioListener.pause = paused;
        }

        /// <summary>
        ///     Stops the stage music altogether.
        /// </summary>
        public void Stop()
        {
            introSource.Stop();
            loopSource[0].Stop();
            loopSource[1].Stop();
            running = false;
        }
    }
}