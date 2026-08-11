Shader "Custom/Distorted/Projector Multiply" 
{
	Properties 
	{
 		_ShadowTex ("Cookie", 2D) = "gray" {}
 		_Falloff ("Falloff distance", Float) = 0.01
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent-1" }
		LOD 200

		Pass
		{
			Tags { "RenderType"="Transparent-1" }
  			ZWrite Off
  			Fog 
			{
   				Color (1,1,1,1)
  			}
  			Blend DstColor Zero
  			AlphaTest Greater 0
  			ColorMask RGB
  			Offset -1, -1

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _ShadowTex;
			float4 _ShadowTex_ST;
			float _Falloff;
			float4x4 _Projector;
			float4 _Distort;

			struct appdata
			{
				float4 pos : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : TEXCOORD0;
				float4 uv : TEXCOORD1;
			};

			v2f vert(appdata a)
			{
				v2f o;
				float4 pos = mul(UNITY_MATRIX_MVP, a.pos);
				float w = pos.w * pos.w;
				pos.x += w * _Distort.x;
				pos.y += w * _Distort.y;
				o.vertex = pos;
				o.uv = mul(_Projector, a.pos);
				o.color = (fixed4)step(o.uv.z, 0.0) + clamp(o.uv.z * _Falloff, 0.0, 1.0);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = lerp(tex2Dproj(_ShadowTex, i.uv), (fixed4)1.0, i.color.x);
				return color;
			}
			ENDHLSL
		}
	}
}