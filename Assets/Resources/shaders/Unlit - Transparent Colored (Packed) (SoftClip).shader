Shader "Unlit/Transparent Colored (Packed) (SoftClip)" 
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
			float2 _ClipSharpness;

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                fixed4 color : TEXCOORD0;
				fixed2 uv : TEXCOORD1;
				fixed2 uv_st : TEXCOORD2;
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
				float2 par1 = ((float2)1.0 - abs(i.uv_st)) * _ClipSharpness;
				fixed4 color = tex2D(_MainTex, i.uv) * i.color;
				color.w = color.w * clamp(min(par1.x, par1.y), 0.0, 1.0);
				return color;
			}
			ENDHLSL
		}
	}
}