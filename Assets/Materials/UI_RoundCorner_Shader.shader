Shader "Custom/NewUnlitUniversalRenderPipelineShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Radius("Corner Radius", Range(0,0.5)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Radius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // center UV around 0.5
                uv = uv * 2.0 - 1.0;

                // compute distance from corner
                float2 corner = abs(uv) - (1.0 - _Radius * 2.0);
                float dist = length(max(corner, 0.0));

                // alpha mask
                float alpha = smoothstep(_Radius, _Radius - 0.01, dist);

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}
