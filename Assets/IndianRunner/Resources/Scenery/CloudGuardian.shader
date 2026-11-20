Shader "IndianRunner/CloudGuardian"
{
    Properties
    {
        _Color ("Colour", Color) = (1,1,1,1)
        _Opacity ("Reveal opacity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 vertex : SV_POSITION; float3 normal : TEXCOORD0; float4 screen : TEXCOORD1; };
            fixed4 _Color;
            float _Opacity;
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex=UnityObjectToClipPos(v.vertex);
                o.normal=UnityObjectToWorldNormal(v.normal);
                o.screen=ComputeScreenPos(o.vertex);
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float shade=.72+.28*saturate(dot(normalize(i.normal),normalize(float3(-.3,.8,-.5))));
                // Keep the lower gameplay view clear even if camera framing changes.
                float skyMask=smoothstep(.54,.63,i.screen.y/i.screen.w);
                return fixed4(_Color.rgb*shade,_Color.a*_Opacity*skyMask);
            }
            ENDCG
        }
    }
    Fallback Off
}
