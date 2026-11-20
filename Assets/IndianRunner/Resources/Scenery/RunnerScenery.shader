Shader "IndianRunner/SceneryUnlit"
{
    Properties
    {
        _MainTex ("Artwork", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _Saturation ("Natural colour saturation", Range(0,1)) = 0.82
        _CutSky ("Cut blue sky from upper facade", Float) = 0
        _Cloudiness ("Weather tint", Range(0,1)) = 0
        _ZWrite ("Write depth", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        Pass
        {
            // No LightMode tag: Always in built-in, SRPDefaultUnlit in URP.
            Cull Off
            ZWrite [_ZWrite]
            ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Tint;
            float _CutSky, _Cloudiness, _Saturation;
            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                // The generated frontage has blue sky above its roofline. Trim
                // only that upper blue region; turquoise shop fabric stays intact.
                if (_CutSky > .5 && i.uv.y > .72 && c.b > c.r * 1.18 && c.g > c.r * 1.08) discard;
                float luminance = dot(c.rgb,fixed3(.2126,.7152,.0722));
                c.rgb = lerp(luminance.xxx,c.rgb,_Saturation);
                c.rgb *= _Tint.rgb;
                c.rgb = lerp(c.rgb, c.rgb * fixed3(.77,.84,.91), _Cloudiness * .55);
                return fixed4(c.rgb, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
