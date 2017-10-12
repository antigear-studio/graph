using UnityEngine;

namespace Devdayo
{
	public struct Device
	{
		public string name;
		public Vector2 pixel;
		public float dpi;
		
		public Device(string name, Vector2 pixel, float dpi)
		{
			this.name = name;
			this.pixel = pixel;
			this.dpi = dpi;
		}
		
		public Vector2 inch { get { return this.pixel / this.dpi; } }
		public Vector2 dp 	{ get { return this.pixel / this.scaleFactor; } }
		
		public Vector2 actual { get { return this.inch * Screen.dpi; } }
		
		public float diagonal { get { return this.inch.magnitude; } }
		public float scaleFactor { get { return this.dpi / 160f; } }
		
		public Device landscape
		{
			get
			{
				// Clone (not reference because it's a struct)
				Device d = this;
				d.pixel = new Vector2(this.pixel.y, this.pixel.x);
                
                // return value type.
                return d;
            }
        }
        
		public int type
		{
			get
			{
				#if UNITY_STANDALONE && !UNITY_EDITOR
				return 0;
				#endif
				
				return (diagonal >= 6.0f) ? 1 : 2;	
			}
		}
		
		public int orientation
		{
			get
			{
				return (pixel.x < pixel.y) ? 0 : 1;
			}
		}
		
		
		
		
		// Classifiers
		// Width & Height based on Portrait Orientation
		private static readonly Device[] all = 
		{
			// DPI from http://dpi.lv/
			
			// Default : Don't Use (Please see "Get" function below).
			new Devdayo.Device("Default", Vector2.one, 1),
			
			// Android
			new Devdayo.Device("Nexus 4 (4.7\", 768 x 1280: xhdpi)", 		new Vector2(768, 1280),  318),
			new Devdayo.Device("Nexus 5 (5.0\", 1080 x 1920: xxhdpi)", 		new Vector2(1080, 1920), 443),
			new Devdayo.Device("Nexus 6 (6.0\", 1440 x 2560: 560dpi)", 		new Vector2(1440, 2560), 560),
			
			new Devdayo.Device("Nexus 7 (2013) (7.0\", 1200 x 1920: xhdpi)", new Vector2(1200, 1920), 323),
			new Devdayo.Device("Nexus 7 (2012) (7.0\", 800 x 1280: tvdpi)",  new Vector2(800,  1280), 216),
			
			new Devdayo.Device("Nexus 9 (8.9\", 1536 x 2048: xhdpi)", 		new Vector2(1536, 2048), 288),
			new Devdayo.Device("Nexus 10 (10.1\", 1600 x 2560: xhdpi)", 	 	new Vector2(1600, 2560), 300),
			
			new Devdayo.Device("Nexus S (4.0\", 480 x 800: hdpi)", 			new Vector2(480, 800),   240),
			new Devdayo.Device("Nexus One (3.7\", 480 x 800: hdpi)", 		new Vector2(480, 800),   240),
			new Devdayo.Device("Galaxy Nexus (4.7\", 720 x 1280: xhdpi)", 	new Vector2(720, 1280),  316),
			
			// Apple
			new Devdayo.Device("Apple iPhone 1, 3G, 3GS (3.5\", 320 x 480: 163dpi)", new Vector2(320, 480),   163),
			new Devdayo.Device("Apple iPhone 4, 4S (3.5\", 640 x 960: 326dpi)", 	 	new Vector2(640, 960),   326),
			new Devdayo.Device("Apple iPhone 5 (4\", 640 x 1136: 326dpi)", 	 		new Vector2(640, 1136),  326),
			new Devdayo.Device("Apple iPhone 6 (4.7\", 750 x 1334: 326dpi)", 		new Vector2(750, 1334),  326),
			new Devdayo.Device("Apple iPhone 6+ (5.5\", 1080 x 1980: 401dpi)", 		new Vector2(1080, 1980), 401),
			
			new Devdayo.Device("Apple iPad 1, 2 (9.7\", 768 x 1024: 132dpi)", 		new Vector2(768, 1024),  132),
			new Devdayo.Device("Apple iPad 3, 4 (9.7\", 1536 x 2048: 264dpi)", 		new Vector2(1536, 2048), 264),
			new Devdayo.Device("Apple iPad Air 1, 2 (9.7\", 1536 x 2048: 264dpi)", 	new Vector2(1536, 2048), 264),
			
			new Devdayo.Device("Apple iPad Mini 1 (7.9\", 768 x 1024: 163dpi)", 	 	new Vector2(768, 1024),  163),
			new Devdayo.Device("Apple iPad Mini 2, 3 (7.9\", 1536 x 2048: 324dpi)",  new Vector2(1536, 2048), 324),
		};
		
		public static Device Get(int index, int orientation)
		{
			if(index > 0)
			{
				Device device = Device.all[index];
				return (orientation == 0) ? device : device.landscape;
			}
			
			string name = "Free Aspect (Desktop: "+Screen.dpi+"dpi)";
			Vector2 pixel = new Vector2(Screen.height, Screen.width);
			float dpi = Screen.dpi;
			
			Device desktop = new Device(name, pixel, dpi);
			return (orientation == 0) ? desktop : desktop.landscape;
		}
		
		public static string[] _orientations = { "Portrait", "Landscape" };
		public static string[] orientations
		{
			get
			{
				return _orientations;
            }
        }
        
        public static string[] _types = { "Desktop", "Tablet", "Mobile" };
        public static string[] types
        {
            get
            {
                return _types;
            }
        }
        
        private static string[] _names = {};
        public static string[] names
        {
            get
            {
                if(0 == _names.Length)
                {
                    int length = all.Length;
                    _names = new string[length];
                    
                    for (int i = 0; i < length; i++) 
                    {
                        _names[i] = Device.Get(i, 0).name;
                    }
                }
                
                return _names;
            }
        }
    }
    
    
}