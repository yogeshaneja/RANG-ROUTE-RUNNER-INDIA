using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    // Cached native UI skin: no package download and no texture creation per frame.
    public sealed class RunnerSkin
    {
        public GUIStyle Title, Subtitle, Text, Caption, Score, Card, Chip, Primary, Secondary, Toggle;
        private readonly List<Texture2D> textures = new List<Texture2D>();
        public RunnerSkin()
        {
            var ink = new Color(.035f,.055f,.065f,.94f);
            var glass = new Color(.05f,.075f,.085f,.88f);
            var gold = new Color(.96f,.68f,.25f);
            Title = Label(40,FontStyle.Bold,new Color(1,.89f,.66f),TextAnchor.MiddleCenter);
            Subtitle = Label(15,FontStyle.Bold,new Color(.92f,.84f,.69f),TextAnchor.MiddleCenter);
            Text = Label(17,FontStyle.Normal,new Color(.94f,.95f,.92f),TextAnchor.MiddleCenter);
            Caption = Label(12,FontStyle.Normal,new Color(.71f,.77f,.75f),TextAnchor.MiddleCenter);
            Score = Label(28,FontStyle.Bold,Color.white,TextAnchor.MiddleLeft);
            Card = Panel(glass);
            Chip = Panel(ink); Chip.fontSize = 18; Chip.fontStyle = FontStyle.Bold; Chip.alignment = TextAnchor.MiddleCenter; Chip.normal.textColor = Color.white;
            Primary = Button(gold,new Color(.14f,.09f,.035f),21);
            Secondary = Button(new Color(.13f,.18f,.19f,.95f),new Color(.95f,.93f,.84f),17);
            Toggle = new GUIStyle(Secondary) { fontSize = 14 };
        }
        private static GUIStyle Label(int size, FontStyle weight, Color colour, TextAnchor alignment)
        {
            var style = new GUIStyle(GUI.skin.label) { fontSize = size,fontStyle = weight,alignment = alignment,wordWrap = true,padding = new RectOffset(4,4,2,2) };
            style.normal.textColor = colour; return style;
        }
        private GUIStyle Panel(Color colour)
        {
            var style = new GUIStyle(GUI.skin.box) { border = new RectOffset(16,16,16,16),padding = new RectOffset(12,12,10,10) };
            style.normal.background = Rounded(colour); return style;
        }
        private GUIStyle Button(Color colour, Color text, int size)
        {
            var style = new GUIStyle(GUI.skin.button) { fontSize = size,fontStyle = FontStyle.Bold,alignment = TextAnchor.MiddleCenter,wordWrap = true,border = new RectOffset(16,16,16,16),padding = new RectOffset(10,10,6,6) };
            style.normal.background = Rounded(colour); style.hover.background = Rounded(Color.Lerp(colour,Color.white,.08f));
            style.active.background = Rounded(Color.Lerp(colour,Color.black,.18f));
            style.normal.textColor = style.hover.textColor = style.active.textColor = text;
            return style;
        }
        private Texture2D Rounded(Color colour)
        {
            const int size = 64; const float radius = 15;
            var tex = new Texture2D(size,size,TextureFormat.RGBA32,false) { name = "Runner cached rounded UI",filterMode = FilterMode.Bilinear,wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[size*size];
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float cx = Mathf.Clamp(x,radius,size-1-radius),cy = Mathf.Clamp(y,radius,size-1-radius);
                float distance = Vector2.Distance(new Vector2(x,y),new Vector2(cx,cy));
                var c = Color.Lerp(colour,colour*new Color(.91f,.91f,.91f,1),1-y/(size-1f));
                c.a = colour.a*Mathf.Clamp01(radius-distance+.5f); pixels[y*size+x] = c;
            }
            tex.SetPixels32(pixels); tex.Apply(false,true); textures.Add(tex); return tex;
        }
        public void Dispose() { foreach (var tex in textures) if (tex != null) Object.Destroy(tex); textures.Clear(); }
    }
}
