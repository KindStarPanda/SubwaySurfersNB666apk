Shader "Custom/Distorted/Additive (falloff)" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
 		_Distort ("Distort", Vector) = (0,0,0,0)
 		_MainColor ("Color (RGBC)", Color) = (1,1,1,0)
 		_Falloff ("Falloff Distance", Float) = 200
	}
	
	SubShader
	{
		Tags { "QUEUE"="Transparent" }
		Pass
		{		
			Tags { "QUEUE"="Transparent" }
			ZWrite Off
  			Fog { Mode Off }
  			Blend One One

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Distort;
			fixed4 _MainColor;
			float _Falloff;

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
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
				float4 pos = mul(UNITY_MATRIX_MVP, a.pos);
				float w = pos.w * pos.w;
				pos.x += w * _Distort.x;
				pos.y += w * _Distort.y;
				o.vertex = pos;
				float par1 = pos.z / _Falloff;
				float par2 = 1.0 - par1 * par1;
				o.color = fixed4(par1, (fixed3)0.0);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * _MainColor * i.color.x;
			}
			ENDHLSL
		}
	}
}