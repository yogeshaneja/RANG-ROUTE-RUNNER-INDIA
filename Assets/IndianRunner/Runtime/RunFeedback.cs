using UnityEngine;

namespace IndianRunner
{
    // Small bounded pool. Cosmetic effects never participate in collision or score.
    public sealed class RunFeedback
    {
        private readonly Transform[] motes = new Transform[18];
        private readonly Vector3[] velocity = new Vector3[18];
        private readonly float[] life = new float[18];
        private int cursor;
        public RunFeedback(Transform parent)
        {
            var gold = PrimitiveArt.Mat(new Color(1,.74f,.20f));
            for (int i = 0; i < motes.Length; i++)
            {
                motes[i] = NaturalMesh.Oval(parent,"Pooled coin glint",Vector3.zero,Vector3.one*.07f,gold,6);
                motes[i].GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                motes[i].gameObject.SetActive(false);
            }
        }
        public void Coin(float x, float y, bool reduced)
        {
            int count = reduced ? 2 : 6;
            for (int i = 0; i < count; i++)
            {
                int at = cursor++%motes.Length; float angle = (i*137.5f+cursor*19)*Mathf.Deg2Rad;
                motes[at].localPosition = new Vector3(x,y+.85f,.25f);
                velocity[at] = new Vector3(Mathf.Cos(angle)*1.1f,.9f+i*.08f,Mathf.Sin(angle)*.55f-.3f);
                life[at] = .42f; motes[at].gameObject.SetActive(true);
            }
        }
        public void Tick(float dt)
        {
            for (int i = 0; i < life.Length; i++)
            {
                if (life[i] <= 0) continue;
                life[i] -= dt; velocity[i] += Vector3.down*(dt*2.4f);
                motes[i].localPosition += velocity[i]*dt;
                motes[i].localScale = Vector3.one*Mathf.Max(.05f,life[i]/.42f);
                if (life[i] <= 0) motes[i].gameObject.SetActive(false);
            }
        }
        public void Reset()
        {
            cursor = 0;
            for (int i = 0; i < life.Length; i++) { life[i] = 0; motes[i].localScale = Vector3.one; motes[i].gameObject.SetActive(false); }
        }
    }
}
