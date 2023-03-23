Shader "Custom/UIBlur" {
	Properties{
		_BlurX("BlurX", Range(0, 0.01)) = 0.001
		_BlurY("BlurY", Range(0, 0.01)) = 0.001
		[HideInInspector][NoScaleOffset] _MainTex("Texture (RGB)", 2D) = "white" {}
	}
		Category{
		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		SubShader
		{
			LOD 100
			ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv: TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float _BlurX;
				float _BlurY;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;

					return o;
				}

				half4 frag(v2f i) : SV_Target
				{
					fixed4 sum = fixed4(0,0,0,0);
					fixed2 texelSize = fixed2(_BlurX, _BlurY);

					#define GET_PIXEL_AT(x, y) tex2D(_MainTex, i.uv + float2(x, y) * texelSize);

					sum += GET_PIXEL_AT(+0.000, +1.000);
					sum += GET_PIXEL_AT(+1.000, +0.400);
					sum += GET_PIXEL_AT(-1.000, +0.400);
					sum += GET_PIXEL_AT(+0.600, -1.000);
					sum += GET_PIXEL_AT(-0.600, -1.000);
					return sum * 0.200;
				}
				ENDCG
			}
		}
	}
}
