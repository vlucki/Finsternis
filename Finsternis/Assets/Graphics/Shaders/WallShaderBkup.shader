Shader "Custom/WallShader"
	{
		Properties
		{
			_BaseColor("Color", Color) = (1,1,1,1)
			_ShadowColor("Shadow Color", Color) = (0,0,0,1)
			_ShadowIntensity("Shadow Intensity", Range(0, 1)) = 0.6
		}
			SubShader
		{
			Tags{ "Queue" = "Transparent" }
			Pass
			{
				Tags{ "LightMode" = "ForwardBase" }
				Cull Back
				Blend SrcAlpha OneMinusSrcAlpha
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				uniform fixed4 _ShadowColor;
				uniform fixed4 _BaseColor;
				uniform float _ShadowIntensity;

				struct v2f
				{
					fixed4 color : COLOR;
					float4 pos : SV_POSITION;
					LIGHTING_COORDS(0,1)
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.color = _BaseColor;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					TRANSFER_VERTEX_TO_FRAGMENT(o);

					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					float attenuation = LIGHT_ATTENUATION(i);
					return fixed4(1,1,1,(1 - attenuation)*_ShadowIntensity) * _ShadowColor * _BaseColor;
				}

				ENDCG
			}
		}
			Fallback "VertexLit"
	}

