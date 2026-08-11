Shader "Custom/Additive Constant Color" 
{
	Properties 
	{
 		_MainTex ("Main Texture", 2D) = "white" {}
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
  			Blend One One

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
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
				o.vertex = mul(UNITY_MATRIX_MVP, a.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * _MainColor;
			}
			ENDHLSL
		}
	}
}