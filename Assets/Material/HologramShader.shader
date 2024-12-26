Shader "UI/HologramShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1
        _GlowColor ("Glow Color", Color) = (0, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
        _GlitchSpeed ("Glitch Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Alpha;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlitchSpeed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float glitch = sin(_Time.y * _GlitchSpeed + i.uv.x * 10) * 0.1;
                col.rgb += _GlowColor.rgb * _GlowIntensity * glitch;
                col.a *= _Alpha;
                return col;
            }
            ENDCG
        }
    }
}
