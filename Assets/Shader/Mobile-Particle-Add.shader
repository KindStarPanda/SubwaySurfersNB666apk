Shader "Mobile/Particles/Additive" 
{
    Properties 
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
    }

    SubShader
	{
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
		LOD 200
		Pass
		{
			Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  			BindChannels {
            Bind "vertex", Vertex
            Bind "color", Color
            Bind "texcoord", TexCoord
            }
            ZWrite Off
            Cull Off
            Fog {
            Color (0,0,0,0)
            }
            Blend SrcAlpha One

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