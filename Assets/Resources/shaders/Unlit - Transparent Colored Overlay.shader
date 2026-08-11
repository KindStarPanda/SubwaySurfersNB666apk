Shader "Unlit/Transparent Colored Overlay" 
{
    Properties 
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader
	{
		LOD 100
        Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
		Pass
		{
			Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0.01
            ColorMask RGB
            ColorMaterial AmbientAndDiffuse
            Offset -1, -1

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
                fixed4 color : TEXCOORD1;
			};

			v2f vert(appdata a)
			{
				v2f o;
				o.uv = a.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.color = a.color;
				o.vertex = mul(UNITY_MATRIX_MVP, a.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * i.color;
			}
			ENDHLSL
		}
	}
}