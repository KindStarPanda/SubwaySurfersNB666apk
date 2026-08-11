Shader "Custom/Sky" 
{
	Properties 
	{
		_MainTex("MainTex", 2D) = "white"{}
	}
	
	SubShader
	{
		Tags { "QUEUE"="Background" "RenderType"="Opaque" }
		LOD 200
		Pass
		{
			Tags { "QUEUE"="Background" "RenderType"="Opaque" }
  			ZWrite Off
  			Cull Front
  			Fog { Mode Off }

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
				return tex2D(_MainTex, i.uv);
			}
			ENDHLSL
		}
	}
	Fallback "Diffuse"
}