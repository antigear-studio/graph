using UnityEngine;

namespace Devdayo
{
    public struct Resolution
    {
        public string name;
        public Vector2 size;

        public Resolution(int w, int h)
        {
            this.name = w + " x " + h +" px";
            this.size = new Vector2(w, h);
        }

        // ref : https://en.wikipedia.org/wiki/Graphics_display_resolution
        private static readonly Resolution[] all = 
		{
            new Devdayo.Resolution(800,  600),
            new Devdayo.Resolution(1024, 600),
            new Devdayo.Resolution(1024, 768),
            new Devdayo.Resolution(1152, 864),
            new Devdayo.Resolution(1280, 720),
            new Devdayo.Resolution(1280, 768),
            new Devdayo.Resolution(1280, 800),
            new Devdayo.Resolution(1280, 960),
            new Devdayo.Resolution(1280, 1024),
            new Devdayo.Resolution(1360, 768),
            new Devdayo.Resolution(1366, 768),
            new Devdayo.Resolution(1400, 1050),
            new Devdayo.Resolution(1440, 900),
            new Devdayo.Resolution(1600, 900),
            new Devdayo.Resolution(1600, 1200),
            new Devdayo.Resolution(1680, 1050),
            new Devdayo.Resolution(1920, 1080),
            new Devdayo.Resolution(1920, 1200),
            new Devdayo.Resolution(2048, 1152),
            new Devdayo.Resolution(2560, 1440),
            new Devdayo.Resolution(2560, 1600),
            new Devdayo.Resolution(2880, 1800),
        };


        public static Resolution Get(int index)
        {
            return Resolution.all[index];
        }

        private static string[] _names = { };
        public static string[] names
        {
            get
            {
                if (0 == _names.Length)
                {
                    int length = all.Length;
                    _names = new string[length];

                    for (int i = 0; i < length; i++)
                    {
                        _names[i] = Resolution.Get(i).name;
                    }
                }

                return _names;
            }
        }
    }

}