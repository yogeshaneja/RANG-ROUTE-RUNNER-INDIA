using System;
using UnityEngine;

namespace IndianRunner
{
    // Four tiny bundled scenic previews, cached for this screen's lifetime.
    public sealed class StageSelectionScreen : IDisposable
    {
        public enum Action { None, Back, Start }
        private readonly Texture2D[] pictures=new Texture2D[RegionProfile.Count];
        private readonly int[] best=new int[RegionProfile.Count];
        private readonly string[] bestText=new string[RegionProfile.Count];
        private readonly string[] names={"Rajasthan","Kerala","Punjab","Goa"};
        private readonly string[] descriptions={"Palace roads & golden dunes","Backwaters, bridges & rain","Harvest fields & village lanes","Ocean views & colourful streets"};
        private readonly string[] highlights={"Camels, pottery & mirror tokens","Forest wildlife & lotus tokens","Tractors, buffalo & Phulkari tokens","Crabs, fishing boats & shell tokens"};
        private Vector2 scroll;
        public int SelectedRegion { get; private set; }
        public StageSelectionScreen()
        {
            for(int i=0;i<pictures.Length;i++) pictures[i]=Resources.Load<Texture2D>("StageSelection/"+names[i]);
        }
        public void Open(int current)
        {
            SelectedRegion=RegionProfile.Valid(current);scroll=Vector2.zero;
            for(int i=0;i<best.Length;i++)
            {
                best[i]=PlayerPrefs.GetInt(RegionProfile.BestKey(i),0);
                bestText[i]="PERSONAL BEST  "+best[i].ToString("N0");
            }
        }
        public Action Draw(float left,float top,float width,float bottom,RunnerSkin skin,bool enabled)
        {
            bool previousEnabled=GUI.enabled;GUI.enabled=enabled;
            Color previousColour=GUI.color;
            GUI.color=new Color(.025f,.06f,.075f,.97f);
            GUI.DrawTexture(new Rect(left,top-10,width,bottom-top+22),Texture2D.whiteTexture);GUI.color=previousColour;
            GUI.Label(new Rect(left+20,top+5,width-40,24),"YOUR JOURNEY ACROSS INDIA   /   STEP 2",skin.Caption);
            GUI.Label(new Rect(left+18,top+30,width-36,43),"CHOOSE YOUR STATE",skin.StageTitle);
            GUI.Label(new Rect(left+24,top+76,width-48,23),"Pick your first stop. More states follow as you run.",skin.Caption);
            float gap=12,footerTop=bottom-164;
            var viewport=new Rect(left+20,top+108,width-40,Mathf.Max(160,footerTop-top-120));
            float cardHeight=Mathf.Clamp((viewport.height-gap)*.5f,224,246);
            float contentHeight=cardHeight*2+gap;
            float contentWidth=viewport.width-(contentHeight>viewport.height ? 18:0);
            float cardWidth=(contentWidth-gap)*.5f;
            scroll=GUI.BeginScrollView(viewport,scroll,new Rect(0,0,contentWidth,contentHeight));
            for(int i=0;i<RegionProfile.Count;i++)
            {
                var card=new Rect((i%2)*(cardWidth+gap),(i/2)*(cardHeight+gap),cardWidth,cardHeight);
                bool selected=i==SelectedRegion;
                if(GUI.Button(card,GUIContent.none,selected ? skin.Primary:skin.Secondary)) SelectedRegion=i;
                GUI.Box(new Rect(card.x+3,card.y+3,card.width-6,card.height-6),GUIContent.none,skin.Card);
                float imageHeight=Mathf.Min((cardWidth-16)*.5625f,112);
                var picture=new Rect(card.x+8,card.y+8,card.width-16,imageHeight);
                if(pictures[i]!=null) GUI.DrawTexture(picture,pictures[i],ScaleMode.ScaleAndCrop);
                else GUI.Box(picture,names[i],skin.Chip);
                if(selected)
                {
                    var badge=new Rect(picture.xMax-83,picture.y+7,76,23);
                    GUI.Box(badge,GUIContent.none,skin.Chip);GUI.Label(badge,"SELECTED",skin.Caption);
                }
                float labelTop=picture.yMax+7;
                GUI.Label(new Rect(card.x+12,labelTop,card.width-24,28),RegionProfile.Name(i),skin.StageName);
                GUI.Label(new Rect(card.x+12,labelTop+30,card.width-24,37),descriptions[i],skin.StageDetail);
                GUI.Label(new Rect(card.x+12,card.yMax-29,card.width-24,23),bestText[i],skin.Caption);
            }
            GUI.EndScrollView();
            GUI.Label(new Rect(left+20,footerTop,width-40,28),"FIRST STOP: "+RegionProfile.Name(SelectedRegion),skin.Subtitle);
            GUI.Label(new Rect(left+24,footerTop+29,width-48,24),highlights[SelectedRegion],skin.Caption);
            Action result=Action.None;
            if(GUI.Button(new Rect(left+24,footerTop+60,width-48,48),"START "+RegionProfile.Name(SelectedRegion)+" RUN  >",skin.Primary))result=Action.Start;
            if(GUI.Button(new Rect(left+24,footerTop+114,width-48,42),"<  BACK TO WELCOME",skin.Secondary))result=Action.Back;
            GUI.enabled=previousEnabled;GUI.color=previousColour;return result;
        }
        public void Dispose()
        {
            for(int i=0;i<pictures.Length;i++)
            {
                if(pictures[i]!=null)Resources.UnloadAsset(pictures[i]);pictures[i]=null;
            }
        }
    }
}
