Shader "Custom/Distorted/Multiply" 
{
	Properties 
	{
 		_MainTex ("Base (RGB)", 2D) = "white" {}
 		_Distort ("Distort", Vector) = (0,0,0,0)
 		_MainColor ("Color (RGBC)", Color) = (1,1,1,0)
	}

	SubShader
	{
		Tags { "QUEUE"="Transparent" }
		LOD 200
		Pass
		{
			Tags { "QUEUE"="Transparent" }
  			Fog { Mode Off }
  			Blend DstColor Zero
  			Offset -1, -1
			
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Distort;
			fixed4 _MainColor;

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata a)
			{
				v2f o;
				o.uv = a.uv * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 pos = mul(UNITY_MATRIX_MVP, a.pos);
				float w = pos.w * pos.w;
				pos.x += w * _Distort.x;
				pos.y += w * _Distort.y;
				o.vertex = pos;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDHLSL
		}
	}
}