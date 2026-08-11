Shader "Unlit/Masked Colored" 
{
	Properties 
	{
 		_MainTex ("Base (RGB) Mask (A)", 2D) = "white" {}
 		_Color ("Tint Color", Color) = (1,1,1,1)
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
  			ColorMask RGB

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

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
				fixed4 color = tex2D(_MainTex, i.uv) * i.color;
				return fixed4(lerp(color, color * _Color, color.w).xyz, color.w);
			}
			ENDHLSL
		}
	}
}