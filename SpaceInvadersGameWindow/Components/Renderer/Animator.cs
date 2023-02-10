using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace GameWindow.Components.Renderer
{
    internal class Animator
    {
        /*
        private bool loop;
        private SpriteRenderer sR;
        private List<Bitmap> clips = new List<Bitmap>();
        private Dictionary<string, Bitmap> namedClips = new Dictionary<string, Bitmap>();
        private int currentClip;
        public Animator(SpriteRenderer sR, bool loop)
        {
            this.sR = sR;
            this.loop = loop;
            currentClip = 0;
        }

        public void PlayClip(string animName)
        {
            sR.Image = namedClips[animName];
        }
        public void NextClip()
        {
            currentClip++;
            if (loop)
            {
                if (currentClip == clips.Count)
                {
                    currentClip = 0;
                }
                sR.Image = clips[currentClip];
            }
            else
                sR.Image = clips[currentClip];
        }
        public void AddNamedClip(string name, Bitmap clip)
        {
            namedClips.Add(name, clip);
        }
        public void AddClip(Bitmap clip)
        {
            clips.Add(clip);
        }
        */
    }
}
