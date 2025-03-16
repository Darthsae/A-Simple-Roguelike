Shader "ASimpleRoguelike/MagicWaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseText ("Noise Texture", 2D) = "white" {}
        _Scale ("Scale", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _Scale;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                fixed4 offset = tex2D(_NoiseTex, i.uv * _Time * 0.1); // Reduced the speed of noise scrolling

                float timeScale = 2.15 * _Scale * _Time;

                // Using the noise texture to influence the amplitude
                float noiseFactor = offset.r * 0.5 + 0.5; // Normalize noise to 0-1 range
                float amplitudeScale = 0.3 * noiseFactor; // Adjust overall amplitude

                float over = 0.25 * (sin(i.uv.x * 0.025) + cos(i.uv.y * 0.125));

                float val1 = amplitudeScale * cos((i.uv.x + offset.r) * timeScale * 1.5); // Increased frequency
                float val2 = amplitudeScale * 0.8 * sin((i.uv.y * 0.25 - offset.g * 1.5) * timeScale);
                float val3 = amplitudeScale * 1.2 * cos((i.uv.y + offset.r) * timeScale * 0.8) - amplitudeScale * 0.6 * sin((i.uv.x - offset.g) * timeScale * 1.2);

                float val11 = amplitudeScale * cos((i.uv.x + offset.r + 1.5) * (timeScale) * 1.5); // Increased frequency
                float val21 = amplitudeScale * 0.8 * cos((i.uv.x - offset.g * 2.4) * timeScale);
                float val31 = amplitudeScale * 1.2 * sin((i.uv.y + offset.r * 3.6) * timeScale * 0.8) - amplitudeScale * 1.2 * sin((i.uv.y * 0.6 - offset.g) * timeScale * 1.2);

                // Combine the wave values and apply to color channels with some variation
                col.r = clamp(col.r + (val1 * 0.8 + val3 * 0.5 + val21 * 1.0 + val11 * 0.3 + over) * 0.76, 0, 1);
                col.g = clamp(col.g + (val2 * 1.0 + val1 * 0.3 + val31 * 0.7 + val21 * 0.6 - over) * 0.76, 0, 1);
                col.b = clamp(col.b + (val3 * 0.7 + val2 * 0.6 + val11 * 0.8 + val31 * 0.5 + over) * 0.76, 0, 1);
                col.a = clamp(col.a + over * val1 * 0.07 + over * val21 * 0.015, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
