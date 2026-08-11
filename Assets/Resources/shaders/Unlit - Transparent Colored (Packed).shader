Shader "Unlit/Transparent Colored (Packed)" 
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
				fixed4 mask = tex2D(_MainTex, i.uv);
				fixed4 par1 = clamp(ceil(i.color - 0.5), 0.0, 1.0);
				fixed4 color = clamp((par1 * 0.51 - i.color) / -0.49, 0.0, 1.0);
				fixed4 par2 = mask * par1;
				color.w = color.w * (par2.x + par2.y + par2.z + par2.w);
				return color;
			}
			ENDHLSL
		}
	}
}