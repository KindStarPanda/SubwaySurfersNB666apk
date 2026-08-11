Shader "Unlit/Transparent Colored (AlphaClip)" 
{
	Properties 
	{
 		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
	}
	
    SubShader
	{
		LOD 200
 		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
		Pass
		{
			Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  			ZWrite Off
  			Cull Off
  			Fog { Mode Off }
  			Blend SrcAlpha OneMinusSrcAlpha
  			ColorMask RGB
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
				fixed2 uv_st : TEXCOORD1;
                fixed4 color : TEXCOORD2;
			};

			v2f vert(appdata a)
			{
				v2f o;
				o.uv = a.uv;
				o.uv_st = a.pos.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.color = a.color;
				o.vertex = mul(UNITY_MATRIX_MVP, a.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv) * i.color;
				float2 uv_st_abs = abs(i.uv_st);
				color.w = step(0.0, 1.0 - max(uv_st_abs.x, uv_st_abs.y)) * color.w;
				return color;
			}
			ENDHLSL
		}
	}
}