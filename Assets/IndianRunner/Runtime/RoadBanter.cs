using System;

namespace IndianRunner
{
    // Original playful, Rajasthan-flavoured Hindi drafts; not verified Marwari.
    // English translations pair with each Hindi line. Readable Roman Hindi avoids relying
    // on unverified Devanagari shaping in the prototype's IMGUI subtitle font.
    public sealed class RoadBanter
    {
        public struct Line
        {
            public string Speaker, Hindi, Caption, English;
            public float Pitch, Rate;
            public Line(string speaker,string hindi,string caption,string english,float pitch,float rate)
            { Speaker=speaker; Hindi=hindi; Caption=caption; English=english; Pitch=pitch; Rate=rate; }
            public string Text(bool english) => english ? English : Hindi;
            public string Subtitle(bool english) => english ? English : Caption;
        }
        private static readonly Line[][] Lines={
            new[]{
                new Line("MOTORCYCLE RIDER","ज़रा किनारे हो जा, महाराजा!","Zara kinaare ho ja, Maharaja!","Move aside a little, Maharaja!",.90f,1.05f),
                new Line("MOTORCYCLE RIDER","ओ हीरो! बीच सड़क पर नाच मत!","O hero! Beech sadak par naach mat!","Hey, hero! No dancing in the middle of the road!",.90f,1.05f),
                new Line("MOTORCYCLE RIDER","ओ छोरा! आँखें खोल के चल!","O chhora! Aankhein khol ke chal!","Hey, lad! Keep your eyes on the road!",.92f,1.05f)},
            new[]{
                new Line("PEACOCK - PLAYFUL VOICE","पहले म्हारा नाच देख ले, छोरा!","Pehle mhara naach dekh le, chhora!","Watch my dance first, young fellow!",1.20f,1.02f),
                new Line("PEACOCK - PLAYFUL VOICE","पंख देख, रास्ता छोड़!","Pankh dekh, raasta chhod!","Admire the feathers, then clear the way!",1.22f,1.05f),
                new Line("PEACOCK - PLAYFUL VOICE","मोर महाराजा को जगह दे ज़रा!","Mor Maharaja ko jagah de zara!","Make some room for the peacock king!",1.18f,1.00f)},
            new[]{
                new Line("CAMEL - PLAYFUL VOICE","ओ छोरा, म्हारी चाल तो शाही है!","O chhora, mhari chaal toh shaahi hai!","Hey, lad! Now that is a royal walk!",.82f,.94f),
                new Line("CAMEL - PLAYFUL VOICE","इतनी जल्दी किस बात की, महाराजा?","Itni jaldi kis baat ki, Maharaja?","What is the hurry, Maharaja?",.84f,.94f),
                new Line("CAMEL - PLAYFUL VOICE","रेत का राजा आ रहा है, हट ज़रा!","Ret ka raja aa raha hai, hat zara!","The king of the sands is coming! Make way!",.82f,.96f)},
            new[]{
                new Line("ELEPHANT - PLAYFUL VOICE","ज़रा हट जा, बड़ा मेहमान आया है!","Zara hat ja, bada mehmaan aaya hai!","Move aside! A very big guest has arrived!",.74f,.90f),
                new Line("ELEPHANT - PLAYFUL VOICE","धीरे छोरा, भारी सवारी आ रही है!","Dheere chhora, bhaari sawaari aa rahi hai!","Easy, lad! A heavy ride is coming through!",.75f,.92f),
                new Line("ELEPHANT - PLAYFUL VOICE","महाराजा को रास्ता दे!","Maharaja ko raasta de!","Make way for the Maharaja!",.76f,.92f)},
            new[]{
                new Line("FESTIVAL CALLER","ओ छोरा, मेले की शान आ रही है!","O chhora, mele ki shaan aa rahi hai!","Hey, lad! Here comes the pride of the fair!",1.02f,1.04f),
                new Line("FESTIVAL CALLER","रास्ता दे, रंगों की सवारी आई!","Raasta de, rangon ki sawaari aayi!","Make way! The parade of colours has arrived!",1.04f,1.04f),
                new Line("FESTIVAL CALLER","नाच बाद में, पहले रास्ता दे!","Naach baad mein, pehle raasta de!","Dance later! Clear the way first!",1.02f,1.04f)},
            new[]{new Line("LADY WITH UMBRELLA","देखो, बारिश हो रही है! इट्स रेनिंग, इट्स रेनिंग!","Dekho, baarish ho rahi hai! It's raining, it's raining!","Look, it is raining! It is raining, it is raining!",1.12f,.98f)}
        };
        private readonly int[] next=new int[Lines.Length];
        private int lastKey=-1,lastKind=-1;
        private float nextAllowed,visibleUntil;
        public Line Current { get; private set; }
        public static Line Preview => Lines[0][0];
        public static Line RainPreview => Lines[5][0];
        public bool Visible(float clock) => clock<visibleUntil && !string.IsNullOrEmpty(Current.Caption);
        public void Reset(int seed)
        {
            for(int i=0;i<next.Length;i++) next[i]=(int)(((uint)seed+(uint)i*7)%(uint)Lines[i].Length);
            lastKey=lastKind=-1; nextAllowed=visibleUntil=0; Current=default(Line);
        }
        private static int Group(int kind) => kind==RajasthanObstacles.Motorcycle ? 0 : kind==RajasthanObstacles.Peacock ? 1 : kind==RajasthanObstacles.Camel ? 2 : kind==RajasthanObstacles.ElephantSound ? 3 : kind==RajasthanObstacles.Procession ? 4 : kind==RajasthanObstacles.UmbrellaLady ? 5 : -1;
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
