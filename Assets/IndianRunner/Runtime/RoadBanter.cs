using System;
using System.Collections.Generic;

namespace IndianRunner
{
    // Original playful, Rajasthan-flavoured Hindi drafts; not verified Marwari.
    // English translations pair with each Hindi line. Readable Roman Hindi avoids relying
    // on unverified Devanagari shaping in the prototype's IMGUI subtitle font.
    public enum VoiceRole { Rider, Peacock, Camel, Elephant, Caller, Lady }

    public sealed class RoadBanter
    {
        public struct Line
        {
            public string Speaker, Hindi, Caption, English;
            public float Pitch, Rate;
            public VoiceRole Role;
            public int Variant;
            public string AudioKey => Role.ToString().ToLowerInvariant()+"_"+Variant;
            public Line(string speaker,string hindi,string caption,string english,float pitch,float rate,VoiceRole role,int variant)
            { Speaker=speaker; Hindi=hindi; Caption=caption; English=english; Pitch=pitch; Rate=rate; Role=role; Variant=variant; }
            public string Text(bool english) => english ? English : Hindi;
            public string Subtitle(bool english) => english ? English : Caption;
        }
        private static readonly Line[][] Lines={
            new[]{
                new Line("MOTORCYCLE RIDER","ज़रा किनारे हो जा, महाराजा!","Zara kinaare ho ja, Maharaja!","Whoa, easy! Make way, Maharaja!",0.97f,1.03f,VoiceRole.Rider,1),
                new Line("MOTORCYCLE RIDER","ओ हीरो! बीच सड़क पर नाच मत!","O hero! Beech sadak par naach mat!","Hey, hero! Save the dancing for later!",1.00f,1.07f,VoiceRole.Rider,2),
                new Line("MOTORCYCLE RIDER","ओ छोरा! आँखें खोल के चल!","O chhora! Aankhein khol ke chal!","Oi, watch the road, my friend!",0.96f,1.01f,VoiceRole.Rider,3)},
            new[]{
                new Line("PEACOCK - PLAYFUL VOICE","पहले म्हारा नाच देख ले, छोरा!","Pehle mhara naach dekh le, chhora!","Watch my dance first, young fellow!",1.07f,1.04f,VoiceRole.Peacock,1),
                new Line("PEACOCK - PLAYFUL VOICE","पंख देख, रास्ता छोड़!","Pankh dekh, raasta chhod!","Admire the feathers, then clear the way!",1.05f,1.09f,VoiceRole.Peacock,2),
                new Line("PEACOCK - PLAYFUL VOICE","मोर महाराजा को जगह दे ज़रा!","Mor Maharaja ko jagah de zara!","Make some room for the peacock king!",1.08f,1.01f,VoiceRole.Peacock,3)},
            new[]{
                new Line("CAMEL - PLAYFUL VOICE","ओ छोरा, म्हारी चाल तो शाही है!","O chhora, mhari chaal toh shaahi hai!","Hey, lad! Now that is a royal walk!",0.94f,0.98f,VoiceRole.Camel,1),
                new Line("CAMEL - PLAYFUL VOICE","इतनी जल्दी किस बात की, महाराजा?","Itni jaldi kis baat ki, Maharaja?","What is the hurry, Maharaja?",0.97f,0.95f,VoiceRole.Camel,2),
                new Line("CAMEL - PLAYFUL VOICE","रेत का राजा आ रहा है, हट ज़रा!","Ret ka raja aa raha hai, hat zara!","The king of the sands is coming! Make way!",0.95f,1.02f,VoiceRole.Camel,3)},
            new[]{
                new Line("ELEPHANT - PLAYFUL VOICE","ज़रा हट जा, बड़ा मेहमान आया है!","Zara hat ja, bada mehmaan aaya hai!","Move aside! A very big guest has arrived!",0.91f,0.97f,VoiceRole.Elephant,1),
                new Line("ELEPHANT - PLAYFUL VOICE","धीरे छोरा, भारी सवारी आ रही है!","Dheere chhora, bhaari sawaari aa rahi hai!","Easy, lad! A heavy ride is coming through!",0.93f,0.94f,VoiceRole.Elephant,2),
                new Line("ELEPHANT - PLAYFUL VOICE","महाराजा को रास्ता दे!","Maharaja ko raasta de!","Make way for the Maharaja!",0.95f,1.00f,VoiceRole.Elephant,3)},
            new[]{
                new Line("FESTIVAL CALLER","ओ छोरा, मेले की शान आ रही है!","O chhora, mele ki shaan aa rahi hai!","Hey, lad! Here comes the pride of the fair!",1.00f,1.06f,VoiceRole.Caller,1),
                new Line("FESTIVAL CALLER","रास्ता दे, रंगों की सवारी आई!","Raasta de, rangon ki sawaari aayi!","Make way! The parade of colours has arrived!",1.02f,1.03f,VoiceRole.Caller,2),
                new Line("FESTIVAL CALLER","नाच बाद में, पहले रास्ता दे!","Naach baad mein, pehle raasta de!","Dance later! Clear the way first!",0.99f,1.08f,VoiceRole.Caller,3)},
            new[]{new Line("LADY WITH UMBRELLA","देखो, बारिश हो रही है! इट्स रेनिंग, इट्स रेनिंग!","Dekho, baarish ho rahi hai! It's raining, it's raining!","Oh, look! It's raining... it's raining!",1.04f,1.01f,VoiceRole.Lady,1)}
        };
        public static IEnumerable<Line> AllLines
        {
            get { foreach(var group in Lines) foreach(var line in group) yield return line; }
        }
        private readonly int[] next=new int[Lines.Length];
        private int lastKey=-1,lastKind=-1;
        private float nextAllowed,visibleUntil;
        public Line Current { get; private set; }
        public static Line Preview => Lines[0][0];
        public static Line RainPreview => Lines[5][0];
        public static Line Sample(VoiceRole role) => Lines[Math.Max(0,Math.Min(Lines.Length-1,(int)role))][0];
        public bool Visible(float clock) => clock<visibleUntil && !string.IsNullOrEmpty(Current.Caption);
        public void Reset(int seed)
        {
            for(int i=0;i<next.Length;i++) next[i]=(int)(((uint)seed+(uint)i*7)%(uint)Lines[i].Length);
            lastKey=lastKind=-1; nextAllowed=visibleUntil=0; Current=default(Line);
        }
        private static int Group(int kind) => kind==RajasthanObstacles.Motorcycle ? 0 : kind==RajasthanObstacles.Peacock ? 1 : kind==RajasthanObstacles.Camel ? 2 : (kind==RajasthanObstacles.ElephantSound || kind==RajasthanObstacles.OncomingElephant) ? 3 : kind==RajasthanObstacles.Procession ? 4 : kind==RajasthanObstacles.UmbrellaLady ? 5 : -1;
        public bool Tick(float clock,int kind,float ahead,int encounterKey,bool reserveForRain=false)
        {
            int group=Group(kind);
            if(group<0 || ahead>8 || ahead< -3 || (lastKey==encounterKey && lastKind==kind)) return false;
            lastKey=encounterKey; lastKind=kind;
            if((reserveForRain && kind!=RajasthanObstacles.UmbrellaLady) || clock<nextAllowed) return false;
            Current=Lines[group][next[group]]; next[group]=(next[group]+1)%Lines[group].Length;
            visibleUntil=clock+4.5f; nextAllowed=clock+9;
            return true;
        }
    }
}
